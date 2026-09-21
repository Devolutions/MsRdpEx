#ifndef MSRDPEX_CORE_API_H
#define MSRDPEX_CORE_API_H

#include <MsRdpEx/MsRdpEx.h>

#include <comdef.h>

struct __declspec(uuid("13F6E86F-EE7D-44D1-AA94-1136B784441D")) __declspec(novtable)
    IMsRdpExCoreApi : public IUnknown
{
public:
    virtual HRESULT __stdcall Load(void) = 0;
    virtual HRESULT __stdcall Unload(void) = 0;
    virtual const char* __stdcall GetMsRdpExDllPath() = 0;
    virtual void __stdcall SetLogEnabled(bool enabled) = 0;
    virtual void __stdcall SetLogLevel(uint32_t logLevel) = 0;
    virtual void __stdcall SetLogFilePath(const char* logFilePath) = 0;
    virtual void __stdcall SetPcapEnabled(bool enabled) = 0;
    virtual void __stdcall SetPcapFilePath(const char* pcapFilePath) = 0;
    virtual void __stdcall SetAxHookEnabled(bool axHookEnabled) = 0;
    virtual bool __stdcall QueryInstanceByWindowHandle(HWND hWnd, LPVOID* ppvObject) = 0;
    virtual bool __stdcall OpenInstanceForWindowHandle(HWND hWnd, LPVOID* ppvObject) = 0;
};

// Optional extension. The original core API IID and vtable remain unchanged.
struct __declspec(uuid("FA68C6C8-38BF-4DEB-9CDA-7FD4F8D16009")) __declspec(novtable)
    IMsRdpExGatewaySettings : public IUnknown
{
public:
    virtual bool __stdcall GetGatewayIsolationEnabled() = 0;
    virtual void __stdcall SetGatewayIsolationEnabled(bool enabled) = 0;
};

#ifdef __cplusplus
extern "C" {
#endif



#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_CORE_API_H
