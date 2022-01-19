#ifndef MSRDPEX_ENVIRONMENT_H
#define MSRDPEX_ENVIRONMENT_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

bool MsRdpEx_SetEnv(const char* name, const char* value);
char* MsRdpEx_GetEnv(const char* name);

bool MsRdpEx_EnvExists(const char* name);

bool MsRdpEx_GetEnvBool(const char* name, bool defaultValue);
int MsRdpEx_GetEnvInt(const char* name, int defaultValue);

char** MsRdpEx_GetEnvironmentVariables(int* envc);
void MsRdpEx_FreeEnvironmentVariables(int envc, char** envs);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_ENVIRONMENT_H */
