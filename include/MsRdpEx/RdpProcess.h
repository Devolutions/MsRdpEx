#ifndef MSRDPEX_PROCESS_LAUNCHER_H
#define MSRDPEX_PROCESS_LAUNCHER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

HRESULT MsRdpEx_LaunchProcess(int argc, char** argv, const char* appName, const char* axName);

char** MsRdpEx_GetArgumentVector(int* argc);
void MsRdpEx_FreeArgumentVector(int argc, char** argv);

HRESULT MsRdpExProcess_CreateInstance(LPVOID* ppvObject);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PROCESS_LAUNCHER_H
