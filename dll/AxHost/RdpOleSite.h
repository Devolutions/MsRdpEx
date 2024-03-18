#ifndef MSRDPEX_OLE_SITE_H
#define MSRDPEX_OLE_SITE_H

#include <MsRdpEx/MsRdpEx.h>

#include "RdpComBase.h"

class CRdpOleClientSite : public IOleClientSite {
public:
    // Constructor and Destructor
    CRdpOleClientSite(IUnknown* pUnkOuter);
    virtual ~CRdpOleClientSite();

    // IUnknown methods
    STDMETHOD(QueryInterface)(REFIID riid, void** ppv) override;
    STDMETHOD_(ULONG, AddRef)() override;
    STDMETHOD_(ULONG, Release)() override;

    // IOleClientSite methods
    STDMETHOD(SaveObject)() override;
    STDMETHOD(GetMoniker)(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk) override;
    STDMETHOD(GetContainer)(IOleContainer** ppContainer) override;
    STDMETHOD(ShowObject)() override;
    STDMETHOD(OnShowWindow)(BOOL fShow) override;
    STDMETHOD(RequestNewObjectLayout)() override;

private:
    ULONG m_refCount;
    IUnknown* m_pUnkOuter;
};

class CRdpOleInPlaceSiteEx : public IOleInPlaceSiteEx {
public:
    // Constructor and Destructor
    CRdpOleInPlaceSiteEx(IUnknown* pUnkOuter);
    virtual ~CRdpOleInPlaceSiteEx();

    // IUnknown methods
    STDMETHOD(QueryInterface)(REFIID riid, void** ppv) override;
    STDMETHOD_(ULONG, AddRef)() override;
    STDMETHOD_(ULONG, Release)() override;

    // IOleWindow methods
    STDMETHOD(GetWindow)(HWND* phwnd) override;
    STDMETHOD(ContextSensitiveHelp)(BOOL fEnterMode) override;

    // IOleInPlaceSite methods
    STDMETHOD(CanInPlaceActivate)() override;
    STDMETHOD(OnInPlaceActivate)() override;
    STDMETHOD(OnUIActivate)() override;
    STDMETHOD(GetWindowContext)(IOleInPlaceFrame** ppFrame,
        IOleInPlaceUIWindow** ppDoc,
        LPRECT lprcPosRect, LPRECT lprcClipRect,
        LPOLEINPLACEFRAMEINFO lpFrameInfo) override;
    STDMETHOD(Scroll)(SIZE scrollExtant) override;
    STDMETHOD(OnUIDeactivate)(BOOL fUndoable) override;
    STDMETHOD(OnInPlaceDeactivate)() override;
    STDMETHOD(DiscardUndoState)() override;
    STDMETHOD(DeactivateAndUndo)() override;
    STDMETHOD(OnPosRectChange)(LPCRECT lprcPosRect) override;

    // IOleInPlaceSiteEx methods
    STDMETHOD(OnInPlaceActivateEx)(BOOL* pfNoRedraw, DWORD dwFlags) override;
    STDMETHOD(OnInPlaceDeactivateEx)(BOOL fNoRedraw) override;
    STDMETHOD(RequestUIActivate)() override;

    // Additional methods specific to your implementation
    STDMETHOD(SetWindow)(HWND hWnd);

private:
    ULONG m_refCount;
    HWND m_hWnd;
    IUnknown* m_pUnkOuter;
};

#endif /* MSRDPEX_OLE_SITE_H */