
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>

#include <MsRdpEx/Detours.h>

static HMODULE g_hModule = NULL;

static bool g_IsOOBClient = false;
static bool g_IsClientProcess = false;

static MsRdpEx_mstscax g_mstscax = { 0 };
static MsRdpEx_rdclientax g_rdclientax = { 0 };

HRESULT STDAPICALLTYPE DllCanUnloadNow()
{
    MsRdpEx_LogPrint(DEBUG, "DllCanUnloadNow");
    return S_FALSE;
}

HRESULT STDAPICALLTYPE DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    HRESULT hr = E_UNEXPECTED;
    char clsid[MSRDPEX_GUID_STRING_SIZE];
    char iid[MSRDPEX_GUID_STRING_SIZE];

    MsRdpEx_GuidBinToStr((GUID*)rclsid, clsid, 0);
    MsRdpEx_GuidBinToStr((GUID*)riid, iid, 0);

    if (g_IsClientProcess) {
        if (g_IsOOBClient) {
            MsRdpEx_LogPrint(DEBUG, "DllGetClassObject_rdclientax(%s, %s)", clsid, iid);
            hr = MsRdpEx_AxDll_DllGetClassObject(g_rdclientax.DllGetClassObject, rclsid, riid, ppv);
        }
        else {
            MsRdpEx_LogPrint(DEBUG, "DllGetClassObject_mstscax(%s, %s)", clsid, iid);
            hr = MsRdpEx_AxDll_DllGetClassObject(g_mstscax.DllGetClassObject, rclsid, riid, ppv);
        }
    }
    else {
        char* axName = MsRdpEx_GetEnv("MSRDPEX_AXNAME");

        MsRdpEx_LogPrint(DEBUG, "DllGetClassObject axName: %s", axName ? axName : "");

        if (MsRdpEx_StringIEquals(axName, "msrdc") || MsRdpEx_StringIEquals(axName, "rdclientax")) {
            if (MsRdpEx_rdclientax_Init(&g_rdclientax)) {
                MsRdpEx_LogPrint(DEBUG, "DllGetClassObject_rdclientax(%s, %s)", clsid, iid);
                hr = MsRdpEx_AxDll_DllGetClassObject(g_rdclientax.DllGetClassObject, rclsid, riid, ppv);
            }
            else {
                MsRdpEx_LogPrint(ERROR, "DllGetClassObject_rdclientax(%s, %s): init failure!", clsid, iid);
            }
        }
        else { // mstsc, mstscax
            if (MsRdpEx_mstscax_Init(&g_mstscax)) {
                MsRdpEx_LogPrint(DEBUG, "DllGetClassObject_mstscax(%s, %s)", clsid, iid);
                hr = MsRdpEx_AxDll_DllGetClassObject(g_mstscax.DllGetClassObject, rclsid, riid, ppv);
            }
            else {
                MsRdpEx_LogPrint(ERROR, "DllGetClassObject_mstscax(%s, %s): init failure!", clsid, iid);
            }
        }

        free(axName);
    }

    return hr;
}

HRESULT DllRegisterServer()
{
    MsRdpEx_LogPrint(DEBUG, "DllRegisterServer");
    return S_OK;
}

HRESULT DllUnregisterServer()
{
    MsRdpEx_LogPrint(DEBUG, "DllUnregisterServer");
    return S_OK;
}

uint64_t DllGetTscCtlVer()
{
    uint64_t version = 0;

    if (g_IsOOBClient) {
        version = g_rdclientax.DllGetTscCtlVer();
    }
    else {
        version = g_mstscax.DllGetTscCtlVer();
    }

    MsRdpEx_LogPrint(DEBUG, "DllGetTscCtlVer: 0x%04X", (unsigned int)version);

    return version;
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllSetAuthProperties");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllSetAuthProperties(properties);
    }
    else {
        hr = g_mstscax.DllSetAuthProperties(properties);
    }

    return hr;
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* a3)
{
    HRESULT hr = E_UNEXPECTED;
    MsRdpEx_LogPrint(DEBUG, "DllSetClaimsToken");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllSetClaimsToken(a1, a2, a3);
    }
    else {
        hr = g_mstscax.DllSetClaimsToken(a1, a2, a3);
    }

    return hr;
}

HRESULT DllGetClaimsToken(WCHAR* a1, BSTR a2, void* a3, void* a4, int a5, int a6,
    void* a7, BSTR* a8, BSTR* a9, BSTR* a10, char* a11, void* a12,
    void* a13, void* a14, void* a15, int* a16, int* a17, void* a18)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllGetClaimsToken2");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18);
    }
    else {
        // unsupported, not called externally
    }

    return hr;
}

HRESULT DllLogoffClaimsToken(WCHAR* a1, WCHAR* a2)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllLogoffClaimsToken2");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllLogoffClaimsToken(a1, a2);
    }
    else {
        // unsupported, not called externally
    }

    return hr;
}

HRESULT DllCancelAuthentication()
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllCancelAuthentication");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllCancelAuthentication();
    }
    else {
        hr = g_mstscax.DllCancelAuthentication();
    }

    return hr;
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllDeleteSavedCreds");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllDeleteSavedCreds(a1, a2);
    }
    else {
        hr = g_mstscax.DllDeleteSavedCreds(a1, a2);
    }

    return hr;
}

uint64_t DllPreCleanUp()
{
    uint64_t status = S_OK;

    MsRdpEx_LogPrint(DEBUG, "DllPreCleanUp");

    if (g_IsOOBClient) {
        status = g_rdclientax.DllPreCleanUp();
    }

    return status;
}

bool MsRdpEx_DetectClientProcess(bool* pIsOOBClient)
{
    char moduleFilePath[MSRDPEX_MAX_PATH] = { 0 };

    *pIsOOBClient = false;

    if (!GetModuleFileNameA(NULL, moduleFilePath, MSRDPEX_MAX_PATH))
        return false;

    const char* moduleFileName = MsRdpEx_FileBase(moduleFilePath);

    if (MsRdpEx_StringIEquals(moduleFileName, "mstsc.exe")) {
        *pIsOOBClient = false;
        return true;
    }
    else if (MsRdpEx_StringIEquals(moduleFileName, "msrdc.exe")) {
        *pIsOOBClient = true;
        return true;
    }

    return false;
}

static bool g_IsLoaded = false;

void MsRdpEx_Load()
{
    g_IsClientProcess = MsRdpEx_DetectClientProcess(&g_IsOOBClient);

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

    MsRdpEx_LogOpen();

    if (g_IsClientProcess) {
        if (g_IsOOBClient) {
            MsRdpEx_rdclientax_Init(&g_rdclientax);
        }
        else {
            MsRdpEx_mstscax_Init(&g_mstscax);
        }
    }

    MsRdpEx_AttachHooks();

    g_IsLoaded = true;
}

void MsRdpEx_Unload()
{
    if (!g_IsLoaded)
        return;

    MsRdpEx_DetachHooks();

    MsRdpEx_mstscax_Uninit(&g_mstscax);
    MsRdpEx_rdclientax_Uninit(&g_rdclientax);

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
