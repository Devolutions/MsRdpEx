using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("809945CC-4B3B-4A92-A6B0-DBF9B5F2EF2D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAdvancedSettings : IDispatch
    {
        void SetCompress(int value);
        int GetCompress();
        void SetBitmapPeristence(int value);
        int GetBitmapPeristence();
        void SetAllowBackgroundInput(int value);
        int GetAllowBackgroundInput();
        void SetKeyBoardLayoutStr(ReadOnlyBinaryStringRef value);
        void SetPluginDlls(ReadOnlyBinaryStringRef value);
        void SetIconFile(ReadOnlyBinaryStringRef value);
        void SetIconIndex(int value);
        void SetContainerHandledFullScreen(int value);
        int GetContainerHandledFullScreen();
        void SetDisableRdpdr(int value);
        int GetDisableRdpdr();
    }

    [GeneratedComInterface]
    [Guid("3C65B4AB-12B3-465B-ACD4-B8DAD3BFF9E2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings : IMsTscAdvancedSettings
    {
        void SetSmoothScroll(int value);
        int GetSmoothScroll();
        void SetAcceleratorPassthrough(int value);
        int GetAcceleratorPassthrough();
        void SetShadowBitmap(int value);
        int GetShadowBitmap();
        void SetTransportType(int value);
        int GetTransportType();
        void SetSasSequence(int value);
        int GetSasSequence();
        void SetEncryptionEnabled(int value);
        int GetEncryptionEnabled();
        void SetDedicatedTerminal(int value);
        int GetDedicatedTerminal();
        void SetRDPPort(int value);
        int GetRDPPort();
        void SetEnableMouse(int value);
        int GetEnableMouse();
        void SetDisableCtrlAltDel(int value);
        int GetDisableCtrlAltDel();
        void SetEnableWindowsKey(int value);
        int GetEnableWindowsKey();
        void SetDoubleClickDetect(int value);
        int GetDoubleClickDetect();
        void SetMaximizeShell(int value);
        int GetMaximizeShell();
        void SetHotKeyFullScreen(int value);
        int GetHotKeyFullScreen();
        void SetHotKeyCtrlEsc(int value);
        int GetHotKeyCtrlEsc();
        void SetHotKeyAltEsc(int value);
        int GetHotKeyAltEsc();
        void SetHotKeyAltTab(int value);
        int GetHotKeyAltTab();
        void SetHotKeyAltShiftTab(int value);
        int GetHotKeyAltShiftTab();
        void SetHotKeyAltSpace(int value);
        int GetHotKeyAltSpace();
        void SetHotKeyCtrlAltDel(int value);
        int GetHotKeyCtrlAltDel();
        void SetOrderDrawThreshold(int value);
        int GetOrderDrawThreshold();
        void SetBitmapCacheSize(int value);
        int GetBitmapCacheSize();
        void SetBitmapVirtualCacheSize(int value);
        int GetBitmapVirtualCacheSize();
        void SetScaleBitmapCachesByBPP(int value);
        int GetScaleBitmapCachesByBPP();
        void SetNumBitmapCaches(int value);
        int GetNumBitmapCaches();
        void SetCachePersistenceActive(int value);
        int GetCachePersistenceActive();
        void SetPersistCacheDirectory(ReadOnlyBinaryStringRef value);
        void SetBrushSupportLevel(int value);
        int GetBrushSupportLevel();
        void SetMinInputSendInterval(int value);
        int GetMinInputSendInterval();
        void SetInputEventsAtOnce(int value);
        int GetInputEventsAtOnce();
        void SetMaxEventCount(int value);
        int GetMaxEventCount();
        void SetKeepAliveInterval(int value);
        int GetKeepAliveInterval();
        void SetShutdownTimeout(int value);
        int GetShutdownTimeout();
        void SetOverallConnectionTimeout(int value);
        int GetOverallConnectionTimeout();
        void SetSingleConnectionTimeout(int value);
        int GetSingleConnectionTimeout();
        void SetKeyboardType(int value);
        int GetKeyboardType();
        void SetKeyboardSubType(int value);
        int GetKeyboardSubType();
        void SetKeyboardFunctionKey(int value);
        int GetKeyboardFunctionKey();
        void SetWinceFixedPalette(int value);
        int GetWinceFixedPalette();
        void SetConnectToServerConsole(VariantBool value);
        VariantBool GetConnectToServerConsole();
        void SetBitmapPersistence(int value);
        int GetBitmapPersistence();
        void SetMinutesToIdleTimeout(int value);
        int GetMinutesToIdleTimeout();
        void SetSmartSizing(VariantBool value);
        VariantBool GetSmartSizing();
        void SetRdpdrLocalPrintingDocName(ReadOnlyBinaryStringRef value);
        BinaryString GetRdpdrLocalPrintingDocName();
        void SetRdpdrClipCleanTempDirString(ReadOnlyBinaryStringRef value);
        BinaryString GetRdpdrClipCleanTempDirString();
        void SetRdpdrClipPasteInfoString(ReadOnlyBinaryStringRef value);
        BinaryString GetRdpdrClipPasteInfoString();
        void SetClearTextPassword(ReadOnlyBinaryStringRef value);
        void SetDisplayConnectionBar(VariantBool value);
        VariantBool GetDisplayConnectionBar();
        void SetPinConnectionBar(VariantBool value);
        VariantBool GetPinConnectionBar();
        void SetGrabFocusOnConnect(VariantBool value);
        VariantBool GetGrabFocusOnConnect();
        void SetLoadBalanceInfo(ReadOnlyBinaryStringRef value);
        BinaryString GetLoadBalanceInfo();
        void SetRedirectDrives(VariantBool value);
        VariantBool GetRedirectDrives();
        void SetRedirectPrinters(VariantBool value);
        VariantBool GetRedirectPrinters();
        void SetRedirectPorts(VariantBool value);
        VariantBool GetRedirectPorts();
        void SetRedirectSmartCards(VariantBool value);
        VariantBool GetRedirectSmartCards();
        void SetBitmapVirtualCache16BppSize(int value);
        int GetBitmapVirtualCache16BppSize();
        void SetBitmapVirtualCache24BppSize(int value);
        int GetBitmapVirtualCache24BppSize();
        void SetPerformanceFlags(int value);
        int GetPerformanceFlags();
        void SetConnectWithEndpoint(in Variant value);
        void SetNotifyTSPublicKey(VariantBool value);
        VariantBool GetNotifyTSPublicKey();
    }

    [GeneratedComInterface]
    [Guid("9AC42117-2B76-4320-AA44-0E616AB8437B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings2 : IMsRdpClientAdvancedSettings
    {
        VariantBool GetCanAutoReconnect();
        void SetEnableAutoReconnect(VariantBool value);
        VariantBool GetEnableAutoReconnect();
        void SetMaxReconnectAttempts(int value);
        int GetMaxReconnectAttempts();
    }

    [GeneratedComInterface]
    [Guid("19CD856B-C542-4C53-ACEE-F127E3BE1A59")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings3 : IMsRdpClientAdvancedSettings2
    {
        void SetConnectionBarShowMinimizeButton(VariantBool value);
        VariantBool GetConnectionBarShowMinimizeButton();
        void SetConnectionBarShowRestoreButton(VariantBool value);
        VariantBool GetConnectionBarShowRestoreButton();
    }

    [GeneratedComInterface]
    [Guid("FBA7F64E-7345-4405-AE50-FA4A763DC0DE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings4 : IMsRdpClientAdvancedSettings3
    {
        void SetAuthenticationLevel(uint value);
        uint GetAuthenticationLevel();
    }

    [GeneratedComInterface]
    [Guid("FBA7F64E-6783-4405-DA45-FA4A763DABD0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings5 : IMsRdpClientAdvancedSettings4
    {
        void SetRedirectClipboard(VariantBool value);
        VariantBool GetRedirectClipboard();
        void SetAudioRedirectionMode(uint value);
        uint GetAudioRedirectionMode();
        void SetConnectionBarShowPinButton(VariantBool value);
        VariantBool GetConnectionBarShowPinButton();
        void SetPublicMode(VariantBool value);
        VariantBool GetPublicMode();
        void SetRedirectDevices(VariantBool value);
        VariantBool GetRedirectDevices();
        void SetRedirectPOSDevices(VariantBool value);
        VariantBool GetRedirectPOSDevices();
        void SetBitmapVirtualCache32BppSize(int value);
        int GetBitmapVirtualCache32BppSize();
    }

    [GeneratedComInterface]
    [Guid("222C4B5D-45D9-4DF0-A7C6-60CF9089D285")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings6 : IMsRdpClientAdvancedSettings5
    {
        void SetRelativeMouseMode(VariantBool value);
        VariantBool GetRelativeMouseMode();
        BinaryString GetAuthenticationServiceClass();
        void SetAuthenticationServiceClass(ReadOnlyBinaryStringRef value);
        BinaryString GetPCB();
        void SetPCB(ReadOnlyBinaryStringRef value);
        void SetHotKeyFocusReleaseLeft(int value);
        int GetHotKeyFocusReleaseLeft();
        void SetHotKeyFocusReleaseRight(int value);
        int GetHotKeyFocusReleaseRight();
        void SetEnableCredSspSupport(VariantBool value);
        VariantBool GetEnableCredSspSupport();
        uint GetAuthenticationType();
        void SetConnectToAdministerServer(VariantBool value);
        VariantBool GetConnectToAdministerServer();
    }

    [GeneratedComInterface]
    [Guid("26036036-4010-4578-8091-0DB9A1EDF9C3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings7 : IMsRdpClientAdvancedSettings6
    {
        void SetAudioCaptureRedirectionMode(VariantBool value);
        VariantBool GetAudioCaptureRedirectionMode();
        void SetVideoPlaybackMode(uint value);
        uint GetVideoPlaybackMode();
        void SetEnableSuperPan(VariantBool value);
        VariantBool GetEnableSuperPan();
        void SetSuperPanAccelerationFactor(uint value);
        uint GetSuperPanAccelerationFactor();
        void SetNegotiateSecurityLayer(VariantBool value);
        VariantBool GetNegotiateSecurityLayer();
        void SetAudioQualityMode(uint value);
        uint GetAudioQualityMode();
        void SetRedirectDirectX(VariantBool value);
        VariantBool GetRedirectDirectX();
        void SetNetworkConnectionType(uint value);
        uint GetNetworkConnectionType();
    }

    [GeneratedComInterface]
    [Guid("89ACB528-2557-4D16-8625-226A30E97E9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings8 : IMsRdpClientAdvancedSettings7
    {
        void SetBandwidthDetection(VariantBool value);

        VariantBool GetBandwidthDetection();

        void SetClientProtocolSpec(ClientSpec value);

        ClientSpec GetClientProtocolSpec();
    }

    public enum ClientSpec
    {
        FullMode,
        ThinClientMode,
        SmallCacheMode,
    }
}
