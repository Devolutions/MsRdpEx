using System.Runtime.InteropServices;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    [ComImport]
    [Guid("327BB5CD-834E-4400-AEF2-B30E15E5D682")]
    public interface IMsTscAx_Redist
    {
        #region IMsTscAx_Redist
        #endregion
    }
    [ComImport]
    [Guid("8C11EFAE-92C3-11D1-BC1E-00C04FA31489")]
    public interface IMsTscAx : IMsTscAx_Redist
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] short Connected { get; }
        [DispId(12)] int DesktopWidth { set; get; }
        [DispId(13)] int DesktopHeight { set; get; }
        [DispId(16)] int StartConnected { set; get; }
        [DispId(17)] int HorizontalScrollBarVisible { get; }
        [DispId(18)] int VerticalScrollBarVisible { get; }
        [DispId(19)] BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] int CipherStrength { get; }
        [DispId(21)] BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] int SecuredSettingsEnabled { get; }
        [DispId(97)] IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] IMsTscDebug Debugger { get; }
        [DispId(30)] void Connect();
        [DispId(31)] void Disconnect();
        [DispId(33)] void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
    }
    [ComImport]
    [Guid("C9D65442-A0F9-45B2-8F73-D61D2DB8CBB6")]
    public interface IMsTscSecuredSettings
    {
        #region IMsTscSecuredSettings
        [DispId(1)] BinaryString StartProgram { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] BinaryString WorkDir { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] int FullScreen { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("809945CC-4B3B-4A92-A6B0-DBF9B5F2EF2D")]
    public interface IMsTscAdvancedSettings
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] int Compress { set; get; }
        [DispId(122)] int BitmapPeristence { set; get; }
        [DispId(161)] int allowBackgroundInput { set; get; }
        [DispId(162)] BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] int IconIndex { set; }
        [DispId(173)] int ContainerHandledFullScreen { set; get; }
        [DispId(174)] int DisableRdpdr { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("209D0EB9-6254-47B1-9033-A98DAE55BB27")]
    public interface IMsTscDebug
    {
        #region IMsTscDebug
        [DispId(200)] int HatchBitmapPDU { set; get; }
        [DispId(201)] int HatchSSBOrder { set; get; }
        [DispId(202)] int HatchMembltOrder { set; get; }
        [DispId(203)] int HatchIndexPDU { set; get; }
        [DispId(204)] int LabelMemblt { set; get; }
        [DispId(205)] int BitmapCacheMonitor { set; get; }
        [DispId(206)] int MallocFailuresPercent { set; get; }
        [DispId(207)] int MallocHugeFailuresPercent { set; get; }
        [DispId(208)] int NetThroughput { set; get; }
        [DispId(209)] BinaryString CLXCmdLine { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(210)] BinaryString CLXDll { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(211)] int RemoteProgramsHatchVisibleRegion { set; get; }
        [DispId(212)] int RemoteProgramsHatchVisibleNoDataRegion { set; get; }
        [DispId(213)] int RemoteProgramsHatchNonVisibleRegion { set; get; }
        [DispId(214)] int RemoteProgramsHatchWindow { set; get; }
        [DispId(215)] int RemoteProgramsStayConnectOnBadCaps { set; get; }
        [DispId(216)] uint ControlType { get; }
        [DispId(217)] bool DecodeGfx { set; }
        #endregion
    }
    [ComImport]
    [Guid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IMsTscAxEvents
    {
        #region IMsTscAxEvents
        [PreserveSig] [DispId(1)] void OnConnecting();
        [PreserveSig] [DispId(2)] void OnConnected();
        [PreserveSig] [DispId(3)] void OnLoginComplete();
        [PreserveSig] [DispId(4)] void OnDisconnected(int discReason);
        [PreserveSig] [DispId(5)] void OnEnterFullScreenMode();
        [PreserveSig] [DispId(6)] void OnLeaveFullScreenMode();
        [PreserveSig] [DispId(7)] void OnChannelReceivedData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString data);
        [PreserveSig] [DispId(8)] void OnRequestGoFullScreen();
        [PreserveSig] [DispId(9)] void OnRequestLeaveFullScreen();
        [PreserveSig] [DispId(10)] void OnFatalError(int errorCode);
        [PreserveSig] [DispId(11)] void OnWarning(int warningCode);
        [PreserveSig] [DispId(12)] void OnRemoteDesktopSizeChange(int width, int height);
        [PreserveSig] [DispId(13)] void OnIdleTimeoutNotification();
        [PreserveSig] [DispId(14)] void OnRequestContainerMinimize();
        [PreserveSig] [DispId(15)] void OnConfirmClose(out bool pfAllowClose);
        [PreserveSig] [DispId(16)] void OnReceivedTSPublicKey([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString publicKey, out bool pfContinueLogon);
        [PreserveSig] [DispId(17)] void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus);
        [PreserveSig] [DispId(18)] void OnAuthenticationWarningDisplayed();
        [PreserveSig] [DispId(19)] void OnAuthenticationWarningDismissed();
        [PreserveSig] [DispId(20)] void OnRemoteProgramResult([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable);
        [PreserveSig] [DispId(21)] void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation);
        [PreserveSig] [DispId(29)] void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute);
        [PreserveSig] [DispId(22)] void OnLogonError(int lError);
        [PreserveSig] [DispId(23)] void OnFocusReleased(int iDirection);
        [PreserveSig] [DispId(24)] void OnUserNameAcquired([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrUserName);
        [PreserveSig] [DispId(26)] void OnMouseInputModeChanged(bool fMouseModeRelative);
        [PreserveSig] [DispId(28)] void OnServiceMessageReceived([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString serviceMessage);
        [PreserveSig] [DispId(30)] void OnConnectionBarPullDown();
        [PreserveSig] [DispId(32)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig] [DispId(35)] void OnDevicesButtonPressed();
        [PreserveSig] [DispId(33)] void OnAutoReconnected();
        [PreserveSig] [DispId(34)] void OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount);
        #endregion
    }
    [ComImport]
    [Guid("92B4A539-7115-4B7C-A5A9-E5D9EFC2780A")]
    public interface IMsRdpClient : IMsTscAx
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] int ColorDepth { set; get; }
        [DispId(101)] IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] bool FullScreen { set; get; }
        [DispId(35)] void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] ControlCloseStatus RequestClose();
        #endregion
    }
    [ComImport]
    [Guid("3C65B4AB-12B3-465B-ACD4-B8DAD3BFF9E2")]
    public interface IMsRdpClientAdvancedSettings : IMsTscAdvancedSettings
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] int SmoothScroll { set; get; }
        [DispId(102)] int AcceleratorPassthrough { set; get; }
        [DispId(103)] int ShadowBitmap { set; get; }
        [DispId(104)] int TransportType { set; get; }
        [DispId(105)] int SasSequence { set; get; }
        [DispId(106)] int EncryptionEnabled { set; get; }
        [DispId(107)] int DedicatedTerminal { set; get; }
        [DispId(108)] int RDPPort { set; get; }
        [DispId(109)] int EnableMouse { set; get; }
        [DispId(110)] int DisableCtrlAltDel { set; get; }
        [DispId(111)] int EnableWindowsKey { set; get; }
        [DispId(112)] int DoubleClickDetect { set; get; }
        [DispId(113)] int MaximizeShell { set; get; }
        [DispId(114)] int HotKeyFullScreen { set; get; }
        [DispId(115)] int HotKeyCtrlEsc { set; get; }
        [DispId(116)] int HotKeyAltEsc { set; get; }
        [DispId(117)] int HotKeyAltTab { set; get; }
        [DispId(118)] int HotKeyAltShiftTab { set; get; }
        [DispId(119)] int HotKeyAltSpace { set; get; }
        [DispId(120)] int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] int orderDrawThreshold { set; get; }
        [DispId(124)] int BitmapCacheSize { set; get; }
        [DispId(125)] int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] int NumBitmapCaches { set; get; }
        [DispId(127)] int CachePersistenceActive { set; get; }
        [DispId(138)] BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] int brushSupportLevel { set; get; }
        [DispId(157)] int minInputSendInterval { set; get; }
        [DispId(158)] int InputEventsAtOnce { set; get; }
        [DispId(159)] int maxEventCount { set; get; }
        [DispId(160)] int keepAliveInterval { set; get; }
        [DispId(163)] int shutdownTimeout { set; get; }
        [DispId(164)] int overallConnectionTimeout { set; get; }
        [DispId(165)] int singleConnectionTimeout { set; get; }
        [DispId(166)] int KeyboardType { set; get; }
        [DispId(167)] int KeyboardSubType { set; get; }
        [DispId(168)] int KeyboardFunctionKey { set; get; }
        [DispId(169)] int WinceFixedPalette { set; get; }
        [DispId(178)] bool ConnectToServerConsole { set; get; }
        [DispId(182)] int BitmapPersistence { set; get; }
        [DispId(183)] int MinutesToIdleTimeout { set; get; }
        [DispId(184)] bool SmartSizing { set; get; }
        [DispId(185)] BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] bool DisplayConnectionBar { set; get; }
        [DispId(188)] bool PinConnectionBar { set; get; }
        [DispId(189)] bool GrabFocusOnConnect { set; get; }
        [DispId(190)] BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] bool RedirectDrives { set; get; }
        [DispId(192)] bool RedirectPrinters { set; get; }
        [DispId(193)] bool RedirectPorts { set; get; }
        [DispId(194)] bool RedirectSmartCards { set; get; }
        [DispId(195)] int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] int PerformanceFlags { set; get; }
        void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] bool NotifyTSPublicKey { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("605BEFCF-39C1-45CC-A811-068FB7BE346D")]
    public interface IMsRdpClientSecuredSettings : IMsTscSecuredSettings
    {
        #region IMsTscSecuredSettings
        [DispId(1)] new BinaryString StartProgram { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString WorkDir { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new int FullScreen { set; get; }
        #endregion
        #region IMsRdpClientSecuredSettings
        [DispId(4)] int KeyboardHookMode { set; get; }
        [DispId(5)] int AudioRedirectionMode { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("C1E6743A-41C1-4A74-832A-0DD06C1C7A0E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsTscNonScriptable
    {
        #region IMsTscNonScriptable
        [DispId(1)] BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        void ResetPassword();
        #endregion
    }
    [ComImport]
    [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable : IMsTscNonScriptable
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
    }
    [ComImport]
    [Guid("E7E17DC4-3B71-4BA7-A8E6-281FFADCA28F")]
    public interface IMsRdpClient2 : IMsRdpClient
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
    }
    [ComImport]
    [Guid("9AC42117-2B76-4320-AA44-0E616AB8437B")]
    public interface IMsRdpClientAdvancedSettings2 : IMsRdpClientAdvancedSettings
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] bool CanAutoReconnect { get; }
        [DispId(206)] bool EnableAutoReconnect { set; get; }
        [DispId(207)] int MaxReconnectAttempts { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("91B7CBC5-A72E-4FA0-9300-D647D7E897FF")]
    public interface IMsRdpClient3 : IMsRdpClient2
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
    }
    [ComImport]
    [Guid("19CD856B-C542-4C53-ACEE-F127E3BE1A59")]
    public interface IMsRdpClientAdvancedSettings3 : IMsRdpClientAdvancedSettings2
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("095E0738-D97D-488B-B9F6-DD0E8D66C0DE")]
    public interface IMsRdpClient4 : IMsRdpClient3
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
    }
    [ComImport]
    [Guid("FBA7F64E-7345-4405-AE50-FA4A763DC0DE")]
    public interface IMsRdpClientAdvancedSettings4 : IMsRdpClientAdvancedSettings3
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] new bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] new bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        [DispId(212)] uint AuthenticationLevel { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("17A5E535-4072-4FA4-AF32-C8D0D47345E9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable2 : IMsRdpClientNonScriptable
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] nint UIParentWindowHandle { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("4EB5335B-6429-477D-B922-E06A28ECD8BF")]
    public interface IMsRdpClient5 : IMsRdpClient4
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
    }
    [ComImport]
    [Guid("720298C0-A099-46F5-9F82-96921BAE4701")]
    public interface IMsRdpClientTransportSettings
    {
        #region IMsRdpClientTransportSettings
        [DispId(210)] BinaryString GatewayHostname { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(211)] uint GatewayUsageMethod { set; get; }
        [DispId(212)] uint GatewayProfileUsageMethod { set; get; }
        [DispId(213)] uint GatewayCredsSource { set; get; }
        [DispId(216)] uint GatewayUserSelectedCredsSource { set; get; }
        [DispId(214)] int GatewayIsSupported { get; }
        [DispId(215)] uint GatewayDefaultUsageMethod { get; }
        #endregion
    }
    [ComImport]
    [Guid("FBA7F64E-6783-4405-DA45-FA4A763DABD0")]
    public interface IMsRdpClientAdvancedSettings5 : IMsRdpClientAdvancedSettings4
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] new bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] new bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        [DispId(212)] new uint AuthenticationLevel { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        [DispId(213)] bool RedirectClipboard { set; get; }
        [DispId(215)] uint AudioRedirectionMode { set; get; }
        [DispId(216)] bool ConnectionBarShowPinButton { set; get; }
        [DispId(217)] bool PublicMode { set; get; }
        [DispId(218)] bool RedirectDevices { set; get; }
        [DispId(219)] bool RedirectPOSDevices { set; get; }
        [DispId(220)] int BitmapVirtualCache32BppSize { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("FDD029F9-467A-4C49-8529-64B521DBD1B4")]
    public interface ITSRemoteProgram
    {
        #region ITSRemoteProgram
        [DispId(200)] bool RemoteProgramMode { set; get; }
        [DispId(201)] void ServerStartProgram([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrExecutablePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrFilePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [ComImport]
    [Guid("D012AE6D-C19A-4BFE-B367-201F8911F134")]
    public interface IMsRdpClientShell
    {
        #region IMsRdpClientShell
        [DispId(201)] void Launch();
        [DispId(202)] BinaryString RdpFileContents { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(203)] void SetRdpProperty([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString szProperty, [MarshalAs(UnmanagedType.Struct)] object Value);
        [DispId(204)] [return: MarshalAs(UnmanagedType.Struct)] object GetRdpProperty([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString szProperty);
        [DispId(205)] bool IsRemoteProgramClientInstalled { get; }
        [DispId(211)] bool PublicMode { set; get; }
        [DispId(212)] void ShowTrustedSitesManagementDialog();
        #endregion
    }
    [ComImport]
    [Guid("B3378D90-0728-45C7-8ED7-B6159FB92219")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable3 : IMsRdpClientNonScriptable2
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] new nint UIParentWindowHandle { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable3
        [DispId(14)] bool ShowRedirectionWarningDialog { set; get; }
        [DispId(15)] bool PromptForCredentials { set; get; }
        [DispId(16)] bool NegotiateSecurityLayer { set; get; }
        [DispId(17)] bool EnableCredSspSupport { set; get; }
        [DispId(21)] bool RedirectDynamicDrives { set; get; }
        [DispId(20)] bool RedirectDynamicDevices { set; get; }
        [DispId(18)] IMsRdpDeviceCollection DeviceCollection { get; }
        [DispId(19)] IMsRdpDriveCollection DriveCollection { get; }
        [DispId(23)] bool WarnAboutSendingCredentials { set; get; }
        [DispId(22)] bool WarnAboutClipboardRedirection { set; get; }
        [DispId(24)] BinaryString ConnectionBarText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
    }
    [ComImport]
    [Guid("56540617-D281-488C-8738-6A8FDF64A118")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpDeviceCollection
    {
        #region IMsRdpDeviceCollection
        void RescanDevices(bool vboolDynRedir);
        IMsRdpDevice get_DeviceByIndex(uint index);
        IMsRdpDevice get_DeviceById([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString devInstanceId);
        [DispId(225)] uint DeviceCount { get; }
        #endregion
    }
    [ComImport]
    [Guid("7FF17599-DA2C-4677-AD35-F60C04FE1585")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpDriveCollection
    {
        #region IMsRdpDriveCollection
        void RescanDrives(bool vboolDynRedir);
        IMsRdpDrive get_DriveByIndex(uint index);
        [DispId(233)] uint DriveCount { get; }
        #endregion
    }
    [ComImport]
    [Guid("D43B7D80-8517-4B6D-9EAC-96AD6800D7F2")]
    public interface IMsRdpClient6 : IMsRdpClient5
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] new IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] new IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] new ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] new IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
        #region IMsRdpClient6
        [DispId(507)] IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        [DispId(506)] IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
    }
    [ComImport]
    [Guid("222C4B5D-45D9-4DF0-A7C6-60CF9089D285")]
    public interface IMsRdpClientAdvancedSettings6 : IMsRdpClientAdvancedSettings5
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] new bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] new bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        [DispId(212)] new uint AuthenticationLevel { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        [DispId(213)] new bool RedirectClipboard { set; get; }
        [DispId(215)] new uint AudioRedirectionMode { set; get; }
        [DispId(216)] new bool ConnectionBarShowPinButton { set; get; }
        [DispId(217)] new bool PublicMode { set; get; }
        [DispId(218)] new bool RedirectDevices { set; get; }
        [DispId(219)] new bool RedirectPOSDevices { set; get; }
        [DispId(220)] new int BitmapVirtualCache32BppSize { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        [DispId(221)] bool RelativeMouseMode { set; get; }
        [DispId(222)] BinaryString AuthenticationServiceClass { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(223)] BinaryString PCB { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(224)] int HotKeyFocusReleaseLeft { set; get; }
        [DispId(225)] int HotKeyFocusReleaseRight { set; get; }
        [DispId(17)] bool EnableCredSspSupport { set; get; }
        [DispId(226)] uint AuthenticationType { get; }
        [DispId(227)] bool ConnectToAdministerServer { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("67341688-D606-4C73-A5D2-2E0489009319")]
    public interface IMsRdpClientTransportSettings2 : IMsRdpClientTransportSettings
    {
        #region IMsRdpClientTransportSettings
        [DispId(210)] new BinaryString GatewayHostname { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(211)] new uint GatewayUsageMethod { set; get; }
        [DispId(212)] new uint GatewayProfileUsageMethod { set; get; }
        [DispId(213)] new uint GatewayCredsSource { set; get; }
        [DispId(216)] new uint GatewayUserSelectedCredsSource { set; get; }
        [DispId(214)] new int GatewayIsSupported { get; }
        [DispId(215)] new uint GatewayDefaultUsageMethod { get; }
        #endregion
        #region IMsRdpClientTransportSettings2
        [DispId(222)] uint GatewayCredSharing { set; get; }
        [DispId(217)] uint GatewayPreAuthRequirement { set; get; }
        [DispId(218)] BinaryString GatewayPreAuthServerAddr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(219)] BinaryString GatewaySupportUrl { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(220)] BinaryString GatewayEncryptedOtpCookie { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(221)] uint GatewayEncryptedOtpCookieSize { set; get; }
        [DispId(223)] BinaryString GatewayUsername { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(224)] BinaryString GatewayDomain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(225)] BinaryString GatewayPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
    }
    [ComImport]
    [Guid("F50FA8AA-1C7D-4F59-B15C-A90CACAE1FCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable4 : IMsRdpClientNonScriptable3
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] new nint UIParentWindowHandle { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable3
        [DispId(14)] new bool ShowRedirectionWarningDialog { set; get; }
        [DispId(15)] new bool PromptForCredentials { set; get; }
        [DispId(16)] new bool NegotiateSecurityLayer { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(21)] new bool RedirectDynamicDrives { set; get; }
        [DispId(20)] new bool RedirectDynamicDevices { set; get; }
        [DispId(18)] new IMsRdpDeviceCollection DeviceCollection { get; }
        [DispId(19)] new IMsRdpDriveCollection DriveCollection { get; }
        [DispId(23)] new bool WarnAboutSendingCredentials { set; get; }
        [DispId(22)] new bool WarnAboutClipboardRedirection { set; get; }
        [DispId(24)] new BinaryString ConnectionBarText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClientNonScriptable4
        [DispId(25)] RedirectionWarningType RedirectionWarningType { set; get; }
        [DispId(28)] bool MarkRdpSettingsSecure { set; get; }
        void set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert);
        [DispId(26)] object PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get; }
        [DispId(27)] bool WarnAboutPrinterRedirection { set; get; }
        [DispId(29)] bool AllowCredentialSaving { set; get; }
        [DispId(30)] bool PromptForCredsOnClient { set; get; }
        [DispId(31)] bool LaunchedViaClientShellInterface { set; get; }
        [DispId(32)] bool TrustedZoneSite { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("B2A5B5CE-3461-444A-91D4-ADD26D070638")]
    public interface IMsRdpClient7 : IMsRdpClient6
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] new IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] new IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] new ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] new IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
        #region IMsRdpClient6
        [DispId(507)] new IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        [DispId(506)] new IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
        #region IMsRdpClient7
        [DispId(600)] IMsRdpClientAdvancedSettings7 AdvancedSettings8 { get; }
        [DispId(601)] IMsRdpClientTransportSettings3 TransportSettings3 { get; }
        [DispId(602)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString GetStatusText(uint statusCode);
        [DispId(603)] IMsRdpClientSecuredSettings2 SecuredSettings3 { get; }
        [DispId(604)] ITSRemoteProgram2 RemoteProgram2 { get; }
        #endregion
    }
    [ComImport]
    [Guid("26036036-4010-4578-8091-0DB9A1EDF9C3")]
    public interface IMsRdpClientAdvancedSettings7 : IMsRdpClientAdvancedSettings6
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] new bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] new bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        [DispId(212)] new uint AuthenticationLevel { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        [DispId(213)] new bool RedirectClipboard { set; get; }
        [DispId(215)] new uint AudioRedirectionMode { set; get; }
        [DispId(216)] new bool ConnectionBarShowPinButton { set; get; }
        [DispId(217)] new bool PublicMode { set; get; }
        [DispId(218)] new bool RedirectDevices { set; get; }
        [DispId(219)] new bool RedirectPOSDevices { set; get; }
        [DispId(220)] new int BitmapVirtualCache32BppSize { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        [DispId(221)] new bool RelativeMouseMode { set; get; }
        [DispId(222)] new BinaryString AuthenticationServiceClass { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(223)] new BinaryString PCB { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(224)] new int HotKeyFocusReleaseLeft { set; get; }
        [DispId(225)] new int HotKeyFocusReleaseRight { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(226)] new uint AuthenticationType { get; }
        [DispId(227)] new bool ConnectToAdministerServer { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings7
        [DispId(228)] bool AudioCaptureRedirectionMode { set; get; }
        [DispId(229)] uint VideoPlaybackMode { set; get; }
        [DispId(230)] bool EnableSuperPan { set; get; }
        [DispId(231)] uint SuperPanAccelerationFactor { set; get; }
        [DispId(232)] bool NegotiateSecurityLayer { set; get; }
        [DispId(233)] uint AudioQualityMode { set; get; }
        [DispId(234)] bool RedirectDirectX { set; get; }
        [DispId(235)] uint NetworkConnectionType { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("3D5B21AC-748D-41DE-8F30-E15169586BD4")]
    public interface IMsRdpClientTransportSettings3 : IMsRdpClientTransportSettings2
    {
        #region IMsRdpClientTransportSettings
        [DispId(210)] new BinaryString GatewayHostname { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(211)] new uint GatewayUsageMethod { set; get; }
        [DispId(212)] new uint GatewayProfileUsageMethod { set; get; }
        [DispId(213)] new uint GatewayCredsSource { set; get; }
        [DispId(216)] new uint GatewayUserSelectedCredsSource { set; get; }
        [DispId(214)] new int GatewayIsSupported { get; }
        [DispId(215)] new uint GatewayDefaultUsageMethod { get; }
        #endregion
        #region IMsRdpClientTransportSettings2
        [DispId(222)] new uint GatewayCredSharing { set; get; }
        [DispId(217)] new uint GatewayPreAuthRequirement { set; get; }
        [DispId(218)] new BinaryString GatewayPreAuthServerAddr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(219)] new BinaryString GatewaySupportUrl { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(220)] new BinaryString GatewayEncryptedOtpCookie { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(221)] new uint GatewayEncryptedOtpCookieSize { set; get; }
        [DispId(223)] new BinaryString GatewayUsername { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(224)] new BinaryString GatewayDomain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(225)] new BinaryString GatewayPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
        #region IMsRdpClientTransportSettings3
        [DispId(226)] uint GatewayCredSourceCookie { set; get; }
        [DispId(227)] BinaryString GatewayAuthCookieServerAddr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(228)] BinaryString GatewayEncryptedAuthCookie { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(229)] uint GatewayEncryptedAuthCookieSize { set; get; }
        [DispId(230)] BinaryString GatewayAuthLoginPage { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
    }
    [ComImport]
    [Guid("25F2CE20-8B1D-4971-A7CD-549DAE201FC0")]
    public interface IMsRdpClientSecuredSettings2 : IMsRdpClientSecuredSettings
    {
        #region IMsTscSecuredSettings
        [DispId(1)] new BinaryString StartProgram { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString WorkDir { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new int FullScreen { set; get; }
        #endregion
        #region IMsRdpClientSecuredSettings
        [DispId(4)] new int KeyboardHookMode { set; get; }
        [DispId(5)] new int AudioRedirectionMode { set; get; }
        #endregion
        #region IMsRdpClientSecuredSettings2
        [DispId(6)] BinaryString PCB { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
    }
    [ComImport]
    [Guid("92C38A7D-241A-418C-9936-099872C9AF20")]
    public interface ITSRemoteProgram2 : ITSRemoteProgram
    {
        #region ITSRemoteProgram
        [DispId(200)] new bool RemoteProgramMode { set; get; }
        [DispId(201)] new void ServerStartProgram([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrExecutablePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrFilePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
        #region ITSRemoteProgram2
        [DispId(202)] BinaryString RemoteApplicationName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(203)] BinaryString RemoteApplicationProgram { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(204)] BinaryString RemoteApplicationArgs { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
    }
    [ComImport]
    [Guid("4F6996D5-D7B1-412C-B0FF-063718566907")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable5 : IMsRdpClientNonScriptable4
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] new nint UIParentWindowHandle { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable3
        [DispId(14)] new bool ShowRedirectionWarningDialog { set; get; }
        [DispId(15)] new bool PromptForCredentials { set; get; }
        [DispId(16)] new bool NegotiateSecurityLayer { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(21)] new bool RedirectDynamicDrives { set; get; }
        [DispId(20)] new bool RedirectDynamicDevices { set; get; }
        [DispId(18)] new IMsRdpDeviceCollection DeviceCollection { get; }
        [DispId(19)] new IMsRdpDriveCollection DriveCollection { get; }
        [DispId(23)] new bool WarnAboutSendingCredentials { set; get; }
        [DispId(22)] new bool WarnAboutClipboardRedirection { set; get; }
        [DispId(24)] new BinaryString ConnectionBarText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClientNonScriptable4
        [DispId(25)] new RedirectionWarningType RedirectionWarningType { set; get; }
        [DispId(28)] new bool MarkRdpSettingsSecure { set; get; }
        new void set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert);
        [DispId(26)] new object PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get; }
        [DispId(27)] new bool WarnAboutPrinterRedirection { set; get; }
        [DispId(29)] new bool AllowCredentialSaving { set; get; }
        [DispId(30)] new bool PromptForCredsOnClient { set; get; }
        [DispId(31)] new bool LaunchedViaClientShellInterface { set; get; }
        [DispId(32)] new bool TrustedZoneSite { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable5
        [DispId(33)] bool UseMultimon { set; get; }
        [DispId(35)] uint RemoteMonitorCount { get; }
        void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);
        [DispId(37)] bool RemoteMonitorLayoutMatchesLocal { get; }
        [DispId(38)] bool DisableConnectionBar { set; }
        [DispId(39)] bool DisableRemoteAppCapsCheck { set; get; }
        [DispId(40)] bool WarnAboutDirectXRedirection { set; get; }
        [DispId(41)] bool AllowPromptingForCredentials { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("FDD029F9-9574-4DEF-8529-64B521CCCAA4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpPreferredRedirectionInfo
    {
        #region IMsRdpPreferredRedirectionInfo
        [DispId(1)] bool UseRedirectionServerName { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("302D8188-0052-4807-806A-362B628F9AC5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpExtendedSettings
    {
        #region IMsRdpExtendedSettings
        void set_Property([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrPropertyName, [In] [MarshalAs(UnmanagedType.Struct)] ref object pValue);
        [return: MarshalAs(UnmanagedType.Struct)] object get_Property([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrPropertyName);
        #endregion
    }
    [ComImport]
    [Guid("4247E044-9271-43A9-BC49-E2AD9E855D62")]
    public interface IMsRdpClient8 : IMsRdpClient7
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] new IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] new IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] new ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] new IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
        #region IMsRdpClient6
        [DispId(507)] new IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        [DispId(506)] new IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
        #region IMsRdpClient7
        [DispId(600)] new IMsRdpClientAdvancedSettings7 AdvancedSettings8 { get; }
        [DispId(601)] new IMsRdpClientTransportSettings3 TransportSettings3 { get; }
        [DispId(602)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetStatusText(uint statusCode);
        [DispId(603)] new IMsRdpClientSecuredSettings2 SecuredSettings3 { get; }
        [DispId(604)] new ITSRemoteProgram2 RemoteProgram2 { get; }
        #endregion
        #region IMsRdpClient8
        [DispId(700)] void SendRemoteAction(RemoteSessionActionType actionType);
        [DispId(701)] IMsRdpClientAdvancedSettings8 AdvancedSettings9 { get; }
        [DispId(702)] ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);
        #endregion
    }
    [ComImport]
    [Guid("89ACB528-2557-4D16-8625-226A30E97E9A")]
    public interface IMsRdpClientAdvancedSettings8 : IMsRdpClientAdvancedSettings7
    {
        #region IMsTscAdvancedSettings
        [DispId(121)] new int Compress { set; get; }
        [DispId(122)] new int BitmapPeristence { set; get; }
        [DispId(161)] new int allowBackgroundInput { set; get; }
        [DispId(162)] new BinaryString KeyBoardLayoutStr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(170)] new BinaryString PluginDlls { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(171)] new BinaryString IconFile { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(172)] new int IconIndex { set; }
        [DispId(173)] new int ContainerHandledFullScreen { set; get; }
        [DispId(174)] new int DisableRdpdr { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings
        [DispId(101)] new int SmoothScroll { set; get; }
        [DispId(102)] new int AcceleratorPassthrough { set; get; }
        [DispId(103)] new int ShadowBitmap { set; get; }
        [DispId(104)] new int TransportType { set; get; }
        [DispId(105)] new int SasSequence { set; get; }
        [DispId(106)] new int EncryptionEnabled { set; get; }
        [DispId(107)] new int DedicatedTerminal { set; get; }
        [DispId(108)] new int RDPPort { set; get; }
        [DispId(109)] new int EnableMouse { set; get; }
        [DispId(110)] new int DisableCtrlAltDel { set; get; }
        [DispId(111)] new int EnableWindowsKey { set; get; }
        [DispId(112)] new int DoubleClickDetect { set; get; }
        [DispId(113)] new int MaximizeShell { set; get; }
        [DispId(114)] new int HotKeyFullScreen { set; get; }
        [DispId(115)] new int HotKeyCtrlEsc { set; get; }
        [DispId(116)] new int HotKeyAltEsc { set; get; }
        [DispId(117)] new int HotKeyAltTab { set; get; }
        [DispId(118)] new int HotKeyAltShiftTab { set; get; }
        [DispId(119)] new int HotKeyAltSpace { set; get; }
        [DispId(120)] new int HotKeyCtrlAltDel { set; get; }
        [DispId(123)] new int orderDrawThreshold { set; get; }
        [DispId(124)] new int BitmapCacheSize { set; get; }
        [DispId(125)] new int BitmapVirtualCacheSize { set; get; }
        [DispId(175)] new int ScaleBitmapCachesByBPP { set; get; }
        [DispId(126)] new int NumBitmapCaches { set; get; }
        [DispId(127)] new int CachePersistenceActive { set; get; }
        [DispId(138)] new BinaryString PersistCacheDirectory { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(156)] new int brushSupportLevel { set; get; }
        [DispId(157)] new int minInputSendInterval { set; get; }
        [DispId(158)] new int InputEventsAtOnce { set; get; }
        [DispId(159)] new int maxEventCount { set; get; }
        [DispId(160)] new int keepAliveInterval { set; get; }
        [DispId(163)] new int shutdownTimeout { set; get; }
        [DispId(164)] new int overallConnectionTimeout { set; get; }
        [DispId(165)] new int singleConnectionTimeout { set; get; }
        [DispId(166)] new int KeyboardType { set; get; }
        [DispId(167)] new int KeyboardSubType { set; get; }
        [DispId(168)] new int KeyboardFunctionKey { set; get; }
        [DispId(169)] new int WinceFixedPalette { set; get; }
        [DispId(178)] new bool ConnectToServerConsole { set; get; }
        [DispId(182)] new int BitmapPersistence { set; get; }
        [DispId(183)] new int MinutesToIdleTimeout { set; get; }
        [DispId(184)] new bool SmartSizing { set; get; }
        [DispId(185)] new BinaryString RdpdrLocalPrintingDocName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(201)] new BinaryString RdpdrClipCleanTempDirString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(202)] new BinaryString RdpdrClipPasteInfoString { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(186)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(187)] new bool DisplayConnectionBar { set; get; }
        [DispId(188)] new bool PinConnectionBar { set; get; }
        [DispId(189)] new bool GrabFocusOnConnect { set; get; }
        [DispId(190)] new BinaryString LoadBalanceInfo { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(191)] new bool RedirectDrives { set; get; }
        [DispId(192)] new bool RedirectPrinters { set; get; }
        [DispId(193)] new bool RedirectPorts { set; get; }
        [DispId(194)] new bool RedirectSmartCards { set; get; }
        [DispId(195)] new int BitmapVirtualCache16BppSize { set; get; }
        [DispId(196)] new int BitmapVirtualCache24BppSize { set; get; }
        [DispId(200)] new int PerformanceFlags { set; get; }
        new void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        [DispId(204)] new bool NotifyTSPublicKey { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        [DispId(205)] new bool CanAutoReconnect { get; }
        [DispId(206)] new bool EnableAutoReconnect { set; get; }
        [DispId(207)] new int MaxReconnectAttempts { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        [DispId(210)] new bool ConnectionBarShowMinimizeButton { set; get; }
        [DispId(211)] new bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        [DispId(212)] new uint AuthenticationLevel { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        [DispId(213)] new bool RedirectClipboard { set; get; }
        [DispId(215)] new uint AudioRedirectionMode { set; get; }
        [DispId(216)] new bool ConnectionBarShowPinButton { set; get; }
        [DispId(217)] new bool PublicMode { set; get; }
        [DispId(218)] new bool RedirectDevices { set; get; }
        [DispId(219)] new bool RedirectPOSDevices { set; get; }
        [DispId(220)] new int BitmapVirtualCache32BppSize { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        [DispId(221)] new bool RelativeMouseMode { set; get; }
        [DispId(222)] new BinaryString AuthenticationServiceClass { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(223)] new BinaryString PCB { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(224)] new int HotKeyFocusReleaseLeft { set; get; }
        [DispId(225)] new int HotKeyFocusReleaseRight { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(226)] new uint AuthenticationType { get; }
        [DispId(227)] new bool ConnectToAdministerServer { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings7
        [DispId(228)] new bool AudioCaptureRedirectionMode { set; get; }
        [DispId(229)] new uint VideoPlaybackMode { set; get; }
        [DispId(230)] new bool EnableSuperPan { set; get; }
        [DispId(231)] new uint SuperPanAccelerationFactor { set; get; }
        [DispId(232)] new bool NegotiateSecurityLayer { set; get; }
        [DispId(233)] new uint AudioQualityMode { set; get; }
        [DispId(234)] new bool RedirectDirectX { set; get; }
        [DispId(235)] new uint NetworkConnectionType { set; get; }
        #endregion
        #region IMsRdpClientAdvancedSettings8
        [DispId(236)] bool BandwidthDetection { set; get; }
        [DispId(237)] ClientSpec ClientProtocolSpec { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("28904001-04B6-436C-A55B-0AF1A0883DC9")]
    public interface IMsRdpClient9 : IMsRdpClient8
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] new IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] new IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] new ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] new IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
        #region IMsRdpClient6
        [DispId(507)] new IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        [DispId(506)] new IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
        #region IMsRdpClient7
        [DispId(600)] new IMsRdpClientAdvancedSettings7 AdvancedSettings8 { get; }
        [DispId(601)] new IMsRdpClientTransportSettings3 TransportSettings3 { get; }
        [DispId(602)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetStatusText(uint statusCode);
        [DispId(603)] new IMsRdpClientSecuredSettings2 SecuredSettings3 { get; }
        [DispId(604)] new ITSRemoteProgram2 RemoteProgram2 { get; }
        #endregion
        #region IMsRdpClient8
        [DispId(700)] new void SendRemoteAction(RemoteSessionActionType actionType);
        [DispId(701)] new IMsRdpClientAdvancedSettings8 AdvancedSettings9 { get; }
        [DispId(702)] new ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);
        #endregion
        #region IMsRdpClient9
        [DispId(800)] IMsRdpClientTransportSettings4 TransportSettings4 { get; }
        [DispId(801)] void SyncSessionDisplaySettings();
        [DispId(802)] void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor);
        [DispId(803)] void attachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        [DispId(804)] void detachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        #endregion
    }
    [ComImport]
    [Guid("011C3236-4D81-4515-9143-067AB630D299")]
    public interface IMsRdpClientTransportSettings4 : IMsRdpClientTransportSettings3
    {
        #region IMsRdpClientTransportSettings
        [DispId(210)] new BinaryString GatewayHostname { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(211)] new uint GatewayUsageMethod { set; get; }
        [DispId(212)] new uint GatewayProfileUsageMethod { set; get; }
        [DispId(213)] new uint GatewayCredsSource { set; get; }
        [DispId(216)] new uint GatewayUserSelectedCredsSource { set; get; }
        [DispId(214)] new int GatewayIsSupported { get; }
        [DispId(215)] new uint GatewayDefaultUsageMethod { get; }
        #endregion
        #region IMsRdpClientTransportSettings2
        [DispId(222)] new uint GatewayCredSharing { set; get; }
        [DispId(217)] new uint GatewayPreAuthRequirement { set; get; }
        [DispId(218)] new BinaryString GatewayPreAuthServerAddr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(219)] new BinaryString GatewaySupportUrl { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(220)] new BinaryString GatewayEncryptedOtpCookie { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(221)] new uint GatewayEncryptedOtpCookieSize { set; get; }
        [DispId(223)] new BinaryString GatewayUsername { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(224)] new BinaryString GatewayDomain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(225)] new BinaryString GatewayPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
        #region IMsRdpClientTransportSettings3
        [DispId(226)] new uint GatewayCredSourceCookie { set; get; }
        [DispId(227)] new BinaryString GatewayAuthCookieServerAddr { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(228)] new BinaryString GatewayEncryptedAuthCookie { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(229)] new uint GatewayEncryptedAuthCookieSize { set; get; }
        [DispId(230)] new BinaryString GatewayAuthLoginPage { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClientTransportSettings4
        [DispId(231)] uint GatewayBrokeringType { set; }
        #endregion
    }
    [ComImport]
    [Guid("7ED92C39-EB38-4927-A70A-708AC5A59321")]
    public interface IMsRdpClient10 : IMsRdpClient9
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        [DispId(1)] new BinaryString Server { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] new BinaryString Domain { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString UserName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString DisconnectedText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString ConnectingText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(6)] new short Connected { get; }
        [DispId(12)] new int DesktopWidth { set; get; }
        [DispId(13)] new int DesktopHeight { set; get; }
        [DispId(16)] new int StartConnected { set; get; }
        [DispId(17)] new int HorizontalScrollBarVisible { get; }
        [DispId(18)] new int VerticalScrollBarVisible { get; }
        [DispId(19)] new BinaryString FullScreenTitle { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(20)] new int CipherStrength { get; }
        [DispId(21)] new BinaryString Version { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(22)] new int SecuredSettingsEnabled { get; }
        [DispId(97)] new IMsTscSecuredSettings SecuredSettings { get; }
        [DispId(98)] new IMsTscAdvancedSettings AdvancedSettings { get; }
        [DispId(99)] new IMsTscDebug Debugger { get; }
        [DispId(30)] new void Connect();
        [DispId(31)] new void Disconnect();
        [DispId(33)] new void CreateVirtualChannels([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString newVal);
        [DispId(34)] new void SendOnVirtualChannel([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString ChanData);
        #endregion
        #region IMsRdpClient
        [DispId(100)] new int ColorDepth { set; get; }
        [DispId(101)] new IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        [DispId(102)] new IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        [DispId(103)] new ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        [DispId(104)] new bool FullScreen { set; get; }
        [DispId(35)] new void SetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, int chanOptions);
        [DispId(36)] new int GetVirtualChannelOptions([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName);
        [DispId(37)] new ControlCloseStatus RequestClose();
        #endregion
        #region IMsRdpClient2
        [DispId(200)] new IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        [DispId(201)] new BinaryString ConnectedStatusText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClient3
        [DispId(300)] new IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
        #region IMsRdpClient4
        [DispId(400)] new IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
        #region IMsRdpClient5
        [DispId(500)] new IMsRdpClientTransportSettings TransportSettings { get; }
        [DispId(502)] new IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        [DispId(503)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        [DispId(504)] new ITSRemoteProgram RemoteProgram { get; }
        [DispId(505)] new IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
        #region IMsRdpClient6
        [DispId(507)] new IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        [DispId(506)] new IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
        #region IMsRdpClient7
        [DispId(600)] new IMsRdpClientAdvancedSettings7 AdvancedSettings8 { get; }
        [DispId(601)] new IMsRdpClientTransportSettings3 TransportSettings3 { get; }
        [DispId(602)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] new BinaryString GetStatusText(uint statusCode);
        [DispId(603)] new IMsRdpClientSecuredSettings2 SecuredSettings3 { get; }
        [DispId(604)] new ITSRemoteProgram2 RemoteProgram2 { get; }
        #endregion
        #region IMsRdpClient8
        [DispId(700)] new void SendRemoteAction(RemoteSessionActionType actionType);
        [DispId(701)] new IMsRdpClientAdvancedSettings8 AdvancedSettings9 { get; }
        [DispId(702)] new ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);
        #endregion
        #region IMsRdpClient9
        [DispId(800)] new IMsRdpClientTransportSettings4 TransportSettings4 { get; }
        [DispId(801)] new void SyncSessionDisplaySettings();
        [DispId(802)] new void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor);
        [DispId(803)] new void attachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        [DispId(804)] new void detachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        #endregion
        #region IMsRdpClient10
        [DispId(900)] ITSRemoteProgram3 RemoteProgram3 { get; }
        #endregion
    }
    [ComImport]
    [Guid("4B84EA77-ACEA-418C-881A-4A8C28AB1510")]
    public interface ITSRemoteProgram3 : ITSRemoteProgram2
    {
        #region ITSRemoteProgram
        [DispId(200)] new bool RemoteProgramMode { set; get; }
        [DispId(201)] new void ServerStartProgram([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrExecutablePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrFilePath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
        #region ITSRemoteProgram2
        [DispId(202)] new BinaryString RemoteApplicationName { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(203)] new BinaryString RemoteApplicationProgram { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(204)] new BinaryString RemoteApplicationArgs { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        #endregion
        #region ITSRemoteProgram3
        [DispId(205)] void ServerStartApp([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrAppUserModelId, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [ComImport]
    [Guid("05293249-B28B-4BD8-BE64-1B2F496B910E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable6 : IMsRdpClientNonScriptable5
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] new nint UIParentWindowHandle { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable3
        [DispId(14)] new bool ShowRedirectionWarningDialog { set; get; }
        [DispId(15)] new bool PromptForCredentials { set; get; }
        [DispId(16)] new bool NegotiateSecurityLayer { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(21)] new bool RedirectDynamicDrives { set; get; }
        [DispId(20)] new bool RedirectDynamicDevices { set; get; }
        [DispId(18)] new IMsRdpDeviceCollection DeviceCollection { get; }
        [DispId(19)] new IMsRdpDriveCollection DriveCollection { get; }
        [DispId(23)] new bool WarnAboutSendingCredentials { set; get; }
        [DispId(22)] new bool WarnAboutClipboardRedirection { set; get; }
        [DispId(24)] new BinaryString ConnectionBarText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClientNonScriptable4
        [DispId(25)] new RedirectionWarningType RedirectionWarningType { set; get; }
        [DispId(28)] new bool MarkRdpSettingsSecure { set; get; }
        new void set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert);
        [DispId(26)] new object PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get; }
        [DispId(27)] new bool WarnAboutPrinterRedirection { set; get; }
        [DispId(29)] new bool AllowCredentialSaving { set; get; }
        [DispId(30)] new bool PromptForCredsOnClient { set; get; }
        [DispId(31)] new bool LaunchedViaClientShellInterface { set; get; }
        [DispId(32)] new bool TrustedZoneSite { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable5
        [DispId(33)] new bool UseMultimon { set; get; }
        [DispId(35)] new uint RemoteMonitorCount { get; }
        new void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);
        [DispId(37)] new bool RemoteMonitorLayoutMatchesLocal { get; }
        [DispId(38)] new bool DisableConnectionBar { set; }
        [DispId(39)] new bool DisableRemoteAppCapsCheck { set; get; }
        [DispId(40)] new bool WarnAboutDirectXRedirection { set; get; }
        [DispId(41)] new bool AllowPromptingForCredentials { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable6
        void SendLocation2D(double latitude, double longitude);
        void SendLocation3D(double latitude, double longitude, int altitude);
        #endregion
    }
    [ComImport]
    [Guid("71B4A60A-FE21-46D8-A39B-8E32BA0C5ECC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClientNonScriptable7 : IMsRdpClientNonScriptable6
    {
        #region IMsTscNonScriptable
        [DispId(1)] new BinaryString ClearTextPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; }
        [DispId(2)] new BinaryString PortablePassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] new BinaryString PortableSalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] new BinaryString BinaryPassword { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] new BinaryString BinarySalt { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        new void ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        new void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        new void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        [DispId(13)] new nint UIParentWindowHandle { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable3
        [DispId(14)] new bool ShowRedirectionWarningDialog { set; get; }
        [DispId(15)] new bool PromptForCredentials { set; get; }
        [DispId(16)] new bool NegotiateSecurityLayer { set; get; }
        [DispId(17)] new bool EnableCredSspSupport { set; get; }
        [DispId(21)] new bool RedirectDynamicDrives { set; get; }
        [DispId(20)] new bool RedirectDynamicDevices { set; get; }
        [DispId(18)] new IMsRdpDeviceCollection DeviceCollection { get; }
        [DispId(19)] new IMsRdpDriveCollection DriveCollection { get; }
        [DispId(23)] new bool WarnAboutSendingCredentials { set; get; }
        [DispId(22)] new bool WarnAboutClipboardRedirection { set; get; }
        [DispId(24)] new BinaryString ConnectionBarText { [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] set; [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        #endregion
        #region IMsRdpClientNonScriptable4
        [DispId(25)] new RedirectionWarningType RedirectionWarningType { set; get; }
        [DispId(28)] new bool MarkRdpSettingsSecure { set; get; }
        new void set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert);
        [DispId(26)] new object PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get; }
        [DispId(27)] new bool WarnAboutPrinterRedirection { set; get; }
        [DispId(29)] new bool AllowCredentialSaving { set; get; }
        [DispId(30)] new bool PromptForCredsOnClient { set; get; }
        [DispId(31)] new bool LaunchedViaClientShellInterface { set; get; }
        [DispId(32)] new bool TrustedZoneSite { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable5
        [DispId(33)] new bool UseMultimon { set; get; }
        [DispId(35)] new uint RemoteMonitorCount { get; }
        new void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);
        [DispId(37)] new bool RemoteMonitorLayoutMatchesLocal { get; }
        [DispId(38)] new bool DisableConnectionBar { set; }
        [DispId(39)] new bool DisableRemoteAppCapsCheck { set; get; }
        [DispId(40)] new bool WarnAboutDirectXRedirection { set; get; }
        [DispId(41)] new bool AllowPromptingForCredentials { set; get; }
        #endregion
        #region IMsRdpClientNonScriptable6
        new void SendLocation2D(double latitude, double longitude);
        new void SendLocation3D(double latitude, double longitude, int altitude);
        #endregion
        #region IMsRdpClientNonScriptable7
        [DispId(44)] IMsRdpCameraRedirConfigCollection CameraRedirConfigCollection { get; }
        void DisableDpiCursorScalingForProcess();
        [DispId(46)] IMsRdpClipboard Clipboard { get; }
        #endregion
    }
    [ComImport]
    [Guid("AE45252B-AAAB-4504-B681-649D6073A37A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpCameraRedirConfigCollection
    {
        #region IMsRdpCameraRedirConfigCollection
        void Rescan();
        [DispId(2)] uint Count { get; }
        IMsRdpCameraRedirConfig get_ByIndex(uint index);
        IMsRdpCameraRedirConfig get_BySymbolicLink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString SymbolicLink);
        IMsRdpCameraRedirConfig get_ByInstanceId([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString InstanceId);
        void AddConfig([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString SymbolicLink, bool fRedirected);
        [DispId(7)] bool RedirectByDefault { set; get; }
        [DispId(8)] bool EncodeVideo { set; get; }
        [DispId(9)] CameraRedirEncodingQuality EncodingQuality { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("2E769EE8-00C7-43DC-AFD9-235D75B72A40")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpClipboard
    {
        #region IMsRdpClipboard
        bool CanSyncLocalClipboardToRemoteSession();
        void SyncLocalClipboardToRemoteSession();
        bool CanSyncRemoteClipboardToLocalSession();
        void SyncRemoteClipboardToLocalSession();
        #endregion
    }
    [ComImport]
    [Guid("57D25668-625A-4905-BE4E-304CAA13F89C")]
    public interface IRemoteDesktopClient
    {
        #region IRemoteDesktopClient
        [DispId(701)] void Connect();
        [DispId(702)] void Disconnect();
        [DispId(703)] void Reconnect(uint width, uint height);
        [DispId(710)] IRemoteDesktopClientSettings Settings { get; }
        [DispId(711)] IRemoteDesktopClientActions Actions { get; }
        [DispId(712)] IRemoteDesktopClientTouchPointer TouchPointer { get; }
        [DispId(704)] void DeleteSavedCredentials([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString serverName);
        [DispId(705)] void UpdateSessionDisplaySettings(uint width, uint height);
        [DispId(706)] void attachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        [DispId(707)] void detachEvent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        #endregion
    }
    [ComImport]
    [Guid("48A0F2A7-2713-431F-BBAC-6F4558E7D64D")]
    public interface IRemoteDesktopClientSettings
    {
        #region IRemoteDesktopClientSettings
        [DispId(722)] void ApplySettings([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString RdpFileContents);
        [DispId(723)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString RetrieveSettings();
        [DispId(721)] [return: MarshalAs(UnmanagedType.Struct)] object GetRdpProperty([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString propertyName);
        [DispId(720)] void SetRdpProperty([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString propertyName, [MarshalAs(UnmanagedType.Struct)] object Value);
        #endregion
    }
    [ComImport]
    [Guid("7D54BC4E-1028-45D4-8B0A-B9B6BFFBA176")]
    public interface IRemoteDesktopClientActions
    {
        #region IRemoteDesktopClientActions
        [DispId(730)] void SuspendScreenUpdates();
        [DispId(731)] void ResumeScreenUpdates();
        [DispId(732)] void ExecuteRemoteAction(RemoteActionType remoteAction);
        [DispId(733)] [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString GetSnapshot(SnapshotEncodingType snapshotEncoding, SnapshotFormatType snapshotFormat, uint snapshotWidth, uint snapshotHeight);
        #endregion
    }
    [ComImport]
    [Guid("260EC22D-8CBC-44B5-9E88-2A37F6C93AE9")]
    public interface IRemoteDesktopClientTouchPointer
    {
        #region IRemoteDesktopClientTouchPointer
        [DispId(740)] bool Enabled { set; get; }
        [DispId(741)] bool EventsEnabled { set; get; }
        [DispId(742)] uint PointerSpeed { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("079863B7-6D47-4105-8BFE-0CDCB360E67D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IRemoteDesktopClientEvents
    {
        #region IRemoteDesktopClientEvents
        [PreserveSig] [DispId(750)] void OnConnecting();
        [PreserveSig] [DispId(751)] void OnConnected();
        [PreserveSig] [DispId(752)] void OnLoginCompleted();
        [PreserveSig] [DispId(753)] void OnDisconnected(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString disconnectErrorMessage);
        [PreserveSig] [DispId(754)] void OnStatusChanged(int statusCode, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString statusMessage);
        [PreserveSig] [DispId(755)] void OnAutoReconnecting(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount);
        [PreserveSig] [DispId(756)] void OnAutoReconnected();
        [PreserveSig] [DispId(757)] void OnDialogDisplaying();
        [PreserveSig] [DispId(758)] void OnDialogDismissed();
        [PreserveSig] [DispId(759)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig] [DispId(760)] void OnAdminMessageReceived([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString adminMessage);
        [PreserveSig] [DispId(761)] void OnKeyCombinationPressed(int keyCombination);
        [PreserveSig] [DispId(762)] void OnRemoteDesktopSizeChanged(int width, int height);
        [PreserveSig] [DispId(800)] void OnTouchPointerCursorMoved(int x, int y);
        #endregion
    }
    [ComImport]
    [Guid("60C3B9C8-9E92-4F5E-A3E7-604A912093EA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpDevice
    {
        #region IMsRdpDevice
        [DispId(222)] BinaryString DeviceInstanceId { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(220)] BinaryString FriendlyName { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(221)] BinaryString DeviceDescription { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(223)] bool RedirectionState { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("D28B5458-F694-47A8-8E61-40356A767E46")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpDrive
    {
        #region IMsRdpDrive
        [DispId(229)] BinaryString Name { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(230)] bool RedirectionState { set; get; }
        #endregion
    }
    [ComImport]
    [Guid("09750604-D625-47C1-9FCD-F09F735705D7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMsRdpCameraRedirConfig
    {
        #region IMsRdpCameraRedirConfig
        [DispId(1)] BinaryString FriendlyName { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(2)] BinaryString SymbolicLink { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(3)] BinaryString InstanceId { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(4)] BinaryString ParentInstanceId { [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] get; }
        [DispId(5)] bool Redirected { set; get; }
        [DispId(6)] bool DeviceExists { get; }
        #endregion
    }
}
