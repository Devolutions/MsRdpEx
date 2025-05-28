
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Sspi.h>
#include <MsRdpEx/KeyMaps.h>
#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/RdpInstance.h>
#include <MsRdpEx/Environment.h>
#include <MsRdpEx/Stopwatch.h>

#include <MsRdpEx/OutputMirror.h>

#include <MsRdpEx/Detours.h>

#include <wincred.h>
#include <psapi.h>
#include <intrin.h>
#include <windowsx.h>

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
    bool interceptedCall = false;
    const char* filename = MsRdpEx_FileBase(lpLibFileName);

    if (MsRdpEx_StringIEquals(filename, "winscard.dll")) {
        char* winscardDll = MsRdpEx_GetEnv("MSRDPEX_WINSCARD_DLL");

        if (MsRdpEx_FileExists(winscardDll)) {
            MsRdpEx_LogPrint(DEBUG, "LoadLibraryA: \"%s\" -> \"%s\"", lpLibFileName, winscardDll);
            hModule = Real_LoadLibraryA(winscardDll);
            interceptedCall = true;
        }

        free(winscardDll);
    }
    
    if (!interceptedCall) {
        // LoadLibraryA calls LoadLibraryExW under the hood, don't log here
        //MsRdpEx_LogPrint(DEBUG, "LoadLibraryA: %s", lpLibFileName);
        hModule = Real_LoadLibraryA(lpLibFileName);
    }

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
        // LoadLibraryW calls LoadLibraryExW under the hood, don't log here
        //MsRdpEx_LogPrint(DEBUG, "LoadLibraryW: %s", lpLibFileNameA);
        hModule = Real_LoadLibraryW(lpLibFileName);
    }

    free(lpLibFileNameA);
    free(msrdpexLibraryW);

    return hModule;
}

typedef HMODULE(WINAPI* Func_LoadLibraryExA)(LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags);
typedef HMODULE(WINAPI* Func_LoadLibraryExW)(LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags);

Func_LoadLibraryExA Real_LoadLibraryExA = NULL;
Func_LoadLibraryExW Real_LoadLibraryExW = NULL;

HMODULE WINAPI Hook_LoadLibraryExA(LPCSTR lpLibFileName, HANDLE hFile, DWORD dwFlags)
{
    HMODULE hModule = NULL;

    // LoadLibraryExA calls LoadLibraryExW under the hood, don't log here
    //MsRdpEx_LogPrint(DEBUG, "LoadLibraryExA: %s", lpLibFileName);
    hModule = Real_LoadLibraryExA(lpLibFileName, hFile, dwFlags);

    return hModule;
}

static LPCWSTR LoadLibraryExW_LastFileName = NULL;

HMODULE WINAPI Hook_LoadLibraryExW(LPCWSTR lpLibFileName, HANDLE hFile, DWORD dwFlags)
{
    HMODULE hModule;
    const char* filename;
    char* lpLibFileNameA = NULL;
    bool interceptedCall = false;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpLibFileName, -1, &lpLibFileNameA, 0, NULL, NULL);

    filename = MsRdpEx_FileBase(lpLibFileNameA);

    if (MsRdpEx_StringIEquals(filename, "credssp.dll")) {
        char* credsspDllA = MsRdpEx_GetEnv("MSRDPEX_CREDSSP_DLL");

        if (credsspDllA)
        {
            WCHAR* credsspDllW = NULL;
            MsRdpEx_ConvertToUnicode(CP_UTF8, 0, credsspDllA, -1, &credsspDllW, 0);

            if (credsspDllW) {
                MsRdpEx_LogPrint(DEBUG, "LoadLibraryExW: \"%s\" -> \"%s\"", lpLibFileNameA, credsspDllA);
                hModule = Real_LoadLibraryExW(credsspDllW, hFile, dwFlags);
                interceptedCall = true;
            }

            free(credsspDllW);
        }

        free(credsspDllA);
    }

    if (!interceptedCall)
    {
        // reduce log verbosity for repeated LoadLibraryExW calls
        // only log .dll calls, exclude .exe and .sys which is noise
        if ((lpLibFileName != LoadLibraryExW_LastFileName) &&
            MsRdpEx_IStringEndsWithW(lpLibFileName, L".dll")) {
            MsRdpEx_LogPrint(DEBUG, "LoadLibraryExW: %s", lpLibFileNameA);
        }

        hModule = Real_LoadLibraryExW(lpLibFileName, hFile, dwFlags);
        LoadLibraryExW_LastFileName = lpLibFileName;
    }

    free(lpLibFileNameA);

    return hModule;
}

typedef struct _DELAYLOAD_PROC_DESCRIPTOR
{
    ULONG ImportDescribedByName;
    union
    {
        PCSTR Name;
        ULONG Ordinal;
    } Description;
} DELAYLOAD_PROC_DESCRIPTOR, *PDELAYLOAD_PROC_DESCRIPTOR;

typedef struct _DELAYLOAD_INFO
{
    ULONG Size;
    PCIMAGE_DELAYLOAD_DESCRIPTOR DelayloadDescriptor;
    PIMAGE_THUNK_DATA ThunkAddress;
    PCSTR TargetDllName;
    DELAYLOAD_PROC_DESCRIPTOR TargetApiDescriptor;
    PVOID TargetModuleBase;
    PVOID Unused;
    ULONG LastError;
} DELAYLOAD_INFO, *PDELAYLOAD_INFO;

typedef PVOID(NTAPI* PDELAYLOAD_FAILURE_DLL_CALLBACK)(ULONG NotificationReason, PDELAYLOAD_INFO DelayloadInfo);

typedef PVOID(NTAPI* PDELAYLOAD_FAILURE_SYSTEM_ROUTINE)(PCSTR DllName, PCSTR ProcedureName);

typedef PVOID(NTAPI* Func_LdrResolveDelayLoadedAPI)(PVOID ParentModuleBase,
    PCIMAGE_DELAYLOAD_DESCRIPTOR DelayloadDescriptor,
    PDELAYLOAD_FAILURE_DLL_CALLBACK FailureDllHook,
    PDELAYLOAD_FAILURE_SYSTEM_ROUTINE FailureSystemHook,
    PIMAGE_THUNK_DATA ThunkAddress, ULONG Flags);

Func_LdrResolveDelayLoadedAPI Real_LdrResolveDelayLoadedAPI = NULL;

PVOID NTAPI Hook_LdrResolveDelayLoadedAPI(PVOID ParentModuleBase,
    PCIMAGE_DELAYLOAD_DESCRIPTOR DelayloadDescriptor,
    PDELAYLOAD_FAILURE_DLL_CALLBACK FailureDllHook,
    PDELAYLOAD_FAILURE_SYSTEM_ROUTINE FailureSystemHook,
    PIMAGE_THUNK_DATA ThunkAddress, ULONG Flags)
{
    PVOID ldrResult = NULL;
    const char* DllName = (const char*) &((uint8_t*)ParentModuleBase)[DelayloadDescriptor->DllNameRVA];

    MsRdpEx_LogPrint(DEBUG, "LdrResolveDelayLoadedAPI: DllName: %s", DllName);

    ldrResult = Real_LdrResolveDelayLoadedAPI(ParentModuleBase,
        DelayloadDescriptor, FailureDllHook, FailureSystemHook, ThunkAddress, Flags);
    
    return ldrResult;
}

typedef struct _UNICODE_STRING
{
	USHORT Length;
	USHORT MaximumLength;
	PWSTR  Buffer;
} UNICODE_STRING, * PUNICODE_STRING;

typedef struct _OBJECT_ATTRIBUTES
{
	ULONG Length;
	HANDLE RootDirectory;
	PUNICODE_STRING ObjectName;
	ULONG Attributes;
	PVOID SecurityDescriptor;
	PVOID SecurityQualityOfService;
} OBJECT_ATTRIBUTES, * POBJECT_ATTRIBUTES;

typedef struct _IO_STATUS_BLOCK {
	union {
		NTSTATUS Status;
		PVOID Pointer;
	};
	ULONG_PTR Information;
} IO_STATUS_BLOCK, * PIO_STATUS_BLOCK;

