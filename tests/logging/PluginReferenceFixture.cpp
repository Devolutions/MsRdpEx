#include "../../dll/RdpInstance.cpp"

extern "C" IMsRdpExInstance* CreatePluginReferenceInstance()
{
    return CMsRdpExInstance_New(NULL);
}
