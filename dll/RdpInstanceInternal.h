#ifndef MSRDPEX_RDP_INSTANCE_INTERNAL_H
#define MSRDPEX_RDP_INSTANCE_INTERNAL_H

#include <MsRdpEx/RdpInstance.h>

IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByOutputPresenterHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByInputCaptureHwnd(HWND hWnd);
// A non-null plugin returned on S_OK owns one reference for the caller to release.
HRESULT MsRdpEx_InstanceManager_AcquireWTSPluginBySessionId(const GUID* sessionId, IUnknown** plugin);
UINT MsRdpEx_Instance_GetGdiReconnectMessage();
UINT_PTR MsRdpEx_Instance_GetHardwareCaptureWatchdogTimerId();
bool MsRdpEx_Instance_RequestGdiReconnect(IMsRdpExInstance* instance);
void MsRdpEx_Instance_ReconnectUsingGdi(IMsRdpExInstance* instance);
bool MsRdpEx_Instance_ArmHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_DisarmHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_HandleHardwareCaptureWatchdog(IMsRdpExInstance* instance);
void MsRdpEx_Instance_NotifyOutputFrame(IMsRdpExInstance* instance);

#endif /* MSRDPEX_RDP_INSTANCE_INTERNAL_H */