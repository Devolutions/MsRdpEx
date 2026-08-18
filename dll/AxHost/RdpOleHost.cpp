#include <MsRdpEx/RdpOleHost.h>

#include <new>
#include <ocidl.h>

#include "RdpComBase.h"
#include "RdpOleSite.h"

class CRdpOleHost : public IUnknown
{
public:
    CRdpOleHost()
        : m_refCount(1), m_hWnd(NULL), m_contained(false), m_clientSiteSet(false),
          m_uiActive(false), m_miscStatus(0),
          m_pOleClientSite(NULL), m_pOleInPlaceSiteEx(NULL), m_pOleObject(NULL),
          m_pOleInPlaceObject(NULL), m_pOleInPlaceActiveObject(NULL)
    {
        SetRectEmpty(&m_bounds);
    }

    STDMETHOD(QueryInterface)(REFIID riid, void** ppv) override
    {
        if (!ppv)
            return E_POINTER;

        *ppv = NULL;

        if (riid == IID_IUnknown)
        {
            *ppv = static_cast<IUnknown*>(this);
            AddRef();
        }
        else if ((riid == IID_IOleClientSite || riid == IID_IOleControlSite ||
                  riid == IID_IParseDisplayName || riid == IID_IOleContainer ||
                  riid == IID_IDispatch || riid == IID_ISimpleFrameSite ||
                  riid == IID_IPropertyNotifySink) && m_pOleClientSite)
        {
            return m_pOleClientSite->QueryInterface(riid, ppv);
        }
        else if ((riid == IID_IOleWindow || riid == IID_IOleInPlaceSite ||
                  riid == IID_IOleInPlaceSiteEx || riid == IID_IOleInPlaceUIWindow ||
                  riid == IID_IOleInPlaceFrame) &&
                 m_pOleInPlaceSiteEx)
        {
            return m_pOleInPlaceSiteEx->QueryInterface(riid, ppv);
        }
        else
        {
            return E_NOINTERFACE;
        }

        return S_OK;
    }

    STDMETHOD_(ULONG, AddRef)() override
    {
        return InterlockedIncrement(&m_refCount);
    }

