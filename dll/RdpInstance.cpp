
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/Memory.h>
#include <MsRdpEx/ArrayList.h>
#include <MsRdpEx/Environment.h>

#include "TSObjects.h"
#include "ComHelpers.h"

extern "C" const GUID IID_IMsRdpExInstance;

class CMsRdpExInstance : public IMsRdpExInstance
{
public:
    CMsRdpExInstance(CMsRdpClient* pMsRdpClient)
    {
        m_refCount = 1;
        m_pMsRdpClient = pMsRdpClient;
        MsRdpEx_GuidGenerate(&m_sessionId);

        char sessionId[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&m_sessionId, sessionId, 0);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExInstance SessionId: %s", sessionId);
    }

    ~CMsRdpExInstance()
    {
        if (m_OutputMirror) {
            MsRdpEx_OutputMirror_Free(m_OutputMirror);
            m_OutputMirror = NULL;
        }

        if (m_pMsRdpExtendedSettings) {
            m_pMsRdpExtendedSettings->Release();
        }
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

        if (!ppvObject) {
            return E_INVALIDARG;
        }

        *ppvObject = NULL;  // Initialize to NULL for safety

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if (riid == IID_IMsRdpExInstance)
        {
            *ppvObject = (LPVOID)((IMsRdpExInstance*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExInstance::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpExInstance::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpExInstance::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IMsRdpExInstance
public:
    HRESULT STDMETHODCALLTYPE GetSessionId(GUID* pSessionId)
    {
        MsRdpEx_GuidCopy(pSessionId, &m_sessionId);
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetRdpClient(LPVOID* ppvObject)
    {
        IUnknown* pMsRdpClient = (IUnknown*)m_pMsRdpClient;
        *ppvObject = (void*)m_pMsRdpClient;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetOutputMirrorObject(LPVOID* ppvObject)
    {
        *ppvObject = m_OutputMirror;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetOutputMirrorObject(LPVOID pvObject)
    {
        m_OutputMirror = (MsRdpEx_OutputMirror*) pvObject;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetOutputMirrorEnabled(bool* outputMirrorEnabled)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        *outputMirrorEnabled = m_pMsRdpExtendedSettings->GetOutputMirrorEnabled();
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetOutputMirrorEnabled(bool outputMirrorEnabled)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        VARIANT propValue;
        VariantInitBool(&propValue, outputMirrorEnabled);
        bstr_t propName = _com_util::ConvertStringToBSTR("OutputMirrorEnabled");
        m_pMsRdpExtendedSettings->put_Property(propName, &propValue);

        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetVideoRecordingEnabled(bool* videoRecordingEnabled)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        *videoRecordingEnabled = m_pMsRdpExtendedSettings->GetVideoRecordingEnabled();
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetVideoRecordingEnabled(bool videoRecordingEnabled)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        VARIANT propValue;
        VariantInitBool(&propValue, videoRecordingEnabled);
        bstr_t propName = _com_util::ConvertStringToBSTR("VideoRecordingEnabled");
        m_pMsRdpExtendedSettings->put_Property(propName, &propValue);

        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetDumpBitmapUpdates(bool* dumpBitmapUpdates)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        *dumpBitmapUpdates = m_pMsRdpExtendedSettings->GetDumpBitmapUpdates();
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetDumpBitmapUpdates(bool dumpBitmapUpdates)
    {
        if (!m_pMsRdpExtendedSettings)
            return E_UNEXPECTED;

        VARIANT propValue;
        VariantInitBool(&propValue, dumpBitmapUpdates);
        bstr_t propName = _com_util::ConvertStringToBSTR("DumpBitmapUpdates");
        m_pMsRdpExtendedSettings->put_Property(propName, &propValue);

        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetCorePropsRawPtr(LPVOID* ppCorePropsRaw)
    {
        *ppCorePropsRaw = m_pCorePropsRaw;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetCorePropsRawPtr(LPVOID pCorePropsRaw)
    {
        m_pCorePropsRaw = (ITSPropertySet*) pCorePropsRaw;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE AttachInputWindow(HWND hInputWnd, void* pUserData)
    {
        m_hInputCaptureWnd = hInputWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetInputWindow(HWND* phInputCaptureWnd)
    {
        *phInputCaptureWnd = m_hInputCaptureWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE AttachOutputWindow(HWND hOutputWnd, void* pUserData)
    {
        m_hOutputPresenterWnd = hOutputWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetOutputWindow(HWND* phOutputPresenterWnd)
    {
        *phOutputPresenterWnd = m_hOutputPresenterWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE AttachTscShellContainerWindow(HWND hTscShellContainerWnd)
    {
        m_hTscShellContainerWnd = hTscShellContainerWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE GetTscShellContainerWindow(HWND* phTscShellContainerWnd)
    {
        *phTscShellContainerWnd = m_hTscShellContainerWnd;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE AttachExtendedSettings(CMsRdpExtendedSettings* pExtendedSettings)
    {
        m_pMsRdpExtendedSettings = pExtendedSettings;
        m_pMsRdpExtendedSettings->AddRef();
        return S_OK;
    }

    bool STDMETHODCALLTYPE GetExtendedSettings(CMsRdpExtendedSettings** ppExtendedSettings)
    {
        *ppExtendedSettings = m_pMsRdpExtendedSettings;
        return m_pMsRdpExtendedSettings ? true : false;
    }

    bool STDMETHODCALLTYPE GetShadowBitmap(HDC* phDC, HBITMAP* phBitmap, uint8_t** pBitmapData,
        uint32_t* pBitmapWidth, uint32_t* pBitmapHeight, uint32_t* pBitmapStep)
    {
        MsRdpEx_OutputMirror* outputMirror = m_OutputMirror;

        if (!outputMirror)
            return false;

        return MsRdpEx_OutputMirror_GetShadowBitmap(outputMirror,
            phDC, phBitmap, pBitmapData, pBitmapWidth, pBitmapHeight, pBitmapStep);
    }

    void STDMETHODCALLTYPE LockShadowBitmap()
    {
        MsRdpEx_OutputMirror* outputMirror = m_OutputMirror;

        if (!outputMirror)
            return;

        MsRdpEx_OutputMirror_Lock(outputMirror);
    }

    void STDMETHODCALLTYPE UnlockShadowBitmap()
    {
        MsRdpEx_OutputMirror* outputMirror = m_OutputMirror;

        if (!outputMirror)
            return;

        MsRdpEx_OutputMirror_Unlock(outputMirror);
    }

    void STDMETHODCALLTYPE GetLastMousePosition(int32_t* posX, int32_t* posY)
    {
        *posX = m_LastMousePosX;
        *posY = m_LastMousePosY;
    }

    void STDMETHODCALLTYPE SetLastMousePosition(int32_t posX, int32_t posY)
    {
        m_LastMousePosX = posX;
        m_LastMousePosY = posY;
    }

    HRESULT STDMETHODCALLTYPE GetWTSPluginObject(LPVOID* ppvObject)
    {
        *ppvObject = m_WTSPlugin;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetWTSPluginObject(LPVOID pvObject)
    {
        m_WTSPlugin = (IUnknown*)pvObject;
        return S_OK;
    }

public:
    GUID m_sessionId;
    ULONG m_refCount;
    CMsRdpClient* m_pMsRdpClient = NULL;
    HWND m_hInputCaptureWnd = NULL;
    HWND m_hOutputPresenterWnd = NULL;
    HWND m_hTscShellContainerWnd = NULL;
    MsRdpEx_OutputMirror* m_OutputMirror = NULL;
    ITSPropertySet* m_pCorePropsRaw = NULL;
    CMsRdpExtendedSettings* m_pMsRdpExtendedSettings = NULL;
    int32_t m_LastMousePosX = 0;
    int32_t m_LastMousePosY = 0;
    IUnknown* m_WTSPlugin = NULL;
};

CMsRdpExInstance* CMsRdpExInstance_New(CMsRdpClient* pMsRdpClient)
{
    CMsRdpExInstance* instance = new CMsRdpExInstance(pMsRdpClient);
    return instance;
}

void MsRdpEx_RdpInstance_Free(CMsRdpExInstance* instance)
{
    instance->Release();
}

typedef struct _MsRdpEx_InstanceManager MsRdpEx_InstanceManager;

struct _MsRdpEx_InstanceManager
{
    MsRdpEx_ArrayList* instances;
};

void MsRdpEx_InstanceManager_Free(MsRdpEx_InstanceManager* ctx);

static int g_RefCount = 0;
static MsRdpEx_InstanceManager* g_InstanceManager = NULL;

bool MsRdpEx_InstanceManager_Add(CMsRdpExInstance* instance)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return false;

    instance->AddRef();
    MsRdpEx_ArrayList_Add(ctx->instances, instance);

    return true;
}

bool MsRdpEx_InstanceManager_Remove(CMsRdpExInstance* instance)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx || !instance)
        return false;

    MsRdpEx_ArrayList_Remove(ctx->instances, instance, false);
    instance->Release();

    return true;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByOutputPresenterHwnd(HWND hWnd)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*)MsRdpEx_ArrayListIt_Next(it);

        found = (obj->m_hOutputPresenterWnd == hWnd) ? true : false;

        if (found)
            break;
    }

    MsRdpEx_ArrayListIt_Finish(it);

    return found ? obj : NULL;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_AttachOutputWindow(HWND hOutputWnd, void* pUserData)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    size_t maxPtrCount = 200;
    ITSPropertySet* pTSCoreProps = NULL;

    for (int i = 0; i < maxPtrCount; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)pUserData)[i];
        if (MsRdpEx_CanReadUnsafePtr(ppTSObject, 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (MsRdpEx_CanReadUnsafePtr(pTSObject, sizeof(ITSObjectBase))) {
                if (pTSObject->marker == TSOBJECT_MARKER) {
                    MsRdpEx_LogPrint(DEBUG, "COPWnd(%d): 0x%08X name: %s refCount: %d",
                        i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                    if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSPropertySet")) {
                        ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                        if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                            pTSCoreProps = pTSProps;
                            break;
                        }
                    }
                }
            }
        }
    }

    void* pCorePropsRaw1 = (void*) pTSCoreProps;
    void* pCorePropsRaw2 = NULL;

    if (!pCorePropsRaw1)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*) MsRdpEx_ArrayListIt_Next(it);

        obj->GetCorePropsRawPtr(&pCorePropsRaw2);

        if (pCorePropsRaw1 == pCorePropsRaw2)
        {
            found = true;
            break;
        }
    }

    MsRdpEx_ArrayListIt_Finish(it);

    if (found) {
        obj->AttachOutputWindow(hOutputWnd, pUserData);
    }

    return found ? obj : NULL;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByInputCaptureHwnd(HWND hWnd)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*)MsRdpEx_ArrayListIt_Next(it);

        found = (obj->m_hInputCaptureWnd == hWnd) ? true : false;

        if (found)
            break;
    }

    MsRdpEx_ArrayListIt_Finish(it);

    return found ? obj : NULL;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_AttachInputWindow(HWND hInputWnd, void* pUserData)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    size_t maxPtrCount = 200;
    ITSPropertySet* pTSCoreProps = NULL;

    for (int i = 0; i < maxPtrCount; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)pUserData)[i];
        if (MsRdpEx_CanReadUnsafePtr(ppTSObject, 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (MsRdpEx_CanReadUnsafePtr(pTSObject, sizeof(ITSObjectBase))) {
                if (pTSObject->marker == TSOBJECT_MARKER) {
                    MsRdpEx_LogPrint(DEBUG, "CIHWnd(%d): 0x%08X name: %s refCount: %d",
                        i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                    if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSPropertySet")) {
                        ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                        if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                            pTSCoreProps = pTSProps;
                            break;
                        }
                    }
                }
            }
        }
    }

    void* pCorePropsRaw1 = (void*)pTSCoreProps;
    void* pCorePropsRaw2 = NULL;

    if (!pCorePropsRaw1)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*)MsRdpEx_ArrayListIt_Next(it);

        obj->GetCorePropsRawPtr(&pCorePropsRaw2);

        if (pCorePropsRaw1 == pCorePropsRaw2)
        {
            found = true;
            break;
        }
    }

    MsRdpEx_ArrayListIt_Finish(it);

    if (found) {
        obj->AttachInputWindow(hInputWnd, pUserData);
    }

    return found ? obj : NULL;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_FindByTscShellContainerWnd(HWND hWnd)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*)MsRdpEx_ArrayListIt_Next(it);

        found = (obj->m_hTscShellContainerWnd == hWnd) ? true : false;

        if (found)
            break;
    }

    MsRdpEx_ArrayListIt_Finish(it);

    return found ? obj : NULL;
}

CMsRdpExInstance* MsRdpEx_InstanceManager_FindBySessionId(GUID* sessionId)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return NULL;

