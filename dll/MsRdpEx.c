
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>

#include <MsRdpEx/Detours.h>

static HMODULE g_hModule = NULL;

static MsRdpEx_AxDll* g_AxDll = NULL;

HRESULT STDAPICALLTYPE DllCanUnloadNow()
{
    MsRdpEx_LogPrint(DEBUG, "DllCanUnloadNow");
    return g_AxDll->DllCanUnloadNow();
}

HRESULT STDAPICALLTYPE DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    char clsid[MSRDPEX_GUID_STRING_SIZE];
    char iid[MSRDPEX_GUID_STRING_SIZE];

    MsRdpEx_GuidBinToStr((GUID*)rclsid, clsid, 0);
    MsRdpEx_GuidBinToStr((GUID*)riid, iid, 0);
    MsRdpEx_LogPrint(DEBUG, "DllGetClassObject(%s, %s)", clsid, iid);

    return MsRdpEx_AxDll_DllGetClassObject(g_AxDll, rclsid, riid, ppv);
}

HRESULT DllRegisterServer()
{
    MsRdpEx_LogPrint(DEBUG, "DllRegisterServer");
    return g_AxDll->DllRegisterServer();
}

HRESULT DllUnregisterServer()
{
    MsRdpEx_LogPrint(DEBUG, "DllUnregisterServer");
    return g_AxDll->DllUnregisterServer();
}

uint64_t DllGetTscCtlVer()
{
    uint64_t version;
    version = g_AxDll->DllGetTscCtlVer();
    MsRdpEx_LogPrint(DEBUG, "DllGetTscCtlVer: 0x%04X", (unsigned int) version);
    return version;
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    MsRdpEx_LogPrint(DEBUG, "DllSetAuthProperties");
    return g_AxDll->DllSetAuthProperties(properties);
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* a3)
{
    MsRdpEx_LogPrint(DEBUG, "DllSetClaimsToken");
    return g_AxDll->DllSetClaimsToken(a1, a2, a3);
}

#if 0
HRESULT DllGetClaimsToken(WCHAR* a1, WCHAR* a2, WCHAR* a3, uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9)
{
    MsRdpEx_LogPrint(DEBUG, "DllGetClaimsToken1");
    return g_AxDll->DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9);
}
#else
HRESULT DllGetClaimsToken(WCHAR* a1, BSTR a2, void* a3, void* a4, int a5, int a6,
    void* a7, BSTR* a8, BSTR* a9, BSTR* a10, char* a11, void* a12,
    void* a13, void* a14, void* a15, int* a16, int* a17, void* a18)
{
    MsRdpEx_LogPrint(DEBUG, "DllGetClaimsToken2");
    return g_AxDll->DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18);
}
#endif

#if 0
HRESULT DllLogoffClaimsToken(WCHAR* a1)
{
    MsRdpEx_LogPrint(DEBUG, "DllLogoffClaimsToken1");
    return g_AxDll->DllLogoffClaimsToken(a1);
}
#else
HRESULT DllLogoffClaimsToken(WCHAR* a1, WCHAR* a2)
{
    MsRdpEx_LogPrint(DEBUG, "DllLogoffClaimsToken2");
    return g_AxDll->DllLogoffClaimsToken(a1, a2);
}
#endif

HRESULT DllCancelAuthentication()
{
    MsRdpEx_LogPrint(DEBUG, "DllCancelAuthentication");
    return g_AxDll->DllCancelAuthentication();
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    MsRdpEx_LogPrint(DEBUG, "DllDeleteSavedCreds");
    return g_AxDll->DllDeleteSavedCreds(a1, a2);
}

uint64_t DllPreCleanUp()
{
    uint64_t status = S_OK;

    MsRdpEx_LogPrint(DEBUG, "DllPreCleanUp");

    if (g_AxDll->DllPreCleanUp)
        status = g_AxDll->DllPreCleanUp();

    return status;
}

// DLL Main

bool MsRdpEx_ShouldLoad()
{
    bool shouldLoad = false;
    char moduleFilePath[MSRDPEX_MAX_PATH] = { 0 };

    if (!GetModuleFileNameA(NULL, moduleFilePath, MSRDPEX_MAX_PATH))
        return false;

    const char* moduleFileName = MsRdpEx_FileBase(moduleFilePath);

    if (MsRdpEx_StringIEquals(moduleFileName, "mstsc.exe") ||
        MsRdpEx_StringIEquals(moduleFileName, "msrdc.exe")) {
        shouldLoad = true;
    }

    return shouldLoad;
}

static bool g_IsLoaded = false;

void MsRdpEx_Load()
{
    char* axName = NULL;
    char* axPath = NULL;
    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    MsRdpEx_LogOpen();

    const char* ModuleFilePath = MsRdpEx_GetPath(MSRDPEX_CURRENT_MODULE_PATH);
    const char* ModuleFileName = MsRdpEx_FileBase(ModuleFilePath);

    MsRdpEx_LogPrint(DEBUG, "ModuleFilePath: %s", ModuleFilePath);

    uint32_t pathId = 0;
    axName = MsRdpEx_GetEnv("MSRDPEX_AXNAME");

    if (axName) {
        MsRdpEx_LogPrint(DEBUG, "AxName: %s", axName);
    }

    if (MsRdpEx_StringIEquals(ModuleFileName, "mstsc.exe")) {
        pathId = MSRDPEX_MSTSCAX_DLL_PATH;
    } else if (MsRdpEx_StringIEquals(ModuleFileName, "msrdc.exe")) {
        pathId = MSRDPEX_RDCLIENTAX_DLL_PATH;
    }

    if (axName) {
        if (MsRdpEx_StringIEquals(axName, "mstsc") || MsRdpEx_StringIEquals(axName, "mstsc.exe") ||
            MsRdpEx_StringIEquals(axName, "mstscax") || MsRdpEx_StringIEquals(axName, "mstscax.dll")) {
            pathId = MSRDPEX_MSTSCAX_DLL_PATH;
        }
        else if (MsRdpEx_StringIEquals(axName, "msrdc") || MsRdpEx_StringIEquals(axName, "msrdc.exe") ||
            MsRdpEx_StringIEquals(axName, "rdclientax") || MsRdpEx_StringIEquals(axName, "rdclientax.dll")) {
            pathId = MSRDPEX_RDCLIENTAX_DLL_PATH;
        } else if (MsRdpEx_FileExists(axName)) {
            axPath = _strdup(axName);
        }
    }

    if (!axPath) {
        if (!pathId) {
            pathId = MSRDPEX_MSTSCAX_DLL_PATH;
        }

        axPath = _strdup(MsRdpEx_GetPath(pathId));
    }

    MsRdpEx_LogPrint(DEBUG, "AxDll: %s", axPath);

    g_AxDll = MsRdpEx_AxDll_New(axPath);

    free(axName);
    free(axPath);

    MsRdpEx_AttachHooks();

    g_IsLoaded = true;
}

void MsRdpEx_Unload()
{
    if (!g_IsLoaded)
        return;

    MsRdpEx_DetachHooks();

    if (g_AxDll) {
        MsRdpEx_AxDll_Free(g_AxDll);
        g_AxDll = NULL;
    }

    MsRdpEx_LogClose();

    g_IsLoaded = false;
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
            if (MsRdpEx_ShouldLoad()) {
                MsRdpEx_Load();
            }
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