typedef NTSTATUS (NTAPI * Func_NtCreateFile)(
	PHANDLE FileHandle, ACCESS_MASK DesiredAccess,
	POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock, PLARGE_INTEGER AllocationSize,
	ULONG FileAttributes, ULONG ShareAccess, ULONG CreateDisposition, ULONG CreateOptions,
	PVOID EaBuffer, ULONG EaLength);

Func_NtCreateFile Real_NtCreateFile = NULL;

NTSTATUS NTAPI Hook_NtCreateFile(PHANDLE FileHandle, ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock, PLARGE_INTEGER AllocationSize,
    ULONG FileAttributes, ULONG ShareAccess, ULONG CreateDisposition, ULONG CreateOptions,
    PVOID EaBuffer, ULONG EaLength)
{
    NTSTATUS ntstatus;
    char* pObjectNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, ObjectAttributes->ObjectName->Buffer,
        ObjectAttributes->ObjectName->Length, &pObjectNameA, 0, NULL, NULL);

    MsRdpEx_LogPrint(DEBUG, "NtCreateFile: %s", pObjectNameA);

    ntstatus = Real_NtCreateFile(FileHandle, DesiredAccess,
        ObjectAttributes, IoStatusBlock, AllocationSize,
        FileAttributes, ShareAccess, CreateDisposition,
        CreateOptions, EaBuffer, EaLength);

    free(pObjectNameA);

    return ntstatus;
}

typedef NTSTATUS(NTAPI* Func_NtOpenFile)(PHANDLE FileHandle, ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock,
    ULONG ShareAccess, ULONG OpenOptions);

Func_NtOpenFile Real_NtOpenFile = NULL;

NTSTATUS NTAPI Hook_NtOpenFile(PHANDLE FileHandle, ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock,
    ULONG ShareAccess, ULONG OpenOptions)
{
    NTSTATUS ntstatus;
    char* pObjectNameA = NULL;
    bool interceptedCall = false;

    if (ObjectAttributes && ObjectAttributes->ObjectName && ObjectAttributes->ObjectName->Buffer &&
        MsRdpEx_IStringEndsWithW(ObjectAttributes->ObjectName->Buffer, L"WinSCard.dll"))
    {
        char* winscardDll = MsRdpEx_GetEnv("MSRDPEX_WINSCARD_DLL");

        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, ObjectAttributes->ObjectName->Buffer,
            ObjectAttributes->ObjectName->Length, &pObjectNameA, 0, NULL, NULL);

        if (MsRdpEx_FileExists(winscardDll)) {
            char NewFilePathA[MSRDPEX_MAX_PATH];
            WCHAR* NewFilePathW = NULL;
            OBJECT_ATTRIBUTES NewObjectAttributes;
            UNICODE_STRING NewObjectName;

            sprintf_s(NewFilePathA, MSRDPEX_MAX_PATH, "\\??\\%s", winscardDll);
            MsRdpEx_ConvertToUnicode(CP_UTF8, 0, NewFilePathA, -1, &NewFilePathW, 0);
            CopyMemory(&NewObjectAttributes, ObjectAttributes, sizeof(OBJECT_ATTRIBUTES));
            NewObjectName.Buffer = NewFilePathW;
            NewObjectName.Length = wcslen(NewFilePathW) * 2;
            NewObjectName.MaximumLength = NewObjectAttributes.ObjectName->Length;
            NewObjectAttributes.ObjectName = &NewObjectName;

            MsRdpEx_LogPrint(DEBUG, "NtOpenFile: replacing %s with %s", pObjectNameA, NewFilePathA);
            ntstatus = Real_NtOpenFile(FileHandle, DesiredAccess, &NewObjectAttributes, IoStatusBlock, ShareAccess, OpenOptions);
            interceptedCall = true;

            free(NewFilePathW);
        }

        free(winscardDll);
    }

    if (!interceptedCall)
    {
        ntstatus = Real_NtOpenFile(FileHandle, DesiredAccess, ObjectAttributes, IoStatusBlock, ShareAccess, OpenOptions);
    }

    free(pObjectNameA);

    return ntstatus;
}

FARPROC(WINAPI* Real_GetProcAddress)(HMODULE hModule, LPCSTR lpProcName) = GetProcAddress;

FARPROC WINAPI Hook_GetProcAddress(HMODULE hModule, LPCSTR lpProcName)
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

bool WINAPI MsRdpEx_CaptureBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY)
{
    RECT rect = { 0 };
    uint32_t frameWidth = 0;
    uint32_t frameHeight = 0;
    bool captured = false;
    bool outputMirrorEnabled = false;
    bool videoRecordingEnabled = false;
    uint32_t videoRecordingQuality = 5;
    bool dumpBitmapUpdates = false;
    IMsRdpExInstance* instance = NULL;
    MsRdpEx_OutputMirror* outputMirror = NULL;
    CMsRdpExtendedSettings* pExtendedSettings = NULL;

    HWND hWnd = WindowFromDC(hdcDst);

    if (!hWnd)
        goto end;

    instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

    if (!instance)
        goto end;

    if (instance)
        instance->GetExtendedSettings(&pExtendedSettings);

    if (!pExtendedSettings)
        goto end;

    MsRdpEx_LogPrint(TRACE, "CaptureBlt: hWnd: %p instance: %p", hWnd, instance);

    outputMirrorEnabled = pExtendedSettings->GetOutputMirrorEnabled();
    videoRecordingEnabled = pExtendedSettings->GetVideoRecordingEnabled();
    videoRecordingQuality = pExtendedSettings->GetVideoRecordingQuality();
    dumpBitmapUpdates = pExtendedSettings->GetDumpBitmapUpdates();

    if (!outputMirrorEnabled)
        goto end;

    if (!GetClientRect(hWnd, &rect))
        goto end;

    LONG bitmapWidth = MsRdpEx_GetRectWidth(&rect);
    LONG bitmapHeight = MsRdpEx_GetRectHeight(&rect);

    instance->GetOutputMirrorObject((LPVOID*) &outputMirror);

    if (!outputMirror) 
    {
        outputMirror = MsRdpEx_OutputMirror_New();
        MsRdpEx_OutputMirror_SetDumpBitmapUpdates(outputMirror, dumpBitmapUpdates);
        MsRdpEx_OutputMirror_SetVideoRecordingEnabled(outputMirror, videoRecordingEnabled);
        MsRdpEx_OutputMirror_SetVideoQualityLevel(outputMirror, videoRecordingQuality);

        char* recordingPath = pExtendedSettings->GetRecordingPath();
        if (recordingPath) {
            MsRdpEx_OutputMirror_SetRecordingPath(outputMirror, recordingPath);
            free(recordingPath);
        }

        char* recordingPipeName = pExtendedSettings->GetRecordingPipeName();
        if (recordingPipeName) {
            MsRdpEx_OutputMirror_SetRecordingPipeName(outputMirror, recordingPipeName);
            free(recordingPipeName);
        }

        const char* sessionId = pExtendedSettings->GetSessionId();
        MsRdpEx_OutputMirror_SetSessionId(outputMirror, sessionId);

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

    MsRdpEx_OutputMirror_Lock(outputMirror);
    HDC hShadowDC = MsRdpEx_OutputMirror_GetShadowDC(outputMirror);
    BitBlt(hShadowDC, dstX, dstY, width, height, hdcSrc, srcX, srcY, SRCCOPY);
    MsRdpEx_OutputMirror_Unlock(outputMirror);
    MsRdpEx_OutputMirror_DumpFrame(outputMirror);

    captured = true;
end:
    return captured;
}

BOOL WINAPI Hook_BitBlt(
    HDC hdcDst, int dstX, int dstY, int width, int height,
    HDC hdcSrc, int srcX, int srcY, DWORD rop)
{
    BOOL status;
    MsRdpEx_Stopwatch stopwatch;

    status = Real_BitBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY, rop);

    MsRdpEx_Stopwatch_Init(&stopwatch, MSRDPEX_PROF_TRACE, true);
    bool captured = MsRdpEx_CaptureBlt(hdcDst, dstX, dstY, width, height, hdcSrc, srcX, srcY);

    if (captured) 
    {
        MsRdpEx_LogPrint(TRACE, "BitBlt: %d,%d %dx%d %d,%d [%.3fms]", dstX, dstY, width, height, srcX, srcY, MsRdpEx_Stopwatch_GetTime(&stopwatch));
    }

    return status;
}

