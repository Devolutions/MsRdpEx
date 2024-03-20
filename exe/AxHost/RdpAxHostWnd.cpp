#include <atlbase.h>
#include <atlwin.h>
#include <atlhost.h>

#include <commctrl.h>
#pragma comment(lib, "ComCtl32.lib")

#include "../com/mstscax.tlh"
using namespace MSTSCLib;

#include "DpiHelper.h"

#include <MsRdpEx/RdpFile.h>

#include <MsRdpEx/MsRdpEx.h>

CComModule _Module;

static HWND GetParentWindowHandle()
{
    char buffer[32];
    HWND hWndParent = 0;

    if (GetEnvironmentVariableA("MSRDPEX_PARENT_WINDOW_HANDLE", buffer, sizeof(buffer)) > 0)
    {
        hWndParent = (HWND)_strtoui64(buffer, NULL, 0);
    }

    return hWndParent;
}

#define IMsTscAxEvents_OnConnectingId 0x00000001
#define IMsTscAxEvents_OnConnectedId 0x00000002
#define IMsTscAxEvents_OnLoginCompleteId 0x00000003
#define IMsTscAxEvents_OnDisconnectedId 0x00000004
#define IMsTscAxEvents_OnEnterFullScreenModeId 0x00000005
#define IMsTscAxEvents_OnLeaveFullScreenModeId 0x00000006
#define IMsTscAxEvents_OnChannelReceivedDataId 0x00000007
#define IMsTscAxEvents_OnRequestGoFullScreenId 0x00000008
#define IMsTscAxEvents_OnRequestLeaveFullScreenId 0x00000009
#define IMsTscAxEvents_OnFatalErrorId 0x0000000a
#define IMsTscAxEvents_OnWarningId 0x0000000b
#define IMsTscAxEvents_OnRemoteDesktopSizeChangeId 0x0000000c
#define IMsTscAxEvents_OnIdleTimeoutNotificationId 0x0000000d
#define IMsTscAxEvents_OnRequestContainerMinimizeId 0x0000000e
#define IMsTscAxEvents_OnConfirmCloseId 0x0000000f
#define IMsTscAxEvents_OnReceivedTSPublicKeyId 0x00000010
#define IMsTscAxEvents_OnAutoReconnectingId 0x00000011
#define IMsTscAxEvents_OnAuthenticationWarningDisplayedId 0x00000012
#define IMsTscAxEvents_OnAuthenticationWarningDismissedId 0x00000013
#define IMsTscAxEvents_OnRemoteProgramResultId 0x00000014
#define IMsTscAxEvents_OnRemoteProgramDisplayedId 0x00000015
#define IMsTscAxEvents_OnRemoteWindowDisplayedId 0x00000016
#define IMsTscAxEvents_OnLogonErrorId 0x00000017
#define IMsTscAxEvents_OnFocusReleasedId 0x00000018
#define IMsTscAxEvents_OnUserNameAcquiredId 0x00000019
#define IMsTscAxEvents_OnMouseInputModeChangedId 0x0000001a
#define IMsTscAxEvents_OnServiceMessageReceivedId 0x0000001b
#define IMsTscAxEvents_OnConnectionBarPullDownId 0x0000001c
#define IMsTscAxEvents_OnNetworkStatusChangedId 0x0000001d
#define IMsTscAxEvents_OnDevicesButtonPressedId 0x0000001e
#define IMsTscAxEvents_OnAutoReconnectedId 0x0000001f
#define IMsTscAxEvents_OnAutoReconnecting2Id 0x00000020

class CRdpEventSink : public IMsTscAxEvents
{
public:
    // IUnknown methods
    STDMETHODIMP CRdpEventSink::QueryInterface(REFIID riid, void** ppv)
    {
        HRESULT hr = S_OK;

        if (!ppv)
            return E_INVALIDARG;

        *ppv = NULL;

        if (riid == IID_IUnknown) {
            *ppv = this;
        }
        else if (riid == IID_IDispatch) {
            *ppv = this;
        }
        else if (riid == DIID_IMsTscAxEvents) {
            *ppv = this;
        }

        if (nullptr != *ppv) {
            ((IUnknown*)*ppv)->AddRef();
        }
        else {
            hr = E_NOINTERFACE;
        }

        return hr;
    }

    STDMETHODIMP_(ULONG) CRdpEventSink::AddRef(void)
    {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHODIMP_(ULONG) CRdpEventSink::Release(void)
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        if (refCount != 0) {
            return refCount;
        }

        delete this;
        return 0;
    }

