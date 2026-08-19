# MsRdpEx Avalonia sample

This Windows-only sample hosts an RDP session inside an Avalonia 12 window
using a real, visible Win32 child window — without using Windows Forms or WPF.

`RdpClientView` comes from the reusable `Devolutions.MsRdpEx.Avalonia` project
and is an Avalonia `NativeControlHost`. MsRdpEx activates the Microsoft RDP
ActiveX control through its own OLE client site and in-place site inside the
hosted child HWND; it does not use Windows Forms `AxHost` or ATL's ActiveX
host. The control renders directly to the screen (including its hardware
DirectX path) and receives native keyboard and mouse input with normal Win32
focus semantics — there is no off-screen HWND, no framebuffer copy, and no
synthetic input forwarding.

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
select **Connect**. A separate native Avalonia session window opens and hosts
the RDP control. The password is passed directly to the RDP control,
cleared from the dialog, and never written to disk by the sample.

The session window uses a custom Avalonia title bar above the native RDP
surface (the hosted HWND always draws above Avalonia content, so a native
title bar would swallow the mstsc-style menu). Right-click the title bar to
open the session context menu. Use the title-bar Close button to disconnect
and close the session.

### Fullscreen and display modes

The session window follows mstsc conventions. Right-click the **title bar**
(not the remote desktop — that click belongs to the native HWND) to open the
context menu:

- **Fullscreen** — choose **Full screen**, or press **Ctrl+Alt+Break** in the
  session. The custom title bar hides, the window covers the screen, and the
  control's floating connection bar (labeled with the server name) auto-hides
  at the top edge, with pin, minimize, restore, and close buttons. Exit with
  Ctrl+Alt+Break or the connection bar's restore button.
- **Smart sizing** — the context menu's checkable **Smart sizing** item scales
  the remote desktop to the window size client-side instead of renegotiating
  the session resolution (the control shows scrollbars when the desktop is
  larger than the window).
- **Zoom** — the context menu's **Zoom** submenu (25%–400%) presents the
  desktop at a fixed scale. 100% returns to the default fit-to-window mode.

The default **Fit the session window** display mode uses the hosted window's
physical pixel size for the initial desktop. Resizing the window sends a
single debounced dynamic-resolution update once the size settles. Explicit
resolutions selected on the Display tab remain fixed.

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

When the resolved host, user, and password values are all present (including
through `RDP_HOSTNAME`, `RDP_USERNAME`, and `RDP_PASSWORD`), the sample opens
the RDP session directly without displaying the connection dialog. The password
is passed in memory to the control and is not written to disk.

## Notes

- The sample targets x64 Windows and Avalonia 12.1.1.
- The sample references the reusable `Devolutions.MsRdpEx.Avalonia` project;
  other Avalonia applications can embed the same `RdpClientView` control.
- The hosted HWND always draws above Avalonia content (airspace), so Avalonia
  overlays cannot compose on top of the RDP view.
- Server certificate and credential prompts are owned by the Microsoft RDP
  control and may appear as separate native dialogs.
