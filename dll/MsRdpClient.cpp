
#include "MsRdpClient.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/RdpCoreApi.h>
#include <MsRdpEx/RdpProcess.h>
#include <MsRdpEx/RdpInstance.h>
#include <MsRdpEx/RdpSettings.h>
#include <MsRdpEx/NameResolver.h>

#include "TSObjects.h"

#pragma warning (disable : 26812)

#include "mstscax.tli"

extern "C" const GUID __declspec(selectany) IID_ITSInstance =
    { 0x7272B11A,0xC627,0x40DC,{0xBB,0x13,0x57,0xDA,0x13,0xC3,0x95,0xF0} };
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

extern "C" const GUID __declspec(selectany) IID_IMsRdpExCoreApi = // 13F6E86F-EE7D-44D1-AA94-1136B784441D
    { 0x13F6E86F,0xEE7D,0x44D1,{0xAA,0x94,0x11,0x36,0xB7,0x84,0x44,0x1D} };
extern "C" const GUID __declspec(selectany) IID_IMsRdpExProcess = // 338784B3-3363-45A2-8ECD-80A65DBAF636
    { 0x338784B3,0x3363,0x45A2,{0x8E,0xCD,0x80,0xA6,0x5D,0xBA,0xF6,0x36} };
extern "C" const GUID __declspec(selectany) IID_IMsRdpExInstance = // 94CDA65A-EFDF-4453-B8B2-2493A12D31C7
    { 0x94CDA65A,0xEFDF,0x4453,{0xB8,0xB2,0x24,0x93,0xA1,0x2D,0x31,0xC7} };

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

