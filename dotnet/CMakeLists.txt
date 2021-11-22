 
set(MSRDPEX_PROPERTIES_SOURCES
    Properties/AssemblyInfo.cs
    Properties/Resources.Designer.cs
    Properties/Resources.resx
    Properties/Settings.Designer.cs
    Properties/Settings.settings)

set(MSRDPEX_MAIN_DLG_SOURCES
    MainDlg.cs
    MainDlg.Designer.cs
    MainDlg.resx)

set(MSRDPEX_RDP_VIEW_SOURCES
    RdpView.cs
    RdpView.Designer.cs
    RdpView.resx)

add_executable(MsRdpEx_App
    App.config
    ${MSRDPEX_MAIN_DLG_SOURCES}
    ${MSRDPEX_RDP_VIEW_SOURCES}
    ${MSRDPEX_PROPERTIES_SOURCES}
    Program.cs)

set_property(TARGET MsRdpEx_App PROPERTY
    VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.2")

set_property(TARGET MsRdpEx_App PROPERTY WIN32_EXECUTABLE TRUE)

set(CMAKE_CSharp_FLAGS "/langversion:latest")

csharp_set_windows_forms_properties(
    ${MSRDPEX_MAIN_DLG_SOURCES}
    ${MSRDPEX_RDP_VIEW_SOURCES}
    ${MSRDPEX_PROPERTIES_SOURCES}
    Program.cs)

set_property(TARGET MsRdpEx_App PROPERTY 
    VS_DOTNET_REFERENCE_AxMSTSCLib "${CMAKE_SOURCE_DIR}/com/AxMSTSCLib.dll")

set_property(TARGET MsRdpEx_App PROPERTY 
    VS_DOTNET_REFERENCE_MSTSCLib "${CMAKE_SOURCE_DIR}/com/MSTSCLib.dll")

set_property(TARGET MsRdpEx_App PROPERTY VS_DOTNET_REFERENCES
    "Microsoft.CSharp"
    "System"
    "System.Core"
    "System.Data"
    "System.Drawing"
    "System.Windows.Forms"
    )
