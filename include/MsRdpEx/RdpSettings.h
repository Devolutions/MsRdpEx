#ifndef MSRDPEX_SETTINGS_H
#define MSRDPEX_SETTINGS_H

#include <MsRdpEx/MsRdpEx.h>

#include "TSObjects.h"

#include <comdef.h>

class CMsRdpExtendedSettings;
class CMsRdpPropertySet;

class CMsRdpExtendedSettings : public IMsRdpExtendedSettings
{
public:
    CMsRdpExtendedSettings(IUnknown* pUnknown, GUID* pSessionId);
    ~CMsRdpExtendedSettings();

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, LPVOID* ppvObject);
    ULONG STDMETHODCALLTYPE AddRef();
    ULONG STDMETHODCALLTYPE Release();

    // IMsRdpExtendedSettings
public:
    HRESULT __stdcall put_Property(BSTR bstrPropertyName, VARIANT* pValue);
    HRESULT __stdcall get_Property(BSTR bstrPropertyName, VARIANT* pValue);

    // additional stuff
public:
    HRESULT __stdcall put_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue);
    HRESULT __stdcall get_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue);
    HRESULT __stdcall put_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue);
    HRESULT __stdcall get_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue);
    HRESULT __stdcall SetTargetPassword(const char* password);
    HRESULT __stdcall SetGatewayPassword(const char* password);
    HRESULT __stdcall SetKdcProxyUrl(const char* kdcProxyUrl);
    HRESULT __stdcall AttachRdpClient(IMsTscAx* pMsTscAx);
    HRESULT __stdcall LoadRdpFile(const char* rdpFileName);
    HRESULT __stdcall GetCorePropsRawPtr(LPVOID* ppCorePropsRaw);
    HRESULT __stdcall GetSessionId(GUID* pSessionId);

private:
    GUID m_sessionId;
    ULONG m_refCount = 0;
    IUnknown* m_pUnknown = NULL;
    IMsTscAx* m_pMsTscAx = NULL;
    IMsRdpExtendedSettings* m_pMsRdpExtendedSettings = NULL;
    ITSPropertySet* m_pCorePropsRaw = NULL;
    CMsRdpPropertySet* m_CoreProps = NULL;
    CMsRdpPropertySet* m_BaseProps = NULL;
    CMsRdpPropertySet* m_TransportProps = NULL;
    char* m_KdcProxyUrl = NULL;
};

#ifdef __cplusplus
extern "C" {
#endif

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx, GUID* pSessionId);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_SETTINGS_H
