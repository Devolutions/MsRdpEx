#include "MsRdpEx.h"

#include <MsRdpEx/Pcap.h>

#include <MsRdpEx/Sspi.h>

#include <intrin.h>

#include <detours.h>

static MsRdpEx_PcapFile* g_PcapFile = NULL;
static bool g_PcapEnabled = true;
static char g_PcapFilePath[MSRDPEX_MAX_PATH] = { 0 };

static MsRdpEx_PcapFile* MsRdpEx_GetPcapFile()
{
	if (g_PcapFile)
		return g_PcapFile;

	if (g_PcapFilePath[0] == '\0') {
		const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
		sprintf_s(g_PcapFilePath, MSRDPEX_MAX_PATH, "%s\\MsRdpEx.pcap", appDataPath);
	}

	g_PcapFile = MsRdpEx_PcapFile_Open(g_PcapFilePath, true);

	return g_PcapFile;
}

static HMODULE g_hSspiCli = NULL;
static HMODULE g_hSecur32 = NULL;

static ENUMERATE_SECURITY_PACKAGES_FN_W Real_EnumerateSecurityPackagesW = NULL;
static QUERY_CREDENTIALS_ATTRIBUTES_FN_W Real_QueryCredentialsAttributesW = NULL;
static ACQUIRE_CREDENTIALS_HANDLE_FN_W Real_AcquireCredentialsHandleW = NULL;
static FREE_CREDENTIALS_HANDLE_FN Real_FreeCredentialsHandle = NULL;

static INITIALIZE_SECURITY_CONTEXT_FN_W Real_InitializeSecurityContextW = NULL;
static ACCEPT_SECURITY_CONTEXT_FN Real_AcceptSecurityContext = NULL;
static COMPLETE_AUTH_TOKEN_FN Real_CompleteAuthToken = NULL;
static DELETE_SECURITY_CONTEXT_FN Real_DeleteSecurityContext = NULL;
static APPLY_CONTROL_TOKEN_FN Real_ApplyControlToken = NULL;
static QUERY_CONTEXT_ATTRIBUTES_FN_W Real_QueryContextAttributesW = NULL;
static IMPERSONATE_SECURITY_CONTEXT_FN Real_ImpersonateSecurityContext = NULL;
static REVERT_SECURITY_CONTEXT_FN Real_RevertSecurityContext = NULL;
static MAKE_SIGNATURE_FN Real_MakeSignature = NULL;
static VERIFY_SIGNATURE_FN Real_VerifySignature = NULL;
static FREE_CONTEXT_BUFFER_FN Real_FreeContextBuffer = NULL;
static QUERY_SECURITY_PACKAGE_INFO_FN_W Real_QuerySecurityPackageInfoW = NULL;

static EXPORT_SECURITY_CONTEXT_FN Real_ExportSecurityContext = NULL;
static IMPORT_SECURITY_CONTEXT_FN_W Real_ImportSecurityContextW = NULL;
static ADD_CREDENTIALS_FN_W Real_AddCredentialsW = NULL;

static QUERY_SECURITY_CONTEXT_TOKEN_FN Real_QuerySecurityContextToken = NULL;
static DECRYPT_MESSAGE_FN Real_DecryptMessage = NULL;
static ENCRYPT_MESSAGE_FN Real_EncryptMessage = NULL;
static SET_CONTEXT_ATTRIBUTES_FN_W Real_SetContextAttributesW = NULL;

static SET_CREDENTIALS_ATTRIBUTES_FN_W Real_SetCredentialsAttributesW = NULL;

// available in Windows 10 and later
static QUERY_CONTEXT_ATTRIBUTES_EX_FN_W Real_QueryContextAttributesExW = NULL;
static QUERY_CREDENTIALS_ATTRIBUTES_EX_FN_W Real_QueryCredentialsAttributesExW = NULL;

static const char* MsRdpEx_GetSecurityStatusString(SECURITY_STATUS status);

