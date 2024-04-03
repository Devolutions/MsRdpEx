#include <MsRdpEx/RdpProcess.h>
#include <MsRdpEx/Environment.h>

#include "RdpAxHostWnd.h"
#include "RdpWinMain.h"

static LPWSTR ProcessAxHostCommandLine(LPWSTR lpCmdLine, bool* axHost)
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

int MsRdpEx_WinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd,
    const char* axName)
{
    HRESULT hr;
    int exitCode = 0;
    bool axHost = false;

    LPWSTR cmdLine = ProcessAxHostCommandLine(lpCmdLine, &axHost);

    if (axHost)
    {
        MsRdpEx_SetEnv("MSRDPEX_AXNAME", axName);
        exitCode = MsRdpEx_AxHost_WinMain(hInstance, hPrevInstance, cmdLine, nShowCmd);
    }
    else
    {
        hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, axName);

        if (FAILED(hr)) {
            exitCode = -1;
        }
    }

    free(cmdLine);

    return exitCode;
}
