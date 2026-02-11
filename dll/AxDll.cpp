
#include "MsRdpEx.h"

#include "MsRdpClient.h"

#include <MsRdpEx/MsRdpEx.h>
#include <MsRdpEx/DvcPluginExtractor.h>

#include <MsRdpEx/Environment.h>

extern bool g_AxHookEnabled;

HRESULT CDECL MsRdpEx_AxDll_DllGetClassObject(fnDllGetClassObject pfnDllGetClassObject, REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    HRESULT hr = pfnDllGetClassObject(rclsid, riid, ppv);

    if (!MsRdpEx_GetAxHookEnabled())
        return hr;

    if (riid == IID_IClassFactory)
    {
        if (hr == S_OK)
        {
            *ppv = MsRdpEx_CClassFactory_New(rclsid, (IClassFactory*) *ppv);
        }
    }

    return hr;
}

bool CDECL MsRdpEx_mstscax_Load(MsRdpEx_mstscax* dll, const char* filename)
{
    bool success = false;

    if (dll->initialized)
        return true;

    ZeroMemory(dll, sizeof(MsRdpEx_mstscax));

    dll->hModule = MsRdpEx_LoadLibrary(filename);

    MsRdpEx_LogPrint(DEBUG, "mstscax_load(%s): %p", filename, dll->hModule);

    if (!dll->hModule)
        goto exit;

    dll->DllCanUnloadNow = (fnDllCanUnloadNow)GetProcAddress(dll->hModule, "DllCanUnloadNow");
    dll->DllGetClassObject = (fnDllGetClassObject)GetProcAddress(dll->hModule, "DllGetClassObject");
    dll->DllRegisterServer = (fnDllRegisterServer)GetProcAddress(dll->hModule, "DllRegisterServer");
    dll->DllUnregisterServer = (fnDllUnregisterServer)GetProcAddress(dll->hModule, "DllUnregisterServer");
    dll->DllGetTscCtlVer = (fnDllGetTscCtlVer)GetProcAddress(dll->hModule, "DllGetTscCtlVer");
    dll->DllSetAuthProperties = (fnDllSetAuthProperties)GetProcAddress(dll->hModule, "DllSetAuthProperties");
    dll->DllGetClaimsToken = (fnDllGetClaimsToken9)GetProcAddress(dll->hModule, "DllGetClaimsToken");
    dll->DllSetClaimsToken = (fnDllSetClaimsToken)GetProcAddress(dll->hModule, "DllSetClaimsToken");
    dll->DllLogoffClaimsToken = (fnDllLogoffClaimsToken1)GetProcAddress(dll->hModule, "DllLogoffClaimsToken");
    dll->DllCancelAuthentication = (fnDllCancelAuthentication)GetProcAddress(dll->hModule, "DllCancelAuthentication");
    dll->DllDeleteSavedCreds = (fnDllDeleteSavedCreds)GetProcAddress(dll->hModule, "DllDeleteSavedCreds");

    dll->tscCtlVer = dll->DllGetTscCtlVer();

    success = true;
exit:
    dll->initialized = true;
    return success;
}

bool CDECL MsRdpEx_mstscax_Init(MsRdpEx_mstscax* dll)
{
    bool success = false;
    char* axPath = NULL;

    if (dll->initialized)
        return true;

    axPath = MsRdpEx_GetEnv("MSRDPEX_MSTSCAX_DLL");

    if (!axPath)
        axPath = _strdup(MsRdpEx_GetPath(MSRDPEX_MSTSCAX_DLL_PATH));

    if (!axPath)
        goto exit;

    success = MsRdpEx_mstscax_Load(dll, axPath);
exit:
    free(axPath);
    return success;
}

void CDECL MsRdpEx_mstscax_Uninit(MsRdpEx_mstscax* dll)
{
    if (dll->hModule) {
        FreeLibrary(dll->hModule);
        dll->hModule = NULL;
    }

    ZeroMemory(dll, sizeof(MsRdpEx_mstscax));
}

bool CDECL MsRdpEx_rdclientax_Load(MsRdpEx_rdclientax* dll, const char* filename)
{
    bool success = false;

    if (dll->initialized)
        return true;

    ZeroMemory(dll, sizeof(MsRdpEx_rdclientax));

    dll->hModule = MsRdpEx_LoadLibrary(filename);

    MsRdpEx_LogPrint(DEBUG, "rdclientax_load(%s): %p", filename, dll->hModule);

    if (!dll->hModule)
        goto exit;

    dll->DllCanUnloadNow = (fnDllCanUnloadNow)GetProcAddress(dll->hModule, "DllCanUnloadNow");
    dll->DllGetClassObject = (fnDllGetClassObject)GetProcAddress(dll->hModule, "DllGetClassObject");
    dll->DllRegisterServer = (fnDllRegisterServer)GetProcAddress(dll->hModule, "DllRegisterServer");
    dll->DllUnregisterServer = (fnDllUnregisterServer)GetProcAddress(dll->hModule, "DllUnregisterServer");
    dll->DllGetTscCtlVer = (fnDllGetTscCtlVer)GetProcAddress(dll->hModule, "DllGetTscCtlVer");
    dll->DllGetNewActivityId = (fnDllGetNewActivityId)GetProcAddress(dll->hModule, "DllGetNewActivityId");
    dll->DllSetAuthProperties = (fnDllSetAuthProperties)GetProcAddress(dll->hModule, "DllSetAuthProperties");
    dll->DllGetClaimsToken = (fnDllGetClaimsToken19)GetProcAddress(dll->hModule, "DllGetClaimsToken");
    dll->DllSetClaimsToken = (fnDllSetClaimsToken)GetProcAddress(dll->hModule, "DllSetClaimsToken");
    dll->DllLogoffClaimsToken = (fnDllLogoffClaimsToken3)GetProcAddress(dll->hModule, "DllLogoffClaimsToken");
    dll->DllCancelAuthentication = (fnDllCancelAuthentication)GetProcAddress(dll->hModule, "DllCancelAuthentication");
    dll->DllDeleteSavedCreds = (fnDllDeleteSavedCreds)GetProcAddress(dll->hModule, "DllDeleteSavedCreds");
    dll->DllPreCleanUp = (fnDllPreCleanUp)GetProcAddress(dll->hModule, "DllPreCleanUp");

    dll->tscCtlVer = dll->DllGetTscCtlVer();

    success = true;
exit:
    dll->initialized = true;
    return success;
}

bool CDECL MsRdpEx_rdclientax_Init(MsRdpEx_rdclientax* dll)
{
    bool success = false;
    char* axPath = NULL;

    if (dll->initialized)
        return true;

    axPath = MsRdpEx_GetEnv("MSRDPEX_RDCLIENTAX_DLL");

    if (!axPath)
        axPath = _strdup(MsRdpEx_GetPath(MSRDPEX_RDCLIENTAX_DLL_PATH));

    if (!axPath)
        goto exit;

    success = MsRdpEx_rdclientax_Load(dll, axPath);
exit:
    free(axPath);
    return success;
}

void CDECL MsRdpEx_rdclientax_Uninit(MsRdpEx_rdclientax* dll)
{
    if (dll->hModule) {
        FreeLibrary(dll->hModule);
        dll->hModule = NULL;
    }

    ZeroMemory(dll, sizeof(MsRdpEx_rdclientax));
}
