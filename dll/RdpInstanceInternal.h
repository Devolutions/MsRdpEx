#ifndef MSRDPEX_RDP_INSTANCE_INTERNAL_H
#define MSRDPEX_RDP_INSTANCE_INTERNAL_H

#include <MsRdpEx/RdpInstance.h>

IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByOutputPresenterHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByInputCaptureHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByScreenPoint(POINT point);

void MsRdpEx_RdpInstance_SetCursor(IMsRdpExInstance* instance, HCURSOR cursor);
void MsRdpEx_RdpInstance_UpdateCursorPosition(
    IMsRdpExInstance* instance, HWND sourceWindow, int32_t x, int32_t y);
void MsRdpEx_RdpInstance_HideCursor(IMsRdpExInstance* instance);
void MsRdpEx_RdpInstance_DumpFrameWithCursor(IMsRdpExInstance* instance);

#endif /* MSRDPEX_RDP_INSTANCE_INTERNAL_H */