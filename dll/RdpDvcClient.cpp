
#include "RdpDvcClient.h"

#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/RdpInstance.h>

//
// CRdpDvcClient class
//

// IUnknown methods

STDMETHODIMP CRdpDvcClient::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    if (!ppv)
        return E_INVALIDARG;

    *ppv = NULL;

    if (riid == (REFIID) IID_IUnknown) {
        *ppv = this;
    }
    else if (riid == (REFIID) IID_IWTSVirtualChannelCallback) {
        MsRdpEx_LogPrint(DEBUG, "CRdpDvcClient::QueryInterface(IID_IWTSVirtualChannelCallback)");
        *ppv = static_cast<IWTSVirtualChannelCallback*>(this);
    }

    if (nullptr != *ppv) {
        ((IUnknown*)*ppv)->AddRef();
    }
    else {
        hr = E_NOINTERFACE;
    }

    MsRdpEx_LogPrint(DEBUG, "CRdpDvcClient::QueryInterface(%s), hr = 0x%08X", iid, hr);

    return hr;
}

STDMETHODIMP_(ULONG) CRdpDvcClient::AddRef(void)
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpDvcClient::Release(void)
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    if (refCount != 0) {
        return refCount;
    }

    delete this;
    return 0;
}

// IWTSVirtualChannelCallback methods

HRESULT STDMETHODCALLTYPE CRdpDvcClient::OnDataReceived(ULONG cbSize, BYTE* pBuffer)
{
    MsRdpEx_LogPrint(DEBUG, "CRdpDvcClient::OnDataReceived(%s)", (const char*)pBuffer);
    HRESULT hr = m_pChannel->Write(cbSize, pBuffer, NULL);
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CRdpDvcClient::OnClose(void)
{
    MsRdpEx_LogPrint(DEBUG, "CRdpDvcClient::OnClose()");
    return S_OK;
}

// Additional methods

void CRdpDvcClient::SetChannel(IWTSVirtualChannel* pChannel)
{
    m_pChannel = pChannel;
}

void CRdpDvcClient::SetListener(IWTSListener* pListener)
{
    m_pListener = pListener;
}

// Additional methods

CRdpDvcClient::CRdpDvcClient(void)
{

}

CRdpDvcClient::~CRdpDvcClient()
{

}

//
// CRdpDvcListener class
//

// IUnknown methods

STDMETHODIMP CRdpDvcListener::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    if (!ppv)
        return E_INVALIDARG;

    *ppv = NULL;

    if (riid == IID_IUnknown) {
        *ppv = this;
    }
    else if (riid == IID_IWTSListenerCallback) {
        *ppv = static_cast<IWTSListenerCallback*>(this);
    }

    if (nullptr != *ppv) {
        ((IUnknown*)*ppv)->AddRef();
    }
    else {
        hr = E_NOINTERFACE;
    }

    MsRdpEx_LogPrint(DEBUG, "CRdpDvcListener::QueryInterface(%s), hr = 0x%08X", iid, hr);

    return hr;
}

STDMETHODIMP_(ULONG) CRdpDvcListener::AddRef(void)
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpDvcListener::Release(void)
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    if (refCount != 0) {
        return refCount;
    }

    delete this;
    return 0;
}

// IWTSListenerCallback methods

HRESULT STDMETHODCALLTYPE CRdpDvcListener::OnNewChannelConnection(IWTSVirtualChannel* pChannel,
    BSTR data, BOOL* pbAccept, IWTSVirtualChannelCallback** ppCallback)
{
    HRESULT hr = S_OK;
    CRdpDvcClient* dvcClient = new CRdpDvcClient();
    IWTSVirtualChannelCallback* pIWTSVirtualChannelCallback = NULL;

    MsRdpEx_LogPrint(DEBUG, "CRdpDvcListener::OnNewChannelConnection()");

    hr = dvcClient->QueryInterface(IID_IWTSVirtualChannelCallback, (void**)&pIWTSVirtualChannelCallback);

    if (FAILED(hr)) {
        return hr;
    }

    dvcClient->SetChannel(pChannel);

    *pbAccept = TRUE;
    *ppCallback = pIWTSVirtualChannelCallback;

    return hr;
}

// Additional methods

CRdpDvcListener::CRdpDvcListener(void)
{

}

CRdpDvcListener::~CRdpDvcListener()
{

}

//
// CRdpDvcPlugin class
//

// IUnknown methods

STDMETHODIMP CRdpDvcPlugin::QueryInterface(REFIID riid, void** ppv)
{
    HRESULT hr = S_OK;

    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    MsRdpEx_LogPrint(DEBUG, "CRdpDvcPlugin::QueryInterface(%s)", iid);

    if (!ppv)
        return E_INVALIDARG;

    *ppv = NULL;

    if (riid == IID_IUnknown) {
        *ppv = this;
    }
    else if (riid == IID_IWTSPlugin) {
        *ppv = static_cast<IWTSPlugin*>(this);
    }

    if (nullptr != *ppv) {
        ((IUnknown*)*ppv)->AddRef();
    }
    else {
        hr = E_NOINTERFACE;
    }

    return hr;
}

