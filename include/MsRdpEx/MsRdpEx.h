#ifndef MSRDPEX_CORE_H
#define MSRDPEX_CORE_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

#include <windows.h>

#ifdef __cplusplus
extern "C" {
#endif

#define MSRDPEX_MAX_PATH        1024

// File Utils

const char* MsRdpEx_FileBase(const char* filename);
bool MsRdpEx_IsFile(const char* filename);
bool MsRdpEx_GetFileBuildVersion(const char* filename, uint64_t* version);
bool MsRdpEx_MakePath(const char* path, LPSECURITY_ATTRIBUTES lpAttributes);

int64_t MsRdpEx_GetUnixTime();

// Paths Utils

#define MSRDPEX_CURRENT_MODULE_PATH     0x00000001
#define MSRDPEX_CURRENT_LIBRARY_PATH    0x00000002
#define MSRDPEX_EXECUTABLE_PATH         0x00000004
#define MSRDPEX_LIBRARY_PATH            0x00000008
#define MSRDPEX_APP_DATA_PATH           0x00000010
#define MSRDPEX_MSTSC_EXE_PATH          0x00000100
#define MSRDPEX_MSTSCAX_DLL_PATH        0x00000200
#define MSRDPEX_MSRDC_EXE_PATH          0x00000400
#define MSRDPEX_RDCLIENTAX_DLL_PATH     0x00000800
#define MSRDPEX_ALL_PATHS               0xFFFFFFFF

bool MsRdpEx_InitPaths(uint32_t pathIds);
const char* MsRdpEx_GetPath(uint32_t pathId);

bool MsRdpEx_PathCchRenameExtension(char* pszPath, size_t cchPath, const char* pszExt);

// String Utils

int MsRdpEx_ConvertFromUnicode(UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar,
                       LPSTR* lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar,
                       LPBOOL lpUsedDefaultChar);

int MsRdpEx_ConvertToUnicode(UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte,
                     LPWSTR* lpWideCharStr, int cchWideChar);

bool MsRdpEx_StringEquals(const char* str1, const char* str2);
bool MsRdpEx_StringIEquals(const char* str1, const char* str2);

// Log Utils

bool MsRdpEx_Log(const char* format, ...);

void MsRdpEx_LogOpen();
void MsRdpEx_LogClose();

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_CORE_H */
