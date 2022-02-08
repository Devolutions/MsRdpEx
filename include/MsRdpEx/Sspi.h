#ifndef MSRDPEX_SSPI_H
#define MSRDPEX_SSPI_H

#include <MsRdpEx/MsRdpEx.h>

#define SECURITY_WIN32
#include <sspi.h>
#include <security.h>
#include <credssp.h>
#include <wincred.h>

#ifdef __cplusplus
extern "C" {
#endif

PSecurityFunctionTableW MsRdpEx_SspiHook_Init(PSecurityFunctionTableW pSecTable);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_SSPI_H
