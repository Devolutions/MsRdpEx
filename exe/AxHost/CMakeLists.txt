# RDP ActiveX Host

add_library(MsRdpEx_AxHost STATIC
    DpiHelper.cpp
    DpiHelper.h
	RdpAxHost.cpp
	RdpAxHost.h)

target_link_libraries(MsRdpEx_AxHost MsRdpEx_Core)
