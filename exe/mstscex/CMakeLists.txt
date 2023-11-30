# mstscex launcher executable

windows_rc_generate_version_info(
    NAME "mstscex" TYPE "EXE"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "mstscex.exe"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES mstscex.rc)

add_executable(mstscex WIN32
	mstscex.cpp
	mstscex.rc)

target_link_libraries(mstscex MsRdpEx_Dll)
