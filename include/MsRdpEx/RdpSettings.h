#ifndef MSRDPEX_SETTINGS_H
#define MSRDPEX_SETTINGS_H

#include <MsRdpEx/MsRdpEx.h>

#include "TSObjects.h"

#include <comdef.h>

#define MOUSE_JIGGLER_METHOD_MOUSE_MOVE         0
#define MOUSE_JIGGLER_METHOD_SPECIAL_KEY        1

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
    HRESULT __stdcall SetRecordingPath(const char* recordingPath);
    HRESULT __stdcall SetRecordingSessionId(const char* sessionId);
    HRESULT __stdcall SetRecordingPipeName(const char* recordingPipeName);
    HRESULT __stdcall AttachRdpClient(IMsTscAx* pMsTscAx);
    HRESULT __stdcall ApplyRdpFile(void* rdpFilePtr);
    HRESULT __stdcall LoadRdpFile(const char* rdpFileName);
    HRESULT __stdcall LoadRdpFileFromNamedPipe(const char* pipeName);
    HRESULT __stdcall GetCorePropsRawPtr(LPVOID* ppCorePropsRaw);
    HRESULT __stdcall PrepareSspiSessionIdHack();
    HRESULT __stdcall DiscardCapturedPinIfNotCertLogon();
    HRESULT __stdcall PrepareMouseJiggler();
    HRESULT __stdcall PrepareVideoRecorder();
    HRESULT __stdcall PrepareExtraSystemMenu();
    char* __stdcall GetKdcProxyUrl();
    char* __stdcall GetKdcProxyName();
    bool GetKerbCertificateLogonEnabled();
    bool GetPasswordContainsSCardPin();
    WCHAR* GetCredentialUserName();
    WCHAR* GetCredentialPin();
    void SetCapturedPin(const WCHAR* pin);
    void ClearCapturedPin();
    bool GetMouseJigglerEnabled();
    uint32_t GetMouseJigglerInterval();
    uint32_t GetMouseJigglerMethod();
    bool GetKeyboardHookToggleShortcutEnabled();
    const char* GetKeyboardHookToggleShortcutKey();
    const char* GetSessionId();
    void GetSessionIdGuid(GUID* pSessionId);
    bool GetOutputMirrorEnabled();
    bool GetVideoRecordingEnabled();
    uint32_t GetVideoRecordingQuality();
    uint32_t GetVideoRecordingFrameRate();
    bool GetVideoRecordingCursor();
    char* GetRecordingPath();
    char* GetRecordingSessionId();
    char* GetRecordingPipeName();
    bool GetDumpBitmapUpdates();
    bool GetExtraSystemMenuEnabled();

private:
    GUID m_sessionId;
    char m_sessionIdStr[MSRDPEX_GUID_STRING_SIZE];
    ULONG m_refCount;
    IUnknown* m_pUnknown = NULL;
    IMsTscAx* m_pMsTscAx = NULL;
    IMsRdpClient7* m_pMsRdpClient7 = NULL;
    IMsRdpExtendedSettings* m_pMsRdpExtendedSettings = NULL;
    IMsRdpClientTransportSettings2* m_pMsRdpClientTransportSettings2 = NULL;
    ITSPropertySet* m_pCorePropsRaw = NULL;
    CMsRdpPropertySet* m_CoreProps = NULL;
    CMsRdpPropertySet* m_BaseProps = NULL;
    CMsRdpPropertySet* m_TransportProps = NULL;
    char* m_KdcProxyUrl = NULL;
    bool m_KerbCertificateLogonEnabled = false;
    bool m_PasswordContainsSCardPin = false;
    BYTE* m_ProtectedPin = NULL;
    DWORD m_ProtectedPinSize = 0;
    WCHAR* m_CapturedCertUserName = NULL;
    bool m_MouseJigglerEnabled = false;
    uint32_t m_MouseJigglerInterval = 60;
    uint32_t m_MouseJigglerMethod = 0;
    bool m_OutputMirrorEnabled = false;
    bool m_VideoRecordingEnabled = false;
    uint32_t m_VideoRecordingQuality = 5;
    uint32_t m_VideoRecordingFrameRate = 0;
    bool m_VideoRecordingCursor = false;
    char* m_RecordingPath = NULL;
    char* m_RecordingSessionId = NULL;
    char* m_RecordingPipeName = NULL;
    bool m_DumpBitmapUpdates = false;
    bool m_ExtraSystemMenuEnabled = true;
    bool m_KeyboardHookToggleShortcutEnabled = false;
    char m_KeyboardHookToggleShortcutKey[32];
    IUnknown* m_pWTSPlugin = NULL;
};

#ifdef __cplusplus
extern "C" {
#endif

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx, GUID* pSessionId);

char* MsRdpEx_KdcProxyUrlToName(const char* kdcProxyUrl);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_SETTINGS_H
