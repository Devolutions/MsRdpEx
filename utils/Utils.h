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

// Paths Utils

#define MSRDPEX_CURRENT_MODULE_PATH     0x00000001
#define MSRDPEX_CURRENT_LIBRARY_PATH    0x00000002
#define MSRDPEX_MSTSC_EXE_PATH          0x00000100
#define MSRDPEX_MSTSCAX_DLL_PATH        0x00000200
#define MSRDPEX_MSRDC_EXE_PATH          0x00000400
#define MSRDPEX_RDCLIENTAX_DLL_PATH     0x00000800
#define MSRDPEX_ALL_PATHS               0xFFFFFFFF

bool MsRdpEx_InitPaths(uint32_t pathIds);
const char* MsRdpEx_GetPath(uint32_t pathId);

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