class CMsRdpClient;

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
            MsRdpEx_LogPrint(DEBUG, "--> CLSID=%s (%s)", (LPCSTR)bstrCLSID, szValue);
        }
        else
        {
            MsRdpEx_LogPrint(DEBUG, "--> CLSID=%s", (LPCSTR)bstrCLSID);
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
            MsRdpEx_LogPrint(DEBUG, "--> IID=%s (%s)", (LPCSTR)bstrIID, szValue);
        }
        else
        {
            MsRdpEx_LogPrint(DEBUG, "--> IID=%s", (LPCSTR)bstrIID);
        }
        CoTaskMemFree(polestrIID);
    }
}

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

        m_pMsRdpExInstance = CMsRdpExInstance_New(this);
        IMsRdpExInstance* pMsRdpExInstance = (IMsRdpExInstance*)m_pMsRdpExInstance;
        pMsRdpExInstance->AddRef();
        pMsRdpExInstance->GetSessionId(&m_sessionId);
        MsRdpEx_InstanceManager_Add(m_pMsRdpExInstance);

        m_pMsRdpExtendedSettings = CMsRdpExtendedSettings_New(pUnknown, (IUnknown*)m_pMsTscAx, &m_sessionId);
        IMsRdpExtendedSettings* pMsRdpExtendedSettings = (IMsRdpExtendedSettings*)m_pMsRdpExtendedSettings;
        pMsRdpExtendedSettings->AddRef();
        pMsRdpExInstance->AttachExtendedSettings(m_pMsRdpExtendedSettings);

        void* pCorePropsRaw = NULL;
        m_pMsRdpExtendedSettings->GetCorePropsRawPtr(&pCorePropsRaw);
        ((IMsRdpExInstance*)m_pMsRdpExInstance)->SetCorePropsRawPtr(pCorePropsRaw);
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

        if (m_pMsRdpExtendedSettings) {
            IMsRdpExtendedSettings* pMsRdpExtendedSettings = (IMsRdpExtendedSettings*) m_pMsRdpExtendedSettings;
            pMsRdpExtendedSettings->Release();
        }
        
        if (m_pMsRdpExInstance) {
            MsRdpEx_InstanceManager_Remove(m_pMsRdpExInstance);
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

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IDispatch) && m_pDispatch)
        {
            *ppvObject = (LPVOID)((IDispatch*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsTscAx) && m_pMsTscAx)
        {
            *ppvObject = (LPVOID)((IMsTscAx*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient) && m_pMsRdpClient)
        {
            *ppvObject = (LPVOID)((IMsRdpClient*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient2) && m_pMsRdpClient2)
        {
            *ppvObject = (LPVOID)((IMsRdpClient2*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient3) && m_pMsRdpClient3)
        {
            *ppvObject = (LPVOID)((IMsRdpClient3*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient4) && m_pMsRdpClient4)
        {
            *ppvObject = (LPVOID)((IMsRdpClient4*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient5) && m_pMsRdpClient5)
        {
            *ppvObject = (LPVOID)((IMsRdpClient5*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient6) && m_pMsRdpClient6)
        {
            *ppvObject = (LPVOID)((IMsRdpClient6*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient7) && m_pMsRdpClient7)
        {
            *ppvObject = (LPVOID)((IMsRdpClient7*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient8) && m_pMsRdpClient8)
        {
            *ppvObject = (LPVOID)((IMsRdpClient8*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient9) && m_pMsRdpClient9)
        {
            *ppvObject = (LPVOID)((IMsRdpClient9*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpClient10) && m_pMsRdpClient10)
        {
            *ppvObject = (LPVOID)((IMsRdpClient10*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpExtendedSettings) && m_pMsRdpExtendedSettings)
        {
            IMsRdpExtendedSettings* pMsRdpExtendedSettings = (IMsRdpExtendedSettings*) m_pMsRdpExtendedSettings;
            hr = pMsRdpExtendedSettings->QueryInterface(IID_IMsRdpExtendedSettings, ppvObject);
        }
        else if ((riid == IID_IMsRdpExInstance) && m_pMsRdpExInstance)
        {
            IMsRdpExInstance* pMsRdpExInstance = (IMsRdpExInstance*) m_pMsRdpExInstance;
            hr = pMsRdpExInstance->QueryInterface(IID_IMsRdpExInstance, ppvObject);
        }
        else
        {
            hr = m_pUnknown->QueryInterface(riid, ppvObject);
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);
        WriteIID(riid);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IDispatch interface
public:
    HRESULT STDMETHODCALLTYPE GetTypeInfoCount(__RPC__out UINT* pctinfo)
    {
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::GetTypeInfoCount");
        return m_pDispatch->GetTypeInfoCount(pctinfo);
    }

    HRESULT STDMETHODCALLTYPE GetTypeInfo(
        UINT iTInfo,
        LCID lcid,
        ITypeInfo** ppTInfo)
    {
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::GetTypeInfo");
        return m_pDispatch->GetTypeInfo(iTInfo, lcid, ppTInfo);
    }

    HRESULT STDMETHODCALLTYPE GetIDsOfNames(
        REFIID riid,
        LPOLESTR* rgszNames,
        UINT cNames,
        LCID lcid,
        DISPID* rgDispId)
    {
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::GetIDsOfNames");
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
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::Invoke");
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
        MsRdpEx_LogPrint(DEBUG, "CMsRdpClient::Connect");

        CMsRdpExtendedSettings* pMsRdpExtendedSettings = m_pMsRdpExtendedSettings;

        m_pMsRdpExtendedSettings->LoadRdpFile(NULL);
        m_pMsRdpExtendedSettings->PrepareSspiSessionIdHack();

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
    GUID m_sessionId;
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
    CMsRdpExInstance* m_pMsRdpExInstance;
    CMsRdpExtendedSettings* m_pMsRdpExtendedSettings;
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

        MsRdpEx_LogPrint(DEBUG, "CClassFactory::QueryInterface(%s)", iid);

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
        MsRdpEx_LogPrint(DEBUG, "CClassFactory::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_LogPrint(DEBUG, "CClassFactory::Release");
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

        MsRdpEx_LogPrint(DEBUG, "CClassFactory::CreateInstance(%s)", iid);
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
        MsRdpEx_LogPrint(DEBUG, "CClassFactory::LockServer");
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
