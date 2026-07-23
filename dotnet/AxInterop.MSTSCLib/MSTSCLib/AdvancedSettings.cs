using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;

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
        void SetKeyBoardLayoutStr(BinaryStringRef value);
        void SetPluginDlls(BinaryStringRef value);
        void SetIconFile(BinaryStringRef value);
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
        void SetPersistCacheDirectory(BinaryStringRef value);
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
        void SetConnectToServerConsole([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetConnectToServerConsole();
        void SetBitmapPersistence(int value);
        int GetBitmapPersistence();
        void SetMinutesToIdleTimeout(int value);
        int GetMinutesToIdleTimeout();
        void SetSmartSizing([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetSmartSizing();
        void SetRdpdrLocalPrintingDocName(BinaryStringRef value);
        BinaryString GetRdpdrLocalPrintingDocName();
        void SetRdpdrClipCleanTempDirString(BinaryStringRef value);
        BinaryString GetRdpdrClipCleanTempDirString();
        void SetRdpdrClipPasteInfoString(BinaryStringRef value);
        BinaryString GetRdpdrClipPasteInfoString();
        void SetClearTextPassword(BinaryStringRef value);
        void SetDisplayConnectionBar([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetDisplayConnectionBar();
        void SetPinConnectionBar([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetPinConnectionBar();
        void SetGrabFocusOnConnect([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetGrabFocusOnConnect();
        void SetLoadBalanceInfo(BinaryStringRef value);
        BinaryString GetLoadBalanceInfo();
        void SetRedirectDrives([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectDrives();
        void SetRedirectPrinters([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectPrinters();
        void SetRedirectPorts([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectPorts();
        void SetRedirectSmartCards([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectSmartCards();
        void SetBitmapVirtualCache16BppSize(int value);
        int GetBitmapVirtualCache16BppSize();
        void SetBitmapVirtualCache24BppSize(int value);
        int GetBitmapVirtualCache24BppSize();
        void SetPerformanceFlags(int value);
        int GetPerformanceFlags();
        void SetConnectWithEndpoint([MarshalUsing(typeof(VariantMarshaller))] in object value);
        void SetNotifyTSPublicKey([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetNotifyTSPublicKey();
    }

    [GeneratedComInterface]
    [Guid("9AC42117-2B76-4320-AA44-0E616AB8437B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings2 : IMsRdpClientAdvancedSettings
    {
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetCanAutoReconnect();
        void SetEnableAutoReconnect([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetEnableAutoReconnect();
        void SetMaxReconnectAttempts(int value);
        int GetMaxReconnectAttempts();
    }

    [GeneratedComInterface]
    [Guid("19CD856B-C542-4C53-ACEE-F127E3BE1A59")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings3 : IMsRdpClientAdvancedSettings2
    {
        void SetConnectionBarShowMinimizeButton([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetConnectionBarShowMinimizeButton();
        void SetConnectionBarShowRestoreButton([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetConnectionBarShowRestoreButton();
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
        void SetRedirectClipboard([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectClipboard();
        void SetAudioRedirectionMode(uint value);
        uint GetAudioRedirectionMode();
        void SetConnectionBarShowPinButton([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetConnectionBarShowPinButton();
        void SetPublicMode([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetPublicMode();
        void SetRedirectDevices([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectDevices();
        void SetRedirectPOSDevices([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectPOSDevices();
        void SetBitmapVirtualCache32BppSize(int value);
        int GetBitmapVirtualCache32BppSize();
    }

    [GeneratedComInterface]
    [Guid("222C4B5D-45D9-4DF0-A7C6-60CF9089D285")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings6 : IMsRdpClientAdvancedSettings5
    {
        void SetRelativeMouseMode([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRelativeMouseMode();
        BinaryString GetAuthenticationServiceClass();
        void SetAuthenticationServiceClass(BinaryStringRef value);
        BinaryString GetPCB();
        void SetPCB(BinaryStringRef value);
        void SetHotKeyFocusReleaseLeft(int value);
        int GetHotKeyFocusReleaseLeft();
        void SetHotKeyFocusReleaseRight(int value);
        int GetHotKeyFocusReleaseRight();
        void SetEnableCredSspSupport([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetEnableCredSspSupport();
        uint GetAuthenticationType();
        void SetConnectToAdministerServer([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetConnectToAdministerServer();
    }

    [GeneratedComInterface]
    [Guid("26036036-4010-4578-8091-0DB9A1EDF9C3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings7 : IMsRdpClientAdvancedSettings6
    {
        void SetAudioCaptureRedirectionMode([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetAudioCaptureRedirectionMode();
        void SetVideoPlaybackMode(uint value);
        uint GetVideoPlaybackMode();
        void SetEnableSuperPan([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetEnableSuperPan();
        void SetSuperPanAccelerationFactor(uint value);
        uint GetSuperPanAccelerationFactor();
        void SetNegotiateSecurityLayer([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetNegotiateSecurityLayer();
        void SetAudioQualityMode(uint value);
        uint GetAudioQualityMode();
        void SetRedirectDirectX([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectDirectX();
        void SetNetworkConnectionType(uint value);
        uint GetNetworkConnectionType();
    }

    [GeneratedComInterface]
    [Guid("89ACB528-2557-4D16-8625-226A30E97E9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings8 : IMsRdpClientAdvancedSettings7
    {
        void SetBandwidthDetection([MarshalAs(UnmanagedType.VariantBool)] bool value);

        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetBandwidthDetection();

        void SetClientProtocolSpec(ClientSpec value);

        ClientSpec GetClientProtocolSpec();
    }

    //public enum ClientSpec
    //{
    //    FullMode,
    //    ThinClientMode,
    //    SmallCacheMode,
    //}
}
