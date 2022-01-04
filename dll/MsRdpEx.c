
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

static HMODULE g_hModule = NULL;

static MsRdpEx_AxDll* g_AxDll = NULL;

HRESULT DllCanUnloadNow()
{
    return g_AxDll->DllCanUnloadNow();
}

HRESULT DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    return MsRdpEx_AxDll_DllGetClassObject(g_AxDll, rclsid, riid, ppv);
}

HRESULT DllRegisterServer()
{
    return g_AxDll->DllRegisterServer();
}

HRESULT DllUnregisterServer()
{
    return g_AxDll->DllUnregisterServer();
}

uint64_t DllGetTscCtlVer()
{
    uint64_t version;
    version = g_AxDll->DllGetTscCtlVer();
    MsRdpEx_Log("DllGetTscCtlVer: 0x%04X", (unsigned int) version);
    return version;
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    MsRdpEx_Log("DllSetAuthProperties");
    return g_AxDll->DllSetAuthProperties(properties);
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* a3)
{
    MsRdpEx_Log("DllSetClaimsToken");
    return g_AxDll->DllSetClaimsToken(a1, a2, a3);
}

HRESULT DllGetClaimsToken(WCHAR* a1, WCHAR* a2, WCHAR* a3, uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9)
{
    MsRdpEx_Log("DllGetClaimsToken");
    return g_AxDll->DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9);
}

HRESULT DllLogoffClaimsToken(WCHAR* a1)
{
    MsRdpEx_Log("DllLogoffClaimsToken");
    return g_AxDll->DllLogoffClaimsToken(a1);
}

HRESULT DllCancelAuthentication()
{
    MsRdpEx_Log("DllCancelAuthentication");
    return g_AxDll->DllCancelAuthentication();
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    MsRdpEx_Log("DllDeleteSavedCreds");
    return g_AxDll->DllDeleteSavedCreds(a1, a2);
}

uint64_t DllPreCleanUp()
{
    uint64_t status = S_OK;

    MsRdpEx_Log("DllPreCleanUp");

    if (g_AxDll->DllPreCleanUp)
        status = g_AxDll->DllPreCleanUp();

    return status;
}

// DLL Main

void MsRdpEx_Load()
{
    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    MsRdpEx_LogOpen();

    const char* ModuleFilePath = MsRdpEx_GetPath(MSRDPEX_CURRENT_MODULE_PATH);
    const char* ModuleFileName = MsRdpEx_FileBase(ModuleFilePath);

    MsRdpEx_Log("ModuleFilePath: %s", ModuleFilePath);

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
