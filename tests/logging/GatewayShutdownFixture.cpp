// Use the production gateway implementation in the shutdown test DLL so the
// fixture can hold its private lock without adding any production test exports.
#include "../../dll/GatewayIsolation.cpp"

static DWORD WINAPI HoldGatewayLock(LPVOID ready)
{
    AcquireSRWLockExclusive(&g_BindingLock);
    SetEvent((HANDLE)ready);
    Sleep(INFINITE); // ExitProcess terminates this worker while it owns the lock.
    return 0;
}

extern "C" bool HoldGatewayBindingLock()
{
    if (!g_GatewayHooksAttached || GatewayRememberBinding((RPC_BINDING_HANDLE)1) != 1)
        return false;
    HANDLE ready = CreateEventW(NULL, TRUE, FALSE, NULL);
    if (!ready) return false;
    HANDLE worker = CreateThread(NULL, 0, HoldGatewayLock, ready, 0, NULL);
    bool held = worker && WaitForSingleObject(ready, 5000) == WAIT_OBJECT_0;
    if (worker) CloseHandle(worker);
    // On timeout the worker may still signal ready; process exit reclaims it.
    if (!worker || held) CloseHandle(ready);
    return held;
}
