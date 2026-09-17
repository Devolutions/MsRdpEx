# RD Gateway binding isolation

A customer confirmed that unique RPC bindings resolve a failure where the first
embedded Microsoft RDP ActiveX gateway connection succeeds and the second fails,
independent of connection order. This confirms a workaround, not credential caching
as the underlying defect. Removing connection reuse may increase connection setup
work and resource usage; compatibility across every gateway has not been established.

Isolation defaults to enabled when MsRdpEx ActiveX hooks are enabled. Only
`RpcBindingSetAuthInfoExW` calls from `mstscax.dll` or `rdclientax.dll` on `ncacn_http`
bindings qualify. After successful authentication setup, MsRdpEx sets
`RPC_C_OPT_UNIQUE_BINDING` once per observed binding lifetime. It does not change
credentials, authentication levels, TLS validation, native return values or last-error
state. Later application option changes are not overridden. Alternate/ANSI RPC
authentication setup APIs are outside this workaround's scope.

## Process-wide property

```csharp
var core = new MsRdpEx.RdpCoreApi();
core.GatewayIsolationEnabled = false; // Set before opening connections.
```

The read/write property is shared by all core API objects and is thread-safe. It
controls the first successful authentication setup observed for each eligible binding.
Already observed bindings retain their behavior, including bindings first observed
while isolation was disabled. A property change neither reconnects existing sessions
nor removes isolation from them. Set it before connecting for a predictable policy.

Native hosts query `IMsRdpExGatewaySettings` on the existing `IMsRdpExCoreApi` object
and call `GetGatewayIsolationEnabled` / `SetGatewayIsolationEnabled`. The original
core IID and vtable are unchanged. Older callers continue to work. Accessing the new
managed property with an older native DLL throws `NotSupportedException`.

`MSRDPEX_GATEWAY_UNIQUE_BINDING` remains an optional startup override (`0`/`false`
or `1`/`true`). It is read once per DLL load; property assignments take precedence
and environment changes during execution are ignored. No environment variable or
logging configuration is required for normal use.

The hooks follow normal ActiveX hook attach/detach. Successful detach clears tracking;
reattachment starts a new tracking period. Failed detach preserves tracking. The
property survives hook toggles but resets on DLL unload/reload. The module is not
pinned. During process termination, tracking is left for Windows to reclaim because
terminated threads may still own its lock; normal detach and explicit DLL unloading
still clear it. Change hook state only when connection work is quiescent, as for the
other MsRdpEx hooks.

## Logging and validation

The investigation-only WinHTTP, SSPI, identity, QoS, RPC completion and option tracing
has been removed, along with `MSRDPEX_GATEWAY_DIAGNOSTICS`. The isolation component
only logs a warning status if isolation cannot be applied. It never logs credentials,
connection addresses or binding metadata. Isolation works with logging disabled.

Build the existing logging test configurations and run CTest for x64, x86 and ARM64.
`logging.gateway-isolation*` covers scope, property changes, authentication/option
failures, error preservation, binding reuse and transaction bookkeeping.
`logging.gateway-core-api.*` exercises the original and extension COM interfaces,
startup overrides, shared property state, hook toggles and actual DLL unloading.
`logging.recording-exit-gateway-lock` verifies that process exit still finalizes a
recording when Windows terminates a worker holding the gateway binding lock.
The managed regression harness is documented in `../gateway-managed/README.md`.

Before release, repeat the customer's two-connection scenario with the cleaned build
and logging disabled. Keep the first connection open while creating the second. Test
the default enabled behavior and a fresh host with the property set to false. The
previous diagnostic ZIPs do not validate this cleaned implementation.

API reference: [RPC binding options](https://learn.microsoft.com/en-us/windows/win32/rpc/binding-option-constants).