    bool found = false;
    CMsRdpExInstance* obj = NULL;
    MsRdpEx_ArrayListIt* it = NULL;

    it = MsRdpEx_ArrayList_It(ctx->instances, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        obj = (CMsRdpExInstance*)MsRdpEx_ArrayListIt_Next(it);

        found = MsRdpEx_GuidIsEqual(&obj->m_sessionId, sessionId);

        if (found)
            break;
    }

    MsRdpEx_ArrayListIt_Finish(it);

    return found ? obj : NULL;
}

CMsRdpExtendedSettings* MsRdpEx_FindExtendedSettingsBySessionId(GUID* sessionId)
{
    CMsRdpExInstance* instance = NULL;
    CMsRdpExtendedSettings* settings = NULL;
    
    instance = MsRdpEx_InstanceManager_FindBySessionId(sessionId);

    if (!instance)
        return NULL;

    settings = instance->m_pMsRdpExtendedSettings;

    return settings;
}

MsRdpEx_InstanceManager* MsRdpEx_InstanceManager_New()
{
    MsRdpEx_InstanceManager* ctx;

    ctx = (MsRdpEx_InstanceManager*)calloc(1, sizeof(MsRdpEx_InstanceManager));

    if (!ctx)
        return NULL;

    ctx->instances = MsRdpEx_ArrayList_New(true);

    if (!ctx->instances)
        goto error;

    MsRdpEx_ArrayList_Object(ctx->instances)->fnObjectFree =
        (MSRDPEX_OBJECT_FREE_FN) MsRdpEx_RdpInstance_Free;

    return ctx;
error:
    MsRdpEx_InstanceManager_Free(ctx);
    return NULL;
}

void MsRdpEx_InstanceManager_Free(MsRdpEx_InstanceManager* ctx)
{
    if (!ctx)
        return;

    if (ctx->instances) {
        MsRdpEx_ArrayList_Free(ctx->instances);
        ctx->instances = NULL;
    }
}

MsRdpEx_InstanceManager* MsRdpEx_InstanceManager_Get()
{
    if (!g_InstanceManager)
        g_InstanceManager = MsRdpEx_InstanceManager_New();

    g_RefCount++;

    return g_InstanceManager;
}

void MsRdpEx_InstanceManager_Release()
{
    g_RefCount--;

    if (g_RefCount < 0)
        g_RefCount = 0;

    if (g_InstanceManager && (g_RefCount < 1))
    {
        MsRdpEx_InstanceManager_Free(g_InstanceManager);
        g_InstanceManager = NULL;
    }
}
