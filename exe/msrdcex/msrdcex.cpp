#include <MsRdpEx/RdpProcess.h>

LRESULT CALLBACK WrapperMsgWindowProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch (uMsg)
    {
        case 0x401: // WM_QUIT
            PostQuitMessage(uMsg);
            break;

        case 0x402: // Connect button
            PostQuitMessage(uMsg);
            break;

        case WM_DESTROY:
            PostQuitMessage(0);
            return 0;
    }

    return DefWindowProc(hWnd, uMsg, wParam, lParam);
}

HWND CreateWrapperMsgWindow(HINSTANCE hInstance)
{
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = WrapperMsgWindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"MsRdpEx_WrapperMsgWindow";

    if (!RegisterClass(&wc)) {
        return NULL;
    }

    HWND hWndMsg = CreateWindowEx(0,
        L"MsRdpEx_WrapperMsgWindow",
        L"MsRdpEx_WrapperMsgWindow",
        0, 0, 0, 0, 0, HWND_MESSAGE,
        NULL, hInstance, NULL);

    return hWndMsg;
}

BOOL MsRdpEx_SetMsgWindowHandle(HWND hWndMsg)
{
    char buffer[32];
    _ui64toa((unsigned long long)hWndMsg, buffer, 10);
    return SetEnvironmentVariableA("MSRDPEX_HWNDMSG", buffer) > 0;
}

int WINAPI wWinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd)
{
    HRESULT hr;
    char mstsc_args[2048];
    char msrdc_args[2048];
    IMsRdpExProcess* mstsc = NULL;
    IMsRdpExProcess* msrdc = NULL;

    if (__argc >= 2)
    {
        // we launched msrdc with command-line arguments
        hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, "msrdc");
        return 0;
    }

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    HWND hWndMsg = CreateWrapperMsgWindow(hInstance);

    MsRdpEx_SetMsgWindowHandle(hWndMsg);

    const char* mstsc_exe = MsRdpEx_GetPath(MSRDPEX_MSTSC_EXE_PATH);
    const char* msrdc_exe = MsRdpEx_GetPath(MSRDPEX_MSRDC_EXE_PATH);
    const char* default_rdp = MsRdpEx_GetPath(MSRDPEX_DEFAULT_RDP_PATH);

    hr = MsRdpExProcess_CreateInstance((LPVOID*)&mstsc);

    sprintf_s(mstsc_args, sizeof(mstsc_args) - 1, "\"%s\"",
        mstsc_exe);

    mstsc->AddRef();
    mstsc->SetFileName(mstsc_exe);
    mstsc->SetArguments(mstsc_args);
    hr = mstsc->StartWithInfo();

    MSG msg = { 0 };

    while (GetMessage(&msg, hWndMsg, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    int exitCode = (int)msg.wParam;

    mstsc->Stop(0);
    mstsc->Release();
    mstsc = NULL;

    if (exitCode == 0x402)
    {
        hr = MsRdpExProcess_CreateInstance((LPVOID*)&msrdc);

        sprintf_s(msrdc_args, sizeof(msrdc_args) - 1, "\"%s\" \"%s\"",
            msrdc_exe, default_rdp);

        msrdc->AddRef();
        msrdc->SetFileName(msrdc_exe);
        msrdc->SetArguments(msrdc_args);
        msrdc->StartWithInfo();
        msrdc->Wait(INFINITE);
        msrdc->Release();
        msrdc = NULL;
    }

    return 0;
}