static SECURITY_STATUS SEC_ENTRY sspi_EnumerateSecurityPackagesW(ULONG* pcPackages,
	PSecPkgInfoW* ppPackageInfo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_EnumerateSecurityPackagesW");

	status = Real_EnumerateSecurityPackagesW(pcPackages, ppPackageInfo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryCredentialsAttributesW(PCredHandle phCredential,
	ULONG ulAttribute, void* pBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_QueryCredentialsAttributesW: phCredential: %p ulAttribute: %d",
		phCredential, ulAttribute);

	status = Real_QueryCredentialsAttributesW(phCredential, ulAttribute, pBuffer);

	return status;
}

typedef struct _SEC_WINNT_AUTH_IDENTITY_OPAQUE {
	uint32_t version;
	uint32_t padding1;
	uint16_t domainSize1;
	uint16_t domainSize2;
	uint32_t padding2;
	uint32_t domainOffset;
	uint32_t padding4;
	uint16_t userSize1;
	uint16_t userSize2;
	uint32_t padding5;
	uint32_t userOffset;
	uint32_t padding7;
	uint32_t padding8;
	uint32_t padding9;
	uint32_t padding10;
	uint32_t padding11;
	uint32_t padding12;
	uint32_t padding13;
} SEC_WINNT_AUTH_IDENTITY_OPAQUE;

static bool sspi_DumpCredSspAuthData(void* pAuthData)
{
	CREDSSP_CRED* pCred = (CREDSSP_CRED*) pAuthData;

	MsRdpEx_Log("CredSSP submit type: %d schannel: %d spnego: %d",
		pCred->Type, pCred->pSchannelCred ? 1 : 0, pCred->pSpnegoCred ? 1 : 0);

	if (pCred->pSpnegoCred) {
		SECURITY_STATUS secStatus = SEC_E_OK;
		bool bUnprotectCredentials = false;
		DWORD dwUnpackFlags = 0;
		char* userA = NULL;
		char* domainA = NULL;
		char* passwordA = NULL;
		WCHAR szUserName[512 + 1];
		WCHAR szDomainName[512 + 1];
		WCHAR szPassword[512 + 1];
		DWORD cchMaxUserName = sizeof(szUserName) / sizeof(WCHAR);
		DWORD cchMaxDomainName = sizeof(szDomainName) / sizeof(WCHAR);
		DWORD cchMaxPassword = sizeof(szPassword) / sizeof(WCHAR);
		DWORD cchUserName = cchMaxUserName;
		DWORD cchDomainName = cchMaxDomainName;
		DWORD cchPassword = cchMaxPassword;
		DWORD cbAuthBuffer = LocalSize((HLOCAL) pCred->pSpnegoCred);

		if (bUnprotectCredentials) {
			dwUnpackFlags |= CRED_PACK_PROTECTED_CREDENTIALS;
		}

		SEC_WINNT_AUTH_IDENTITY_OPAQUE* pAuthOpaque = (SEC_WINNT_AUTH_IDENTITY_OPAQUE*) pCred->pSpnegoCred;

		if (1) {
			char* pUserA = NULL;
			char* pDomainA = NULL;
			uint16_t userLength = pAuthOpaque->userSize1 / 2;
			uint16_t domainLength = pAuthOpaque->domainSize1 / 2;
			WCHAR* pUserW = (WCHAR*)&((uint8_t*)pAuthOpaque)[pAuthOpaque->userOffset];
			WCHAR* pDomainW = (WCHAR*)&((uint8_t*)pAuthOpaque)[pAuthOpaque->domainOffset];

			if (userLength)
				MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pUserW, userLength, &pUserA, 0, NULL, NULL);

			if (domainLength)
				MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pDomainW, domainLength, &pDomainA, 0, NULL, NULL);

			MsRdpEx_Log("AUTH_OPAQUE: User: \"%s\" Domain: \"%s\"", pUserA, pDomainA);

			free(pUserA);
			free(pDomainA);
		}

		BOOL success = CredUnPackAuthenticationBufferW(dwUnpackFlags,
			pCred->pSpnegoCred, cbAuthBuffer, szUserName, &cchUserName,
			szDomainName, &cchDomainName, szPassword, &cchPassword);

		if (cchUserName == cchMaxUserName)
			cchUserName = 0;

		if (cchDomainName == cchMaxDomainName)
			cchDomainName = 0;

		if (cchPassword == cchMaxPassword)
			cchPassword = 0;

		if (!bUnprotectCredentials) {
			cchPassword = 0;
		}

		if (success) {
			if (cchUserName)
				MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, szUserName, cchUserName, &userA, 0, NULL, NULL);

			if (cchDomainName)
				MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, szDomainName, cchDomainName, &domainA, 0, NULL, NULL);

			if (cchPassword)
				MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, szPassword, cchPassword, &passwordA, 0, NULL, NULL);

			MsRdpEx_Log("CredUnPackAuthenticationBufferW: Size: %d User: %s (%d) Domain: %s (%d) Password: %s (%d)",
				cbAuthBuffer,
				userA ? userA : "", cchUserName,
				domainA ? domainA : "", cchDomainName,
				passwordA ? passwordA : "", cchPassword);

			free(userA);
			free(domainA);
			free(passwordA);
		}
	}

	return true;
}

static bool sspi_SetKdcProxySettings(PCredHandle phCredential, const char* proxyServer)
{
	SECURITY_STATUS status;
	WCHAR* pProxyServerW = NULL;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, proxyServer, -1, &pProxyServerW, 0) < 1)
		return false;

	DWORD cchProxyServer = wcslen(pProxyServerW);
	SecPkgCredentials_KdcProxySettingsW* pKdcProxySettings = NULL;
	DWORD cbKdcProxySettings = sizeof(SecPkgCredentials_KdcProxySettingsW);
	DWORD cbProxyServer = cchProxyServer * sizeof(WCHAR);
	unsigned long cbBuffer = cbKdcProxySettings + cbProxyServer;
	uint8_t* pBuffer = (uint8_t*) calloc(1, cbBuffer);

	if (pBuffer) {
		pKdcProxySettings = (SecPkgCredentials_KdcProxySettingsW*) pBuffer;

		pKdcProxySettings->Version = KDC_PROXY_SETTINGS_V1;
		pKdcProxySettings->Flags = KDC_PROXY_SETTINGS_FLAGS_FORCEPROXY;
		pKdcProxySettings->ProxyServerOffset = cbKdcProxySettings;
		pKdcProxySettings->ProxyServerLength = cbProxyServer / sizeof(WCHAR);
		pKdcProxySettings->ClientTlsCredOffset = 0;
		pKdcProxySettings->ClientTlsCredLength = 0;
		memcpy(&pBuffer[pKdcProxySettings->ProxyServerOffset], pProxyServerW, cbProxyServer);

		MsRdpEx_Log("Injecting KdcProxySettings: %s", proxyServer);
		status = SetCredentialsAttributesW(phCredential, SECPKG_CRED_ATTR_KDC_PROXY_SETTINGS, (void*) pBuffer, cbBuffer);
	}

	free(pProxyServerW);
	free(pBuffer);

	return true;
}

static SECURITY_STATUS SEC_ENTRY sspi_AcquireCredentialsHandleW(
	SEC_WCHAR* pszPrincipal, SEC_WCHAR* pszPackage, ULONG fCredentialUse, void* pvLogonID,
	void* pAuthData, SEC_GET_KEY_FN pGetKeyFn, void* pvGetKeyArgument, PCredHandle phCredential,
	PTimeStamp ptsExpiry)
{
	SECURITY_STATUS status;
	char* pszPrincipalA = NULL;
	char* pszPackageA = NULL;

	if (pszPrincipal)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszPrincipal, -1, &pszPrincipalA, 0, NULL, NULL);

	if (pszPackage)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszPackage, -1, &pszPackageA, 0, NULL, NULL);

#if 0
	if (pAuthData && MsRdpEx_StringIEquals(pszPackageA, "CREDSSP")) {
		sspi_DumpCredSspAuthData(pAuthData);
	}
