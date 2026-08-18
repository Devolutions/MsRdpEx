# Devolutions.MsRdpEx.Avalonia

`RdpClientView` is a reusable Windows-only Avalonia 12 `UserControl` that
renders the MsRdpEx output-mirror bitmap without placing the RDP ActiveX HWND
inside the Avalonia visual tree. It forwards pointer, wheel, keyboard, drag,
cursor, and dynamic-resolution behavior to the off-screen RDP session.

The hosting path does not use Windows Forms `AxHost`, Avalonia
`NativeControlHost`, or ATL's ActiveX host. MsRdpEx directly implements the OLE
client-site and in-place-site interfaces needed to activate the control in its
off-screen HWND. Only the mirrored bitmap is presented in Avalonia.

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

## Focus, activation, and keyboard routing

The view drives OLE UI activation from Avalonia focus (UI-activate on
`GotFocus`, UI-deactivate on `LostFocus`) and forwards top-level window
`Activated`/`Deactivated` to the OLE frame. Window deactivation also releases
any forwarded keys and mouse buttons so they never stay pressed remotely.
While the view is focused, a low-level keyboard hook forwards keys to the
remote session; keys pass through untouched when the window is inactive or no
connection is live, so Tab and focus traversal keep working.

Set `LocalKeyFilter` to let the hosting application claim keys (menu
accelerators, global shortcuts) before they are forwarded remotely:

```csharp
RdpView.LocalKeyFilter = (virtualKey, keyUp, systemKey) =>
    virtualKey == 0x77; // keep F8 local, both press and release
```

A claimed key passes through to normal local processing and is never sent to
the remote session. Answer consistently for a key press and its release.
`UseRemoteKeyboardShortcuts` still controls whether Windows/system
combinations (Alt+Tab, Alt+F4, Windows key) stay local.

## Dispose contract

Detaching the view from the visual tree intentionally keeps the session alive
for docking and tab reparenting. Owners **must** call `Dispose()` on final
close, on the Avalonia UI thread (`VerifyAccess` throws otherwise): final
teardown of the OLE host, the keyboard hook, and COM wrappers happens there.
OLE is uninitialized only when this library performed the initialization
itself and no other `RdpClientView` session remains; OLE initialized by the
hosting application is never torn down.

## Known limitations

- **File drop replaces the local clipboard content.** Dropping local files
  onto the view stages them as `CF_HDROP` on the Windows clipboard and sends
  a synthetic Ctrl+V to the remote session; any previous clipboard content is
  discarded.
- **No remote-to-local pointer drag-out.** Dragging a file out of the remote
  session onto the local desktop is not supported: the OLE drag source lives
  on the off-screen control window and cannot reach the local desktop.
