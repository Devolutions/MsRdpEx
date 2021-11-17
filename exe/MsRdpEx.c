
#include "MsRdpEx.h"

#include "Utils.h"

#include <detours.h>

int main(int argc, char** argv)
{
    BOOL fSuccess;
    char szCommandLine[2048];
    STARTUPINFOA StartupInfo;
    PROCESS_INFORMATION ProcessInfo;

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    ZeroMemory(szCommandLine, sizeof(szCommandLine));

    if ((argc == 1) || (argv[1][0] == '/') || MsRdpEx_IsFile(argv[1]))
    {
        strcpy(szCommandLine, "mstsc.exe ");
    }
    for (int i = 1; i < argc; i++)
    {
        strcat(szCommandLine, argv[i]);
        strcat(szCommandLine, " ");
    }

    ZeroMemory(&StartupInfo, sizeof(StartupInfo));
    StartupInfo.cb = sizeof(StartupInfo);

    ZeroMemory(&ProcessInfo, sizeof(ProcessInfo));

    const char* lpApplicationName = MsRdpEx_GetPath(MSRDPEX_MSTSC_EXE_PATH);
    char* lpCommandLine = szCommandLine;
    DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;

    char ModuleFileName[1024];
    GetModuleFileNameA(NULL, ModuleFileName, 1024);
    
    size_t length = strlen(ModuleFileName);
    
    if (length > 4) {
        strcpy(&ModuleFileName[length - 4], ".dll");
    }

    const char* lpDllName = ModuleFileName;

    fSuccess = DetourCreateProcessWithDllExA(
        lpApplicationName, /* lpApplicationName */
        lpCommandLine, /* lpCommandLine */
        NULL, /* lpProcessAttributes */
        NULL, /* lpThreadAttributes */
        TRUE, /* bInheritHandles */
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

    ResumeThread(ProcessInfo.hThread);
    WaitForSingleObject(ProcessInfo.hProcess, INFINITE);
    CloseHandle(ProcessInfo.hProcess);
    CloseHandle(ProcessInfo.hThread);

    return 0;
}