    // IDispatch methods
    STDMETHODIMP CRdpEventSink::GetTypeInfoCount(UINT* pctinfo)
    {
        *pctinfo = 0;
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo)
    {
        return E_NOTIMPL;
    }

    STDMETHODIMP CRdpEventSink::GetIDsOfNames(REFIID riid, LPOLESTR* rgszNames, UINT cNames,
        LCID lcid, DISPID* rgDispId)
    {
        return E_NOTIMPL;
    }

    STDMETHODIMP CRdpEventSink::Invoke(DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags,
        DISPPARAMS* pDispParams, VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo, UINT* puArgErr)
    {
            HRESULT hr = E_NOTIMPL;

            switch (dispIdMember)
            {
            case IMsTscAxEvents_OnConnectingId:
                hr = OnConnecting();
                break;

            case IMsTscAxEvents_OnConnectedId:
                hr = OnConnected();
                break;

            case IMsTscAxEvents_OnLoginCompleteId:
                hr = OnLoginComplete();
                break;

            case IMsTscAxEvents_OnDisconnectedId:
                hr = OnDisconnected(pDispParams->rgvarg->lVal);
                break;

            case IMsTscAxEvents_OnEnterFullScreenModeId:
                hr = OnEnterFullScreenMode();
                break;

            case IMsTscAxEvents_OnLeaveFullScreenModeId:
                hr = OnLeaveFullScreenMode();
                break;

            case IMsTscAxEvents_OnRemoteDesktopSizeChangeId:
                hr = OnRemoteDesktopSizeChange(pDispParams->rgvarg[1].lVal, pDispParams->rgvarg[0].lVal);
                break;

            case IMsTscAxEvents_OnRequestContainerMinimizeId:
                hr = OnRequestContainerMinimize();
                break;

            case IMsTscAxEvents_OnConfirmCloseId:
                hr = OnConfirmClose(pDispParams->rgvarg[0].pboolVal);
                break;
        }

        return hr;
    }

    // IMsTscAxEvents methods
    STDMETHODIMP CRdpEventSink::OnConnecting()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnConnected()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnLoginComplete()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnDisconnected(long discReason)
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnEnterFullScreenMode()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnLeaveFullScreenMode()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnRemoteDesktopSizeChange(long width, long height)
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnRequestContainerMinimize()
    {
        return S_OK;
    }

    STDMETHODIMP CRdpEventSink::OnConfirmClose(VARIANT_BOOL* pfAllowClose)
    {
        *pfAllowClose = VARIANT_TRUE;
        return S_OK;
    }

    CRdpEventSink(HWND hParentWnd)
    {
        m_refCount = 0;
        m_hParentWnd = hParentWnd;
    }

    virtual ~CRdpEventSink()
    {

    }

private:
    ULONG m_refCount = 0;
    HWND m_hParentWnd = NULL;
};

class CRdpAxHostWnd : public IUnknown
{
public:

    CRdpAxHostWnd::CRdpAxHostWnd()
    {
        m_connected = false;
        m_desktopWidth = 1024;
        m_desktopHeight = 768;

        ZeroMemory(m_hostname, sizeof(m_hostname));
        ZeroMemory(m_username, sizeof(m_username));
        ZeroMemory(m_domain, sizeof(m_domain));
        ZeroMemory(m_password, sizeof(m_password));

        m_stopEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    }

    CRdpAxHostWnd::~CRdpAxHostWnd()
    {
        if (m_stopEvent) {
            CloseHandle(m_stopEvent);
            m_stopEvent = NULL;
        }
    }

    STDMETHODIMP CRdpAxHostWnd::QueryInterface(REFIID riid, void** ppv)
    {
        HRESULT hr = S_OK;

        if (!ppv)
            return E_INVALIDARG;

        *ppv = NULL;

        if (riid == IID_IUnknown) {
            *ppv = this;
        }

        if (nullptr != *ppv) {
            ((IUnknown*)*ppv)->AddRef();
        } else {
            hr = E_NOINTERFACE;
        }

        return hr;
    }

    STDMETHODIMP_(ULONG) CRdpAxHostWnd::AddRef(void)
    {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHODIMP_(ULONG) CRdpAxHostWnd::Release(void)
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        if (refCount != 0) {
            return refCount;
        }

        delete this;
        return 0;
    }