BOOL (WINAPI * Real_StretchBlt)(
    HDC hdcDest, int xDest, int yDest, int wDest, int hDest,
    HDC hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, DWORD rop
    ) = StretchBlt;

BOOL WINAPI Hook_StretchBlt(
    HDC hdcDst, int dstX, int dstY, int dstW, int dstH,
    HDC hdcSrc, int srcX, int srcY, int srcW, int srcH, DWORD rop)
{
    BOOL status;
    MsRdpEx_Stopwatch stopwatch;

    status = Real_StretchBlt(hdcDst, dstX, dstY, dstW, dstH, hdcSrc, srcX, srcY, srcW, srcH, rop);

    MsRdpEx_Stopwatch_Init(&stopwatch, MSRDPEX_PROF_TRACE, true);
    bool captured = MsRdpEx_CaptureBlt(hdcDst, srcX, srcY, srcW, srcH, hdcSrc, srcX, srcY);

    if (captured) 
    {
        MsRdpEx_LogPrint(TRACE, "StretchBlt: %d,%d %dx%d %d,%d %dx%d [%.3fms])", dstX, dstY, dstW, dstH, srcX, srcY, srcW, srcH, MsRdpEx_Stopwatch_GetTime(&stopwatch));
    }

end:
    return status;
}

#define SYSMENU_RDP_RANGE_FIRST_ID               7100
#define SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID         7101
#define SYSMENU_RDP_SEND_CTRL_ALT_END_ID         7102
#define SYSMENU_RDP_RESIZE_TO_FIT_WINDOW_ID      7103
#define SYSMENU_RDP_RANGE_LAST_ID                7104

static WNDPROC Real_TscShellContainerWndProc = NULL;

static HWND g_hTscShellContainerWnd = NULL;

LRESULT CALLBACK Hook_TscShellContainerWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    LRESULT result;

    CMsRdpExInstance* instance = MsRdpEx_InstanceManager_FindByTscShellContainerWnd(hWnd);

    //MsRdpEx_LogPrint(DEBUG, "TscShellContainerWnd: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

    if (uMsg == WM_COMMAND)
        uMsg = WM_SYSCOMMAND; // TrackPopupMenu sends WM_COMMAND instead of WM_SYSCOMMAND

    result = Real_TscShellContainerWndProc(hWnd, uMsg, wParam, lParam);

    if (uMsg == WM_NCCREATE)
    {
        g_hTscShellContainerWnd = hWnd;
    }

    if (instance)
    {
        HWND hInputCaptureWnd = 0;

        ((IMsRdpExInstance*)instance)->GetInputWindow(&hInputCaptureWnd);

        if (hInputCaptureWnd)
        {
            if (uMsg == WM_SYSCOMMAND)
            {
                if ((wParam >= SYSMENU_RDP_RANGE_FIRST_ID) && (wParam <= SYSMENU_RDP_RANGE_LAST_ID)) {
                    SendMessage(hInputCaptureWnd, WM_SYSCOMMAND, wParam, lParam);
                }
            }
        }
    }

    return result;
}

static WNDPROC Real_BBarWndProc = NULL;

static HMENU g_hExtraMenu = NULL;

LRESULT CALLBACK Hook_BBarWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    LRESULT result;

    //MsRdpEx_LogPrint(DEBUG, "BBarWnd: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

    result = Real_BBarWndProc(hWnd, uMsg, wParam, lParam);

    if (uMsg == WM_CONTEXTMENU)
    {
        if (g_hExtraMenu && g_hTscShellContainerWnd)
        {
            int xPos = GET_X_LPARAM(lParam);
            int yPos = GET_Y_LPARAM(lParam);
            HMENU hSystemMenu = GetSystemMenu(g_hTscShellContainerWnd, FALSE);
            TrackPopupMenu(hSystemMenu, TPM_RIGHTBUTTON, xPos, yPos, 0, g_hTscShellContainerWnd, NULL);
        }
    }

    return result;
}

static WNDPROC Real_OPWndProc_mstscax = NULL;
static WNDPROC Real_OPWndProc_rdclientax = NULL;

extern void MsRdpEx_OutputWindow_OnCreate(HWND hWnd, void* pUserData);

LRESULT CALLBACK Hook_OPWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL isOOBClient)
{
	LRESULT result;
	char* lpWindowNameA = NULL;
    CMsRdpExInstance* instance = NULL;
	
	MsRdpEx_LogPrint(DEBUG, "OPWndProc: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

	if (uMsg == WM_NCCREATE)
	{
		CREATESTRUCTW* createStruct = (CREATESTRUCTW*) lParam;
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, createStruct->lpszName, -1, &lpWindowNameA, 0, NULL, NULL);
        MsRdpEx_LogPrint(DEBUG, "Window Create: %s", lpWindowNameA);
	}

    if (isOOBClient) {
        result = Real_OPWndProc_rdclientax(hWnd, uMsg, wParam, lParam);
    } else {
        result = Real_OPWndProc_mstscax(hWnd, uMsg, wParam, lParam);
    }

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

LRESULT CALLBACK Hook_OPWndProc_mstscax(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    return Hook_OPWndProc(hWnd, uMsg, wParam, lParam, FALSE);
}

LRESULT CALLBACK Hook_OPWndProc_rdclientax(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    return Hook_OPWndProc(hWnd, uMsg, wParam, lParam, TRUE);
}

ATOM (WINAPI * Real_RegisterClassExW)(const WNDCLASSEXW* wndClassEx) = RegisterClassExW;

ATOM WINAPI Hook_RegisterClassExW(WNDCLASSEXW* wndClass)
{
    ATOM wndClassAtom;
    char* lpClassNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, wndClass->lpszClassName, -1, &lpClassNameA, 0, NULL, NULL);

    MsRdpEx_LogPrint(DEBUG, "RegisterClassExW: %s", lpClassNameA);

    if (MsRdpEx_StringEquals(lpClassNameA, "TscShellContainerClass")) {
        Real_TscShellContainerWndProc = wndClass->lpfnWndProc;
        wndClass->lpfnWndProc = Hook_TscShellContainerWndProc;
    }
    else if (MsRdpEx_StringEquals(lpClassNameA, "BBarWindowClass")) {
        Real_BBarWndProc = wndClass->lpfnWndProc;
        wndClass->lpfnWndProc = Hook_BBarWndProc;
    }
    else if (MsRdpEx_StringEquals(lpClassNameA, "OPWindowClass")) {
        if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
            Real_OPWndProc_rdclientax = wndClass->lpfnWndProc;
            wndClass->lpszClassName = L"OPWindowClass_rdclientax";
            wndClass->lpfnWndProc = Hook_OPWndProc_rdclientax;
        } else {
            Real_OPWndProc_mstscax = wndClass->lpfnWndProc;
            wndClass->lpszClassName = L"OPWindowClass_mstscax";
            wndClass->lpfnWndProc = Hook_OPWndProc_mstscax;
        }
    }

    wndClassAtom = Real_RegisterClassExW(wndClass);

    free(lpClassNameA);

    return wndClassAtom;
}

static WNDPROC Real_IHWndProc_mstscax = NULL;
static WNDPROC Real_IHWndProc_rdclientax = NULL;

#define MOUSE_JIGGLER_MOVE_MOUSE_TIMER_ID        4301
#define MOUSE_JIGGLER_SPECIAL_KEY_TIMER_ID       4302

