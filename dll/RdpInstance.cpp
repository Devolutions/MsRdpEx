
#include <MsRdpEx/RdpInstance.h>

#include <MsRdpEx/ArrayList.h>

#include "TSObjects.h"

extern "C" const GUID IID_IMsRdpExInstance;

class CMsRdpExInstance : public IMsRdpExInstance
{
public:
    CMsRdpExInstance(CMsRdpClient* pMsRdpClient)
    {
        m_refCount = 0;
        m_pMsRdpClient = pMsRdpClient;
    }

    ~CMsRdpExInstance()
    {
        if (m_OutputMirror) {
            MsRdpEx_OutputMirror_Free(m_OutputMirror);
            m_OutputMirror = NULL;
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
        MsRdpEx_Log("CMsRdpExInstance::QueryInterface");

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }
        else if (riid == IID_IMsRdpExInstance)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            m_refCount++;
            return S_OK;
        }

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        MsRdpEx_Log("CMsRdpExInstance::AddRef");
        return ++m_refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        MsRdpEx_Log("CMsRdpExInstance::Release");
        if (--m_refCount == 0)
        {
            MsRdpEx_Log("--> deleting object");
            delete this;
            return 0;
        }
        MsRdpEx_Log("--> refCount=%d", m_refCount);
        return m_refCount;
    }

    // IMsRdpExInstance
public:
    HRESULT STDMETHODCALLTYPE GetRdpClient(LPVOID* ppvObject)
    {
        IUnknown* pMsRdpClient = (IUnknown*)m_pMsRdpClient;
        return pMsRdpClient->QueryInterface(IID_IUnknown, ppvObject);
    }

    HRESULT STDMETHODCALLTYPE GetOutputMirror(LPVOID* ppvObject)
    {
        *ppvObject = m_OutputMirror;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE SetOutputMirror(LPVOID pvObject)
    {
        m_OutputMirror = (MsRdpEx_OutputMirror*) pvObject;
        return S_OK;
    }

    HRESULT STDMETHODCALLTYPE AttachOutputWindow(HWND hOutputWnd, void* pUserData)
    {
        m_hOutputPresenterWnd = hOutputWnd;

        size_t memStatus;
        MEMORY_BASIC_INFORMATION memInfo;

        size_t maxPtrCount = 200;
        ITSPropertySet* pTSCoreProps = NULL;
        ITSPropertySet* pTSBaseProps = NULL;

        for (int i = 0; i < maxPtrCount; i++) {
            ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)pUserData)[i];
            memStatus = VirtualQuery(ppTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
            if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize >= 8)) {
                ITSObjectBase* pTSObject = *ppTSObject;
                if (pTSObject) {
                    memStatus = VirtualQuery(pTSObject, &memInfo, sizeof(MEMORY_BASIC_INFORMATION));
                    if ((memStatus != 0) && (memInfo.State == MEM_COMMIT) && (memInfo.RegionSize > 16)) {
                        if (pTSObject->marker == TSOBJECT_MARKER) {
                            MsRdpEx_Log("COPWnd(%d): 0x%08X name: %s refCount: %d",
                                i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                            if (!strcmp(pTSObject->name, "CTSPropertySet")) {
                                ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                                if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                                    pTSCoreProps = pTSProps;
                                }
                                else if (!pTSBaseProps && TsPropertyMap_IsBaseProps(pTSProps)) {
                                    pTSBaseProps = pTSProps;
                                }
                            }
                        }
                    }
                }
            }
        }

        MsRdpEx_Log("pTSCoreProps2: %p", pTSCoreProps);

        return S_OK;
    }

public:
    ULONG m_refCount = NULL;
    CMsRdpClient* m_pMsRdpClient = NULL;
    HWND m_hOutputPresenterWnd = NULL;
    MsRdpEx_OutputMirror* m_OutputMirror = NULL;
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

    MsRdpEx_ArrayList_Add(ctx->instances, instance);

    return true;
}

bool MsRdpEx_InstanceManager_Remove(CMsRdpExInstance* instance, bool free)
{
    MsRdpEx_InstanceManager* ctx = g_InstanceManager;

    if (!ctx)
        return false;

    MsRdpEx_ArrayList_Remove(ctx->instances, instance, free);

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
