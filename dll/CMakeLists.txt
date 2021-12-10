
add_library(MsRdpEx_Dll SHARED
    MsRdpClient.cpp
    MsRdpClient.h
    AxDll.cpp
    ApiHooks.c
    Recorder.c
    Recorder.h
    Bitmap.c
    WinMsg.c
	MsRdpEx.c
	MsRdpEx.h
    MsRdpEx.def)

set_target_properties(MsRdpEx_Dll PROPERTIES OUTPUT_NAME "MsRdpEx")

target_link_libraries(MsRdpEx_Dll MsRdpEx_Core user32.lib ws2_32.lib secur32.lib advapi32.lib)
