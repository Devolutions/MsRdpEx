# MsRdpEx Avalonia sample

This Windows-only sample renders an RDP session into an Avalonia 12 control
without placing a native HWND in the Avalonia visual tree and without using
Windows Forms or WPF.

`RdpClientView` comes from the reusable `Devolutions.MsRdpEx.Avalonia` project
and is a regular Avalonia `UserControl`. MsRdpEx activates the Microsoft RDP
ActiveX control through its own OLE client site and in-place site in an
off-screen HWND; it does not use Windows Forms `AxHost` or ATL's ActiveX host.
MsRdpEx mirrors the session output into a top-down 32-bit DIB, and the control
copies that backing buffer into an Avalonia `WriteableBitmap`. Pointer, wheel,
and keyboard input are mapped back to the RDP input window.

`build-x64/Release/MsRdpEx.dll` is required and is copied beside the sample
executable automatically when the project builds.

## Run

From the repository root:

```powershell
cmake --build .\build-x64 --config Release
dotnet run --project .\dotnet\MsRdpEx_AvaloniaApp\MsRdpEx_AvaloniaApp.csproj
```

The app opens with a connection-settings dialog modeled after the Microsoft
Remote Desktop Connection dialog. Enter the destination host, user name,
password, and optional domain, choose an optional desktop resolution, then
select **Connect**. A separate native Avalonia session window opens and renders
the RDP backing bitmap. The password is passed directly to the RDP control,
cleared from the dialog, and never written to disk by the sample.

The session window contains only the edge-to-edge RDP surface. Use the standard
window Close button to disconnect and close the session.

The default **Fit the session window** display mode uses the Avalonia canvas's
physical pixel size for the initial desktop. Resizing the window sends a
debounced dynamic-resolution update after login. Explicit resolutions selected
on the Display tab remain fixed and are only scaled for presentation.

## Command line and environment

The dialog accepts the same `RDP_HOSTNAME`, `RDP_USERNAME`, `RDP_PASSWORD`, and
`RDP_FILENAME` environment variables as `MsRdpEx_App`. `RDP_DOMAIN` is also
supported. Command-line values override the environment and can use either
long option names or the common RDP aliases:

```powershell
MsRdpEx_AvaloniaApp.exe --hostname it-help-dc --username administrator --domain CONTOSO
MsRdpEx_AvaloniaApp.exe /v:it-help-dc /u:administrator /d:CONTOSO
MsRdpEx_AvaloniaApp.exe --filename .\connection.rdp
MsRdpEx_AvaloniaApp.exe .\connection.rdp
```

Supported long names are `--hostname`, `--username`, `--password`, `--domain`,
and `--filename`; their `--RDP_*` forms are accepted as well. Supplying a
password on a command line can expose it through process inspection, so the
password field or `RDP_PASSWORD` should be preferred.

## Notes

- The sample targets x64 Windows and Avalonia 12.1.1.
- The sample references the reusable `Devolutions.MsRdpEx.Avalonia` project;
  other Avalonia applications can embed the same `RdpClientView` control.
- The visible surface is fully Avalonia-rendered, so Avalonia overlays and
  transforms can compose with it normally.
- The sample polls the MsRdpEx shadow bitmap at approximately 30 frames per
  second. A production control may want dirty-region signaling instead.
- Server certificate and credential prompts are owned by the Microsoft RDP
  control and may appear as separate native dialogs.
