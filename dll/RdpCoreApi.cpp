
#include <MsRdpEx/RdpProcess.h>
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/RdpCoreApi.h>

#include "MsRdpEx.h"

extern "C" const GUID IID_IMsRdpExCoreApi;
extern "C" const GUID IID_IMsRdpExInstance;
extern "C" const GUID IID_IMsRdpExProcess;

class CMsRdpExCoreApi : public IMsRdpExCoreApi
{
public:
    CMsRdpExCoreApi()
    {
        m_refCount = 1;
        MsRdpEx_PathCchDetect(m_MsRdpExDllPath, MSRDPEX_MAX_PATH, MSRDPEX_CURRENT_LIBRARY_PATH);
    }

    ~CMsRdpExCoreApi()
    {

    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr = E_NOINTERFACE;
        ULONG refCount = m_refCount;
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if (riid == IID_IMsRdpExCoreApi)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExCoreApi::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExCoreApi::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExCoreApi::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IMsRdpExCoreApi
public:
    HRESULT __stdcall Load()
    {
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExCoreApi::Load");
        return S_OK;
    }

    HRESULT __stdcall Unload()
    {
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExCoreApi::Unload");
        return S_OK;
    }

    const char* __stdcall GetMsRdpExDllPath()
    {
        return (const char*) m_MsRdpExDllPath;
    }

    void __stdcall SetLogEnabled(bool logEnabled)
    {
        MsRdpEx_SetLogEnabled(logEnabled);
    }

    void __stdcall SetLogLevel(uint32_t logLevel)
    {
        MsRdpEx_SetLogLevel(logLevel);
    }

    void __stdcall SetLogFilePath(const char* logFilePath)
    {
        MsRdpEx_SetLogFilePath(logFilePath);
    }

    void __stdcall SetPcapEnabled(bool pcapEnabled)
    {
        MsRdpEx_SetPcapEnabled(pcapEnabled);
    }

    void __stdcall SetPcapFilePath(const char* pcapFilePath)
    {
        MsRdpEx_SetPcapFilePath(pcapFilePath);
    }

    void __stdcall SetAxHookEnabled(bool axHookEnabled)
    {
        MsRdpEx_SetAxHookEnabled(axHookEnabled);
    }

    bool __stdcall QueryInstanceByWindowHandle(HWND hWnd, LPVOID* ppvObject)
    {
        IMsRdpExInstance* instance = NULL;

        instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

        if (!instance)
            return false;

        instance->AddRef();
        *ppvObject = (LPVOID)instance;

        return true;
    }

    bool __stdcall OpenInstanceForWindowHandle(HWND hWnd, LPVOID* ppvObject)
    {
        IMsRdpExInstance* instance = NULL;

        instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

        if (!instance)
        {
            instance = (IMsRdpExInstance*) CMsRdpExInstance_New(NULL);
            instance->AttachOutputWindow(hWnd, NULL);
            MsRdpEx_InstanceManager_Add((CMsRdpExInstance*) instance);
        }
        else
        {
            instance->AddRef();
        }

        *ppvObject = (LPVOID)instance;

        return true;
    }

private:
    ULONG m_refCount;
    char m_MsRdpExDllPath[MSRDPEX_MAX_PATH] = { 0 };
};

HRESULT CDECL MsRdpExCoreApi_CreateInstance(LPVOID* ppvObject)
{
    CMsRdpExCoreApi* pObj = new CMsRdpExCoreApi();
    *ppvObject = (LPVOID) pObj;
    return S_OK;
}

HRESULT CDECL MsRdpEx_CreateInstance(REFCLSID riid, LPVOID* ppvObject)
{
    IUnknown* pUnknown = NULL;
    HRESULT hr = E_NOINTERFACE;

    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    MsRdpEx_LogPrint(DEBUG, "MsRdpEx_CreateInstance(%s)", iid);

    if (riid == IID_IMsRdpExCoreApi) {
        hr = MsRdpExCoreApi_CreateInstance((LPVOID*) &pUnknown);
    }
    else if (riid == IID_IMsRdpExProcess) {
        hr = MsRdpExProcess_CreateInstance((LPVOID*) &pUnknown);
    }

    if (hr == S_OK) {
        hr = pUnknown->QueryInterface(riid, ppvObject);
    }

    return hr;
}
