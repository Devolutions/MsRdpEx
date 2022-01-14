#ifndef MSRDPEX_SETTINGS_H
#define MSRDPEX_SETTINGS_H

#include <MsRdpEx/MsRdpEx.h>

#include <comdef.h>

class CMsRdpExtendedSettings;

#ifdef __cplusplus
extern "C" {
#endif

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_SETTINGS_H
