
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/MsRdpEx.h>

#include "../dll/MsRdpEx.h"
#include "DpiHelper.h"

#include "../com/mstscax.tlh"
using namespace MSTSCLib;

#include <atlbase.h>
#include <oleidl.h>
#include <commctrl.h>
#pragma comment(lib, "ComCtl32.lib")

#ifndef SafeRelease
#define SafeRelease(_x) { if ((_x) != nullptr) { (_x)->Release(); (_x) = nullptr; } }
#endif

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

class CRdpOleClientSite : public IOleClientSite
{
public:
    CRdpOleClientSite(IUnknown* pUnkOuter)
    {
        m_refCount = 0;
        m_pUnkOuter = pUnkOuter;
        m_pUnkOuter->AddRef();
    }
    
    virtual ~CRdpOleClientSite()
    {
        SafeRelease(m_pUnkOuter);
    }

    // IUnknown methods
    STDMETHODIMP QueryInterface(REFIID riid, void** ppv) override
    {
        HRESULT hr = S_OK;

        if (!ppv)
            return E_INVALIDARG;
        
        *ppv = NULL;

        if (riid == IID_IUnknown) {
            *ppv = static_cast<IUnknown*>(this);
        }
        else if (riid == IID_IOleClientSite) {
            *ppv = static_cast<IOleClientSite*>(this);
        }
        else if (m_pUnkOuter) {
            return m_pUnkOuter->QueryInterface(riid, ppv);
        }
        else {
            hr = E_NOINTERFACE;
        }

        if (*ppv) {
            AddRef();
        }

        return hr;
    }

    STDMETHODIMP_(ULONG) AddRef() override
    {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHODIMP_(ULONG) Release() override
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);
        if (refCount == 0) {
            delete this;
        }
        return refCount;
    }

    // IOleClientSite methods
    STDMETHODIMP SaveObject() override
    {
        return S_OK;
    }

    STDMETHODIMP GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk) override
    {
        *ppmk = NULL;
        return E_NOTIMPL;
    }

    STDMETHODIMP GetContainer(IOleContainer** ppContainer) override
    {
        *ppContainer = NULL;
        return E_NOTIMPL;
    }

    STDMETHODIMP ShowObject() override
    {
        return S_OK;
    }

    STDMETHODIMP OnShowWindow(BOOL fShow) override
    {
        return S_OK;
    }

    STDMETHODIMP RequestNewObjectLayout() override
    {
        return E_NOTIMPL;
    }

private:
    ULONG m_refCount = 0;
    IUnknown* m_pUnkOuter = NULL;
};

class CRdpOleInPlaceSiteEx : public IOleInPlaceSiteEx
{
public:
    CRdpOleInPlaceSiteEx(IUnknown* pUnkOuter)
        : m_refCount(0), m_hWnd(0)
    {
        m_pUnkOuter = pUnkOuter;
        m_pUnkOuter->AddRef();
    }

    virtual ~CRdpOleInPlaceSiteEx()
    {
        SafeRelease(m_pUnkOuter);
    }

    // IUnknown methods
    STDMETHODIMP QueryInterface(REFIID riid, void** ppv) override
    {
        HRESULT hr = S_OK;

        if (!ppv) return E_INVALIDARG;
        *ppv = NULL;

        if (riid == IID_IUnknown) {
            *ppv = static_cast<IUnknown*>(this);
        }
        else if (riid == IID_IOleWindow || riid == IID_IOleInPlaceSite || riid == IID_IOleInPlaceSiteEx) {
            *ppv = static_cast<IOleInPlaceSiteEx*>(this);
        }
        else if (m_pUnkOuter) {
            return m_pUnkOuter->QueryInterface(riid, ppv);
        }
        else {
            hr = E_NOINTERFACE;
        }

        if (*ppv) {
            AddRef();
        }

        return hr;
    }