STDMETHODIMP_(ULONG) CRdpDvcPlugin::AddRef(void)
{
    return InterlockedIncrement(&m_refCount);
}

STDMETHODIMP_(ULONG) CRdpDvcPlugin::Release(void)
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    if (refCount != 0) {
        return refCount;
    }

    delete this;
    return 0;
}

// IWTSPlugin methods

HRESULT STDMETHODCALLTYPE CRdpDvcPlugin::Initialize(IWTSVirtualChannelManager* pChannelMgr)
{
    HRESULT hr = S_OK;
    CRdpDvcListener* dvcListener = new CRdpDvcListener();
    IWTSListener* pIWTSListener = NULL;
    IWTSListenerCallback* pIWTSListenerCallback = NULL;

    hr = dvcListener->QueryInterface(IID_IWTSListenerCallback, (void**)&pIWTSListenerCallback);

    if (FAILED(hr)) {
        return hr;
    }

    hr = pChannelMgr->CreateListener("DvcSample", 0, pIWTSListenerCallback, &pIWTSListener);

    return hr;
}

HRESULT STDMETHODCALLTYPE CRdpDvcPlugin::Connected(void)
{
    MsRdpEx_LogPrint(DEBUG, "CRdpDvcPlugin::Connected()");
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CRdpDvcPlugin::Disconnected(DWORD dwDisconnectCode)
{
    MsRdpEx_LogPrint(DEBUG, "CRdpDvcPlugin::Disconnected()");
    return S_OK;
}

HRESULT STDMETHODCALLTYPE CRdpDvcPlugin::Terminated(void)
{
    MsRdpEx_LogPrint(DEBUG, "CRdpDvcPlugin::Terminated()");
    return S_OK;
}

// Additional methods

CRdpDvcPlugin::CRdpDvcPlugin(void)
{

}

CRdpDvcPlugin::~CRdpDvcPlugin()
{

}

// CDvcPluginClassFactory class

class CDvcPluginClassFactory : IClassFactory
{
public:
    CDvcPluginClassFactory(CMsRdpExInstance* instance)
    {
        m_instance = instance;
    }

    ~CDvcPluginClassFactory()
    {

    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, LPVOID* ppvObject)
    {
        HRESULT hr = E_NOINTERFACE;

        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        MsRdpEx_LogPrint(DEBUG, "CDvcPluginClassFactory::QueryInterface(%s)", iid);

        if (riid == IID_IUnknown) {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        if (riid == IID_IClassFactory) {
            *ppvObject = (LPVOID)((IClassFactory*)this);
            m_refCount++;
            return S_OK;
        }

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        return InterlockedIncrement(&m_refCount);
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        if (refCount != 0) {
            return refCount;
        }

        delete this;
        return 0;
    }

    // IClassFactory interface
public:
    HRESULT STDMETHODCALLTYPE CreateInstance(IUnknown* pUnkOuter, REFIID riid, LPVOID* ppvObject)
    {
        HRESULT hr = E_NOINTERFACE;

        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        if (riid == IID_IWTSPlugin) {
            IUnknown* wtsPlugin = NULL;
            IMsRdpExInstance* rdpInstance = (IMsRdpExInstance*)m_instance;
            rdpInstance->GetWTSPluginObject((void**)&wtsPlugin);

            if (wtsPlugin) {
                MsRdpEx_LogPrint(DEBUG, "CDvcPluginClassFactory using registered WTSPlugin");
                hr = wtsPlugin->QueryInterface(riid, ppvObject);
            }
            else {
                MsRdpEx_LogPrint(DEBUG, "CDvcPluginClassFactory using built-in WTSPlugin");
                CRdpDvcPlugin* dvcPlugin = new CRdpDvcPlugin();
                hr = dvcPlugin->QueryInterface(riid, ppvObject);
            }
        }

        MsRdpEx_LogPrint(DEBUG, "CDvcPluginClassFactory::CreateInstance(%s), hr = 0x%08X", iid, hr);

        return hr;
    }

    HRESULT STDMETHODCALLTYPE LockServer(BOOL fLock)
    {
        MsRdpEx_LogPrint(DEBUG, "CDvcPluginClassFactory::LockServer");
        return S_OK;
    }

private:
    ULONG m_refCount = 0;
    CMsRdpExInstance* m_instance = NULL;
};

HRESULT STDAPICALLTYPE DllGetClassObject_DvcPlugin(REFCLSID rclsid, REFIID riid, LPVOID* ppv, void* instance)
{
    HRESULT hr = E_NOINTERFACE;

    if (riid == (REFIID) IID_IClassFactory)
    {
        CDvcPluginClassFactory* classFactory = new CDvcPluginClassFactory((CMsRdpExInstance*) instance);
        *ppv = (LPVOID) classFactory;
        hr = S_OK;
    }

    return hr;
}