LRESULT CALLBACK Hook_IHWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL isOOBClient)
{
    LRESULT result;
    char* lpWindowNameA = NULL;
    IMsRdpExInstance* instance = NULL;
    CMsRdpExtendedSettings* pExtendedSettings = NULL;

    //MsRdpEx_LogPrint(DEBUG, "IHWndProc: %s (%d)", MsRdpEx_GetWindowMessageName(uMsg), uMsg);

    if (uMsg == WM_NCCREATE)
    {
        CREATESTRUCTW* createStruct = (CREATESTRUCTW*)lParam;
        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, createStruct->lpszName, -1, &lpWindowNameA, 0, NULL, NULL);
        MsRdpEx_LogPrint(DEBUG, "Window Create: %s", lpWindowNameA);
    }
    else
    {
        instance = (IMsRdpExInstance*)MsRdpEx_InstanceManager_FindByInputCaptureHwnd(hWnd);

        if (instance)
            instance->GetExtendedSettings(&pExtendedSettings);
    }

    if (isOOBClient) {
        result = Real_IHWndProc_rdclientax(hWnd, uMsg, wParam, lParam);
    } else {
        result = Real_IHWndProc_mstscax(hWnd, uMsg, wParam, lParam);
    }

    if (uMsg == WM_NCCREATE)
    {
        CREATESTRUCTW* createStruct = (CREATESTRUCTW*)lParam;
        void* pUserData = createStruct->lpCreateParams;

        instance = (IMsRdpExInstance*)MsRdpEx_InstanceManager_AttachInputWindow(hWnd, pUserData);

        if (instance)
        {
            instance->GetExtendedSettings(&pExtendedSettings);

            if (pExtendedSettings)
            {
                bool mouseJigglerEnabled = pExtendedSettings->GetMouseJigglerEnabled();
                uint32_t mouseJigglerInterval = pExtendedSettings->GetMouseJigglerInterval();
                uint32_t mouseJigglerMethod = pExtendedSettings->GetMouseJigglerMethod();

                MsRdpEx_LogPrint(DEBUG, "Mouse Jiggler: Enabled=%d, Interval=%d, Method=%d",
                    mouseJigglerEnabled ? 1 : 0, mouseJigglerInterval, mouseJigglerMethod);

                if (mouseJigglerEnabled)
                {
                    uint32_t timerEventId = MOUSE_JIGGLER_MOVE_MOUSE_TIMER_ID;

                    switch (mouseJigglerMethod)
                    {
                    case 0:
                        timerEventId = MOUSE_JIGGLER_MOVE_MOUSE_TIMER_ID;
                        break;

                    case 1:
                        timerEventId = MOUSE_JIGGLER_SPECIAL_KEY_TIMER_ID;
                        break;
                    }

                    SetTimer(hWnd, timerEventId, mouseJigglerInterval * 1000, NULL);
                }

                if (pExtendedSettings->GetExtraSystemMenuEnabled())
                {
                    char className[256];
                    HWND hParentWnd = GetAncestor(hWnd, GA_ROOT);
                    GetClassNameA(hParentWnd, className, sizeof(className) - 1);

                    if (MsRdpEx_StringEquals(className, "TscShellContainerClass"))
                    {
                        MsRdpEx_LogPrint(DEBUG, "Ancestor: %s hWnd: %p", className, hParentWnd);

                        HMENU hSystemMenu = GetSystemMenu(hParentWnd, FALSE);
                        HMENU hExtraMenu = CreateMenu();
                        AppendMenu(hExtraMenu, MF_STRING, SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID, L"Send Ctrl+Alt+Del");
                        AppendMenu(hExtraMenu, MF_STRING, SYSMENU_RDP_SEND_CTRL_ALT_END_ID, L"Send Ctrl+Alt+End");
                        AppendMenu(hExtraMenu, MF_SEPARATOR, 0, NULL);
                        AppendMenu(hExtraMenu, MF_STRING, SYSMENU_RDP_RESIZE_TO_FIT_WINDOW_ID, L"Resize to fit window");

                        AppendMenu(hSystemMenu, MF_SEPARATOR, 0, NULL);
                        AppendMenu(hSystemMenu, MF_POPUP, (::UINT_PTR)hExtraMenu, L"Extra");

                        g_hExtraMenu = hExtraMenu;

                        instance->AttachTscShellContainerWindow(hParentWnd);
                    }
                }
            }
        }
        else
        {
            MsRdpEx_LogPrint(DEBUG, "Failed to attach input window! hWnd: %p pUserData: %p", hWnd, pUserData);
        }
    }
    else if (uMsg == WM_SYSCOMMAND)
    {
        switch (wParam)
        {
            case SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID:
                MsRdpEx_LogPrint(DEBUG, "Send Ctrl+Alt+Del");
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_CONTROL, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_MENU, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_DELETE, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_DELETE, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_MENU, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_CONTROL, (LPARAM)0x0);
                break;

            case SYSMENU_RDP_SEND_CTRL_ALT_END_ID:
                MsRdpEx_LogPrint(DEBUG, "Send Ctrl+Alt+End");
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_CONTROL, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_MENU, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)VK_END, (LPARAM)0x01000000);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_END, (LPARAM)0x01000000);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_MENU, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)VK_CONTROL, (LPARAM)0x0);
                break;

            case SYSMENU_RDP_RESIZE_TO_FIT_WINDOW_ID:
                MsRdpEx_LogPrint(DEBUG, "Resize to fit window");
                {
                    IUnknown* pUnknown = NULL;
                    IMsRdpClient9* pMsRdpClient9 = NULL;

                    if (instance)
                        instance->GetRdpClient((LPVOID*)&pUnknown);

                    if (pUnknown)
                        pUnknown->QueryInterface(IID_IMsRdpClient9, (LPVOID*)&pMsRdpClient9);

                    HWND hUIContainerWnd = GetParent(hWnd);
                    HWND hUIMainWnd = GetParent(hUIContainerWnd);

                    if (pMsRdpClient9 && hUIMainWnd)
                    {
                        RECT clientRect;
                        GetClientRect(hUIMainWnd, &clientRect);

                        ULONG ulDesktopWidth = clientRect.right - clientRect.left;
                        ULONG ulDesktopHeight = clientRect.bottom - clientRect.top;
                        ULONG ulPhysicalWidth = ulDesktopWidth;
                        ULONG ulPhysicalHeight = ulDesktopHeight;
                        ULONG ulOrientation = 0;
                        ULONG ulDesktopScaleFactor = 100;
                        ULONG ulDeviceScaleFactor = 100;

                        MsRdpEx_LogPrint(DEBUG, "UpdateSessionDisplaySettings(%dx%d)",
                            ulDesktopWidth, ulDesktopHeight);

                        pMsRdpClient9->UpdateSessionDisplaySettings(
                            ulDesktopWidth,
                            ulDesktopHeight,
                            ulPhysicalWidth,
                            ulPhysicalHeight,
                            ulOrientation,
                            ulDesktopScaleFactor,
                            ulDeviceScaleFactor);
                    }

                    if (pMsRdpClient9)
                        pMsRdpClient9->Release();
                }
                break;
        }
    }
    else if (uMsg == WM_TIMER)
    {
        if (wParam == MOUSE_JIGGLER_MOVE_MOUSE_TIMER_ID)
        {
            if (instance)
            {
                int32_t oldPosX = 0;
                int32_t oldPosY = 0;
                instance->GetLastMousePosition(&oldPosX, &oldPosY);

                int32_t newPosX = oldPosX + 1;
                int32_t newPosY = oldPosY + 1;
                SendMessage(hWnd, WM_MOUSEMOVE, 0, MAKELPARAM(newPosX, newPosY));
                SendMessage(hWnd, WM_MOUSEMOVE, 0, MAKELPARAM(oldPosX, oldPosY));
            }
        }
        else if (wParam == MOUSE_JIGGLER_SPECIAL_KEY_TIMER_ID)
        {
            if (instance)
            {
                uint32_t specialKey = VK_F15; // 'F15' is ignored by most applications
                SendMessage(hWnd, WM_KEYDOWN, (WPARAM)specialKey, (LPARAM)0x0);
                SendMessage(hWnd, WM_KEYUP, (WPARAM)specialKey, (LPARAM)0x0);
            }
        }
    }
    else if (uMsg == WM_MOUSEMOVE)
    {
        int32_t mousePosX = GET_X_LPARAM(lParam);
        int32_t mousePosY = GET_Y_LPARAM(lParam);

        if (instance)
        {
            instance->SetLastMousePosition(mousePosX, mousePosY);
        }
    }
    else if (uMsg == WM_KEYDOWN)
    {
        // https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientsecuredsettings-keyboardhookmode

        bool ctrlAltDown = ((GetKeyState(VK_CONTROL) & 0x8000) && (GetKeyState(VK_MENU) & 0x8000));
        bool KeyboardHookToggleShortcutEnabled = pExtendedSettings ? pExtendedSettings->GetKeyboardHookToggleShortcutEnabled() : false;
        const char* KeyboardHookToggleShortcutKey = pExtendedSettings ? pExtendedSettings->GetKeyboardHookToggleShortcutKey() : "";

        if (KeyboardHookToggleShortcutEnabled && ctrlAltDown &&
            (wParam == MsRdpEx_KeyNameToVKCode(KeyboardHookToggleShortcutKey)))
        {
            IUnknown* pUnknown = NULL;
            IMsRdpClient9* pMsRdpClient9 = NULL;
            IMsRdpClientSecuredSettings2* pMsRdpClientSecuredSettings = NULL;

            if (instance)
                instance->GetRdpClient((LPVOID*)&pUnknown);

            if (pUnknown)
                pUnknown->QueryInterface(IID_IMsRdpClient9, (LPVOID*)&pMsRdpClient9);

            if (pMsRdpClient9)
                pMsRdpClient9->get_SecuredSettings3(&pMsRdpClientSecuredSettings);

            LONG keyboardHookMode = 0;
            pMsRdpClientSecuredSettings->get_KeyboardHookMode(&keyboardHookMode);

            // toggle keyboard hook mode between local and remote

            switch (keyboardHookMode)
            {
                case 0: // Apply key combinations only locally at the client computer.
                    keyboardHookMode = 1;
                    break;

                case 1: // Apply key combinations at the remote server.
                case 2: // Apply key combinations to the remote server only when the client is running in full-screen mode
                default:
                    keyboardHookMode = 0;
                    break;
            }

            MsRdpEx_LogPrint(DEBUG, "New KeyboardHookMode: %d", keyboardHookMode);
            pMsRdpClientSecuredSettings->put_KeyboardHookMode(keyboardHookMode);

            VARIANT propValue;
            VariantInit(&propValue);
            propValue.vt = VT_I4;
            propValue.intVal = keyboardHookMode;
            bstr_t propName = _com_util::ConvertStringToBSTR("KeyboardHookMode");
            pExtendedSettings->put_BaseProperty(propName, &propValue);

            if (pMsRdpClient9)
                pMsRdpClient9->Release();

            if (pMsRdpClientSecuredSettings)
                pMsRdpClientSecuredSettings->Release();
        }
    }
    else if (uMsg == WM_KEYUP)
    {

    }
    else if (uMsg == WM_NCDESTROY)
    {
        KillTimer(hWnd, MOUSE_JIGGLER_MOVE_MOUSE_TIMER_ID);
        KillTimer(hWnd, MOUSE_JIGGLER_SPECIAL_KEY_TIMER_ID);
    }

    free(lpWindowNameA);

    return result;
}

