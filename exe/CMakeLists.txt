
include_directories("../utils")

add_executable(MsRdpEx_Exe
    "../utils/File.c"
	MsRdpEx.c
	MsRdpEx.h)

set_target_properties(MsRdpEx_Exe PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Exe detours version.lib)
