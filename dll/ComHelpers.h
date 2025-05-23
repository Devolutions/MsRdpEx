#ifndef MSRDPEX_COM_HELPERS_H
#define MSRDPEX_COM_HELPERS_H

#include <MsRdpEx/MsRdpEx.h>

#include <atlbase.h>
#include <oleidl.h>
#include <commctrl.h>

#ifndef SafeRelease
#define SafeRelease(_x) { if ((_x) != nullptr) { (_x)->Release(); (_x) = nullptr; } }
#endif

#ifndef ToVariantBool
#define ToVariantBool(_b) ((_b) ? VARIANT_TRUE : VARIANT_FALSE)
#endif

#define VariantInitBool(pv, b)                            \
    do {                                                  \
        VariantInit((pv));                                \
        (pv)->vt = VT_BOOL;                               \
        (pv)->boolVal = ((b) ? VARIANT_TRUE : VARIANT_FALSE); \
    } while (0)

#endif /* MSRDPEX_COM_HELPERS_H */