LRESULT CALLBACK Hook_IHWndProc_rdclientax(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    return Hook_IHWndProc(hWnd, uMsg, wParam, lParam, TRUE);
}

LRESULT CALLBACK Hook_IHWndProc_mstscax(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    return Hook_IHWndProc(hWnd, uMsg, wParam, lParam, FALSE);
}

ATOM(WINAPI* Real_RegisterClassW)(const WNDCLASSW* wndClass) = RegisterClassW;

ATOM WINAPI Hook_RegisterClassW(WNDCLASSW* wndClass)
{
    ATOM wndClassAtom;
    char* lpClassNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, wndClass->lpszClassName, -1, &lpClassNameA, 0, NULL, NULL);

    MsRdpEx_LogPrint(DEBUG, "RegisterClassW: %s", lpClassNameA);

    if (MsRdpEx_StringEquals(lpClassNameA, "IHWindowClass")) {
        if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
            Real_IHWndProc_rdclientax = wndClass->lpfnWndProc;
            wndClass->lpszClassName = L"IHWindowClass_rdclientax";
            wndClass->lpfnWndProc = Hook_IHWndProc_rdclientax;
        } else {
            Real_IHWndProc_mstscax = wndClass->lpfnWndProc;
            wndClass->lpszClassName = L"IHWindowClass_mstscax";
            wndClass->lpfnWndProc = Hook_IHWndProc_mstscax;
        }
    }

    wndClassAtom = Real_RegisterClassW(wndClass);

    free(lpClassNameA);

    return wndClassAtom;
}

BOOL(WINAPI* Real_GetClassInfoW)(HINSTANCE hInstance, LPCWSTR lpClassName, LPWNDCLASSW lpWndClass) = GetClassInfoW;

BOOL WINAPI Hook_GetClassInfoW(HINSTANCE hInstance, LPCWSTR lpClassName, LPWNDCLASSW lpWndClass)
{
    BOOL result;

    if (lpClassName && (((ULONG_PTR)lpClassName) > 0xFFFF))
    {
        if (MsRdpEx_StringEqualsW(lpClassName, L"IHWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"IHWindowClass_rdclientax";
            } else {
                lpClassName = L"IHWindowClass_mstscax";
            }
        } else if (MsRdpEx_StringEqualsW(lpClassName, L"OPWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"OPWindowClass_rdclientax";
            } else {
                lpClassName = L"OPWindowClass_mstscax";
            }
        }
    }

    result = Real_GetClassInfoW(hInstance, lpClassName, lpWndClass);

    return result;
}

BOOL(WINAPI* Real_UnregisterClassW)(LPCWSTR lpClassName, HINSTANCE hInstance) = UnregisterClassW;

BOOL WINAPI Hook_UnregisterClassW(LPCWSTR lpClassName, HINSTANCE hInstance)
{
    BOOL result;

    if (lpClassName && (((ULONG_PTR)lpClassName) > 0xFFFF))
    {
        if (MsRdpEx_StringEqualsW(lpClassName, L"IHWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"IHWindowClass_rdclientax";
            } else {
                lpClassName = L"IHWindowClass_mstscax";
            }
        } else if (MsRdpEx_StringEqualsW(lpClassName, L"OPWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"OPWindowClass_rdclientax";
            } else {
                lpClassName = L"OPWindowClass_mstscax";
            }
        }
    }

    result = Real_UnregisterClassW(lpClassName, hInstance);

    return result;
}

HWND(WINAPI* Real_CreateWindowExW)(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle,
    int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam) = CreateWindowExW;

HWND WINAPI Hook_CreateWindowExW(DWORD dwExStyle, LPCWSTR lpClassName, LPCWSTR lpWindowName, DWORD dwStyle,
    int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam)
{
    HWND hWnd;
    char* lpClassNameA = NULL;

    if (lpClassName && (((ULONG_PTR)lpClassName) > 0xFFFF))
    {
        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpClassName, -1, &lpClassNameA, 0, NULL, NULL);

        MsRdpEx_LogPrint(TRACE, "CreateWindowExW: %s", lpClassNameA);

        if (MsRdpEx_StringEqualsW(lpClassName, L"OPWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"OPWindowClass_rdclientax";
            } else {
                lpClassName = L"OPWindowClass_mstscax";
            }
        } else if (MsRdpEx_StringEqualsW(lpClassName, L"IHWindowClass")) {
            if (MsRdpEx_IsAddressInRdclientAxModule(_ReturnAddress())) {
                lpClassName = L"IHWindowClass_rdclientax";
            } else {
                lpClassName = L"IHWindowClass_mstscax";
            }
        }
    }

    hWnd = Real_CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

    free(lpClassNameA);

    return hWnd;
}

#define ID_MAINDLG_COMPUTER_TEXTBOX     5012

