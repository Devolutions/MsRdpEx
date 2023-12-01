
#include <MsRdpEx/RdpProcess.h>

#include "MsRdpEx.h"
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/Environment.h>

#include <MsRdpEx/Detours.h>

extern "C" const GUID IID_IMsRdpExProcess;

BOOL MsRdpEx_DetourCreateProcessWithDllEx(
    LPCSTR lpApplicationName, LPSTR lpCommandLine,
    LPSECURITY_ATTRIBUTES lpProcessAttributes, LPSECURITY_ATTRIBUTES lpThreadAttributes,
    BOOL bInheritHandles, DWORD dwCreationFlags, LPVOID lpEnvironment, LPCSTR lpCurrentDirectory,
    LPSTARTUPINFOA lpStartupInfo, LPPROCESS_INFORMATION lpProcessInfo, LPCSTR lpDllName,
    PDETOUR_CREATE_PROCESS_ROUTINEA pfCreateProcessA)
{
    BOOL fSuccess = FALSE;
    WCHAR* lpApplicationNameW = NULL;
    WCHAR* lpCommandLineW = NULL;
    WCHAR* lpCurrentDirectoryW = NULL;
    LPVOID lpEnvironmentW = NULL;
    STARTUPINFOW startupInfoW = { 0 };

    if (lpApplicationName && (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpApplicationName, -1, &lpApplicationNameW, 0) < 1)) {
        goto exit;
    }

    if (lpCommandLine && (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpCommandLine, -1, &lpCommandLineW, 0) < 1)) {
        goto exit;
    }

    if (lpCurrentDirectory && (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpCurrentDirectory, -1, &lpCurrentDirectoryW, 0) < 1)) {
        goto exit;
    }

    if (lpEnvironment) {
        lpEnvironmentW = MsRdpEx_ConvertStringBlockToUnicode((const char*)lpEnvironment);

        if (!lpEnvironmentW) {
            goto exit;
        }
    }

#if 0
    fSuccess = DetourCreateProcessWithDllExA(
        lpApplicationName, lpCommandLine,
        NULL, NULL,
        bInheritHandles, dwCreationFlags,
        lpEnvironment, lpCurrentDirectory,
        lpStartupInfo, lpProcessInfo,
        lpDllName,
        NULL);
#else
    dwCreationFlags |= CREATE_UNICODE_ENVIRONMENT;

    // lpDllName is ANSI and doesn't support Unicode
    // https://github.com/microsoft/Detours/issues/283

    fSuccess = DetourCreateProcessWithDllExW(
        lpApplicationNameW, lpCommandLineW,
        NULL, NULL,
        bInheritHandles, dwCreationFlags,
        lpEnvironmentW, lpCurrentDirectoryW,
        &startupInfoW, lpProcessInfo,
        lpDllName,
        NULL);

    if (lpStartupInfo) {
        // only lpReserved, lpDesktop, lpTitle struct fields are LPWSTR
        CopyMemory(&startupInfoW, lpStartupInfo, sizeof(LPSTARTUPINFOA));
    }
#endif

exit:
    free(lpApplicationNameW);
    free(lpCommandLineW);
    free(lpCurrentDirectoryW);
    MsRdpEx_FreeStringBlock((const char*)lpEnvironmentW);
    return fSuccess;
}

class CMsRdpExProcess : public IMsRdpExProcess
{
public:
    CMsRdpExProcess()
    {
        m_refCount = 0;
        m_exitCode = 0;
        m_hasExited = false;
        m_filename = NULL;
        m_arguments = NULL;
        m_argumentBlock = NULL;
        m_environmentBlock = NULL;
        m_workingDirectory = NULL;
        ZeroMemory(&m_startupInfo, sizeof(STARTUPINFOA));
        ZeroMemory(&m_processInfo, sizeof(PROCESS_INFORMATION));
    }

