# Devolutions.MsRdpEx.Avalonia

`RdpClientView` is a reusable Windows-only Avalonia 12 control that hosts the
Microsoft RDP ActiveX control in a real, visible Win32 child window inside the
Avalonia window. It builds on Avalonia's `NativeControlHost`: MsRdpEx supplies
the OLE client-site and in-place-site interfaces required to activate the
control, and the control renders directly to the screen.

Compared with a bitmap-mirror approach, native hosting means:

- **No off-screen HWND and no framebuffer copy.** The RDP control's window is
  the visible surface, so hardware (DirectX) rendering presents directly.
- **Native input.** Keyboard and mouse input flow through the control's own
  window with normal Win32 focus semantics — no low-level keyboard hook, no
  synthetic message forwarding, and no `allowBackgroundInput` state to manage.
  Minimize/restore, focus switches, and window activation behave exactly like
  a windowed `mstsc.exe` session.

The hosting path does not use Windows Forms `AxHost` or ATL's ActiveX host.

The view can connect with `RdpConnectionSettings`, or a host application can
use `GetClient<T>()` after `ClientReady` to configure generated MSTSCLib
interfaces directly before connecting.

When consuming `Devolutions.MsRdpEx` from NuGet, enable the control and the
required generated COM projection in the project file:

```xml
<PropertyGroup>
  <MsRdpExAvalonia>true</MsRdpExAvalonia>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Avalonia" Version="12.1.1" />
  <PackageReference Include="Devolutions.MsRdpEx" Version="..." />
</ItemGroup>
```

Then place the view in Avalonia XAML:

```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:rdp="using:Devolutions.MsRdpEx.Avalonia">
  <rdp:RdpClientView x:Name="RdpView" />
</Window>
```

Avalonia remains an application dependency so the base MsRdpEx package does
not impose Avalonia on its WinForms and .NET Framework consumers.

## Focus and keyboard

Keyboard handling belongs to the RDP control itself. While the hosted window
has focus, the control's own keyboard hook decides which keys go to the remote
session, exactly like a windowed Remote Desktop Connection. `FocusSession()`
moves native focus to the remote session programmatically; the control's
`FocusReleased` event is forwarded so Tab/Shift+Tab move focus back to the
hosting application's next Avalonia control.

`UseRemoteKeyboardShortcuts` (default true) maps to the control's
`KeyboardHookMode`: when true, Windows/system combinations such as Alt+Tab are
sent to the remote session while the control is focused; when false they stay
local.

## Resizing, dynamic resolution, and fullscreen

The hosted window tracks the view's layout bounds automatically. When the
connection uses dynamic resolution, a resize applies a single
`UpdateSessionDisplaySettings` renegotiation once the size has been stable for
a short debounce interval, rather than one update per intermediate size.

Fullscreen is container-driven: setting `FullScreen` (or pressing
Ctrl+Alt+Break in the session) covers the screen with the containing window
and switches the control to fullscreen rendering with its floating connection
bar (auto-hiding, with pin/minimize/restore/close). The bar and keyboard
requests flow back through the view, and restoring from the taskbar after a
minimize returns to fullscreen.

`DisplayMode` mirrors the mstsc system-menu display options:
`FitToWindow` (dynamic resolution follows the viewport), `SmartSizing`
(client-side scaling with scrollbars), and `Zoom` with `ZoomLevel` (25–400%;
100% means fit-to-window).

## Dispose contract

Detaching the view from the visual tree intentionally keeps the session alive
for docking and tab reparenting (`NativeControlHost` defers native destruction
across reparenting). Owners **must** call `Dispose()` on final close, on the
Avalonia UI thread (`VerifyAccess` throws otherwise): final teardown of the
OLE host and COM wrappers happens there. OLE is uninitialized only when this
library performed the initialization itself and no other `RdpClientView`
session remains; a balancing uninitialize deferred because OLE was already
initialized by the hosting application is carried forward to the last
surviving session instead.

## Known limitations

- **Airspace.** The native window always draws above Avalonia visuals.
  Avalonia overlays, popups, and transforms cannot compose on top of the RDP
  view; use separate windows for floating UI.
- **No local file drop into the remote session.** Avalonia drag-drop events do
  not reach the view because the native child window covers it. Use clipboard
  copy/paste or drive redirection instead.
- Server certificate and credential prompts are owned by the Microsoft RDP
  control and appear as native dialogs parented to the top-level window.
