
#include "MsRdpEx.h"

HRESULT DllCanUnloadNow()
{
    return S_OK;
}

HRESULT DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    return S_FALSE;
}

HRESULT DllRegisterServer()
{
    return S_FALSE;
}

HRESULT DllUnregisterServer()
{
    return S_FALSE;
}

uint64_t DllGetTscCtlVer()
{
    return 0; // FileVersionInfo
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    return S_OK;
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* p3)
{
    return S_OK;
}

HRESULT DllGetClaimsToken(WCHAR* a1, WCHAR* a2, WCHAR* a3, uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, RECT* a8, WCHAR* a9)
{
    return S_OK;
}

HRESULT DllLogoffClaimsToken(WCHAR* a1)
{
    return S_OK;
}

HRESULT DllCancelAuthentication()
{
    return S_OK;
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    return S_OK;
}

// MsRdpEx_AxDll

MsRdpEx_AxDll* MsRdpEx_AxDll_New(const char* filename)
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

void MsRdpEx_AxDll_Free(MsRdpEx_AxDll* dll)
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
