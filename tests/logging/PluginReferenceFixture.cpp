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

ATOM WINAPI Hook_RegisterClassExW(WNDCLASSEXW* wndClass);

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

extern "C" bool TryRegisterDetachedInstance(IMsRdpExInstance* instance)
{
    return MsRdpEx_InstanceManager_Add((CMsRdpExInstance*)instance);
}

extern "C" bool TryRemoveDetachedInstance(IMsRdpExInstance* instance)
{
    return MsRdpEx_InstanceManager_Remove((CMsRdpExInstance*)instance);
}

extern "C" ATOM RegisterDetachedOutputWindowClass()
{
    if (!MsRdpEx_InstanceManager_Get())
        return 0;

    WNDCLASSEXW windowClass = { sizeof(windowClass) };
    windowClass.lpfnWndProc = DefWindowProcW;
    windowClass.hInstance = GetModuleHandleW(NULL);
    windowClass.lpszClassName = L"OPWindowClass";
    ATOM atom = Hook_RegisterClassExW(&windowClass);
    if (!atom)
        MsRdpEx_InstanceManager_Release();
    return atom;
}

extern "C" IMsRdpExInstance* AcquireDetachedOutputInstance(HWND hWnd)
{
    return MsRdpEx_InstanceManager_AcquireByOutputPresenterHwnd(hWnd);
}

extern "C" void ReleaseDetachedInstanceManager()
{
    MsRdpEx_InstanceManager_Release();
}
