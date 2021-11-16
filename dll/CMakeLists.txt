
include_directories("../utils")

add_library(MsRdpEx_Dll SHARED
    "../utils/File.c"
	MsRdpEx.c
	MsRdpEx.h
    MsRdpEx.def)

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Dll detours version.lib)
