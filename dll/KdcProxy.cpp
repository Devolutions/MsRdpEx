
#include <MsRdpEx/Environment.h>

#include "KdcProxy.h"

bool MsRdpEx_SetKdcProxyUrl(const char* targetName, const char* kdcProxyUrl)
{
	return MsRdpEx_SetEnv("MSRDPEX_KDC_PROXY_URL", kdcProxyUrl);
}

char* MsRdpEx_GetKdcProxyUrl(const char* targetName)
{
	return MsRdpEx_GetEnv("MSRDPEX_KDC_PROXY_URL");
}
