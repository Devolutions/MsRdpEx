
#include <MsRdpEx/RdpSettings.h>

#include <MsRdpEx/Memory.h>
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/Environment.h>
#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/Detours.h>

#include <intrin.h>

#include "TSObjects.h"

extern "C" const GUID IID_ITSPropertySet;

static bool g_TSPropertySet_Hooked = false;

static ITSPropertySet_SetBoolProperty Real_ITSPropertySet_SetBoolProperty = NULL;
static ITSPropertySet_GetBoolProperty Real_ITSPropertySet_GetBoolProperty = NULL;
static ITSPropertySet_SetIntProperty Real_ITSPropertySet_SetIntProperty = NULL;
static ITSPropertySet_GetIntProperty Real_ITSPropertySet_GetIntProperty = NULL;
static ITSPropertySet_SetStringProperty Real_ITSPropertySet_SetStringProperty = NULL;
static ITSPropertySet_GetStringProperty Real_ITSPropertySet_GetStringProperty = NULL;

static HRESULT Hook_ITSPropertySet_SetBoolProperty(ITSPropertySet* This, const char* propName, int propValue)
{
    HRESULT hr;

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetBoolProperty(%s, %d)", propName, propValue);

    if (MsRdpEx_StringIEquals(propName, "UsingSavedCreds")) {
        // Workaround for "Always prompt for password upon connection" GPO":
        // The RDP ActiveX sets the "UsingSavedCreds" to true if the password is set.
        // This is obviously a problem for credential injection, so force it to false.
        // https://theitbros.com/enable-saved-credentials-usage-rdp/
        // https://twitter.com/awakecoding/status/1367953137210957826
        // HKLM:\SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\fPromptForPassword = 1
        propValue = 0;
    }

    if (MsRdpEx_StringIEquals(propName, "ShellMarkRdpSecure")) {
        // Workaround for RDP file signature requirement.
        // Always mark RDP files as "signed" even when they're not
        propValue = 1;
    }

    if (MsRdpEx_StringIEquals(propName, "ShowRedirectionWarningDialog")) {
        // Workaround for RDP file signature requirement.
        // Disable the redirection warning dialog to avoid some checks.
        propValue = 0;
    }

    hr = Real_ITSPropertySet_SetBoolProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetBoolProperty(ITSPropertySet* This, const char* propName, int* propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetBoolProperty(This, propName, propValue);

    if (MsRdpEx_StringIEquals(propName, "TSGTransportIsUsed")) {
        if (MsRdpEx_IsAddressInModule(_ReturnAddress(), L"mstscax.dll") ||
            MsRdpEx_IsAddressInModule(_ReturnAddress(), L"rdclientax.dll")) {
            // Workaround to apply KDCProxyName value when not using RD Gateway
            // This enables injection of KDC proxy settings at all times.
            *propValue = 1;
        }
    }

    if (MsRdpEx_StringIEquals(propName, "UseNewOutput")) {
        // Workaround to prevent disconnect error code 3334 with latest MSRDC
        *propValue = 1;
    }

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetBoolProperty(%s, %d)", propName, *propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_SetIntProperty(ITSPropertySet* This, const char* propName, int propValue)
{
    HRESULT hr;

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetIntProperty(%s, %d)", propName, propValue);

    hr = Real_ITSPropertySet_SetIntProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetIntProperty(ITSPropertySet* This, const char* propName, int* propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetIntProperty(This, propName, propValue);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetIntProperty(%s, %d)", propName, *propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_SetStringProperty(ITSPropertySet* This, const char* propName, WCHAR* propValue)
{
    HRESULT hr;

    char* propValueA = _com_util::ConvertBSTRToString((BSTR) propValue);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetStringProperty(%s, \"%s\")", propName, propValueA);

    hr = Real_ITSPropertySet_SetStringProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetStringProperty(ITSPropertySet* This, const char* propName, WCHAR** propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetStringProperty(This, propName, propValue);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetStringProperty(%s)", propName);

    return hr;
}

static bool TSPropertySet_Hook(ITSPropertySet* pTSPropertySet)
{
    LONG error;

    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    Real_ITSPropertySet_SetBoolProperty = pTSPropertySet->vtbl->SetBoolProperty;
    Real_ITSPropertySet_GetBoolProperty = pTSPropertySet->vtbl->GetBoolProperty;
    Real_ITSPropertySet_SetIntProperty = pTSPropertySet->vtbl->SetIntProperty;
    Real_ITSPropertySet_GetIntProperty = pTSPropertySet->vtbl->GetIntProperty;
    Real_ITSPropertySet_SetStringProperty = pTSPropertySet->vtbl->SetStringProperty;
    Real_ITSPropertySet_GetStringProperty = pTSPropertySet->vtbl->GetStringProperty;

    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetBoolProperty), Hook_ITSPropertySet_SetBoolProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetBoolProperty), Hook_ITSPropertySet_GetBoolProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetIntProperty), Hook_ITSPropertySet_SetIntProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetIntProperty), Hook_ITSPropertySet_GetIntProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetStringProperty), Hook_ITSPropertySet_SetStringProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetStringProperty), Hook_ITSPropertySet_GetStringProperty);

    error = DetourTransactionCommit();
    return true;
}

