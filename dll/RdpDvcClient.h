#ifndef MSRDPEX_DVC_CLIENT_H
#define MSRDPEX_DVC_CLIENT_H

#include "MsRdpEx.h"

#include <comdef.h>
#include <atlbase.h>
#include <oleidl.h>
#include <commctrl.h>

#include <tsvirtualchannels.h>

class CRdpDvcClient :
    public IWTSVirtualChannelCallback
{
public:
    // IUnknown methods
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppv) override;
    ULONG STDMETHODCALLTYPE AddRef() override;
    ULONG STDMETHODCALLTYPE Release() override;

    // IWTSVirtualChannelCallback methods
    HRESULT STDMETHODCALLTYPE OnDataReceived(ULONG cbSize, BYTE* pBuffer) override;
    HRESULT STDMETHODCALLTYPE OnClose(void) override;

    // Additional methods
    void SetChannel(IWTSVirtualChannel* pChannel);
    void SetListener(IWTSListener* pListener);

    CRdpDvcClient(void);
    virtual ~CRdpDvcClient();

private:
    ULONG m_refCount = 0;
    IWTSVirtualChannel* m_pChannel = NULL;
    IWTSListener* m_pListener = NULL;
};

class CRdpDvcListener :
    public IWTSListenerCallback
{
public:
    // IUnknown methods
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppv) override;
    ULONG STDMETHODCALLTYPE AddRef() override;
    ULONG STDMETHODCALLTYPE Release() override;

    // IWTSListenerCallback methods
    HRESULT STDMETHODCALLTYPE OnNewChannelConnection(IWTSVirtualChannel* pChannel,
        BSTR data, BOOL* pbAccept, IWTSVirtualChannelCallback** ppCallback) override;

    // Additional methods
    CRdpDvcListener(void);
    virtual ~CRdpDvcListener();
private:
    ULONG m_refCount = 0;
};

class CRdpDvcPlugin :
    public IWTSPlugin
{
public:
    // IUnknown methods
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppv) override;
    ULONG STDMETHODCALLTYPE AddRef() override;
    ULONG STDMETHODCALLTYPE Release() override;

    // IWTSPlugin methods
    HRESULT STDMETHODCALLTYPE Initialize(IWTSVirtualChannelManager* pChannelMgr) override;
    HRESULT STDMETHODCALLTYPE Connected() override;
    HRESULT STDMETHODCALLTYPE Disconnected(DWORD dwDisconnectCode) override;
    HRESULT STDMETHODCALLTYPE Terminated() override;

    // Additional methods
    CRdpDvcPlugin(void);
    virtual ~CRdpDvcPlugin();

private:
    ULONG m_refCount = 0;
    IWTSVirtualChannel* m_pChannel = NULL;
};

extern "C" const GUID CLSID_IMsRdpExDVCPlugin;

HRESULT STDAPICALLTYPE DllGetClassObject_DvcPlugin(REFCLSID rclsid, REFIID riid, LPVOID* ppv, void* instance);

#endif /* MSRDPEX_DVC_CLIENT_H */
