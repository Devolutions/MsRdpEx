#ifndef MSRDPEX_PROCESS_LAUNCHER_H
#define MSRDPEX_PROCESS_LAUNCHER_H

#include <MsRdpEx/MsRdpEx.h>

#include <oleauto.h>

struct __declspec(novtable)
    IMsRdpExProcess : public IUnknown
{
public:
    virtual void __stdcall SetFileName(const char* filename) = 0;
    virtual void __stdcall SetArguments(const char* arguments) = 0;
    virtual void __stdcall SetArgumentBlock(const char* argumentBlock) = 0;
    virtual void __stdcall SetEnvironmentBlock(const char* environmentBlock) = 0;
    virtual void __stdcall SetWorkingDirectory(const char* workingDirectory) = 0;
    virtual HRESULT __stdcall StartWithInfo() = 0;
    virtual HRESULT __stdcall Start(int argc, char** argv, const char* appName, const char* axName) = 0;
    virtual HRESULT __stdcall Stop(uint32_t exitCode) = 0;
    virtual HRESULT __stdcall Wait(uint32_t milliseconds) = 0;
    virtual uint32_t __stdcall GetProcessId() = 0;
    virtual uint32_t __stdcall GetExitCode() = 0;
};

#ifdef __cplusplus
extern "C" {
#endif

HRESULT MsRdpEx_LaunchProcess(int argc, char** argv, const char* appName, const char* axName);

char** MsRdpEx_GetArgumentVector(int* argc);
void MsRdpEx_FreeArgumentVector(int argc, char** argv);

HRESULT MsRdpExProcess_CreateInstance(LPVOID* ppvObject);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PROCESS_LAUNCHER_H
