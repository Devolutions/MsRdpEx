
#include <MsRdpEx/ProcessLauncher.h>

#include "MsRdpEx.h"
#include <MsRdpEx/RdpFile.h>

#include <detours.h>

int MsRdpEx_LaunchProcessMain(int argc, char** argv, char* appName)
{
    BOOL fSuccess;
    DWORD exitCode = 0;
    uint32_t appPathId = 0;
    char szCommandLine[2048];
    STARTUPINFOA StartupInfo;
    PROCESS_INFORMATION ProcessInfo;

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    ZeroMemory(szCommandLine, sizeof(szCommandLine));

    if (!appName) {
        appPathId = MSRDPEX_MSTSC_EXE_PATH;
    }

    if (appName) {
        if (MsRdpEx_StringIEquals(appName, "mstsc") || MsRdpEx_StringIEquals(appName, "mstsc.exe")) {
            appPathId = MSRDPEX_MSTSC_EXE_PATH;
        } else if (MsRdpEx_StringIEquals(appName, "msrdc") || MsRdpEx_StringIEquals(appName, "msrdc.exe")) {
            appPathId = MSRDPEX_MSRDC_EXE_PATH;
        }
    }

    const char* lpApplicationName = appName;
    
    if (appPathId) {
        lpApplicationName = MsRdpEx_GetPath(appPathId);
    }

    for (int i = 1; i < argc; i++)
    {
        strncat(szCommandLine, argv[i], sizeof(szCommandLine) - 1);
        strncat(szCommandLine, " ", sizeof(szCommandLine) - 1);
    }

    ZeroMemory(&StartupInfo, sizeof(StartupInfo));
    StartupInfo.cb = sizeof(StartupInfo);

    ZeroMemory(&ProcessInfo, sizeof(ProcessInfo));

    char* lpCommandLine = szCommandLine;
    DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;
    const char* lpDllName = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);

    fSuccess = DetourCreateProcessWithDllExA(
        lpApplicationName, /* lpApplicationName */
        lpCommandLine, /* lpCommandLine */
        NULL, /* lpProcessAttributes */
        NULL, /* lpThreadAttributes */
        FALSE, /* bInheritHandles */
        dwCreationFlags, /* dwCreationFlags */
        NULL, /* lpEnvironment */
        NULL, /* lpCurrentDirectory */
        &StartupInfo, /* lpStartupInfo */
        &ProcessInfo, /* lpProcessInformation */
        lpDllName, /* lpDllName */
        NULL /* pfCreateProcessW */
    );

    if (!fSuccess) {
        return -1;
    }

    ResumeThread(ProcessInfo.hThread);
    WaitForSingleObject(ProcessInfo.hProcess, INFINITE);
    GetExitCodeProcess(ProcessInfo.hProcess, &exitCode);

    CloseHandle(ProcessInfo.hProcess);
    CloseHandle(ProcessInfo.hThread);

    return 0;
}
