
add_library(MsRdpEx_Dll SHARED
    AxDll.c
    Hooks.c
	MsRdpEx.c
	MsRdpEx.h
    MsRdpEx.def)

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Dll MsRdpEx_Utils ws2_32.lib secur32.lib advapi32.lib)
