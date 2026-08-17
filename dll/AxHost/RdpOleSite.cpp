
#include "RdpOleSite.h"

CRdpOleClientSite::CRdpOleClientSite(IUnknown* pUnkOuter)
{
    m_refCount = 1;
    m_pUnkOuter = pUnkOuter;
    m_pUnkOuter->AddRef();
}

CRdpOleClientSite::~CRdpOleClientSite()
{
    SafeRelease(m_pUnkOuter);
}

// IUnknown methods
STDMETHODIMP CRdpOleClientSite::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    if (!ppv)
        return E_INVALIDARG;

    *ppv = NULL;

    // All site tear-offs must expose the outer host as their controlling
    // IUnknown. Returning a different IUnknown from each site violates COM
    // identity and confuses controls that cache more than one site interface.
    if (riid == IID_IUnknown && m_pUnkOuter) {
        return m_pUnkOuter->QueryInterface(riid, ppv);
    }
    else if (riid == IID_IOleClientSite) {
        *ppv = static_cast<IOleClientSite*>(this);
    }
    else if (riid == IID_IOleControlSite) {
        *ppv = static_cast<IOleControlSite*>(this);
    }
    else if (riid == IID_IParseDisplayName || riid == IID_IOleContainer) {
        *ppv = static_cast<IOleContainer*>(this);
    }
    else if (riid == IID_IDispatch) {
        *ppv = static_cast<IDispatch*>(this);
    }
    else if (riid == IID_ISimpleFrameSite) {
        *ppv = static_cast<ISimpleFrameSite*>(this);
    }
    else if (riid == IID_IPropertyNotifySink) {
        *ppv = static_cast<IPropertyNotifySink*>(this);
    }
    else if (m_pUnkOuter) {
        return m_pUnkOuter->QueryInterface(riid, ppv);
    }
    else {
        hr = E_NOINTERFACE;
    }

    if (*ppv) {
        AddRef();
    }

    return hr;
}

STDMETHODIMP_(ULONG) CRdpOleClientSite::AddRef()
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpOleClientSite::Release()
{
    ULONG refCount = InterlockedDecrement(&m_refCount);
    if (refCount == 0) {
        delete this;
    }
    return refCount;
}

