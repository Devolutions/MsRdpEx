#ifndef MSRDPEX_PROCESS_LAUNCHER_H
#define MSRDPEX_PROCESS_LAUNCHER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

int MsRdpEx_LaunchProcessMain(int argc, char** argv, const char* appName);

char** MsRdpEx_GetArgumentVector(int* argc);
void MsRdpEx_FreeArgumentVector(int argc, char** argv);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PROCESS_LAUNCHER_H
