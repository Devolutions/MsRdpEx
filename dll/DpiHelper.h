#ifndef MSRDPEX_DPI_HELPER_H
#define MSRDPEX_DPI_HELPER_H

#include <MsRdpEx/MsRdpEx.h>

#include <ShTypes.h>

#ifndef SCALING_ENUMS_DECLARED

typedef enum
{
	DEVICE_PRIMARY = 0,
	DEVICE_IMMERSIVE = 1,
} DISPLAY_DEVICE_TYPE;

typedef enum
{
	SCF_VALUE_NONE = 0x00,
	SCF_SCALE = 0x01,
	SCF_PHYSICAL = 0x02,
} SCALE_CHANGE_FLAGS;

DEFINE_ENUM_FLAG_OPERATORS(SCALE_CHANGE_FLAGS);

#define SCALING_ENUMS_DECLARED
#endif

#ifndef DPI_ENUMS_DECLARED

typedef enum PROCESS_DPI_AWARENESS
{
	PROCESS_DPI_UNAWARE = 0,
	PROCESS_SYSTEM_DPI_AWARE = 1,
	PROCESS_PER_MONITOR_DPI_AWARE = 2
} PROCESS_DPI_AWARENESS;

typedef enum MONITOR_DPI_TYPE
{
	MDT_EFFECTIVE_DPI = 0,
	MDT_ANGULAR_DPI = 1,
	MDT_RAW_DPI = 2,
	MDT_DEFAULT = MDT_EFFECTIVE_DPI
} MONITOR_DPI_TYPE;

#define DPI_ENUMS_DECLARED
#endif

#ifndef SHELL_UI_COMPONENT_ENUMS_DECLARED

typedef enum
{
	SHELL_UI_COMPONENT_TASKBARS = 0,
	SHELL_UI_COMPONENT_NOTIFICATIONAREA = 1,
	SHELL_UI_COMPONENT_DESKBAND = 2,
} SHELL_UI_COMPONENT;

#define SHELL_UI_COMPONENT_ENUMS_DECLARED
#endif

#define WM_DPICHANGED 0x02E0
#define WM_GETDPISCALEDSIZE 0x02E4

typedef DEVICE_SCALE_FACTOR(WINAPI * fnGetScaleFactorForDevice)(DISPLAY_DEVICE_TYPE deviceType);
typedef HRESULT(WINAPI * fnRegisterScaleChangeNotifications)(DISPLAY_DEVICE_TYPE displayDevice, HWND hwndNotify,
							     uint32_t uMsgNotify, DWORD* pdwCookie);
typedef HRESULT(WINAPI * fnRevokeScaleChangeNotifications)(DISPLAY_DEVICE_TYPE displayDevice, DWORD dwCookie);
typedef HRESULT(WINAPI * fnGetScaleFactorForMonitor)(HMONITOR hMonitor, DEVICE_SCALE_FACTOR* pScale);
typedef HRESULT(WINAPI * fnRegisterScaleChangeEvent)(HANDLE hEvent, DWORD_PTR* pdwCookie);
typedef HRESULT(WINAPI * fnUnregisterScaleChangeEvent)(DWORD_PTR dwCookie);
typedef HRESULT(WINAPI * fnSetProcessDpiAwareness)(PROCESS_DPI_AWARENESS value);
typedef HRESULT(WINAPI * fnGetProcessDpiAwareness)(HANDLE hProcess, PROCESS_DPI_AWARENESS* value);
typedef HRESULT(WINAPI * fnGetDpiForMonitor)(HMONITOR hMonitor, MONITOR_DPI_TYPE dpiType, uint32_t* dpiX,
					     uint32_t* dpiY);
typedef uint32_t(WINAPI * fnGetDpiForShellUIComponent)(SHELL_UI_COMPONENT component);
typedef BOOL(WINAPI * fnAdjustWindowRectExForDpi)(LPRECT lpRect, DWORD dwStyle, BOOL bMenu, DWORD dwExStyle,
						  UINT dpi);
typedef BOOL(WINAPI * fnEnableNonClientDpiScaling)(HWND hwnd);

struct dpi_helper
{
	HMODULE hShCore;
	HMODULE hUser32;
	fnGetScaleFactorForDevice GetScaleFactorForDevice;
	fnRegisterScaleChangeNotifications RegisterScaleChangeNotifications;
	fnRevokeScaleChangeNotifications RevokeScaleChangeNotifications;
	fnGetScaleFactorForMonitor GetScaleFactorForMonitor;
	fnRegisterScaleChangeEvent RegisterScaleChangeEvent;
	fnUnregisterScaleChangeEvent UnregisterScaleChangeEvent;
	fnSetProcessDpiAwareness SetProcessDpiAwareness;
	fnGetProcessDpiAwareness GetProcessDpiAwareness;
	fnGetDpiForMonitor GetDpiForMonitor;
	fnGetDpiForShellUIComponent GetDpiForShellUIComponent;
	fnAdjustWindowRectExForDpi AdjustWindowRectExForDpi;
	fnEnableNonClientDpiScaling EnableNonClientDpiScaling;
};
typedef struct dpi_helper DpiHelper;

#ifdef __cplusplus
extern "C" {
#endif

DpiHelper* DpiHelper_Get();
void DpiHelper_Release();

float DpiHelper_GetScaleFactor(int dpi);
int DpiHelper_ScaleValue(int value, int dpi);

DEVICE_SCALE_FACTOR MsRdpEx_GetScaleFactorForDevice(DISPLAY_DEVICE_TYPE deviceType);
HRESULT MsRdpEx_RegisterScaleChangeNotifications(DISPLAY_DEVICE_TYPE displayDevice, HWND hwndNotify, uint32_t uMsgNotify,
	                                 DWORD* pdwCookie);
HRESULT MsRdpEx_RevokeScaleChangeNotifications(DISPLAY_DEVICE_TYPE displayDevice, DWORD dwCookie);
HRESULT MsRdpEx_GetScaleFactorForMonitor(HMONITOR hMonitor, DEVICE_SCALE_FACTOR* pScale);
HRESULT MsRdpEx_RegisterScaleChangeEvent(HANDLE hEvent, DWORD_PTR* pdwCookie);
HRESULT MsRdpEx_UnregisterScaleChangeEvent(DWORD_PTR dwCookie);
HRESULT MsRdpEx_SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);
HRESULT MsRdpEx_GetProcessDpiAwareness(HANDLE hProcess, PROCESS_DPI_AWARENESS* value);
HRESULT MsRdpEx_GetDpiForMonitor(HMONITOR hMonitor, MONITOR_DPI_TYPE dpiType, uint32_t* dpiX, uint32_t* dpiY);
uint32_t MsRdpEx_GetDpiForShellUIComponent(SHELL_UI_COMPONENT component);
BOOL MsRdpEx_AdjustWindowRectExForDpi(LPRECT lpRect, DWORD dwStyle, BOOL bMenu, DWORD dwExStyle, UINT dpi);
BOOL MsRdpEx_EnableNonClientDpiScaling(HWND hwnd);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_DPI_HELPER_H */
