#ifndef MSRDPEX_INSTANCE_H
#define MSRDPEX_INSTANCE_H

#include <MsRdpEx/MsRdpEx.h>
#include <MsRdpEx/RdpSettings.h>
#include <MsRdpEx/OutputMirror.h>

#include <comdef.h>

struct __declspec(novtable)
    IMsRdpExInstance : public IUnknown
{
public:
    virtual HRESULT __stdcall GetSessionId(GUID* pSessionId) = 0;
    virtual HRESULT __stdcall GetRdpClient(LPVOID* ppvObject) = 0;
    virtual HRESULT __stdcall GetOutputMirrorObject(LPVOID* ppvObject) = 0;
    virtual HRESULT __stdcall SetOutputMirrorObject(LPVOID pvObject) = 0;
    virtual HRESULT __stdcall GetOutputMirrorEnabled(bool* outputMirrorEnabled) = 0;
    virtual HRESULT __stdcall SetOutputMirrorEnabled(bool outputMirrorEnabled) = 0;
    virtual HRESULT __stdcall GetVideoRecordingEnabled(bool* videoRecordingEnabled) = 0;
    virtual HRESULT __stdcall SetVideoRecordingEnabled(bool videoRecordingEnabled) = 0;
    virtual HRESULT __stdcall GetDumpBitmapUpdates(bool* dumpBitmapUpdates) = 0;
    virtual HRESULT __stdcall SetDumpBitmapUpdates(bool dumpBitmapUpdates) = 0;
    virtual HRESULT __stdcall GetCorePropsRawPtr(LPVOID* ppCorePropsRaw) = 0;
    virtual HRESULT __stdcall SetCorePropsRawPtr(LPVOID pCorePropsRaw) = 0;
    virtual HRESULT __stdcall AttachInputWindow(HWND hOutputWnd, void* pUserData) = 0;
    virtual HRESULT __stdcall AttachOutputWindow(HWND hOutputWnd, void* pUserData) = 0;
    virtual HRESULT __stdcall AttachExtendedSettings(CMsRdpExtendedSettings* pExtendedSettings) = 0;
    virtual bool __stdcall GetExtendedSettings(CMsRdpExtendedSettings** ppExtendedSettings) = 0;
    virtual bool __stdcall GetShadowBitmap(HDC* phDC, HBITMAP* phBitmap, uint8_t** pBitmapData,
        uint32_t* pBitmapWidth, uint32_t* pBitmapHeight, uint32_t* pBitmapStep) = 0;
    virtual void __stdcall LockShadowBitmap() = 0;
    virtual void __stdcall UnlockShadowBitmap() = 0;
    virtual void __stdcall GetLastMousePosition(int32_t* posX, int32_t* posY) = 0;
    virtual void __stdcall SetLastMousePosition(int32_t posX, int32_t posY) = 0;
};

class CMsRdpExInstance;
class CMsRdpClient;

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_InstanceManager MsRdpEx_InstanceManager;

bool MsRdpEx_InstanceManager_Add(CMsRdpExInstance* instance);
bool MsRdpEx_InstanceManager_Remove(CMsRdpExInstance* instance);

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(HWND hWnd);

CMsRdpExInstance* MsRdpEx_InstanceManager_AttachOutputWindow(HWND hOutputWnd, void* pUserData);

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByInputCaptureHwnd(HWND hWnd);

CMsRdpExInstance* MsRdpEx_InstanceManager_AttachInputWindow(HWND hInputWnd, void* pUserData);

CMsRdpExInstance* MsRdpEx_InstanceManager_FindBySessionId(GUID* sessionId);

CMsRdpExtendedSettings* MsRdpEx_FindExtendedSettingsBySessionId(GUID* sessionId);

MsRdpEx_InstanceManager* MsRdpEx_InstanceManager_Get();
void MsRdpEx_InstanceManager_Release();

CMsRdpExInstance* CMsRdpExInstance_New(CMsRdpClient* pMsRdpClient);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_INSTANCE_H
