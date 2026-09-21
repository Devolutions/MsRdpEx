// Wrap the production entry point only in the test DLL. Process exit can report
// success even if an abandoned SRW lock prevents DllMain from completing.
#define DllMain GatewayProductionDllMain
#include "../../dll/MsRdpEx.cpp"
#undef DllMain

static HANDLE g_ShutdownCompleted = NULL;
extern "C" bool HoldGatewayBindingLock();

extern "C" bool PrepareGatewayShutdown(const wchar_t* eventName, bool holdLock)
{
    g_ShutdownCompleted = OpenEventW(EVENT_MODIFY_STATE, FALSE, eventName);
    return g_ShutdownCompleted && (!holdLock || HoldGatewayBindingLock());
}

BOOL WINAPI DllMain(HMODULE module, DWORD reason, LPVOID reserved)
{
    BOOL result = GatewayProductionDllMain(module, reason, reserved);
    if (reason == DLL_PROCESS_DETACH && g_ShutdownCompleted) {
        SetEvent(g_ShutdownCompleted);
        CloseHandle(g_ShutdownCompleted);
        g_ShutdownCompleted = NULL;
    }
    return result;
}
