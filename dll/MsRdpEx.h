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

typedef HRESULT (WINAPI* fnDllGetNewActivityId)(BSTR* pbstrActivityId);

// exported by mstscax.dll, but never called
typedef HRESULT (WINAPI * fnDllSetAuthProperties)(uint64_t properties);

// exported by mstscax.dll, but never called
// no longer exported by recent rdclientax.dll
typedef HRESULT (WINAPI * fnDllSetClaimsToken)(uint64_t a1, uint64_t a2, BSTR refreshToken);

// exported by mstscax.dll, but never called
typedef HRESULT (WINAPI * fnDllGetClaimsToken9)(
    BSTR clientAddress,
    BSTR claimsHint,
    BSTR userNameHint,
    BSTR userDomainHint,
    HWND parentWindow,
    BSTR* claimsToken,
    BSTR* actualAuthority,
    BSTR logonCertAuthority,
    BSTR wvdActivityId);

// rdclientax.dll 1.2.2322.0, 1.2.2459.0
typedef HRESULT(WINAPI* fnDllGetClaimsToken17)(
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
    BOOL* isRetriableError);

// rdclientax.dll 1.2.2606.0
typedef HRESULT(WINAPI* fnDllGetClaimsToken18)(
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
    BSTR resourceAppId);

// rdclientax.dll 1.2.3004, 1.2.3316+
typedef HRESULT(WINAPI* fnDllGetClaimsToken19)(
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
    BOOL invalidateCache,
    BSTR resourceAppId);

// rdclientax.dll 1.2.4065.0+
typedef HRESULT(WINAPI* fnDllGetClaimsToken21)(
    BSTR clientAddress,
    BSTR claimsHint,
    BSTR userNameHint,
    BSTR userDomainHint,
    UINT uiSilentRetrievalMode,
    BOOL allowCredPrompt,
    HWND parentWindow,
    void* cloudPCParameters,
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
    BSTR diagnosticsUrl,
    BOOL invalidateCache,
    BSTR resourceAppId);

// exported by mstscax.dll, but never called
typedef HRESULT (WINAPI * fnDllLogoffClaimsToken1)(BSTR claimsHint);

// rdclientax.dll 1.2.2322.0, 1.2.2459.0, 1.2.2606.0
typedef HRESULT (WINAPI* fnDllLogoffClaimsToken2)(BSTR claimsHint, BSTR clientId);

// rdclientax.dll 1.2.3004.0+
typedef HRESULT (WINAPI* fnDllLogoffClaimsToken3)(BSTR claimsHint, BSTR clientId, BSTR username);

typedef HRESULT (WINAPI * fnDllCancelAuthentication)();

typedef HRESULT (WINAPI * fnDllDeleteSavedCreds)(BSTR workspaceId, BSTR username);

typedef uint64_t (WINAPI * fnDllPreCleanUp)();

struct _MsRdpEx_mstscax
{
    bool initialized;
    HMODULE hModule;
    DWORD tscCtlVer;
    fnDllCanUnloadNow DllCanUnloadNow;
    fnDllGetClassObject DllGetClassObject;
    fnDllRegisterServer DllRegisterServer;
    fnDllUnregisterServer DllUnregisterServer;
    fnDllGetTscCtlVer DllGetTscCtlVer;
    fnDllSetAuthProperties DllSetAuthProperties;
    fnDllGetClaimsToken9 DllGetClaimsToken;
    fnDllSetClaimsToken DllSetClaimsToken;
    fnDllLogoffClaimsToken1 DllLogoffClaimsToken;
    fnDllCancelAuthentication DllCancelAuthentication;
    fnDllDeleteSavedCreds DllDeleteSavedCreds;
};
typedef struct _MsRdpEx_mstscax MsRdpEx_mstscax;

struct _MsRdpEx_rdclientax
{
    bool initialized;
    HMODULE hModule;
    DWORD tscCtlVer;
    fnDllCanUnloadNow DllCanUnloadNow;
    fnDllGetClassObject DllGetClassObject;
    fnDllRegisterServer DllRegisterServer;
    fnDllUnregisterServer DllUnregisterServer;
    fnDllGetTscCtlVer DllGetTscCtlVer;
    fnDllGetNewActivityId DllGetNewActivityId;
    fnDllSetAuthProperties DllSetAuthProperties;
    fnDllGetClaimsToken19 DllGetClaimsToken;
    fnDllSetClaimsToken DllSetClaimsToken;
    fnDllLogoffClaimsToken3 DllLogoffClaimsToken;
    fnDllCancelAuthentication DllCancelAuthentication;
    fnDllDeleteSavedCreds DllDeleteSavedCreds;
    fnDllPreCleanUp DllPreCleanUp;
};
typedef struct _MsRdpEx_rdclientax MsRdpEx_rdclientax;

void MsRdpEx_SetAxHookEnabled(bool axHookEnabled);

HRESULT MsRdpEx_AxDll_DllGetClassObject(fnDllGetClassObject pfnDllGetClassObject, REFCLSID rclsid, REFIID riid, LPVOID* ppv);

bool CDECL MsRdpEx_mstscax_Load(MsRdpEx_mstscax* dll, const char* filename);
bool CDECL MsRdpEx_mstscax_Init(MsRdpEx_mstscax* dll);
void CDECL MsRdpEx_mstscax_Uninit(MsRdpEx_mstscax* dll);

bool CDECL MsRdpEx_rdclientax_Load(MsRdpEx_rdclientax* dll, const char* filename);
bool CDECL MsRdpEx_rdclientax_Init(MsRdpEx_rdclientax* dll);
void CDECL MsRdpEx_rdclientax_Uninit(MsRdpEx_rdclientax* dll);

bool MsRdpEx_GetAxHookEnabled();

LONG MsRdpEx_AttachHooks();
LONG MsRdpEx_DetachHooks();

bool MsRdpEx_IsAddressInModule(PVOID pAddress, LPCTSTR pszModule);
bool MsRdpEx_IsAddressInRdpAxModule(PVOID pAddress);

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
