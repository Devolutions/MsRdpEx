#include "GatewayIsolation.h"
#include "MsRdpEx.h"
#include <MsRdpEx/Environment.h>
#include <MsRdpEx/Detours.h>
#include <rpc.h>
#include <intrin.h>

static INIT_ONCE g_GatewaySettingsInit = INIT_ONCE_STATIC_INIT;
static volatile LONG g_GatewayIsolationEnabled = TRUE;
static bool g_GatewayHooksAttached = false;
static bool g_GatewayProcessExiting = false;
enum class GatewayHookChange { None, Attach, Detach };
static GatewayHookChange g_GatewayHookChange = GatewayHookChange::None;

static BOOL CALLBACK GatewayInitializeSettings(PINIT_ONCE, PVOID, PVOID*)
{
    InterlockedExchange(&g_GatewayIsolationEnabled,
        MsRdpEx_GetEnvBool("MSRDPEX_GATEWAY_UNIQUE_BINDING", true) ? TRUE : FALSE);
    return TRUE;
}

bool MsRdpEx_GetGatewayIsolationEnabled()
{
    InitOnceExecuteOnce(&g_GatewaySettingsInit, GatewayInitializeSettings, NULL, NULL);
    return InterlockedCompareExchange(&g_GatewayIsolationEnabled, 0, 0) != FALSE;
}

void MsRdpEx_SetGatewayIsolationEnabled(bool enabled)
{
    InitOnceExecuteOnce(&g_GatewaySettingsInit, GatewayInitializeSettings, NULL, NULL);
    InterlockedExchange(&g_GatewayIsolationEnabled, enabled ? TRUE : FALSE);
}

static decltype(&RpcBindingSetAuthInfoExW) Real_RpcBindingSetAuthInfoExW = RpcBindingSetAuthInfoExW;
static decltype(&RpcBindingFree) Real_RpcBindingFree = RpcBindingFree;
static decltype(&RpcBindingSetOption) Real_RpcBindingSetOption = RpcBindingSetOption;

// Remember successful authentication setup even when isolation is disabled.
// A later property change must not alter a binding already observed in this hook lifetime.
struct GatewayBinding { RPC_BINDING_HANDLE handle; GatewayBinding* next; };
static SRWLOCK g_BindingLock = SRWLOCK_INIT;
static GatewayBinding* g_Bindings = NULL;

// Return 1 for a new lifetime, 0 for a previously observed lifetime, -1 on allocation failure.
static int GatewayRememberBinding(RPC_BINDING_HANDLE binding)
{
    AcquireSRWLockExclusive(&g_BindingLock);
    for (auto* current = g_Bindings; current; current = current->next) {
        if (current->handle == binding) {
            ReleaseSRWLockExclusive(&g_BindingLock);
            return 0;
        }
    }
    auto* item = (GatewayBinding*)HeapAlloc(GetProcessHeap(), 0, sizeof(GatewayBinding));
    if (item) {
        item->handle = binding;
        item->next = g_Bindings;
        g_Bindings = item;
    }
    ReleaseSRWLockExclusive(&g_BindingLock);
    return item ? 1 : -1;
}

static GatewayBinding* GatewayTakeBinding(RPC_BINDING_HANDLE binding)
{
    GatewayBinding* retired = NULL;
    AcquireSRWLockExclusive(&g_BindingLock);
    auto** link = &g_Bindings;
    while (*link) {
        if ((*link)->handle == binding) {
            retired = *link;
            *link = retired->next;
            retired->next = NULL;
            break;
        }
        link = &(*link)->next;
    }
    ReleaseSRWLockExclusive(&g_BindingLock);
    return retired;
}

static void GatewayRestoreBinding(GatewayBinding* retired)
{
    if (!retired) return;
    AcquireSRWLockExclusive(&g_BindingLock);
    auto* current = g_Bindings;
    while (current && current->handle != retired->handle) current = current->next;
    if (!current) {
        retired->next = g_Bindings;
        g_Bindings = retired;
    }
    ReleaseSRWLockExclusive(&g_BindingLock);
    if (current) HeapFree(GetProcessHeap(), 0, retired);
}

static void GatewayClearBindings()
{
    AcquireSRWLockExclusive(&g_BindingLock);
    while (g_Bindings) {
        auto* item = g_Bindings;
        g_Bindings = item->next;
        HeapFree(GetProcessHeap(), 0, item);
    }
    ReleaseSRWLockExclusive(&g_BindingLock);
}

