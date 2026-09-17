# Managed gateway property regression tests

Build the actual managed library for both supported frameworks, then the test harness.
Run from the repository root, using an absolute output path:

```powershell
$managed = Join-Path (Get-Location) 'build-gateway-managed/Release'
dotnet build dotnet/Devolutions.MsRdpEx/Devolutions.MsRdpEx.csproj -c Release "-p:CMakeOutputPath=$managed" -p:BuildProjectReferences=false
dotnet build tests/gateway-managed/GatewaySettingsTest.csproj -c Release "-p:MsRdpExManagedRoot=$managed"
```

Run each mode in a fresh process with an absolute path to the chosen native DLL:

```powershell
# net48 harness is x64; use the x64 native DLL.
./tests/gateway-managed/bin/Release/net48/GatewaySettingsTest.exe C:/absolute/path/x64/MsRdpEx.dll default
./tests/gateway-managed/bin/Release/net48/GatewaySettingsTest.exe C:/absolute/path/x64/MsRdpEx.dll off

# Match the native DLL architecture to the dotnet host (e.g. ARM64 on an ARM64 host).
dotnet tests/gateway-managed/bin/Release/net8.0-windows/GatewaySettingsTest.dll C:/absolute/path/arm64/MsRdpEx.dll default
dotnet tests/gateway-managed/bin/Release/net8.0-windows/GatewaySettingsTest.dll C:/absolute/path/arm64/MsRdpEx.dll off
```

For each framework also run `older` with a pre-change native DLL of matching
architecture. This verifies that the original API remains usable and both property
accessors throw `NotSupportedException` when the optional interface is unavailable.
Do not replace the built managed library for this compatibility test.

Tests cover default enablement, startup opt-out, property precedence, sharing across
objects/threads and hook toggles. They do not contact a gateway. Native CTest covers
RPC binding behavior, COM identity and actual DLL unloading separately.
