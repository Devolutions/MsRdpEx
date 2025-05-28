
#include "MsRdpEx.h"

#include <MsRdpEx/NamedPipe.h>

#include <fcntl.h>
#include <errno.h>

#define XMF_NAMED_PIPE_BUFFER_SIZE    8192

int MsRdpEx_NamedPipe_Read(HANDLE np_handle, uint8_t* data, size_t size)
{
    DWORD cb_read = 0;
    
    if (!ReadFile(np_handle, data, size, &cb_read, NULL)) {
        return -1;
    }

    return (int) cb_read;
}

int MsRdpEx_NamedPipe_Write(HANDLE np_handle, const uint8_t* data, size_t size)
{
    DWORD cb_write = 0;

    if (!WriteFile(np_handle, (void*)data, (DWORD)size, &cb_write, NULL)) {
        return -1;
    }

    return (int) cb_write;
}

HANDLE MsRdpEx_NamedPipe_Open(const char* np_name)
{
    HANDLE np_handle;
    char filename[MSRDPEX_MAX_PATH];

    if (!np_name)
        return NULL;

    sprintf_s(filename, sizeof(filename) - 1, "\\\\.\\pipe\\%s", np_name);

    np_handle = CreateFileA(filename, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);

    if (np_handle == INVALID_HANDLE_VALUE) {
        return NULL;
    }

    return np_handle;
}

HANDLE MsRdpEx_NamedPipe_Create(const char* np_name, int max_clients)
{
    HANDLE np_handle;
    char filename[MSRDPEX_MAX_PATH];

    if (!np_name)
        return NULL;

    sprintf_s(filename, sizeof(filename) - 1, "\\\\.\\pipe\\%s", np_name);

    np_handle = CreateNamedPipeA(filename,
        PIPE_ACCESS_DUPLEX,
        PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT,
        max_clients,
        XMF_NAMED_PIPE_BUFFER_SIZE,
        XMF_NAMED_PIPE_BUFFER_SIZE,
        0, NULL);

    if (np_handle == INVALID_HANDLE_VALUE) {
        return NULL;
    }

    return np_handle;
}

HANDLE MsRdpEx_NamedPipe_Accept(HANDLE np_handle)
{
    if (!ConnectNamedPipe(np_handle, NULL)) {
        return NULL;
    }
    return np_handle;
}


void MsRdpEx_NamedPipe_Close(HANDLE np_handle)
{
    CloseHandle(np_handle);
}
