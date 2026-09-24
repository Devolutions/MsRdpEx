#include "../../dll/RdpInstance.cpp"

ATOM WINAPI Hook_RegisterClassExW(WNDCLASSEXW* wndClass);

extern "C" IMsRdpExInstance* CreatePluginReferenceInstance()
{
    return CMsRdpExInstance_New(NULL);
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
