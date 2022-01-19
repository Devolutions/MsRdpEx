
set(MSRDPEX_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/include/MsRdpEx")

set(MSRDPEX_HEADERS
    "${MSRDPEX_INCLUDE_DIR}/ArrayList.h"
    "${MSRDPEX_INCLUDE_DIR}/HashTable.h"
    "${MSRDPEX_INCLUDE_DIR}/Environment.h"
    "${MSRDPEX_INCLUDE_DIR}/NameResolver.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpFile.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpProcess.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpInstance.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpSettings.h"
    "${MSRDPEX_INCLUDE_DIR}/OutputMirror.h"
    "${MSRDPEX_INCLUDE_DIR}/VideoRecorder.h"
    "${MSRDPEX_INCLUDE_DIR}/MsRdpEx.h")

add_library(MsRdpEx_Dll SHARED
    ArrayList.c
    HashTable.c
    Environment.c
    NameResolver.c
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
    Bitmap.c
    WinMsg.c
    RdpFile.c
    RdpProcess.cpp
    RdpInstance.cpp
    RdpSettings.cpp
    TSObjects.cpp
    TSObjects.h
	MsRdpEx.c
	MsRdpEx.h
    MsRdpEx.def
    ${MSRDPEX_HEADERS})

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Dll detours version.lib userenv.lib user32.lib ws2_32.lib secur32.lib advapi32.lib)
