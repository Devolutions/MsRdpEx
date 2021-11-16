
add_library(MsRdpEx_Utils STATIC
    File.c)

target_link_libraries(MsRdpEx_Utils detours version.lib)
