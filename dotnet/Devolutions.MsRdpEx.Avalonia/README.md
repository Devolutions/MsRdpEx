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
