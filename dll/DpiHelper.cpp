
#include "DpiHelper.h"

#include <shellapi.h>

/**
 * High DPI Reference:
 * https://msdn.microsoft.com/en-us/library/windows/desktop/hh447398/
 *
 * Display Scaling in Windows 10:
 * https://blogs.technet.microsoft.com/askcore/2015/12/08/display-scaling-in-windows-10/
 *
 * Display Scaling changes for the Windows 10 Anniversary Update:
 * https://blogs.technet.microsoft.com/askcore/2016/08/16/display-scaling-changes-for-the-windows-10-anniversary-update/
 */

static int g_RefCount = 0;
static DpiHelper* g_Dpi = NULL;

float DpiHelper_GetScaleFactor(int dpi)
{
	return MulDiv(100, dpi, USER_DEFAULT_SCREEN_DPI) / (float) 100;
}

int DpiHelper_ScaleValue(int value, int dpi)
{
	return MulDiv(value, dpi, USER_DEFAULT_SCREEN_DPI);
}

DpiHelper* DpiHelper_New()
{
	DpiHelper* ctx;

	ctx = (DpiHelper*) calloc(1, sizeof(DpiHelper));

	if (!ctx)
		return NULL;

	ctx->hShCore = LoadLibraryA("shcore.dll");
	ctx->hUser32 = LoadLibraryA("user32.dll");

	if (ctx->hShCore)
	{
		ctx->GetScaleFactorForDevice = (fnGetScaleFactorForDevice)
			GetProcAddress(ctx->hShCore, "GetScaleFactorForDevice");

		ctx->RegisterScaleChangeNotifications = (fnRegisterScaleChangeNotifications)
			GetProcAddress(ctx->hShCore, "RegisterScaleChangeNotifications");

		ctx->RevokeScaleChangeNotifications = (fnRevokeScaleChangeNotifications)
			GetProcAddress(ctx->hShCore, "RevokeScaleChangeNotifications");

		ctx->GetScaleFactorForMonitor = (fnGetScaleFactorForMonitor)
			GetProcAddress(ctx->hShCore, "GetScaleFactorForMonitor");

		ctx->RegisterScaleChangeEvent = (fnRegisterScaleChangeEvent)
			GetProcAddress(ctx->hShCore, "RegisterScaleChangeEvent");

		ctx->UnregisterScaleChangeEvent = (fnUnregisterScaleChangeEvent)
			GetProcAddress(ctx->hShCore, "UnregisterScaleChangeEvent");

		ctx->SetProcessDpiAwareness = (fnSetProcessDpiAwareness)
			GetProcAddress(ctx->hShCore, "SetProcessDpiAwareness");

		ctx->GetProcessDpiAwareness = (fnGetProcessDpiAwareness)
			GetProcAddress(ctx->hShCore, "GetProcessDpiAwareness");

		ctx->GetDpiForMonitor = (fnGetDpiForMonitor)
			GetProcAddress(ctx->hShCore, "GetDpiForMonitor");

		ctx->GetDpiForShellUIComponent = (fnGetDpiForShellUIComponent)
			GetProcAddress(ctx->hShCore, "GetDpiForShellUIComponent");

		ctx->AdjustWindowRectExForDpi = (fnAdjustWindowRectExForDpi)
			GetProcAddress(ctx->hUser32, "AdjustWindowRectExForDpi");

		ctx->EnableNonClientDpiScaling = (fnEnableNonClientDpiScaling)
			GetProcAddress(ctx->hUser32, "EnableNonClientDpiScaling");
	}

	return ctx;
}

void DpiHelper_Free(DpiHelper* ctx)
{
	if (!ctx)
		return;

	if (ctx->hShCore)
	{
		FreeLibrary(ctx->hShCore);
		ctx->hShCore = NULL;
	}

	if (ctx->hUser32)
	{
		FreeLibrary(ctx->hUser32);
		ctx->hUser32 = NULL;
	}

	free(ctx);
}

DpiHelper* DpiHelper_Get()
{
	if (!g_Dpi)
		g_Dpi = DpiHelper_New();

	g_RefCount++;

	return g_Dpi;
}

void DpiHelper_Release()
{
	g_RefCount--;

	if (g_RefCount < 0)
		g_RefCount = 0;

	if (g_Dpi && (g_RefCount < 1))
	{
		DpiHelper_Free(g_Dpi);
		g_Dpi = NULL;
	}
}

DEVICE_SCALE_FACTOR MsRdpEx_GetScaleFactorForDevice(DISPLAY_DEVICE_TYPE deviceType)
{
	if (g_Dpi && g_Dpi->GetScaleFactorForDevice)
		return g_Dpi->GetScaleFactorForDevice(deviceType);

	return SCALE_100_PERCENT;
}

