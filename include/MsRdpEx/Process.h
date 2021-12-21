#ifndef MSRDPEX_PROCESS_LAUNCHER_H
#define MSRDPEX_PROCESS_LAUNCHER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_Process MsRdpEx_Process;

DWORD MsRdpEx_Process_Wait(MsRdpEx_Process* ctx, DWORD dwMilliseconds);

MsRdpEx_Process* MsRdpEx_Process_New();
void MsRdpEx_Process_Free(MsRdpEx_Process* ctx);

int MsRdpEx_LaunchProcessMain(int argc, char** argv, const char* appName);

char** MsRdpEx_GetArgumentVector(int* argc);
void MsRdpEx_FreeArgumentVector(int argc, char** argv);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PROCESS_LAUNCHER_H
