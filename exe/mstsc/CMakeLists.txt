# mstsc launcher executable

windows_rc_generate_version_info(
    NAME "mstsc" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "mstsc.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES mstsc.rc)

add_executable(mstsc WIN32
	../MsRdpEx.c
	mstsc.rc)

target_link_libraries(mstsc MsRdpEx_Dll)
