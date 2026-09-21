// Production hooks, native RPC binding objects, and fake transport entry points.
// No gateway, real credentials, network access, or test exports required.
#include "../../dll/GatewayIsolation.cpp"
#include <cstdarg>
#include <string>
#include <stdexcept>
#include <iostream>
#include <thread>

static std::string captured;
static bool rdpCaller = true;
static bool logEnabled = true;
static bool startupEnabled = true;
static int environmentReads = 0;
extern "C" bool MsRdpEx_IsLogLevelActive(uint32_t) { return logEnabled; }
extern "C" bool MsRdpEx_Log(uint32_t, const char* format, ...)
{
    char buffer[4096]; va_list args; va_start(args, format);
    vsnprintf(buffer, sizeof(buffer), format, args); va_end(args);
    captured += buffer; captured += '\n';
    SetLastError(999);
    return true;
}
extern "C" bool MsRdpEx_GetEnvBool(const char*, bool fallback) { ++environmentReads; return startupEnabled ? fallback : false; }
bool MsRdpEx_IsAddressInRdpAxModule(PVOID) { return rdpCaller; }

static void Check(bool condition, const char* message)
{
    if (!condition) throw std::runtime_error(message);
}
static RPC_STATUS authStatus = RPC_S_OK, optionStatus = RPC_S_OK;
static int optionCalls = 0;
static RPC_STATUS RPC_ENTRY FakeAuth(RPC_BINDING_HANDLE, RPC_WSTR, ULONG, ULONG,
    RPC_AUTH_IDENTITY_HANDLE, ULONG, RPC_SECURITY_QOS*)
{
    SetLastError(123); return authStatus;
}
static RPC_STATUS RPC_ENTRY FakeOption(RPC_BINDING_HANDLE, ULONG option, ULONG_PTR value)
{
    Check(option == RPC_C_OPT_UNIQUE_BINDING && value == TRUE, "Unexpected binding mutation");
    ++optionCalls; SetLastError(456); return optionStatus;
}
static RPC_BINDING_HANDLE Binding(const WCHAR* text)
{
    RPC_BINDING_HANDLE binding = NULL;
    Check(RpcBindingFromStringBindingW((RPC_WSTR)text, &binding) == RPC_S_OK, "Cannot create RPC test binding");
    return binding;
}
static void Auth(RPC_BINDING_HANDLE binding)
{
    Check(GatewayRpcBindingSetAuthInfoExW(binding, NULL, RPC_C_AUTHN_LEVEL_PKT_PRIVACY,
        RPC_C_AUTHN_GSS_NEGOTIATE, NULL, 0, NULL) == authStatus, "Auth result changed");
    Check(GetLastError() == 123, "Auth last error changed");
}
static bool GatewayOwnsBinding(RPC_BINDING_HANDLE binding)
{
    bool found = false;
    AcquireSRWLockShared(&g_BindingLock);
    for (auto* current = g_Bindings; current; current = current->next)
        if (current->handle == binding) found = true;
    ReleaseSRWLockShared(&g_BindingLock);
    return found;
}

static void TestBindings()
{
    Real_RpcBindingSetAuthInfoExW = FakeAuth;
    Real_RpcBindingSetOption = FakeOption;
    MsRdpEx_SetGatewayIsolationEnabled(true);
    auto first = Binding(L"ncacn_http:example.invalid[3388]");
    auto local = Binding(L"ncalrpc:[MsRdpExUnitTest]");
    Auth(local); Check(optionCalls == 0, "Modified non-HTTP binding");
    GatewayRpcBindingFree(&local);
    rdpCaller = false; Auth(first); Check(optionCalls == 0, "Modified unrelated caller binding");
    rdpCaller = true; authStatus = RPC_S_ACCESS_DENIED;
    Auth(first); Check(optionCalls == 0 && !GatewayOwnsBinding(first), "Tracked failed authentication");
    authStatus = RPC_S_OK; Auth(first); Auth(first);
    Check(optionCalls == 1 && captured.empty(), "Isolation not applied once, or success logged");
    MsRdpEx_SetGatewayIsolationEnabled(false); Auth(first);
    Check(optionCalls == 1, "Property modified existing binding");
    auto disabled = Binding(L"ncacn_http:example.invalid[3389]");
    Auth(disabled); Check(GatewayOwnsBinding(disabled) && optionCalls == 1, "Disabled binding not tracked");
    MsRdpEx_SetGatewayIsolationEnabled(true); Auth(disabled);
    Check(optionCalls == 1, "Reenabled isolation changed an observed binding");
    auto failed = Binding(L"ncacn_http:example.invalid[3390]");
    optionStatus = RPC_S_CANNOT_SUPPORT; Auth(failed); Auth(failed);
    Check(optionCalls == 2 && captured.find("could not be applied: status=") != std::string::npos,
        "Option failure not reported or repeated");
    Check(captured.find("example.invalid") == std::string::npos, "Connection metadata logged");
    auto unlogged = Binding(L"ncacn_http:example.invalid[3391]");
    logEnabled = false; optionStatus = RPC_S_OK; Auth(unlogged);
    Check(optionCalls == 3, "Isolation depends on logging");
    GatewayRpcBindingFree(&first); GatewayRpcBindingFree(&disabled);
    GatewayRpcBindingFree(&failed); GatewayRpcBindingFree(&unlogged);
    Check(!g_Bindings, "Binding records leaked");
}

