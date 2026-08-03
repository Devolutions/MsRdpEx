# Microsoft RDP Extensions (MsRdpEx)

## Installation

Download and install the latest MsRdpEx MSI package [GitHub releases](https://github.com/Devolutions/MsRdpEx/releases).

After installation, the launcher executables and API hooking DLL can be found in "%ProgramFiles%\Devolutions\MsRdpEx":

![MsRdpEx Installed](./images/MsRdpEx_installed.png)

The installer automatically associates .RDP files with mstscex, and .RDPW files with msrdcex, so you can get started right away. Simply revert the file type association to use the original Microsoft Remote Desktop Clients without the extensions, or launch mstsc.exe/msrdc.exe manually.

This repository also contains a C# [nuget package](https://www.nuget.org/packages/Devolutions.MsRdpEx) that can be used to consume the RDP ActiveX interface with or without API hooking, along with launching mstsc.exe or msrdc.exe as external processes using MsRdpEx.dll.

### COM interop selection

The package selects legacy COM interop by default, preserving the original `Interop.MSTSCLib.dll` and `AxInterop.MSTSCLib.dll` APIs for both .NET Framework 4.8 and `net8.0-windows` consumers:

```xml
<PackageReference Include="Devolutions.MsRdpEx" Version="..." />
```

Set `MsRdpExComInterop` to `Generated` to reference the source-generated, NativeAOT-compatible `Interop.MSTSCLib.Generated.dll` instead:

```xml
<PropertyGroup>
  <MsRdpExComInterop>Generated</MsRdpExComInterop>
</PropertyGroup>
```

For a WinForms ActiveX host alongside the standalone generated projection, set `MsRdpExGeneratedWinForms` separately:

```xml
<PropertyGroup>
  <MsRdpExComInterop>Generated</MsRdpExComInterop>
  <MsRdpExGeneratedWinForms>true</MsRdpExGeneratedWinForms>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

This adds `Interop.MSTSCLib.Generated.WinForms.dll`, whose `MsRdpEx.WinForms.GeneratedRdpClientHost` hosts a generated RDP proxy with a configurable CLSID, ActiveX DLL name, and optional MsRdpEx DLL path; call `GetClient<T>()` after the control is created. It is a WinForms-only feature and is not NativeAOT-compatible.

`GeneratedWinForms` remains available for applications moving to the generated projection that require its `AxMSTSCLib` wrapper types. Existing legacy `AxMSTSCLib` consumers remain on the default `Legacy` mode. Set `MsRdpExComInterop` to `None` when an application needs neither interop assembly.

Packages that ship generated assets expose `MsRdpExGeneratedInteropSupported=true`, `MsRdpExGeneratedWinFormsInteropSupported=true`, and `MsRdpExGeneratedWinFormsHostSupported=true` after the package targets are imported. Selecting a generated mode or host validates that its assembly is present and fails with a clear package capability error otherwise.

#### Legacy and generated API compatibility

`Generated` preserves the public `MSTSCLib` interface names and enums from the legacy assembly. It uses source-generated COM interfaces internally, so raw `MsRdpEx.Interop` interfaces expose explicit `GetX` and `SetX` methods; use the `MSTSCLib` compatibility interfaces and extensions for property-shaped APIs. `MSTSCLibExtensions` provides conventional methods for extended-settings values, endpoint and publisher-certificate-chain configuration, byte-preserving load-balance information, generated interface conversion, and drive, device, and camera collection lookups.

Use `RdpClientFactory.CreateClient10()` to activate the Microsoft RDP Client Control version 11 without legacy coclasses. The returned compatibility interface can use `Subscribe()` to obtain an `RdpClientEventSubscription`; dispose it to unadvise the COM connection point. The subscription exposes all legacy `IMsTscAxEvents` callbacks as managed events, including mutable confirmation, public-key, and auto-reconnect response event arguments for COM by-ref callbacks. This runtime API requires Windows and an apartment-threaded caller.

The NativeAOT package-consumer fixture accepts `--com-integration` to check activation, generated proxy invocation, and event Advise/Unadvise behavior without opening a remote connection. It exits with code `77` and prints `SKIPPED` when the RDP Client Control version 11 is not registered.

The generated assets deliberately do not provide legacy COM coclasses (such as `MsRdpClient10Class`), classic `*_Event`/`SinkHelper` event helpers, or MIDL implementation-detail types. Use `Legacy` when an application needs those APIs. The additive WinForms host has the same generated interop boundary; `GeneratedWinForms` instead adds the older `AxMSTSCLib` host controls.

The legacy and generated assets define overlapping `MSTSCLib` type names and cannot be referenced by the same application. Select exactly one `MsRdpExComInterop` mode per project.

## Extended .RDP File Options

MsRdpEx processes additional .RDP file options that are not normally supported by mstsc.exe:

| RDP setting                        | Description            | Values                 | Default value          |
|------------------------------------|------------------------|------------------------|:----------------------:|
| KDCProxyURL:s:value | Kerberos KDC Proxy HTTPS URL | KDC Proxy HTTPS *URL*, not using error-prone KDCProxyName format, and unrestricted in length, like https://<hostname>:443/KdcProxy | - |
| UserSpecifiedServerName:s:value | Server name used for TLS and Kerberos server validation | explicit server name (usually the machine FQDN) | same as DNS hostname used for RDP server |
| EnableMouseJiggler:i:value | Enable RDP mouse jiggler | 0/1 | 0 |
| MouseJigglerInterval:i:value | RDP mouse jiggler interval in seconds | Interval in seconds | 60 |
| MouseJigglerMethod:i:value | RDP mouse jiggler method | 0/1 | 0 |
| AllowBackgroundInput:i:value | Allow background input events when window is not in focus | 0/1 | 0 |
| EnableRelativeMouse:i:value | Enable relative mouse mode | 0/1 | 0 |
| DisableCredentialsDelegation:i:value | Disable CredSSP credential delegation | 0/1 | 0 |
| RedirectedAuthentication:i:value | Enable Remote Credential Guard | 0/1 | 0 |
| RestrictedLogon:i:value | Enable Restricted Admin Mode | 0/1 | 0 |
| DisableUDPTransport:i:value | Disable RDP UDP transport (TCP only) | 0/1 | 0 |
| ConnectToChildSession:i:value | Connect to child session | 0/1 | 0 |
| EnableHardwareMode:i:value | Disable DirectX client presenter (force GDI client presenter) | 0/1 | 1 |
| ClearTextPassword:s:value | Target RDP server password - use for testing only | Insecure password | - |
| GatewayPassword:s:value | RD Gateway server password - use for testing only | Insecure password | - |
| KerbCertificateLogon:i:value | Hand LSASS a KERB_CERTIFICATE_LOGON for smart card logon (see below) | 0/1 | 0 |

### Smart card certificate logon (KerbCertificateLogon)

When a smart card certificate and its PIN are both pre-supplied (an unattended logon, with no
interactive prompt), the CredSSP/SSPI credential that reaches LSASS can take a code path in
`tspkg` that calls `CryptAcquireCertificatePrivateKey` without
`CRYPT_ACQUIRE_ALLOW_NCRYPT_KEY_FLAG`, which fails for keys backed by a CNG/NCrypt key storage
provider (the common case for smart cards). The interactive Windows credential prompt is not
affected, because it packs the credential as a `KERB_CERTIFICATE_LOGON` structure that `tspkg`
handles on a different path.

Setting `KerbCertificateLogon:i:1` opts the session in to a client-side workaround: MsRdpEx hooks
`AcquireCredentialsHandleW` and, for the matching session, hands LSASS a `KERB_CERTIFICATE_LOGON`
credential (`CredsspCertificateCreds`). It obtains it either by:

- **synthesizing** it from the session's marshaled certificate user name and PIN — used when the
  host does not pass the credential in-band, such as the in-process RDP ActiveX control; or
- **rewriting** a marshaled smart card credential that is already present in the
  `AcquireCredentialsHandleW` auth data — such as the out-of-process `mstsc.exe` case.

The workaround is strictly opt-in and stays inert unless a PIN is available: with no PIN the
credential is left untouched so the normal Windows prompt path is used. It only activates for a
marshaled certificate credential; any other credential is passed through unchanged.

`KerbCertificateLogon` is independent of `PasswordContainsSCardPin`. The latter is the stock RDP
setting that tells the client the password field holds a smart card PIN, so that a smart card
credential is delegated to the remote and no prompt is shown; a connection manager doing an
unattended certificate + PIN logon typically sets both.

The captured PIN is kept only for the lifetime of the connection, encrypted in memory with DPAPI
(`CryptProtectMemory`), decrypted only transiently while the credential is built, and zeroed
before it is released. No PINs, passwords, or certificate bytes are logged.

Set the `MSRDPEX_SSPI_SMARTCARD_DEBUG=1` environment variable to emit additional (secret-free)
CredSSP credential metadata to the log while diagnosing smart card logon issues.

## Extended RDP client logs

MsRdpEx also supports extended logging controlled by environment variables:

```powershell
$Env:MSRDPEX_LOG_ENABLED="1"
$Env:MSRDPEX_LOG_LEVEL="DEBUG"
.\mstscex.exe <destination.rdp>
```

If you don't pass a .RDP file, the mstsc.exe GUI will launch normally, but you won't be able to leverage any of the extended MsRdpEx .RDP file options. The default log file path location is in "%LocalAppData%\MsRdpEx\MsRdpEx.log". You can override log settings using the MSRDPEX_LOG_LEVEL and MSRDPEX_LOG_FILE_PATH environment variables:

```powershell
$Env:MSRDPEX_LOG_ENABLED="1"
$Env:MSRDPEX_LOG_LEVEL="TRACE"
$Env:MSRDPEX_LOG_FILE_PATH="C:\Windows\Temp\MsRdpEx.log"
.\mstscex.exe
```

The trace log level is extremely verbose, so it should only be used when necessary. The MsRdpEx logging is very helpful in understanding the Microsoft RDP client internals.

## Public ActiveX backends

Set `MSRDPEX_AX_BACKEND=public` for a non-Microsoft `mstscax`-compatible replacement selected by `MSRDPEX_MSTSCAX_DLL`. MsRdpEx also automatically uses public mode when that replacement DLL is named `ironrdpax.dll`, unless `MSRDPEX_AX_BACKEND` is explicitly set. MsRdpEx continues to load the replacement DLL and forward its public COM class factory, but does not inspect the replacement client's undocumented Microsoft object layout.

Public mode intentionally disables private extended-settings/property hooks and extended `.RDP` option processing, input/output window association (including output mirroring and recording), SSPI session correlation, and other features that depend on Microsoft private ActiveX internals. The default `private` mode retains standard `mstscax.dll` behavior. Any explicit backend value other than `public` retains private mode; automatic `ironrdpax.dll` detection applies only when the backend option is unset.

## Building from source

Generate the Visual Studio project files for your target platform:

```powershell
mkdir build-x64 && cd build-x64
cmake -G "Visual Studio 18 2026" -A x64 ..
```

Open the Visual Studio solution or build it from the command-line:

```powershell
cmake --build . --config Release
```

You should now have mstscex.exe and MsRdpEx.dll.
