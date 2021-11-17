
#include "Utils.h"

const char* MsRdpEx_FileBase(const char* filename)
{
    size_t length;
    char* separator;

    if (!filename)
        return NULL;

    separator = strrchr(filename, '\\');

    if (!separator)
        separator = strrchr(filename, '/');

    if (!separator)
        return filename;

    length = strlen(filename);

    if ((length - (separator - filename)) > 1)
        return separator + 1;

    return filename;
}

bool MsRdpEx_IsFile(const char* filename)
{
    FILE* fp = fopen(filename, "rb");

    if (!fp)
        return false;

    fclose(fp);
    
    return true;
}

bool MsRdpEx_GetFileBuildVersion(const char* filename, uint64_t* version)
{
    DWORD dwHandle = 0;
    char* buffer = NULL;
    bool success = false;
    VS_FIXEDFILEINFO* pvi;
    
    DWORD size = GetFileVersionInfoSizeA(filename, &dwHandle);

    if (size < 1)
        goto exit;

    buffer = (char*) malloc(size);

    if (!buffer)
        goto exit;

    if (!GetFileVersionInfoA(filename, dwHandle, size, buffer))
        goto exit;
    
    size = sizeof(VS_FIXEDFILEINFO);

    if (!VerQueryValueA(buffer, "\\", (LPVOID*) &pvi, (UINT*) &size))
        goto exit;

    *version = (uint64_t)(pvi->dwFileVersionLS >> 16);

    success = true;

exit:
    free(buffer);
    return success;
}
