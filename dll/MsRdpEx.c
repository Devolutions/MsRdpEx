
#include "MsRdpEx.h"

#include "Utils.h"

FILE* g_LogFile = NULL;

static MsRdpEx_AxDll* g_AxDll = NULL;

HRESULT DllCanUnloadNow()
{
    fprintf(g_LogFile, "DllCanUnloadNow\n");
    return g_AxDll->DllCanUnloadNow();
}

HRESULT DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    fprintf(g_LogFile, "DllGetClassObject\n");
    return MsRdpEx_AxDll_DllGetClassObject(g_AxDll, rclsid, riid, ppv);
}

HRESULT DllRegisterServer()
{
    fprintf(g_LogFile, "DllRegisterServer\n");
    return g_AxDll->DllRegisterServer();
}

HRESULT DllUnregisterServer()
{
    fprintf(g_LogFile, "DllUnregisterServer\n");
    return g_AxDll->DllUnregisterServer();
}

uint64_t DllGetTscCtlVer()
{
    uint64_t version;
    version = g_AxDll->DllGetTscCtlVer();
    fprintf(g_LogFile, "DllGetTscCtlVer: 0x%04X\n", (unsigned int) version);
    return version;
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    fprintf(g_LogFile, "DllSetAuthProperties\n");
    return g_AxDll->DllSetAuthProperties(properties);
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* a3)
{
    fprintf(g_LogFile, "DllSetClaimsToken\n");
    return g_AxDll->DllSetClaimsToken(a1, a2, a3);
}

HRESULT DllGetClaimsToken(WCHAR* a1, WCHAR* a2, WCHAR* a3, uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9)
{
    fprintf(g_LogFile, "DllGetClaimsToken\n");
    return g_AxDll->DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9);
}

HRESULT DllLogoffClaimsToken(WCHAR* a1)
{
    fprintf(g_LogFile, "DllLogoffClaimsToken\n");
    return g_AxDll->DllLogoffClaimsToken(a1);
}

HRESULT DllCancelAuthentication()
{
    fprintf(g_LogFile, "DllCancelAuthentication\n");
    return g_AxDll->DllCancelAuthentication();
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    fprintf(g_LogFile, "DllDeleteSavedCreds\n");
    return g_AxDll->DllDeleteSavedCreds(a1, a2);
}

// Logger

void MsRdpEx_LogOpen()
{
    char filename[MSRDPEX_MAX_PATH];
    strcpy_s(filename, MSRDPEX_MAX_PATH, "C:\\Windows\\Temp\\MsRdpEx.log");
    g_LogFile = fopen(filename, "wb");
}

void MsRdpEx_LogClose()
{
    if (g_LogFile) {
        fclose(g_LogFile);
        g_LogFile = NULL;
    }
}

// DLL Main

void MsRdpEx_Load()
{
    MsRdpEx_LogOpen();

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    const char* ModuleFilePath = MsRdpEx_GetPath(MSRDPEX_CURRENT_MODULE_PATH);
    const char* ModuleFileName = MsRdpEx_FileBase(ModuleFilePath);

    fprintf(g_LogFile, "ModuleFilePath: %s\n", ModuleFilePath);

    uint32_t pathId = MSRDPEX_MSTSCAX_DLL_PATH;

    if (MsRdpEx_StringIEquals(ModuleFileName, "mstsc.exe")) {
        pathId = MSRDPEX_MSTSCAX_DLL_PATH;
    } else if (MsRdpEx_StringIEquals(ModuleFileName, "msrdc.exe")) {
        pathId = MSRDPEX_RDCLIENTAX_DLL_PATH;
    }

    g_AxDll = MsRdpEx_AxDll_New(MsRdpEx_GetPath(pathId));

    MsRdpEx_AttachHooks();
}

void MsRdpEx_Unload()
{
    MsRdpEx_DetachHooks();

    if (g_AxDll) {
        MsRdpEx_AxDll_Free(g_AxDll);
        g_AxDll = NULL;
    }

    MsRdpEx_LogClose();
}

static HMODULE g_hModule = NULL;

BOOL WINAPI DllMain(HMODULE hModule, DWORD dwReason, LPVOID reserved)
{
    if (DetourIsHelperProcess()) {
        return TRUE;
    }

    switch (dwReason) 
    { 
        case DLL_PROCESS_ATTACH:
            g_hModule = hModule;
            DisableThreadLibraryCalls(hModule);
            MsRdpEx_Load();
            break;

        case DLL_PROCESS_DETACH:
            MsRdpEx_Unload();
            break;

        case DLL_THREAD_ATTACH:
            break;

        case DLL_THREAD_DETACH:
            break;
    }

    return TRUE;
}
