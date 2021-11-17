#ifndef MSRDPEX_H
#define MSRDPEX_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

#include <windows.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef HRESULT (WINAPI * fnDllCanUnloadNow)();

typedef HRESULT (WINAPI * fnDllGetClassObject)(REFCLSID rclsid, REFIID riid, LPVOID* ppv);

typedef HRESULT (WINAPI * fnDllRegisterServer)();

typedef HRESULT (WINAPI * fnDllUnregisterServer)();

typedef uint64_t (WINAPI * fnDllGetTscCtlVer)();

typedef HRESULT (WINAPI * fnDllSetAuthProperties)(uint64_t properties);

typedef HRESULT (WINAPI * fnDllSetClaimsToken)(uint64_t a1, uint64_t a2, WCHAR* p3);

typedef HRESULT (WINAPI * fnDllGetClaimsToken)(WCHAR* a1, WCHAR* a2, WCHAR* a3,
    uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9);

typedef HRESULT (WINAPI * fnDllLogoffClaimsToken)(WCHAR* a1);

typedef HRESULT (WINAPI * fnDllCancelAuthentication)();

typedef HRESULT (WINAPI * fnDllDeleteSavedCreds)(WCHAR* a1, WCHAR* a2);

struct _MsRdpEx_AxDll
{
    HMODULE hModule;
    fnDllCanUnloadNow DllCanUnloadNow;
    fnDllGetClassObject DllGetClassObject;
    fnDllRegisterServer DllRegisterServer;
    fnDllUnregisterServer DllUnregisterServer;
    fnDllGetTscCtlVer DllGetTscCtlVer;
    fnDllSetAuthProperties DllSetAuthProperties;
    fnDllGetClaimsToken DllGetClaimsToken;
    fnDllSetClaimsToken DllSetClaimsToken;
    fnDllLogoffClaimsToken DllLogoffClaimsToken;
    fnDllCancelAuthentication DllCancelAuthentication;
    fnDllDeleteSavedCreds DllDeleteSavedCreds;
};
typedef struct _MsRdpEx_AxDll MsRdpEx_AxDll;

MsRdpEx_AxDll* MsRdpEx_AxDll_New(const char* filename);
void MsRdpEx_AxDll_Free(MsRdpEx_AxDll* dll);

LONG MsRdpEx_AttachHooks();
LONG MsRdpEx_DetachHooks();

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_H */
