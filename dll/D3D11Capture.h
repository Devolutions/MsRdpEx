#ifndef MSRDPEX_D3D11_CAPTURE_H
#define MSRDPEX_D3D11_CAPTURE_H

#include "RdpInstanceInternal.h"

void MsRdpEx_D3D11Capture_RegisterPendingInstance(IMsRdpExInstance* instance);
void MsRdpEx_D3D11Capture_ReleaseInstance(IMsRdpExInstance* instance);
void MsRdpEx_D3D11Capture_AttachHooks();
void MsRdpEx_D3D11Capture_DetachHooks();
void MsRdpEx_D3D11Capture_Shutdown();

#endif /* MSRDPEX_D3D11_CAPTURE_H */
