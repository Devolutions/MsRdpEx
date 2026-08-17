#ifndef MSRDPEX_RDP_OLE_HOST_H
#define MSRDPEX_RDP_OLE_HOST_H

#include <MsRdpEx/MsRdpEx.h>

#include <oleidl.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_RdpOleHost MsRdpEx_RdpOleHost;

/*
 * Activates an OLE control in-place in hWnd and owns the OLE client-site and
 * in-place-site lifetime. The caller retains ownership of pControl and hWnd.
 * Release the returned opaque host with MsRdpEx_RdpOleHost_Release before the
 * window is destroyed.
 */
HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_Attach(
    IUnknown* pControl,
    HWND hWnd,
    LPCRECT pBounds,
    MsRdpEx_RdpOleHost** ppHost);

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetBounds(
    MsRdpEx_RdpOleHost* pHost,
    LPCRECT pBounds);

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetActive(
    MsRdpEx_RdpOleHost* pHost,
    BOOL active);

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_TranslateAccelerator(
    MsRdpEx_RdpOleHost* pHost,
    LPMSG pMsg);

void STDAPICALLTYPE MsRdpEx_RdpOleHost_Release(
    MsRdpEx_RdpOleHost* pHost);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_RDP_OLE_HOST_H */
