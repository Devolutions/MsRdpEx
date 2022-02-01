#include "MsRdpEx.h"

#include <MsRdpEx/Sspi.h>

static PSecurityFunctionTableW g_RealTable = NULL;
static PSecurityFunctionTableW g_HookTable = NULL;

static const char* MsRdpEx_GetSecurityStatusString(SECURITY_STATUS status);

static SECURITY_STATUS SEC_ENTRY sspi_EnumerateSecurityPackagesW(ULONG* pcPackages,
	PSecPkgInfoW* ppPackageInfo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("EnumerateSecurityPackagesW");

	if (!(g_RealTable && g_RealTable->EnumerateSecurityPackagesW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->EnumerateSecurityPackagesW(pcPackages, ppPackageInfo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryCredentialsAttributesW(PCredHandle phCredential,
	ULONG ulAttribute, void* pBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("QueryCredentialsAttributesW");

	if (!(g_RealTable && g_RealTable->QueryCredentialsAttributesW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->QueryCredentialsAttributesW(phCredential, ulAttribute, pBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_AcquireCredentialsHandleW(
	SEC_WCHAR* pszPrincipal, SEC_WCHAR* pszPackage, ULONG fCredentialUse, void* pvLogonID,
	void* pAuthData, SEC_GET_KEY_FN pGetKeyFn, void* pvGetKeyArgument, PCredHandle phCredential,
	PTimeStamp ptsExpiry)
{
	SECURITY_STATUS status;
	char* pszPrincipalA = NULL;
	char* pszPackageA = NULL;

	if (!(g_RealTable && g_RealTable->AcquireCredentialsHandleW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	if (pszPrincipal)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszPrincipal, -1, &pszPrincipalA, 0, NULL, NULL);

	if (pszPackage)
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pszPackage, -1, &pszPackageA, 0, NULL, NULL);

	MsRdpEx_Log("AcquireCredentialsHandleW(principal=\"%s\", package=\"%s\")",
		pszPrincipalA ? pszPrincipalA : "",
		pszPackageA ? pszPackageA : "");

	if (MsRdpEx_StringIEquals(pszPackageA, "CREDSSP") && pAuthData) {
		CREDSSP_CRED* pCred = (CREDSSP_CRED*) pAuthData;
		MsRdpEx_Log("CredSSP submit type: %d schannel: %d spnego: %d",
			pCred->Type, pCred->pSchannelCred ? 1:0, pCred->pSpnegoCred ? 1:0);

		if (pCred->pSpnegoCred) {
			SECURITY_STATUS secStatus = SEC_E_OK;
			bool bUnprotectCredentials = false;
			DWORD dwUnpackFlags = 0;
			char* userA = NULL;
			char* domainA = NULL;
			char* passwordA = NULL;
			WCHAR szUserName[512+1];
			WCHAR szDomainName[512+1];
			WCHAR szPassword[512+1];
			DWORD cchMaxUserName = sizeof(szUserName) / sizeof(WCHAR);
			DWORD cchMaxDomainName = sizeof(szDomainName) / sizeof(WCHAR);
			DWORD cchMaxPassword = sizeof(szPassword) / sizeof(WCHAR);
			DWORD cchUserName = cchMaxUserName;
			DWORD cchDomainName = cchMaxDomainName;
			DWORD cchPassword = cchMaxPassword;
			DWORD cbAuthBuffer = LocalSize((HLOCAL)pCred->pSpnegoCred);

			if (bUnprotectCredentials) {
				dwUnpackFlags |= CRED_PACK_PROTECTED_CREDENTIALS;
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
	}

	status = g_RealTable->AcquireCredentialsHandleW(pszPrincipal, pszPackage, fCredentialUse, pvLogonID,
		pAuthData, pGetKeyFn, pvGetKeyArgument,
		phCredential, ptsExpiry);

	free(pszPrincipalA);
	free(pszPackageA);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_FreeCredentialsHandle(PCredHandle phCredential)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("FreeCredentialsHandle");

	if (!(g_RealTable && g_RealTable->FreeCredentialsHandle))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->FreeCredentialsHandle(phCredential);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_InitializeSecurityContextW(
	PCredHandle phCredential, PCtxtHandle phContext, SEC_WCHAR* pszTargetName, ULONG fContextReq,
	ULONG Reserved1, ULONG TargetDataRep, PSecBufferDesc pInput, ULONG Reserved2,
	PCtxtHandle phNewContext, PSecBufferDesc pOutput, PULONG pfContextAttr, PTimeStamp ptsExpiry)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("InitializeSecurityContextW");

	if (!(g_RealTable && g_RealTable->InitializeSecurityContextW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->InitializeSecurityContextW(
		phCredential, phContext, pszTargetName, fContextReq, Reserved1, TargetDataRep, pInput,
		Reserved2, phNewContext, pOutput, pfContextAttr, ptsExpiry);

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

	MsRdpEx_Log("AcceptSecurityContext");

	if (!(g_RealTable && g_RealTable->AcceptSecurityContext))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->AcceptSecurityContext(phCredential,
		phContext, pInput, fContextReq, TargetDataRep,
		phNewContext, pOutput, pfContextAttr, ptsTimeStamp);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_CompleteAuthToken(PCtxtHandle phContext, PSecBufferDesc pToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("CompleteAuthToken");

	if (!(g_RealTable && g_RealTable->CompleteAuthToken))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->CompleteAuthToken(phContext, pToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_DeleteSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("DeleteSecurityContext");

	if (!(g_RealTable && g_RealTable->DeleteSecurityContext))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->DeleteSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ApplyControlToken(PCtxtHandle phContext, PSecBufferDesc pInput)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("ApplyControlToken");

	if (!(g_RealTable && g_RealTable->ApplyControlToken))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->ApplyControlToken(phContext, pInput);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QueryContextAttributesW(PCtxtHandle phContext, ULONG ulAttribute,
	void* pBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("QueryContextAttributesW");

	if (!(g_RealTable && g_RealTable->QueryContextAttributesW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->QueryContextAttributesW(phContext, ulAttribute, pBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ImpersonateSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("ImpersonateSecurityContext");

	if (!(g_RealTable && g_RealTable->ImpersonateSecurityContext))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->ImpersonateSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_RevertSecurityContext(PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("RevertSecurityContext");

	if (!(g_RealTable && g_RealTable->RevertSecurityContext))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->RevertSecurityContext(phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_MakeSignature(PCtxtHandle phContext, ULONG fQOP,
	PSecBufferDesc pMessage, ULONG MessageSeqNo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("MakeSignature");

	if (!(g_RealTable && g_RealTable->MakeSignature))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->MakeSignature(phContext, fQOP, pMessage, MessageSeqNo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_VerifySignature(PCtxtHandle phContext, PSecBufferDesc pMessage,
	ULONG MessageSeqNo, PULONG pfQOP)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("VerifySignature");

	if (!(g_RealTable && g_RealTable->VerifySignature))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->VerifySignature(phContext, pMessage, MessageSeqNo, pfQOP);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_FreeContextBuffer(void* pvContextBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("FreeContextBuffer");

	if (!(g_RealTable && g_RealTable->FreeContextBuffer))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->FreeContextBuffer(pvContextBuffer);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QuerySecurityPackageInfoW(SEC_WCHAR* pszPackageName,
	PSecPkgInfoW* ppPackageInfo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("QuerySecurityPackageInfoW");

	if (!(g_RealTable && g_RealTable->QuerySecurityPackageInfoW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->QuerySecurityPackageInfoW(pszPackageName, ppPackageInfo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ExportSecurityContext(PCtxtHandle phContext, ULONG fFlags,
	PSecBuffer pPackedContext, HANDLE* pToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("ExportSecurityContext");

	if (!(g_RealTable && g_RealTable->ExportSecurityContext))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->ExportSecurityContext(phContext, fFlags, pPackedContext, pToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_ImportSecurityContextW(SEC_WCHAR* pszPackage,
	PSecBuffer pPackedContext, HANDLE pToken, PCtxtHandle phContext)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("ImportSecurityContextW");

	if (!(g_RealTable && g_RealTable->ImportSecurityContextW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->ImportSecurityContextW(pszPackage, pPackedContext, pToken, phContext);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_QuerySecurityContextToken(PCtxtHandle phContext, HANDLE* phToken)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("QuerySecurityContextToken");

	if (!(g_RealTable && g_RealTable->QuerySecurityContextToken))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->QuerySecurityContextToken(phContext, phToken);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_EncryptMessage(PCtxtHandle phContext, ULONG fQOP,
	PSecBufferDesc pMessage, ULONG MessageSeqNo)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("EncryptMessage seqNo: %d", MessageSeqNo);

	if (!(g_RealTable && g_RealTable->EncryptMessage))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->EncryptMessage(phContext, fQOP, pMessage, MessageSeqNo);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_DecryptMessage(PCtxtHandle phContext, PSecBufferDesc pMessage,
	ULONG MessageSeqNo, PULONG pfQOP)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("DecryptMessage seqNo: %d", MessageSeqNo);

	if (!(g_RealTable && g_RealTable->DecryptMessage))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->DecryptMessage(phContext, pMessage, MessageSeqNo, pfQOP);

	return status;
}

static SECURITY_STATUS SEC_ENTRY sspi_SetContextAttributesW(PCtxtHandle phContext, ULONG ulAttribute,
	void* pBuffer, ULONG cbBuffer)
{
	SECURITY_STATUS status;

	MsRdpEx_Log("SetContextAttributesW");

	if (!(g_RealTable && g_RealTable->SetContextAttributesW))
	{
		return SEC_E_UNSUPPORTED_FUNCTION;
	}

	status = g_RealTable->SetContextAttributesW(phContext, ulAttribute, pBuffer, cbBuffer);

	return status;
}

static const SecurityFunctionTableW sspi_SecurityFunctionTableW = {
	1,                                /* dwVersion */
	sspi_EnumerateSecurityPackagesW,  /* EnumerateSecurityPackages */
	sspi_QueryCredentialsAttributesW, /* QueryCredentialsAttributes */
	sspi_AcquireCredentialsHandleW,   /* AcquireCredentialsHandle */
	sspi_FreeCredentialsHandle,       /* FreeCredentialsHandle */
	NULL,                             /* Reserved2 */
	sspi_InitializeSecurityContextW,  /* InitializeSecurityContext */
	sspi_AcceptSecurityContext,       /* AcceptSecurityContext */
	sspi_CompleteAuthToken,           /* CompleteAuthToken */
	sspi_DeleteSecurityContext,       /* DeleteSecurityContext */
	sspi_ApplyControlToken,           /* ApplyControlToken */
	sspi_QueryContextAttributesW,     /* QueryContextAttributes */
	sspi_ImpersonateSecurityContext,  /* ImpersonateSecurityContext */
	sspi_RevertSecurityContext,       /* RevertSecurityContext */
	sspi_MakeSignature,               /* MakeSignature */
	sspi_VerifySignature,             /* VerifySignature */
	sspi_FreeContextBuffer,           /* FreeContextBuffer */
	sspi_QuerySecurityPackageInfoW,   /* QuerySecurityPackageInfo */
	NULL,                             /* Reserved3 */
	NULL,                             /* Reserved4 */
	sspi_ExportSecurityContext,       /* ExportSecurityContext */
	sspi_ImportSecurityContextW,      /* ImportSecurityContext */
	NULL,                             /* AddCredentials */
	NULL,                             /* Reserved8 */
	sspi_QuerySecurityContextToken,   /* QuerySecurityContextToken */
	sspi_EncryptMessage,              /* EncryptMessage */
	sspi_DecryptMessage,              /* DecryptMessage */
	sspi_SetContextAttributesW,       /* SetContextAttributes */
};

PSecurityFunctionTableW MsRdpEx_SspiHook_Init(PSecurityFunctionTableW pSecTable)
{
	g_RealTable = pSecTable;
	g_HookTable = (PSecurityFunctionTableW) & sspi_SecurityFunctionTableW;
	MsRdpEx_Log("InitSecurityInterfaceW");
	return g_HookTable;
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