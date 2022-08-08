
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Sspi.h>
#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/OutputMirror.h>

#include <MsRdpEx/Detours.h>

HMODULE (WINAPI* Real_LoadLibraryA)(LPCSTR lpLibFileName) = LoadLibraryA;
HMODULE (WINAPI* Real_LoadLibraryW)(LPCWSTR lpLibFileName) = LoadLibraryW;

HMODULE MsRdpEx_LoadLibrary(const char* filename)
{
    HMODULE hModule = NULL;
    WCHAR* filenameW = NULL;

    if (!filename)
        return NULL;

    MsRdpEx_LogPrint(DEBUG, "LoadLibraryX: %s", filename);

    if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, filename, -1, &filenameW, 0) < 1)
        return NULL;

    hModule = Real_LoadLibraryW(filenameW);

    free(filenameW);

    return hModule;
}

HMODULE Hook_LoadLibraryA(LPCSTR lpLibFileName)
{
    HMODULE hModule = NULL;
    
    MsRdpEx_LogPrint(DEBUG, "LoadLibraryA: %s", lpLibFileName);

    hModule = Real_LoadLibraryA(lpLibFileName);

    return hModule;
}

HMODULE Hook_LoadLibraryW(LPCWSTR lpLibFileName)
{
    HMODULE hModule;
    const char* filename;
    char* lpLibFileNameA = NULL;
    const char* msrdpexLibraryA = NULL;
    WCHAR* msrdpexLibraryW = NULL;
    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpLibFileName, -1, &lpLibFileNameA, 0, NULL, NULL);

    filename = MsRdpEx_FileBase(lpLibFileNameA);

    if (MsRdpEx_StringIEquals(filename, "mstscax.dll")) {
        msrdpexLibraryA = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);
        MsRdpEx_ConvertToUnicode(CP_UTF8, 0, msrdpexLibraryA, -1, &msrdpexLibraryW, 0);
        MsRdpEx_LogPrint(DEBUG, "LoadLibraryW: \"%s\" -> \"%s\"", lpLibFileNameA, msrdpexLibraryA);
        hModule = Real_LoadLibraryW(msrdpexLibraryW);
    }
    else if (MsRdpEx_StringIEquals(filename, "rdclientax.dll")) {
        msrdpexLibraryA = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);
        MsRdpEx_ConvertToUnicode(CP_UTF8, 0, msrdpexLibraryA, -1, &msrdpexLibraryW, 0);
        MsRdpEx_LogPrint(DEBUG, "LoadLibraryW: \"%s\" -> \"%s\"", lpLibFileNameA, msrdpexLibraryA);
        hModule = Real_LoadLibraryW(msrdpexLibraryW);
    }
    else {
        MsRdpEx_LogPrint(DEBUG, "LoadLibraryW: %s", lpLibFileNameA);
        hModule = Real_LoadLibraryW(lpLibFileName);
    }

    free(lpLibFileNameA);
    free(msrdpexLibraryW);

    return hModule;
}

FARPROC (WINAPI* Real_GetProcAddress)(HMODULE hModule, LPCSTR lpProcName) = GetProcAddress;

FARPROC Hook_GetProcAddress(HMODULE hModule, LPCSTR lpProcName)
{
    FARPROC procAddr;

    procAddr = Real_GetProcAddress(hModule, lpProcName);

    return procAddr;
}

INT (WINAPI* Real_GetAddrInfoW)(PCWSTR pNodeName, PCWSTR pServiceName,
    const ADDRINFOW* pHints, PADDRINFOW* ppResult) = GetAddrInfoW;

INT WINAPI Hook_GetAddrInfoW(PCWSTR pNodeNameW, PCWSTR pServiceName,
    const ADDRINFOW* pHints, PADDRINFOW* ppResult)
{
    int status;
    char* newNameA = NULL;
    WCHAR* newNameW = NULL;
    char* pNodeNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pNodeNameW, -1, &pNodeNameA, 0, NULL, NULL);

    MsRdpEx_LogPrint(DEBUG, "GetAddrInfoW: %s", pNodeNameA);

    if (MsRdpEx_NameResolver_GetMapping(pNodeNameA, &newNameA)) {
        MsRdpEx_ConvertToUnicode(CP_UTF8, 0, newNameA, -1, &newNameW, 0);
        pNodeNameW = newNameW;
        free(newNameA);
    }

    status = Real_GetAddrInfoW(pNodeNameW, pServiceName, pHints, ppResult);

    free(pNodeNameA);
    free(newNameW);
    return status;
}

