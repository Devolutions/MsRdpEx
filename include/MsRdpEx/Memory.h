#ifndef MSRDPEX_MEMORY_H
#define MSRDPEX_MEMORY_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

bool MsRdpEx_CanReadUnsafePtr(void* ptr, size_t size);
bool MsRdpEx_StringEqualsUnsafePtr(const char* ptr, const char* str);
bool MsRdpEx_StringIEqualsUnsafePtr(const char* ptr, const char* str);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_MEMORY_H