    STDMETHODIMP_(ULONG) AddRef() override
    {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHODIMP_(ULONG) Release() override
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);
        if (refCount == 0) {
            delete this;
        }
        return refCount;
    }

    // IOleWindow methods
    STDMETHODIMP GetWindow(HWND* phwnd) override
    {
        *phwnd = m_hWnd;
        return S_OK;
    }

    STDMETHODIMP ContextSensitiveHelp(BOOL fEnterMode) override
    {
        return S_OK;
    }

    // IOleInPlaceSite methods
    STDMETHODIMP CanInPlaceActivate() override
    {
        return S_OK;
    }

    STDMETHODIMP OnInPlaceActivate() override
    {
        return S_OK;
    }

    STDMETHODIMP OnUIActivate() override
    {
        return S_OK;
    }

    STDMETHODIMP GetWindowContext(IOleInPlaceFrame** ppFrame, IOleInPlaceUIWindow** ppDoc,
        LPRECT lprcPosRect, LPRECT lprcClipRect,
        LPOLEINPLACEFRAMEINFO lpFrameInfo) override
    {
        RECT rect;

        *ppFrame = NULL;
        *ppDoc = NULL;
        lpFrameInfo = NULL;

        if (GetClientRect(m_hWnd, &rect))
        {
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            SetRect(lprcClipRect, 0, 0, width, height);
            SetRect(lprcPosRect, 0, 0, width, height);
        }

        return S_OK;
    }

    STDMETHODIMP Scroll(SIZE scrollExtant) override
    {
        return S_OK;
    }

    STDMETHODIMP OnUIDeactivate(BOOL fUndoable) override
    {
        return S_OK;
    }

    STDMETHODIMP OnInPlaceDeactivate() override
    {
        return S_OK;
    }

    STDMETHODIMP DiscardUndoState() override
    {
        return S_OK;
    }

    STDMETHODIMP DeactivateAndUndo() override
    {
        return S_OK;
    }

    STDMETHODIMP OnPosRectChange(LPCRECT lprcPosRect) override
    {
        return S_OK;
    }

    // IOleInPlaceSiteEx methods
    STDMETHODIMP OnInPlaceActivateEx(BOOL* pfNoRedraw, DWORD dwFlags) override
    {
        *pfNoRedraw = TRUE;
        return S_OK;
    }

    STDMETHODIMP OnInPlaceDeactivateEx(BOOL fNoRedraw) override
    {
        return S_OK;
    }

    STDMETHODIMP RequestUIActivate() override
    {
        return S_OK;
    }

    // additional methods
    STDMETHODIMP SetWindow(HWND hWnd)
    {
        m_hWnd = hWnd;
        return S_OK;
    }

private:
    ULONG m_refCount = 0;
    HWND m_hWnd = 0;
    IUnknown* m_pUnkOuter = NULL;
};

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

    CRdpEventSink(HWND hWndParent)
    {
        m_refCount = 0;
        m_hWndParent = hWndParent;
    }

    virtual ~CRdpEventSink()
    {

    }

private:
    ULONG m_refCount = 0;
    HWND m_hWndParent = NULL;
};