INT (WINAPI* Real_GetAddrInfoExW)(
    PCWSTR pName, PCWSTR pServiceName, DWORD dwNameSpace, LPGUID lpNspId, const ADDRINFOEXW* hints,
    PADDRINFOEXW* ppResult, struct timeval* timeout, LPOVERLAPPED lpOverlapped,
    LPLOOKUPSERVICE_COMPLETION_ROUTINE lpCompletionRoutine, LPHANDLE lpHandle) = GetAddrInfoExW;

INT WINAPI Hook_GetAddrInfoExW(
    PCWSTR pName, PCWSTR pServiceName, DWORD dwNameSpace, LPGUID lpNspId, const ADDRINFOEXW* hints,
    PADDRINFOEXW* ppResult, struct timeval* timeout, LPOVERLAPPED lpOverlapped,
    LPLOOKUPSERVICE_COMPLETION_ROUTINE lpCompletionRoutine, LPHANDLE lpHandle)
{
    return Real_GetAddrInfoExW(pName, pServiceName, dwNameSpace, lpNspId, hints,
        ppResult, timeout, lpOverlapped, lpCompletionRoutine, lpHandle);
}

BOOL (WINAPI * Real_BitBlt)(
    HDC hdc, int x, int y, int cx, int cy,
    HDC hdcSrc, int x1, int y1, DWORD rop
    ) = BitBlt;

bool MsRdpEx_CaptureBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY)
{
    RECT rect = { 0 };
    uint32_t frameWidth = 0;
    uint32_t frameHeight = 0;
    bool captured = false;
    bool outputMirrorEnabled = false;
    bool videoRecordingEnabled = false;
    bool dumpBitmapUpdates = false;
    IMsRdpExInstance* instance = NULL;
    MsRdpEx_OutputMirror* outputMirror = NULL;

    HWND hWnd = WindowFromDC(hdcDst);

    if (!hWnd)
        goto end;

    instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

    if (!instance)
        goto end;

    MsRdpEx_LogPrint(TRACE, "CaptureBlt: hWnd: %p instance: %p", hWnd, instance);

    instance->GetOutputMirrorEnabled(&outputMirrorEnabled);
    instance->GetVideoRecordingEnabled(&videoRecordingEnabled);
    instance->GetDumpBitmapUpdates(&dumpBitmapUpdates);

    if (!outputMirrorEnabled)
        goto end;

    if (!GetClientRect(hWnd, &rect))
        goto end;

    LONG bitmapWidth = MsRdpEx_GetRectWidth(&rect);
    LONG bitmapHeight = MsRdpEx_GetRectHeight(&rect);

    instance->GetOutputMirrorObject((LPVOID*) &outputMirror);

    if (!outputMirror) {
        outputMirror = MsRdpEx_OutputMirror_New();
        MsRdpEx_OutputMirror_SetDumpBitmapUpdates(outputMirror, dumpBitmapUpdates);
        MsRdpEx_OutputMirror_SetVideoRecordingEnabled(outputMirror, videoRecordingEnabled);
        instance->SetOutputMirrorObject((LPVOID) outputMirror);
    }

    MsRdpEx_OutputMirror_GetFrameSize(outputMirror, &frameWidth, &frameHeight);

    if ((frameWidth != bitmapWidth) || (frameHeight != bitmapHeight))
    {
        MsRdpEx_OutputMirror_Uninit(outputMirror);
        MsRdpEx_OutputMirror_SetSourceDC(outputMirror, hdcSrc);
        MsRdpEx_OutputMirror_SetFrameSize(outputMirror, bitmapWidth, bitmapHeight);
        MsRdpEx_OutputMirror_Init(outputMirror);
    }

    HDC hShadowDC = MsRdpEx_OutputMirror_GetShadowDC(outputMirror);
    BitBlt(hShadowDC, dstX, dstY, width, height, hdcSrc, srcX, srcY, SRCCOPY);
    MsRdpEx_OutputMirror_DumpFrame(outputMirror);

    captured = true;
end:
    return captured;
}

