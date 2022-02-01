
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Sspi.h>
#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/OutputMirror.h>

#include <detours.h>

HMODULE (WINAPI * Real_LoadLibraryW)(LPCWSTR lpLibFileName) = LoadLibraryW;

HMODULE Hook_LoadLibraryW(LPCWSTR lpLibFileName)
{
    HMODULE hModule;
    const char* filename;
    char* lpLibFileNameA = NULL;
    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpLibFileName, -1, &lpLibFileNameA, 0, NULL, NULL);

    filename = MsRdpEx_FileBase(lpLibFileNameA);

    MsRdpEx_Log("LoadLibraryW: %s", lpLibFileNameA);

    if (MsRdpEx_StringIEquals(filename, "mstscax.dll")) {
        hModule = LoadLibraryA(MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH));
    }
    else if (MsRdpEx_StringIEquals(filename, "rdclientax.dll")) {
        hModule = LoadLibraryA(MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH));
    } else {
        hModule = Real_LoadLibraryW(lpLibFileName);
    }

    free(lpLibFileNameA);

    return hModule;
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

    MsRdpEx_Log("GetAddrInfoW: %s", pNodeNameA);

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

    MsRdpEx_Log("CaptureBlt: hWnd: %p instance: %p", hWnd, instance);

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

    MsRdpEx_Log("BitBlt: %d,%d %dx%d %d,%d", dstX, dstY, width, height, srcX, srcY);

    status = Real_BitBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY, rop);

    bool captured = MsRdpEx_CaptureBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY);
    
    if (captured) {
        MsRdpEx_Log("BitBlt: %d,%d %dx%d %d,%d", dstX, dstY, width, height, srcX, srcY);
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
        MsRdpEx_Log("StretchBlt: %d,%d %dx%d %d,%d %dx%d", dstX, dstY, dstW, dstH, srcX, srcY, srcW, srcH);
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
	
	MsRdpEx_Log("OPWndProc: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

	if (uMsg == WM_NCCREATE)
	{
		CREATESTRUCTW* createStruct = (CREATESTRUCTW*) lParam;
		void* lpCreateParams = createStruct->lpCreateParams;

		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, createStruct->lpszName, -1, &lpWindowNameA, 0, NULL, NULL);
        MsRdpEx_Log("Window Create: %s", lpWindowNameA);
	}

	result = Real_OPWndProc(hWnd, uMsg, wParam, lParam);

	if (uMsg == WM_NCCREATE)
	{
		void* pUserData = (void*) GetWindowLongPtrW(hWnd, GWLP_USERDATA);

        instance = MsRdpEx_InstanceManager_AttachOutputWindow(hWnd, pUserData);

        if (!instance) {
            IMsRdpExInstance* instance = (IMsRdpExInstance*) CMsRdpExInstance_New(NULL);
            MsRdpEx_Log("Creating detached RDP instance: %p hWnd: %p", instance, hWnd);
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

    MsRdpEx_Log("RegisterClassExW: %s", lpClassNameA);

    if (MsRdpEx_StringEquals(lpClassNameA, "OPWindowClass")) {
        Real_OPWndProc = wndClassEx->lpfnWndProc;
        wndClassEx->lpfnWndProc = Hook_OPWndProc;
    }

    wndClassAtom = Real_RegisterClassExW(wndClassEx);

    free(lpClassNameA);

    return wndClassAtom;
}

PSecurityFunctionTableW (SEC_ENTRY* Real_InitSecurityInterfaceW)(void) = InitSecurityInterfaceW;

PSecurityFunctionTableW Hook_InitSecurityInterfaceW(void)
{
    PSecurityFunctionTableW pSecTable = Real_InitSecurityInterfaceW();
    return MsRdpEx_SspiHook_Init(pSecTable);
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
    DetourAttach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    //DetourAttach((PVOID*)(&Real_GetAddrInfoW), Hook_GetAddrInfoW);
    //DetourAttach((PVOID*)(&Real_GetAddrInfoExW), Hook_GetAddrInfoExW);
    DetourAttach((PVOID*)(&Real_BitBlt), Hook_BitBlt);
    DetourAttach((PVOID*)(&Real_StretchBlt), Hook_StretchBlt);
    DetourAttach((PVOID*)(&Real_RegisterClassExW), Hook_RegisterClassExW);
    DetourAttach((PVOID*)(&Real_InitSecurityInterfaceW), Hook_InitSecurityInterfaceW);
    error = DetourTransactionCommit();
    return error;
}

LONG MsRdpEx_DetachHooks()
{
    LONG error;
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourDetach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    //DetourDetach((PVOID*)(&Real_GetAddrInfoW), Hook_GetAddrInfoW);
    //DetourDetach((PVOID*)(&Real_GetAddrInfoExW), Hook_GetAddrInfoExW);
    DetourDetach((PVOID*)(&Real_BitBlt), Hook_BitBlt);
    DetourDetach((PVOID*)(&Real_StretchBlt), Hook_StretchBlt);
    DetourDetach((PVOID*)(&Real_RegisterClassExW), Hook_RegisterClassExW);
    DetourDetach((PVOID*)(&Real_InitSecurityInterfaceW), Hook_InitSecurityInterfaceW);
    error = DetourTransactionCommit();
    MsRdpEx_GlobalUninit();
    return error;
}