    STDMETHOD_(ULONG, Release)() override
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);
        if (refCount == 0)
            delete this;
        return refCount;
    }

    HRESULT Attach(IUnknown* pControl, HWND hWnd, LPCRECT pBounds)
    {
        HRESULT hr = S_OK;
        IPersistStreamInit* pPersistStreamInit = NULL;

        if (!pControl || !hWnd || !IsWindow(hWnd))
            return E_INVALIDARG;

        if (!pBounds || pBounds->right <= pBounds->left || pBounds->bottom <= pBounds->top)
            return E_INVALIDARG;

        m_hWnd = hWnd;
        m_bounds = *pBounds;

        m_pOleClientSite = new (std::nothrow) CRdpOleClientSite(this);
        m_pOleInPlaceSiteEx = new (std::nothrow) CRdpOleInPlaceSiteEx(this);
        if (!m_pOleClientSite || !m_pOleInPlaceSiteEx)
        {
            hr = E_OUTOFMEMORY;
            goto error;
        }

        m_pOleInPlaceSiteEx->SetWindow(hWnd);

        hr = pControl->QueryInterface(IID_IOleObject, (void**)&m_pOleObject);
        if (FAILED(hr))
            goto error;

        // The Microsoft RDP control advertises SETCLIENTSITEFIRST. Honor the
        // control's actual misc-status contract rather than relying on its
        // historical tolerance of the reverse initialization order.
        hr = m_pOleObject->GetMiscStatus(DVASPECT_CONTENT, &m_miscStatus);
        if (FAILED(hr))
            m_miscStatus = 0;

        if (m_miscStatus & OLEMISC_SETCLIENTSITEFIRST)
        {
            hr = m_pOleObject->SetClientSite(m_pOleClientSite);
            if (FAILED(hr))
                goto error;
            m_clientSiteSet = true;
        }

        hr = pControl->QueryInterface(IID_IOleInPlaceObject, (void**)&m_pOleInPlaceObject);
        if (FAILED(hr))
            goto error;
        m_pOleInPlaceSiteEx->SetInPlaceObject(m_pOleInPlaceObject);

        hr = pControl->QueryInterface(IID_IOleInPlaceActiveObject, (void**)&m_pOleInPlaceActiveObject);
        if (FAILED(hr))
            goto error;

        hr = pControl->QueryInterface(IID_IPersistStreamInit, (void**)&pPersistStreamInit);
        if (SUCCEEDED(hr))
        {
            hr = pPersistStreamInit->InitNew();
            SafeRelease(pPersistStreamInit);
            if (hr == E_NOTIMPL)
                hr = S_OK;
            if (FAILED(hr))
                goto error;
        }
        else if (hr == E_NOINTERFACE)
        {
            hr = S_OK;
        }
        else
        {
            goto error;
        }

        if (!m_clientSiteSet)
        {
            hr = m_pOleObject->SetClientSite(m_pOleClientSite);
            if (FAILED(hr))
                goto error;
            m_clientSiteSet = true;
        }

        m_pOleObject->SetHostNames(L"MsRdpEx", L"RdpClientView");

        hr = OleSetContainedObject(pControl, TRUE);
        if (FAILED(hr))
            goto error;
        m_contained = true;

        hr = m_pOleObject->DoVerb(
            OLEIVERB_INPLACEACTIVATE,
            NULL,
            m_pOleClientSite,
            0,
            hWnd,
            &m_bounds);
        if (FAILED(hr))
        {
            hr = m_pOleObject->DoVerb(
                OLEIVERB_PRIMARY,
                NULL,
                m_pOleClientSite,
                0,
                hWnd,
                &m_bounds);
        }
        if (FAILED(hr))
            goto error;

        return SetBounds(pBounds);

    error:
        SafeRelease(pPersistStreamInit);
        Detach();
        return hr;
    }

    HRESULT SetBounds(LPCRECT pBounds)
    {
        if (!pBounds)
            return E_POINTER;

        if (pBounds->right <= pBounds->left || pBounds->bottom <= pBounds->top)
            return E_INVALIDARG;

        m_bounds = *pBounds;
        if (!m_pOleInPlaceObject)
            return OLE_E_NOT_INPLACEACTIVE;

        if ((m_miscStatus & OLEMISC_RECOMPOSEONRESIZE) && m_pOleObject)
        {
            // Match AxHost's OLE HIMETRIC basis. The in-place rectangle is
            // already in physical pixels; using the host HWND's monitor DPI
            // here causes the control to apply DPI scaling a second time.
            HDC hdc = GetDC(NULL);
            const int dpiX = hdc ? GetDeviceCaps(hdc, LOGPIXELSX) : USER_DEFAULT_SCREEN_DPI;
            const int dpiY = hdc ? GetDeviceCaps(hdc, LOGPIXELSY) : USER_DEFAULT_SCREEN_DPI;
            if (hdc)
                ReleaseDC(NULL, hdc);

            SIZEL extent = {
                MulDiv(pBounds->right - pBounds->left, 2540, dpiX),
                MulDiv(pBounds->bottom - pBounds->top, 2540, dpiY)
            };
            HRESULT hr = m_pOleObject->SetExtent(DVASPECT_CONTENT, &extent);
            if (FAILED(hr))
                return hr;
        }

        return m_pOleInPlaceObject->SetObjectRects(&m_bounds, &m_bounds);
    }

    HRESULT SetActive(BOOL active)
    {
        if (!m_pOleInPlaceActiveObject || !m_pOleObject)
            return OLE_E_NOT_INPLACEACTIVE;

        // Do not issue OLEIVERB_UIACTIVATE here. This host presents the
        // control through the off-screen output mirror and feeds it synthetic
        // input (allowBackgroundInput=1); a real UIACTIVATE makes mstscax call
        // SetFocus on its own window and change the active window, hijacking
        // keyboard focus from the Avalonia surface and breaking later input
        // and teardown. Frame/doc activation notifications give the control
        // the activation state it needs (FocusReleased, focus tracking)
        // without the focus grab.
        if (m_uiActive == (active != FALSE))
            return S_OK;
        m_uiActive = active != FALSE;

        HRESULT frameHr = m_pOleInPlaceActiveObject->OnFrameWindowActivate(active);
        HRESULT docHr = m_pOleInPlaceActiveObject->OnDocWindowActivate(active);

        return FAILED(frameHr) ? frameHr : docHr;
    }

    HRESULT SetFrameActive(BOOL active)
    {
        // Forwards top-level window activation only. Unlike SetActive this
        // never changes the UI-active state: Avalonia retains logical focus
        // when its window deactivates, so the control must stay UI-active.
        return m_pOleInPlaceActiveObject
            ? m_pOleInPlaceActiveObject->OnFrameWindowActivate(active)
            : OLE_E_NOT_INPLACEACTIVE;
    }

    HRESULT SetFrameWindow(HWND hWndFrame)
    {
        if (!m_pOleInPlaceSiteEx)
            return OLE_E_NOT_INPLACEACTIVE;

        return m_pOleInPlaceSiteEx->SetFrameWindow(hWndFrame);
    }

    HRESULT TranslateAccelerator(LPMSG pMsg)
    {
        if (!pMsg)
            return E_POINTER;
        return m_pOleInPlaceActiveObject
            ? m_pOleInPlaceActiveObject->TranslateAccelerator(pMsg)
            : OLE_E_NOT_INPLACEACTIVE;
    }

    void Detach()
    {
        if (m_pOleInPlaceSiteEx)
            m_pOleInPlaceSiteEx->SetInPlaceObject(NULL);

        if (m_pOleInPlaceObject)
        {
            m_pOleInPlaceObject->UIDeactivate();
            m_pOleInPlaceObject->InPlaceDeactivate();
        }

        if (m_pOleObject)
        {
            m_pOleObject->Close(OLECLOSE_NOSAVE);

            if (m_contained)
            {
                OleSetContainedObject(m_pOleObject, FALSE);
                m_contained = false;
            }

            if (m_clientSiteSet)
            {
                m_pOleObject->SetClientSite(NULL);
                m_clientSiteSet = false;
            }
        }

        SafeRelease(m_pOleInPlaceActiveObject);
        SafeRelease(m_pOleInPlaceObject);
        SafeRelease(m_pOleObject);
        SafeRelease(m_pOleInPlaceSiteEx);
        SafeRelease(m_pOleClientSite);

        m_hWnd = NULL;
        m_uiActive = false;
        m_miscStatus = 0;
        SetRectEmpty(&m_bounds);
    }

