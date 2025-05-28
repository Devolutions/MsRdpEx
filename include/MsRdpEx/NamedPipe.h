#ifndef MSRDPEX_NAMED_PIPE_H
#define MSRDPEX_NAMED_PIPE_H

#include "MsRdpEx.h"

#ifdef __cplusplus
extern "C" {
#endif

int MsRdpEx_NamedPipe_Read(HANDLE np_handle, uint8_t* data, size_t size);
int MsRdpEx_NamedPipe_Write(HANDLE np_handle, const uint8_t* data, size_t size);

HANDLE MsRdpEx_NamedPipe_Open(const char* np_name);
HANDLE MsRdpEx_NamedPipe_Create(const char* np_name, int max_clients);
HANDLE MsRdpEx_NamedPipe_Accept(HANDLE np_handle);

void MsRdpEx_NamedPipe_Close(HANDLE np_handle);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_NAMED_PIPE_H */