static DLGPROC Real_MainDlgProc = NULL;

HWND MsRdpEx_GetMsgWindowHandle()
{
    char buffer[32];
    HWND hWndMsg = NULL;

    if (GetEnvironmentVariableA("MSRDPEX_HWNDMSG", buffer, sizeof(buffer)) > 0)
    {
        hWndMsg = (HWND)_strtoui64(buffer, NULL, 0);
    }

    return hWndMsg;
}

INT_PTR CALLBACK Hook_MainDlgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    INT_PTR dlgResult = FALSE;
    bool interceptedCall = false;

    HWND hWndMsg = MsRdpEx_GetMsgWindowHandle();

    if (hWndMsg)
    {
        if (uMsg == WM_DESTROY)
        {
            MsRdpEx_LogPrint(DEBUG, "MainDlgProc WM_DESTROY");
            PostMessage(hWndMsg, 0x0401, NULL, NULL);
        }
        else if (uMsg == WM_COMMAND)
        {
            MsRdpEx_LogPrint(DEBUG, "MainDlgProc WM_COMMAND: %d", (int)wParam);

            if (wParam == IDOK) // "Connect" button
            {
                char connectionStringA[256];
                GetDlgItemTextA(hWnd, ID_MAINDLG_COMPUTER_TEXTBOX, connectionStringA, 256);
                MsRdpEx_LogPrint(DEBUG, "MainDlgProc: Connect(%s)", connectionStringA);

                PostMessage(hWndMsg, 0x0402, NULL, NULL);
                interceptedCall = true;
                dlgResult = TRUE;
            }
        }
    }

    if (!interceptedCall) {
        dlgResult = Real_MainDlgProc(hWnd, uMsg, wParam, lParam);
    }

    return dlgResult;
}

HWND (WINAPI* Real_CreateDialogParamW)(HINSTANCE hInstance,
    LPCWSTR lpTemplateName, HWND hWndParent,
    DLGPROC lpDialogFunc, LPARAM dwInitParam) = CreateDialogParamW;

HWND WINAPI Hook_CreateDialogParamW(HINSTANCE hInstance,
    LPCWSTR lpTemplateName, HWND hWndParent,
    DLGPROC lpDialogFunc, LPARAM dwInitParam)
{
    HWND hWndDialog;
    HWND hWndMsg = MsRdpEx_GetMsgWindowHandle();

    if (hWndMsg)
    {
        if (lpTemplateName == MAKEINTRESOURCE(15001)) {
            MsRdpEx_LogPrint(DEBUG, "CreateDialogParamW: CMainDlg");
            Real_MainDlgProc = lpDialogFunc;
            lpDialogFunc = Hook_MainDlgProc;
        }
    }

    hWndDialog = Real_CreateDialogParamW(hInstance, lpTemplateName, hWndParent, lpDialogFunc, dwInitParam);

    return hWndDialog;
}

BOOL (WINAPI * Real_CredReadW)(LPCWSTR TargetName, DWORD Type, DWORD Flags, PCREDENTIALW* Credential) = CredReadW;

BOOL WINAPI Hook_CredReadW(LPCWSTR TargetName, DWORD Type, DWORD Flags, PCREDENTIALW* Credential)
{
    BOOL success = FALSE;
    char* TargetNameA = NULL;

    MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, TargetName, -1, &TargetNameA, 0, NULL, NULL);

    if (TargetName && TargetNameA && !wcsncmp(TargetName, L"TERMSRV/", 8)) {
        WCHAR* CredTargetNameW = NULL;
        char* CredTargetNameA = MsRdpEx_GetEnv("MSRDPEX_CRED_TARGET_NAME");

        if (CredTargetNameA && !MsRdpEx_StringEquals(CredTargetNameA, TargetNameA)) {
            MsRdpEx_ConvertToUnicode(CP_UTF8, 0, CredTargetNameA, -1, &CredTargetNameW, 0);

            success = Real_CredReadW(CredTargetNameW, Type, Flags, Credential);

            MsRdpEx_LogPrint(DEBUG, "CredReadW(OldTargetName=\"%s\")", TargetNameA);
            MsRdpEx_LogPrint(DEBUG, "CredReadW(NewTargetName=\"%s\", Type=%d, Flags=0x%08X), success: %d", CredTargetNameA, Type, Flags, success);
        }

        free(CredTargetNameA);
        free(CredTargetNameW);
    }

    if (!success) {
        success = Real_CredReadW(TargetName, Type, Flags, Credential);

        MsRdpEx_LogPrint(DEBUG, "CredReadW(TargetName=\"%s\", Type=%d, Flags=0x%08X), success: %d", TargetNameA, Type, Flags, success);
    }

    free(TargetNameA);
    return success;
}

#include <dpapi.h>
#pragma comment(lib, "crypt32.lib")

BOOL(WINAPI* Real_CryptProtectMemory)(LPVOID pDataIn, DWORD cbDataIn, DWORD dwFlags) = CryptProtectMemory;
BOOL(WINAPI* Real_CryptUnprotectMemory)(LPVOID pDataIn, DWORD cbDataIn, DWORD dwFlags) = CryptUnprotectMemory;

BOOL WINAPI Hook_CryptProtectMemory(LPVOID pDataIn, DWORD cbDataIn, DWORD dwFlags)
{
    BOOL success;

    MsRdpEx_LogPrint(DEBUG, "CryptProtectMemory(cbDataIn=%d, dwFlags=%d)", cbDataIn, dwFlags);

    success = Real_CryptProtectMemory(pDataIn, cbDataIn, dwFlags);

    return success;
}

BOOL WINAPI Hook_CryptUnprotectMemory(LPVOID pDataIn, DWORD cbDataIn, DWORD dwFlags)
{
    BOOL success;

    MsRdpEx_LogPrint(DEBUG, "CryptUnprotectMemory(cbDataIn=%d, dwFlags=%d)", cbDataIn, dwFlags);

    success = Real_CryptUnprotectMemory(pDataIn, cbDataIn, dwFlags);

    return success;
}

BOOL(WINAPI* Real_CryptProtectData)(DATA_BLOB* pDataIn, LPCWSTR szDataDescr, DATA_BLOB* pOptionalEntropy,
    PVOID pvReserved, CRYPTPROTECT_PROMPTSTRUCT* pPromptStruct, DWORD dwFlags, DATA_BLOB* pDataOut) = CryptProtectData;

BOOL(WINAPI* Real_CryptUnprotectData)(DATA_BLOB* pDataIn, LPWSTR* ppszDataDescr, DATA_BLOB* pOptionalEntropy,
    PVOID pvReserved, CRYPTPROTECT_PROMPTSTRUCT* pPromptStruct, DWORD dwFlags, DATA_BLOB* pDataOut) = CryptUnprotectData;

BOOL WINAPI Hook_CryptProtectData(DATA_BLOB* pDataIn, LPCWSTR szDataDescr, DATA_BLOB* pOptionalEntropy,
    PVOID pvReserved, CRYPTPROTECT_PROMPTSTRUCT* pPromptStruct, DWORD dwFlags, DATA_BLOB* pDataOut)
{
    BOOL success;

    MsRdpEx_LogPrint(DEBUG, "CryptProtectData(pDataIn->cbData=%d, dwFlags=%d):", pDataIn->cbData, dwFlags);
    MsRdpEx_LogDump(TRACE, (uint8_t*)pDataIn->pbData, (size_t)pDataIn->cbData);

    success = Real_CryptProtectData(pDataIn, szDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataOut);

    MsRdpEx_LogPrint(DEBUG, "CryptProtectData(pDataOut->cbData=%d):", pDataOut->cbData);
    MsRdpEx_LogDump(TRACE, (uint8_t*)pDataOut->pbData, (size_t)pDataOut->cbData);

    return success;
}

