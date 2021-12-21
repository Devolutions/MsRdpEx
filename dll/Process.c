
#include <MsRdpEx/Process.h>

#include "MsRdpEx.h"
#include <MsRdpEx/RdpFile.h>

#include <detours.h>

struct _MsRdpEx_Process
{
    STARTUPINFOA startupInfo;
    PROCESS_INFORMATION processInfo;
    DWORD exitCode;
};

int MsRdpEx_LaunchProcessMain(int argc, char** argv, const char* appName)
{
    int status = 0;
    BOOL fSuccess;
    uint32_t appPathId = 0;
    char szCommandLine[2048];
    MsRdpEx_Process* ctx;
    STARTUPINFOA* startupInfo;
    PROCESS_INFORMATION* processInfo;

    ctx = MsRdpEx_Process_New();

    if (!ctx)
        return -1;

    startupInfo = &ctx->startupInfo;
    processInfo = &ctx->processInfo;

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    if (!appName) {
        appPathId = MSRDPEX_MSTSC_EXE_PATH;
    }

    if (appName) {
        if (MsRdpEx_StringIEquals(appName, "mstsc") || MsRdpEx_StringIEquals(appName, "mstsc.exe")) {
            appPathId = MSRDPEX_MSTSC_EXE_PATH;
        }
        else if (MsRdpEx_StringIEquals(appName, "msrdc") || MsRdpEx_StringIEquals(appName, "msrdc.exe")) {
            appPathId = MSRDPEX_MSRDC_EXE_PATH;
        }
    }

    const char* lpApplicationName = appName;

    if (appPathId) {
        lpApplicationName = MsRdpEx_GetPath(appPathId);
    }

    ZeroMemory(szCommandLine, sizeof(szCommandLine));

    for (int i = 1; i < argc; i++)
    {
        strncat(szCommandLine, argv[i], sizeof(szCommandLine) - 1);
        strncat(szCommandLine, " ", sizeof(szCommandLine) - 1);
    }

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
        startupInfo, /* lpStartupInfo */
        processInfo, /* lpProcessInformation */
        lpDllName, /* lpDllName */
        NULL /* pfCreateProcessW */
    );

    if (!fSuccess) {
        status = -1;
        goto exit;
    }

    ResumeThread(processInfo->hThread);

    MsRdpEx_Process_Wait(ctx, INFINITE);

exit:
    MsRdpEx_Process_Free(ctx);
    return status;
}

DWORD MsRdpEx_Process_Wait(MsRdpEx_Process* ctx, DWORD dwMilliseconds)
{
    PROCESS_INFORMATION* processInfo = &ctx->processInfo;

    WaitForSingleObject(processInfo->hProcess, dwMilliseconds);
    GetExitCodeProcess(processInfo->hProcess, &ctx->exitCode);

    CloseHandle(processInfo->hProcess);
    CloseHandle(processInfo->hThread);

    return ctx->exitCode;
}

MsRdpEx_Process* MsRdpEx_Process_New()
{
    MsRdpEx_Process* ctx;

    ctx = (MsRdpEx_Process*) calloc(1, sizeof(MsRdpEx_Process));

    if (!ctx)
        return NULL;

    ctx->startupInfo.cb = sizeof(STARTUPINFOA);

    return ctx;
}

void MsRdpEx_Process_Free(MsRdpEx_Process* ctx)
{
    if (!ctx)
        return;
}
