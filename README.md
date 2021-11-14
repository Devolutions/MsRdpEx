# Microsoft RDP Extensions (msrdpex)

The official Microsoft RDP client is the only one with an exhaustive implementation of the [entire feature set](https://docs.microsoft.com/en-us/windows-server/remote/remote-desktop-services/clients/remote-desktop-app-compare). While FreeRDP is open source and available on all platforms, most vendors offering RDP support on Windows use the [Microsoft RDP ActiveX interface](https://docs.microsoft.com/en-us/windows/win32/termserv/remote-desktop-activex-control) instead. The solution is not to choose between FreeRDP and the Microsoft ActiveX, but to support them both while extending the Microsoft RDP client. In addition to this, Microsoft has been pushing for its "modern" remote desktop client (msrdc.exe) over the "classic" terminal services client (mstsc.exe) without providing the means to fully migrate to it. While both remote desktop clients use essentially the same RDP engine, they have multiple differences and limitations imposed by their user interfaces.

# Initial Goals

[We have recently confirmed](https://twitter.com/awakecoding/status/1459169582619496499) that the modern RDP client core (rdclientax.dll) is binary-compatible with the RDP client classic core (mstscax.dll). The first goal of this project is to make it easier for third-party vendors to selectively load the ActiveX interfaces from either one of them, at runtime, for projects written in C or C#.

The second goal is to make it easier to launch msrdc.exe as a replacement of mstsc.exe, and avoid many of the issues that affect the window size handling. The biggest problem is that msrdc.exe doesn't have a GUI, and the one provided with the modern remote desktop client can't be used for direct RDP connectivity.

## mstsc.exe / mstscax.dll

The Microsoft Terminal Services Client (mstsc.exe) is the built-in RDP client that most people in the industry are familiar with. It has a simple user interface that can be used to select a few RDP options and then connect to the RDP server of your choice. Since it is shipped as part of Windows, it has a long release cycle. The [Microsoft RDP ActiveX interface](https://docs.microsoft.com/en-us/windows/win32/termserv/remote-desktop-activex-control) is contained in mstscax.dll, the companion DLL to mstsc.exe.

## msrdc.exe / rdclientax.dll

The [Microsoft Remote Desktop Client (msrdc.exe)](https://docs.microsoft.com/en-us/windows-server/remote/remote-desktop-services/clients/windowsdesktop) is the modern RDP client that is now shipped separately from Windows, and therefore has a faster release cycle. Unlike mstsc.exe, msrdc.exe only accepts .RDP files to launch new connections - the GUI is provided by its companion executable, msrdcw.exe. Unfortunately, the msrdcw.exe user interface is extremely limited: you can only subscribe to Azure Virtual Desktop web feeds and launch RDP connections from it. The rdclientax.dll companion DLL contains the same ActiveX interfaces as mstscax.dll with the exception that it is shipped separately from Windows.
