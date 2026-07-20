# Microsoft RDP Extensions (MsRdpEx)

## Installation

Download and install the latest MsRdpEx MSI package [GitHub releases](https://github.com/Devolutions/MsRdpEx/releases).

After installation, the launcher executables and API hooking DLL can be found in "%ProgramFiles%\Devolutions\MsRdpEx":

![MsRdpEx Installed](./images/MsRdpEx_installed.png)

The installer automatically associates .RDP files with mstscex, and .RDPW files with msrdcex, so you can get started right away. Simply revert the file type association to use the original Microsoft Remote Desktop Clients without the extensions, or launch mstsc.exe/msrdc.exe manually.

This repository also contains a C# [nuget package](https://www.nuget.org/packages/Devolutions.MsRdpEx) that can be used to consume the RDP ActiveX interface with or without API hooking, along with launching mstsc.exe or msrdc.exe as external processes using MsRdpEx.dll.

### COM interop selection

The package selects legacy COM interop by default, preserving the original `Interop.MSTSCLib.dll` API:

```xml
<PackageReference Include="Devolutions.MsRdpEx" Version="..." />
```

Set `MsRdpExComInterop` to `Generated` to reference the source-generated, NativeAOT-compatible `Interop.MSTSCLib.Generated.dll` instead:

```xml
<PropertyGroup>
  <MsRdpExComInterop>Generated</MsRdpExComInterop>
</PropertyGroup>
```

`GeneratedWinForms` selects the source-generated `AxInterop.MSTSCLib.dll` ActiveX host wrapper. It is intended for WinForms and is not NativeAOT-compatible. Set the property to `None` when an application needs neither interop assembly.

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
