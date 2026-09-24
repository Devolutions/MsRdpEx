#ifndef MSRDPEX_RDP_INSTANCE_INTERNAL_H
#define MSRDPEX_RDP_INSTANCE_INTERNAL_H

#include <MsRdpEx/RdpInstance.h>

class MsRdpEx_WTSPluginReference
{
public:
    explicit MsRdpEx_WTSPluginReference(IUnknown* plugin);
    IUnknown* Get() const;
    void AddRef();
    void Release();

private:
    volatile LONG m_refCount = 1;
    IUnknown* m_plugin;
};

IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByOutputPresenterHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByInputCaptureHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireBySessionId(const GUID* sessionId);
// The returned holder owns one internal reference; call Release when done.
HRESULT MsRdpEx_InstanceManager_AcquireWTSPluginBySessionId(
    const GUID* sessionId, MsRdpEx_WTSPluginReference** plugin);
UINT MsRdpEx_Instance_GetGdiReconnectMessage();
UINT_PTR MsRdpEx_Instance_GetHardwareCaptureWatchdogTimerId();
bool MsRdpEx_Instance_RequestGdiReconnect(IMsRdpExInstance* instance);
void MsRdpEx_Instance_ReconnectUsingGdi(IMsRdpExInstance* instance);
bool MsRdpEx_Instance_ArmHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_DisarmHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_HandleHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_NotifyOutputFrame(IMsRdpExInstance* instance);

#endif /* MSRDPEX_RDP_INSTANCE_INTERNAL_H */