BOOL WINAPI Hook_CryptUnprotectData(DATA_BLOB* pDataIn, LPWSTR* ppszDataDescr, DATA_BLOB* pOptionalEntropy,
    PVOID pvReserved, CRYPTPROTECT_PROMPTSTRUCT* pPromptStruct, DWORD dwFlags, DATA_BLOB* pDataOut)
{
    BOOL success;

    MsRdpEx_LogPrint(DEBUG, "CryptUnprotectData(pDataIn->cbData=%d, dwFlags=%d):", pDataIn->cbData, dwFlags);
    MsRdpEx_LogDump(TRACE, (uint8_t*)pDataIn->pbData, (size_t)pDataIn->cbData);

    success = Real_CryptUnprotectData(pDataIn, ppszDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataOut);

    MsRdpEx_LogPrint(DEBUG, "CryptUnprotectData(pDataOut->cbData=%d):", pDataOut->cbData);
    MsRdpEx_LogDump(TRACE, (uint8_t*)pDataOut->pbData, (size_t)pDataOut->cbData);

    return success;
}

typedef LSTATUS(WINAPI* Func_RegOpenKeyExW)(HKEY hKey, LPCWSTR lpSubKey, DWORD ulOptions, REGSAM samDesired, PHKEY phkResult);
typedef LSTATUS(WINAPI* Func_RegQueryValueExW)(HKEY hKey, LPCWSTR lpValueName, LPDWORD lpReserved, LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData);
typedef LSTATUS(WINAPI* Func_RegCloseKey)(HKEY hKey);

Func_RegOpenKeyExW Real_RegOpenKeyExW = NULL;
Func_RegQueryValueExW Real_RegQueryValueExW = NULL;
Func_RegCloseKey Real_RegCloseKey = NULL;

static HKEY g_hKeySecurityProviders = NULL;

LSTATUS WINAPI Hook_RegOpenKeyExW(HKEY hKey, LPCWSTR lpSubKey, DWORD ulOptions, REGSAM samDesired, PHKEY phkResult)
{
    LSTATUS lstatus;
    char* lpSubKeyA = NULL;

    if (lpSubKey)
    {
        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpSubKey, -1, &lpSubKeyA, 0, NULL, NULL);
        MsRdpEx_LogPrint(DEBUG, "RegOpenKeyExW(%s)", lpSubKeyA);
    }

    lstatus = Real_RegOpenKeyExW(hKey, lpSubKey, ulOptions, samDesired, phkResult);

    if (phkResult && MsRdpEx_StringEqualsW(lpSubKey, L"System\\CurrentControlSet\\Control\\SecurityProviders"))
    {
        g_hKeySecurityProviders = *phkResult;
    }

    free(lpSubKeyA);

    return lstatus;
}

LSTATUS WINAPI Hook_RegQueryValueExW(HKEY hKey, LPCWSTR lpValueName, LPDWORD lpReserved, LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData)
{
    LSTATUS lstatus;
    char* lpValueNameA = NULL;
    bool interceptedCall = false;

    if (lpValueName)
    {
        MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, lpValueName, -1, &lpValueNameA, 0, NULL, NULL);
        MsRdpEx_LogPrint(TRACE, "RegQueryValueExW(%s, lpType: %p, lpData: %p, lpcbData: %p (%d))",
            lpValueNameA, lpType, lpData, lpcbData, lpcbData ? *lpcbData : -1);
    }

    if ((hKey == g_hKeySecurityProviders) && MsRdpEx_StringEqualsW(lpValueName, L"SecurityProviders")
        && MsRdpEx_EnvExists("MSRDPEX_CREDSSP_DLL"))
    {
        WCHAR* credsspDllW = NULL;
        char* credsspDll = MsRdpEx_GetEnv("MSRDPEX_CREDSSP_DLL");

        // DLL needs to be named "credssp.dll" to work (no workaround yet)
        if (credsspDll && MsRdpEx_IStringEndsWith(credsspDll, "credssp.dll"))
        {
            MsRdpEx_ConvertToUnicode(CP_UTF8, 0, credsspDll, -1, &credsspDllW, 0);

            if (credsspDllW)
            {
                size_t cbData = (wcslen(credsspDllW) + 1) * 2;

                if (!lpData)
                {
                    if (lpType)
                        *lpType = REG_SZ;

                    if (lpcbData)
                        *lpcbData = cbData;

                    interceptedCall = true;
                    lstatus = ERROR_SUCCESS;
                }
                else if (lpcbData)
                {
                    if (lpType)
                        *lpType = REG_SZ;

                    if (*lpcbData >= cbData)
                    {
                        *lpcbData = cbData;
                        memcpy(lpData, (void*)credsspDllW, cbData);

                        interceptedCall = true;
                        lstatus = ERROR_SUCCESS;
                    }
                    else
                    {
                        interceptedCall = true;
                        lstatus = ERROR_MORE_DATA;
                    }
                }
            }
        }

        free(credsspDllW);
        free(credsspDll);
    }
    
    if (!interceptedCall)
    {
        lstatus = Real_RegQueryValueExW(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
    }

    free(lpValueNameA);

    return lstatus;
}

LSTATUS WINAPI Hook_RegCloseKey(HKEY hKey)
{
    LSTATUS lstatus;

    lstatus = Real_RegCloseKey(hKey);

    if (hKey == g_hKeySecurityProviders)
    {
        g_hKeySecurityProviders = NULL;
    }

    return lstatus;
}

typedef BOOL(WINAPI* Func_DeleteMenu)(HMENU hMenu, UINT uPosition, UINT uFlags);

static Func_DeleteMenu Real_DeleteMenu = DeleteMenu;

BOOL WINAPI Hook_DeleteMenu(HMENU hMenu, UINT uPosition, UINT uFlags)
{
    BOOL success = TRUE;
    BOOL skipDelete = FALSE;

    if (!(uFlags & MF_BYPOSITION) && MsRdpEx_IsAddressInRdpExeModule(_ReturnAddress()))
    {
        MENUITEMINFOA mii = { 0 };
        mii.cbSize = sizeof(MENUITEMINFOA);
        mii.fMask = MIIM_ID | MIIM_STRING;
        char itemName[256] = { 0 };
        mii.dwTypeData = itemName;
        mii.cch = sizeof(itemName);

        if (GetMenuItemInfoA(hMenu, uPosition, FALSE, &mii))
        {
            if ((mii.wID == 0) && (mii.cch == 0)) {
                MsRdpEx_LogPrint(DEBUG, "DeleteMenu hMenu: %p uPosition: 0x%04X uFlags 0x%04X", hMenu, uPosition, uFlags);
                MsRdpEx_LogPrint(DEBUG, "Skipping deletion of menu item ID: 0x%04X, name: %s, cch: %d", mii.wID, itemName, mii.cch);
                skipDelete = TRUE; // work around bug where separator menu items are deleted by CContainerWnd::SyncClipboardMenu()
            }
        }
    }

    if (!skipDelete) {
        success = Real_DeleteMenu(hMenu, uPosition, uFlags);
    }

    return success;
}

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

bool MsRdpEx_IsAddressInRdpAxModule(PVOID pAddress)
{
    return MsRdpEx_IsAddressInModule(pAddress, L"mstscax.dll") ||
        MsRdpEx_IsAddressInModule(pAddress, L"rdclientax.dll");
}

bool MsRdpEx_IsAddressInMstscAxModule(PVOID pAddress)
{
    return MsRdpEx_IsAddressInModule(pAddress, L"mstscax.dll");
}

bool MsRdpEx_IsAddressInRdclientAxModule(PVOID pAddress)
{
    return MsRdpEx_IsAddressInModule(pAddress, L"rdclientax.dll");
}

