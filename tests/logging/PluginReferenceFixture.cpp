#include "../../dll/RdpInstance.cpp"

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
