#ifndef MSRDPEX_OLE_SITE_H
#define MSRDPEX_OLE_SITE_H

#include <MsRdpEx/MsRdpEx.h>

#include "RdpComBase.h"

class CRdpOleClientSite :
    public IOleClientSite,
    public IOleControlSite,
    public IOleContainer,
    public IDispatch,
    public ISimpleFrameSite,
    public IPropertyNotifySink {
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

    // IParseDisplayName / IOleContainer methods
    STDMETHOD(ParseDisplayName)(IBindCtx* pbc, LPOLESTR pszDisplayName,
        ULONG* pchEaten, IMoniker** ppmkOut) override;
    STDMETHOD(EnumObjects)(DWORD grfFlags, IEnumUnknown** ppenum) override;
    STDMETHOD(LockContainer)(BOOL fLock) override;

    // IOleControlSite methods
    STDMETHOD(OnControlInfoChanged)() override;
    STDMETHOD(LockInPlaceActive)(BOOL fLock) override;
    STDMETHOD(GetExtendedControl)(IDispatch** ppDisp) override;
    STDMETHOD(TransformCoords)(POINTL* pPtlHimetric, POINTF* pPtfContainer, DWORD dwFlags) override;
    STDMETHOD(TranslateAccelerator)(LPMSG pMsg, DWORD grfModifiers) override;
    STDMETHOD(OnFocus)(BOOL fGotFocus) override;
    STDMETHOD(ShowPropertyFrame)() override;

    // IDispatch methods (ambient container properties)
    STDMETHOD(GetTypeInfoCount)(UINT* pctinfo) override;
    STDMETHOD(GetTypeInfo)(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo) override;
    STDMETHOD(GetIDsOfNames)(REFIID riid, LPOLESTR* rgszNames, UINT cNames,
        LCID lcid, DISPID* rgDispId) override;
    STDMETHOD(Invoke)(DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags,
        DISPPARAMS* pDispParams, VARIANT* pVarResult, EXCEPINFO* pExcepInfo,
        UINT* puArgErr) override;

    // ISimpleFrameSite methods
    STDMETHOD(PreMessageFilter)(HWND hWnd, UINT msg, WPARAM wp, LPARAM lp,
        LRESULT* plResult, DWORD* pdwCookie) override;
    STDMETHOD(PostMessageFilter)(HWND hWnd, UINT msg, WPARAM wp, LPARAM lp,
        LRESULT* plResult, DWORD dwCookie) override;

    // IPropertyNotifySink methods
    STDMETHOD(OnChanged)(DISPID dispID) override;
    STDMETHOD(OnRequestEdit)(DISPID dispID) override;

private:
    ULONG m_refCount;
    IUnknown* m_pUnkOuter;
};

class CRdpOleInPlaceSiteEx : public IOleInPlaceSiteEx, public IOleInPlaceFrame {
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

    // IOleInPlaceUIWindow methods
    STDMETHOD(GetBorder)(LPRECT lprectBorder) override;
    STDMETHOD(RequestBorderSpace)(LPCBORDERWIDTHS pborderwidths) override;
    STDMETHOD(SetBorderSpace)(LPCBORDERWIDTHS pborderwidths) override;
    STDMETHOD(SetActiveObject)(IOleInPlaceActiveObject* pActiveObject, LPCOLESTR pszObjName) override;

    // IOleInPlaceFrame methods
    STDMETHOD(InsertMenus)(HMENU hmenuShared, LPOLEMENUGROUPWIDTHS lpMenuWidths) override;
    STDMETHOD(SetMenu)(HMENU hmenuShared, HOLEMENU holemenu, HWND hwndActiveObject) override;
    STDMETHOD(RemoveMenus)(HMENU hmenuShared) override;
    STDMETHOD(SetStatusText)(LPCOLESTR pszStatusText) override;
    STDMETHOD(EnableModeless)(BOOL fEnable) override;
    STDMETHOD(TranslateAccelerator)(LPMSG lpmsg, WORD wID) override;

    // Additional methods specific to your implementation
    STDMETHOD(SetWindow)(HWND hWnd);
    STDMETHOD(SetInPlaceObject)(IOleInPlaceObject* pOleInPlaceObject);

private:
    ULONG m_refCount;
    HWND m_hWnd;
    IOleInPlaceObject* m_pOleInPlaceObject; // weak; owned by the outer host
    IUnknown* m_pUnkOuter;
};

#endif /* MSRDPEX_OLE_SITE_H */
