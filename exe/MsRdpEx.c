
#include "MsRdpEx.h"

#include "Utils.h"

#include <detours.h>

int main(int argc, char** argv)
{
    char szCommandLine[2048];
    STARTUPINFOA StartupInfo;
    PROCESS_INFORMATION ProcessInfo;
    BOOL fSuccess;

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

    fSuccess = CreateProcessA(
        NULL,
        szCommandLine,
        NULL,
        NULL,
        FALSE,
        CREATE_SUSPENDED,
        NULL,
        NULL,
        &StartupInfo,
        &ProcessInfo);

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
