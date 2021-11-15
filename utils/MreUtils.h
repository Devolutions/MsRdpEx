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

// File Utils

const char* MreFile_Base(const char* filename);
bool MreFile_GetModuleVersion(const char* filename, char* version);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_UTILS_H */
