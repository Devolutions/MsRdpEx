#include <MsRdpEx/RdpProcess.h>

#include "../AxHost/RdpAxHost.h"

LPWSTR ProcessAxHostCommandLine(LPWSTR lpCmdLine, bool* axHost)
{
    *axHost = false;
    LPCWSTR cmdArg = L"/axhost";
    WCHAR* cmdLine = _wcsdup(lpCmdLine);
    WCHAR* found = wcsstr(cmdLine, cmdArg);

    if (found != NULL) {
        *axHost = true;
        memmove(found, found + wcslen(cmdArg),
            (wcslen(found + wcslen(cmdArg)) + 1) * sizeof(WCHAR));
    }

    return cmdLine;
}

int WINAPI wWinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd)
{
    HRESULT hr;
    int exitCode = 0;
    bool axHost = false;

    LPWSTR cmdLine = ProcessAxHostCommandLine(lpCmdLine, &axHost);

    if (axHost)
    {
        exitCode = MsRdpEx_AxHost_WinMain(hInstance, hPrevInstance, cmdLine, nShowCmd);
    }
    else
    {
        hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, "mstsc");

        if (FAILED(hr)) {
            exitCode = -1;
        }
    }

    free(cmdLine);

    return exitCode;
}
