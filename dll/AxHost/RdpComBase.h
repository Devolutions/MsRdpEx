#ifndef MSRDPEX_COM_HELPER_H
#define MSRDPEX_COM_HELPER_H

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

#include "../com/mstscax.tlh"
using namespace MSTSCLib;

#endif /* MSRDPEX_COM_HELPER_H */