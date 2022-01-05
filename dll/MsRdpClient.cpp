
#include "MsRdpClient.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/NameResolver.h>

#pragma warning (disable : 26812)

#include "mstscax.tlh"
#include "mstscax.tli"

extern "C" const GUID __declspec(selectany) IID_ITSPropertySet =
    { 0x7272B10D,0xC627,0x90DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_ITSNameResolver =
    { 0x7272B10D,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_ITSTransport =
    { 0x7272B10E,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_IMstscAxInternal =
    { 0x7272B1A7,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_ITSCoreApi =
    { 0x7272B115,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_ITSCoreApiInternal =
    { 0x7272B137,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
extern "C" const GUID __declspec(selectany) IID_ITSWin32CoreApi =
    { 0x7272B113,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };

typedef struct _CIUnknown CIUnknown;

typedef struct IUnknownVtbl
{
    HRESULT(STDMETHODCALLTYPE* QueryInterface)(IUnknown* This, REFIID riid, void** ppvObject);
    ULONG(STDMETHODCALLTYPE* AddRef)(IUnknown* This);
    ULONG(STDMETHODCALLTYPE* Release)(IUnknown* This);
} IUnknownVtbl;

struct _CIUnknown
{
    IUnknownVtbl* vtbl;
};

typedef struct _IMstscAxInternal IMstscAxInternal;

typedef struct IMstscAxInternalVtbl
{
    HRESULT(STDMETHODCALLTYPE* QueryInterface)(IMstscAxInternal* This, REFIID riid, void** ppvObject);
    ULONG(STDMETHODCALLTYPE* AddRef)(IMstscAxInternal* This);
    ULONG(STDMETHODCALLTYPE* Release)(IMstscAxInternal* This);
    HRESULT(STDMETHODCALLTYPE* GetVChannels)(IMstscAxInternal* This, void** pChannels);
    HRESULT(STDMETHODCALLTYPE* GetCorrelationId)(IMstscAxInternal* This, GUID* pGuidCorrelationId);
} IMstscAxInternalVtbl;

struct _IMstscAxInternal
{
    IMstscAxInternalVtbl* vtbl;
};

/**
 * property entry types:
 * 1: int
 * 2: bool
 * 3: string
 * 4: binary
 * 7: IUnknown
 */

#pragma pack(push, 1)

struct tagPROPERTY_ENTRY_EX
{
    const char* propName;
    uint8_t propType;
    uint8_t padding1[23];
    uint8_t padding2[32];
    uint64_t padding3;
};
typedef struct tagPROPERTY_ENTRY_EX PROPERTY_ENTRY_EX;

#pragma pack(pop)

typedef struct _ITSPropertySet ITSPropertySet;

typedef struct ITSPropertySetVtbl
{
    HRESULT(STDMETHODCALLTYPE* QueryInterface)(ITSPropertySet* This, REFIID riid, void** ppvObject);
    ULONG(STDMETHODCALLTYPE* AddRef)(ITSPropertySet* This);
    ULONG(STDMETHODCALLTYPE* Release)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* SetProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(STDMETHODCALLTYPE* SetBoolProperty)(ITSPropertySet* This, const char* propName, bool propValue);
    HRESULT(STDMETHODCALLTYPE* SetIntProperty)(ITSPropertySet* This, const char* propName, int propValue);
    HRESULT(STDMETHODCALLTYPE* SetIUnknownProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(STDMETHODCALLTYPE* SetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(STDMETHODCALLTYPE* SetSecureStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(STDMETHODCALLTYPE* SetUlongPtrProperty)(ITSPropertySet* This, const char* propName, ULONG_PTR propValue);
} ITSPropertySetVtbl;

struct _ITSPropertySet
{
    ITSPropertySetVtbl* vtbl;
    void* INonDelegatingUnknownVtbl;
    void* TSObjectVtbl;
    const char* name;
    uint32_t marker;
    uint32_t refCount;
    void* unknown1;
    void* unknown2;
    void* unknown3;
    PROPERTY_ENTRY_EX* propMap;
    uint32_t propCount;
};

typedef struct _ITSObjectBase ITSObjectBase;

struct _ITSObjectBase
{
    IUnknown* vtbl;
    IUnknown* INonDelegatingUnknownVtbl;
    IUnknown* TSObjectVtbl;
    const char* name;
    uint32_t marker;
    uint32_t refCount;
};

typedef struct _ITSCoreApi ITSCoreApi;

typedef struct ITSCoreApiVtbl
{
    HRESULT(STDMETHODCALLTYPE* QueryInterface)(ITSCoreApi* This, REFIID riid, void** ppvObject);
    ULONG(STDMETHODCALLTYPE* AddRef)(ITSCoreApi* This);
    ULONG(STDMETHODCALLTYPE* Release)(ITSCoreApi* This);
} ITSCoreApiVtbl;

struct _ITSCoreApi
{
    ITSCoreApiVtbl* vtbl;
};

using namespace MSTSCLib;

static VOID WriteCLSID(REFCLSID rclsid)
{
    LPOLESTR polestrCLSID;
    if (StringFromCLSID(rclsid, &polestrCLSID) == S_OK)
    {
        HKEY hKey;
        LONG lStatus;
        char szSubKey[128];
        char szValue[128];
        LONG cbValue;

        _bstr_t bstrCLSID = polestrCLSID;

        sprintf(szSubKey, "CLSID\\%s", (LPCSTR)bstrCLSID);
        ZeroMemory(szValue, sizeof(szValue));
        cbValue = sizeof(szValue);
        lStatus = RegQueryValueA(HKEY_CLASSES_ROOT, szSubKey, szValue, &cbValue);
        if ((lStatus == ERROR_SUCCESS) && (strlen(szValue) > 0))
        {
            MsRdpEx_Log("--> CLSID=%s (%s)", (LPCSTR)bstrCLSID, szValue);
        }
        else
        {
            MsRdpEx_Log("--> CLSID=%s", (LPCSTR)bstrCLSID);
        }
        CoTaskMemFree(polestrCLSID);
    }
}

static VOID WriteIID(REFIID riid)
{
    LPOLESTR polestrIID;

    if (StringFromIID(riid, &polestrIID) == S_OK)
    {
        HKEY hKey;
        LONG lStatus;
        char szSubKey[128];
        char szValue[128];
        LONG cbValue;

        _bstr_t bstrIID = polestrIID;

        sprintf(szSubKey, "Interface\\%s", (LPCSTR)bstrIID);
        ZeroMemory(szValue, sizeof(szValue));
        cbValue = sizeof(szValue);
        lStatus = RegQueryValueA(HKEY_CLASSES_ROOT, szSubKey, szValue, &cbValue);
        if ((lStatus == ERROR_SUCCESS) && (strlen(szValue) > 0))
        {
            MsRdpEx_Log("--> IID=%s (%s)", (LPCSTR)bstrIID, szValue);
        }
        else
        {
            MsRdpEx_Log("--> IID=%s", (LPCSTR)bstrIID);
        }
        CoTaskMemFree(polestrIID);
    }
}

class CMsRdpExtendedSettings : public IMsRdpExtendedSettings
{
public:
    CMsRdpExtendedSettings(IUnknown* pUnknown)
    {
        m_refCount = 0;
        m_pUnknown = pUnknown;
        pUnknown->QueryInterface(IID_IMsRdpExtendedSettings, (LPVOID*)&m_pMsRdpExtendedSettings);
    }

    ~CMsRdpExtendedSettings()
    {
        m_pUnknown->Release();
        if (m_pMsRdpExtendedSettings) m_pMsRdpExtendedSettings->Release();
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr;
        MsRdpEx_Log("CMsRdpExtendedSettings::QueryInterface");
        WriteIID(riid);

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

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CMsRdpExtendedSettings::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
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

    // IMsRdpExtendedSettings
public:
    HRESULT __stdcall put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_Log("CMsRdpExtendedSettings::put_Property(%s)", propName);
        return m_pMsRdpExtendedSettings->put_Property(bstrPropertyName, pValue);
    }

    HRESULT __stdcall get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_Log("CMsRdpExtendedSettings::get_Property(%s)", propName);
        return m_pMsRdpExtendedSettings->get_Property(bstrPropertyName, pValue);
    }

private:
    ULONG m_refCount;
    IUnknown* m_pUnknown;
    IMsRdpExtendedSettings* m_pMsRdpExtendedSettings;
};

class CMsRdpClient : public IMsRdpClient10
{
public:
    CMsRdpClient(IUnknown* pUnknown)
    {
        m_refCount = 0;
        m_pUnknown = pUnknown;

        pUnknown->QueryInterface(IID_IDispatch, (LPVOID*)&m_pDispatch);
        pUnknown->QueryInterface(IID_IMsTscAx, (LPVOID*)&m_pMsTscAx);
        pUnknown->QueryInterface(IID_IMsRdpClient, (LPVOID*)&m_pMsRdpClient);
        pUnknown->QueryInterface(IID_IMsRdpClient2, (LPVOID*)&m_pMsRdpClient2);
        pUnknown->QueryInterface(IID_IMsRdpClient3, (LPVOID*)&m_pMsRdpClient3);
        pUnknown->QueryInterface(IID_IMsRdpClient4, (LPVOID*)&m_pMsRdpClient4);
        pUnknown->QueryInterface(IID_IMsRdpClient5, (LPVOID*)&m_pMsRdpClient5);
        pUnknown->QueryInterface(IID_IMsRdpClient6, (LPVOID*)&m_pMsRdpClient6);
        pUnknown->QueryInterface(IID_IMsRdpClient7, (LPVOID*)&m_pMsRdpClient7);
        pUnknown->QueryInterface(IID_IMsRdpClient8, (LPVOID*)&m_pMsRdpClient8);
        pUnknown->QueryInterface(IID_IMsRdpClient9, (LPVOID*)&m_pMsRdpClient9);
        pUnknown->QueryInterface(IID_IMsRdpClient10, (LPVOID*)&m_pMsRdpClient10);

        m_MsRdpExtendedSettings = new CMsRdpExtendedSettings(pUnknown);
    }

    ~CMsRdpClient()
    {
        m_pUnknown->Release();
        if (m_pDispatch) m_pDispatch->Release();
        if (m_pMsTscAx) m_pMsTscAx->Release();
        if (m_pMsRdpClient) m_pMsRdpClient->Release();
        if (m_pMsRdpClient2) m_pMsRdpClient2->Release();
        if (m_pMsRdpClient3) m_pMsRdpClient3->Release();
        if (m_pMsRdpClient4) m_pMsRdpClient4->Release();
        if (m_pMsRdpClient5) m_pMsRdpClient5->Release();
        if (m_pMsRdpClient6) m_pMsRdpClient6->Release();
        if (m_pMsRdpClient7) m_pMsRdpClient7->Release();
        if (m_pMsRdpClient8) m_pMsRdpClient8->Release();
        if (m_pMsRdpClient9) m_pMsRdpClient9->Release();
        if (m_pMsRdpClient10) m_pMsRdpClient10->Release();

        delete m_MsRdpExtendedSettings;
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr;
        MsRdpEx_Log("CMsRdpClient::QueryInterface");
        WriteIID(riid);

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IDispatch) && m_pDispatch)
        {
            *ppvObject = (LPVOID)((IDispatch*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsTscAx) && m_pMsTscAx)
        {
            *ppvObject = (LPVOID)((IMsTscAx*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient) && m_pMsRdpClient)
        {
            *ppvObject = (LPVOID)((IMsRdpClient*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient2) && m_pMsRdpClient2)
        {
            *ppvObject = (LPVOID)((IMsRdpClient2*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient3) && m_pMsRdpClient3)
        {
            *ppvObject = (LPVOID)((IMsRdpClient3*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient4) && m_pMsRdpClient4)
        {
            *ppvObject = (LPVOID)((IMsRdpClient4*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient5) && m_pMsRdpClient5)
        {
            *ppvObject = (LPVOID)((IMsRdpClient5*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient6) && m_pMsRdpClient6)
        {
            *ppvObject = (LPVOID)((IMsRdpClient6*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient7) && m_pMsRdpClient7)
        {
            *ppvObject = (LPVOID)((IMsRdpClient7*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient8) && m_pMsRdpClient8)
        {
            *ppvObject = (LPVOID)((IMsRdpClient8*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient9) && m_pMsRdpClient9)
        {
            *ppvObject = (LPVOID)((IMsRdpClient9*)this);
            m_refCount++;
            return S_OK;
        }
        if ((riid == IID_IMsRdpClient10) && m_pMsRdpClient10)
        {
            *ppvObject = (LPVOID)((IMsRdpClient10*)this);
            m_refCount++;
            return S_OK;
        }

        if ((riid == IID_IMsRdpExtendedSettings) && m_MsRdpExtendedSettings)
        {
            *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)m_MsRdpExtendedSettings);
            return S_OK;
        }

        hr = m_pUnknown->QueryInterface(riid, ppvObject);
        MsRdpEx_Log("--> hr=%x", hr);
        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CMsRdpClient::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_Log("CMsRdpClient::Release");
        if (--m_refCount == 0)
        {
            MsRdpEx_Log("--> deleting object");
            delete this;
            return 0;
        }
        MsRdpEx_Log("--> refCount=%d", m_refCount);
        return m_refCount;
    }

    // IDispatch interface
public:
    HRESULT STDMETHODCALLTYPE GetTypeInfoCount(__RPC__out UINT* pctinfo)
    {
        MsRdpEx_Log("CMsRdpClient::GetTypeInfoCount");
        return m_pDispatch->GetTypeInfoCount(pctinfo);
    }

    HRESULT STDMETHODCALLTYPE GetTypeInfo(
        UINT iTInfo,
        LCID lcid,
        ITypeInfo** ppTInfo)
    {
        MsRdpEx_Log("CMsRdpClient::GetTypeInfo");
        return m_pDispatch->GetTypeInfo(iTInfo, lcid, ppTInfo);
    }

    HRESULT STDMETHODCALLTYPE GetIDsOfNames(
        REFIID riid,
        LPOLESTR* rgszNames,
        UINT cNames,
        LCID lcid,
        DISPID* rgDispId)
    {
        MsRdpEx_Log("CMsRdpClient::GetIDsOfNames");
        return m_pDispatch->GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId);
    }

    HRESULT STDMETHODCALLTYPE Invoke(
        DISPID dispIdMember,
        REFIID riid,
        LCID lcid,
        WORD wFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        UINT* puArgErr)
    {
        MsRdpEx_Log("CMsRdpClient::Invoke");
        return m_pDispatch->Invoke(dispIdMember, riid, lcid, wFlags, pDispParams, pVarResult, pExcepInfo, puArgErr);
    }

    // IMsTscAx interface
public:
    HRESULT __stdcall put_Server(BSTR pServer) {
        return m_pMsTscAx->put_Server(pServer);
    }

    HRESULT __stdcall get_Server(BSTR* pServer) {
        return m_pMsTscAx->get_Server(pServer);
    }

    HRESULT __stdcall put_Domain(BSTR pDomain) {
        return m_pMsTscAx->put_Domain(pDomain);
    }

    HRESULT __stdcall get_Domain(BSTR* pDomain) {
        return m_pMsTscAx->get_Domain(pDomain);
    }

    HRESULT __stdcall put_UserName(BSTR pUserName) {
        return m_pMsTscAx->put_UserName(pUserName);
    }

    HRESULT __stdcall get_UserName(BSTR* pUserName) {
        return m_pMsTscAx->get_UserName(pUserName);
    }

    HRESULT __stdcall put_DisconnectedText(BSTR pDisconnectedText) {
        return m_pMsTscAx->put_DisconnectedText(pDisconnectedText);
    }

    HRESULT __stdcall get_DisconnectedText(BSTR* pDisconnectedText) {
        return m_pMsTscAx->get_DisconnectedText(pDisconnectedText);
    }

    HRESULT __stdcall put_ConnectingText(BSTR pConnectingText) {
        return m_pMsTscAx->put_ConnectingText(pConnectingText);
    }

    HRESULT __stdcall get_ConnectingText(BSTR* pConnectingText) {
        return m_pMsTscAx->get_ConnectingText(pConnectingText);
    }

    HRESULT __stdcall get_Connected(short* pIsConnected) {
        return m_pMsTscAx->get_Connected(pIsConnected);
    }

    HRESULT __stdcall put_DesktopWidth(long pVal) {
        return m_pMsTscAx->put_DesktopWidth(pVal);
    }

    HRESULT __stdcall get_DesktopWidth(long* pVal) {
        return m_pMsTscAx->get_DesktopWidth(pVal);
    }

    HRESULT __stdcall put_DesktopHeight(long pVal) {
        return m_pMsTscAx->put_DesktopHeight(pVal);
    }

    HRESULT __stdcall get_DesktopHeight(long* pVal) {
        return m_pMsTscAx->get_DesktopHeight(pVal);
    }

    HRESULT __stdcall put_StartConnected(long pfStartConnected) {
        return m_pMsTscAx->put_StartConnected(pfStartConnected);
    }

    HRESULT __stdcall get_StartConnected(long* pfStartConnected) {
        return m_pMsTscAx->get_StartConnected(pfStartConnected);
    }

    HRESULT __stdcall get_HorizontalScrollBarVisible(long* pfHScrollVisible) {
        return m_pMsTscAx->get_HorizontalScrollBarVisible(pfHScrollVisible);
    }

    HRESULT __stdcall get_VerticalScrollBarVisible(long* pfVScrollVisible) {
        return m_pMsTscAx->get_VerticalScrollBarVisible(pfVScrollVisible);
    }

    HRESULT __stdcall put_FullScreenTitle(BSTR _arg1) {
        return m_pMsTscAx->put_FullScreenTitle(_arg1);
    }

    HRESULT __stdcall get_CipherStrength(long* pCipherStrength) {
        return m_pMsTscAx->get_CipherStrength(pCipherStrength);
    }

    HRESULT __stdcall get_Version(BSTR* pVersion) {
        return m_pMsTscAx->get_Version(pVersion);
    }

    HRESULT __stdcall get_SecuredSettingsEnabled(long* pSecuredSettingsEnabled) {
        return m_pMsTscAx->get_SecuredSettingsEnabled(pSecuredSettingsEnabled);
    }

    HRESULT __stdcall get_SecuredSettings(struct IMsTscSecuredSettings** ppSecuredSettings) {
        return m_pMsTscAx->get_SecuredSettings(ppSecuredSettings);
    }

    HRESULT __stdcall get_AdvancedSettings(struct IMsTscAdvancedSettings** ppAdvSettings) {
        return m_pMsTscAx->get_AdvancedSettings(ppAdvSettings);
    }

    HRESULT __stdcall get_Debugger(struct IMsTscDebug** ppDebugger) {
        return m_pMsTscAx->get_Debugger(ppDebugger);
    }

    HRESULT __stdcall raw_Connect() {
        HRESULT hr;
        MsRdpEx_Log("CMsRdpClient::Connect");

        CMsRdpExtendedSettings* pMsRdpExtendedSettings = m_MsRdpExtendedSettings;

        IMsRdpClientNonScriptable3* pMsRdpClientNonScriptable3 = NULL;
        hr = m_pMsTscAx->QueryInterface(IID_IMsRdpClientNonScriptable3, (LPVOID*)&pMsRdpClientNonScriptable3);

        IMstscAxInternal* pMstscAxInternal = NULL;
        hr = m_pMsTscAx->QueryInterface(IID_IMstscAxInternal, (LPVOID*)&pMstscAxInternal);

        size_t memStatus;
        MEMORY_BASIC_INFORMATION memInfo;

        size_t pMsTscAx = (size_t)(void*)m_pMsTscAx;
        size_t maxPtrCount = 1200;

        memStatus = VirtualQuery((void*)m_pMsTscAx, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));

        if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize > 16)) {
            maxPtrCount = (memInfo.RegionSize / 8);
            MsRdpEx_Log("pMsTscAx: 0x%08X, size: %d ptrCount: %d", (size_t)pMsTscAx, memInfo.RegionSize, (int)maxPtrCount);
        }

        ITSPropertySet* pTSPropertySet1 = NULL;
        ITSPropertySet* pTSPropertySet2 = NULL;

        for (int i = 0; i < maxPtrCount; i++) {
            ITSObjectBase** ppTSObject = (ITSObjectBase**) &((size_t*)m_pMsTscAx)[i];
            memStatus = VirtualQuery(ppTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
            if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize >= 8)) {
                ITSObjectBase* pTSObject = *ppTSObject;
                if (pTSObject) {
                    memStatus = VirtualQuery(pTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
                    if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize > 16)) {
                        if (pTSObject->marker == 0xDBCAABCD) {
                            MsRdpEx_Log("MsTscAx(%d): 0x%08X name: %s refCount: %d",
                                i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                            if (!strcmp(pTSObject->name, "CTSPropertySet")) {
                                if (!pTSPropertySet1) {
                                    pTSPropertySet1 = (ITSPropertySet*)pTSObject;
                                } else if (!pTSPropertySet2 && ((void*) pTSPropertySet1 != pTSObject)) {
                                    pTSPropertySet2 = (ITSPropertySet*) pTSObject;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (pTSPropertySet1) {
            hr = pTSPropertySet1->vtbl->QueryInterface(pTSPropertySet1, IID_ITSPropertySet, (LPVOID*)&pTSPropertySet1);
            MsRdpEx_Log("TSPropertySet1::QueryInterface(IID_ITSPropertySet) %s!", (hr == S_OK) ? "success" : "failure");
        }

        if (pTSPropertySet2) {
            hr = pTSPropertySet2->vtbl->QueryInterface(pTSPropertySet2, IID_ITSPropertySet, (LPVOID*)&pTSPropertySet2);
            MsRdpEx_Log("TSPropertySet2::QueryInterface(IID_ITSPropertySet) %s!", (hr == S_OK) ? "success" : "failure");
        }

        if (pTSPropertySet1) {
            hr = pTSPropertySet1->vtbl->SetStringProperty(pTSPropertySet1, "UserSpecifiedServerName", L"yolo.contoso.com");
            MsRdpEx_Log("ITSPropertySet1::SetStringProperty(UserSpecifiedServerName) %s!", (hr == S_OK) ? "success" : "failure");

            hr = pTSPropertySet2->vtbl->SetBoolProperty(pTSPropertySet2, "Fullscreen", true);
            MsRdpEx_Log("ITSPropertySet1::SetBoolProperty(Fullscreen) %s!", (hr == S_OK) ? "success" : "failure");
        }

        if (pTSPropertySet1) {
            uint32_t propCount = pTSPropertySet1->propCount;
            PROPERTY_ENTRY_EX* propMap = pTSPropertySet1->propMap;

            MsRdpEx_Log("TSPropertySet1(%d)", propCount);

            for (int i = 0; i < propCount; i++)
            {
                PROPERTY_ENTRY_EX* prop = &propMap[i];
                MsRdpEx_Log("name: %s type: %d", prop->propName, (int)prop->propType);
            }
        }

        if (pTSPropertySet2) {
            uint32_t propCount = pTSPropertySet2->propCount;
            PROPERTY_ENTRY_EX* propMap = pTSPropertySet2->propMap;

            MsRdpEx_Log("TSPropertySet2(%d)", propCount);

            for (int i = 0; i < propCount; i++)
            {
                PROPERTY_ENTRY_EX* prop = &propMap[i];
                MsRdpEx_Log("name: %s type: %d", prop->propName, (int)prop->propType);
            }
        }

        if (pMstscAxInternal)
        {
            GUID guidCorrelationId = GUID_NULL;
            char correlationId[MSRDPEX_GUID_STRING_SIZE];

            hr = pMstscAxInternal->vtbl->GetCorrelationId(pMstscAxInternal, &guidCorrelationId);
            MsRdpEx_GuidBinToStr(&guidCorrelationId, correlationId, 0);
            MsRdpEx_Log("CorrelationId: %s", correlationId);
        }

        char* filename = MsRdpEx_GetRdpFilenameFromCommandLine();

        if (filename) {
            MsRdpEx_Log("Loading %s RDP", filename);
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
                    else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ClearTextPassword")) {
                        bstr_t ClearTextPassword = _com_util::ConvertStringToBSTR(entry->value);
                        pMsRdpClientNonScriptable3->PutClearTextPassword(ClearTextPassword);
                    }
                    else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ConnectionBarText")) {
                        bstr_t ConnectionBarText = _com_util::ConvertStringToBSTR(entry->value);
                        pMsRdpClientNonScriptable3->PutConnectionBarText(ConnectionBarText);
                    }
                    else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ServerNameUsedForAuthentication")) {
                        char* oldServerName = NULL;
                        bstr_t ServerNameUsedForAuthentication = _com_util::ConvertStringToBSTR(entry->value);
                        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, m_pMsTscAx->GetServer().GetBSTR(), -1, &oldServerName, 0, NULL, NULL);
                        m_pMsTscAx->PutServer(ServerNameUsedForAuthentication);
                        MsRdpEx_NameResolver_RemapName(entry->value, oldServerName);
                    }
                }

                MsRdpEx_ArrayListIt_Finish(it);
            }
            MsRdpEx_RdpFile_Free(rdpFile);
            free(filename);
        }

        if (pMsRdpClientNonScriptable3) {
            pMsRdpClientNonScriptable3->Release();
        }

        if (pMstscAxInternal) {
            pMstscAxInternal->vtbl->Release(pMstscAxInternal);
        }

        //DumpMsTscProperties(m_pUnknown);
        return m_pMsTscAx->raw_Connect();
    }

    HRESULT __stdcall raw_Disconnect() {
        return m_pMsTscAx->raw_Disconnect();
    }

    HRESULT __stdcall raw_CreateVirtualChannels(BSTR newVal) {
        return m_pMsTscAx->raw_CreateVirtualChannels(newVal);
    }

    HRESULT __stdcall raw_SendOnVirtualChannel(BSTR chanName, BSTR ChanData) {
        return m_pMsTscAx->raw_SendOnVirtualChannel(chanName, ChanData);
    }

    // IMsRdpClient interface
public:
    HRESULT __stdcall put_ColorDepth(long pcolorDepth) {
        return m_pMsRdpClient->put_ColorDepth(pcolorDepth);
    }

    HRESULT __stdcall get_ColorDepth(long* pcolorDepth) {
        return m_pMsRdpClient->get_ColorDepth(pcolorDepth);
    }

    HRESULT __stdcall get_AdvancedSettings2(struct IMsRdpClientAdvancedSettings** ppAdvSettings) {
        return m_pMsRdpClient->get_AdvancedSettings2(ppAdvSettings);
    }

    HRESULT __stdcall get_SecuredSettings2(struct IMsRdpClientSecuredSettings** ppSecuredSettings) {
        return m_pMsRdpClient->get_SecuredSettings2(ppSecuredSettings);
    }

    HRESULT __stdcall get_ExtendedDisconnectReason(ExtendedDisconnectReasonCode* pExtendedDisconnectReason) {
        return m_pMsRdpClient->get_ExtendedDisconnectReason(pExtendedDisconnectReason);
    }

    HRESULT __stdcall put_FullScreen(VARIANT_BOOL pfFullScreen) {
        return m_pMsRdpClient->put_FullScreen(pfFullScreen);
    }

    HRESULT __stdcall get_FullScreen(VARIANT_BOOL* pfFullScreen) {
        return m_pMsRdpClient->get_FullScreen(pfFullScreen);
    }

    HRESULT __stdcall raw_SetVirtualChannelOptions(BSTR chanName, long chanOptions) {
        return m_pMsRdpClient->raw_SetVirtualChannelOptions(chanName, chanOptions);
    }

    HRESULT __stdcall raw_GetVirtualChannelOptions(BSTR chanName, long* pChanOptions) {
        return m_pMsRdpClient->raw_GetVirtualChannelOptions(chanName, pChanOptions);
    }

    HRESULT __stdcall raw_RequestClose(ControlCloseStatus* pCloseStatus) {
        return m_pMsRdpClient->raw_RequestClose(pCloseStatus);
    }

    // IMsRdpClient2 interface
public:
    HRESULT __stdcall get_AdvancedSettings3(struct IMsRdpClientAdvancedSettings2** ppAdvSettings) {
        return m_pMsRdpClient2->get_AdvancedSettings3(ppAdvSettings);
    }

    HRESULT __stdcall put_ConnectedStatusText(BSTR pConnectedStatusText) {
        return m_pMsRdpClient2->put_ConnectedStatusText(pConnectedStatusText);
    }

    HRESULT __stdcall get_ConnectedStatusText(BSTR* pConnectedStatusText) {
        return m_pMsRdpClient2->get_ConnectedStatusText(pConnectedStatusText);
    }

    // IMsRdpClient3 interface
public:
    HRESULT __stdcall get_AdvancedSettings4(struct IMsRdpClientAdvancedSettings3** ppAdvSettings) {
        return m_pMsRdpClient3->get_AdvancedSettings4(ppAdvSettings);
    }

    // IMsRdpClient4 interface
public:
    HRESULT __stdcall get_AdvancedSettings5(struct IMsRdpClientAdvancedSettings4** ppAdvSettings) {
        return m_pMsRdpClient4->get_AdvancedSettings5(ppAdvSettings);
    }

    // IMsRdpClient5 interface
public:
    HRESULT __stdcall get_TransportSettings(struct IMsRdpClientTransportSettings** ppXportSet) {
        return m_pMsRdpClient5->get_TransportSettings(ppXportSet);
    }

    HRESULT __stdcall get_AdvancedSettings6(struct IMsRdpClientAdvancedSettings5** ppAdvSettings) {
        return m_pMsRdpClient5->get_AdvancedSettings6(ppAdvSettings);
    }

    HRESULT __stdcall raw_GetErrorDescription(
        unsigned int disconnectReason,
        unsigned int ExtendedDisconnectReason,
        BSTR* pBstrErrorMsg
    ) {
        return m_pMsRdpClient5->raw_GetErrorDescription(disconnectReason, ExtendedDisconnectReason, pBstrErrorMsg);
    }

    HRESULT __stdcall get_RemoteProgram(struct ITSRemoteProgram** ppRemoteProgram) {
        return m_pMsRdpClient5->get_RemoteProgram(ppRemoteProgram);
    }

    HRESULT __stdcall get_MsRdpClientShell(struct IMsRdpClientShell** ppLauncher) {
        return m_pMsRdpClient5->get_MsRdpClientShell(ppLauncher);
    }

    // IMsRdpClient6 interface
public:
    HRESULT __stdcall get_AdvancedSettings7(struct IMsRdpClientAdvancedSettings6** ppAdvSettings) {
        return m_pMsRdpClient6->get_AdvancedSettings7(ppAdvSettings);
    }

    HRESULT __stdcall get_TransportSettings2(struct IMsRdpClientTransportSettings2** ppXportSet2) {
        return m_pMsRdpClient6->get_TransportSettings2(ppXportSet2);
    }

    // IMsRdpClient7 interface
public:
    HRESULT __stdcall get_AdvancedSettings8(struct IMsRdpClientAdvancedSettings7** ppAdvSettings) {
        return m_pMsRdpClient7->get_AdvancedSettings8(ppAdvSettings);
    }

    HRESULT __stdcall get_TransportSettings3(struct IMsRdpClientTransportSettings3** ppXportSet3) {
        return m_pMsRdpClient7->get_TransportSettings3(ppXportSet3);
    }

    HRESULT __stdcall raw_GetStatusText(
        unsigned int statusCode,
        BSTR* pBstrStatusText
    ) {
        return m_pMsRdpClient7->raw_GetStatusText(statusCode, pBstrStatusText);
    }

    HRESULT __stdcall get_SecuredSettings3(struct IMsRdpClientSecuredSettings2** ppSecuredSettings) {
        return m_pMsRdpClient7->get_SecuredSettings3(ppSecuredSettings);
    }

    HRESULT __stdcall get_RemoteProgram2(struct ITSRemoteProgram2** ppRemoteProgram) {
        return m_pMsRdpClient7->get_RemoteProgram2(ppRemoteProgram);
    }

    // IMsRdpClient8 interface
public:
    HRESULT __stdcall raw_SendRemoteAction(RemoteSessionActionType actionType) {
        return m_pMsRdpClient8->raw_SendRemoteAction(actionType);
    }

    HRESULT __stdcall get_AdvancedSettings9(struct IMsRdpClientAdvancedSettings8** ppAdvSettings) {
        return m_pMsRdpClient8->get_AdvancedSettings9(ppAdvSettings);
    }

    HRESULT __stdcall raw_Reconnect(
        unsigned long ulWidth,
        unsigned long ulHeight,
        ControlReconnectStatus* pReconnectStatus
    ) {
        return m_pMsRdpClient8->raw_Reconnect(ulWidth, ulHeight, pReconnectStatus);
    }

    // IMsRdpClient9 interface
public:
    HRESULT __stdcall get_TransportSettings4(
        struct IMsRdpClientTransportSettings4** ppXportSet4) {
        return m_pMsRdpClient9->get_TransportSettings4(ppXportSet4);
    }

    HRESULT __stdcall raw_SyncSessionDisplaySettings() {
        return m_pMsRdpClient9->raw_SyncSessionDisplaySettings();
    }

    HRESULT __stdcall raw_UpdateSessionDisplaySettings (
        unsigned long ulDesktopWidth, unsigned long ulDesktopHeight,
        unsigned long ulPhysicalWidth, unsigned long ulPhysicalHeight,
        unsigned long ulOrientation,
        unsigned long ulDesktopScaleFactor,
        unsigned long ulDeviceScaleFactor) {
        return m_pMsRdpClient9->raw_UpdateSessionDisplaySettings(
            ulDesktopWidth, ulDesktopHeight,
            ulPhysicalWidth, ulPhysicalHeight,
            ulOrientation,
            ulDesktopScaleFactor,
            ulDeviceScaleFactor);
    }

    HRESULT __stdcall raw_attachEvent(BSTR eventName, IDispatch* callback) {
        return m_pMsRdpClient9->raw_attachEvent(eventName, callback);
    }

    HRESULT __stdcall raw_detachEvent(BSTR eventName, IDispatch* callback) {
        return m_pMsRdpClient9->raw_detachEvent(eventName, callback);
    }

    // IMsRdpClient10 interface
public:
    HRESULT __stdcall get_RemoteProgram3(struct ITSRemoteProgram3** ppRemoteProgram) {
        return m_pMsRdpClient10->get_RemoteProgram3(ppRemoteProgram);
    }

private:
    ULONG m_refCount;
    IUnknown* m_pUnknown;
    IDispatch* m_pDispatch;
    IMsTscAx* m_pMsTscAx;
    IMsRdpClient* m_pMsRdpClient;
    IMsRdpClient2* m_pMsRdpClient2;
    IMsRdpClient3* m_pMsRdpClient3;
    IMsRdpClient4* m_pMsRdpClient4;
    IMsRdpClient5* m_pMsRdpClient5;
    IMsRdpClient6* m_pMsRdpClient6;
    IMsRdpClient7* m_pMsRdpClient7;
    IMsRdpClient8* m_pMsRdpClient8;
    IMsRdpClient9* m_pMsRdpClient9;
    IMsRdpClient10* m_pMsRdpClient10;
    CMsRdpExtendedSettings* m_MsRdpExtendedSettings;
};

class CClassFactory : IClassFactory
{
public:
    CClassFactory(REFCLSID rclsid, IClassFactory* pDelegate)
    {
        m_clsid = rclsid;
        m_pDelegate = pDelegate;
        m_refCount = 1;
    }

    ~CClassFactory()
    {
        m_pDelegate->Release();
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        MsRdpEx_Log("CClassFactory::QueryInterface(%s)", iid);

        if (riid == IID_IUnknown) {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        if (riid == IID_IClassFactory) {
            *ppvObject = (LPVOID)((IClassFactory*)this);
            m_refCount++;
            return S_OK;
        }

        return m_pDelegate->QueryInterface(riid, ppvObject);
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CClassFactory::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_Log("CClassFactory::Release");
        if (--m_refCount == 0)
        {
            delete this;
            return 0;
        }
        return m_refCount;
    }

    // IClassFactory interface
public:
    HRESULT STDMETHODCALLTYPE CreateInstance(
        IUnknown* pUnkOuter,
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        MsRdpEx_Log("CClassFactory::CreateInstance(%s)", iid);
        WriteCLSID(m_clsid);
        WriteIID(riid);

        HRESULT hr = m_pDelegate->CreateInstance(pUnkOuter, riid, ppvObject);

        if (hr == S_OK)
        {
            if ((m_clsid == CLSID_MsRdpClientNotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient2NotSafeForScripting) || 
                (m_clsid == CLSID_MsRdpClient3NotSafeForScripting) || 
                (m_clsid == CLSID_MsRdpClient4NotSafeForScripting) || 
                (m_clsid == CLSID_MsRdpClient5NotSafeForScripting) || 
                (m_clsid == CLSID_MsRdpClient6NotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient7NotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient8NotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient9NotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient10NotSafeForScripting) ||
                (m_clsid == CLSID_MsRdpClient11NotSafeForScripting))
            {
                CMsRdpClient* pMsRdpClient = new CMsRdpClient((IUnknown*)*ppvObject);
                hr = pMsRdpClient->QueryInterface(riid, ppvObject);
            }
        }

        return hr;
    }

    HRESULT STDMETHODCALLTYPE LockServer(
        BOOL fLock
    )
    {
        MsRdpEx_Log("CClassFactory::LockServer");
        return m_pDelegate->LockServer(fLock);
    }

private:
    CLSID m_clsid;
    IClassFactory* m_pDelegate;
    ULONG m_refCount;
};

void* CDECL MsRdpEx_CClassFactory_New(REFCLSID rclsid, IClassFactory* pDelegate)
{
    return (void*) new CClassFactory(rclsid, pDelegate);
}
