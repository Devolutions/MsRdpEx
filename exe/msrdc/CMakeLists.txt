# msrdc launcher executable

windows_rc_generate_version_info(
    NAME "msrdc" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "msrdc.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES msrdc.rc)

add_executable(msrdc WIN32
	../MsRdpEx.c
	msrdc.rc)

target_link_libraries(msrdc MsRdpEx_Dll)
