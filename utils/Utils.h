#ifndef MSRDPEX_UTILS_H
#define MSRDPEX_UTILS_H

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

// String Utils

int MsRdpEx_ConvertFromUnicode(UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar,
                       LPSTR* lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar,
                       LPBOOL lpUsedDefaultChar);

int MsRdpEx_ConvertToUnicode(UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte,
                     LPWSTR* lpWideCharStr, int cchWideChar);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_UTILS_H */
