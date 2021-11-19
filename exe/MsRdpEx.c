
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <detours.h>

int main(int argc, char** argv)
{
    BOOL fSuccess;
    DWORD exitCode = 0;
    char szCommandLine[2048];
    STARTUPINFOA StartupInfo;
    PROCESS_INFORMATION ProcessInfo;

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    ZeroMemory(szCommandLine, sizeof(szCommandLine));

    uint32_t appPathId = MSRDPEX_MSTSC_EXE_PATH;
    
    appPathId = MSRDPEX_MSRDC_EXE_PATH;

    const char* lpApplicationName = MsRdpEx_GetPath(appPathId);

    if ((argc == 1) || (argv[1][0] == '/') || MsRdpEx_IsFile(argv[1]))
    {
        sprintf_s(szCommandLine, sizeof(szCommandLine), "%s ",
                MsRdpEx_FileBase(lpApplicationName));
    }
    for (int i = 1; i < argc; i++)
    {
        strcat(szCommandLine, argv[i]);
        strcat(szCommandLine, " ");
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

    if (!fSuccess)
    {
        printf("Could not start application (LastError=%d)\n", GetLastError());
        exit(1);
    }

    printf("lpApplicationName: %s\n", lpApplicationName);
    printf("lpCommandLine: %s\n", lpCommandLine);
    printf("lpDllName: %s\n", lpDllName);

    ResumeThread(ProcessInfo.hThread);
    WaitForSingleObject(ProcessInfo.hProcess, INFINITE);
    GetExitCodeProcess(ProcessInfo.hProcess, &exitCode);

    CloseHandle(ProcessInfo.hProcess);
    CloseHandle(ProcessInfo.hThread);

    printf("child process terminated with exit code: %d\n", exitCode);

    return 0;
}