#endif

	status = Real_AcquireCredentialsHandleW(pszPrincipal, pszPackage, fCredentialUse, pvLogonID,
		pAuthData, pGetKeyFn, pvGetKeyArgument,
		phCredential, ptsExpiry);

	MsRdpEx_Log("sspi_AcquireCredentialsHandleW(principal=\"%s\", package=\"%s\", phCredential=%p,%p)",
		pszPrincipalA ? pszPrincipalA : "",
		pszPackageA ? pszPackageA : "",
		(void*)phCredential->dwLower, (void*) phCredential->dwUpper);

	char* proxyServer = NULL;

	if (proxyServer && (MsRdpEx_StringIEquals(pszPackageA, "CREDSSP") || MsRdpEx_StringIEquals(pszPackageA, "TSSSP"))) {
		sspi_SetKdcProxySettings(phCredential, proxyServer);
	}

	free(pszPrincipalA);
	free(pszPackageA);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_FreeCredentialsHandle(PCredHandle phCredential)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_FreeCredentialsHandle: phCredential: %p,%p",
		(void*)phCredential->dwLower, (void*)phCredential->dwUpper);

	status = Real_FreeCredentialsHandle(phCredential);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_InitializeSecurityContextW(
	PCredHandle phCredential, PCtxtHandle phContext, SEC_WCHAR* pszTargetName, ULONG fContextReq,
	ULONG Reserved1, ULONG TargetDataRep, PSecBufferDesc pInput, ULONG Reserved2,
	PCtxtHandle phNewContext, PSecBufferDesc pOutput, PULONG pfContextAttr, PTimeStamp ptsExpiry)
{
	SECURITY_STATUS status;
	unsigned long iBuffer = 0;
	char* pszTargetNameA = NULL;
	PSecBuffer pSecBuffer = NULL;

	if (pszTargetName)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszTargetName, -1, &pszTargetNameA, 0, NULL, NULL);

	MsRdpEx_Log("sspi_InitializeSecurityContextW: pszTargetName: %s fContextReq: 0x%08X TargetDataRep: 0x%08X",
		pszTargetNameA ? pszTargetNameA : "", fContextReq, TargetDataRep);

	if (pInput) {
		for (iBuffer = 0; iBuffer < pInput->cBuffers; iBuffer++) {
			pSecBuffer = &pInput->pBuffers[iBuffer];

			if ((pSecBuffer->cbBuffer < 1) || (!pSecBuffer->pvBuffer)) {
				continue;
			}

			MsRdpEx_Log("InputBuffer[%d](type:%d length:%d):", iBuffer, pSecBuffer->BufferType, pSecBuffer->cbBuffer);
			MsRdpEx_LogHexDump((uint8_t*)pSecBuffer->pvBuffer, (size_t)pSecBuffer->cbBuffer);
		}
	}

	status = Real_InitializeSecurityContextW(
		phCredential, phContext, pszTargetName, fContextReq, Reserved1, TargetDataRep, pInput,
		Reserved2, phNewContext, pOutput, pfContextAttr, ptsExpiry);

	if (pOutput) {
		for (iBuffer = 0; iBuffer < pOutput->cBuffers; iBuffer++) {
			pSecBuffer = &pOutput->pBuffers[iBuffer];

			if ((pSecBuffer->cbBuffer < 1) || (!pSecBuffer->pvBuffer)) {
				continue;
			}

			MsRdpEx_Log("OutputBuffer[%d](type:%d length:%d):", iBuffer, pSecBuffer->BufferType, pSecBuffer->cbBuffer);
			MsRdpEx_LogHexDump((uint8_t*)pSecBuffer->pvBuffer, (size_t)pSecBuffer->cbBuffer);
		}
	}

	free(pszTargetNameA);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_AcceptSecurityContext(PCredHandle phCredential,
	PCtxtHandle phContext, PSecBufferDesc pInput,
	ULONG fContextReq, ULONG TargetDataRep,
	PCtxtHandle phNewContext,
	PSecBufferDesc pOutput, PULONG pfContextAttr,
	PTimeStamp ptsTimeStamp)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_AcceptSecurityContext");

	status = Real_AcceptSecurityContext(phCredential,
		phContext, pInput, fContextReq, TargetDataRep,
		phNewContext, pOutput, pfContextAttr, ptsTimeStamp);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_CompleteAuthToken(PCtxtHandle phContext, PSecBufferDesc pToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_CompleteAuthToken");

	status = Real_CompleteAuthToken(phContext, pToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_DeleteSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_DeleteSecurityContext");

	status = Real_DeleteSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ApplyControlToken(PCtxtHandle phContext, PSecBufferDesc pInput)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_ApplyControlToken");

	status = Real_ApplyControlToken(phContext, pInput);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryContextAttributesW(PCtxtHandle phContext, ULONG ulAttribute,
	void* pBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_QueryContextAttributesW: %d", (int) ulAttribute);

	status = Real_QueryContextAttributesW(phContext, ulAttribute, pBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ImpersonateSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_ImpersonateSecurityContext");

	status = Real_ImpersonateSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_RevertSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_RevertSecurityContext");

	status = Real_RevertSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_MakeSignature(PCtxtHandle phContext, ULONG fQOP,
	PSecBufferDesc pMessage, ULONG MessageSeqNo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_MakeSignature");

	status = Real_MakeSignature(phContext, fQOP, pMessage, MessageSeqNo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_VerifySignature(PCtxtHandle phContext, PSecBufferDesc pMessage,
	ULONG MessageSeqNo, PULONG pfQOP)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_VerifySignature");

	status = Real_VerifySignature(phContext, pMessage, MessageSeqNo, pfQOP);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_FreeContextBuffer(void* pvContextBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_FreeContextBuffer");

	status = Real_FreeContextBuffer(pvContextBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QuerySecurityPackageInfoW(SEC_WCHAR* pszPackageName,
	PSecPkgInfoW* ppPackageInfo)
{
	SECURITY_STATUS status;
	char* pszPackageNameA = NULL;

	if (pszPackageName)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszPackageName, -1, &pszPackageNameA, 0, NULL, NULL);

	MsRdpEx_Log("sspi_QuerySecurityPackageInfoW: %s",
		pszPackageNameA ? pszPackageNameA : "");

	status = Real_QuerySecurityPackageInfoW(pszPackageName, ppPackageInfo);

	free(pszPackageNameA);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ExportSecurityContext(PCtxtHandle phContext, ULONG fFlags,
	PSecBuffer pPackedContext, HANDLE* pToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_ExportSecurityContext");

	status = Real_ExportSecurityContext(phContext, fFlags, pPackedContext, pToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ImportSecurityContextW(SEC_WCHAR* pszPackage,
	PSecBuffer pPackedContext, HANDLE pToken, PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_ImportSecurityContextW");

	status = Real_ImportSecurityContextW(pszPackage, pPackedContext, pToken, phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_AddCredentialsW(PCredHandle hCredentials, LPWSTR pszPrincipal, LPWSTR pszPackage,
	unsigned long fCredentialUse, void* pAuthData, SEC_GET_KEY_FN pGetKeyFn, void* pvGetKeyArgument, PTimeStamp ptsExpiry)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_AddCredentialsW");

	status = Real_AddCredentialsW(hCredentials, pszPrincipal, pszPackage,
		fCredentialUse, pAuthData, pGetKeyFn, pvGetKeyArgument, ptsExpiry);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QuerySecurityContextToken(PCtxtHandle phContext, HANDLE* phToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_QuerySecurityContextToken");

	status = Real_QuerySecurityContextToken(phContext, phToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_EncryptMessage(PCtxtHandle phContext, ULONG fQOP,
	PSecBufferDesc pMessage, ULONG MessageSeqNo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_EncryptMessage phContext: %p seqNo: %d cbBuffers: %d ReturnAddress: %p",
		phContext, MessageSeqNo, pMessage->cBuffers, _ReturnAddress());

#if 0
	if (!MsRdpEx_IsAddressInModule(_ReturnAddress(), L"mstscax.dll"))
	{
		return Real_EncryptMessage(phContext, fQOP, pMessage, MessageSeqNo);
	}
#endif

	for (unsigned long iBuffer = 0; iBuffer < pMessage->cBuffers; iBuffer++) {
		PSecBuffer pSecBuffer = &pMessage->pBuffers[iBuffer];
		unsigned long BufferType = pSecBuffer->BufferType & ~(SECBUFFER_ATTRMASK);

		if ((pSecBuffer->cbBuffer < 1) || (!pSecBuffer->pvBuffer)) {
			continue;
		}

		if (BufferType != SECBUFFER_DATA) {
			continue;
		}

		MsRdpEx_PcapFile* pcap = MsRdpEx_GetPcapFile();

		if (pcap) {
			MsRdpEx_PcapFile_Lock(pcap);
			MsRdpEx_PcapFile_WritePacket(pcap,
				(const uint8_t*) pSecBuffer->pvBuffer,
				pSecBuffer->cbBuffer, PCAP_PACKET_FLAG_OUTBOUND);
			MsRdpEx_PcapFile_Unlock(pcap);
		}

		MsRdpEx_Log("SecBuffer[%d](type:%d length:%d):", iBuffer, BufferType, pSecBuffer->cbBuffer);
		MsRdpEx_LogHexDump((uint8_t*)pSecBuffer->pvBuffer, (size_t)pSecBuffer->cbBuffer);
	}

	status = Real_EncryptMessage(phContext, fQOP, pMessage, MessageSeqNo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_DecryptMessage(PCtxtHandle phContext, PSecBufferDesc pMessage,
	ULONG MessageSeqNo, PULONG pfQOP)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_DecryptMessage phContext: %p seqNo: %d ReturnAddress: %p",
		phContext, MessageSeqNo, _ReturnAddress());

	status = Real_DecryptMessage(phContext, pMessage, MessageSeqNo, pfQOP);

#if 0
	if (!MsRdpEx_IsAddressInModule(_ReturnAddress(), L"mstscax.dll"))
	{
		return status;
	}
#endif

	for (unsigned long iBuffer = 0; iBuffer < pMessage->cBuffers; iBuffer++) {
		PSecBuffer pSecBuffer = &pMessage->pBuffers[iBuffer];
		unsigned long BufferType = pSecBuffer->BufferType & ~(SECBUFFER_ATTRMASK);

		if ((pSecBuffer->cbBuffer < 1) || (!pSecBuffer->pvBuffer)) {
			continue;
		}

		if (BufferType != SECBUFFER_DATA) {
			continue;
		}

		MsRdpEx_PcapFile* pcap = MsRdpEx_GetPcapFile();

		if (pcap) {
			MsRdpEx_PcapFile_Lock(pcap);
			MsRdpEx_PcapFile_WritePacket(pcap,
				(const uint8_t*) pSecBuffer->pvBuffer,
				pSecBuffer->cbBuffer, PCAP_PACKET_FLAG_INBOUND);
			MsRdpEx_PcapFile_Unlock(pcap);
		}

		MsRdpEx_Log("SecBuffer[%d](type:%d length:%d):", iBuffer, BufferType, pSecBuffer->cbBuffer);
		MsRdpEx_LogHexDump((uint8_t*)pSecBuffer->pvBuffer, (size_t)pSecBuffer->cbBuffer);
	}

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_SetContextAttributesW(PCtxtHandle phContext, ULONG ulAttribute,
	void* pBuffer, ULONG cbBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_SetContextAttributesW");

	status = Real_SetContextAttributesW(phContext, ulAttribute, pBuffer, cbBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_SetCredentialsAttributesW(PCredHandle phCredential,
	unsigned long ulAttribute, void* pBuffer, unsigned long cbBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_SetCredentialsAttributesW: ulAttribute: %d cbBuffer: %d phCredential: %p,%p",
		ulAttribute, cbBuffer, (void*)phCredential->dwLower, (void*)phCredential->dwUpper);

	if (ulAttribute == SECPKG_CRED_ATTR_KDC_PROXY_SETTINGS) {
		char* pProxyServerA = NULL;
		WCHAR* proxyServerW = NULL;
		PSecPkgCredentials_KdcProxySettingsW pKdcProxySettings = (PSecPkgCredentials_KdcProxySettingsW) pBuffer;
		DWORD cchProxyServer = pKdcProxySettings->ProxyServerLength;
		WCHAR* pProxyServerW = (WCHAR*)&((uint8_t*)pBuffer)[pKdcProxySettings->ProxyServerOffset];

		if (cchProxyServer) {
			MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pProxyServerW, cchProxyServer, &pProxyServerA, 0, NULL, NULL);
		}

		MsRdpEx_Log("KdcProxySettings: Version: %d Flags: 0x%08X", pKdcProxySettings->Version, pKdcProxySettings->Flags);

		if (pProxyServerA) {
			MsRdpEx_Log("ProxyServer: %s", pProxyServerA);
		}
	}

	status = Real_SetCredentialsAttributesW(phCredential, ulAttribute, pBuffer, cbBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryContextAttributesExW(PCtxtHandle phContext,
	unsigned long ulAttribute, void* pBuffer, unsigned long cbBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_QueryContextAttributesExW: ulAttribute: %d cbBuffer: %d", ulAttribute, cbBuffer);

	status = Real_QueryContextAttributesExW(phContext, ulAttribute, pBuffer, cbBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryCredentialsAttributesExW(PCredHandle phCredential,
	unsigned long ulAttribute, void* pBuffer, unsigned long cbBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("sspi_QueryCredentialsAttributesExW: ulAttribute: %d cbBuffer: %d", ulAttribute, cbBuffer);

	status = Real_QueryCredentialsAttributesExW(phCredential, ulAttribute, pBuffer, cbBuffer);

	return status;
}

#define MSRDPEX_DETOUR_ATTACH(_realFn, _hookFn) \
	if (_realFn) DetourAttach((PVOID*)(&_realFn), _hookFn);

#define MSRDPEX_DETOUR_DETACH(_realFn, _hookFn) \
	if (_realFn) DetourDetach((PVOID*)(&_realFn), _hookFn);

LONG MsRdpEx_AttachSspiHooks()
{
	g_hSspiCli = GetModuleHandleA("sspicli.dll");

	if (!g_hSspiCli)
		return -1;

	g_hSecur32 = GetModuleHandleA("secur32.dll");

	if (!g_hSecur32)
		return -1;

	Real_EnumerateSecurityPackagesW = (ENUMERATE_SECURITY_PACKAGES_FN_W) GetProcAddress(g_hSspiCli, "EnumerateSecurityPackagesW");
	Real_QueryCredentialsAttributesW = (QUERY_CREDENTIALS_ATTRIBUTES_FN) GetProcAddress(g_hSspiCli, "QueryCredentialsAttributesW");
	Real_AcquireCredentialsHandleW = (ACQUIRE_CREDENTIALS_HANDLE_FN_W) GetProcAddress(g_hSspiCli, "AcquireCredentialsHandleW");
	Real_FreeCredentialsHandle = (FREE_CREDENTIALS_HANDLE_FN) GetProcAddress(g_hSspiCli, "FreeCredentialsHandle");

	Real_InitializeSecurityContextW = (INITIALIZE_SECURITY_CONTEXT_FN_W) GetProcAddress(g_hSspiCli, "InitializeSecurityContextW");
	Real_AcceptSecurityContext = (ACCEPT_SECURITY_CONTEXT_FN) GetProcAddress(g_hSspiCli, "AcceptSecurityContext");
	Real_CompleteAuthToken = (COMPLETE_AUTH_TOKEN_FN) GetProcAddress(g_hSspiCli, "CompleteAuthToken");
	Real_DeleteSecurityContext = (DELETE_SECURITY_CONTEXT_FN) GetProcAddress(g_hSspiCli, "DeleteSecurityContext");
	Real_ApplyControlToken = (APPLY_CONTROL_TOKEN_FN) GetProcAddress(g_hSspiCli, "ApplyControlToken");
	Real_QueryContextAttributesW = (QUERY_CONTEXT_ATTRIBUTES_FN_W) GetProcAddress(g_hSspiCli, "QueryContextAttributesW");
	Real_ImpersonateSecurityContext = (IMPERSONATE_SECURITY_CONTEXT_FN) GetProcAddress(g_hSspiCli, "ImpersonateSecurityContext");
	Real_RevertSecurityContext = (REVERT_SECURITY_CONTEXT_FN) GetProcAddress(g_hSspiCli, "RevertSecurityContext");
	Real_MakeSignature = (MAKE_SIGNATURE_FN) GetProcAddress(g_hSspiCli, "MakeSignature");
	Real_VerifySignature = (VERIFY_SIGNATURE_FN) GetProcAddress(g_hSspiCli, "VerifySignature");
	Real_FreeContextBuffer = (FREE_CONTEXT_BUFFER_FN) GetProcAddress(g_hSspiCli, "FreeContextBuffer");
	Real_QuerySecurityPackageInfoW = (QUERY_SECURITY_PACKAGE_INFO_FN_W) GetProcAddress(g_hSspiCli, "QuerySecurityPackageInfoW");

	Real_ExportSecurityContext = (EXPORT_SECURITY_CONTEXT_FN) GetProcAddress(g_hSspiCli, "ExportSecurityContext");
	Real_ImportSecurityContextW = (IMPORT_SECURITY_CONTEXT_FN_W) GetProcAddress(g_hSspiCli, "ImportSecurityContextW");
	Real_AddCredentialsW = (ADD_CREDENTIALS_FN_W) GetProcAddress(g_hSspiCli, "AddCredentialsW");

	Real_QuerySecurityContextToken = (QUERY_SECURITY_CONTEXT_TOKEN_FN) GetProcAddress(g_hSspiCli, "QuerySecurityContextToken");
	Real_EncryptMessage = (ENCRYPT_MESSAGE_FN) GetProcAddress(g_hSspiCli, "EncryptMessage");
	Real_DecryptMessage = (DECRYPT_MESSAGE_FN) GetProcAddress(g_hSspiCli, "DecryptMessage");
	Real_SetContextAttributesW = (SET_CONTEXT_ATTRIBUTES_FN_W) GetProcAddress(g_hSspiCli, "SetContextAttributesW");

	Real_SetCredentialsAttributesW = (SET_CREDENTIALS_ATTRIBUTES_FN_W) GetProcAddress(g_hSecur32, "SetCredentialsAttributesW");

	Real_QueryContextAttributesExW = (QUERY_CONTEXT_ATTRIBUTES_EX_FN_W) GetProcAddress(g_hSspiCli, "QueryContextAttributesExW");
	Real_QueryCredentialsAttributesExW = (QUERY_CREDENTIALS_ATTRIBUTES_EX_FN_W) GetProcAddress(g_hSspiCli, "QueryCredentialsAttributesExW");

	MSRDPEX_DETOUR_ATTACH(Real_EnumerateSecurityPackagesW, sspi_EnumerateSecurityPackagesW);
	MSRDPEX_DETOUR_ATTACH(Real_QueryCredentialsAttributesW, sspi_QueryCredentialsAttributesW);
	MSRDPEX_DETOUR_ATTACH(Real_AcquireCredentialsHandleW, sspi_AcquireCredentialsHandleW);
	MSRDPEX_DETOUR_ATTACH(Real_FreeCredentialsHandle, sspi_FreeCredentialsHandle);

	MSRDPEX_DETOUR_ATTACH(Real_InitializeSecurityContextW, sspi_InitializeSecurityContextW);
	MSRDPEX_DETOUR_ATTACH(Real_AcceptSecurityContext, sspi_AcceptSecurityContext);
	MSRDPEX_DETOUR_ATTACH(Real_CompleteAuthToken, sspi_CompleteAuthToken);
	MSRDPEX_DETOUR_ATTACH(Real_DeleteSecurityContext, sspi_DeleteSecurityContext);
	MSRDPEX_DETOUR_ATTACH(Real_ApplyControlToken, sspi_ApplyControlToken);
	MSRDPEX_DETOUR_ATTACH(Real_QueryContextAttributesW, sspi_QueryContextAttributesW);
	MSRDPEX_DETOUR_ATTACH(Real_ImpersonateSecurityContext, sspi_ImpersonateSecurityContext);
	MSRDPEX_DETOUR_ATTACH(Real_RevertSecurityContext, sspi_RevertSecurityContext);
	MSRDPEX_DETOUR_ATTACH(Real_MakeSignature, sspi_MakeSignature);
	MSRDPEX_DETOUR_ATTACH(Real_VerifySignature, sspi_VerifySignature);
	MSRDPEX_DETOUR_ATTACH(Real_FreeContextBuffer, sspi_FreeContextBuffer);
	MSRDPEX_DETOUR_ATTACH(Real_QuerySecurityPackageInfoW, sspi_QuerySecurityPackageInfoW);

	MSRDPEX_DETOUR_ATTACH(Real_ExportSecurityContext, sspi_ExportSecurityContext);
	MSRDPEX_DETOUR_ATTACH(Real_ImportSecurityContextW, sspi_ImportSecurityContextW);
	MSRDPEX_DETOUR_ATTACH(Real_AddCredentialsW, sspi_AddCredentialsW);

	MSRDPEX_DETOUR_ATTACH(Real_QuerySecurityContextToken, sspi_QuerySecurityContextToken);
	MSRDPEX_DETOUR_ATTACH(Real_EncryptMessage, sspi_EncryptMessage);
	MSRDPEX_DETOUR_ATTACH(Real_DecryptMessage, sspi_DecryptMessage);
	MSRDPEX_DETOUR_ATTACH(Real_SetContextAttributesW, sspi_SetContextAttributesW);

	MSRDPEX_DETOUR_ATTACH(Real_SetCredentialsAttributesW, sspi_SetCredentialsAttributesW);

	MSRDPEX_DETOUR_ATTACH(Real_QueryContextAttributesExW, sspi_QueryContextAttributesExW);
	MSRDPEX_DETOUR_ATTACH(Real_QueryCredentialsAttributesExW, sspi_QueryCredentialsAttributesExW);

	return 0;
}

LONG MsRdpEx_DetachSspiHooks()
{
	MSRDPEX_DETOUR_DETACH(Real_EnumerateSecurityPackagesW, sspi_EnumerateSecurityPackagesW);
	MSRDPEX_DETOUR_DETACH(Real_QueryCredentialsAttributesW, sspi_QueryCredentialsAttributesW);
	MSRDPEX_DETOUR_DETACH(Real_AcquireCredentialsHandleW, sspi_AcquireCredentialsHandleW);
	MSRDPEX_DETOUR_DETACH(Real_FreeCredentialsHandle, sspi_FreeCredentialsHandle);

	MSRDPEX_DETOUR_DETACH(Real_InitializeSecurityContextW, sspi_InitializeSecurityContextW);
	MSRDPEX_DETOUR_DETACH(Real_AcceptSecurityContext, sspi_AcceptSecurityContext);
	MSRDPEX_DETOUR_DETACH(Real_CompleteAuthToken, sspi_CompleteAuthToken);
	MSRDPEX_DETOUR_DETACH(Real_DeleteSecurityContext, sspi_DeleteSecurityContext);
	MSRDPEX_DETOUR_DETACH(Real_ApplyControlToken, sspi_ApplyControlToken);
	MSRDPEX_DETOUR_DETACH(Real_QueryContextAttributesW, sspi_QueryContextAttributesW);
	MSRDPEX_DETOUR_DETACH(Real_ImpersonateSecurityContext, sspi_ImpersonateSecurityContext);
	MSRDPEX_DETOUR_DETACH(Real_RevertSecurityContext, sspi_RevertSecurityContext);
	MSRDPEX_DETOUR_DETACH(Real_MakeSignature, sspi_MakeSignature);
	MSRDPEX_DETOUR_DETACH(Real_VerifySignature, sspi_VerifySignature);
	MSRDPEX_DETOUR_DETACH(Real_FreeContextBuffer, sspi_FreeContextBuffer);
	MSRDPEX_DETOUR_DETACH(Real_QuerySecurityPackageInfoW, sspi_QuerySecurityPackageInfoW);

	MSRDPEX_DETOUR_DETACH(Real_ExportSecurityContext, sspi_ExportSecurityContext);
	MSRDPEX_DETOUR_DETACH(Real_ImportSecurityContextW, sspi_ImportSecurityContextW);
	MSRDPEX_DETOUR_DETACH(Real_AddCredentialsW, sspi_AddCredentialsW);

	MSRDPEX_DETOUR_DETACH(Real_QuerySecurityContextToken, sspi_QuerySecurityContextToken);
	MSRDPEX_DETOUR_DETACH(Real_EncryptMessage, sspi_EncryptMessage);
	MSRDPEX_DETOUR_DETACH(Real_DecryptMessage, sspi_DecryptMessage);
	MSRDPEX_DETOUR_DETACH(Real_SetContextAttributesW, sspi_SetContextAttributesW);

	MSRDPEX_DETOUR_DETACH(Real_SetCredentialsAttributesW, sspi_SetCredentialsAttributesW);

	MSRDPEX_DETOUR_DETACH(Real_QueryContextAttributesExW, sspi_QueryContextAttributesExW);
	MSRDPEX_DETOUR_DETACH(Real_QueryCredentialsAttributesExW, sspi_QueryCredentialsAttributesExW);

	return 0;
}

static const char* MsRdpEx_GetSecurityStatusString(SECURITY_STATUS status)
{
	switch (status)
	{
		case SEC_E_OK:
			return "SEC_E_OK";

		case SEC_E_INSUFFICIENT_MEMORY:
			return "SEC_E_INSUFFICIENT_MEMORY";

		case SEC_E_INVALID_HANDLE:
			return "SEC_E_INVALID_HANDLE";

		case SEC_E_UNSUPPORTED_FUNCTION:
			return "SEC_E_UNSUPPORTED_FUNCTION";

		case SEC_E_TARGET_UNKNOWN:
			return "SEC_E_TARGET_UNKNOWN";

		case SEC_E_INTERNAL_ERROR:
			return "SEC_E_INTERNAL_ERROR";

		case SEC_E_SECPKG_NOT_FOUND:
			return "SEC_E_SECPKG_NOT_FOUND";

		case SEC_E_NOT_OWNER:
			return "SEC_E_NOT_OWNER";

		case SEC_E_CANNOT_INSTALL:
			return "SEC_E_CANNOT_INSTALL";

		case SEC_E_INVALID_TOKEN:
			return "SEC_E_INVALID_TOKEN";

		case SEC_E_CANNOT_PACK:
			return "SEC_E_CANNOT_PACK";

		case SEC_E_QOP_NOT_SUPPORTED:
			return "SEC_E_QOP_NOT_SUPPORTED";

		case SEC_E_NO_IMPERSONATION:
			return "SEC_E_NO_IMPERSONATION";

		case SEC_E_LOGON_DENIED:
			return "SEC_E_LOGON_DENIED";

		case SEC_E_UNKNOWN_CREDENTIALS:
			return "SEC_E_UNKNOWN_CREDENTIALS";

		case SEC_E_NO_CREDENTIALS:
			return "SEC_E_NO_CREDENTIALS";

		case SEC_E_MESSAGE_ALTERED:
			return "SEC_E_MESSAGE_ALTERED";

		case SEC_E_OUT_OF_SEQUENCE:
			return "SEC_E_OUT_OF_SEQUENCE";

		case SEC_E_NO_AUTHENTICATING_AUTHORITY:
			return "SEC_E_NO_AUTHENTICATING_AUTHORITY";

		case SEC_E_BAD_PKGID:
			return "SEC_E_BAD_PKGID";

		case SEC_E_CONTEXT_EXPIRED:
			return "SEC_E_CONTEXT_EXPIRED";

		case SEC_E_INCOMPLETE_MESSAGE:
			return "SEC_E_INCOMPLETE_MESSAGE";

		case SEC_E_INCOMPLETE_CREDENTIALS:
			return "SEC_E_INCOMPLETE_CREDENTIALS";

		case SEC_E_BUFFER_TOO_SMALL:
			return "SEC_E_BUFFER_TOO_SMALL";

		case SEC_E_WRONG_PRINCIPAL:
			return "SEC_E_WRONG_PRINCIPAL";

		case SEC_E_TIME_SKEW:
			return "SEC_E_TIME_SKEW";

		case SEC_E_UNTRUSTED_ROOT:
			return "SEC_E_UNTRUSTED_ROOT";

		case SEC_E_ILLEGAL_MESSAGE:
			return "SEC_E_ILLEGAL_MESSAGE";

		case SEC_E_CERT_UNKNOWN:
			return "SEC_E_CERT_UNKNOWN";

		case SEC_E_CERT_EXPIRED:
			return "SEC_E_CERT_EXPIRED";

		case SEC_E_ENCRYPT_FAILURE:
			return "SEC_E_ENCRYPT_FAILURE";

		case SEC_E_DECRYPT_FAILURE:
			return "SEC_E_DECRYPT_FAILURE";

		case SEC_E_ALGORITHM_MISMATCH:
			return "SEC_E_ALGORITHM_MISMATCH";

		case SEC_E_SECURITY_QOS_FAILED:
			return "SEC_E_SECURITY_QOS_FAILED";

		case SEC_E_UNFINISHED_CONTEXT_DELETED:
			return "SEC_E_UNFINISHED_CONTEXT_DELETED";

		case SEC_E_NO_TGT_REPLY:
			return "SEC_E_NO_TGT_REPLY";

		case SEC_E_NO_IP_ADDRESSES:
			return "SEC_E_NO_IP_ADDRESSES";

		case SEC_E_WRONG_CREDENTIAL_HANDLE:
			return "SEC_E_WRONG_CREDENTIAL_HANDLE";

		case SEC_E_CRYPTO_SYSTEM_INVALID:
			return "SEC_E_CRYPTO_SYSTEM_INVALID";

		case SEC_E_MAX_REFERRALS_EXCEEDED:
			return "SEC_E_MAX_REFERRALS_EXCEEDED";

		case SEC_E_MUST_BE_KDC:
			return "SEC_E_MUST_BE_KDC";

		case SEC_E_STRONG_CRYPTO_NOT_SUPPORTED:
			return "SEC_E_STRONG_CRYPTO_NOT_SUPPORTED";

		case SEC_E_TOO_MANY_PRINCIPALS:
			return "SEC_E_TOO_MANY_PRINCIPALS";

		case SEC_E_NO_PA_DATA:
			return "SEC_E_NO_PA_DATA";

		case SEC_E_PKINIT_NAME_MISMATCH:
			return "SEC_E_PKINIT_NAME_MISMATCH";

		case SEC_E_SMARTCARD_LOGON_REQUIRED:
			return "SEC_E_SMARTCARD_LOGON_REQUIRED";

		case SEC_E_SHUTDOWN_IN_PROGRESS:
			return "SEC_E_SHUTDOWN_IN_PROGRESS";

		case SEC_E_KDC_INVALID_REQUEST:
			return "SEC_E_KDC_INVALID_REQUEST";

		case SEC_E_KDC_UNABLE_TO_REFER:
			return "SEC_E_KDC_UNABLE_TO_REFER";

		case SEC_E_KDC_UNKNOWN_ETYPE:
			return "SEC_E_KDC_UNKNOWN_ETYPE";

		case SEC_E_UNSUPPORTED_PREAUTH:
			return "SEC_E_UNSUPPORTED_PREAUTH";

		case SEC_E_DELEGATION_REQUIRED:
			return "SEC_E_DELEGATION_REQUIRED";

		case SEC_E_BAD_BINDINGS:
			return "SEC_E_BAD_BINDINGS";

		case SEC_E_MULTIPLE_ACCOUNTS:
			return "SEC_E_MULTIPLE_ACCOUNTS";

		case SEC_E_NO_KERB_KEY:
			return "SEC_E_NO_KERB_KEY";

		case SEC_E_CERT_WRONG_USAGE:
			return "SEC_E_CERT_WRONG_USAGE";

		case SEC_E_DOWNGRADE_DETECTED:
			return "SEC_E_DOWNGRADE_DETECTED";

		case SEC_E_SMARTCARD_CERT_REVOKED:
			return "SEC_E_SMARTCARD_CERT_REVOKED";

		case SEC_E_ISSUING_CA_UNTRUSTED:
			return "SEC_E_ISSUING_CA_UNTRUSTED";

		case SEC_E_REVOCATION_OFFLINE_C:
			return "SEC_E_REVOCATION_OFFLINE_C";

		case SEC_E_PKINIT_CLIENT_FAILURE:
			return "SEC_E_PKINIT_CLIENT_FAILURE";

		case SEC_E_SMARTCARD_CERT_EXPIRED:
			return "SEC_E_SMARTCARD_CERT_EXPIRED";

		case SEC_E_NO_S4U_PROT_SUPPORT:
			return "SEC_E_NO_S4U_PROT_SUPPORT";

		case SEC_E_CROSSREALM_DELEGATION_FAILURE:
			return "SEC_E_CROSSREALM_DELEGATION_FAILURE";

		case SEC_E_REVOCATION_OFFLINE_KDC:
			return "SEC_E_REVOCATION_OFFLINE_KDC";

		case SEC_E_ISSUING_CA_UNTRUSTED_KDC:
			return "SEC_E_ISSUING_CA_UNTRUSTED_KDC";

		case SEC_E_KDC_CERT_EXPIRED:
			return "SEC_E_KDC_CERT_EXPIRED";

		case SEC_E_KDC_CERT_REVOKED:
			return "SEC_E_KDC_CERT_REVOKED";

		case SEC_E_INVALID_PARAMETER:
			return "SEC_E_INVALID_PARAMETER";

		case SEC_E_DELEGATION_POLICY:
			return "SEC_E_DELEGATION_POLICY";

		case SEC_E_POLICY_NLTM_ONLY:
			return "SEC_E_POLICY_NLTM_ONLY";

		case SEC_E_NO_CONTEXT:
			return "SEC_E_NO_CONTEXT";

		case SEC_E_PKU2U_CERT_FAILURE:
			return "SEC_E_PKU2U_CERT_FAILURE";

		case SEC_E_MUTUAL_AUTH_FAILED:
			return "SEC_E_MUTUAL_AUTH_FAILED";

		case SEC_I_CONTINUE_NEEDED:
			return "SEC_I_CONTINUE_NEEDED";

		case SEC_I_COMPLETE_NEEDED:
			return "SEC_I_COMPLETE_NEEDED";

		case SEC_I_COMPLETE_AND_CONTINUE:
			return "SEC_I_COMPLETE_AND_CONTINUE";

		case SEC_I_LOCAL_LOGON:
			return "SEC_I_LOCAL_LOGON";

		case SEC_I_CONTEXT_EXPIRED:
			return "SEC_I_CONTEXT_EXPIRED";

		case SEC_I_INCOMPLETE_CREDENTIALS:
			return "SEC_I_INCOMPLETE_CREDENTIALS";

		case SEC_I_RENEGOTIATE:
			return "SEC_I_RENEGOTIATE";

		case SEC_I_NO_LSA_CONTEXT:
			return "SEC_I_NO_LSA_CONTEXT";

		case SEC_I_SIGNATURE_NEEDED:
			return "SEC_I_SIGNATURE_NEEDED";

		case SEC_I_NO_RENEGOTIATION:
			return "SEC_I_NO_RENEGOTIATION";
	}

	return "SEC_I_UNKNOWN";
}