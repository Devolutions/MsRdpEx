
include_external_msproject(AxInterop.MSTSCLib
    "${CMAKE_CURRENT_SOURCE_DIR}/AxInterop.MSTSCLib/AxInterop.MSTSCLib.csproj")

include_external_msproject(Devolutions.MsRdpEx
    "${CMAKE_CURRENT_SOURCE_DIR}/Devolutions.MsRdpEx/Devolutions.MsRdpEx.csproj")

include_external_msproject(MsRdpEx_App
    "${CMAKE_CURRENT_SOURCE_DIR}/MsRdpEx_App/MsRdpEx_App.csproj")

include_external_msproject(NowProto
    "${CMAKE_CURRENT_SOURCE_DIR}/now-proto/nugets/Devolutions.NowProto/Devolutions.NowProto.csproj")

include_external_msproject(NowClient
    "${CMAKE_CURRENT_SOURCE_DIR}/now-proto/nugets/Devolutions.NowClient/Devolutions.NowClient.csproj")

configure_file("${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props.in"
    "${CMAKE_CURRENT_SOURCE_DIR}/Directory.Build.props" @ONLY)