    STDMETHODIMP CRdpAxHostWnd::LoadControl()
    {
        HRESULT hr;
        IClassFactory* pClassFactory = NULL;
        IConnectionPoint* pConnectionPoint = NULL;
        IConnectionPointContainer* pConnectionPointContainer = NULL;
        
        hr = MsRdpEx_DllGetClassObject(CLSID_MsRdpClient, IID_IClassFactory, (void**) &pClassFactory);

        if (FAILED(hr)) {
            return hr;
        }

        hr = pClassFactory->CreateInstance(NULL, IID_IMsRdpClient, (void**) &m_rdpClient);

        pClassFactory->Release();
        pClassFactory = NULL;

        if (FAILED(hr)) {
            return hr;
        }

        hr = m_rdpClient->QueryInterface(IID_IConnectionPointContainer, (void**) &pConnectionPointContainer);

        if (FAILED(hr)) {
            return hr;
        }

        hr = pConnectionPointContainer->FindConnectionPoint(DIID_IMsTscAxEvents, &pConnectionPoint);

        if (FAILED(hr)) {
            return hr;
        }

        pConnectionPointContainer->Release();
        pConnectionPointContainer = NULL;

        m_eventSink = new CRdpEventSink(m_hParentWnd);
        m_eventSink->AddRef();

        hr = pConnectionPoint->Advise((IUnknown*)m_eventSink, &m_dwAdviseCookie);

        if (FAILED(hr)) {
            return hr;
        }

        pConnectionPoint->Release();
        pConnectionPoint = NULL;

        m_rdpClient->AddRef();

        return S_OK;
    }

    STDMETHODIMP CRdpAxHostWnd::LoadRdpFile()
    {
        MsRdpEx_ArrayListIt* it = NULL;
        MsRdpEx_RdpFileEntry* entry = NULL;
        MsRdpEx_RdpFile* rdpFile = MsRdpEx_RdpFile_New();
        char* filename = MsRdpEx_GetRdpFilenameFromCommandLine();

        if (!filename)
            return S_FALSE;

        if (!MsRdpEx_RdpFile_Load(rdpFile, filename))
            return S_FALSE;

        it = MsRdpEx_ArrayList_It(rdpFile->entries, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

        while (!MsRdpEx_ArrayListIt_Done(it))
        {
            VARIANT value;
            VariantInit(&value);

            entry = (MsRdpEx_RdpFileEntry*)MsRdpEx_ArrayListIt_Next(it);

            if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "full address")) {
                strcpy_s(m_hostname, sizeof(m_hostname) - 1, entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "desktopwidth")) {
                if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                    m_desktopWidth = value.intVal;
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "desktopheight")) {
                if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                    m_desktopHeight = value.intVal;
                }
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "username")) {
                strcpy_s(m_username, sizeof(m_username) - 1, entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "domain")) {
                strcpy_s(m_domain, sizeof(m_domain) - 1, entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetUserName")) {
                strcpy_s(m_username, sizeof(m_username) - 1, entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetDomain")) {
                strcpy_s(m_domain, sizeof(m_domain) - 1, entry->value);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ClearTextPassword")) {
                strcpy_s(m_password, sizeof(m_password) - 1, entry->value);
            }
        }

        MsRdpEx_ArrayListIt_Finish(it);

        MsRdpEx_RdpFile_Free(rdpFile);
        free(filename);

        return S_OK;
    }

    bool m_connected = false;
    char m_hostname[256];
    char m_username[256];
    char m_domain[256];
    char m_password[256];
    int m_desktopWidth = 1024;
    int m_desktopHeight = 768;
    HANDLE m_stopEvent = NULL;

private:
    LONG m_refCount = 0;
    HWND m_hWnd = NULL;
    HWND m_hParentWnd = NULL;
    DWORD m_dwAdviseCookie = 0;
    IMsRdpClient* m_rdpClient = NULL;
    CRdpEventSink* m_eventSink = NULL;
};

int MsRdpEx_AxHost_WinMain(
    HINSTANCE hInstance,
    HINSTANCE hPrevInstance,
    LPWSTR lpCmdLine,
    int nCmdShow)
{
    MSG msg;
    HRESULT hr;

    MsRdpEx_Load();

    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

    if (FAILED(hr)) {
        return -1;
    }

    INITCOMMONCONTROLSEX icex;
    icex.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icex.dwICC = ICC_STANDARD_CLASSES;
    InitCommonControlsEx(&icex);

    CRdpAxHostWnd rdpWindow;

    rdpWindow.LoadRdpFile();
    rdpWindow.LoadControl();
    int desktopWidth = rdpWindow.m_desktopWidth;
    int desktopHeight = rdpWindow.m_desktopHeight;
    RECT windowRect = { 0, 0, desktopWidth, desktopHeight };

    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);

        if (WaitForSingleObject(rdpWindow.m_stopEvent, 0) == WAIT_OBJECT_0)
            break;
    }

    MsRdpEx_Unload();

    CoUninitialize();

    return 0;
}
