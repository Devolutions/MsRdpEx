# RDP ActiveX Host

add_library(MsRdpEx_AxHost STATIC
    DpiHelper.cpp
    DpiHelper.h
    RdpAxHostWnd.cpp
    RdpAxHostWnd.h
    RdpEventSink.cpp
    RdpEventSink.h
    RdpOleSite.cpp
    RdpOleSite.h
    RdpComBase.h)

target_link_libraries(MsRdpEx_AxHost MsRdpEx_Core)
