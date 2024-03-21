
#include "RdpEventSink.h"

// IUnknown methods
STDMETHODIMP CRdpEventSink::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    if (!ppv)
        return E_INVALIDARG;

    *ppv = NULL;

    if (riid == IID_IUnknown) {
        *ppv = this;
    }
    else if (riid == IID_IDispatch) {
        *ppv = this;
    }
    else if (riid == DIID_IMsTscAxEvents) {
        *ppv = this;
    }

    if (nullptr != *ppv) {
        ((IUnknown*)*ppv)->AddRef();
    }
    else {
        hr = E_NOINTERFACE;
    }

    return hr;
}

STDMETHODIMP_(ULONG) CRdpEventSink::AddRef(void)
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpEventSink::Release(void)
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    if (refCount != 0) {
        return refCount;
    }

    delete this;
    return 0;
}

// IDispatch methods
STDMETHODIMP CRdpEventSink::GetTypeInfoCount(UINT* pctinfo)
{
    *pctinfo = 0;
    return S_OK;
}

STDMETHODIMP CRdpEventSink::GetTypeInfo(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo)
{
    return E_NOTIMPL;
}

STDMETHODIMP CRdpEventSink::GetIDsOfNames(REFIID riid, LPOLESTR* rgszNames, UINT cNames,
    LCID lcid, DISPID* rgDispId)
{
    return E_NOTIMPL;
}

STDMETHODIMP CRdpEventSink::Invoke(DISPID dispIdMember,
    REFIID riid, LCID lcid, WORD wFlags,
    DISPPARAMS* pDispParams, VARIANT* pVarResult,
    EXCEPINFO* pExcepInfo, UINT* puArgErr)
{
    HRESULT hr = E_NOTIMPL;

    switch (dispIdMember)
    {
    case IMsTscAxEvents_OnConnectingId:
        hr = OnConnecting();
        break;

    case IMsTscAxEvents_OnConnectedId:
        hr = OnConnected();
        break;

    case IMsTscAxEvents_OnLoginCompleteId:
        hr = OnLoginComplete();
        break;

    case IMsTscAxEvents_OnDisconnectedId:
        hr = OnDisconnected(pDispParams->rgvarg->lVal);
        break;

    case IMsTscAxEvents_OnEnterFullScreenModeId:
        hr = OnEnterFullScreenMode();
        break;

    case IMsTscAxEvents_OnLeaveFullScreenModeId:
        hr = OnLeaveFullScreenMode();
        break;

    case IMsTscAxEvents_OnRemoteDesktopSizeChangeId:
        hr = OnRemoteDesktopSizeChange(pDispParams->rgvarg[1].lVal, pDispParams->rgvarg[0].lVal);
        break;

    case IMsTscAxEvents_OnRequestContainerMinimizeId:
        hr = OnRequestContainerMinimize();
        break;

    case IMsTscAxEvents_OnConfirmCloseId:
        hr = OnConfirmClose(pDispParams->rgvarg[0].pboolVal);
        break;
    }

    return hr;
}

// IMsTscAxEvents methods
STDMETHODIMP CRdpEventSink::OnConnecting()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnConnected()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnLoginComplete()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnDisconnected(long discReason)
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnEnterFullScreenMode()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnLeaveFullScreenMode()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnRemoteDesktopSizeChange(long width, long height)
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnRequestContainerMinimize()
{
    return S_OK;
}

STDMETHODIMP CRdpEventSink::OnConfirmClose(VARIANT_BOOL* pfAllowClose)
{
    *pfAllowClose = VARIANT_TRUE;
    return S_OK;
}

CRdpEventSink::CRdpEventSink(HWND hWndParent)
{
    m_refCount = 0;
    m_hWndParent = hWndParent;
}

CRdpEventSink::~CRdpEventSink()
{

}
