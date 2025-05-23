
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#ifdef _WIN32
#include <shlwapi.h>
#include <shlobj.h>
#pragma comment(lib, "shlwapi.lib")
#endif

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

char* MsRdpEx_FileDir(const char* filename)
{
	size_t length;
	char* separator;

	if (!filename)
		return NULL;

	separator = strrchr(filename, '\\');

	if (!separator)
		separator = strrchr(filename, '/');

	if (!separator)
		return NULL;

	length = (separator - filename) + 1; // include the separator
	char* dirname = (char*)malloc(length + 1);
	if (!dirname)
		return NULL;

	memcpy(dirname, filename, length);
	dirname[length] = '\0';

	return dirname;
}

bool MsRdpEx_FileExists(const char* filename)
{
    bool result = false;
	WCHAR* filenameW = NULL;

    if (!filename)
        return false;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, filename, -1, &filenameW, 0) < 1)
		return false;

	result = PathFileExistsW(filenameW) ? true : false;
	free(filenameW);

	return result;
}

FILE* MsRdpEx_FileOpen(const char* path, const char* mode)
{
#ifndef _WIN32
	return fopen(path, mode);
#else
	LPWSTR lpPathW = NULL;
	LPWSTR lpModeW = NULL;
	FILE* result = NULL;

	if (!path || !mode)
		return NULL;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, path, -1, &lpPathW, 0) < 1)
		goto cleanup;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, mode, -1, &lpModeW, 0) < 1)
		goto cleanup;

	result = _wfopen(lpPathW, lpModeW);

cleanup:
	free(lpPathW);
	free(lpModeW);
	return result;
#endif
}

uint8_t* MsRdpEx_FileLoad(const char* filename, size_t* size, uint32_t zpad)
{
	FILE* fp = NULL;
	uint8_t* data = NULL;

	if (!filename || !size)
		return NULL;

	*size = 0;

	fp = MsRdpEx_FileOpen(filename, "rb");

	if (!fp)
		return NULL;

	MsRdpEx_FileSeek(fp, 0, SEEK_END);
	*size = MsRdpEx_FileTell(fp);
	MsRdpEx_FileSeek(fp, 0, SEEK_SET);

	data = malloc(*size + zpad);

	if (!data)
		goto exit;

	if (fread(data, 1, *size, fp) != *size)
	{
		free(data);
		data = NULL;
		*size = 0;
		goto exit;
	}

	if (zpad) {
		memset(&data[*size], 0, zpad);
		*size = *size + zpad;
	}

exit:
	fclose(fp);
	return data;
}

bool MsRdpEx_FileSave(const char* filename, uint8_t* data, size_t size, int mode)
{
	FILE* fp = NULL;
	bool success = true;

	if (!filename || !data)
		return false;

	fp = MsRdpEx_FileOpen(filename, "wb");

	if (!fp)
		return false;

	if (fwrite(data, 1, size, fp) != size)
	{
		success = false;
	}

	fclose(fp);
	return success;
}

int MsRdpEx_FileSeek(FILE* fp, uint64_t offset, int origin)
{
#ifdef _WIN32
	return (int) _fseeki64(fp, offset, origin);
#elif defined(__APPLE__)
	return (int) fseeko(fp, offset, origin);
#else
	return (int) fseeko(fp, offset, origin);
#endif
}

uint64_t MsRdpEx_FileTell(FILE* fp)
{
#ifdef _WIN32
	return (uint64_t) _ftelli64(fp);
#elif defined(__APPLE__)
	return (uint64_t) ftello(fp);
#else
	return (uint64_t) ftello(fp);
#endif
}

uint64_t MsRdpEx_FileSize(const char* filename)
{
	FILE* fp = NULL;
	uint64_t fileSize;

	fp = MsRdpEx_FileOpen(filename, "rb");

	if (!fp)
		return 0;

	MsRdpEx_FileSeek(fp, 0, SEEK_END);
	fileSize = MsRdpEx_FileTell(fp);
	fclose(fp);

	return fileSize;
}

bool MsRdpEx_GetFileBuildVersion(const char* filename, uint64_t* version)
{
	bool success = false;
    DWORD dwHandle = 0;
    WCHAR* buffer = NULL;
	WCHAR* filenameW = NULL;
    VS_FIXEDFILEINFO* pvi = NULL;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, filename, -1, &filenameW, 0) < 1)
	{
		goto exit;
	}
    
    DWORD size = GetFileVersionInfoSizeW(filenameW, &dwHandle);

    if (size < 1)
        goto exit;

    buffer = (WCHAR*) malloc(size);

    if (!buffer)
        goto exit;

    if (!GetFileVersionInfoW(filenameW, 0, size, buffer))
        goto exit;
    
    size = sizeof(VS_FIXEDFILEINFO);

    if (!VerQueryValueW(buffer, L"\\", (LPVOID*) &pvi, (UINT*) &size))
        goto exit;

    *version = (uint64_t)(pvi->dwFileVersionLS >> 16);

    success = true;

exit:
	free(filenameW);
    free(buffer);
    return success;
}

bool MsRdpEx_MakePath(const char* path, LPSECURITY_ATTRIBUTES lpAttributes)
{
	bool result = false;
	WCHAR* pathW = NULL;

	if (!path)
		return false;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, path, -1, &pathW, 0) < 1)
	{
		goto exit;
	}

    result = SHCreateDirectoryExW(NULL, pathW, lpAttributes) == ERROR_SUCCESS;

exit:
	free(pathW);
    return result;
}

BOOL MsRdpEx_MoveFile(LPCSTR lpExistingFileName, LPCSTR lpNewFileName, DWORD flags)
{
	BOOL result = FALSE;
	LPWSTR lpExistingFileNameW = NULL;
	LPWSTR lpNewFileNameW = NULL;

	if (!lpExistingFileName)
		goto cleanup;

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpExistingFileName, -1, &lpExistingFileNameW, 0) < 1)
		goto cleanup;

	if (lpNewFileName)
	{
		if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpNewFileName, -1, &lpNewFileNameW, 0) < 1)
			goto cleanup;
	}

	result = MoveFileExW(lpExistingFileNameW, lpNewFileNameW, flags);

cleanup:
	free(lpExistingFileNameW);
	free(lpNewFileNameW);
	return result;
}

uint64_t MsRdpEx_GetUnixTime()
{
    uint64_t unixTime;
    // number of seconds since Epoch (1970-01-01 00:00:00 +0000 UTC)
    unixTime = (uint64_t)time(NULL);
    return unixTime;
}
