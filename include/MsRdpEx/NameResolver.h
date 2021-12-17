#ifndef MSRDPEX_NAME_RESOLVER_H
#define MSRDPEX_NAME_RESOLVER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_NameResolver MsRdpEx_NameResolver;

bool MsRdpEx_NameResolver_GetMapping(const char* oldName, char** pNewName);

bool MsRdpEx_NameResolver_RemapName(const char* oldName, const char* newName);
bool MsRdpEx_NameResolver_UnmapName(const char* name);

MsRdpEx_NameResolver* MsRdpEx_NameResolver_Get();
void MsRdpEx_NameResolver_Release();

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_NAME_RESOLVER_H
