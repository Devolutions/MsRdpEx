
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include "OutputMirror.h"

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

BOOL (WINAPI * Real_BitBlt)(
    HDC hdc, int x, int y, int cx, int cy,
    HDC hdcSrc, int x1, int y1, DWORD rop
    ) = BitBlt;

static HWND g_hOutputPresenterWnd = NULL;
static MsRdpEx_OutputMirror* g_OutputMirror = NULL;

bool MsRdpEx_CaptureBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY)
{
    RECT rect = { 0 };
    bool captured = false;

    HWND  hWnd = WindowFromDC(hdcDst);

    if (!hWnd)
        goto end;

    if (hWnd != g_hOutputPresenterWnd)
        goto end;

    if (!GetClientRect(hWnd, &rect))
        goto end;

    LONG bitmapWidth = MsRdpEx_GetRectWidth(&rect);
    LONG bitmapHeight = MsRdpEx_GetRectHeight(&rect);

    if (!g_OutputMirror) {
        g_OutputMirror = MsRdpEx_OutputMirror_New();
    }

    if ((g_OutputMirror->bitmapWidth != bitmapWidth) || (g_OutputMirror->bitmapHeight != bitmapHeight))
    {
        MsRdpEx_OutputMirror_Uninit(g_OutputMirror);
        MsRdpEx_OutputMirror_SetSourceDC(g_OutputMirror, hdcSrc);
        MsRdpEx_OutputMirror_SetFrameSize(g_OutputMirror, bitmapWidth, bitmapHeight);
        MsRdpEx_OutputMirror_Init(g_OutputMirror);
    }

    HDC hShadowDC = MsRdpEx_OutputMirror_GetShadowDC(g_OutputMirror);
    BitBlt(hShadowDC, dstX, dstY, width, height, hdcSrc, srcX, srcY, SRCCOPY);
    MsRdpEx_OutputMirror_DumpFrame(g_OutputMirror);

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

typedef struct
{
	void* param1;
	void* param2;
	void* name;
	void* marker;
} COPWnd;

LRESULT CALLBACK Hook_OPWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	LRESULT result;
	char* lpWindowNameA = NULL;
	
	MsRdpEx_Log("OPWndProc: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

	if (uMsg == WM_NCCREATE)
	{
		CREATESTRUCTW* createStruct = (CREATESTRUCTW*) lParam;
		void* lpCreateParams = createStruct->lpCreateParams;

		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, createStruct->lpszName, -1, &lpWindowNameA, 0, NULL, NULL);

		g_hOutputPresenterWnd = hWnd;
		COPWnd* pOPWnd = (COPWnd*) lpCreateParams;
		MsRdpEx_Log("Window Create: %s name: %s hWnd: %p", lpWindowNameA, pOPWnd->name, hWnd);
	}

	result = Real_OPWndProc(hWnd, uMsg, wParam, lParam);

	if (uMsg == WM_NCCREATE)
	{
		void* pUserData = (void*) GetWindowLongPtrW(hWnd, GWLP_USERDATA);
	}
	else if (uMsg == WM_NCDESTROY)
	{

	}

	free(lpWindowNameA);

	return result;
}

ATOM (WINAPI * Real_RegisterClassExW)(WNDCLASSEXW* wndClassEx) = RegisterClassExW;

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

LONG MsRdpEx_AttachHooks()
{
    LONG error;
    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourAttach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    DetourAttach((PVOID*)(&Real_BitBlt), Hook_BitBlt);
    DetourAttach((PVOID*)(&Real_StretchBlt), Hook_StretchBlt);
    DetourAttach((PVOID*)(&Real_RegisterClassExW), Hook_RegisterClassExW);
    error = DetourTransactionCommit();
    return error;
}

LONG MsRdpEx_DetachHooks()
{
    LONG error;
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourDetach((PVOID*)(&Real_LoadLibraryW), Hook_LoadLibraryW);
    DetourDetach((PVOID*)(&Real_BitBlt), Hook_BitBlt);
    DetourDetach((PVOID*)(&Real_StretchBlt), Hook_StretchBlt);
    DetourDetach((PVOID*)(&Real_RegisterClassExW), Hook_RegisterClassExW);
    error = DetourTransactionCommit();
    return error;
}
