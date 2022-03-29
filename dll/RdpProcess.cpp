
#include <MsRdpEx/RdpProcess.h>

#include "MsRdpEx.h"
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/Environment.h>

#include <MsRdpEx/Detours.h>

extern "C" const GUID IID_IMsRdpExProcess;

struct __declspec(novtable)
    IMsRdpExProcess : public IUnknown
{
public:
    virtual void __stdcall SetFileName(const char* filename) = 0;
    virtual void __stdcall SetArgumentBlock(const char* argumentBlock) = 0;
    virtual void __stdcall SetEnvironmentBlock(const char* environmentBlock) = 0;
    virtual void __stdcall SetWorkingDirectory(const char* workingDirectory) = 0;
    virtual HRESULT __stdcall Start(int argc, char** argv, const char* appName, const char* axName) = 0;
    virtual HRESULT __stdcall Stop(uint32_t exitCode) = 0;
    virtual HRESULT __stdcall Wait(uint32_t milliseconds) = 0;
    virtual uint32_t __stdcall GetProcessId() = 0;
    virtual uint32_t __stdcall GetExitCode() = 0;
};

class CMsRdpExProcess : public IMsRdpExProcess
{
public:
    CMsRdpExProcess()
    {
        m_refCount = 0;
        m_exitCode = 0;
        m_filename = NULL;
        m_argumentBlock = NULL;
        m_environmentBlock = NULL;
        m_workingDirectory = NULL;
        ZeroMemory(&m_startupInfo, sizeof(STARTUPINFOA));
        ZeroMemory(&m_processInfo, sizeof(PROCESS_INFORMATION));
    }

    ~CMsRdpExProcess()
    {
        free(m_filename);
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

    HRESULT STDMETHODCALLTYPE Start(int argc, char** argv, const char* appName, const char* axName)
    {
        HRESULT hr = S_OK;
        uint32_t appPathId = 0;
        char szCommandLine[2048];
        STARTUPINFOA* startupInfo;
        PROCESS_INFORMATION* processInfo;

        startupInfo = &m_startupInfo;
        processInfo = &m_processInfo;

        MsRdpEx_InitPaths(MSRDPEX_ALL_PATHS);

        if (axName) {
            SetEnvironmentVariableA("MSRDPEX_AXNAME", axName);

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
