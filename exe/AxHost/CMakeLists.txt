# RDP ActiveX Host

add_library(MsRdpEx_AxHost STATIC
    DpiHelper.cpp
    DpiHelper.h
    RdpAxHostWnd.cpp
    RdpAxHostWnd.h
	RdpAxHost.cpp
	RdpAxHost.h)

target_link_libraries(MsRdpEx_AxHost MsRdpEx_Core)
