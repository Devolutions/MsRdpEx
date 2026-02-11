
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
    "${MSRDPEX_INCLUDE_DIR}/Stopwatch.h"
    "${MSRDPEX_INCLUDE_DIR}/Stream.h"
    "${MSRDPEX_INCLUDE_DIR}/Detours.h"
    "${MSRDPEX_INCLUDE_DIR}/Memory.h"
    "${MSRDPEX_INCLUDE_DIR}/Pcap.h"
    "${MSRDPEX_INCLUDE_DIR}/ArrayList.h"
    "${MSRDPEX_INCLUDE_DIR}/HashTable.h"
    "${MSRDPEX_INCLUDE_DIR}/KeyMaps.h"
    "${MSRDPEX_INCLUDE_DIR}/Environment.h"
    "${MSRDPEX_INCLUDE_DIR}/NameResolver.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpFile.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpCoreApi.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpProcess.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpInstance.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpSettings.h"
    "${MSRDPEX_INCLUDE_DIR}/OutputMirror.h"
    "${MSRDPEX_INCLUDE_DIR}/VideoRecorder.h"
    "${MSRDPEX_INCLUDE_DIR}/RecordingManifest.h"
    "${MSRDPEX_INCLUDE_DIR}/NamedPipe.h"
    "${MSRDPEX_INCLUDE_DIR}/DvcPluginExtractor.h"
    "${MSRDPEX_INCLUDE_DIR}/MsRdpEx.h")

set(MSRDPEX_SOURCES
    ArrayList.c
    HashTable.c
    KeyMaps.cpp
    Environment.c
    NameResolver.c
    Stopwatch.c
    DpiHelper.cpp
    DpiHelper.h
    Sspi.cpp
    Stream.cpp
    Detours.cpp
    Memory.c
    File.c
    Log.c
    Paths.c
    String.c
    MsRdpClient.cpp
    MsRdpClient.h
    ApiHooks.cpp
    OutputMirror.c
    VideoRecorder.c
    RecordingManifest.c
    NamedPipe.c
    Pcap.cpp
    Bitmap.c
    WinMsg.c
    RdpFile.c
    RdpCoreApi.cpp
    RdpProcess.cpp
    RdpInstance.cpp
    RdpSettings.cpp
    RdpDvcClient.cpp
    RdpDvcClient.h
    TSObjects.cpp
    TSObjects.h
    DvcPluginExtractor.cpp
    MsRdpEx.cpp
    MsRdpEx.h
    AxDll.cpp)

set(MSRDPEX_AXHOST_SOURCE_DIR "${CMAKE_SOURCE_DIR}/dll/AxHost")
include_directories(${AXHOST_SOURCE_DIR})

set(MSRDPEX_AXHOST_SOURCES
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpAxHostWnd.cpp"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpAxHostWnd.h"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpEventSink.cpp"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpEventSink.h"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpOleSite.cpp"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpOleSite.h"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpComBase.h"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpWinMain.cpp"
    "${MSRDPEX_AXHOST_SOURCE_DIR}/RdpWinMain.h")

add_library(MsRdpEx_Dll SHARED
    ${MSRDPEX_SOURCES}
    ${MSRDPEX_HEADERS}
    ${MSRDPEX_RESOURCES}
    ${MSRDPEX_AXHOST_SOURCES}
    MsRdpEx.def)

set(MSRDPEX_LIBS
    detours
    version.lib
    userenv.lib
    user32.lib
    rpcrt4.lib
    ws2_32.lib
    secur32.lib
    credui.lib
    advapi32.lib
    comctl32.lib)

target_compile_options(MsRdpEx_Dll PRIVATE /W4)

target_link_libraries(MsRdpEx_Dll ${MSRDPEX_LIBS})

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

# Test executable for DVC plugin extractor
add_executable(TestDvcExtractor TestDvcExtractor.cpp)
target_link_libraries(TestDvcExtractor ole32.lib)
set_target_properties(TestDvcExtractor PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY_DEBUG "${CMAKE_BINARY_DIR}/Debug"
    RUNTIME_OUTPUT_DIRECTORY_RELEASE "${CMAKE_BINARY_DIR}/Release")
