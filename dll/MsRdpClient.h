#ifndef MSRDPEX_CLIENT_H
#define MSRDPEX_CLIENT_H

#include "MsRdpEx.h"

#include <comdef.h>

#ifdef __cplusplus
extern "C" {
#endif

void* MsRdpEx_CClassFactory_New(REFCLSID rclsid, IClassFactory* pDelegate);

#ifdef __cplusplus
}

class CMsRdpClient;
HRESULT MsRdpEx_CMsRdpClient_ReconnectInGdiMode(CMsRdpClient* rdpClient);
#endif

#endif /* MSRDPEX_CLIENT_H */
