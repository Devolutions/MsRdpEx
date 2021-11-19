
#include "MsRdpEx.h"

#include "MsRdpClient.h"

#include <MsRdpEx/MsRdpEx.h>

HRESULT CDECL MsRdpEx_AxDll_DllGetClassObject(MsRdpEx_AxDll* axDll, REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    HRESULT hr = axDll->DllGetClassObject(rclsid, riid, ppv);

    if (riid == IID_IClassFactory)
    {
        if (hr == S_OK)
        {
            *ppv = MsRdpEx_CClassFactory_New(rclsid, (IClassFactory*) *ppv);
        }
    }

    return hr;
}

MsRdpEx_AxDll* CDECL MsRdpEx_AxDll_New(const char* filename)
{
    HMODULE hModule;
    MsRdpEx_AxDll* dll;

    hModule = LoadLibraryA(filename);

    if (!hModule)
        return NULL;

    dll = (MsRdpEx_AxDll*) malloc(sizeof(MsRdpEx_AxDll));

    if (!dll)
        return NULL;

    ZeroMemory(dll, sizeof(MsRdpEx_AxDll));

    dll->hModule = hModule;
    dll->DllCanUnloadNow = (fnDllCanUnloadNow) GetProcAddress(hModule, "DllCanUnloadNow");
    dll->DllGetClassObject = (fnDllGetClassObject) GetProcAddress(hModule, "DllGetClassObject");
    dll->DllRegisterServer = (fnDllRegisterServer) GetProcAddress(hModule, "DllRegisterServer");
    dll->DllUnregisterServer = (fnDllUnregisterServer) GetProcAddress(hModule, "DllUnregisterServer");
    dll->DllGetTscCtlVer = (fnDllGetTscCtlVer) GetProcAddress(hModule, "DllGetTscCtlVer");
    dll->DllSetAuthProperties = (fnDllSetAuthProperties) GetProcAddress(hModule, "DllSetAuthProperties");
    dll->DllGetClaimsToken = (fnDllGetClaimsToken) GetProcAddress(hModule, "DllGetClaimsToken");
    dll->DllSetClaimsToken = (fnDllSetClaimsToken) GetProcAddress(hModule, "DllSetClaimsToken");
    dll->DllLogoffClaimsToken = (fnDllLogoffClaimsToken) GetProcAddress(hModule, "DllLogoffClaimsToken");
    dll->DllCancelAuthentication = (fnDllCancelAuthentication) GetProcAddress(hModule, "DllCancelAuthentication");
    dll->DllDeleteSavedCreds = (fnDllDeleteSavedCreds) GetProcAddress(hModule, "DllDeleteSavedCreds");

    return dll;
}

void CDECL MsRdpEx_AxDll_Free(MsRdpEx_AxDll* dll)
{
    if (!dll)
        return;
    
    if (dll->hModule) {
        FreeLibrary(dll->hModule);
        dll->hModule = NULL;
    }

    ZeroMemory(dll, sizeof(MsRdpEx_AxDll));
    
    free(dll);
}