#define RdpAxHostWnd_ConnectMsgId  (WM_APP+0x101)

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

        m_pOleClientSite = new CRdpOleClientSite((IUnknown*)this);
        m_pOleClientSite->AddRef();

        m_pOleInPlaceSiteEx = new CRdpOleInPlaceSiteEx((IUnknown*)this);
        m_pOleInPlaceSiteEx->AddRef();
    }

    CRdpAxHostWnd::~CRdpAxHostWnd()
    {
        Uninit();

        if (m_stopEvent) {
            CloseHandle(m_stopEvent);
            m_stopEvent = NULL;
        }
    }

    STDMETHODIMP CRdpAxHostWnd::Uninit()
    {
        HRESULT hr = S_OK;

        if (m_eventSink)
        {
            IConnectionPoint* pConnectionPoint = NULL;
            IConnectionPointContainer* pConnectionPointContainer = NULL;

            if (m_rdpClient)
            {
                hr = m_rdpClient->QueryInterface(IID_IConnectionPointContainer, (void**)&pConnectionPointContainer);

                if (SUCCEEDED(hr)) {
                    hr = pConnectionPointContainer->FindConnectionPoint(DIID_IMsTscAxEvents, &pConnectionPoint);

                    if (SUCCEEDED(hr)) {
                        pConnectionPoint->Unadvise(m_dwAdviseCookie);
                    }
                }

                SafeRelease(pConnectionPoint);
                SafeRelease(pConnectionPointContainer);
            }

            SafeRelease(m_eventSink);
        }

        SafeRelease(m_pOleInPlaceActiveObject);
        SafeRelease(m_pOleObject);
        SafeRelease(m_rdpClient);
        SafeRelease(m_pOleInPlaceSiteEx);
        SafeRelease(m_pOleClientSite);
        SafeRelease(m_pOleInPlaceObject);

        return S_OK;
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
        else if (riid == IID_IOleClientSite) {
            *ppv = (void*)m_pOleClientSite;
        }
        else if (riid == IID_IOleInPlaceSiteEx) {
            *ppv = (void*)m_pOleInPlaceSiteEx;
        }

        if (nullptr != *ppv) {
            ((IUnknown*)*ppv)->AddRef();
        }
        else {
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

    static LRESULT CALLBACK CRdpAxHostWnd::StaticWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
    {
        CRdpAxHostWnd* pRdpAxHostWnd = (CRdpAxHostWnd*) GetWindowLongPtr(hWnd, GWLP_USERDATA);

        if (pRdpAxHostWnd) {
            return pRdpAxHostWnd->WndProc(hWnd, uMsg, wParam, lParam);
        }
        else {
            return DefWindowProc(hWnd, uMsg, wParam, lParam);
        }
    }

    LRESULT CALLBACK CRdpAxHostWnd::WndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
    {
        HWND hWndObject = NULL;

        MsRdpEx_LogPrint(DEBUG, "CRdpAxHostWnd::WndProc %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

        switch (uMsg)
        {
        case WM_DESTROY:
            SetEvent(m_stopEvent);
            break;

        case WM_ACTIVATEAPP:
            if (m_pOleInPlaceActiveObject)
            {
                m_pOleInPlaceActiveObject->OnFrameWindowActivate((BOOL)wParam);
            }
            break;

        case WM_SETFOCUS:
            if (m_pOleInPlaceActiveObject)
            {
                m_pOleInPlaceActiveObject->GetWindow(&hWndObject);
                SetFocus(hWndObject);
            }
            break;

        case WM_PALETTECHANGED:
        case WM_QUERYNEWPALETTE:
        case WM_SYSCOLORCHANGE:
            if (m_pOleInPlaceActiveObject)
            {
                m_pOleInPlaceActiveObject->GetWindow(&hWndObject);
                SendMessage(hWndObject, uMsg, wParam, lParam);
            }
            return 1;
            break;

        case WM_SIZE:
            {
                RECT rect;
                int width = LOWORD(lParam);
                int height = HIWORD(lParam);

                rect.left = 0;
                rect.top = 0;
                rect.right = width;
                rect.bottom = height;

                if (m_pOleInPlaceObject)
                {
                    m_pOleInPlaceObject->SetObjectRects(&rect, &rect);
                }

                if (m_pOleInPlaceActiveObject)
                {
                    m_pOleInPlaceActiveObject->GetWindow(&hWndObject);
                    SendMessage(hWndObject, WM_SIZE, wParam, lParam);
                }     
                return 0;
            }
            break;

        case RdpAxHostWnd_ConnectMsgId:
            this->Connect();
            break;

        default:
            return DefWindowProc(hWnd, uMsg, wParam, lParam);
            break;
        }

        return 0;
    }

    STDMETHODIMP CRdpAxHostWnd::CreateAxControl()
    {
        HRESULT hr;
        IClassFactory* pClassFactory = NULL;
        IConnectionPoint* pConnectionPoint = NULL;
        IConnectionPointContainer* pConnectionPointContainer = NULL;
        
        hr = MsRdpEx_DllGetClassObject(CLSID_MsRdpClientNotSafeForScripting, IID_IClassFactory, (void**) &pClassFactory);

        if (FAILED(hr)) {
            return hr;
        }

        hr = pClassFactory->CreateInstance(NULL, IID_IMsRdpClient, (void**) &m_rdpClient);

        SafeRelease(pClassFactory);

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

        SafeRelease(pConnectionPointContainer);

        m_eventSink = new CRdpEventSink(m_hWndParent);
        m_eventSink->AddRef();

        hr = pConnectionPoint->Advise((IUnknown*)m_eventSink, &m_dwAdviseCookie);

        if (FAILED(hr)) {
            return hr;
        }

        SafeRelease(pConnectionPoint);

        hr = m_rdpClient->QueryInterface(IID_IOleObject, (void**)&m_pOleObject);

        if (FAILED(hr)) {
            return hr;
        }

        hr = m_rdpClient->QueryInterface(IID_IOleInPlaceActiveObject, (void**)&m_pOleInPlaceActiveObject);

        if (FAILED(hr)) {
            return hr;
        }

        hr = m_rdpClient->QueryInterface(IID_IOleInPlaceObject, (void**)&m_pOleInPlaceObject);

        if (FAILED(hr)) {
            return hr;
        }

        hr = m_pOleObject->SetClientSite(m_pOleClientSite);

        if (FAILED(hr)) {
            return hr;
        }

        m_rdpClient->AddRef();

        return S_OK;
    }

    STDMETHODIMP CRdpAxHostWnd::CreateAxWindow(HWND hWndParent, HINSTANCE hInstance)
    {
        HRESULT hr = S_OK;
        WNDCLASSEX wndClass;
        LPWSTR lpClassName = L"MsRdpEx_AxHostWnd";
        LPWSTR lpWindowName = L"Remote Desktop Client Active Host";

        ZeroMemory(&wndClass, sizeof(WNDCLASSEX));
        wndClass.cbSize = sizeof(WNDCLASSEX);
        wndClass.style = 0;
        wndClass.lpfnWndProc = CRdpAxHostWnd::StaticWndProc;
        wndClass.cbClsExtra = 0;
        wndClass.cbWndExtra = 0;
        wndClass.hInstance = hInstance;
        wndClass.hIcon = NULL;
        wndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
        wndClass.hbrBackground = (HBRUSH)GetStockObject(NULL_BRUSH);
        wndClass.lpszMenuName = NULL;
        wndClass.lpszClassName = lpClassName;
        wndClass.hIconSm = NULL;

        if (!RegisterClassEx(&wndClass)) {
            DWORD lastError = GetLastError();
            if (lastError != ERROR_CLASS_ALREADY_EXISTS) {
                return E_FAIL;
            }
        }

        DWORD dwStyle = WS_OVERLAPPEDWINDOW | WS_VISIBLE;
        DWORD dwExStyle = 0;

        if (hWndParent)
        {
            dwStyle = WS_CHILD | WS_BORDER;
            dwExStyle &= ~(WS_EX_TOPMOST);
        }

        HMENU hMenu = NULL;
        int windowX = 0;
        int windowY = 0;
        int windowWidth = m_desktopWidth;
        int windowHeight = m_desktopHeight;
        RECT windowRect = { windowX, windowY, windowWidth, windowHeight };

        m_hWnd = CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle,
            windowX, windowY, windowWidth, windowHeight, hWndParent, hMenu, hInstance, (void*)this);

        if (!m_hWnd) {
            return E_FAIL;
        }

        SetWindowLongPtr(m_hWnd, GWLP_USERDATA, (LONG_PTR)this);

        m_pOleInPlaceSiteEx->SetWindow(m_hWnd);

        hr = m_pOleObject->DoVerb(OLEIVERB_PRIMARY, NULL, m_pOleClientSite, 0, m_hWnd, &windowRect);

        if (FAILED(hr)) {
            return E_FAIL;
        }

        ShowWindow(m_hWnd, SW_SHOWNORMAL);

        return hr;
    }

    STDMETHODIMP CRdpAxHostWnd::Connect()
    {
        HRESULT hr = S_OK;

        IMsRdpClientAdvancedSettings8* pAdvancedSettings = NULL;
        hr = m_rdpClient->get_AdvancedSettings9(&pAdvancedSettings);

        if (FAILED(hr))
            return E_FAIL;

        pAdvancedSettings->put_EnableCredSspSupport(VARIANT_TRUE);
        SafeRelease(pAdvancedSettings);

        m_rdpClient->put_ColorDepth(32);
        m_rdpClient->put_DesktopWidth(m_desktopWidth);
        m_rdpClient->put_DesktopHeight(m_desktopHeight);

        m_rdpClient->put_Server(CComBSTR(m_hostname));
        m_rdpClient->put_UserName(CComBSTR(m_username));
        m_rdpClient->put_Domain(CComBSTR(m_domain));

        IMsTscNonScriptable* pMsTscNonScriptable = NULL;
        hr = m_rdpClient->QueryInterface(IID_IMsTscNonScriptable, (void**)&pMsTscNonScriptable);

        if (FAILED(hr))
            return E_FAIL;

        hr = pMsTscNonScriptable->put_ClearTextPassword(CComBSTR(m_password));

        SafeRelease(pMsTscNonScriptable);

        if (FAILED(hr))
            return E_FAIL;

        hr = m_rdpClient->Connect();

        return hr;
    }

    STDMETHODIMP CRdpAxHostWnd::AdjustWindowSize()
    {
        HWND hWnd = m_hWnd;
        RECT windowRect;
        DWORD dwStyle = (DWORD)::GetWindowLongPtr(hWnd, GWL_STYLE);
        DWORD dwStyleEx = (DWORD)::GetWindowLongPtr(hWnd, GWL_EXSTYLE);

        uint32_t dpiX = 0;
        uint32_t dpiY = 0;
        HMONITOR monitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
        GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY);

        ::GetWindowRect(hWnd, &windowRect);
        ::AdjustWindowRectExForDpi(&windowRect, dwStyle, FALSE, dwStyleEx, dpiX);

        int windowPosX = 0;
        int windowPosY = 0;
        int windowWidth = windowRect.right - windowRect.left;
        int windowHeight = windowRect.bottom - windowRect.top;

        ::SetWindowPos(hWnd, NULL, windowPosX, windowPosY, windowWidth, windowHeight, SWP_FRAMECHANGED);

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

    HWND CRdpAxHostWnd::GetWindowHandle() {
        return m_hWnd;
    }

    HANDLE CRdpAxHostWnd::GetStopEvent() {
        return m_stopEvent;
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
    HWND m_hWndParent = NULL;
    DWORD m_dwAdviseCookie = 0;
    IMsRdpClient9* m_rdpClient = NULL;
    CRdpEventSink* m_eventSink = NULL;
    CRdpOleClientSite* m_pOleClientSite = NULL;
    CRdpOleInPlaceSiteEx* m_pOleInPlaceSiteEx = NULL;
    IOleObject* m_pOleObject = NULL;
    IOleInPlaceObject* m_pOleInPlaceObject = NULL;
    IOleInPlaceActiveObject* m_pOleInPlaceActiveObject = NULL;
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

    HWND hParentWnd = GetParentWindowHandle();

    CRdpAxHostWnd rdpWindow;

    rdpWindow.LoadRdpFile();
    rdpWindow.CreateAxControl();
    rdpWindow.CreateAxWindow(hParentWnd, hInstance);

    if (!hParentWnd) {
        rdpWindow.AdjustWindowSize();
    }

    HWND hRdpWnd = rdpWindow.GetWindowHandle();
    PostMessage(hRdpWnd, RdpAxHostWnd_ConnectMsgId, 0, 0);

    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);

        if (WaitForSingleObject(rdpWindow.GetStopEvent(), 0) == WAIT_OBJECT_0)
            break;
    }

    MsRdpEx_Unload();

    CoUninitialize();

    return 0;
}
