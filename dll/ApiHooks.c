
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

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

static int g_CaptureIndex = 0;
static HWND g_hOutputPresenterWnd = NULL;

BOOL Hook_BitBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY, DWORD rop)
{
    BOOL status;
    RECT rect = { 0 };
	HWND hWnd = WindowFromDC(hdcDst);

	if (!hWnd)
		goto real;

	if (hWnd != g_hOutputPresenterWnd)
		goto real;

    if (!GetClientRect(hWnd, &rect))
        goto real;

    LONG bitmapWidth = MsRdpEx_GetRectWidth(&rect);
    LONG bitmapHeight = MsRdpEx_GetRectHeight(&rect);

    uint8_t* bitmapData = NULL;
    HDC hShadowDC = CreateCompatibleDC(hdcDst);
    HBITMAP hShadowBitmap = MsRdpEx_CreateDIBSection(hdcSrc, bitmapWidth, bitmapHeight, 32, &bitmapData);

    if (!hShadowBitmap)
    {
        MsRdpEx_Log("bitmap width: %d height: %d", bitmapWidth, bitmapHeight);
        goto real;
    }

    HGDIOBJ hShadowObject = SelectObject(hShadowDC, hShadowBitmap);
    BitBlt(hShadowDC, dstX, dstY, width, height, hdcSrc, srcX, srcY, SRCCOPY);

    char filename[MSRDPEX_MAX_PATH];
    sprintf_s(filename, MSRDPEX_MAX_PATH, "C:\\Windows\\Temp\\MsRdpEx\\image_%04d.bmp", g_CaptureIndex);
    MsRdpEx_WriteBitmapFile(filename, bitmapData, bitmapWidth, bitmapHeight, 32);
    g_CaptureIndex++;

    SelectObject(hShadowDC, hShadowObject);
    DeleteObject(hShadowBitmap);
    ReleaseDC(NULL, hShadowDC);
    DeleteDC(hShadowDC);

    MsRdpEx_Log("BitBlt: %d,%d %dx%d %d,%d hWnd: %p", dstX, dstY, width, height, srcX, srcY, hWnd);

real:
    status = Real_BitBlt(hdcDst, dstX, dstY, width, height, hdcSrc, dstX, dstY, rop);
    return status;
}

BOOL (WINAPI * Real_StretchBlt)(
    HDC hdcDest, int xDest, int yDest, int wDest, int hDest,
    HDC hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, DWORD rop
    ) = StretchBlt;

BOOL Hook_StretchBlt(
    HDC hdcDest, int xDest, int yDest, int wDest, int hDest,
    HDC hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, DWORD rop)
{
    BOOL status;

    MsRdpEx_Log("StretchBlt: %d,%d %dx%d %d,%d %dx%d", xDest, yDest, wDest, hDest, xSrc, ySrc, wSrc, hSrc);

    status = Real_StretchBlt(hdcDest, xDest, yDest, wDest, hDest, hdcSrc, xSrc, ySrc, wSrc, hSrc, rop);

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
