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

LONG MsRdpEx_AttachSspiHooks();
LONG MsRdpEx_DetachSspiHooks();
void MsRdpEx_Sspi_BeginSession(GUID* sessionId);
void MsRdpEx_Sspi_EndSession(GUID* sessionId);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_SSPI_H
