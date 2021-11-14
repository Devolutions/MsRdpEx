
#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>

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

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    return S_OK;
}

HRESULT DllCancelAuthentication()
{
    return S_OK;
}
