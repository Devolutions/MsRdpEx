#include <atlbase.h>
#include <atlwin.h>
#include <atlhost.h>

#include "../com/mstscax.tlh"

#define RDP_HOSTNAME "IT-HELP-GW.ad.it-help.ninja"
#define RDP_USERNAME "Administrator@ad.it-help.ninja"
#define RDP_PASSWORD ""
#define RDP_DESKTOP_WIDTH 1024
#define RDP_DESKTOP_HEIGHT 768
#define RDP_WINDOW_TITLE "RDP Client"

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
    DECLARE_WND_CLASS_EX(NULL, 0, -1)

    BEGIN_MSG_MAP(CRdpWindow)
        MESSAGE_HANDLER(WM_CREATE, OnCreate)
    END_MSG_MAP()

    LRESULT OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled) {
        HRESULT hr = S_OK;
        CAxWindow2 axWindow;
        CComPtr<IUnknown> control;
        int desktopWidth = RDP_DESKTOP_WIDTH;
        int desktopHeight = RDP_DESKTOP_HEIGHT;

        RECT rect = { 0, 0, desktopWidth, desktopHeight };
        axWindow.Create(m_hWnd, rect, nullptr, WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN);

        if (axWindow.m_hWnd == nullptr) {
            return -1;
        }

        hr = axWindow.CreateControlEx(
            OLESTR("MsTscAx.MsTscAx"), nullptr, nullptr, &control,
            __uuidof(MSTSCLib::IMsTscAxEvents),
            reinterpret_cast<IUnknown*>(static_cast<RdpEventsSink*>(this)));

        hr = axWindow.QueryControl(__uuidof(MSTSCLib::IMsRdpClient9), reinterpret_cast<void**>(&m_rdpClient));

        if (FAILED(hr))
            return -1;

        hr = m_rdpClient->get_AdvancedSettings9(&m_advancedSettings);

        if (FAILED(hr))
            return -1;

        m_advancedSettings->put_EnableCredSspSupport(VARIANT_TRUE);

        m_rdpClient->put_ColorDepth(32);
        m_rdpClient->put_DesktopWidth(desktopWidth);
        m_rdpClient->put_DesktopHeight(desktopHeight);

        m_rdpClient->put_Server(CComBSTR(RDP_HOSTNAME));
        m_rdpClient->put_UserName(CComBSTR(RDP_USERNAME));

        CComPtr<MSTSCLib::IMsTscNonScriptable> nonScriptable;
        hr = m_rdpClient->QueryInterface(__uuidof(MSTSCLib::IMsTscNonScriptable), (void**)&nonScriptable);
        
        if (FAILED(hr))
            return -1;

        hr = nonScriptable->put_ClearTextPassword(CComBSTR(RDP_PASSWORD));

        if (FAILED(hr))
            return -1;

        m_rdpClient->Connect();

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
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnLoginComplete() {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnDisconnected(long reason) {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnFatalError(long error_code) {
        return S_OK;
    }

    STDMETHODIMP CRdpWindow::OnConfirmClose(VARIANT_BOOL* allow_close) {
        *allow_close = VARIANT_TRUE;
        return S_OK;
    }

private:
    typedef IDispEventImpl<1,
        CRdpWindow,
        &__uuidof(MSTSCLib::IMsTscAxEvents),
        &__uuidof(MSTSCLib::__MSTSCLib),
        1,
        0>
        RdpEventsSink;

    CComPtr<MSTSCLib::IMsRdpClient9> m_rdpClient;
    CComPtr<MSTSCLib::IMsRdpClientAdvancedSettings8> m_advancedSettings;
};

int WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine, _In_ int nCmdShow)
{
    MSG msg;
    HRESULT hr;

    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

    if (FAILED(hr)) {
        return -1;
    }

    AtlAxWinInit();

    CRdpWindow wnd;
    RECT rect = { 0, 0, RDP_DESKTOP_WIDTH + 20, RDP_DESKTOP_HEIGHT + 48 };
    wnd.Create(NULL, rect, _T(RDP_WINDOW_TITLE), WS_OVERLAPPEDWINDOW | WS_VISIBLE);

    if (wnd.m_hWnd == NULL) {
        return -1;
    }
    
    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    CoUninitialize();

    return 0;
}
