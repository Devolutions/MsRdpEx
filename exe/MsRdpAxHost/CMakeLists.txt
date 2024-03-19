# MsRdpAxHost executable

windows_rc_generate_version_info(
    NAME "MsRdpAxHost" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "MsRdpAxHost.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES MsRdpAxHost.rc)

add_executable(MsRdpAxHost WIN32
    DpiHelper.cpp
    DpiHelper.h
	MsRdpAxHost.cpp
	MsRdpAxHost.rc)

target_link_libraries(MsRdpAxHost MsRdpEx_Dll)