// IOleClientSite methods
STDMETHODIMP CRdpOleClientSite::SaveObject()
{
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk)
{
    UNREFERENCED_PARAMETER(dwAssign);
    UNREFERENCED_PARAMETER(dwWhichMoniker);

    if (!ppmk)
        return E_POINTER;

    *ppmk = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::GetContainer(IOleContainer** ppContainer)
{
    if (!ppContainer)
        return E_POINTER;

    *ppContainer = static_cast<IOleContainer*>(this);
    AddRef();
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::ShowObject()
{
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::OnShowWindow(BOOL fShow)
{
    UNREFERENCED_PARAMETER(fShow);
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::RequestNewObjectLayout()
{
    return E_NOTIMPL;
}

// IParseDisplayName / IOleContainer methods
STDMETHODIMP CRdpOleClientSite::ParseDisplayName(
    IBindCtx* pbc, LPOLESTR pszDisplayName, ULONG* pchEaten, IMoniker** ppmkOut)
{
    UNREFERENCED_PARAMETER(pbc);
    UNREFERENCED_PARAMETER(pszDisplayName);
    if (!pchEaten || !ppmkOut)
        return E_POINTER;
    *pchEaten = 0;
    *ppmkOut = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::EnumObjects(DWORD grfFlags, IEnumUnknown** ppenum)
{
    UNREFERENCED_PARAMETER(grfFlags);
    if (!ppenum)
        return E_POINTER;
    *ppenum = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::LockContainer(BOOL fLock)
{
    UNREFERENCED_PARAMETER(fLock);
    return S_OK;
}

// IOleControlSite methods
STDMETHODIMP CRdpOleClientSite::OnControlInfoChanged()
{
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::LockInPlaceActive(BOOL fLock)
{
    UNREFERENCED_PARAMETER(fLock);
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::GetExtendedControl(IDispatch** ppDisp)
{
    if (!ppDisp)
        return E_POINTER;

    *ppDisp = NULL;
    return E_NOTIMPL;
}

static HRESULT RdpOleGetLogPixels(IUnknown* pUnkOuter, int* pLogPixelsX, int* pLogPixelsY)
{
    UNREFERENCED_PARAMETER(pUnkOuter);

    if (!pLogPixelsX || !pLogPixelsY)
        return E_POINTER;

    *pLogPixelsX = USER_DEFAULT_SCREEN_DPI;
    *pLogPixelsY = USER_DEFAULT_SCREEN_DPI;

    // OLE HIMETRIC conversion uses the screen/logical coordinate basis. Using
    // the hidden host HWND's per-monitor DPI double-scales the RDP child in a
    // PerMonitorV2 process (for example, 640 becomes 426 at 150% scaling).
    HDC hdc = GetDC(NULL);
    if (!hdc)
        return E_FAIL;

    *pLogPixelsX = GetDeviceCaps(hdc, LOGPIXELSX);
    *pLogPixelsY = GetDeviceCaps(hdc, LOGPIXELSY);
    ReleaseDC(NULL, hdc);
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::TransformCoords(
    POINTL* pPtlHimetric, POINTF* pPtfContainer, DWORD dwFlags)
{
    if (!pPtlHimetric || !pPtfContainer)
        return E_INVALIDARG;

    const DWORD direction = dwFlags & (XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_CONTAINERTOHIMETRIC);
    const DWORD kind = dwFlags & (XFORMCOORDS_POSITION | XFORMCOORDS_SIZE);
    if ((direction != XFORMCOORDS_HIMETRICTOCONTAINER &&
         direction != XFORMCOORDS_CONTAINERTOHIMETRIC) ||
        (kind != XFORMCOORDS_POSITION && kind != XFORMCOORDS_SIZE))
    {
        return E_INVALIDARG;
    }

    int logPixelsX = USER_DEFAULT_SCREEN_DPI;
    int logPixelsY = USER_DEFAULT_SCREEN_DPI;
    RdpOleGetLogPixels(m_pUnkOuter, &logPixelsX, &logPixelsY);

    if (direction == XFORMCOORDS_HIMETRICTOCONTAINER)
    {
        pPtfContainer->x = (FLOAT)MulDiv(pPtlHimetric->x, logPixelsX, 2540);
        pPtfContainer->y = (FLOAT)MulDiv(pPtlHimetric->y, logPixelsY, 2540);
    }
    else
    {
        pPtlHimetric->x = MulDiv((LONG)pPtfContainer->x, 2540, logPixelsX);
        pPtlHimetric->y = MulDiv((LONG)pPtfContainer->y, 2540, logPixelsY);
    }

    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::TranslateAccelerator(LPMSG pMsg, DWORD grfModifiers)
{
    UNREFERENCED_PARAMETER(grfModifiers);
    return pMsg ? S_FALSE : E_POINTER;
}

STDMETHODIMP CRdpOleClientSite::OnFocus(BOOL fGotFocus)
{
    UNREFERENCED_PARAMETER(fGotFocus);
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::ShowPropertyFrame()
{
    return E_NOTIMPL;
}

// IDispatch ambient properties
STDMETHODIMP CRdpOleClientSite::GetTypeInfoCount(UINT* pctinfo)
{
    if (!pctinfo)
        return E_POINTER;

    *pctinfo = 0;
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo)
{
    UNREFERENCED_PARAMETER(iTInfo);
    UNREFERENCED_PARAMETER(lcid);

    if (!ppTInfo)
        return E_POINTER;

    *ppTInfo = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::GetIDsOfNames(
    REFIID riid, LPOLESTR* rgszNames, UINT cNames, LCID lcid, DISPID* rgDispId)
{
    UNREFERENCED_PARAMETER(riid);
    UNREFERENCED_PARAMETER(rgszNames);
    UNREFERENCED_PARAMETER(cNames);
    UNREFERENCED_PARAMETER(lcid);

    if (rgDispId)
        *rgDispId = DISPID_UNKNOWN;
    return DISP_E_UNKNOWNNAME;
}

STDMETHODIMP CRdpOleClientSite::Invoke(
    DISPID dispIdMember, REFIID riid, LCID lcid, WORD wFlags,
    DISPPARAMS* pDispParams, VARIANT* pVarResult, EXCEPINFO* pExcepInfo,
    UINT* puArgErr)
{
    UNREFERENCED_PARAMETER(riid);
    UNREFERENCED_PARAMETER(lcid);
    UNREFERENCED_PARAMETER(pDispParams);
    UNREFERENCED_PARAMETER(pExcepInfo);
    UNREFERENCED_PARAMETER(puArgErr);

    if (!pVarResult)
        return E_POINTER;
    if (!(wFlags & DISPATCH_PROPERTYGET))
        return DISP_E_MEMBERNOTFOUND;

    VariantInit(pVarResult);

    switch (dispIdMember)
    {
        case DISPID_AMBIENT_USERMODE:
        case DISPID_AMBIENT_MESSAGEREFLECT:
        case DISPID_AMBIENT_SUPPORTSMNEMONICS:
        case DISPID_AMBIENT_AUTOCLIP:
            V_VT(pVarResult) = VT_BOOL;
            V_BOOL(pVarResult) = VARIANT_TRUE;
            return S_OK;

        case DISPID_AMBIENT_UIDEAD:
        case DISPID_AMBIENT_SHOWGRABHANDLES:
        case DISPID_AMBIENT_SHOWHATCHING:
        case DISPID_AMBIENT_DISPLAYASDEFAULT:
            V_VT(pVarResult) = VT_BOOL;
            V_BOOL(pVarResult) = VARIANT_FALSE;
            return S_OK;

        case DISPID_AMBIENT_LOCALEID:
            V_VT(pVarResult) = VT_I4;
            V_I4(pVarResult) = (LONG)GetThreadLocale();
            return S_OK;

        case DISPID_AMBIENT_BACKCOLOR:
            V_VT(pVarResult) = VT_I4;
            V_I4(pVarResult) = (LONG)GetSysColor(COLOR_WINDOW);
            return S_OK;

        case DISPID_AMBIENT_FORECOLOR:
            V_VT(pVarResult) = VT_I4;
            V_I4(pVarResult) = (LONG)GetSysColor(COLOR_WINDOWTEXT);
            return S_OK;

        case DISPID_AMBIENT_DISPLAYNAME:
            V_VT(pVarResult) = VT_BSTR;
            V_BSTR(pVarResult) = SysAllocString(L"RdpClientView");
            return V_BSTR(pVarResult) ? S_OK : E_OUTOFMEMORY;

        case DISPID_AMBIENT_APPEARANCE:
            V_VT(pVarResult) = VT_I2;
            V_I2(pVarResult) = 0;
            return S_OK;

        default:
            return DISP_E_MEMBERNOTFOUND;
    }
}

// ISimpleFrameSite methods
STDMETHODIMP CRdpOleClientSite::PreMessageFilter(
    HWND hWnd, UINT msg, WPARAM wp, LPARAM lp, LRESULT* plResult, DWORD* pdwCookie)
{
    UNREFERENCED_PARAMETER(hWnd);
    UNREFERENCED_PARAMETER(msg);
    UNREFERENCED_PARAMETER(wp);
    UNREFERENCED_PARAMETER(lp);

    if (!plResult || !pdwCookie)
        return E_POINTER;

    *plResult = 0;
    *pdwCookie = 0;
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::PostMessageFilter(
    HWND hWnd, UINT msg, WPARAM wp, LPARAM lp, LRESULT* plResult, DWORD dwCookie)
{
    UNREFERENCED_PARAMETER(hWnd);
    UNREFERENCED_PARAMETER(msg);
    UNREFERENCED_PARAMETER(wp);
    UNREFERENCED_PARAMETER(lp);
    UNREFERENCED_PARAMETER(dwCookie);

    if (!plResult)
        return E_POINTER;

    *plResult = 0;
    return S_FALSE;
}

// IPropertyNotifySink methods
STDMETHODIMP CRdpOleClientSite::OnChanged(DISPID dispID)
{
    UNREFERENCED_PARAMETER(dispID);
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::OnRequestEdit(DISPID dispID)
{
    UNREFERENCED_PARAMETER(dispID);
    return S_OK;
}

CRdpOleInPlaceSiteEx::CRdpOleInPlaceSiteEx(IUnknown* pUnkOuter)
    : m_refCount(1), m_hWnd(0), m_pOleInPlaceObject(NULL)
{
    m_pUnkOuter = pUnkOuter;
    m_pUnkOuter->AddRef();
}

CRdpOleInPlaceSiteEx::~CRdpOleInPlaceSiteEx()
{
    m_pOleInPlaceObject = NULL;
    SafeRelease(m_pUnkOuter);
}

// IUnknown methods
STDMETHODIMP CRdpOleInPlaceSiteEx::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    if (!ppv) return E_INVALIDARG;
    *ppv = NULL;

    if (riid == IID_IUnknown && m_pUnkOuter) {
        return m_pUnkOuter->QueryInterface(riid, ppv);
    }
    else if (riid == IID_IOleWindow || riid == IID_IOleInPlaceSite || riid == IID_IOleInPlaceSiteEx) {
        *ppv = static_cast<IOleInPlaceSiteEx*>(this);
    }
    else if (riid == IID_IOleInPlaceUIWindow || riid == IID_IOleInPlaceFrame) {
        *ppv = static_cast<IOleInPlaceFrame*>(this);
    }
    else if (m_pUnkOuter) {
        return m_pUnkOuter->QueryInterface(riid, ppv);
    }
    else {
        hr = E_NOINTERFACE;
    }

    if (*ppv) {
        AddRef();
    }

    return hr;
}

STDMETHODIMP_(ULONG) CRdpOleInPlaceSiteEx::AddRef()
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpOleInPlaceSiteEx::Release()
{
    ULONG refCount = InterlockedDecrement(&m_refCount);
    if (refCount == 0) {
        delete this;
    }
    return refCount;
}

// IOleWindow methods
STDMETHODIMP CRdpOleInPlaceSiteEx::GetWindow(HWND* phwnd)
{
    if (!phwnd)
        return E_POINTER;

    *phwnd = m_hWnd;
    return m_hWnd ? S_OK : E_FAIL;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::ContextSensitiveHelp(BOOL fEnterMode)
{
    UNREFERENCED_PARAMETER(fEnterMode);
    return E_NOTIMPL;
}

// IOleInPlaceSite methods
STDMETHODIMP CRdpOleInPlaceSiteEx::CanInPlaceActivate()
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceActivate()
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnUIActivate()
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::GetWindowContext(IOleInPlaceFrame** ppFrame, IOleInPlaceUIWindow** ppDoc,
    LPRECT lprcPosRect, LPRECT lprcClipRect,
    LPOLEINPLACEFRAMEINFO lpFrameInfo)
{
    RECT rect;

    if (!ppFrame || !ppDoc || !lprcPosRect || !lprcClipRect)
        return E_POINTER;

    *ppFrame = NULL;
    *ppDoc = NULL;

    if (lpFrameInfo)
    {
        lpFrameInfo->cb = sizeof(OLEINPLACEFRAMEINFO);
        lpFrameInfo->fMDIApp = FALSE;
        lpFrameInfo->hwndFrame = m_hWnd;
        lpFrameInfo->haccel = NULL;
        lpFrameInfo->cAccelEntries = 0;
    }

    if (!m_hWnd || !GetClientRect(m_hWnd, &rect))
        return E_FAIL;

    *ppFrame = static_cast<IOleInPlaceFrame*>(this);
    AddRef();

    int width = rect.right - rect.left;
    int height = rect.bottom - rect.top;
    SetRect(lprcClipRect, 0, 0, width, height);
    SetRect(lprcPosRect, 0, 0, width, height);

    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::Scroll(SIZE scrollExtant)
{
    UNREFERENCED_PARAMETER(scrollExtant);
    return S_FALSE;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnUIDeactivate(BOOL fUndoable)
{
    UNREFERENCED_PARAMETER(fUndoable);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceDeactivate()
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::DiscardUndoState()
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::DeactivateAndUndo()
{
    return m_pOleInPlaceObject ? m_pOleInPlaceObject->UIDeactivate() : OLE_E_NOT_INPLACEACTIVE;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnPosRectChange(LPCRECT lprcPosRect)
{
    if (!lprcPosRect)
        return E_POINTER;
    return m_pOleInPlaceObject
        ? m_pOleInPlaceObject->SetObjectRects(lprcPosRect, lprcPosRect)
        : OLE_E_NOT_INPLACEACTIVE;
}

// IOleInPlaceSiteEx methods
STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceActivateEx(BOOL* pfNoRedraw, DWORD dwFlags)
{
    UNREFERENCED_PARAMETER(dwFlags);

    if (pfNoRedraw)
        *pfNoRedraw = FALSE;
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceDeactivateEx(BOOL fNoRedraw)
{
    UNREFERENCED_PARAMETER(fNoRedraw);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::RequestUIActivate()
{
    return S_OK;
}

// additional methods
STDMETHODIMP CRdpOleInPlaceSiteEx::SetWindow(HWND hWnd)
{
    m_hWnd = hWnd;
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::SetInPlaceObject(IOleInPlaceObject* pOleInPlaceObject)
{
    m_pOleInPlaceObject = pOleInPlaceObject;
    return S_OK;
}

// IOleInPlaceUIWindow methods
STDMETHODIMP CRdpOleInPlaceSiteEx::GetBorder(LPRECT lprectBorder)
{
    if (!lprectBorder)
        return E_POINTER;
    if (!m_hWnd || !GetClientRect(m_hWnd, lprectBorder))
        return E_FAIL;
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::RequestBorderSpace(LPCBORDERWIDTHS pborderwidths)
{
    UNREFERENCED_PARAMETER(pborderwidths);
    return INPLACE_E_NOTOOLSPACE;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::SetBorderSpace(LPCBORDERWIDTHS pborderwidths)
{
    UNREFERENCED_PARAMETER(pborderwidths);
    return INPLACE_E_NOTOOLSPACE;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::SetActiveObject(
    IOleInPlaceActiveObject* pActiveObject, LPCOLESTR pszObjName)
{
    UNREFERENCED_PARAMETER(pActiveObject);
    UNREFERENCED_PARAMETER(pszObjName);
    return S_OK;
}

// IOleInPlaceFrame methods. This host deliberately supplies no merged menus or
// tool space, but it must provide a real frame identity for OLE activation.
STDMETHODIMP CRdpOleInPlaceSiteEx::InsertMenus(
    HMENU hmenuShared, LPOLEMENUGROUPWIDTHS lpMenuWidths)
{
    UNREFERENCED_PARAMETER(hmenuShared);
    UNREFERENCED_PARAMETER(lpMenuWidths);
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::SetMenu(
    HMENU hmenuShared, HOLEMENU holemenu, HWND hwndActiveObject)
{
    UNREFERENCED_PARAMETER(hmenuShared);
    UNREFERENCED_PARAMETER(holemenu);
    UNREFERENCED_PARAMETER(hwndActiveObject);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::RemoveMenus(HMENU hmenuShared)
{
    UNREFERENCED_PARAMETER(hmenuShared);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::SetStatusText(LPCOLESTR pszStatusText)
{
    UNREFERENCED_PARAMETER(pszStatusText);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::EnableModeless(BOOL fEnable)
{
    UNREFERENCED_PARAMETER(fEnable);
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::TranslateAccelerator(LPMSG lpmsg, WORD wID)
{
    UNREFERENCED_PARAMETER(wID);
    return lpmsg ? S_FALSE : E_POINTER;
}
