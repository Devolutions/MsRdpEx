#include "../MsRdpEx.h"

#include <stdio.h>

static int Require(bool condition, const char* message)
{
    if (condition)
        return 0;

    fprintf(stderr, "%s\n", message);
    return 1;
}

int main()
{
    int failures = 0;

    failures += Require(!MsRdpEx_IsIronRdpAxBackendName(NULL), "NULL backend must use private layouts");
    failures += Require(!MsRdpEx_IsIronRdpAxBackendName("mstsc"), "unknown backend must use private layouts");
    failures += Require(MsRdpEx_IsIronRdpAxBackendName("ironrdp"), "ironrdp backend must be recognized");
    failures += Require(MsRdpEx_IsIronRdpAxBackendName("IrOnRdP"), "backend detection must be case-insensitive");

    _putenv_s("MSRDPEX_AX_BACKEND", "");
    failures += Require(MsRdpEx_UsePrivateAxLayout(), "unset backend must use private layouts");

    _putenv_s("MSRDPEX_AX_BACKEND", "unsupported");
    failures += Require(MsRdpEx_UsePrivateAxLayout(), "unknown backend must use private layouts");

    _putenv_s("MSRDPEX_AX_BACKEND", "IRONRDP");
    failures += Require(!MsRdpEx_UsePrivateAxLayout(), "IronRDP backend must disable private layouts");

    _putenv_s("MSRDPEX_AX_BACKEND", "");
    return failures ? 1 : 0;
}
