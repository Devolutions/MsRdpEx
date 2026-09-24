#include "../../dll/RdpInstance.cpp"

#include <cstdlib>

static thread_local bool g_FailNextPluginHolderAllocation = false;

void* __cdecl operator new(size_t size, const std::nothrow_t&) noexcept
{
    if (g_FailNextPluginHolderAllocation && size == sizeof(MsRdpEx_WTSPluginReference)) {
        g_FailNextPluginHolderAllocation = false;
        return nullptr;
    }
    return std::malloc(size ? size : 1);
}

extern "C" void FailNextPluginHolderAllocation()
{
    g_FailNextPluginHolderAllocation = true;
}

extern "C" IMsRdpExInstance* CreatePluginReferenceInstance()
{
    return CMsRdpExInstance_New(NULL);
}

extern "C" bool RegisterPluginReferenceInstance(IMsRdpExInstance* instance)
{
    if (!MsRdpEx_InstanceManager_Get())
        return false;
    if (MsRdpEx_InstanceManager_Add((CMsRdpExInstance*)instance))
        return true;
    MsRdpEx_InstanceManager_Release();
    return false;
}

extern "C" bool UnregisterPluginReferenceInstance(IMsRdpExInstance* instance)
{
    bool removed = MsRdpEx_InstanceManager_Remove((CMsRdpExInstance*)instance);
    MsRdpEx_InstanceManager_Release();
    return removed;
}

extern "C" HRESULT CreatePluginReferenceFactory(REFCLSID sessionId, IClassFactory** factory)
{
    return MsRdpEx_DllGetClassObject(sessionId, IID_IClassFactory, (void**)factory);
}