bool MsRdpEx_IsAddressInRdpExeModule(PVOID pAddress)
{
    return MsRdpEx_IsAddressInModule(pAddress, L"mstsc.exe") ||
        MsRdpEx_IsAddressInModule(pAddress, L"msrdc.exe");
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

static bool g_IsHooked = false;

static HMODULE g_hNtDll = NULL;
static HMODULE g_hKernelBase = NULL;

LONG MsRdpEx_AttachHooks()
{
    LONG error;

    if (g_IsHooked)
    {
        return NO_ERROR;
    }

    g_hNtDll = GetModuleHandleW(L"ntdll.dll");
    MSRDPEX_GETPROCADDRESS(Real_LdrResolveDelayLoadedAPI, Func_LdrResolveDelayLoadedAPI, g_hNtDll, "LdrResolveDelayLoadedAPI");
    MSRDPEX_GETPROCADDRESS(Real_NtCreateFile, Func_NtCreateFile, g_hNtDll, "NtCreateFile");
    MSRDPEX_GETPROCADDRESS(Real_NtOpenFile, Func_NtOpenFile, g_hNtDll, "NtOpenFile");

    g_hKernelBase = GetModuleHandleW(L"KernelBase.dll");
    MSRDPEX_GETPROCADDRESS(Real_RegOpenKeyExW, Func_RegOpenKeyExW, g_hKernelBase, "RegOpenKeyExW");
    MSRDPEX_GETPROCADDRESS(Real_RegQueryValueExW, Func_RegQueryValueExW, g_hKernelBase, "RegQueryValueExW");
    MSRDPEX_GETPROCADDRESS(Real_RegCloseKey, Func_RegCloseKey, g_hKernelBase, "RegCloseKey");
    MSRDPEX_GETPROCADDRESS(Real_LoadLibraryExA, Func_LoadLibraryExA, g_hKernelBase, "LoadLibraryExA");
    MSRDPEX_GETPROCADDRESS(Real_LoadLibraryExW, Func_LoadLibraryExW, g_hKernelBase, "LoadLibraryExW");

    MsRdpEx_GlobalInit();
    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryA, Hook_LoadLibraryA);
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryW, Hook_LoadLibraryW);
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryExA, Hook_LoadLibraryExA);
    MSRDPEX_DETOUR_ATTACH(Real_LoadLibraryExW, Hook_LoadLibraryExW);
    //MSRDPEX_DETOUR_ATTACH(Real_GetProcAddress, Hook_GetProcAddress);
    //MSRDPEX_DETOUR_ATTACH(Real_GetAddrInfoW, Hook_GetAddrInfoW);
    //MSRDPEX_DETOUR_ATTACH(Real_GetAddrInfoExW, Hook_GetAddrInfoExW);

    //MSRDPEX_DETOUR_ATTACH(Real_LdrResolveDelayLoadedAPI, Hook_LdrResolveDelayLoadedAPI);
    //MSRDPEX_DETOUR_ATTACH(Real_NtCreateFile, Hook_NtCreateFile);
    MSRDPEX_DETOUR_ATTACH(Real_NtOpenFile, Hook_NtOpenFile);

    MSRDPEX_DETOUR_ATTACH(Real_BitBlt, Hook_BitBlt);
    MSRDPEX_DETOUR_ATTACH(Real_StretchBlt, Hook_StretchBlt);
    MSRDPEX_DETOUR_ATTACH(Real_RegisterClassExW, Hook_RegisterClassExW);
    MSRDPEX_DETOUR_ATTACH(Real_RegisterClassW, Hook_RegisterClassW);
    MSRDPEX_DETOUR_ATTACH(Real_GetClassInfoW, Hook_GetClassInfoW);
    MSRDPEX_DETOUR_ATTACH(Real_UnregisterClassW, Hook_UnregisterClassW);
    MSRDPEX_DETOUR_ATTACH(Real_CreateWindowExW, Hook_CreateWindowExW);
    MSRDPEX_DETOUR_ATTACH(Real_CreateDialogParamW, Hook_CreateDialogParamW);
    
    MSRDPEX_DETOUR_ATTACH(Real_CredReadW, Hook_CredReadW);
    //MSRDPEX_DETOUR_ATTACH(Real_CryptProtectMemory, Hook_CryptProtectMemory);
    //MSRDPEX_DETOUR_ATTACH(Real_CryptUnprotectMemory, Hook_CryptUnprotectMemory);
    //MSRDPEX_DETOUR_ATTACH(Real_CryptProtectData, Hook_CryptProtectData);
    //MSRDPEX_DETOUR_ATTACH(Real_CryptUnprotectData, Hook_CryptUnprotectData);
    
    //MSRDPEX_DETOUR_ATTACH(Real_RegOpenKeyExW, Hook_RegOpenKeyExW);
    //MSRDPEX_DETOUR_ATTACH(Real_RegQueryValueExW, Hook_RegQueryValueExW);
    //MSRDPEX_DETOUR_ATTACH(Real_RegCloseKey, Hook_RegCloseKey);

    MSRDPEX_DETOUR_ATTACH(Real_DeleteMenu, Hook_DeleteMenu);
    
    MsRdpEx_AttachSspiHooks();
    error = DetourTransactionCommit();

    if (error == NO_ERROR)
    {
        g_IsHooked = true;
    }
    else
    {
        MsRdpEx_GlobalUninit();
    }

    return error;
}

LONG MsRdpEx_DetachHooks()
{
    LONG error;

    if (!g_IsHooked)
    {
        return NO_ERROR;
    }

    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryA, Hook_LoadLibraryA);
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryW, Hook_LoadLibraryW);
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryExA, Hook_LoadLibraryExA);
    MSRDPEX_DETOUR_DETACH(Real_LoadLibraryExW, Hook_LoadLibraryExW);
    //MSRDPEX_DETOUR_DETACH(Real_GetProcAddress, Hook_GetProcAddress);
    //MSRDPEX_DETOUR_DETACH(Real_GetAddrInfoW, Hook_GetAddrInfoW);
    //MSRDPEX_DETOUR_DETACH(Real_GetAddrInfoExW, Hook_GetAddrInfoExW);

    //MSRDPEX_DETOUR_DETACH(Real_LdrResolveDelayLoadedAPI, Hook_LdrResolveDelayLoadedAPI);
    //MSRDPEX_DETOUR_DETACH(Real_NtCreateFile, Hook_NtCreateFile);
    MSRDPEX_DETOUR_DETACH(Real_NtOpenFile, Hook_NtOpenFile);
    
    MSRDPEX_DETOUR_DETACH(Real_BitBlt, Hook_BitBlt);
    MSRDPEX_DETOUR_DETACH(Real_StretchBlt, Hook_StretchBlt);
    MSRDPEX_DETOUR_DETACH(Real_RegisterClassExW, Hook_RegisterClassExW);
    MSRDPEX_DETOUR_DETACH(Real_RegisterClassW, Hook_RegisterClassW);
    MSRDPEX_DETOUR_DETACH(Real_GetClassInfoW, Hook_GetClassInfoW);
    MSRDPEX_DETOUR_DETACH(Real_UnregisterClassW, Hook_UnregisterClassW);
    MSRDPEX_DETOUR_DETACH(Real_CreateWindowExW, Hook_CreateWindowExW);
    MSRDPEX_DETOUR_DETACH(Real_CreateDialogParamW, Hook_CreateDialogParamW);

    MSRDPEX_DETOUR_DETACH(Real_CredReadW, Hook_CredReadW);
    //MSRDPEX_DETOUR_DETACH(Real_CryptProtectMemory, Hook_CryptProtectMemory);
    //MSRDPEX_DETOUR_DETACH(Real_CryptUnprotectMemory, Hook_CryptUnprotectMemory);
    //MSRDPEX_DETOUR_DETACH(Real_CryptProtectData, Hook_CryptProtectData);
    //MSRDPEX_DETOUR_DETACH(Real_CryptUnprotectData, Hook_CryptUnprotectData);
    
    //MSRDPEX_DETOUR_DETACH(Real_RegOpenKeyExW, Hook_RegOpenKeyExW);
    //MSRDPEX_DETOUR_DETACH(Real_RegQueryValueExW, Hook_RegQueryValueExW);
    //MSRDPEX_DETOUR_DETACH(Real_RegCloseKey, Hook_RegCloseKey);

    MSRDPEX_DETOUR_DETACH(Real_DeleteMenu, Hook_DeleteMenu);
    
    MsRdpEx_DetachSspiHooks();
    error = DetourTransactionCommit();

    if (error == NO_ERROR)
    {
        g_IsHooked = false;
    }

    MsRdpEx_GlobalUninit();
    return error;
}
