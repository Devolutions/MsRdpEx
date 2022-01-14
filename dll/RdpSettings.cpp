
#include <MsRdpEx/RdpSettings.h>

#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/NameResolver.h>

extern "C" const GUID IID_ITSPropertySet;

class CMsRdpPropertySet : public IMsRdpExtendedSettings
{
public:
    CMsRdpPropertySet(IUnknown* pUnknown)
    {
        m_refCount = 0;
        m_pUnknown = pUnknown;
        pUnknown->QueryInterface(IID_ITSPropertySet, (LPVOID*)&m_pTSPropertySet);
    }

    ~CMsRdpPropertySet()
    {
        m_pUnknown->Release();
        if (m_pTSPropertySet) m_pTSPropertySet->vtbl->Release(m_pTSPropertySet);
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr;
        MsRdpEx_Log("CMsRdpPropertySet::QueryInterface");

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpExtendedSettings) && m_pTSPropertySet)
        {
            *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
            m_refCount++;
            return S_OK;
        }

        hr = m_pUnknown->QueryInterface(riid, ppvObject);
        MsRdpEx_Log("--> hr=%x", hr);
        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CMsRdpPropertySet::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_Log("CMsRdpPropertySet::Release");
        if (--m_refCount == 0)
        {
            MsRdpEx_Log("--> deleting object");
            delete this;
            return 0;
        }
        MsRdpEx_Log("--> refCount=%d", m_refCount);
        return m_refCount;
    }

    // IMsRdpExtendedSettings
