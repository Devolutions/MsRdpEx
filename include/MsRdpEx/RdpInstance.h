#ifndef MSRDPEX_INSTANCE_H
#define MSRDPEX_INSTANCE_H

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/OutputMirror.h>

#include <comdef.h>

struct __declspec(novtable)
    IMsRdpExInstance : public IUnknown
{
public:
    virtual HRESULT __stdcall GetRdpClient(LPVOID* ppvObject) = 0;
    virtual HRESULT __stdcall GetOutputMirror(LPVOID* ppvObject) = 0;
    virtual HRESULT __stdcall SetOutputMirror(LPVOID pvObject) = 0;
    virtual HRESULT __stdcall GetCorePropsRawPtr(LPVOID* ppCorePropsRaw) = 0;
    virtual HRESULT __stdcall SetCorePropsRawPtr(LPVOID pCorePropsRaw) = 0;
    virtual HRESULT __stdcall AttachOutputWindow(HWND hOutputWnd, void* pUserData) = 0;
};

class CMsRdpExInstance;
class CMsRdpClient;

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_InstanceManager MsRdpEx_InstanceManager;

bool MsRdpEx_InstanceManager_Add(CMsRdpExInstance* instance);
bool MsRdpEx_InstanceManager_Remove(CMsRdpExInstance* instance, bool free);

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(HWND hWnd);

CMsRdpExInstance* MsRdpEx_InstanceManager_AttachOutputWindow(HWND hOutputWnd, void* pUserData);

MsRdpEx_InstanceManager* MsRdpEx_InstanceManager_Get();
void MsRdpEx_InstanceManager_Release();

CMsRdpExInstance* CMsRdpExInstance_New(CMsRdpClient* pMsRdpClient);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_INSTANCE_H
