
add_library(MsRdpEx_Utils STATIC
    File.c
    Paths.c
    String.c)

target_link_libraries(MsRdpEx_Utils detours version.lib)
