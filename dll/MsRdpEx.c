
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>

#include <MsRdpEx/Detours.h>

#include <stdarg.h>

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

HRESULT DllGetNewActivityId(BSTR* pbstrActivityId)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllGetNewActivityId");

    if (g_IsOOBClient) {
        if (g_rdclientax.DllGetNewActivityId) {
            hr = g_rdclientax.DllGetNewActivityId(pbstrActivityId);
        }
    }
    else {
        // unsupported
    }

    return hr;
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

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, BSTR refreshToken)
{
    HRESULT hr = E_UNEXPECTED;
    MsRdpEx_LogPrint(DEBUG, "DllSetClaimsToken");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllSetClaimsToken(a1, a2, refreshToken);
    }
    else {
        hr = g_mstscax.DllSetClaimsToken(a1, a2, refreshToken);
    }

    return hr;
}

HRESULT DllGetClaimsToken(
    BSTR clientAddress,
    BSTR claimsHint,
    BSTR userNameHint,
    BSTR userDomainHint,
    UINT uiSilentRetrievalMode,
    BOOL allowCredPrompt,
    HWND parentWindow,
    BSTR* claimsToken,
    BSTR* actualAuthority,
    BSTR* actualUserName,
    RECT* position,
    BSTR windowTitle,
    BSTR logonCertAuthority,
    BSTR* resultMsg,
    BSTR wvdActivityId,
    BOOL* isAcquiredSilently,
    BOOL* isRetriableError,
    ...)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllGetClaimsToken");

    if (g_IsOOBClient) {
        va_list vl;
        va_start(vl, isRetriableError);
        DWORD version = g_rdclientax.tscCtlVer;

        if (version >= 3004) {
            BOOL invalidateCache = va_arg(vl, BOOL);
            BSTR resourceAppId = va_arg(vl, BSTR);

            hr = ((fnDllGetClaimsToken19)g_rdclientax.DllGetClaimsToken)(
                clientAddress,
                claimsHint,
                userNameHint,
                userDomainHint,
                uiSilentRetrievalMode,
                allowCredPrompt,
                parentWindow,
                claimsToken,
                actualAuthority,
                actualUserName,
                position,
                windowTitle,
                logonCertAuthority,
                resultMsg,
                wvdActivityId,
                isAcquiredSilently,
                isRetriableError,
                invalidateCache,
                resourceAppId);
        }
        else if (version >= 2606) {
            BSTR resourceAppId = va_arg(vl, BSTR);

            hr = ((fnDllGetClaimsToken18)g_rdclientax.DllGetClaimsToken)(
                clientAddress,
                claimsHint,
                userNameHint,
                userDomainHint,
                uiSilentRetrievalMode,
                allowCredPrompt,
                parentWindow,
                claimsToken,
                actualAuthority,
                actualUserName,
                position,
                windowTitle,
                logonCertAuthority,
                resultMsg,
                wvdActivityId,
                isAcquiredSilently,
                isRetriableError,
                resourceAppId);
        }
        else {
            hr = ((fnDllGetClaimsToken17)g_rdclientax.DllGetClaimsToken)(
                clientAddress,
                claimsHint,
                userNameHint,
                userDomainHint,
                uiSilentRetrievalMode,
                allowCredPrompt,
                parentWindow,
                claimsToken,
                actualAuthority,
                actualUserName,
                position,
                windowTitle,
                logonCertAuthority,
                resultMsg,
                wvdActivityId,
                isAcquiredSilently,
                isRetriableError);
        }

        va_end(vl);
    }
    else {
        // this function is never called in mstscax.dll
    }

    return hr;
}

HRESULT DllLogoffClaimsToken(BSTR claimsHint, ...)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllLogoffClaimsToken");

    va_list vl;
    va_start(vl, claimsHint);

    if (g_IsOOBClient) {
        DWORD version = g_rdclientax.tscCtlVer;

        if (version >= 3004) {
            BSTR clientId = va_arg(vl, BSTR);
            BSTR username = va_arg(vl, BSTR);
            hr = ((fnDllLogoffClaimsToken3)g_rdclientax.DllGetClaimsToken)(claimsHint, clientId, username);
        }
        else {
            BSTR clientId = va_arg(vl, BSTR);
            hr = ((fnDllLogoffClaimsToken2)g_rdclientax.DllGetClaimsToken)(claimsHint, clientId);
        }
    }
    else {
        // this function is never called in mstscax.dll
        hr = ((fnDllLogoffClaimsToken1)g_rdclientax.DllGetClaimsToken)(claimsHint);
    }

    va_end(vl);

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

HRESULT DllDeleteSavedCreds(BSTR workspaceId, BSTR username)
{
    HRESULT hr = E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "DllDeleteSavedCreds");

    if (g_IsOOBClient) {
        hr = g_rdclientax.DllDeleteSavedCreds(workspaceId, username);
    }
    else {
        hr = g_mstscax.DllDeleteSavedCreds(workspaceId, username);
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
    if (g_IsLoaded)
        return;

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
