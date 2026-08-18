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
 * window is destroyed. The caller must initialize OLE on the calling thread
 * before Attach. The returned host, its HWND, and every host operation are
 * apartment-bound: call SetBounds, SetActive, TranslateAccelerator, and Release
 * only from the same thread that called Attach.
 */
HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_Attach(
    IUnknown* pControl,
    HWND hWnd,
    LPCRECT pBounds,
    MsRdpEx_RdpOleHost** ppHost);

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetBounds(
    MsRdpEx_RdpOleHost* pHost,
    LPCRECT pBounds);

/*
 * Pushes focus-driven activation state (OnFrameWindowActivate /
 * OnDocWindowActivate) to the control. This deliberately does not issue
 * OLEIVERB_UIACTIVATE: in the output-mirror hosting model the control is fed
 * synthetic input (allowBackgroundInput=1), and a real UI-activate makes
 * mstscax grab keyboard focus and change the active window, hijacking input
 * from the host surface.
 */
HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetActive(
    MsRdpEx_RdpOleHost* pHost,
    BOOL active);

/*
 * Forwards top-level window activation (OnFrameWindowActivate) without
 * changing the control's UI-active state. Use this for window
 * Activated/Deactivated notifications; use SetActive for focus changes.
 */
HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetFrameActive(
    MsRdpEx_RdpOleHost* pHost,
    BOOL active);

/*
 * Sets the real top-level window reported as the OLE frame window
 * (OLEINPLACEFRAMEINFO::hwndFrame) so control-owned dialogs such as
 * certificate and credential prompts are parented to a visible window.
 * The control stays parented to the HWND passed to Attach. Pass NULL to
 * fall back to the Attach HWND.
 */
HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetFrameWindow(
    MsRdpEx_RdpOleHost* pHost,
    HWND hWndFrame);

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_TranslateAccelerator(
    MsRdpEx_RdpOleHost* pHost,
    LPMSG pMsg);

void STDAPICALLTYPE MsRdpEx_RdpOleHost_Release(
    MsRdpEx_RdpOleHost* pHost);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_RDP_OLE_HOST_H */
