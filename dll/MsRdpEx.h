#ifndef MSRDPEX_H
#define MSRDPEX_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

#include <windows.h>

#include <oleauto.h>

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

typedef HRESULT (WINAPI * fnDllGetClaimsToken1)(WCHAR* a1, WCHAR* a2, WCHAR* a3,
    uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9);

typedef HRESULT(WINAPI* fnDllGetClaimsToken2)(
    WCHAR* a1, BSTR a2, void* a3, void* a4, int a5, int a6,
    void* a7, BSTR* a8, BSTR* a9, BSTR* a10, char* a11, void* a12,
    void* a13, void* a14, void* a15, int* a16, int* a17, void* a18);

typedef HRESULT (WINAPI * fnDllLogoffClaimsToken1)(WCHAR* a1);

typedef HRESULT(WINAPI* fnDllLogoffClaimsToken2)(WCHAR* a1, WCHAR* a2);

typedef HRESULT (WINAPI * fnDllCancelAuthentication)();

typedef HRESULT (WINAPI * fnDllDeleteSavedCreds)(WCHAR* a1, WCHAR* a2);

typedef uint64_t (WINAPI * fnDllPreCleanUp)();

struct _MsRdpEx_AxDll
{
    HMODULE hModule;
    fnDllCanUnloadNow DllCanUnloadNow;
    fnDllGetClassObject DllGetClassObject;
    fnDllRegisterServer DllRegisterServer;
    fnDllUnregisterServer DllUnregisterServer;
    fnDllGetTscCtlVer DllGetTscCtlVer;
    fnDllSetAuthProperties DllSetAuthProperties;
    fnDllGetClaimsToken2 DllGetClaimsToken;
    fnDllSetClaimsToken DllSetClaimsToken;
    fnDllLogoffClaimsToken2 DllLogoffClaimsToken;
    fnDllCancelAuthentication DllCancelAuthentication;
    fnDllDeleteSavedCreds DllDeleteSavedCreds;
    fnDllPreCleanUp DllPreCleanUp;
};
typedef struct _MsRdpEx_AxDll MsRdpEx_AxDll;

void MsRdpEx_SetAxHookEnabled(bool axHookEnabled);

HRESULT MsRdpEx_AxDll_DllGetClassObject(MsRdpEx_AxDll* axDll, REFCLSID rclsid, REFIID riid, LPVOID* ppv);

MsRdpEx_AxDll* MsRdpEx_AxDll_New(const char* filename);
void MsRdpEx_AxDll_Free(MsRdpEx_AxDll* dll);

LONG MsRdpEx_AttachHooks();
LONG MsRdpEx_DetachHooks();

bool MsRdpEx_IsAddressInModule(PVOID pAddress, LPCTSTR pszModule);

LONG MsRdpEx_GetRectWidth(LPRECT rect);
LONG MsRdpEx_GetRectHeight(LPRECT rect);
void MsRdpEx_GetRectSize(LPRECT rect, LONG* pWidth, LONG* pHeight);

const char* MsRdpEx_GetWindowMessageName(uint32_t uMsg);

HBITMAP MsRdpEx_CreateDIBSection(HDC hDC, int width, int height, int bpp, uint8_t** ppPixelData);
bool MsRdpEx_WriteBitmapFile(const char* filename, uint8_t* data, int width, int height, int bpp);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_H */