BOOL Hook_BitBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY, DWORD rop)
{
    BOOL status;

    status = Real_BitBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY, rop);

    bool captured = MsRdpEx_CaptureBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY);
    
    if (captured) {
        MsRdpEx_LogPrint(TRACE, "BitBlt: %d,%d %dx%d %d,%d", dstX, dstY, width, height, srcX, srcY);
    }

    return status;
}

BOOL (WINAPI * Real_StretchBlt)(
    HDC hdcDest, int xDest, int yDest, int wDest, int hDest,
    HDC hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, DWORD rop
    ) = StretchBlt;

BOOL Hook_StretchBlt(
    HDC hdcDst, int dstX, int dstY, int dstW, int dstH,
    HDC hdcSrc, int srcX, int srcY, int srcW, int srcH, DWORD rop)
{
    BOOL status;

    status = Real_StretchBlt(hdcDst, dstX, dstY, dstW, dstH, hdcSrc, srcX, srcY, srcW, srcH, rop);

    bool captured = MsRdpEx_CaptureBlt(hdcDst, srcX, srcY, srcW, srcH, hdcSrc, srcX, srcY);

    if (captured) {
        MsRdpEx_LogPrint(TRACE, "StretchBlt: %d,%d %dx%d %d,%d %dx%d", dstX, dstY, dstW, dstH, srcX, srcY, srcW, srcH);
    }

end:
    return status;
}

static WNDPROC Real_OPWndProc = NULL;

extern void MsRdpEx_OutputWindow_OnCreate(HWND hWnd, void* pUserData);

LRESULT CALLBACK Hook_OPWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	LRESULT result;
	char* lpWindowNameA = NULL;
    CMsRdpExInstance* instance = NULL;
	
	MsRdpEx_LogPrint(DEBUG, "OPWndProc: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

	if (uMsg == WM_NCCREATE)
	{
		CREATESTRUCTW* createStruct = (CREATESTRUCTW*) lParam;
		void* lpCreateParams = createStruct->lpCreateParams;

		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, createStruct->lpszName, -1, &lpWindowNameA, 0, NULL, NULL);
        MsRdpEx_LogPrint(DEBUG, "Window Create: %s", lpWindowNameA);
	}

	result = Real_OPWndProc(hWnd, uMsg, wParam, lParam);

	if (uMsg == WM_NCCREATE)
	{
		void* pUserData = (void*) GetWindowLongPtrW(hWnd, GWLP_USERDATA);

        instance = MsRdpEx_InstanceManager_AttachOutputWindow(hWnd, pUserData);

        if (!instance) {
            IMsRdpExInstance* instance = (IMsRdpExInstance*) CMsRdpExInstance_New(NULL);
            MsRdpEx_LogPrint(DEBUG, "Creating detached RDP instance: %p hWnd: %p", instance, hWnd);
            instance->AttachOutputWindow(hWnd, pUserData);
            MsRdpEx_InstanceManager_Add((CMsRdpExInstance*) instance);
        }
	}
	else if (uMsg == WM_NCDESTROY)
	{
        IUnknown* pRdpClient = NULL;
        IMsRdpExInstance* instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

        if (instance) {
            instance->GetRdpClient((LPVOID*) &pRdpClient);

            if (!pRdpClient) {
                MsRdpEx_InstanceManager_Remove((CMsRdpExInstance*) instance);
            }
        }
	}

	free(lpWindowNameA);

	return result;
}

ATOM (WINAPI * Real_RegisterClassExW)(const WNDCLASSEXW* wndClassEx) = RegisterClassExW;

