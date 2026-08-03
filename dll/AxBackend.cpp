#include "MsRdpEx.h"

#include <string.h>

bool CDECL MsRdpEx_IsIronRdpAxBackendName(const char* backend)
{
    return backend && (_stricmp(backend, "ironrdp") == 0);
}

bool CDECL MsRdpEx_UsePrivateAxLayout()
{
    return !MsRdpEx_IsIronRdpAxBackendName(getenv("MSRDPEX_AX_BACKEND"));
}
