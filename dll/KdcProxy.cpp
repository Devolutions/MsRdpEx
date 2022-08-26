
#include <MsRdpEx/Environment.h>

#include "KdcProxy.h"

bool MsRdpEx_SetKdcProxyUrl(const char* kdcProxyUrl)
{
	return MsRdpEx_SetEnv("MSRDPEX_KDC_PROXY_URL", kdcProxyUrl);
}

char* MsRdpEx_GetKdcProxyUrl()
{
	return MsRdpEx_GetEnv("MSRDPEX_KDC_PROXY_URL");
}

char* MsRdpEx_GetKdcProxyName()
{
	char* kdcProxyName = NULL;
	char* kdcProxyUrl = MsRdpEx_GetKdcProxyUrl();

	if (!kdcProxyUrl)
		return NULL;

	kdcProxyName = MsRdpEx_KdcProxyUrlToName(kdcProxyUrl);
	free(kdcProxyUrl);

	return kdcProxyName;
}

char* MsRdpEx_KdcProxyUrlToName(const char* kdcProxyUrl)
{
	char* path = NULL;
	const char* host = NULL;
	char* kdcProxyName = NULL;

	// https://<host>[:<port>][/path]
	// <host>:[:<port>][:<path>]

	host = strstr(kdcProxyUrl, "://");

	if (!host)
		return NULL;

	host = &host[3];

	kdcProxyName = _strdup(host);

	if (!kdcProxyName)
		return NULL;

	path = (char*)strchr(kdcProxyName, '/');

	if (path)
		*path = ':';

	return kdcProxyName;
}