    ~CMsRdpExProcess()
    {
        free(m_filename);
        free(m_arguments);
        MsRdpEx_FreeStringBlock(m_argumentBlock);
        MsRdpEx_FreeStringBlock(m_environmentBlock);
        free(m_workingDirectory);
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr = E_NOINTERFACE;
        ULONG refCount = m_refCount;
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if (riid == IID_IMsRdpExProcess)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExProcess::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExProcess::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExProcess::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IMsRdpExProcess
public:

    void STDMETHODCALLTYPE SetFileName(const char* filename)
    {
        if (m_filename) {
            free(m_filename);
            m_filename = NULL;
        }

        if (filename) {
            m_filename = _strdup(filename);
        }
    }

    void STDMETHODCALLTYPE SetArguments(const char* arguments)
    {
        if (m_arguments) {
            free(m_arguments);
            m_arguments = NULL;
        }

        if (arguments) {
            m_arguments = _strdup(arguments);
        }
    }

    void STDMETHODCALLTYPE SetArgumentBlock(const char* argumentBlock)
    {
        if (m_argumentBlock) {
            MsRdpEx_FreeStringBlock(argumentBlock);
            argumentBlock = NULL;
        }

        if (argumentBlock) {
            m_argumentBlock = MsRdpEx_CloneStringBlock(argumentBlock);
        }
    }

    void STDMETHODCALLTYPE SetEnvironmentBlock(const char* environmentBlock)
    {
        if (m_environmentBlock) {
            MsRdpEx_FreeStringBlock(environmentBlock);
            environmentBlock = NULL;
        }

        if (environmentBlock) {
            m_environmentBlock = MsRdpEx_CloneStringBlock(environmentBlock);
        }
    }

    void STDMETHODCALLTYPE SetWorkingDirectory(const char* workingDirectory)
    {
        if (m_workingDirectory) {
            free(m_workingDirectory);
            m_workingDirectory = NULL;
        }

        if (workingDirectory) {
            m_workingDirectory = _strdup(workingDirectory);
        }
    }

    HRESULT STDMETHODCALLTYPE StartWithInfo()
    {
        int argc = 0;
        char** argv = NULL;
        HRESULT hr = S_OK;
        STARTUPINFOA* startupInfo;
        PROCESS_INFORMATION* processInfo;
        const char* lpApplicationName = NULL;
        char* lpCommandLine = NULL;
        char* lpEnvironment = NULL;
        char* lpCurrentDirectory = NULL;

        startupInfo = &m_startupInfo;
        processInfo = &m_processInfo;

        MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

        if (m_arguments) {
            lpCommandLine = _strdup(m_arguments);
        }
        else {
            argv = MsRdpEx_GetStringVectorFromBlock(&argc, m_argumentBlock);
            lpCommandLine = MsRdpEx_StringJoin(argv, argc, ' ');
            MsRdpEx_FreeStringVector(argc, argv);
        }

        lpApplicationName = m_filename;
        lpEnvironment = m_environmentBlock;
        lpCurrentDirectory = m_workingDirectory;

        DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;
        const char* lpDllName = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);

        MsRdpEx_LogPrint(DEBUG, "IMsRdpExProcess::Start(FileName=\"%s\", "
            "Arguments=\"%s\", WorkingDirectory=\"%s\", DllName=\"%s\")",
            lpApplicationName, lpCommandLine, lpCurrentDirectory, lpDllName);

        BOOL fSuccess = MsRdpEx_DetourCreateProcessWithDllEx(
            lpApplicationName, /* lpApplicationName */
            lpCommandLine, /* lpCommandLine */
            NULL, /* lpProcessAttributes */
            NULL, /* lpThreadAttributes */
            FALSE, /* bInheritHandles */
            dwCreationFlags, /* dwCreationFlags */
            lpEnvironment, /* lpEnvironment */
            lpCurrentDirectory, /* lpCurrentDirectory */
            startupInfo, /* lpStartupInfo */
            processInfo, /* lpProcessInformation */
            lpDllName, /* lpDllName */
            NULL /* pfCreateProcessW */
        );

        free(lpCommandLine);

        if (!fSuccess) {
            DWORD lastError = GetLastError();
            MsRdpEx_LogPrint(DEBUG, "IMsRdpExProcess::Start() failure: 0x%08X", lastError);
            hr = E_FAIL;
            goto exit;
        }

        ResumeThread(processInfo->hThread);

    exit:
        return hr;
    }

    HRESULT STDMETHODCALLTYPE Start(int argc, char** argv, const char* appName, const char* axName)
    {
        HRESULT hr = S_OK;
        uint32_t appPathId = 0;
        char* lpCommandLine = NULL;
        STARTUPINFOA* startupInfo;
        PROCESS_INFORMATION* processInfo;
        const char* axNameEnv = NULL;

        startupInfo = &m_startupInfo;
        processInfo = &m_processInfo;

        MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

        axNameEnv = MsRdpEx_GetEnv("MSRDPEX_AXNAME");

        if (!axName && axNameEnv) {
            axName = axNameEnv;
        }

        if (axName) {
            MsRdpEx_SetEnv("MSRDPEX_AXNAME", axName);

            if (!appName)
                appName = axName;
        }

        if (!appName) {
            appPathId = MSRDPEX_MSTSC_EXE_PATH;
        }

        if (appName) {
            if (MsRdpEx_StringIEquals(appName, "mstsc") || MsRdpEx_StringIEquals(appName, "mstsc.exe")) {
                appPathId = MSRDPEX_MSTSC_EXE_PATH;
            }
            else if (MsRdpEx_StringIEquals(appName, "msrdc") || MsRdpEx_StringIEquals(appName, "msrdc.exe")) {
                appPathId = MSRDPEX_MSRDC_EXE_PATH;
            }
        }

        const char* lpApplicationName = appName;

        if (appPathId) {
            lpApplicationName = MsRdpEx_GetPath(appPathId);
        }

        char quotedAppFilePath[MSRDPEX_MAX_PATH];
        char quotedRdpFilePath[MSRDPEX_MAX_PATH];

        sprintf_s(quotedAppFilePath, MSRDPEX_MAX_PATH - 1, "\"%s\"", lpApplicationName);

        if ((argc >= 2) && (MsRdpEx_IStringEndsWith(argv[1], ".rdp") || MsRdpEx_IStringEndsWith(argv[1], ".rdpw")))
        {
            sprintf_s(quotedRdpFilePath, MSRDPEX_MAX_PATH - 1, "\"%s\"", argv[1]);

            char* argv0 = argv[0];
            char* argv1 = argv[1];
            argv[0] = (char*)quotedAppFilePath;
            argv[1] = (char*)quotedRdpFilePath;
            lpCommandLine = MsRdpEx_StringJoin(argv, argc, ' ');
            argv[0] = argv0;
            argv[1] = argv1;
        }
        else
        {
            char* argv0 = argv[0];
            argv[0] = (char*) quotedAppFilePath;
            lpCommandLine = MsRdpEx_StringJoin(argv, argc, ' ');
            argv[0] = argv0;
        }

        DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;
        const char* lpDllName = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);