static HANDLE reuseRetired = NULL, reuseResume = NULL;
static RPC_STATUS RPC_ENTRY ReusedBindingFree(RPC_BINDING_HANDLE* binding)
{
    // Model native free/reallocation at the identical address deterministically.
    // Keep the native allocation alive so parsing the new logical binding is valid.
    *binding = NULL;
    SetEvent(reuseRetired);
    WaitForSingleObject(reuseResume, 5000);
    SetLastError(321);
    return RPC_S_OK;
}
static RPC_STATUS RPC_ENTRY FailedBindingFree(RPC_BINDING_HANDLE*)
{
    SetLastError(654); return RPC_S_INVALID_BINDING;
}
static void TestBindingReuse()
{
    rdpCaller = true; MsRdpEx_SetGatewayIsolationEnabled(true);
    Real_RpcBindingSetAuthInfoExW = FakeAuth; Real_RpcBindingSetOption = FakeOption;
    auto binding = Binding(L"ncacn_http:example.invalid[3388]");
    Auth(binding); int before = optionCalls;
    auto original = binding;
    Real_RpcBindingFree = FailedBindingFree;
    Check(GatewayRpcBindingFree(&binding) == RPC_S_INVALID_BINDING && GetLastError() == 654 && binding == original,
        "Failed free result or handle changed");
    Auth(binding);
    Check(optionCalls == before && GatewayOwnsBinding(binding), "Failed free lost the original isolation record");
    reuseRetired = CreateEventW(NULL, TRUE, FALSE, NULL);
    reuseResume = CreateEventW(NULL, TRUE, FALSE, NULL);
    Check(reuseRetired && reuseResume, "Cannot create binding reuse events");
    Real_RpcBindingFree = ReusedBindingFree;
    RPC_STATUS freeStatus = RPC_S_CALL_FAILED; DWORD freeError = 0;
    std::thread freer([&] { freeStatus = GatewayRpcBindingFree(&binding); freeError = GetLastError(); });
    DWORD retired = WaitForSingleObject(reuseRetired, 5000);
    Auth(original); // A newly allocated binding may already reuse this address.
    int after = optionCalls;
    SetEvent(reuseResume); freer.join();
    Check(retired == WAIT_OBJECT_0 && freeStatus == RPC_S_OK && freeError == 321 && binding == NULL,
        "Successful free result changed");
    Check(after == before + 1, "Reused binding skipped isolation while old free was pending");
    Check(GatewayOwnsBinding(original), "Old free removed the new binding's tracking record");
    Real_RpcBindingFree = RpcBindingFree;
    GatewayRpcBindingFree(&original);
    Check(g_Bindings == NULL, "Binding reuse leaked records");
    CloseHandle(reuseRetired); CloseHandle(reuseResume);
}

static void TestHookBookkeeping()
{
    auto binding = Binding(L"ncacn_http:example.invalid[3388]");
    Auth(binding);
    g_GatewayHookChange = GatewayHookChange::Attach;
    MsRdpEx_GatewayIsolationHooksCommitted(ERROR_INVALID_OPERATION);
    Check(!g_GatewayHooksAttached && GatewayOwnsBinding(binding), "Failed attach changed state");
    g_GatewayHookChange = GatewayHookChange::Attach;
    MsRdpEx_GatewayIsolationHooksCommitted(NO_ERROR);
    g_GatewayHookChange = GatewayHookChange::Detach;
    MsRdpEx_GatewayIsolationHooksCommitted(ERROR_INVALID_OPERATION);
    Check(g_GatewayHooksAttached && GatewayOwnsBinding(binding), "Failed detach lost state");
    g_GatewayHookChange = GatewayHookChange::Detach;
    MsRdpEx_GatewayIsolationHooksCommitted(NO_ERROR);
    Check(!g_GatewayHooksAttached && !g_Bindings, "Successful detach did not clear state");
    g_GatewayHookChange = GatewayHookChange::Attach;
    MsRdpEx_GatewayIsolationHooksCommitted(NO_ERROR);
    int before = optionCalls; Auth(binding);
    Check(optionCalls == before + 1, "New hook lifetime retained stale records");
    GatewayRpcBindingFree(&binding);
    g_GatewayHookChange = GatewayHookChange::Detach;
    MsRdpEx_GatewayIsolationHooksCommitted(NO_ERROR);
}

int main(int argc, char** argv)
{
    try {
        startupEnabled = argc < 2 || strcmp(argv[1], "off") != 0;
        Check(MsRdpEx_GetGatewayIsolationEnabled() == startupEnabled, "Startup setting ignored");
        startupEnabled = !startupEnabled;
        MsRdpEx_SetGatewayIsolationEnabled(false);
        Check(!MsRdpEx_GetGatewayIsolationEnabled() && environmentReads == 1, "Environment reapplied");
        std::thread setter([] { MsRdpEx_SetGatewayIsolationEnabled(true); }); setter.join();
        Check(MsRdpEx_GetGatewayIsolationEnabled(), "Property is not process-wide");
        TestBindings(); TestBindingReuse(); TestHookBookkeeping();
        std::cout << "Gateway isolation tests passed\n";
        return 0;
    } catch (const std::exception& e) { std::cerr << e.what() << '\n'; return 1; }
}
