
include_external_msproject(AxInterop.MSTSCLib
    "${CMAKE_CURRENT_SOURCE_DIR}/AxInterop.MSTSCLib/AxInterop.MSTSCLib.csproj")

include_external_msproject(MsRdpEx_App
    "${CMAKE_CURRENT_SOURCE_DIR}/MsRdpEx_App/MsRdpEx_App.csproj")

configure_file("${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props.in"
    "${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props" @ONLY)
