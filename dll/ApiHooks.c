
#include "MsRdpEx.h"

#include "Utils.h"

extern FILE* g_LogFile;

HMODULE (WINAPI * Real_LoadLibraryW)(LPCWSTR lpLibFileName) = LoadLibraryW;

HMODULE Hook_LoadLibraryW(LPCWSTR lpLibFileName)
{
    HMODULE hModule;
    const char* filename;
    char* lpLibFileNameA = NULL;
    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpLibFileName, -1, &lpLibFileNameA, 0, NULL, NULL);

    filename = MsRdpEx_FileBase(lpLibFileNameA);

    fprintf(g_LogFile, "LoadLibraryW: %s\n", lpLibFileNameA);

    if (MsRdpEx_StringIEquals(filename, "mstscax.dll")) {
        hModule = LoadLibraryA(MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH));
    }
    else if (MsRdpEx_StringIEquals(filename, "rdclientax.dll")) {
        hModule = LoadLibraryA(MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH));
    } else {
        hModule = Real_LoadLibraryW(lpLibFileName);
    }

    free(lpLibFileNameA);

    return hModule;
}

LONG MsRdpEx_AttachHooks()
{
    LONG error;
    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourAttach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    error = DetourTransactionCommit();
    return error;
}

LONG MsRdpEx_DetachHooks()
{
    LONG error;
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourDetach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    error = DetourTransactionCommit();
    return error;
}