static bool GatewayIsRdpHttpBinding(RPC_BINDING_HANDLE binding, void* caller)
{
    if (!MsRdpEx_IsAddressInRdpAxModule(caller)) return false;
    RPC_WSTR text = NULL, protocol = NULL;
    bool match = false;
    if (RpcBindingToStringBindingW(binding, &text) == RPC_S_OK &&
        RpcStringBindingParseW(text, NULL, &protocol, NULL, NULL, NULL) == RPC_S_OK)
        match = protocol && wcscmp((WCHAR*)protocol, L"ncacn_http") == 0;
    if (text) RpcStringFreeW(&text);
    if (protocol) RpcStringFreeW(&protocol);
    return match;
}

static RPC_STATUS RPC_ENTRY GatewayRpcBindingSetAuthInfoExW(RPC_BINDING_HANDLE binding, RPC_WSTR principal,
    ULONG level, ULONG service, RPC_AUTH_IDENTITY_HANDLE identity, ULONG authorization, RPC_SECURITY_QOS* qos)
{
    DWORD incomingError = GetLastError();
    bool gateway = GatewayIsRdpHttpBinding(binding, _ReturnAddress());
    SetLastError(incomingError);
    RPC_STATUS status = Real_RpcBindingSetAuthInfoExW(binding, principal, level, service, identity, authorization, qos);
    DWORD error = GetLastError();
    if (gateway && status == RPC_S_OK) {
        int remembered = GatewayRememberBinding(binding);
        if (remembered != 0 && MsRdpEx_GetGatewayIsolationEnabled()) {
            RPC_STATUS optionStatus = remembered < 0 ? RPC_S_OUT_OF_MEMORY :
                Real_RpcBindingSetOption(binding, RPC_C_OPT_UNIQUE_BINDING, TRUE);
            if (optionStatus != RPC_S_OK)
                MsRdpEx_LogPrint(WARN, "Gateway isolation could not be applied: status=0x%08lX", optionStatus);
        }
    }
    SetLastError(error);
    return status;
}

static RPC_STATUS RPC_ENTRY GatewayRpcBindingFree(RPC_BINDING_HANDLE* binding)
{
    DWORD incomingError = GetLastError();
    RPC_BINDING_HANDLE handle = binding ? *binding : NULL;
    // Retire this lifetime before native free can make its address reusable.
    // Finish using the detached node, never a lookup by the recycled handle.
    GatewayBinding* retired = GatewayTakeBinding(handle);
    SetLastError(incomingError);
    RPC_STATUS status = Real_RpcBindingFree(binding);
    DWORD error = GetLastError();
    if (status != RPC_S_OK) GatewayRestoreBinding(retired);
    else if (retired) HeapFree(GetProcessHeap(), 0, retired);
    SetLastError(error);
    return status;
}

void MsRdpEx_AttachGatewayIsolationHooks()
{
    // Initialize before the first binding; later environment changes do not reset the property.
    MsRdpEx_GetGatewayIsolationEnabled();
    if (g_GatewayHooksAttached) return;
    g_GatewayHookChange = GatewayHookChange::Attach;
    MSRDPEX_DETOUR_ATTACH(Real_RpcBindingSetAuthInfoExW, GatewayRpcBindingSetAuthInfoExW);
    MSRDPEX_DETOUR_ATTACH(Real_RpcBindingFree, GatewayRpcBindingFree);
}

void MsRdpEx_DetachGatewayIsolationHooks()
{
    if (!g_GatewayHooksAttached) return;
    g_GatewayHookChange = GatewayHookChange::Detach;
    MSRDPEX_DETOUR_DETACH(Real_RpcBindingSetAuthInfoExW, GatewayRpcBindingSetAuthInfoExW);
    MSRDPEX_DETOUR_DETACH(Real_RpcBindingFree, GatewayRpcBindingFree);
}

void MsRdpEx_GatewayIsolationPrepareForProcessExit()
{
    // Called from DllMain after Windows has stopped all other threads. Their
    // binding locks may remain owned; let process exit reclaim the records.
    g_GatewayProcessExiting = true;
}

void MsRdpEx_GatewayIsolationHooksCommitted(LONG error)
{
    if (error == NO_ERROR) {
        if (g_GatewayHookChange == GatewayHookChange::Attach)
            g_GatewayHooksAttached = true;
        else if (g_GatewayHookChange == GatewayHookChange::Detach) {
            g_GatewayHooksAttached = false;
            if (!g_GatewayProcessExiting)
                GatewayClearBindings();
        }
    }
    g_GatewayHookChange = GatewayHookChange::None;
}
