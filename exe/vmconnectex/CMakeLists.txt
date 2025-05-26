# vmconnectex launcher executable

windows_rc_generate_version_info(
    NAME "vmconnectex" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "vmconnectex.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES vmconnectex.rc)

add_executable(vmconnectex WIN32
	vmconnectex.cpp
	vmconnectex.rc)

target_link_libraries(vmconnectex MsRdpEx_Dll)
