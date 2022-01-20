
#include <MsRdpEx/RdpProcess.h>
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/RdpCoreApi.h>

extern "C" const GUID IID_IMsRdpExCoreApi;
extern "C" const GUID IID_IMsRdpExInstance;
extern "C" const GUID IID_IMsRdpExProcess;

class CMsRdpExCoreApi : public IMsRdpExCoreApi
{
public:
    CMsRdpExCoreApi()
    {
        m_refCount = 0;
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
        MsRdpEx_Log("CMsRdpExCoreApi::QueryInterface");

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        if (riid == IID_IMsRdpExCoreApi)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }

        MsRdpEx_Log("--> hr=%x", hr);
        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CMsRdpExCoreApi::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_Log("CMsRdpExCoreApi::Release");
        if (--m_refCount == 0)
        {
            MsRdpEx_Log("--> deleting object");
            delete this;
            return 0;
        }
        MsRdpEx_Log("--> refCount=%d", m_refCount);
        return m_refCount;
    }

    // IMsRdpExCoreApi
public:
    HRESULT __stdcall Load()
    {
        MsRdpEx_Load();
        MsRdpEx_Log("CMsRdpExCoreApi::Load");
        return S_OK;
    }

    HRESULT __stdcall Unload()
    {
        MsRdpEx_Log("CMsRdpExCoreApi::Unload");
        MsRdpEx_Unload();
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

    void __stdcall SetLogFilePath(const char* logFilePath)
    {
        MsRdpEx_SetLogFilePath(logFilePath);
    }

    bool __stdcall QueryInstanceByWindowHandle(HWND hWnd, LPVOID* ppvObject)
    {
        HRESULT hr;
        IMsRdpExInstance* instance = NULL;

        instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(hWnd);

        if (!instance)
            return false;

        hr = instance->QueryInterface(IID_IMsRdpExInstance, ppvObject);

        return (hr == S_OK) ? true : false;
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

    MsRdpEx_Log("MsRdpEx_CreateInstance(%s)", iid);

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
