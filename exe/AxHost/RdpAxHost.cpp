#include <atlbase.h>
#include <atlwin.h>
#include <atlhost.h>

#include <commctrl.h>
#pragma comment(lib, "ComCtl32.lib")

#include "../com/mstscax.tlh"

#include "DpiHelper.h"

#include <MsRdpEx/RdpFile.h>

CComModule _Module;

class CRdpWindow : public CWindowImpl<CRdpWindow, CWindow, CWinTraits<WS_OVERLAPPEDWINDOW | WS_VISIBLE>>,
    public IDispEventImpl<1,
    CRdpWindow,
    &__uuidof(MSTSCLib::IMsTscAxEvents),
    &__uuidof(MSTSCLib::__MSTSCLib),
    1,
    0>
{
public:
    DECLARE_WND_CLASS_EX(L"CRdpWindow", 0, -1)

    BEGIN_MSG_MAP(CRdpWindow)
        MESSAGE_HANDLER(WM_CREATE, OnCreate)
        MESSAGE_HANDLER(WM_CLOSE, OnClose)
        MESSAGE_HANDLER(WM_DESTROY, OnDestroy)
    END_MSG_MAP()

    CRdpWindow::CRdpWindow()
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

    CRdpWindow::~CRdpWindow()
    {
        if (m_stopEvent) {
            CloseHandle(m_stopEvent);
            m_stopEvent = NULL;
        }
    }

    LRESULT OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) {
        HRESULT hr = S_OK;

        RECT windowRect = { 0, 0, m_desktopWidth, m_desktopHeight };
        m_axWindow.Create(m_hWnd, windowRect, nullptr, WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN);

        if (m_axWindow.m_hWnd == nullptr) {
            return -1;
        }

        LPCOLESTR controlName = OLESTR("MsTscAx.MsTscAx");

        CComPtr<IAxWinHostWindow> spWinHost;
        hr = m_axWindow.QueryHost(&spWinHost);

        if (SUCCEEDED(hr)) {
            hr = spWinHost->CreateControlEx(controlName, m_axWindow.m_hWnd, nullptr, &m_control,
                __uuidof(MSTSCLib::IMsTscAxEvents), reinterpret_cast<IUnknown*>(static_cast<RdpEventsSink*>(this)));
        }

        m_control.Detach();

        hr = m_axWindow.QueryControl(__uuidof(MSTSCLib::IMsRdpClient9), reinterpret_cast<void**>(&m_rdpClient));

        if (FAILED(hr))
            return -1;

        CComPtr<MSTSCLib::IMsRdpClientAdvancedSettings8> advancedSettings;
        hr = m_rdpClient->get_AdvancedSettings9(&advancedSettings);

        if (FAILED(hr))
            return -1;

        advancedSettings->put_EnableCredSspSupport(VARIANT_TRUE);

        m_rdpClient->put_ColorDepth(32);
        m_rdpClient->put_DesktopWidth(m_desktopWidth);
        m_rdpClient->put_DesktopHeight(m_desktopHeight);

        m_rdpClient->put_Server(CComBSTR(m_hostname));
        m_rdpClient->put_UserName(CComBSTR(m_username));
        m_rdpClient->put_Domain(CComBSTR(m_domain));

        CComPtr<MSTSCLib::IMsTscNonScriptable> nonScriptable;
        hr = m_rdpClient->QueryInterface(__uuidof(MSTSCLib::IMsTscNonScriptable), (void**)&nonScriptable);
        
        if (FAILED(hr))
            return -1;

        hr = nonScriptable->put_ClearTextPassword(CComBSTR(m_password));

        if (FAILED(hr))
            return -1;

        m_rdpClient->Connect();

        return 0;
    }

    LRESULT OnClose(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) {
        MsRdpEx_LogPrint(DEBUG, "CRdpWindow::OnClose connected: %d", m_connected ? 1:0);
        if (m_connected) {
            m_rdpClient->RequestClose();
            m_rdpClient->Disconnect();
            m_rdpClient = nullptr;
            m_connected = false;
        }
        bHandled = FALSE;
        return 0;
    }

    LRESULT OnDestroy(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) {
        MsRdpEx_LogPrint(DEBUG, "CRdpWindow::OnDestroy");
        AtlAdviseSinkMap(this, false);
        SetEvent(m_stopEvent);
        bHandled = FALSE;
        return 0;
    }

    BEGIN_SINK_MAP(CRdpWindow)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            2,
            &CRdpWindow::OnConnected)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            3,
            &CRdpWindow::OnLoginComplete)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            4,
            &CRdpWindow::OnDisconnected)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            10,
            &CRdpWindow::OnFatalError)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            15,
            &CRdpWindow::OnConfirmClose)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            18,
            &CRdpWindow::OnAuthenticationWarningDisplayed)
        SINK_ENTRY_EX(1,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            19,
            &CRdpWindow::OnAuthenticationWarningDismissed)
    END_SINK_MAP()

    STDMETHODIMP CRdpWindow::OnAuthenticationWarningDisplayed() {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnAuthenticationWarningDismissed() {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnConnected() {
        NotifyConnected();
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnLoginComplete() {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnDisconnected(long reason) {
        NotifyDisconnected();
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnFatalError(long error_code) {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnConfirmClose(VARIANT_BOOL* allow_close) {
        *allow_close = VARIANT_TRUE;
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::NotifyConnected() {
        m_connected = true;
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::NotifyDisconnected() {
        m_connected = false;
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::AdjustWindowSize(int desktopWidth, int desktopHeight)
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

    STDMETHODIMP CRdpWindow::LoadRdpFile()
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

            entry = (MsRdpEx_RdpFileEntry*) MsRdpEx_ArrayListIt_Next(it);

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
    typedef IDispEventImpl<1,
        CRdpWindow,
        &__uuidof(MSTSCLib::IMsTscAxEvents),
        &__uuidof(MSTSCLib::__MSTSCLib),
        1,
        0>
        RdpEventsSink;

    CAxWindow m_axWindow;
    DWORD m_adviseCookie = 0;
    CComPtr<IUnknown> m_control;
    CComPtr<MSTSCLib::IMsRdpClient9> m_rdpClient;
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

    AtlAxWinInit();

    INITCOMMONCONTROLSEX icex;
    icex.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icex.dwICC = ICC_STANDARD_CLASSES;
    InitCommonControlsEx(&icex);

    CRdpWindow rdpWindow;

    rdpWindow.LoadRdpFile();
    int desktopWidth = rdpWindow.m_desktopWidth;
    int desktopHeight = rdpWindow.m_desktopHeight;
    RECT windowRect = { 0, 0, desktopWidth, desktopHeight };

    rdpWindow.Create(NULL, windowRect, _T("Remote Desktop Client"), WS_OVERLAPPEDWINDOW | WS_VISIBLE);

    HWND hWnd = rdpWindow.m_hWnd;

    if (hWnd == NULL) {
        return -1;
    }

    rdpWindow.AdjustWindowSize(desktopWidth, desktopHeight);

    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);

        if (WaitForSingleObject(rdpWindow.m_stopEvent, 0) == WAIT_OBJECT_0)
            break;
    }

    _Module.Term();

    MsRdpEx_Unload();

    CoUninitialize();

    return 0;
}
