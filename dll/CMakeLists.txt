
set(MSRDPEX_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/include/MsRdpEx")

set(MSRDPEX_RESOURCES
    MsRdpEx.rc)

windows_rc_generate_version_info(
    NAME "MsRdpEx" TYPE "DLL"
    VERSION "${MSRDPEX_VERSION}"
    FILENAME "MsRdpEx.dll"
    VENDOR "${MSRDPEX_VENDOR}"
    COPYRIGHT "${MSRDPEX_COPYRIGHT}"
    OUTPUT ${CMAKE_CURRENT_BINARY_DIR}/version.rc)

source_group("Resources" FILES ${MSRDPEX_RESOURCES})

set(MSRDPEX_HEADERS
    "${MSRDPEX_INCLUDE_DIR}/Sspi.h"
    "${MSRDPEX_INCLUDE_DIR}/Stream.h"
    "${MSRDPEX_INCLUDE_DIR}/Memory.h"
    "${MSRDPEX_INCLUDE_DIR}/Pcap.h"
    "${MSRDPEX_INCLUDE_DIR}/ArrayList.h"
    "${MSRDPEX_INCLUDE_DIR}/HashTable.h"
    "${MSRDPEX_INCLUDE_DIR}/Environment.h"
    "${MSRDPEX_INCLUDE_DIR}/NameResolver.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpFile.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpCoreApi.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpProcess.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpInstance.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpSettings.h"
    "${MSRDPEX_INCLUDE_DIR}/OutputMirror.h"
    "${MSRDPEX_INCLUDE_DIR}/VideoRecorder.h"
    "${MSRDPEX_INCLUDE_DIR}/MsRdpEx.h")

set(MSRDPEX_SOURCES
    ArrayList.c
    HashTable.c
    Environment.c
    NameResolver.c
    Sspi.cpp
    Stream.cpp
    Memory.c
    File.c
    Log.c
    Paths.c
    String.c
    MsRdpClient.cpp
    MsRdpClient.h
    AxDll.cpp
    ApiHooks.cpp
    OutputMirror.c
    VideoRecorder.c
    Pcap.cpp
    Bitmap.c
    WinMsg.c
    RdpFile.c
    RdpCoreApi.cpp
    RdpProcess.cpp
    RdpInstance.cpp
    RdpSettings.cpp
    TSObjects.cpp
    TSObjects.h
    MsRdpEx.c
    MsRdpEx.h)

add_library(MsRdpEx_Dll SHARED
    ${MSRDPEX_SOURCES}
    ${MSRDPEX_HEADERS}
    ${MSRDPEX_RESOURCES}
    MsRdpEx.def)

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Dll detours
    version.lib userenv.lib user32.lib rpcrt4.lib ws2_32.lib secur32.lib credui.lib advapi32.lib)
