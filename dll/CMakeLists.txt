
include_directories("../utils")

add_library(MsRdpEx SHARED
    "../utils/MreFile.c"
	MsRdpEx.c
	MsRdpEx.h
    MsRdpEx.def)

target_link_libraries(MsRdpEx detours version.lib)
