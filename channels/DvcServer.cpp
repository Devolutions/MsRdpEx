#include <windows.h>
#include <wtsapi32.h>
#include <pchannel.h>
#include <crtdbg.h>
#include <stdio.h>

#include <stdint.h>

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

int HandleLogoffMessage()
{
    HANDLE hToken;
    TOKEN_PRIVILEGES tkp;

    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken))
        return 1;

    LookupPrivilegeValue(NULL, SE_SHUTDOWN_NAME, &tkp.Privileges[0].Luid);

    tkp.PrivilegeCount = 1;
    tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    AdjustTokenPrivileges(hToken, FALSE, &tkp, 0, (PTOKEN_PRIVILEGES)NULL, 0);

    if (GetLastError() != ERROR_SUCCESS)
        return 1;

    if (!ExitWindowsEx(EWX_LOGOFF, 0)) {
        return 1;
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

            ULONG packetSize = dwRead - sizeof(CHANNEL_PDU_HEADER);
            TotalRead += packetSize;
            BYTE* pData = (BYTE*)(pHdr + 1);

            printf("read %d packet bytes\n", packetSize);
            
            uint8_t msgFlags = pData[0];
            uint8_t msgType = pData[1];
            uint16_t msgSize = (pData[2] << 8) | pData[3];

            printf("msgType: %d, msgFlags: %d, msgSize: %d\n", msgType, msgFlags, msgSize);

            if (msgType == 13)
            {
                HandleLogoffMessage();
            }

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
