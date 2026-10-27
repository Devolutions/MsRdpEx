#ifndef MSRDPEX_SMART_CARD_CONTEXT_H
#define MSRDPEX_SMART_CARD_CONTEXT_H

#include <windows.h>

enum class MsRdpEx_SmartCardContextMode { Off, Observe, Apply };
enum class MsRdpEx_SmartCardContextStatus { Ineligible, Ready, NoMatch, Ambiguous, Unsupported, Error };

struct MsRdpEx_SmartCardContext
{
    MsRdpEx_SmartCardContextMode mode = MsRdpEx_SmartCardContextMode::Off;
    MsRdpEx_SmartCardContextStatus status = MsRdpEx_SmartCardContextStatus::Ineligible;
    const char* stage = "not-prepared";
    DWORD error = ERROR_SUCCESS;
    BYTE certificateHash[20] = {};
    WCHAR card[512] = {};
    WCHAR reader[512] = {};
    WCHAR container[512] = {};
    WCHAR provider[512] = {};
    DWORD keySpec = 0;
};

MsRdpEx_SmartCardContextMode MsRdpEx_GetSmartCardContextMode();
bool MsRdpEx_GetSmartCardCertificateHash(const WCHAR* userName, BYTE (&hash)[20]);
void MsRdpEx_ResolveSmartCardContext(const WCHAR* userName, MsRdpEx_SmartCardContext* context);
void MsRdpEx_LogSmartCardContext(const MsRdpEx_SmartCardContext& context);

#endif
