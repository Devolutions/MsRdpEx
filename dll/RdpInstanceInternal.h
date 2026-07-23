#ifndef MSRDPEX_RDP_INSTANCE_INTERNAL_H
#define MSRDPEX_RDP_INSTANCE_INTERNAL_H

#include <MsRdpEx/RdpInstance.h>

IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByOutputPresenterHwnd(HWND hWnd);
IMsRdpExInstance* MsRdpEx_InstanceManager_AcquireByInputCaptureHwnd(HWND hWnd);

#endif /* MSRDPEX_RDP_INSTANCE_INTERNAL_H */