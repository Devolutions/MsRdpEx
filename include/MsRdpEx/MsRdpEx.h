#ifndef MSRDPEX_CORE_H
#define MSRDPEX_CORE_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdbool.h>

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>

#ifdef __cplusplus
extern "C" {
#endif

#define MSRDPEX_MAX_PATH                    1024

// File Utils

#define MSRDPEX_PATH_SLASH_CHR              '/'
#define MSRDPEX_PATH_SLASH_STR              "/"

#define MSRDPEX_PATH_BACKSLASH_CHR          '\\'
#define MSRDPEX_PATH_BACKSLASH_STR          "\\"

#ifdef _WIN32
#define MSRDPEX_PATH_SEPARATOR_CHR          MSRDPEX_PATH_BACKSLASH_CHR
#define MSRDPEX_PATH_SEPARATOR_STR          MSRDPEX_PATH_BACKSLASH_STR
#else
#define MSRDPEX_PATH_SEPARATOR_CHR          MSRDPEX_PATH_SLASH_CHR
#define MSRDPEX_PATH_SEPARATOR_STR          MSRDPEX_PATH_SLASH_STR
#endif

const char* MsRdpEx_FileBase(const char* filename);
bool MsRdpEx_FileExists(const char* filename);

FILE* MsRdpEx_FileOpen(const char* path, const char* mode);
uint8_t* MsRdpEx_FileLoad(const char* filename, size_t* size, uint32_t zpad);
bool MsRdpEx_FileSave(const char* filename, uint8_t* data, size_t size, int mode);
int MsRdpEx_FileSeek(FILE* fp, uint64_t offset, int origin);
uint64_t MsRdpEx_FileTell(FILE* fp);
uint64_t MsRdpEx_FileSize(const char* filename);

bool MsRdpEx_GetFileBuildVersion(const char* filename, uint64_t* version);
bool MsRdpEx_MakePath(const char* path, LPSECURITY_ATTRIBUTES lpAttributes);

uint64_t MsRdpEx_GetUnixTime();

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

bool MsRdpEx_PathCchDetect(char* pszPath, size_t cchPath, uint32_t pathId);

bool MsRdpEx_PathCchRenameExtension(char* pszPath, size_t cchPath, const char* pszExt);

// String Utils

int MsRdpEx_ConvertFromUnicode(UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar,
                       LPSTR* lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar,
                       LPBOOL lpUsedDefaultChar);

int MsRdpEx_ConvertToUnicode(UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte,
                     LPWSTR* lpWideCharStr, int cchWideChar);

bool MsRdpEx_StringEquals(const char* str1, const char* str2);
bool MsRdpEx_StringIEquals(const char* str1, const char* str2);

bool MsRdpEx_StringEndsWith(const char* str, const char* val);
bool MsRdpEx_IStringEndsWith(const char* str, const char* val);

#define MSRDPEX_STRING_FLAG_UPPERCASE       0x00000001
#define MSRDPEX_STRING_FLAG_NO_TERMINATOR   0x00000002
#define MSRDPEX_GUID_STRING_SIZE            37 /* enough space for a GUID string + null terminator */

bool MsRdpEx_GuidGenerate(GUID* guid);
void MsRdpEx_GuidCopy(GUID* dst, const GUID* src);
int MsRdpEx_GuidCompare(const GUID* guid1, const GUID* guid2);
bool MsRdpEx_GuidIsEqual(const GUID* guid1, const GUID* guid2);
bool MsRdpEx_GuidIsNil(const GUID* guid);
bool MsRdpEx_GuidSetNil(GUID* guid);

bool MsRdpEx_GuidBinToStr(const GUID* guid, char* str, uint32_t flags);
bool MsRdpEx_GuidStrToBin(const char* str, GUID* guid, uint32_t flags);

uint8_t* MsRdpEx_HexToBin(const char* hex, uint8_t* bin, int size, uint32_t flags);
char* MsRdpEx_BinToHex(const uint8_t* bin, char* hex, int size, uint32_t flags);

// Log Utils

bool MsRdpEx_Log(const char* format, ...);
void MsRdpEx_LogHexDump(const uint8_t* data, size_t size);

void MsRdpEx_LogOpen();
void MsRdpEx_LogClose();
void MsRdpEx_SetLogEnabled(bool logEnabled);
void MsRdpEx_SetLogFilePath(const char* logFilePath);

// DLL Main

void MsRdpEx_Load();
void MsRdpEx_Unload();

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_CORE_H */