ATOM Hook_RegisterClassExW(WNDCLASSEXW* wndClassEx)
{
    ATOM wndClassAtom;
    char* lpClassNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, wndClassEx->lpszClassName, -1, &lpClassNameA, 0, NULL, NULL);

    MsRdpEx_LogPrint(DEBUG, "RegisterClassExW: %s", lpClassNameA);

    if (MsRdpEx_StringEquals(lpClassNameA, "OPWindowClass")) {
        Real_OPWndProc = wndClassEx->lpfnWndProc;
        wndClassEx->lpfnWndProc = Hook_OPWndProc;
    }

    wndClassAtom = Real_RegisterClassExW(wndClassEx);

    free(lpClassNameA);

    return wndClassAtom;
}

#include <psapi.h>

bool MsRdpEx_IsAddressInModule(PVOID pAddress, LPCTSTR pszModule)
{
    bool result;
    HMODULE hModule;
    LPVOID pStartAddr;
    LPVOID pEndAddr;
    MODULEINFO mi;
    MEMORY_BASIC_INFORMATION mbi;

    // Check the validity of the given address.
    if (VirtualQuery(pAddress, &mbi, sizeof(mbi)) == 0)
        return false;

    // Retrieve information regarding the specified module.
    hModule = GetModuleHandle(pszModule);

    if (!hModule)
        return false;

    if (!GetModuleInformation(GetCurrentProcess(), hModule, &mi, sizeof(mi)))
        return false;

    // Check the validity of the given address.
    if (VirtualQuery(pAddress, &mbi, sizeof(mbi)) == 0)
        return false;

    // Determine if the specified address is in the module. 
    pStartAddr = mi.lpBaseOfDll;
    pEndAddr = (LPVOID)((PBYTE)mi.lpBaseOfDll + mi.SizeOfImage - 1);

    result = (pAddress >= pStartAddr) && (pAddress <= pEndAddr) ? true : false;

    return result;
}

void MsRdpEx_GlobalInit()
{
    MsRdpEx_NameResolver_Get();
    MsRdpEx_InstanceManager_Get();
}

void MsRdpEx_GlobalUninit()
{
    MsRdpEx_NameResolver_Release();
    MsRdpEx_InstanceManager_Release();
}

LONG MsRdpEx_AttachHooks()
{
    LONG error;
    MsRdpEx_GlobalInit();
    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryA, Hook_LoadLibraryA);
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryW, Hook_LoadLibraryW);
    //MSRDPEX_DETOUR_ATTACH(Real_GetProcAddress, Hook_GetProcAddress);
    //MSRDPEX_DETOUR_ATTACH(Real_GetAddrInfoW, Hook_GetAddrInfoW);
    //MSRDPEX_DETOUR_ATTACH(Real_GetAddrInfoExW, Hook_GetAddrInfoExW);
    MSRDPEX_DETOUR_ATTACH(Real_BitBlt, Hook_BitBlt);
    MSRDPEX_DETOUR_ATTACH(Real_StretchBlt, Hook_StretchBlt);
    MSRDPEX_DETOUR_ATTACH(Real_RegisterClassExW, Hook_RegisterClassExW);
    MsRdpEx_AttachSspiHooks();
    error = DetourTransactionCommit();
    return error;
}

LONG MsRdpEx_DetachHooks()
{
    LONG error;
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryA, Hook_LoadLibraryA);
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryW, Hook_LoadLibraryW);
    //MSRDPEX_DETOUR_DETACH(Real_GetProcAddress, Hook_GetProcAddress);
    //MSRDPEX_DETOUR_DETACH(Real_GetAddrInfoW, Hook_GetAddrInfoW);
    //MSRDPEX_DETOUR_DETACH(Real_GetAddrInfoExW, Hook_GetAddrInfoExW);
    MSRDPEX_DETOUR_DETACH(Real_BitBlt, Hook_BitBlt);
    MSRDPEX_DETOUR_DETACH(Real_StretchBlt, Hook_StretchBlt);
    MSRDPEX_DETOUR_DETACH(Real_RegisterClassExW, Hook_RegisterClassExW);
    MsRdpEx_DetachSspiHooks();
    error = DetourTransactionCommit();
    MsRdpEx_GlobalUninit();
    return error;
}