        BOOL fSuccess = MsRdpEx_DetourCreateProcessWithDllEx(
            lpApplicationName, /* lpApplicationName */
            lpCommandLine, /* lpCommandLine */
            NULL, /* lpProcessAttributes */
            NULL, /* lpThreadAttributes */
            FALSE, /* bInheritHandles */
            dwCreationFlags, /* dwCreationFlags */
            NULL, /* lpEnvironment */
            NULL, /* lpCurrentDirectory */
            startupInfo, /* lpStartupInfo */
            processInfo, /* lpProcessInformation */
            lpDllName, /* lpDllName */
            NULL /* pfCreateProcessW */
        );

        free(lpCommandLine);

        if (!fSuccess) {
            hr = E_FAIL;
            goto exit;
        }

        ResumeThread(processInfo->hThread);

    exit:
        free((void*)axNameEnv);
        return hr;
    }

    HRESULT STDMETHODCALLTYPE Stop(uint32_t exitCode)
    {
        BOOL fSuccess;

        if (!m_processInfo.hProcess) {
            return E_HANDLE;
        }

        fSuccess = TerminateProcess(m_processInfo.hProcess, exitCode);

        if (fSuccess) {
            CloseHandle(m_processInfo.hProcess);
            m_processInfo.hProcess = NULL;
            m_hasExited = true;
        }

        m_exitCode = exitCode;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE Wait(uint32_t milliseconds)
    {
        HRESULT hr = S_OK;

        if (!m_processInfo.hProcess) {
            return E_HANDLE;
        }

        DWORD waitStatus = WaitForSingleObject(m_processInfo.hProcess, milliseconds);

        if (waitStatus == WAIT_OBJECT_0) {
            GetExitCodeProcess(m_processInfo.hProcess, &m_exitCode);
            CloseHandle(m_processInfo.hProcess);
            m_processInfo.hProcess = NULL;
            m_hasExited = true;
        } else if (waitStatus == WAIT_TIMEOUT) {
            hr = E_PENDING;
        } else {
            hr = E_FAIL;
        }

        return hr;
    }

    uint32_t STDMETHODCALLTYPE GetProcessId()
    {
        return ::GetProcessId(m_processInfo.hProcess);
    }

    uint32_t STDMETHODCALLTYPE GetExitCode()
    {
        return m_exitCode;
    }

private:
    ULONG m_refCount;
    char* m_filename;
    char* m_arguments;
    char* m_argumentBlock;
    char* m_environmentBlock;
    char* m_workingDirectory;
    STARTUPINFOA m_startupInfo;
    PROCESS_INFORMATION m_processInfo;
    DWORD m_exitCode;
    bool m_hasExited;
};

HRESULT MsRdpEx_LaunchProcess(int argc, char** argv, const char* appName, const char* axName)
{
    HRESULT hr = S_OK;
    IMsRdpExProcess* rdpProcess = NULL;
    bool freeArgumentVector = false;

    if (argc == -1) {
        argv = MsRdpEx_GetArgumentVector(&argc);
        freeArgumentVector = true;
    }

    if ((argc < 0) || !argv)
        return E_UNEXPECTED;

    hr = MsRdpExProcess_CreateInstance((LPVOID*)&rdpProcess);

    if (hr != S_OK)
        goto exit;

    rdpProcess->AddRef();

    hr = rdpProcess->Start(argc, argv, appName, axName);

    if (hr != S_OK)
        goto exit;

    rdpProcess->Wait(INFINITE);

exit:
    if (rdpProcess) {
        rdpProcess->Release();
    }

    if (freeArgumentVector) {
        MsRdpEx_FreeArgumentVector(argc, argv);
    }

    return hr;
}

HRESULT CDECL MsRdpExProcess_CreateInstance(LPVOID* ppvObject)
{
    CMsRdpExProcess* pObj = new CMsRdpExProcess();
    *ppvObject = (LPVOID) pObj;
    return S_OK;
}