HRESULT MsRdpEx_RegisterScaleChangeNotifications(DISPLAY_DEVICE_TYPE displayDevice, HWND hwndNotify, uint32_t uMsgNotify,
	                                 DWORD* pdwCookie)
{
	if (g_Dpi && g_Dpi->RegisterScaleChangeNotifications)
		return g_Dpi->RegisterScaleChangeNotifications(displayDevice, hwndNotify, uMsgNotify, pdwCookie);

	return S_FALSE;
}

HRESULT MsRdpEx_RevokeScaleChangeNotifications(DISPLAY_DEVICE_TYPE displayDevice, DWORD dwCookie)
{
	if (g_Dpi && g_Dpi->RevokeScaleChangeNotifications)
		return g_Dpi->RevokeScaleChangeNotifications(displayDevice, dwCookie);

	return S_FALSE;
}

HRESULT MsRdpEx_GetScaleFactorForMonitor(HMONITOR hMonitor, DEVICE_SCALE_FACTOR* pScale)
{
	if (g_Dpi && g_Dpi->GetScaleFactorForMonitor)
		return g_Dpi->GetScaleFactorForMonitor(hMonitor, pScale);

	return S_FALSE;
}

HRESULT MsRdpEx_RegisterScaleChangeEvent(HANDLE hEvent, DWORD_PTR* pdwCookie)
{
	if (g_Dpi && g_Dpi->RegisterScaleChangeEvent)
		return g_Dpi->RegisterScaleChangeEvent(hEvent, pdwCookie);

	return S_FALSE;
}

HRESULT MsRdpEx_UnregisterScaleChangeEvent(DWORD_PTR dwCookie)
{
	if (g_Dpi && g_Dpi->UnregisterScaleChangeEvent)
		return g_Dpi->UnregisterScaleChangeEvent(dwCookie);

	return S_FALSE;
}

HRESULT MsRdpEx_SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value)
{
	if (g_Dpi && g_Dpi->SetProcessDpiAwareness)
		return g_Dpi->SetProcessDpiAwareness(value);

	return S_FALSE;
}

HRESULT MsRdpEx_GetProcessDpiAwareness(HANDLE hProcess, PROCESS_DPI_AWARENESS* value)
{
	if (g_Dpi && g_Dpi->GetProcessDpiAwareness)
		return g_Dpi->GetProcessDpiAwareness(hProcess, value);

	return S_FALSE;
}

HRESULT MsRdpEx_GetDpiForMonitor(HMONITOR hMonitor, MONITOR_DPI_TYPE dpiType, uint32_t* dpiX, uint32_t* dpiY)
{
	HDC hDC;
	int logPixelsX = 96;
	int logPixelsY = 96;

	if (g_Dpi && g_Dpi->GetDpiForMonitor)
	{
		return g_Dpi->GetDpiForMonitor(hMonitor, dpiType, dpiX, dpiY);
	}
	else
	{
		hDC = GetDC(NULL);

		if (hDC)
		{
			logPixelsX = GetDeviceCaps(hDC, LOGPIXELSX);
			logPixelsY = GetDeviceCaps(hDC, LOGPIXELSY);
			ReleaseDC(NULL, hDC);
		}

		*dpiX = logPixelsX;
		*dpiY = logPixelsY;

		return S_OK;
	}

	return S_FALSE;
}

uint32_t MsRdpEx_GetDpiForShellUIComponent(SHELL_UI_COMPONENT component)
{
	if (g_Dpi && g_Dpi->GetDpiForShellUIComponent)
		return g_Dpi->GetDpiForShellUIComponent(component);

	return 0;
}

BOOL MsRdpEx_AdjustWindowRectExForDpi(LPRECT lpRect, DWORD dwStyle, BOOL bMenu, DWORD dwExStyle, UINT dpi)
{
	if (g_Dpi && g_Dpi->AdjustWindowRectExForDpi)
	{
		return g_Dpi->AdjustWindowRectExForDpi(lpRect, dwStyle, bMenu, dwExStyle, dpi);
	}
	else
	{
		return AdjustWindowRectEx(lpRect, dwStyle, bMenu, dwExStyle);
	}

	return FALSE;
}

BOOL MsRdpEx_EnableNonClientDpiScaling(HWND hwnd)
{
	if (g_Dpi && g_Dpi->EnableNonClientDpiScaling)
		return g_Dpi->EnableNonClientDpiScaling(hwnd);

	return FALSE;
}
