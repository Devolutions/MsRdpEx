
set(MSRDPEX_INCLUDE_DIR "${CMAKE_SOURCE_DIR}/include/MsRdpEx")

set(MSRDPEX_CORE_HEADERS
    "${MSRDPEX_INCLUDE_DIR}/ArrayList.h"
    "${MSRDPEX_INCLUDE_DIR}/HashTable.h"
    "${MSRDPEX_INCLUDE_DIR}/NameResolver.h"
    "${MSRDPEX_INCLUDE_DIR}/OutputMirror.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpFile.h"
    "${MSRDPEX_INCLUDE_DIR}/RdpSession.h"
    "${MSRDPEX_INCLUDE_DIR}/VideoRecorder.h"
    "${MSRDPEX_INCLUDE_DIR}/MsRdpEx.h")

add_library(MsRdpEx_Core STATIC
    ArrayList.c
    HashTable.c
    NameResolver.c
    File.c
    Log.c
    Paths.c
    RdpFile.c
    RdpSession.c
    String.c
    ${MSRDPEX_CORE_HEADERS})

target_link_libraries(MsRdpEx_Core detours version.lib)
