
#include "MreUtils.h"

const char* MreFile_Base(const char* filename)
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

bool MreFile_GetVersion(const char* filename, char* version)
{
    DWORD dwHandle;
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

    sprintf(version, "%d.%d.%d.%d",
        pvi->dwProductVersionMS >> 16,
        pvi->dwFileVersionMS & 0xFFFF,
        pvi->dwFileVersionLS >> 16,
        pvi->dwFileVersionLS & 0xFFFF);

    success = true;

exit:
    free(buffer);
    return success;
}
