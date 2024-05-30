#include <windows.h>
#include <wtsapi32.h>
#include <pchannel.h>
#include <crtdbg.h>
#include <stdio.h>

DWORD OpenVirtualChannel(const char* channelName, HANDLE* phFile)
{
    HANDLE hWTSHandle = NULL;
    HANDLE hWTSFileHandle;
    PVOID vcFileHandlePtr = NULL;
    DWORD len;
    DWORD rc = ERROR_SUCCESS;

    hWTSHandle = WTSVirtualChannelOpenEx(WTS_CURRENT_SESSION, (LPSTR)channelName, WTS_CHANNEL_OPTION_DYNAMIC);

    if (!hWTSHandle)
    {
        rc = GetLastError();
        printf("WTSVirtualChannelOpenEx API Call Failed: GetLastError() = %d\n", GetLastError());
        goto exitpt;
    }

    BOOL bSuccess = WTSVirtualChannelQuery(hWTSHandle, WTSVirtualFileHandle, &vcFileHandlePtr, &len);

    if (!bSuccess)
    {
        rc = GetLastError();
        goto exitpt;
    }
    
    if (len != sizeof(HANDLE))
    {
        rc = ERROR_INVALID_PARAMETER;
        goto exitpt;
    }

    hWTSFileHandle = *(HANDLE*)vcFileHandlePtr;

    bSuccess = DuplicateHandle(GetCurrentProcess(),
        hWTSFileHandle, GetCurrentProcess(), phFile, 0, FALSE, DUPLICATE_SAME_ACCESS);

    if (!bSuccess)
    {
        rc = GetLastError();
        goto exitpt;
    }

    rc = ERROR_SUCCESS;
exitpt:
    if (vcFileHandlePtr)
    {
        WTSFreeMemory(vcFileHandlePtr);
    }
    if (hWTSHandle)
    {
        WTSVirtualChannelClose(hWTSHandle);
    }
    return rc;
}

DWORD WriteVirtualChannelMessage(HANDLE hFile, ULONG cbSize, BYTE* pBuffer)
{
    BYTE WriteBuffer[1024];
    DWORD dwWritten;
    BOOL bSuccess;
    HANDLE hEvent;

    hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

    OVERLAPPED overlapped = { 0 };
    overlapped.hEvent = hEvent;

    bSuccess = WriteFile(hFile, pBuffer, cbSize, &dwWritten, &overlapped);

    if (!bSuccess)
    {
        if (GetLastError() == ERROR_IO_PENDING)
        {
            DWORD dwStatus = WaitForSingleObject(overlapped.hEvent, 10000);
            bSuccess = GetOverlappedResult(hFile, &overlapped, &dwWritten, FALSE);
        }
    }

    if (!bSuccess)
    {
        DWORD error = GetLastError();
        return error;
    }

    return 0;
}

DWORD HandleVirtualChannel(HANDLE hFile)
{
    BYTE ReadBuffer[CHANNEL_PDU_LENGTH];
    DWORD dwRead;
    BYTE b = 0;
    CHANNEL_PDU_HEADER* pHdr = (CHANNEL_PDU_HEADER*)ReadBuffer;
    BOOL bSuccess;
    HANDLE hEvent;

    const char* cmd = "whoami";
    ULONG cbSize = strlen(cmd) + 1;
    BYTE* pBuffer = (BYTE*)cmd;
    WriteVirtualChannelMessage(hFile, cbSize, pBuffer);

    hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

    do
    {
        OVERLAPPED overlapped = { 0 };
        DWORD TotalRead = 0;

        do {
            overlapped.hEvent = hEvent;
            bSuccess = ReadFile(hFile, ReadBuffer, sizeof(ReadBuffer), &dwRead, &overlapped);

            if (!bSuccess)
            {
                if (GetLastError() == ERROR_IO_PENDING)
                {
                    DWORD dwStatus = WaitForSingleObject(overlapped.hEvent, INFINITE);
                    bSuccess = GetOverlappedResult(hFile, &overlapped, &dwRead, FALSE);
                }
            }

            if (!bSuccess)
            {
                DWORD error = GetLastError();
                return error;
            }

            printf("read %d bytes\n", dwRead);

            ULONG packetSize = dwRead - sizeof(*pHdr);
            TotalRead += packetSize;
            PBYTE pData = (PBYTE)(pHdr + 1);

            printf(">> %s\n", (const char*)pData);

        } while (0 == (pHdr->flags & CHANNEL_FLAG_LAST));

    } while (true);

    return 0;
}

INT _cdecl wmain(INT argc, __in_ecount(argc) WCHAR** argv)
{
    DWORD rc;
    HANDLE hFile;
    const char* channelName = "DvcSample";

    printf("Opening %s dynamic virtual channel\n", channelName);
    rc = OpenVirtualChannel(channelName, &hFile);

    if (ERROR_SUCCESS != rc)
    {
        printf("Failed to open %s dynamic virtual channel\n", channelName);
        return 0;
    }
    else
    {
        printf("%s dynamic virtual channel is opened\n", channelName);
    }

    HandleVirtualChannel(hFile);

    CloseHandle(hFile);

    return 0;
}
