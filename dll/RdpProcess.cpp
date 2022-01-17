
#include <MsRdpEx/RdpProcess.h>

#include "MsRdpEx.h"
#include <MsRdpEx/RdpFile.h>

#include <detours.h>

struct _MsRdpEx_Process
{
    STARTUPINFOA startupInfo;
    PROCESS_INFORMATION processInfo;
    DWORD exitCode;
};

HRESULT MsRdpEx_LaunchProcess(int argc, char** argv, const char* appName)
{
    HRESULT hr = S_OK;
    BOOL fSuccess;
    uint32_t appPathId = 0;
    char szCommandLine[2048];
    MsRdpEx_Process* ctx;
    STARTUPINFOA* startupInfo;
    PROCESS_INFORMATION* processInfo;
    bool freeArgumentVector = false;

    if (argc == -1) {
        argv = MsRdpEx_GetArgumentVector(&argc);
        freeArgumentVector = true;
    }

    if ((argc < 0) || !argv)
        return E_UNEXPECTED;

    ctx = MsRdpEx_Process_New();

    if (!ctx)
        return E_UNEXPECTED;

    startupInfo = &ctx->startupInfo;
    processInfo = &ctx->processInfo;

    MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

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

    ZeroMemory(szCommandLine, sizeof(szCommandLine));

    for (int i = 1; i < argc; i++)
    {
        strncat(szCommandLine, argv[i], sizeof(szCommandLine) - 1);
        strncat(szCommandLine, " ", sizeof(szCommandLine) - 1);
    }

    char* lpCommandLine = szCommandLine;
    DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;
    const char* lpDllName = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);

    fSuccess = DetourCreateProcessWithDllExA(
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

    if (!fSuccess) {
        hr = E_FAIL;
        goto exit;
    }

    ResumeThread(processInfo->hThread);

    MsRdpEx_Process_Wait(ctx, INFINITE);

exit:
    if (freeArgumentVector) {
        MsRdpEx_FreeArgumentVector(argc, argv);
    }

    MsRdpEx_Process_Free(ctx);
    return hr;
}

DWORD MsRdpEx_Process_Wait(MsRdpEx_Process* ctx, DWORD dwMilliseconds)
{
    PROCESS_INFORMATION* processInfo = &ctx->processInfo;

    WaitForSingleObject(processInfo->hProcess, dwMilliseconds);
    GetExitCodeProcess(processInfo->hProcess, &ctx->exitCode);

    CloseHandle(processInfo->hProcess);
    CloseHandle(processInfo->hThread);

    return ctx->exitCode;
}

MsRdpEx_Process* MsRdpEx_Process_New()
{
    MsRdpEx_Process* ctx;

    ctx = (MsRdpEx_Process*) calloc(1, sizeof(MsRdpEx_Process));

    if (!ctx)
        return NULL;

    ctx->startupInfo.cb = sizeof(STARTUPINFOA);

    return ctx;
}

void MsRdpEx_Process_Free(MsRdpEx_Process* ctx)
{
    if (!ctx)
        return;
}

extern "C" const GUID IID_IMsRdpExProcess;

struct __declspec(novtable)
    IMsRdpExProcess : public IUnknown
{
public:
    virtual HRESULT __stdcall Start(const char* appName, int argc, char** argv) = 0;
    virtual HRESULT __stdcall Stop(uint32_t exitCode) = 0;
    virtual HRESULT __stdcall Wait(uint32_t milliseconds) = 0;
    virtual HRESULT __stdcall GetExitCode(uint32_t* pExitCode) = 0;
};

class CMsRdpExProcess : public IMsRdpExProcess
{
public:
    CMsRdpExProcess()
    {
        m_refCount = 0;
        m_exitCode = 0;
        ZeroMemory(&m_startupInfo, sizeof(STARTUPINFOA));
        ZeroMemory(&m_processInfo, sizeof(PROCESS_INFORMATION));
    }

    ~CMsRdpExProcess()
    {

    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr = E_NOINTERFACE;

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        else if (riid == IID_IMsRdpExProcess)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        if (--m_refCount == 0)
        {
            delete this;
            return 0;
        }

        return m_refCount;
    }

    // IMsRdpExProcess
public:
    HRESULT STDMETHODCALLTYPE Start(const char* appName, int argc, char** argv)
    {
        HRESULT hr = S_OK;
        uint32_t appPathId = 0;
        char szCommandLine[2048];
        STARTUPINFOA* startupInfo;
        PROCESS_INFORMATION* processInfo;

        startupInfo = &m_startupInfo;
        processInfo = &m_processInfo;

        MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

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

        ZeroMemory(szCommandLine, sizeof(szCommandLine));

        for (int i = 1; i < argc; i++)
        {
            strncat(szCommandLine, argv[i], sizeof(szCommandLine) - 1);
            strncat(szCommandLine, " ", sizeof(szCommandLine) - 1);
        }

        char* lpCommandLine = szCommandLine;
        DWORD dwCreationFlags = CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED;
        const char* lpDllName = MsRdpEx_GetPath(MSRDPEX_LIBRARY_PATH);

        BOOL fSuccess = DetourCreateProcessWithDllExA(
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

        if (!fSuccess) {
            hr = E_FAIL;
            goto exit;
        }

        ResumeThread(processInfo->hThread);

    exit:
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

    HRESULT STDMETHODCALLTYPE GetExitCode(uint32_t* pExitCode)
    {
        *pExitCode = m_exitCode;
        return S_OK;
    }

private:
    ULONG m_refCount;
    STARTUPINFOA m_startupInfo;
    PROCESS_INFORMATION m_processInfo;
    DWORD m_exitCode;
    bool m_hasExited;
};

HRESULT CDECL MsRdpExProcess_CreateInstance(LPVOID* ppvObject)
{
    CMsRdpExProcess* pObj = new CMsRdpExProcess();
    return pObj->QueryInterface(IID_IMsRdpExProcess, ppvObject);
}
