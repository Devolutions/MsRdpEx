#ifndef MSRDPEX_KDC_PROXY_H
#define MSRDPEX_KDC_PROXY_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

bool MsRdpEx_SetKdcProxyUrl(const char* kdcProxyUrl);

char* MsRdpEx_GetKdcProxyUrl();
char* MsRdpEx_GetKdcProxyName();

char* MsRdpEx_KdcProxyUrlToName(const char* kdcProxyUrl);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_KDC_PROXY_H

