
#include "RdpOleSite.h"

CRdpOleClientSite::CRdpOleClientSite(IUnknown* pUnkOuter)
{
    m_refCount = 0;
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

    if (riid == IID_IUnknown) {
        *ppv = static_cast<IUnknown*>(this);
    }
    else if (riid == IID_IOleClientSite) {
        *ppv = static_cast<IOleClientSite*>(this);
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
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::GetMoniker(DWORD dwAssign, DWORD dwWhichMoniker, IMoniker** ppmk)
{
    *ppmk = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::GetContainer(IOleContainer** ppContainer)
{
    *ppContainer = NULL;
    return E_NOTIMPL;
}

STDMETHODIMP CRdpOleClientSite::ShowObject()
{
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::OnShowWindow(BOOL fShow)
{
    return S_OK;
}

STDMETHODIMP CRdpOleClientSite::RequestNewObjectLayout()
{
    return E_NOTIMPL;
}

CRdpOleInPlaceSiteEx::CRdpOleInPlaceSiteEx(IUnknown* pUnkOuter)
    : m_refCount(0), m_hWnd(0)
{
    m_pUnkOuter = pUnkOuter;
    m_pUnkOuter->AddRef();
}

CRdpOleInPlaceSiteEx::~CRdpOleInPlaceSiteEx()
{
    SafeRelease(m_pUnkOuter);
}

// IUnknown methods
STDMETHODIMP CRdpOleInPlaceSiteEx::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    if (!ppv) return E_INVALIDARG;
    *ppv = NULL;

    if (riid == IID_IUnknown) {
        *ppv = static_cast<IUnknown*>(this);
    }
    else if (riid == IID_IOleWindow || riid == IID_IOleInPlaceSite || riid == IID_IOleInPlaceSiteEx) {
        *ppv = static_cast<IOleInPlaceSiteEx*>(this);
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
    *phwnd = m_hWnd;
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::ContextSensitiveHelp(BOOL fEnterMode)
{
    return S_OK;
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

    *ppFrame = NULL;
    *ppDoc = NULL;
    lpFrameInfo = NULL;

    if (GetClientRect(m_hWnd, &rect))
    {
        int width = rect.right - rect.left;
        int height = rect.bottom - rect.top;
        SetRect(lprcClipRect, 0, 0, width, height);
        SetRect(lprcPosRect, 0, 0, width, height);
    }

    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::Scroll(SIZE scrollExtant)
{
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnUIDeactivate(BOOL fUndoable)
{
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
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnPosRectChange(LPCRECT lprcPosRect)
{
    return S_OK;
}

// IOleInPlaceSiteEx methods
STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceActivateEx(BOOL* pfNoRedraw, DWORD dwFlags)
{
    *pfNoRedraw = TRUE;
    return S_OK;
}

STDMETHODIMP CRdpOleInPlaceSiteEx::OnInPlaceDeactivateEx(BOOL fNoRedraw)
{
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