class CMsRdpPropertySet : public IMsRdpExtendedSettings
{
public:
    CMsRdpPropertySet(IUnknown* pUnknown)
    {
        m_refCount = 0;
        m_pUnknown = pUnknown;
        pUnknown->QueryInterface(IID_ITSPropertySet, (LPVOID*)&m_pTSPropertySet);

        if (!g_TSPropertySet_Hooked) {
            TSPropertySet_Hook(m_pTSPropertySet);
            g_TSPropertySet_Hooked = true;
        }
    }

    ~CMsRdpPropertySet()
    {
        m_pUnknown->Release();
        if (m_pTSPropertySet) {
            m_pTSPropertySet->vtbl->Release(m_pTSPropertySet);
            m_pTSPropertySet = NULL;
        }
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr = E_NOINTERFACE;
        ULONG refCount = m_refCount;
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::QueryInterface");

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpExtendedSettings) && m_pTSPropertySet)
        {
            *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else
        {
            hr = m_pUnknown->QueryInterface(riid, ppvObject);
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IMsRdpExtendedSettings
public:
    HRESULT __stdcall put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::put_Property(%s, vt: %d)", propName, pValue->vt);
        
        if (pValue->vt == VT_BOOL)
        {
            return SetVBoolProperty(propName, pValue->boolVal);
        }
        else if (pValue->vt == VT_I4)
        {
            return SetIntProperty(propName, pValue->intVal);
        }
        else if (pValue->vt == VT_UI4)
        {
            return SetIntProperty(propName, pValue->uintVal);
        }
        else if (pValue->vt == VT_BSTR)
        {
            return SetBStrProperty(propName, pValue->bstrVal);
        }

        return E_INVALIDARG;
    }

    HRESULT __stdcall get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        HRESULT hr = E_INVALIDARG;
        uint8_t propType = 0;
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::get_Property(%s)", propName);

        VariantInit(pValue);

        if (GetTSPropertyType(m_pTSPropertySet, propName, &propType)) {
            if (propType == TSPROPERTY_TYPE_STRING) {
                hr = GetBStrProperty(propName, &pValue->bstrVal);

                if (hr == S_OK) {
                    pValue->vt = VT_BSTR;
                }
            }
            else if (propType == TSPROPERTY_TYPE_BOOL) {
                hr = GetVBoolProperty(propName, &pValue->boolVal);

                if (hr == S_OK) {
                    pValue->vt = VT_BOOL;
                }
            }
        }

        return hr;
    }

    HRESULT __stdcall SetVBoolProperty(const char* propName, VARIANT_BOOL propValue) {
        return m_pTSPropertySet->vtbl->SetBoolProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetIntProperty(const char* propName, uint32_t propValue) {
        return m_pTSPropertySet->vtbl->SetIntProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetBStrProperty(const char* propName, BSTR propValue) {
        return m_pTSPropertySet->vtbl->SetStringProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetSecureStringProperty(const char* propName, BSTR propValue) {
        return m_pTSPropertySet->vtbl->SetSecureStringProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall GetVBoolProperty(const char* propName, VARIANT_BOOL* propValue) {
        HRESULT hr;
        int iVal = 0;
        hr = m_pTSPropertySet->vtbl->GetBoolProperty(m_pTSPropertySet, propName, &iVal);
        *propValue = iVal ? VARIANT_TRUE : VARIANT_FALSE;
        return hr;
    }

    HRESULT __stdcall GetBStrProperty(const char* propName, BSTR* propValue) {
        HRESULT hr;
        BSTR bstrVal = NULL;
        WCHAR* wstrVal = NULL;

        hr = m_pTSPropertySet->vtbl->GetStringProperty(m_pTSPropertySet, propName, &wstrVal);

        if (hr != S_OK)
            return hr;

        *propValue = SysAllocString(wstrVal);

        return hr;
    }

private:
    ULONG m_refCount;
    IUnknown* m_pUnknown;
    ITSPropertySet* m_pTSPropertySet;
};

CMsRdpExtendedSettings::CMsRdpExtendedSettings(IUnknown* pUnknown, GUID* pSessionId)
{
    m_refCount = 0;
    m_pUnknown = pUnknown;

    MsRdpEx_GuidCopy(&m_sessionId, pSessionId);

    pUnknown->QueryInterface(IID_IMsRdpExtendedSettings, (LPVOID*)&m_pMsRdpExtendedSettings);

    if (m_pMsRdpExtendedSettings)
        m_pMsRdpExtendedSettings->AddRef();
}

CMsRdpExtendedSettings::~CMsRdpExtendedSettings()
{
    this->SetKdcProxyUrl(NULL);

    if (m_pMsRdpExtendedSettings)
        m_pMsRdpExtendedSettings->Release();
}

HRESULT STDMETHODCALLTYPE CMsRdpExtendedSettings::QueryInterface(
    REFIID riid,
    LPVOID* ppvObject
)
{
    HRESULT hr = E_NOINTERFACE;
    ULONG refCount = m_refCount;
    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    if (riid == IID_IUnknown)
    {
        *ppvObject = (LPVOID)((IUnknown*)this);
        refCount = InterlockedIncrement(&m_refCount);
        hr = S_OK;
    }
    else if ((riid == IID_IMsRdpExtendedSettings) && m_pMsRdpExtendedSettings)
    {
        *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
        refCount = InterlockedIncrement(&m_refCount);
        hr = S_OK;
    }
    else
    {
        hr = m_pUnknown->QueryInterface(riid, ppvObject);
    }

    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

    return hr;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::AddRef()
{
    ULONG refCount = InterlockedIncrement(&m_refCount);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::AddRef() = %d", refCount);
    return refCount;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::Release()
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::Release() = %d", refCount);

    if (refCount == 0)
    {
        delete this;
        return 0;
    }

    return refCount;
}

HRESULT __stdcall CMsRdpExtendedSettings::put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    HRESULT hr = E_INVALIDARG;
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_Property(%s)", propName);

    if (MsRdpEx_StringEquals(propName, "KDCProxyURL"))
    {
        if (pValue->vt != VT_BSTR)
            goto end;

        char* propValue = _com_util::ConvertBSTRToString(pValue->bstrVal);

        if (propValue) {
            hr = this->SetKdcProxyUrl(propValue);
        }

        free(propValue);
    }
    else
    {
        if (pValue->vt == VT_BSTR) {
            char* propValueA = _com_util::ConvertBSTRToString((BSTR)pValue->bstrVal);
            MsRdpEx_LogPrint(TRACE, "CMsRdpExtendedSettings::put_Property(%s, \"%s\")", propName, propValueA);
        }

        hr = m_pMsRdpExtendedSettings->put_Property(bstrPropertyName, pValue);
    }

end:
    return hr;
}

HRESULT __stdcall CMsRdpExtendedSettings::get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    HRESULT hr = E_INVALIDARG;
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_Property(%s)", propName);

    VariantInit(pValue);

    if (MsRdpEx_StringEquals(propName, "CoreProperties")) {
        if (!m_CoreProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_CoreProps->QueryInterface(IID_IUnknown, (LPVOID*) &pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "BaseProperties")) {
        if (!m_BaseProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_BaseProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "TransportProperties")) {
        if (!m_TransportProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_TransportProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "KDCProxyURL")) {
        pValue->vt = VT_BSTR;
        const char* kdcProxyUrl = m_KdcProxyUrl ? m_KdcProxyUrl : "";
        pValue->bstrVal = _com_util::ConvertStringToBSTR(kdcProxyUrl);
        hr = S_OK;
    }
    else {
        hr = m_pMsRdpExtendedSettings->get_Property(bstrPropertyName, pValue);
    }

    return hr;
}

// additional functions

HRESULT __stdcall CMsRdpExtendedSettings::put_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_CoreProperty(%s)", propName);

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_CoreProperty(%s)", propName);

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->get_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::put_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_BaseProperty(%s)", propName);

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_BaseProperty(%s)", propName);

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->get_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::SetTargetPassword(const char* password) {
    if (!m_CoreProps)
        return E_INVALIDARG;

    bstr_t propValue = _com_util::ConvertStringToBSTR(password);
    m_CoreProps->SetSecureStringProperty("Password", propValue);

    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetGatewayPassword(const char* password) {
    if (!m_TransportProps)
        return E_INVALIDARG;

    bstr_t propValue = _com_util::ConvertStringToBSTR(password);
    m_TransportProps->SetSecureStringProperty("GatewayPassword", propValue);

    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetKdcProxyUrl(const char* kdcProxyUrl) {
    free(m_KdcProxyUrl);
    m_KdcProxyUrl = NULL;

    if (kdcProxyUrl) {
        m_KdcProxyUrl = _strdup(kdcProxyUrl);
    }
    return S_OK;
}

HRESULT CMsRdpExtendedSettings::AttachRdpClient(IMsTscAx* pMsTscAx)
{
    HRESULT hr;

    m_pMsTscAx = pMsTscAx;

    ITSObjectBase* pTSWin32CoreApi = NULL;
    ITSPropertySet* pTSCoreProps = NULL;
    ITSPropertySet* pTSBaseProps = NULL;
    ITSPropertySet* pTSTransportProps = NULL;

    for (int i = 0; i < 500; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)m_pMsTscAx)[i];
        if (MsRdpEx_CanReadUnsafePtr(ppTSObject, 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (MsRdpEx_CanReadUnsafePtr(pTSObject, sizeof(ITSObjectBase))) {
                if (pTSObject->marker == TSOBJECT_MARKER) {
                    MsRdpEx_LogPrint(DEBUG, "MsTscAx(%d): 0x%08X name: %s refCount: %d",
                        i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                    if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSPropertySet")) {
                        ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                        if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                            pTSCoreProps = pTSProps;
                        }
                        else if (!pTSBaseProps && TsPropertyMap_IsBaseProps(pTSProps)) {
                            pTSBaseProps = pTSProps;
                        }
                    }
                    else if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSWin32CoreApi")) {
                        pTSWin32CoreApi = pTSObject;
                    }
                }
            }
        }
    }

    MsRdpEx_LogPrint(DEBUG, "pTSCoreProps: %p", pTSCoreProps);
    MsRdpEx_LogPrint(DEBUG, "pTSBaseProps: %p", pTSBaseProps);
    MsRdpEx_LogPrint(DEBUG, "CTSWin32CoreApi: %p", pTSWin32CoreApi);

    if (pTSCoreProps)
    {
        m_pCorePropsRaw = pTSCoreProps;
        m_CoreProps = new CMsRdpPropertySet((IUnknown*)pTSCoreProps);
        //DumpTSPropertyMap(pTSCoreProps, "Core");
    }

    if (pTSBaseProps)
    {
        m_BaseProps = new CMsRdpPropertySet((IUnknown*)pTSBaseProps);
        //DumpTSPropertyMap(pTSBaseProps, "Base");
    }

    if (pTSTransportProps)
    {
        m_TransportProps = new CMsRdpPropertySet((IUnknown*)pTSTransportProps);
        //DumpTSPropertyMap(pTSTransportProps, "Transport");
    }

    return S_OK;
}

HRESULT CMsRdpExtendedSettings::LoadRdpFile(const char* rdpFileName)
{
    char* filename;

    if (rdpFileName)
        filename = _strdup(rdpFileName);
    else
        filename = MsRdpEx_GetRdpFilenameFromCommandLine();

    if (!filename)
        return E_UNEXPECTED;

    CMsRdpExtendedSettings* pMsRdpExtendedSettings = this;

    MsRdpEx_LogPrint(DEBUG, "Loading %s", filename);
    MsRdpEx_RdpFile* rdpFile = MsRdpEx_RdpFile_New();

    if (MsRdpEx_RdpFile_Load(rdpFile, filename)) {
        MsRdpEx_ArrayListIt* it = NULL;
        MsRdpEx_RdpFileEntry* entry = NULL;

        it = MsRdpEx_ArrayList_It(rdpFile->entries, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

        while (!MsRdpEx_ArrayListIt_Done(it))
        {
            entry = (MsRdpEx_RdpFileEntry*) MsRdpEx_ArrayListIt_Next(it);

            if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableCredentialsDelegation")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                    pMsRdpExtendedSettings->PutProperty(propName, &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RedirectedAuthentication")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                    pMsRdpExtendedSettings->PutProperty(propName, &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RestrictedLogon")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                    pMsRdpExtendedSettings->PutProperty(propName, &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "UserSpecifiedServerName")) {
                VARIANT value;
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
                value.bstrVal = propValue;
                value.vt = VT_BSTR;
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableUDPTransport")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                    pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableHardwareMode")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                    pMsRdpExtendedSettings->put_Property(propName, &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ClearTextPassword")) {
                pMsRdpExtendedSettings->SetTargetPassword(entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "GatewayPassword")) {
                pMsRdpExtendedSettings->SetGatewayPassword(entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetUserName")) {
                VARIANT value;
                bstr_t propName = _com_util::ConvertStringToBSTR("UserName");
                bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
                value.bstrVal = propValue;
                value.vt = VT_BSTR;
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetDomain")) {
                VARIANT value;
                bstr_t propName = _com_util::ConvertStringToBSTR("Domain");
                bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
                value.bstrVal = propValue;
                value.vt = VT_BSTR;
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "KDCProxyURL")) {
                pMsRdpExtendedSettings->SetKdcProxyUrl(entry->value);
            }
        }

        MsRdpEx_ArrayListIt_Finish(it);
    }
    MsRdpEx_RdpFile_Free(rdpFile);
    free(filename);

    return S_OK;
}

HRESULT CMsRdpExtendedSettings::GetCorePropsRawPtr(LPVOID* ppCorePropsRaw)
{
    *ppCorePropsRaw = m_pCorePropsRaw;
    return S_OK;
}

HRESULT CMsRdpExtendedSettings::PrepareSspiSessionIdHack()
{
    HRESULT hr = S_OK;
    char fakeKdcProxyName[256];
    char sessionId[MSRDPEX_GUID_STRING_SIZE];

    if (!m_CoreProps) {
        MsRdpEx_LogPrint(ERROR, "PrepareSspiSessionIdHack - m_CoreProps is NULL!");
        return E_UNEXPECTED;
    }

    MsRdpEx_GuidBinToStr((GUID*)&m_sessionId, sessionId, 0);
    sprintf_s(fakeKdcProxyName, sizeof(fakeKdcProxyName) - 1, "MsRdpEx/%s", sessionId);

    BSTR fakeKdcProxyNameB = _com_util::ConvertStringToBSTR(fakeKdcProxyName);
    m_CoreProps->SetBStrProperty("KDCProxyName", fakeKdcProxyNameB);
    SysFreeString(fakeKdcProxyNameB);

    return hr;
}

char* CMsRdpExtendedSettings::GetKdcProxyUrl()
{
    if (m_KdcProxyUrl)
        return _strdup(m_KdcProxyUrl);

    return NULL;
}

char* CMsRdpExtendedSettings::GetKdcProxyName()
{
    return MsRdpEx_KdcProxyUrlToName(m_KdcProxyUrl);
}

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx, GUID* pSessionId)
{
    CMsRdpExtendedSettings* settings = new CMsRdpExtendedSettings(pUnknown, pSessionId);
    settings->AttachRdpClient((IMsTscAx*) pMsTscAx);
    return settings;
}

char* MsRdpEx_KdcProxyUrlToName(const char* kdcProxyUrl)
{
    char* path = NULL;
    const char* host = NULL;
    char* kdcProxyName = NULL;

    // https://<host>[:<port>][/path]
    // <host>:[:<port>][:<path>]

    if (!kdcProxyUrl)
        return NULL;

    host = strstr(kdcProxyUrl, "://");

    if (!host)
        return NULL;

    host = &host[3];

    kdcProxyName = _strdup(host);

    if (!kdcProxyName)
        return NULL;

    path = (char*)strchr(kdcProxyName, '/');

    if (path)
        *path = ':';

    return kdcProxyName;
}
