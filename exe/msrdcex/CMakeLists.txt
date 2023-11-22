# msrdcex launcher executable

windows_rc_generate_version_info(
    NAME "msrdcex" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "msrdcex.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES msrdcex.rc)

add_executable(msrdcex WIN32
	msrdcex.c
	msrdcex.rc)

target_link_libraries(msrdcex MsRdpEx_Dll)
