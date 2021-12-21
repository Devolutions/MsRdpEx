
add_executable(MsRdpEx_Exe WIN32
	MsRdpEx.c
	MsRdpEx.h)

set_target_properties(MsRdpEx_Exe PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Exe MsRdpEx_Core)
target_link_libraries(MsRdpEx_Exe MsRdpEx_Dll)
