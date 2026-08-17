
include_external_msproject(AxInterop.MSTSCLib
    "${CMAKE_CURRENT_SOURCE_DIR}/AxInterop.MSTSCLib/AxInterop.MSTSCLib.csproj")

include_external_msproject(AxInterop.MSTSCLib.Legacy
    "${CMAKE_CURRENT_SOURCE_DIR}/AxInterop.MSTSCLib.Legacy/AxInterop.MSTSCLib.Legacy.csproj")

include_external_msproject(Interop.MSTSCLib.Generated.WinForms
    "${CMAKE_CURRENT_SOURCE_DIR}/Interop.MSTSCLib.Generated.WinForms/Interop.MSTSCLib.Generated.WinForms.csproj")

include_external_msproject(Devolutions.MsRdpEx.Avalonia
    "${CMAKE_CURRENT_SOURCE_DIR}/Devolutions.MsRdpEx.Avalonia/Devolutions.MsRdpEx.Avalonia.csproj")

include_external_msproject(Devolutions.MsRdpEx
    "${CMAKE_CURRENT_SOURCE_DIR}/Devolutions.MsRdpEx/Devolutions.MsRdpEx.csproj")

include_external_msproject(MsRdpEx_App
    "${CMAKE_CURRENT_SOURCE_DIR}/MsRdpEx_App/MsRdpEx_App.csproj")

include_external_msproject(MsRdpEx_AvaloniaApp
    "${CMAKE_CURRENT_SOURCE_DIR}/MsRdpEx_AvaloniaApp/MsRdpEx_AvaloniaApp.csproj")

configure_file("${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props.in"
    "${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props" @ONLY)