public:
    HRESULT __stdcall put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_Log("CMsRdpPropertySet::put_Property(%s)", propName);
        
        if (pValue->vt == VT_BOOL)
        {
            return SetVBoolProperty(propName, pValue->boolVal);
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
        MsRdpEx_Log("CMsRdpPropertySet::get_Property(%s)", propName);

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
        return m_pTSPropertySet->vtbl->SetBoolProperty(m_pTSPropertySet, propName, propValue ? true : false);
    }

    HRESULT __stdcall SetIntProperty(const char* propName, uint32_t propValue) {
        return m_pTSPropertySet->vtbl->SetIntProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetBStrProperty(const char* propName, BSTR propValue) {
        return m_pTSPropertySet->vtbl->SetStringProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall GetVBoolProperty(const char* propName, VARIANT_BOOL* propValue) {
        return m_pTSPropertySet->vtbl->GetBoolProperty(m_pTSPropertySet, propName, propValue);
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

CMsRdpExtendedSettings::CMsRdpExtendedSettings(IUnknown* pUnknown)
{
    m_refCount = 0;
    m_pUnknown = pUnknown;
    pUnknown->QueryInterface(IID_IMsRdpExtendedSettings, (LPVOID*)&m_pMsRdpExtendedSettings);
}

CMsRdpExtendedSettings::~CMsRdpExtendedSettings()
{
    m_pUnknown->Release();
    if (m_pMsRdpExtendedSettings) m_pMsRdpExtendedSettings->Release();
}

HRESULT STDMETHODCALLTYPE CMsRdpExtendedSettings::QueryInterface(
    REFIID riid,
    LPVOID* ppvObject
)
{
    HRESULT hr;
    MsRdpEx_Log("CMsRdpExtendedSettings::QueryInterface");

    if (riid == IID_IUnknown)
    {
        *ppvObject = (LPVOID)((IUnknown*)this);
        m_refCount++;
        return S_OK;
    }
    if ((riid == IID_IMsRdpExtendedSettings) && m_pMsRdpExtendedSettings)
    {
        *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
        m_refCount++;
        return S_OK;
    }

    hr = m_pUnknown->QueryInterface(riid, ppvObject);
    MsRdpEx_Log("--> hr=%x", hr);
    return hr;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::AddRef()
{
    MsRdpEx_Log("CMsRdpExtendedSettings::AddRef");
    return ++m_refCount;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::Release()
{
    MsRdpEx_Log("CMsRdpExtendedSettings::Release");
    if (--m_refCount == 0)
    {
        MsRdpEx_Log("--> deleting object");
        delete this;
        return 0;
    }
    MsRdpEx_Log("--> refCount=%d", m_refCount);
    return m_refCount;
}

HRESULT __stdcall CMsRdpExtendedSettings::put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::put_Property(%s)", propName);
    return m_pMsRdpExtendedSettings->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::get_Property(%s)", propName);

    VariantInit(pValue);

    if (MsRdpEx_StringEquals(propName, "CoreProperties")) {
        if (!m_CoreProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        return m_CoreProps->QueryInterface(IID_IUnknown, (LPVOID*) &pValue->punkVal);
    }
    else if (MsRdpEx_StringEquals(propName, "BaseProperties")) {
        if (!m_BaseProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        return m_BaseProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);
    }
    else if (MsRdpEx_StringEquals(propName, "TransportProperties")) {
        if (!m_TransportProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        return m_TransportProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);
    }

    return m_pMsRdpExtendedSettings->get_Property(bstrPropertyName, pValue);
}

// additional functions

HRESULT __stdcall CMsRdpExtendedSettings::put_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::put_CoreProperty(%s)", propName);

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::get_CoreProperty(%s)", propName);

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->get_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::put_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::put_BaseProperty(%s)", propName);

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_Log("CMsRdpExtendedSettings::get_BaseProperty(%s)", propName);

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->get_Property(bstrPropertyName, pValue);
}

HRESULT CMsRdpExtendedSettings::AttachRdpClient(IMsTscAx* pMsTscAx)
{
    HRESULT hr;

    m_pMsTscAx = pMsTscAx;

    size_t memStatus;
    MEMORY_BASIC_INFORMATION memInfo;

    size_t maxPtrCount = 1000;
    ITSObjectBase* pTSWin32CoreApi = NULL;
    ITSPropertySet* pTSCoreProps = NULL;
    ITSPropertySet* pTSBaseProps = NULL;
    ITSPropertySet* pTSTransportProps = NULL;

    for (int i = 0; i < maxPtrCount; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)m_pMsTscAx)[i];
        memStatus = VirtualQuery(ppTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
        if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize >= 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (pTSObject) {
                memStatus = VirtualQuery(pTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
                if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize > 16)) {
                    if (pTSObject->marker == TSOBJECT_MARKER) {
                        MsRdpEx_Log("MsTscAx(%d): 0x%08X name: %s refCount: %d",
                            i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                        if (!strcmp(pTSObject->name, "CTSPropertySet")) {
                            ITSPropertySet* pTSProps = (ITSPropertySet*) pTSObject;

                            if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                                pTSCoreProps = pTSProps;
                            } else if (!pTSBaseProps && TsPropertyMap_IsBaseProps(pTSProps)) {
                                pTSBaseProps = pTSProps;
                            }
                        }
                        else if (!strcmp(pTSObject->name, "CTSWin32CoreApi")) {
                            pTSWin32CoreApi = pTSObject;
                        }
                    }
                }
            }
        }
    }

    for (int i = 0; i < maxPtrCount; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)pTSWin32CoreApi)[i];
        memStatus = VirtualQuery(ppTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
        if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize >= 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (pTSObject) {
                memStatus = VirtualQuery(pTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
                if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize > 16)) {
                    if (pTSObject->marker == TSOBJECT_MARKER) {
                        MsRdpEx_Log("TSWin32CoreApi(%d): 0x%08X name: %s refCount: %d",
                            i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                        if (!strcmp(pTSObject->name, "CTSPropertySet")) {
                            ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                            if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                                pTSCoreProps = pTSProps;
                            }
                            else if (!pTSBaseProps && TsPropertyMap_IsBaseProps(pTSProps)) {
                                pTSBaseProps = pTSProps;
                            }
                        }
                    }
                }
            }
        }
    }

    MsRdpEx_Log("pTSCoreProps1: %p", pTSCoreProps);

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

    MsRdpEx_Log("Loading %s RDP", filename);
    MsRdpEx_RdpFile* rdpFile = MsRdpEx_RdpFile_New();

    if (MsRdpEx_RdpFile_Load(rdpFile, filename)) {
        MsRdpEx_ArrayListIt* it = NULL;
        MsRdpEx_RdpFileEntry* entry = NULL;

        it = MsRdpEx_ArrayList_It(rdpFile->entries, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

        while (!MsRdpEx_ArrayListIt_Done(it))
        {
            entry = (MsRdpEx_RdpFileEntry*)MsRdpEx_ArrayListIt_Next(it);

            if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableCredentialsDelegation")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    pMsRdpExtendedSettings->PutProperty("DisableCredentialsDelegation", &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RedirectedAuthentication")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    pMsRdpExtendedSettings->PutProperty("RedirectedAuthentication", &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RestrictedLogon")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    pMsRdpExtendedSettings->PutProperty("RestrictedLogon", &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "AutoLogon")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    pMsRdpExtendedSettings->PutProperty("AutoLogon", &value);
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ServerNameUsedForAuthentication")) {
                char* oldServerName = NULL;
                bstr_t ServerNameUsedForAuthentication = _com_util::ConvertStringToBSTR(entry->value);
                MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, m_pMsTscAx->GetServer().GetBSTR(), -1, &oldServerName, 0, NULL, NULL);
                m_pMsTscAx->PutServer(ServerNameUsedForAuthentication);
                MsRdpEx_NameResolver_RemapName(entry->value, oldServerName);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableUDPTransport")) {
                VARIANT value;
                if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                    bstr_t propName = _com_util::ConvertStringToBSTR(entry->value);
                    pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
                }
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

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx)
{
    CMsRdpExtendedSettings* settings = new CMsRdpExtendedSettings(pUnknown);
    settings->AttachRdpClient((IMsTscAx*) pMsTscAx);
    return settings;
}
