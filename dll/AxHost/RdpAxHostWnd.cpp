
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/MsRdpEx.h>

#include "../MsRdpEx.h"
#include "../DpiHelper.h"
#include "RdpComBase.h"
#include "RdpEventSink.h"
#include "RdpOleSite.h"

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

#define RdpAxHostWnd_ConnectMsgId  (WM_APP+0x101)

#define SYSCOMMAND_SMART_SIZING_ID          0xF001
#define SYSCOMMAND_ZOOM_LEVEL_50_ID         0xF0A0
#define SYSCOMMAND_ZOOM_LEVEL_75_ID         0xF0A1
#define SYSCOMMAND_ZOOM_LEVEL_100_ID        0xF0A2
#define SYSCOMMAND_ZOOM_LEVEL_125_ID        0xF0A3
#define SYSCOMMAND_ZOOM_LEVEL_150_ID        0xF0A4
#define SYSCOMMAND_ZOOM_LEVEL_175_ID        0xF0A5
#define SYSCOMMAND_ZOOM_LEVEL_200_ID        0xF0A6
#define SYSCOMMAND_ZOOM_LEVEL_250_ID        0xF0A7
#define SYSCOMMAND_ZOOM_LEVEL_300_ID        0xF0A8

#define ToMenuCheckFlag(_condition) ((_condition) ? MF_CHECKED : MF_UNCHECKED)

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

        case WM_SYSCOMMAND:
            if (wParam == SYSCOMMAND_SMART_SIZING_ID)
            {
                m_smartSizing = !m_smartSizing;

                if (m_smartSizing)
                {
                    m_zoomLevel = 100;
                    VARIANT varZoomLevel;
                    VariantInit(&varZoomLevel);
                    varZoomLevel.vt = VT_UI4;
                    varZoomLevel.ulVal = m_zoomLevel;
                    IMsRdpExtendedSettings* pExtendedSettings = NULL;
                    m_rdpClient->QueryInterface(IID_IMsRdpExtendedSettings, (void**)&pExtendedSettings);
                    pExtendedSettings->put_Property(CComBSTR(L"ZoomLevel"), &varZoomLevel);
                    VariantClear(&varZoomLevel);
                    SafeRelease(pExtendedSettings);
                }

                IMsRdpClientAdvancedSettings8* pAdvancedSettings = NULL;
                HRESULT hr = m_rdpClient->get_AdvancedSettings9(&pAdvancedSettings);
                pAdvancedSettings->put_SmartSizing(ToVariantBool(m_smartSizing));
                SafeRelease(pAdvancedSettings);

                this->UpdateMenu();
            }
            else if ((wParam >= SYSCOMMAND_ZOOM_LEVEL_50_ID) && (wParam <= SYSCOMMAND_ZOOM_LEVEL_300_ID))
            {
                m_zoomLevel = 100;

                switch (wParam)
                {
                    case SYSCOMMAND_ZOOM_LEVEL_50_ID:
                        m_zoomLevel = 50;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_75_ID:
                        m_zoomLevel = 75;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_100_ID:
                        m_zoomLevel = 100;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_125_ID:
                        m_zoomLevel = 125;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_150_ID:
                        m_zoomLevel = 150;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_175_ID:
                        m_zoomLevel = 175;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_200_ID:
                        m_zoomLevel = 200;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_250_ID:
                        m_zoomLevel = 250;
                        break;
                    case SYSCOMMAND_ZOOM_LEVEL_300_ID:
                        m_zoomLevel = 300;
                        break;
                }

                m_smartSizing = false;

                this->UpdateMenu();

                IMsRdpClientAdvancedSettings8* pAdvancedSettings = NULL;
                HRESULT hr = m_rdpClient->get_AdvancedSettings9(&pAdvancedSettings);
                pAdvancedSettings->put_SmartSizing(ToVariantBool(m_smartSizing));
                SafeRelease(pAdvancedSettings);

                VARIANT varZoomLevel;
                VariantInit(&varZoomLevel);
                varZoomLevel.vt = VT_UI4;
                varZoomLevel.ulVal = m_zoomLevel;
                IMsRdpExtendedSettings* pExtendedSettings = NULL;
                m_rdpClient->QueryInterface(IID_IMsRdpExtendedSettings, (void**)&pExtendedSettings);
                pExtendedSettings->put_Property(CComBSTR(L"ZoomLevel"), &varZoomLevel);
                VariantClear(&varZoomLevel);
                SafeRelease(pExtendedSettings);
            }
            else
            {
                return DefWindowProc(hWnd, uMsg, wParam, lParam);
            }
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
            dwStyle = WS_CHILD | WS_CLIPSIBLINGS | WS_CLIPCHILDREN;
            dwExStyle = 0;
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

        this->CreateMenu();

        ShowWindow(m_hWnd, SW_SHOWNORMAL);

        return hr;
    }

    STDMETHODIMP CRdpAxHostWnd::CreateMenu()
    {
        HMENU hSysMenu = ::GetSystemMenu(m_hWnd, FALSE);

        // Create Zoom submenu
        HMENU hZoomMenu = CreatePopupMenu();
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel ==  50), SYSCOMMAND_ZOOM_LEVEL_50_ID, _T("50%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel ==  75), SYSCOMMAND_ZOOM_LEVEL_75_ID, _T("75%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 100), SYSCOMMAND_ZOOM_LEVEL_100_ID, _T("100%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 125), SYSCOMMAND_ZOOM_LEVEL_125_ID, _T("125%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 150), SYSCOMMAND_ZOOM_LEVEL_150_ID, _T("150%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 175), SYSCOMMAND_ZOOM_LEVEL_175_ID, _T("175%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 200), SYSCOMMAND_ZOOM_LEVEL_200_ID, _T("200%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 250), SYSCOMMAND_ZOOM_LEVEL_250_ID, _T("250%"));
        ::AppendMenu(hZoomMenu, MF_STRING | ToMenuCheckFlag(m_zoomLevel == 300), SYSCOMMAND_ZOOM_LEVEL_300_ID, _T("300%"));

        // Add Zoom submenu to the system menu
        ::AppendMenu(hSysMenu, MF_SEPARATOR, 0, NULL);
        ::AppendMenu(hSysMenu, MF_POPUP, (UINT_PTR)hZoomMenu, _T("Zoom"));

        ::AppendMenu(hSysMenu, MF_STRING | ToMenuCheckFlag(m_smartSizing), SYSCOMMAND_SMART_SIZING_ID, _T("Smart Sizing"));

        return S_OK;
    }

    STDMETHODIMP CRdpAxHostWnd::UpdateMenu()
    {
        HMENU hSysMenu = ::GetSystemMenu(m_hWnd, FALSE);

        ::CheckMenuItem(hSysMenu, SYSCOMMAND_SMART_SIZING_ID,
            MF_BYCOMMAND | ToMenuCheckFlag(m_smartSizing));

        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_50_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 50) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_75_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 75) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_100_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 100) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_125_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 125) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_150_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 150) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_175_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 175) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_200_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 200) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_250_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 250) && !m_smartSizing));
        ::CheckMenuItem(hSysMenu, SYSCOMMAND_ZOOM_LEVEL_300_ID,
            MF_BYCOMMAND | ToMenuCheckFlag((m_zoomLevel == 300) && !m_smartSizing));

        return S_OK;
    }

    STDMETHODIMP CRdpAxHostWnd::Connect()
    {
        HRESULT hr = S_OK;

        IMsRdpClientAdvancedSettings8* pAdvancedSettings = NULL;
        hr = m_rdpClient->get_AdvancedSettings9(&pAdvancedSettings);

        if (FAILED(hr))
            return E_FAIL;

        pAdvancedSettings->put_EnableCredSspSupport(ToVariantBool(m_credsspSupport));
        pAdvancedSettings->put_SmartSizing(ToVariantBool(m_smartSizing));
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

        // required for rdclientax.dll
        VARIANT varRequestUseNewOutputPresenter;
        VariantInit(&varRequestUseNewOutputPresenter);
        varRequestUseNewOutputPresenter.vt = VT_BOOL;
        varRequestUseNewOutputPresenter.ulVal = VARIANT_TRUE;
        IMsRdpExtendedSettings* pExtendedSettings = NULL;
        m_rdpClient->QueryInterface(IID_IMsRdpExtendedSettings, (void**)&pExtendedSettings);
        pExtendedSettings->put_Property(CComBSTR(L"RequestUseNewOutputPresenter"), &varRequestUseNewOutputPresenter);
        VariantClear(&varRequestUseNewOutputPresenter);
        SafeRelease(pExtendedSettings);

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
        MsRdpEx_GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY);

        ::GetWindowRect(hWnd, &windowRect);
        MsRdpEx_AdjustWindowRectExForDpi(&windowRect, dwStyle, FALSE, dwStyleEx, dpiX);

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
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "smart sizing")) {
                MsRdpEx_RdpFileEntry_GetBoolValue(entry, &m_smartSizing);
            }
            else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableCredSspSupport")) {
                MsRdpEx_RdpFileEntry_GetBoolValue(entry, &m_credsspSupport);
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
    bool m_credsspSupport = true;
    int m_desktopWidth = 1024;
    int m_desktopHeight = 768;
    int m_zoomLevel = 100;
    bool m_smartSizing = false;
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