private:
    ~CRdpOleHost() = default;

    LONG m_refCount;
    HWND m_hWnd;
    RECT m_bounds;
    bool m_contained;
    bool m_clientSiteSet;
    bool m_uiActive;
    DWORD m_miscStatus;
    CRdpOleClientSite* m_pOleClientSite;
    CRdpOleInPlaceSiteEx* m_pOleInPlaceSiteEx;
    IOleObject* m_pOleObject;
    IOleInPlaceObject* m_pOleInPlaceObject;
    IOleInPlaceActiveObject* m_pOleInPlaceActiveObject;
};

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_Attach(
    IUnknown* pControl,
    HWND hWnd,
    LPCRECT pBounds,
    MsRdpEx_RdpOleHost** ppHost)
{
    if (!ppHost)
        return E_POINTER;

    *ppHost = NULL;

    if (!pBounds)
        return E_POINTER;

    CRdpOleHost* pHost = new (std::nothrow) CRdpOleHost();
    if (!pHost)
        return E_OUTOFMEMORY;

    HRESULT hr = pHost->Attach(pControl, hWnd, pBounds);
    if (FAILED(hr))
    {
        pHost->Release();
        return hr;
    }

    *ppHost = reinterpret_cast<MsRdpEx_RdpOleHost*>(pHost);
    return S_OK;
}

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetBounds(
    MsRdpEx_RdpOleHost* pHost,
    LPCRECT pBounds)
{
    if (!pHost)
        return E_POINTER;

    return reinterpret_cast<CRdpOleHost*>(pHost)->SetBounds(pBounds);
}

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetActive(
    MsRdpEx_RdpOleHost* pHost,
    BOOL active)
{
    if (!pHost)
        return E_POINTER;

    return reinterpret_cast<CRdpOleHost*>(pHost)->SetActive(active);
}

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetFrameActive(
    MsRdpEx_RdpOleHost* pHost,
    BOOL active)
{
    if (!pHost)
        return E_POINTER;

    return reinterpret_cast<CRdpOleHost*>(pHost)->SetFrameActive(active);
}

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_SetFrameWindow(
    MsRdpEx_RdpOleHost* pHost,
    HWND hWndFrame)
{
    if (!pHost)
        return E_POINTER;

    return reinterpret_cast<CRdpOleHost*>(pHost)->SetFrameWindow(hWndFrame);
}

HRESULT STDAPICALLTYPE MsRdpEx_RdpOleHost_TranslateAccelerator(
    MsRdpEx_RdpOleHost* pHost,
    LPMSG pMsg)
{
    if (!pHost)
        return E_POINTER;

    return reinterpret_cast<CRdpOleHost*>(pHost)->TranslateAccelerator(pMsg);
}

void STDAPICALLTYPE MsRdpEx_RdpOleHost_Release(MsRdpEx_RdpOleHost* pHost)
{
    if (!pHost)
        return;

    CRdpOleHost* pOleHost = reinterpret_cast<CRdpOleHost*>(pHost);
    pOleHost->Detach();
    pOleHost->Release();
}
