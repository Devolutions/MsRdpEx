using System.Runtime.InteropServices;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscAx_RedistProxy : IMsTscAx_Redist //  /* : IDispatch */
    {
        #region IMsTscAx_Redist
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscAxProxy : IMsTscAx //  : IMsTscAx_Redist
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscSecuredSettingsProxy : IMsTscSecuredSettings //  /* : IDispatch */
    {
        #region IMsTscSecuredSettings
        BinaryString IMsTscSecuredSettings.StartProgram { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetStartProgram(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetStartProgram(); }
        BinaryString IMsTscSecuredSettings.WorkDir { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetWorkDir(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetWorkDir(); }
        int IMsTscSecuredSettings.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetFullScreen(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscAdvancedSettingsProxy : IMsTscAdvancedSettings //  /* : IDispatch */
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscDebugProxy : IMsTscDebug //  /* : IDispatch */
    {
        #region IMsTscDebug
        int IMsTscDebug.HatchBitmapPDU { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetHatchBitmapPDU(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetHatchBitmapPDU(); }
        int IMsTscDebug.HatchSSBOrder { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetHatchSSBOrder(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetHatchSSBOrder(); }
        int IMsTscDebug.HatchMembltOrder { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetHatchMembltOrder(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetHatchMembltOrder(); }
        int IMsTscDebug.HatchIndexPDU { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetHatchIndexPDU(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetHatchIndexPDU(); }
        int IMsTscDebug.LabelMemblt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetLabelMemblt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetLabelMemblt(); }
        int IMsTscDebug.BitmapCacheMonitor { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetBitmapCacheMonitor(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetBitmapCacheMonitor(); }
        int IMsTscDebug.MallocFailuresPercent { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetMallocFailuresPercent(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetMallocFailuresPercent(); }
        int IMsTscDebug.MallocHugeFailuresPercent { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetMallocHugeFailuresPercent(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetMallocHugeFailuresPercent(); }
        int IMsTscDebug.NetThroughput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetNetThroughput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetNetThroughput(); }
        BinaryString IMsTscDebug.CLXCmdLine { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetCLXCmdLine(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetCLXCmdLine(); }
        BinaryString IMsTscDebug.CLXDll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetCLXDll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetCLXDll(); }
        int IMsTscDebug.RemoteProgramsHatchVisibleRegion { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetRemoteProgramsHatchVisibleRegion(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetRemoteProgramsHatchVisibleRegion(); }
        int IMsTscDebug.RemoteProgramsHatchVisibleNoDataRegion { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetRemoteProgramsHatchVisibleNoDataRegion(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetRemoteProgramsHatchVisibleNoDataRegion(); }
        int IMsTscDebug.RemoteProgramsHatchNonVisibleRegion { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetRemoteProgramsHatchNonVisibleRegion(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetRemoteProgramsHatchNonVisibleRegion(); }
        int IMsTscDebug.RemoteProgramsHatchWindow { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetRemoteProgramsHatchWindow(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetRemoteProgramsHatchWindow(); }
        int IMsTscDebug.RemoteProgramsStayConnectOnBadCaps { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetRemoteProgramsStayConnectOnBadCaps(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetRemoteProgramsStayConnectOnBadCaps(); }
        uint IMsTscDebug.ControlType { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).GetControlType(); }
        bool IMsTscDebug.DecodeGfx { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscDebug>(this).SetDecodeGfx(value); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscAxEventsProxy : IMsTscAxEvents //  /* : IDispatch */
    {
        #region IMsTscAxEvents
        [PreserveSig] void IMsTscAxEvents.OnConnecting() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnConnecting();
        [PreserveSig] void IMsTscAxEvents.OnConnected() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnConnected();
        [PreserveSig] void IMsTscAxEvents.OnLoginComplete() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnLoginComplete();
        [PreserveSig] void IMsTscAxEvents.OnDisconnected(int discReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnDisconnected(discReason);
        [PreserveSig] void IMsTscAxEvents.OnEnterFullScreenMode() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnEnterFullScreenMode();
        [PreserveSig] void IMsTscAxEvents.OnLeaveFullScreenMode() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnLeaveFullScreenMode();
        [PreserveSig] void IMsTscAxEvents.OnChannelReceivedData(BinaryString chanName, BinaryString data) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnChannelReceivedData(chanName, data);
        [PreserveSig] void IMsTscAxEvents.OnRequestGoFullScreen() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRequestGoFullScreen();
        [PreserveSig] void IMsTscAxEvents.OnRequestLeaveFullScreen() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRequestLeaveFullScreen();
        [PreserveSig] void IMsTscAxEvents.OnFatalError(int errorCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnFatalError(errorCode);
        [PreserveSig] void IMsTscAxEvents.OnWarning(int warningCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnWarning(warningCode);
        [PreserveSig] void IMsTscAxEvents.OnRemoteDesktopSizeChange(int width, int height) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRemoteDesktopSizeChange(width, height);
        [PreserveSig] void IMsTscAxEvents.OnIdleTimeoutNotification() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnIdleTimeoutNotification();
        [PreserveSig] void IMsTscAxEvents.OnRequestContainerMinimize() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRequestContainerMinimize();
        [PreserveSig] void IMsTscAxEvents.OnConfirmClose(out bool pfAllowClose) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnConfirmClose(out pfAllowClose);
        [PreserveSig] void IMsTscAxEvents.OnReceivedTSPublicKey(BinaryString publicKey, out bool pfContinueLogon) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnReceivedTSPublicKey(publicKey, out pfContinueLogon);
        [PreserveSig] void IMsTscAxEvents.OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnAutoReconnecting(disconnectReason, attemptCount, out pArcContinueStatus);
        [PreserveSig] void IMsTscAxEvents.OnAuthenticationWarningDisplayed() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnAuthenticationWarningDisplayed();
        [PreserveSig] void IMsTscAxEvents.OnAuthenticationWarningDismissed() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnAuthenticationWarningDismissed();
        [PreserveSig] void IMsTscAxEvents.OnRemoteProgramResult(BinaryString bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRemoteProgramResult(bstrRemoteProgram, lError, vbIsExecutable);
        [PreserveSig] void IMsTscAxEvents.OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRemoteProgramDisplayed(vbDisplayed, uDisplayInformation);
        [PreserveSig] void IMsTscAxEvents.OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnRemoteWindowDisplayed(vbDisplayed, hwnd, windowAttribute);
        [PreserveSig] void IMsTscAxEvents.OnLogonError(int lError) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnLogonError(lError);
        [PreserveSig] void IMsTscAxEvents.OnFocusReleased(int iDirection) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnFocusReleased(iDirection);
        [PreserveSig] void IMsTscAxEvents.OnUserNameAcquired(BinaryString bstrUserName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnUserNameAcquired(bstrUserName);
        [PreserveSig] void IMsTscAxEvents.OnMouseInputModeChanged(bool fMouseModeRelative) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnMouseInputModeChanged(fMouseModeRelative);
        [PreserveSig] void IMsTscAxEvents.OnServiceMessageReceived(BinaryString serviceMessage) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnServiceMessageReceived(serviceMessage);
        [PreserveSig] void IMsTscAxEvents.OnConnectionBarPullDown() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnConnectionBarPullDown();
        [PreserveSig] void IMsTscAxEvents.OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnNetworkStatusChanged(qualityLevel, bandwidth, rtt);
        [PreserveSig] void IMsTscAxEvents.OnDevicesButtonPressed() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnDevicesButtonPressed();
        [PreserveSig] void IMsTscAxEvents.OnAutoReconnected() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnAutoReconnected();
        [PreserveSig] void IMsTscAxEvents.OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAxEvents>(this).OnAutoReconnecting2(disconnectReason, networkAvailable, attemptCount, maxAttemptCount);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientProxy : IMsRdpClient //  : IMsTscAx
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettingsProxy : IMsRdpClientAdvancedSettings //  : IMsTscAdvancedSettings
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientSecuredSettingsProxy : IMsRdpClientSecuredSettings //  : IMsTscSecuredSettings
    {
        #region IMsTscSecuredSettings
        BinaryString IMsTscSecuredSettings.StartProgram { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetStartProgram(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetStartProgram(); }
        BinaryString IMsTscSecuredSettings.WorkDir { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetWorkDir(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetWorkDir(); }
        int IMsTscSecuredSettings.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetFullScreen(); }
        #endregion
        #region IMsRdpClientSecuredSettings
        int IMsRdpClientSecuredSettings.KeyboardHookMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).SetKeyboardHookMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).GetKeyboardHookMode(); }
        int IMsRdpClientSecuredSettings.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).GetAudioRedirectionMode(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsTscNonScriptableProxy : IMsTscNonScriptable //
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptableProxy : IMsRdpClientNonScriptable //  : IMsTscNonScriptable
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient2Proxy : IMsRdpClient2 //  : IMsRdpClient
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings2Proxy : IMsRdpClientAdvancedSettings2 //  : IMsRdpClientAdvancedSettings
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient3Proxy : IMsRdpClient3 //  : IMsRdpClient2
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings3Proxy : IMsRdpClientAdvancedSettings3 //  : IMsRdpClientAdvancedSettings2
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient4Proxy : IMsRdpClient4 //  : IMsRdpClient3
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings4Proxy : IMsRdpClientAdvancedSettings4 //  : IMsRdpClientAdvancedSettings3
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        uint IMsRdpClientAdvancedSettings4.AuthenticationLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).SetAuthenticationLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).GetAuthenticationLevel(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable2Proxy : IMsRdpClientNonScriptable2 //  : IMsRdpClientNonScriptable
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient5Proxy : IMsRdpClient5 //  : IMsRdpClient4
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientTransportSettingsProxy : IMsRdpClientTransportSettings //  /* : IDispatch */
    {
        #region IMsRdpClientTransportSettings
        BinaryString IMsRdpClientTransportSettings.GatewayHostname { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayHostname(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayHostname(); }
        uint IMsRdpClientTransportSettings.GatewayUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayProfileUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayProfileUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayProfileUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayCredsSource(); }
        uint IMsRdpClientTransportSettings.GatewayUserSelectedCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUserSelectedCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUserSelectedCredsSource(); }
        int IMsRdpClientTransportSettings.GatewayIsSupported { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayIsSupported(); }
        uint IMsRdpClientTransportSettings.GatewayDefaultUsageMethod { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayDefaultUsageMethod(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings5Proxy : IMsRdpClientAdvancedSettings5 //  : IMsRdpClientAdvancedSettings4
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        uint IMsRdpClientAdvancedSettings4.AuthenticationLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).SetAuthenticationLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).GetAuthenticationLevel(); }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        bool IMsRdpClientAdvancedSettings5.RedirectClipboard { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectClipboard(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectClipboard(); }
        uint IMsRdpClientAdvancedSettings5.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetAudioRedirectionMode(); }
        bool IMsRdpClientAdvancedSettings5.ConnectionBarShowPinButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetConnectionBarShowPinButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetConnectionBarShowPinButton(); }
        bool IMsRdpClientAdvancedSettings5.PublicMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetPublicMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetPublicMode(); }
        bool IMsRdpClientAdvancedSettings5.RedirectDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectDevices(); }
        bool IMsRdpClientAdvancedSettings5.RedirectPOSDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectPOSDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectPOSDevices(); }
        int IMsRdpClientAdvancedSettings5.BitmapVirtualCache32BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetBitmapVirtualCache32BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetBitmapVirtualCache32BppSize(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface ITSRemoteProgramProxy : ITSRemoteProgram //  /* : IDispatch */
    {
        #region ITSRemoteProgram
        bool ITSRemoteProgram.RemoteProgramMode { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).SetRemoteProgramMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).GetRemoteProgramMode(); }
        void ITSRemoteProgram.ServerStartProgram(BinaryString bstrExecutablePath, BinaryString bstrFilePath, BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer) => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).ServerStartProgram(bstrExecutablePath, bstrFilePath, bstrWorkingDirectory, vbExpandEnvVarInWorkingDirectoryOnServer, bstrArguments, vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientShellProxy : IMsRdpClientShell //  /* : IDispatch */
    {
        #region IMsRdpClientShell
        void IMsRdpClientShell.Launch() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).Launch();
        BinaryString IMsRdpClientShell.RdpFileContents { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).SetRdpFileContents(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).GetRdpFileContents(); }
        void IMsRdpClientShell.SetRdpProperty(BinaryString szProperty, [MarshalAs(UnmanagedType.Struct)] object Value) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).SetRdpProperty(szProperty, Value);
        [return: MarshalAs(UnmanagedType.Struct)] object IMsRdpClientShell.GetRdpProperty(BinaryString szProperty) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).GetRdpProperty(szProperty);
        bool IMsRdpClientShell.IsRemoteProgramClientInstalled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).GetIsRemoteProgramClientInstalled(); }
        bool IMsRdpClientShell.PublicMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).SetPublicMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).GetPublicMode(); }
        void IMsRdpClientShell.ShowTrustedSitesManagementDialog() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientShell>(this).ShowTrustedSitesManagementDialog();
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable3Proxy : IMsRdpClientNonScriptable3 //  : IMsRdpClientNonScriptable2
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
        #region IMsRdpClientNonScriptable3
        bool IMsRdpClientNonScriptable3.ShowRedirectionWarningDialog { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetShowRedirectionWarningDialog(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetShowRedirectionWarningDialog(); }
        bool IMsRdpClientNonScriptable3.PromptForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetPromptForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetPromptForCredentials(); }
        bool IMsRdpClientNonScriptable3.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetNegotiateSecurityLayer(); }
        bool IMsRdpClientNonScriptable3.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetEnableCredSspSupport(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDrives(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDevices(); }
        IMsRdpDeviceCollection IMsRdpClientNonScriptable3.DeviceCollection { get => ProxyObject.Pack<IMsRdpDeviceCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDeviceCollection()); }
        IMsRdpDriveCollection IMsRdpClientNonScriptable3.DriveCollection { get => ProxyObject.Pack<IMsRdpDriveCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDriveCollection()); }
        bool IMsRdpClientNonScriptable3.WarnAboutSendingCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutSendingCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutSendingCredentials(); }
        bool IMsRdpClientNonScriptable3.WarnAboutClipboardRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutClipboardRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutClipboardRedirection(); }
        BinaryString IMsRdpClientNonScriptable3.ConnectionBarText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetConnectionBarText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetConnectionBarText(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpDeviceCollectionProxy : IMsRdpDeviceCollection //
    {
        #region IMsRdpDeviceCollection
        void IMsRdpDeviceCollection.RescanDevices(bool vboolDynRedir) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDeviceCollection>(this).RescanDevices(vboolDynRedir);
        IMsRdpDevice IMsRdpDeviceCollection.get_DeviceByIndex(uint index) => ProxyObject.Pack<IMsRdpDevice>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDeviceCollection>(this).get_DeviceByIndex(index));
        IMsRdpDevice IMsRdpDeviceCollection.get_DeviceById(BinaryString devInstanceId) => ProxyObject.Pack<IMsRdpDevice>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDeviceCollection>(this).get_DeviceById(devInstanceId));
        uint IMsRdpDeviceCollection.DeviceCount { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDeviceCollection>(this).GetDeviceCount(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpDriveCollectionProxy : IMsRdpDriveCollection //
    {
        #region IMsRdpDriveCollection
        void IMsRdpDriveCollection.RescanDrives(bool vboolDynRedir) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDriveCollection>(this).RescanDrives(vboolDynRedir);
        IMsRdpDrive IMsRdpDriveCollection.get_DriveByIndex(uint index) => ProxyObject.Pack<IMsRdpDrive>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDriveCollection>(this).get_DriveByIndex(index));
        uint IMsRdpDriveCollection.DriveCount { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDriveCollection>(this).GetDriveCount(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient6Proxy : IMsRdpClient6 //  : IMsRdpClient5
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 IMsRdpClient6.AdvancedSettings7 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings6>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetAdvancedSettings7()); }
        IMsRdpClientTransportSettings2 IMsRdpClient6.TransportSettings2 { get => ProxyObject.Pack<IMsRdpClientTransportSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetTransportSettings2()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings6Proxy : IMsRdpClientAdvancedSettings6 //  : IMsRdpClientAdvancedSettings5
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        uint IMsRdpClientAdvancedSettings4.AuthenticationLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).SetAuthenticationLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).GetAuthenticationLevel(); }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        bool IMsRdpClientAdvancedSettings5.RedirectClipboard { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectClipboard(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectClipboard(); }
        uint IMsRdpClientAdvancedSettings5.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetAudioRedirectionMode(); }
        bool IMsRdpClientAdvancedSettings5.ConnectionBarShowPinButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetConnectionBarShowPinButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetConnectionBarShowPinButton(); }
        bool IMsRdpClientAdvancedSettings5.PublicMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetPublicMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetPublicMode(); }
        bool IMsRdpClientAdvancedSettings5.RedirectDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectDevices(); }
        bool IMsRdpClientAdvancedSettings5.RedirectPOSDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectPOSDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectPOSDevices(); }
        int IMsRdpClientAdvancedSettings5.BitmapVirtualCache32BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetBitmapVirtualCache32BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetBitmapVirtualCache32BppSize(); }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        bool IMsRdpClientAdvancedSettings6.RelativeMouseMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetRelativeMouseMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetRelativeMouseMode(); }
        BinaryString IMsRdpClientAdvancedSettings6.AuthenticationServiceClass { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationServiceClass(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetAuthenticationServiceClass(value); }
        BinaryString IMsRdpClientAdvancedSettings6.PCB { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetPCB(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetPCB(value); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseLeft { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseLeft(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseLeft(); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseRight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseRight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseRight(); }
        bool IMsRdpClientAdvancedSettings6.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetEnableCredSspSupport(); }
        uint IMsRdpClientAdvancedSettings6.AuthenticationType { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationType(); }
        bool IMsRdpClientAdvancedSettings6.ConnectToAdministerServer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetConnectToAdministerServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetConnectToAdministerServer(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientTransportSettings2Proxy : IMsRdpClientTransportSettings2 //  : IMsRdpClientTransportSettings
    {
        #region IMsRdpClientTransportSettings
        BinaryString IMsRdpClientTransportSettings.GatewayHostname { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayHostname(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayHostname(); }
        uint IMsRdpClientTransportSettings.GatewayUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayProfileUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayProfileUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayProfileUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayCredsSource(); }
        uint IMsRdpClientTransportSettings.GatewayUserSelectedCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUserSelectedCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUserSelectedCredsSource(); }
        int IMsRdpClientTransportSettings.GatewayIsSupported { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayIsSupported(); }
        uint IMsRdpClientTransportSettings.GatewayDefaultUsageMethod { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayDefaultUsageMethod(); }
        #endregion
        #region IMsRdpClientTransportSettings2
        uint IMsRdpClientTransportSettings2.GatewayCredSharing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayCredSharing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayCredSharing(); }
        uint IMsRdpClientTransportSettings2.GatewayPreAuthRequirement { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthRequirement(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthRequirement(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPreAuthServerAddr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthServerAddr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthServerAddr(); }
        BinaryString IMsRdpClientTransportSettings2.GatewaySupportUrl { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewaySupportUrl(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewaySupportUrl(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookie(); }
        uint IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookieSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookieSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookieSize(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayUsername { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayUsername(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayUsername(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayDomain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayDomain(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPassword(value); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable4Proxy : IMsRdpClientNonScriptable4 //  : IMsRdpClientNonScriptable3
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
        #region IMsRdpClientNonScriptable3
        bool IMsRdpClientNonScriptable3.ShowRedirectionWarningDialog { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetShowRedirectionWarningDialog(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetShowRedirectionWarningDialog(); }
        bool IMsRdpClientNonScriptable3.PromptForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetPromptForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetPromptForCredentials(); }
        bool IMsRdpClientNonScriptable3.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetNegotiateSecurityLayer(); }
        bool IMsRdpClientNonScriptable3.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetEnableCredSspSupport(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDrives(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDevices(); }
        IMsRdpDeviceCollection IMsRdpClientNonScriptable3.DeviceCollection { get => ProxyObject.Pack<IMsRdpDeviceCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDeviceCollection()); }
        IMsRdpDriveCollection IMsRdpClientNonScriptable3.DriveCollection { get => ProxyObject.Pack<IMsRdpDriveCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDriveCollection()); }
        bool IMsRdpClientNonScriptable3.WarnAboutSendingCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutSendingCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutSendingCredentials(); }
        bool IMsRdpClientNonScriptable3.WarnAboutClipboardRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutClipboardRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutClipboardRedirection(); }
        BinaryString IMsRdpClientNonScriptable3.ConnectionBarText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetConnectionBarText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetConnectionBarText(); }
        #endregion
        #region IMsRdpClientNonScriptable4
        RedirectionWarningType IMsRdpClientNonScriptable4.RedirectionWarningType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetRedirectionWarningType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetRedirectionWarningType(); }
        bool IMsRdpClientNonScriptable4.MarkRdpSettingsSecure { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetMarkRdpSettingsSecure(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetMarkRdpSettingsSecure(); }
        void IMsRdpClientNonScriptable4.set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).set_PublisherCertificateChain(in pVarCert);
        object IMsRdpClientNonScriptable4.PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPublisherCertificateChain(); }
        bool IMsRdpClientNonScriptable4.WarnAboutPrinterRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetWarnAboutPrinterRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetWarnAboutPrinterRedirection(); }
        bool IMsRdpClientNonScriptable4.AllowCredentialSaving { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetAllowCredentialSaving(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetAllowCredentialSaving(); }
        bool IMsRdpClientNonScriptable4.PromptForCredsOnClient { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetPromptForCredsOnClient(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPromptForCredsOnClient(); }
        bool IMsRdpClientNonScriptable4.LaunchedViaClientShellInterface { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetLaunchedViaClientShellInterface(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetLaunchedViaClientShellInterface(); }
        bool IMsRdpClientNonScriptable4.TrustedZoneSite { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetTrustedZoneSite(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetTrustedZoneSite(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient7Proxy : IMsRdpClient7 //  : IMsRdpClient6
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 IMsRdpClient6.AdvancedSettings7 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings6>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetAdvancedSettings7()); }
        IMsRdpClientTransportSettings2 IMsRdpClient6.TransportSettings2 { get => ProxyObject.Pack<IMsRdpClientTransportSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetTransportSettings2()); }
        #endregion
        #region IMsRdpClient7
        IMsRdpClientAdvancedSettings7 IMsRdpClient7.AdvancedSettings8 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings7>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetAdvancedSettings8()); }
        IMsRdpClientTransportSettings3 IMsRdpClient7.TransportSettings3 { get => ProxyObject.Pack<IMsRdpClientTransportSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetTransportSettings3()); }
        BinaryString IMsRdpClient7.GetStatusText(uint statusCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetStatusText(statusCode);
        IMsRdpClientSecuredSettings2 IMsRdpClient7.SecuredSettings3 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetSecuredSettings3()); }
        ITSRemoteProgram2 IMsRdpClient7.RemoteProgram2 { get => ProxyObject.Pack<ITSRemoteProgram2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetRemoteProgram2()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings7Proxy : IMsRdpClientAdvancedSettings7 //  : IMsRdpClientAdvancedSettings6
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        uint IMsRdpClientAdvancedSettings4.AuthenticationLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).SetAuthenticationLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).GetAuthenticationLevel(); }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        bool IMsRdpClientAdvancedSettings5.RedirectClipboard { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectClipboard(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectClipboard(); }
        uint IMsRdpClientAdvancedSettings5.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetAudioRedirectionMode(); }
        bool IMsRdpClientAdvancedSettings5.ConnectionBarShowPinButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetConnectionBarShowPinButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetConnectionBarShowPinButton(); }
        bool IMsRdpClientAdvancedSettings5.PublicMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetPublicMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetPublicMode(); }
        bool IMsRdpClientAdvancedSettings5.RedirectDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectDevices(); }
        bool IMsRdpClientAdvancedSettings5.RedirectPOSDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectPOSDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectPOSDevices(); }
        int IMsRdpClientAdvancedSettings5.BitmapVirtualCache32BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetBitmapVirtualCache32BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetBitmapVirtualCache32BppSize(); }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        bool IMsRdpClientAdvancedSettings6.RelativeMouseMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetRelativeMouseMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetRelativeMouseMode(); }
        BinaryString IMsRdpClientAdvancedSettings6.AuthenticationServiceClass { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationServiceClass(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetAuthenticationServiceClass(value); }
        BinaryString IMsRdpClientAdvancedSettings6.PCB { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetPCB(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetPCB(value); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseLeft { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseLeft(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseLeft(); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseRight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseRight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseRight(); }
        bool IMsRdpClientAdvancedSettings6.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetEnableCredSspSupport(); }
        uint IMsRdpClientAdvancedSettings6.AuthenticationType { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationType(); }
        bool IMsRdpClientAdvancedSettings6.ConnectToAdministerServer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetConnectToAdministerServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetConnectToAdministerServer(); }
        #endregion
        #region IMsRdpClientAdvancedSettings7
        bool IMsRdpClientAdvancedSettings7.AudioCaptureRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetAudioCaptureRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetAudioCaptureRedirectionMode(); }
        uint IMsRdpClientAdvancedSettings7.VideoPlaybackMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetVideoPlaybackMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetVideoPlaybackMode(); }
        bool IMsRdpClientAdvancedSettings7.EnableSuperPan { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetEnableSuperPan(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetEnableSuperPan(); }
        uint IMsRdpClientAdvancedSettings7.SuperPanAccelerationFactor { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetSuperPanAccelerationFactor(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetSuperPanAccelerationFactor(); }
        bool IMsRdpClientAdvancedSettings7.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetNegotiateSecurityLayer(); }
        uint IMsRdpClientAdvancedSettings7.AudioQualityMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetAudioQualityMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetAudioQualityMode(); }
        bool IMsRdpClientAdvancedSettings7.RedirectDirectX { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetRedirectDirectX(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetRedirectDirectX(); }
        uint IMsRdpClientAdvancedSettings7.NetworkConnectionType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetNetworkConnectionType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetNetworkConnectionType(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientTransportSettings3Proxy : IMsRdpClientTransportSettings3 //  : IMsRdpClientTransportSettings2
    {
        #region IMsRdpClientTransportSettings
        BinaryString IMsRdpClientTransportSettings.GatewayHostname { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayHostname(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayHostname(); }
        uint IMsRdpClientTransportSettings.GatewayUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayProfileUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayProfileUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayProfileUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayCredsSource(); }
        uint IMsRdpClientTransportSettings.GatewayUserSelectedCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUserSelectedCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUserSelectedCredsSource(); }
        int IMsRdpClientTransportSettings.GatewayIsSupported { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayIsSupported(); }
        uint IMsRdpClientTransportSettings.GatewayDefaultUsageMethod { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayDefaultUsageMethod(); }
        #endregion
        #region IMsRdpClientTransportSettings2
        uint IMsRdpClientTransportSettings2.GatewayCredSharing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayCredSharing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayCredSharing(); }
        uint IMsRdpClientTransportSettings2.GatewayPreAuthRequirement { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthRequirement(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthRequirement(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPreAuthServerAddr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthServerAddr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthServerAddr(); }
        BinaryString IMsRdpClientTransportSettings2.GatewaySupportUrl { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewaySupportUrl(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewaySupportUrl(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookie(); }
        uint IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookieSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookieSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookieSize(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayUsername { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayUsername(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayUsername(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayDomain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayDomain(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPassword(value); }
        #endregion
        #region IMsRdpClientTransportSettings3
        uint IMsRdpClientTransportSettings3.GatewayCredSourceCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayCredSourceCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayCredSourceCookie(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayAuthCookieServerAddr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayAuthCookieServerAddr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayAuthCookieServerAddr(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayEncryptedAuthCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayEncryptedAuthCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayEncryptedAuthCookie(); }
        uint IMsRdpClientTransportSettings3.GatewayEncryptedAuthCookieSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayEncryptedAuthCookieSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayEncryptedAuthCookieSize(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayAuthLoginPage { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayAuthLoginPage(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayAuthLoginPage(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientSecuredSettings2Proxy : IMsRdpClientSecuredSettings2 //  : IMsRdpClientSecuredSettings
    {
        #region IMsTscSecuredSettings
        BinaryString IMsTscSecuredSettings.StartProgram { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetStartProgram(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetStartProgram(); }
        BinaryString IMsTscSecuredSettings.WorkDir { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetWorkDir(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetWorkDir(); }
        int IMsTscSecuredSettings.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscSecuredSettings>(this).GetFullScreen(); }
        #endregion
        #region IMsRdpClientSecuredSettings
        int IMsRdpClientSecuredSettings.KeyboardHookMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).SetKeyboardHookMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).GetKeyboardHookMode(); }
        int IMsRdpClientSecuredSettings.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings>(this).GetAudioRedirectionMode(); }
        #endregion
        #region IMsRdpClientSecuredSettings2
        BinaryString IMsRdpClientSecuredSettings2.PCB { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings2>(this).GetPCB(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientSecuredSettings2>(this).SetPCB(value); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface ITSRemoteProgram2Proxy : ITSRemoteProgram2 //  : ITSRemoteProgram
    {
        #region ITSRemoteProgram
        bool ITSRemoteProgram.RemoteProgramMode { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).SetRemoteProgramMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).GetRemoteProgramMode(); }
        void ITSRemoteProgram.ServerStartProgram(BinaryString bstrExecutablePath, BinaryString bstrFilePath, BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer) => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).ServerStartProgram(bstrExecutablePath, bstrFilePath, bstrWorkingDirectory, vbExpandEnvVarInWorkingDirectoryOnServer, bstrArguments, vbExpandEnvVarInArgumentsOnServer);
        #endregion
        #region ITSRemoteProgram2
        BinaryString ITSRemoteProgram2.RemoteApplicationName { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationName(value); }
        BinaryString ITSRemoteProgram2.RemoteApplicationProgram { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationProgram(value); }
        BinaryString ITSRemoteProgram2.RemoteApplicationArgs { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationArgs(value); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable5Proxy : IMsRdpClientNonScriptable5 //  : IMsRdpClientNonScriptable4
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
        #region IMsRdpClientNonScriptable3
        bool IMsRdpClientNonScriptable3.ShowRedirectionWarningDialog { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetShowRedirectionWarningDialog(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetShowRedirectionWarningDialog(); }
        bool IMsRdpClientNonScriptable3.PromptForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetPromptForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetPromptForCredentials(); }
        bool IMsRdpClientNonScriptable3.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetNegotiateSecurityLayer(); }
        bool IMsRdpClientNonScriptable3.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetEnableCredSspSupport(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDrives(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDevices(); }
        IMsRdpDeviceCollection IMsRdpClientNonScriptable3.DeviceCollection { get => ProxyObject.Pack<IMsRdpDeviceCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDeviceCollection()); }
        IMsRdpDriveCollection IMsRdpClientNonScriptable3.DriveCollection { get => ProxyObject.Pack<IMsRdpDriveCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDriveCollection()); }
        bool IMsRdpClientNonScriptable3.WarnAboutSendingCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutSendingCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutSendingCredentials(); }
        bool IMsRdpClientNonScriptable3.WarnAboutClipboardRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutClipboardRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutClipboardRedirection(); }
        BinaryString IMsRdpClientNonScriptable3.ConnectionBarText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetConnectionBarText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetConnectionBarText(); }
        #endregion
        #region IMsRdpClientNonScriptable4
        RedirectionWarningType IMsRdpClientNonScriptable4.RedirectionWarningType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetRedirectionWarningType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetRedirectionWarningType(); }
        bool IMsRdpClientNonScriptable4.MarkRdpSettingsSecure { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetMarkRdpSettingsSecure(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetMarkRdpSettingsSecure(); }
        void IMsRdpClientNonScriptable4.set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).set_PublisherCertificateChain(in pVarCert);
        object IMsRdpClientNonScriptable4.PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPublisherCertificateChain(); }
        bool IMsRdpClientNonScriptable4.WarnAboutPrinterRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetWarnAboutPrinterRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetWarnAboutPrinterRedirection(); }
        bool IMsRdpClientNonScriptable4.AllowCredentialSaving { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetAllowCredentialSaving(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetAllowCredentialSaving(); }
        bool IMsRdpClientNonScriptable4.PromptForCredsOnClient { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetPromptForCredsOnClient(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPromptForCredsOnClient(); }
        bool IMsRdpClientNonScriptable4.LaunchedViaClientShellInterface { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetLaunchedViaClientShellInterface(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetLaunchedViaClientShellInterface(); }
        bool IMsRdpClientNonScriptable4.TrustedZoneSite { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetTrustedZoneSite(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetTrustedZoneSite(); }
        #endregion
        #region IMsRdpClientNonScriptable5
        bool IMsRdpClientNonScriptable5.UseMultimon { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetUseMultimon(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetUseMultimon(); }
        uint IMsRdpClientNonScriptable5.RemoteMonitorCount { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorCount(); }
        void IMsRdpClientNonScriptable5.GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorsBoundingBox(out pLeft, out pTop, out pRight, out pBottom);
        bool IMsRdpClientNonScriptable5.RemoteMonitorLayoutMatchesLocal { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorLayoutMatchesLocal(); }
        bool IMsRdpClientNonScriptable5.DisableConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableConnectionBar(value); }
        bool IMsRdpClientNonScriptable5.DisableRemoteAppCapsCheck { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableRemoteAppCapsCheck(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetDisableRemoteAppCapsCheck(); }
        bool IMsRdpClientNonScriptable5.WarnAboutDirectXRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetWarnAboutDirectXRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetWarnAboutDirectXRedirection(); }
        bool IMsRdpClientNonScriptable5.AllowPromptingForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetAllowPromptingForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetAllowPromptingForCredentials(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpPreferredRedirectionInfoProxy : IMsRdpPreferredRedirectionInfo //
    {
        #region IMsRdpPreferredRedirectionInfo
        bool IMsRdpPreferredRedirectionInfo.UseRedirectionServerName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpPreferredRedirectionInfo>(this).SetUseRedirectionServerName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpPreferredRedirectionInfo>(this).GetUseRedirectionServerName(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpExtendedSettingsProxy : IMsRdpExtendedSettings //
    {
        #region IMsRdpExtendedSettings
        void IMsRdpExtendedSettings.set_Property(BinaryString bstrPropertyName, [In] [MarshalAs(UnmanagedType.Struct)] ref object pValue) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpExtendedSettings>(this).set_Property(bstrPropertyName, in pValue);
        [return: MarshalAs(UnmanagedType.Struct)] object IMsRdpExtendedSettings.get_Property(BinaryString bstrPropertyName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpExtendedSettings>(this).get_Property(bstrPropertyName);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient8Proxy : IMsRdpClient8 //  : IMsRdpClient7
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 IMsRdpClient6.AdvancedSettings7 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings6>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetAdvancedSettings7()); }
        IMsRdpClientTransportSettings2 IMsRdpClient6.TransportSettings2 { get => ProxyObject.Pack<IMsRdpClientTransportSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetTransportSettings2()); }
        #endregion
        #region IMsRdpClient7
        IMsRdpClientAdvancedSettings7 IMsRdpClient7.AdvancedSettings8 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings7>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetAdvancedSettings8()); }
        IMsRdpClientTransportSettings3 IMsRdpClient7.TransportSettings3 { get => ProxyObject.Pack<IMsRdpClientTransportSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetTransportSettings3()); }
        BinaryString IMsRdpClient7.GetStatusText(uint statusCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetStatusText(statusCode);
        IMsRdpClientSecuredSettings2 IMsRdpClient7.SecuredSettings3 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetSecuredSettings3()); }
        ITSRemoteProgram2 IMsRdpClient7.RemoteProgram2 { get => ProxyObject.Pack<ITSRemoteProgram2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetRemoteProgram2()); }
        #endregion
        #region IMsRdpClient8
        void IMsRdpClient8.SendRemoteAction(RemoteSessionActionType actionType) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).SendRemoteAction(actionType);
        IMsRdpClientAdvancedSettings8 IMsRdpClient8.AdvancedSettings9 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings8>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).GetAdvancedSettings9()); }
        ControlReconnectStatus IMsRdpClient8.Reconnect(uint ulWidth, uint ulHeight) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).Reconnect(ulWidth, ulHeight);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientAdvancedSettings8Proxy : IMsRdpClientAdvancedSettings8 //  : IMsRdpClientAdvancedSettings7
    {
        #region IMsTscAdvancedSettings
        int IMsTscAdvancedSettings.Compress { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetCompress(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetCompress(); }
        int IMsTscAdvancedSettings.BitmapPeristence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetBitmapPeristence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetBitmapPeristence(); }
        int IMsTscAdvancedSettings.allowBackgroundInput { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetAllowBackgroundInput(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetAllowBackgroundInput(); }
        BinaryString IMsTscAdvancedSettings.KeyBoardLayoutStr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetKeyBoardLayoutStr(value); }
        BinaryString IMsTscAdvancedSettings.PluginDlls { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetPluginDlls(value); }
        BinaryString IMsTscAdvancedSettings.IconFile { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconFile(value); }
        int IMsTscAdvancedSettings.IconIndex { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetIconIndex(value); }
        int IMsTscAdvancedSettings.ContainerHandledFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetContainerHandledFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetContainerHandledFullScreen(); }
        int IMsTscAdvancedSettings.DisableRdpdr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).SetDisableRdpdr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAdvancedSettings>(this).GetDisableRdpdr(); }
        #endregion
        #region IMsRdpClientAdvancedSettings
        int IMsRdpClientAdvancedSettings.SmoothScroll { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmoothScroll(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmoothScroll(); }
        int IMsRdpClientAdvancedSettings.AcceleratorPassthrough { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetAcceleratorPassthrough(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetAcceleratorPassthrough(); }
        int IMsRdpClientAdvancedSettings.ShadowBitmap { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShadowBitmap(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShadowBitmap(); }
        int IMsRdpClientAdvancedSettings.TransportType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetTransportType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetTransportType(); }
        int IMsRdpClientAdvancedSettings.SasSequence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSasSequence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSasSequence(); }
        int IMsRdpClientAdvancedSettings.EncryptionEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEncryptionEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEncryptionEnabled(); }
        int IMsRdpClientAdvancedSettings.DedicatedTerminal { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDedicatedTerminal(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDedicatedTerminal(); }
        int IMsRdpClientAdvancedSettings.RDPPort { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRDPPort(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRDPPort(); }
        int IMsRdpClientAdvancedSettings.EnableMouse { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableMouse(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableMouse(); }
        int IMsRdpClientAdvancedSettings.DisableCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisableCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisableCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.EnableWindowsKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetEnableWindowsKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetEnableWindowsKey(); }
        int IMsRdpClientAdvancedSettings.DoubleClickDetect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDoubleClickDetect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDoubleClickDetect(); }
        int IMsRdpClientAdvancedSettings.MaximizeShell { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaximizeShell(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaximizeShell(); }
        int IMsRdpClientAdvancedSettings.HotKeyFullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyFullScreen(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltEsc { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltEsc(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltEsc(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltShiftTab { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltShiftTab(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltShiftTab(); }
        int IMsRdpClientAdvancedSettings.HotKeyAltSpace { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyAltSpace(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyAltSpace(); }
        int IMsRdpClientAdvancedSettings.HotKeyCtrlAltDel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetHotKeyCtrlAltDel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetHotKeyCtrlAltDel(); }
        int IMsRdpClientAdvancedSettings.orderDrawThreshold { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOrderDrawThreshold(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOrderDrawThreshold(); }
        int IMsRdpClientAdvancedSettings.BitmapCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapCacheSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCacheSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCacheSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCacheSize(); }
        int IMsRdpClientAdvancedSettings.ScaleBitmapCachesByBPP { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetScaleBitmapCachesByBPP(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetScaleBitmapCachesByBPP(); }
        int IMsRdpClientAdvancedSettings.NumBitmapCaches { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNumBitmapCaches(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNumBitmapCaches(); }
        int IMsRdpClientAdvancedSettings.CachePersistenceActive { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetCachePersistenceActive(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetCachePersistenceActive(); }
        BinaryString IMsRdpClientAdvancedSettings.PersistCacheDirectory { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPersistCacheDirectory(value); }
        int IMsRdpClientAdvancedSettings.brushSupportLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBrushSupportLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBrushSupportLevel(); }
        int IMsRdpClientAdvancedSettings.minInputSendInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinInputSendInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinInputSendInterval(); }
        int IMsRdpClientAdvancedSettings.InputEventsAtOnce { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetInputEventsAtOnce(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetInputEventsAtOnce(); }
        int IMsRdpClientAdvancedSettings.maxEventCount { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMaxEventCount(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMaxEventCount(); }
        int IMsRdpClientAdvancedSettings.keepAliveInterval { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeepAliveInterval(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeepAliveInterval(); }
        int IMsRdpClientAdvancedSettings.shutdownTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetShutdownTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetShutdownTimeout(); }
        int IMsRdpClientAdvancedSettings.overallConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetOverallConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetOverallConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.singleConnectionTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSingleConnectionTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSingleConnectionTimeout(); }
        int IMsRdpClientAdvancedSettings.KeyboardType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardType(); }
        int IMsRdpClientAdvancedSettings.KeyboardSubType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardSubType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardSubType(); }
        int IMsRdpClientAdvancedSettings.KeyboardFunctionKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetKeyboardFunctionKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetKeyboardFunctionKey(); }
        int IMsRdpClientAdvancedSettings.WinceFixedPalette { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetWinceFixedPalette(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetWinceFixedPalette(); }
        bool IMsRdpClientAdvancedSettings.ConnectToServerConsole { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetConnectToServerConsole(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetConnectToServerConsole(); }
        int IMsRdpClientAdvancedSettings.BitmapPersistence { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapPersistence(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapPersistence(); }
        int IMsRdpClientAdvancedSettings.MinutesToIdleTimeout { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetMinutesToIdleTimeout(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetMinutesToIdleTimeout(); }
        bool IMsRdpClientAdvancedSettings.SmartSizing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetSmartSizing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetSmartSizing(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrLocalPrintingDocName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrLocalPrintingDocName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrLocalPrintingDocName(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipCleanTempDirString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipCleanTempDirString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipCleanTempDirString(); }
        BinaryString IMsRdpClientAdvancedSettings.RdpdrClipPasteInfoString { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRdpdrClipPasteInfoString(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRdpdrClipPasteInfoString(); }
        BinaryString IMsRdpClientAdvancedSettings.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetClearTextPassword(value); }
        bool IMsRdpClientAdvancedSettings.DisplayConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetDisplayConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetDisplayConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.PinConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPinConnectionBar(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPinConnectionBar(); }
        bool IMsRdpClientAdvancedSettings.GrabFocusOnConnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetGrabFocusOnConnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetGrabFocusOnConnect(); }
        BinaryString IMsRdpClientAdvancedSettings.LoadBalanceInfo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetLoadBalanceInfo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetLoadBalanceInfo(); }
        bool IMsRdpClientAdvancedSettings.RedirectDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectDrives(); }
        bool IMsRdpClientAdvancedSettings.RedirectPrinters { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPrinters(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPrinters(); }
        bool IMsRdpClientAdvancedSettings.RedirectPorts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectPorts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectPorts(); }
        bool IMsRdpClientAdvancedSettings.RedirectSmartCards { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetRedirectSmartCards(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetRedirectSmartCards(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache16BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache16BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache16BppSize(); }
        int IMsRdpClientAdvancedSettings.BitmapVirtualCache24BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetBitmapVirtualCache24BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetBitmapVirtualCache24BppSize(); }
        int IMsRdpClientAdvancedSettings.PerformanceFlags { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetPerformanceFlags(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetPerformanceFlags(); }
        void IMsRdpClientAdvancedSettings.set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).set_ConnectWithEndpoint(in A_1);
        bool IMsRdpClientAdvancedSettings.NotifyTSPublicKey { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).SetNotifyTSPublicKey(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings>(this).GetNotifyTSPublicKey(); }
        #endregion
        #region IMsRdpClientAdvancedSettings2
        bool IMsRdpClientAdvancedSettings2.CanAutoReconnect { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetCanAutoReconnect(); }
        bool IMsRdpClientAdvancedSettings2.EnableAutoReconnect { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetEnableAutoReconnect(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetEnableAutoReconnect(); }
        int IMsRdpClientAdvancedSettings2.MaxReconnectAttempts { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).SetMaxReconnectAttempts(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings2>(this).GetMaxReconnectAttempts(); }
        #endregion
        #region IMsRdpClientAdvancedSettings3
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowMinimizeButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowMinimizeButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowMinimizeButton(); }
        bool IMsRdpClientAdvancedSettings3.ConnectionBarShowRestoreButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).SetConnectionBarShowRestoreButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings3>(this).GetConnectionBarShowRestoreButton(); }
        #endregion
        #region IMsRdpClientAdvancedSettings4
        uint IMsRdpClientAdvancedSettings4.AuthenticationLevel { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).SetAuthenticationLevel(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings4>(this).GetAuthenticationLevel(); }
        #endregion
        #region IMsRdpClientAdvancedSettings5
        bool IMsRdpClientAdvancedSettings5.RedirectClipboard { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectClipboard(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectClipboard(); }
        uint IMsRdpClientAdvancedSettings5.AudioRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetAudioRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetAudioRedirectionMode(); }
        bool IMsRdpClientAdvancedSettings5.ConnectionBarShowPinButton { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetConnectionBarShowPinButton(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetConnectionBarShowPinButton(); }
        bool IMsRdpClientAdvancedSettings5.PublicMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetPublicMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetPublicMode(); }
        bool IMsRdpClientAdvancedSettings5.RedirectDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectDevices(); }
        bool IMsRdpClientAdvancedSettings5.RedirectPOSDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetRedirectPOSDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetRedirectPOSDevices(); }
        int IMsRdpClientAdvancedSettings5.BitmapVirtualCache32BppSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).SetBitmapVirtualCache32BppSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings5>(this).GetBitmapVirtualCache32BppSize(); }
        #endregion
        #region IMsRdpClientAdvancedSettings6
        bool IMsRdpClientAdvancedSettings6.RelativeMouseMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetRelativeMouseMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetRelativeMouseMode(); }
        BinaryString IMsRdpClientAdvancedSettings6.AuthenticationServiceClass { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationServiceClass(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetAuthenticationServiceClass(value); }
        BinaryString IMsRdpClientAdvancedSettings6.PCB { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetPCB(); set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetPCB(value); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseLeft { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseLeft(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseLeft(); }
        int IMsRdpClientAdvancedSettings6.HotKeyFocusReleaseRight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetHotKeyFocusReleaseRight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetHotKeyFocusReleaseRight(); }
        bool IMsRdpClientAdvancedSettings6.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetEnableCredSspSupport(); }
        uint IMsRdpClientAdvancedSettings6.AuthenticationType { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetAuthenticationType(); }
        bool IMsRdpClientAdvancedSettings6.ConnectToAdministerServer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).SetConnectToAdministerServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings6>(this).GetConnectToAdministerServer(); }
        #endregion
        #region IMsRdpClientAdvancedSettings7
        bool IMsRdpClientAdvancedSettings7.AudioCaptureRedirectionMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetAudioCaptureRedirectionMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetAudioCaptureRedirectionMode(); }
        uint IMsRdpClientAdvancedSettings7.VideoPlaybackMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetVideoPlaybackMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetVideoPlaybackMode(); }
        bool IMsRdpClientAdvancedSettings7.EnableSuperPan { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetEnableSuperPan(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetEnableSuperPan(); }
        uint IMsRdpClientAdvancedSettings7.SuperPanAccelerationFactor { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetSuperPanAccelerationFactor(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetSuperPanAccelerationFactor(); }
        bool IMsRdpClientAdvancedSettings7.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetNegotiateSecurityLayer(); }
        uint IMsRdpClientAdvancedSettings7.AudioQualityMode { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetAudioQualityMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetAudioQualityMode(); }
        bool IMsRdpClientAdvancedSettings7.RedirectDirectX { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetRedirectDirectX(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetRedirectDirectX(); }
        uint IMsRdpClientAdvancedSettings7.NetworkConnectionType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).SetNetworkConnectionType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings7>(this).GetNetworkConnectionType(); }
        #endregion
        #region IMsRdpClientAdvancedSettings8
        bool IMsRdpClientAdvancedSettings8.BandwidthDetection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings8>(this).SetBandwidthDetection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings8>(this).GetBandwidthDetection(); }
        ClientSpec IMsRdpClientAdvancedSettings8.ClientProtocolSpec { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings8>(this).SetClientProtocolSpec(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientAdvancedSettings8>(this).GetClientProtocolSpec(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient9Proxy : IMsRdpClient9 //  : IMsRdpClient8
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 IMsRdpClient6.AdvancedSettings7 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings6>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetAdvancedSettings7()); }
        IMsRdpClientTransportSettings2 IMsRdpClient6.TransportSettings2 { get => ProxyObject.Pack<IMsRdpClientTransportSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetTransportSettings2()); }
        #endregion
        #region IMsRdpClient7
        IMsRdpClientAdvancedSettings7 IMsRdpClient7.AdvancedSettings8 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings7>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetAdvancedSettings8()); }
        IMsRdpClientTransportSettings3 IMsRdpClient7.TransportSettings3 { get => ProxyObject.Pack<IMsRdpClientTransportSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetTransportSettings3()); }
        BinaryString IMsRdpClient7.GetStatusText(uint statusCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetStatusText(statusCode);
        IMsRdpClientSecuredSettings2 IMsRdpClient7.SecuredSettings3 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetSecuredSettings3()); }
        ITSRemoteProgram2 IMsRdpClient7.RemoteProgram2 { get => ProxyObject.Pack<ITSRemoteProgram2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetRemoteProgram2()); }
        #endregion
        #region IMsRdpClient8
        void IMsRdpClient8.SendRemoteAction(RemoteSessionActionType actionType) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).SendRemoteAction(actionType);
        IMsRdpClientAdvancedSettings8 IMsRdpClient8.AdvancedSettings9 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings8>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).GetAdvancedSettings9()); }
        ControlReconnectStatus IMsRdpClient8.Reconnect(uint ulWidth, uint ulHeight) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).Reconnect(ulWidth, ulHeight);
        #endregion
        #region IMsRdpClient9
        IMsRdpClientTransportSettings4 IMsRdpClient9.TransportSettings4 { get => ProxyObject.Pack<IMsRdpClientTransportSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).GetTransportSettings4()); }
        void IMsRdpClient9.SyncSessionDisplaySettings() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).SyncSessionDisplaySettings();
        void IMsRdpClient9.UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).UpdateSessionDisplaySettings(ulDesktopWidth, ulDesktopHeight, ulPhysicalWidth, ulPhysicalHeight, ulOrientation, ulDesktopScaleFactor, ulDeviceScaleFactor);
        void IMsRdpClient9.attachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).attachEvent(eventName, callback);
        void IMsRdpClient9.detachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).detachEvent(eventName, callback);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientTransportSettings4Proxy : IMsRdpClientTransportSettings4 //  : IMsRdpClientTransportSettings3
    {
        #region IMsRdpClientTransportSettings
        BinaryString IMsRdpClientTransportSettings.GatewayHostname { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayHostname(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayHostname(); }
        uint IMsRdpClientTransportSettings.GatewayUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayProfileUsageMethod { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayProfileUsageMethod(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayProfileUsageMethod(); }
        uint IMsRdpClientTransportSettings.GatewayCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayCredsSource(); }
        uint IMsRdpClientTransportSettings.GatewayUserSelectedCredsSource { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).SetGatewayUserSelectedCredsSource(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayUserSelectedCredsSource(); }
        int IMsRdpClientTransportSettings.GatewayIsSupported { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayIsSupported(); }
        uint IMsRdpClientTransportSettings.GatewayDefaultUsageMethod { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings>(this).GetGatewayDefaultUsageMethod(); }
        #endregion
        #region IMsRdpClientTransportSettings2
        uint IMsRdpClientTransportSettings2.GatewayCredSharing { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayCredSharing(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayCredSharing(); }
        uint IMsRdpClientTransportSettings2.GatewayPreAuthRequirement { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthRequirement(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthRequirement(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPreAuthServerAddr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPreAuthServerAddr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayPreAuthServerAddr(); }
        BinaryString IMsRdpClientTransportSettings2.GatewaySupportUrl { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewaySupportUrl(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewaySupportUrl(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookie(); }
        uint IMsRdpClientTransportSettings2.GatewayEncryptedOtpCookieSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayEncryptedOtpCookieSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayEncryptedOtpCookieSize(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayUsername { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayUsername(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayUsername(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayDomain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).GetGatewayDomain(); }
        BinaryString IMsRdpClientTransportSettings2.GatewayPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings2>(this).SetGatewayPassword(value); }
        #endregion
        #region IMsRdpClientTransportSettings3
        uint IMsRdpClientTransportSettings3.GatewayCredSourceCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayCredSourceCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayCredSourceCookie(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayAuthCookieServerAddr { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayAuthCookieServerAddr(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayAuthCookieServerAddr(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayEncryptedAuthCookie { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayEncryptedAuthCookie(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayEncryptedAuthCookie(); }
        uint IMsRdpClientTransportSettings3.GatewayEncryptedAuthCookieSize { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayEncryptedAuthCookieSize(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayEncryptedAuthCookieSize(); }
        BinaryString IMsRdpClientTransportSettings3.GatewayAuthLoginPage { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).SetGatewayAuthLoginPage(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings3>(this).GetGatewayAuthLoginPage(); }
        #endregion
        #region IMsRdpClientTransportSettings4
        uint IMsRdpClientTransportSettings4.GatewayBrokeringType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientTransportSettings4>(this).SetGatewayBrokeringType(value); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClient10Proxy : IMsRdpClient10 //  : IMsRdpClient9
    {
        #region IMsTscAx_Redist
        #endregion
        #region IMsTscAx
        BinaryString IMsTscAx.Server { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetServer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetServer(); }
        BinaryString IMsTscAx.Domain { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDomain(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDomain(); }
        BinaryString IMsTscAx.UserName { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetUserName(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetUserName(); }
        BinaryString IMsTscAx.DisconnectedText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDisconnectedText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDisconnectedText(); }
        BinaryString IMsTscAx.ConnectingText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetConnectingText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnectingText(); }
        short IMsTscAx.Connected { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetConnected(); }
        int IMsTscAx.DesktopWidth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopWidth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopWidth(); }
        int IMsTscAx.DesktopHeight { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetDesktopHeight(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDesktopHeight(); }
        int IMsTscAx.StartConnected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetStartConnected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetStartConnected(); }
        int IMsTscAx.HorizontalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetHorizontalScrollBarVisible(); }
        int IMsTscAx.VerticalScrollBarVisible { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVerticalScrollBarVisible(); }
        BinaryString IMsTscAx.FullScreenTitle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SetFullScreenTitle(value); }
        int IMsTscAx.CipherStrength { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetCipherStrength(); }
        BinaryString IMsTscAx.Version { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetVersion(); }
        int IMsTscAx.SecuredSettingsEnabled { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettingsEnabled(); }
        IMsTscSecuredSettings IMsTscAx.SecuredSettings { get => ProxyObject.Pack<IMsTscSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetSecuredSettings()); }
        IMsTscAdvancedSettings IMsTscAx.AdvancedSettings { get => ProxyObject.Pack<IMsTscAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetAdvancedSettings()); }
        IMsTscDebug IMsTscAx.Debugger { get => ProxyObject.Pack<IMsTscDebug>(ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).GetDebugger()); }
        void IMsTscAx.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Connect();
        void IMsTscAx.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).Disconnect();
        void IMsTscAx.CreateVirtualChannels(BinaryString newVal) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).CreateVirtualChannels(newVal);
        void IMsTscAx.SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscAx>(this).SendOnVirtualChannel(chanName, ChanData);
        #endregion
        #region IMsRdpClient
        int IMsRdpClient.ColorDepth { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetColorDepth(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetColorDepth(); }
        IMsRdpClientAdvancedSettings IMsRdpClient.AdvancedSettings2 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetAdvancedSettings2()); }
        IMsRdpClientSecuredSettings IMsRdpClient.SecuredSettings2 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetSecuredSettings2()); }
        ExtendedDisconnectReasonCode IMsRdpClient.ExtendedDisconnectReason { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetExtendedDisconnectReason(); }
        bool IMsRdpClient.FullScreen { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetFullScreen(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetFullScreen(); }
        void IMsRdpClient.SetVirtualChannelOptions(BinaryString chanName, int chanOptions) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).SetVirtualChannelOptions(chanName, chanOptions);
        int IMsRdpClient.GetVirtualChannelOptions(BinaryString chanName) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).GetVirtualChannelOptions(chanName);
        ControlCloseStatus IMsRdpClient.RequestClose() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient>(this).RequestClose();
        #endregion
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 IMsRdpClient2.AdvancedSettings3 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetAdvancedSettings3()); }
        BinaryString IMsRdpClient2.ConnectedStatusText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).SetConnectedStatusText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient2>(this).GetConnectedStatusText(); }
        #endregion
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 IMsRdpClient3.AdvancedSettings4 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient3>(this).GetAdvancedSettings4()); }
        #endregion
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 IMsRdpClient4.AdvancedSettings5 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient4>(this).GetAdvancedSettings5()); }
        #endregion
        #region IMsRdpClient5
        IMsRdpClientTransportSettings IMsRdpClient5.TransportSettings { get => ProxyObject.Pack<IMsRdpClientTransportSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetTransportSettings()); }
        IMsRdpClientAdvancedSettings5 IMsRdpClient5.AdvancedSettings6 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings5>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetAdvancedSettings6()); }
        BinaryString IMsRdpClient5.GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetErrorDescription(disconnectReason, ExtendedDisconnectReason);
        ITSRemoteProgram IMsRdpClient5.RemoteProgram { get => ProxyObject.Pack<ITSRemoteProgram>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetRemoteProgram()); }
        IMsRdpClientShell IMsRdpClient5.MsRdpClientShell { get => ProxyObject.Pack<IMsRdpClientShell>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient5>(this).GetMsRdpClientShell()); }
        #endregion
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 IMsRdpClient6.AdvancedSettings7 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings6>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetAdvancedSettings7()); }
        IMsRdpClientTransportSettings2 IMsRdpClient6.TransportSettings2 { get => ProxyObject.Pack<IMsRdpClientTransportSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient6>(this).GetTransportSettings2()); }
        #endregion
        #region IMsRdpClient7
        IMsRdpClientAdvancedSettings7 IMsRdpClient7.AdvancedSettings8 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings7>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetAdvancedSettings8()); }
        IMsRdpClientTransportSettings3 IMsRdpClient7.TransportSettings3 { get => ProxyObject.Pack<IMsRdpClientTransportSettings3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetTransportSettings3()); }
        BinaryString IMsRdpClient7.GetStatusText(uint statusCode) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetStatusText(statusCode);
        IMsRdpClientSecuredSettings2 IMsRdpClient7.SecuredSettings3 { get => ProxyObject.Pack<IMsRdpClientSecuredSettings2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetSecuredSettings3()); }
        ITSRemoteProgram2 IMsRdpClient7.RemoteProgram2 { get => ProxyObject.Pack<ITSRemoteProgram2>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient7>(this).GetRemoteProgram2()); }
        #endregion
        #region IMsRdpClient8
        void IMsRdpClient8.SendRemoteAction(RemoteSessionActionType actionType) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).SendRemoteAction(actionType);
        IMsRdpClientAdvancedSettings8 IMsRdpClient8.AdvancedSettings9 { get => ProxyObject.Pack<IMsRdpClientAdvancedSettings8>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).GetAdvancedSettings9()); }
        ControlReconnectStatus IMsRdpClient8.Reconnect(uint ulWidth, uint ulHeight) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient8>(this).Reconnect(ulWidth, ulHeight);
        #endregion
        #region IMsRdpClient9
        IMsRdpClientTransportSettings4 IMsRdpClient9.TransportSettings4 { get => ProxyObject.Pack<IMsRdpClientTransportSettings4>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).GetTransportSettings4()); }
        void IMsRdpClient9.SyncSessionDisplaySettings() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).SyncSessionDisplaySettings();
        void IMsRdpClient9.UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).UpdateSessionDisplaySettings(ulDesktopWidth, ulDesktopHeight, ulPhysicalWidth, ulPhysicalHeight, ulOrientation, ulDesktopScaleFactor, ulDeviceScaleFactor);
        void IMsRdpClient9.attachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).attachEvent(eventName, callback);
        void IMsRdpClient9.detachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient9>(this).detachEvent(eventName, callback);
        #endregion
        #region IMsRdpClient10
        ITSRemoteProgram3 IMsRdpClient10.RemoteProgram3 { get => ProxyObject.Pack<ITSRemoteProgram3>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClient10>(this).GetRemoteProgram3()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface ITSRemoteProgram3Proxy : ITSRemoteProgram3 //  : ITSRemoteProgram2
    {
        #region ITSRemoteProgram
        bool ITSRemoteProgram.RemoteProgramMode { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).SetRemoteProgramMode(value); get => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).GetRemoteProgramMode(); }
        void ITSRemoteProgram.ServerStartProgram(BinaryString bstrExecutablePath, BinaryString bstrFilePath, BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer) => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram>(this).ServerStartProgram(bstrExecutablePath, bstrFilePath, bstrWorkingDirectory, vbExpandEnvVarInWorkingDirectoryOnServer, bstrArguments, vbExpandEnvVarInArgumentsOnServer);
        #endregion
        #region ITSRemoteProgram2
        BinaryString ITSRemoteProgram2.RemoteApplicationName { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationName(value); }
        BinaryString ITSRemoteProgram2.RemoteApplicationProgram { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationProgram(value); }
        BinaryString ITSRemoteProgram2.RemoteApplicationArgs { set => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram2>(this).SetRemoteApplicationArgs(value); }
        #endregion
        #region ITSRemoteProgram3
        void ITSRemoteProgram3.ServerStartApp(BinaryString bstrAppUserModelId, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer) => ProxyObject.Unpack<MsRdpEx.Interop.ITSRemoteProgram3>(this).ServerStartApp(bstrAppUserModelId, bstrArguments, vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable6Proxy : IMsRdpClientNonScriptable6 //  : IMsRdpClientNonScriptable5
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
        #region IMsRdpClientNonScriptable3
        bool IMsRdpClientNonScriptable3.ShowRedirectionWarningDialog { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetShowRedirectionWarningDialog(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetShowRedirectionWarningDialog(); }
        bool IMsRdpClientNonScriptable3.PromptForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetPromptForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetPromptForCredentials(); }
        bool IMsRdpClientNonScriptable3.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetNegotiateSecurityLayer(); }
        bool IMsRdpClientNonScriptable3.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetEnableCredSspSupport(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDrives(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDevices(); }
        IMsRdpDeviceCollection IMsRdpClientNonScriptable3.DeviceCollection { get => ProxyObject.Pack<IMsRdpDeviceCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDeviceCollection()); }
        IMsRdpDriveCollection IMsRdpClientNonScriptable3.DriveCollection { get => ProxyObject.Pack<IMsRdpDriveCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDriveCollection()); }
        bool IMsRdpClientNonScriptable3.WarnAboutSendingCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutSendingCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutSendingCredentials(); }
        bool IMsRdpClientNonScriptable3.WarnAboutClipboardRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutClipboardRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutClipboardRedirection(); }
        BinaryString IMsRdpClientNonScriptable3.ConnectionBarText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetConnectionBarText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetConnectionBarText(); }
        #endregion
        #region IMsRdpClientNonScriptable4
        RedirectionWarningType IMsRdpClientNonScriptable4.RedirectionWarningType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetRedirectionWarningType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetRedirectionWarningType(); }
        bool IMsRdpClientNonScriptable4.MarkRdpSettingsSecure { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetMarkRdpSettingsSecure(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetMarkRdpSettingsSecure(); }
        void IMsRdpClientNonScriptable4.set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).set_PublisherCertificateChain(in pVarCert);
        object IMsRdpClientNonScriptable4.PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPublisherCertificateChain(); }
        bool IMsRdpClientNonScriptable4.WarnAboutPrinterRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetWarnAboutPrinterRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetWarnAboutPrinterRedirection(); }
        bool IMsRdpClientNonScriptable4.AllowCredentialSaving { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetAllowCredentialSaving(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetAllowCredentialSaving(); }
        bool IMsRdpClientNonScriptable4.PromptForCredsOnClient { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetPromptForCredsOnClient(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPromptForCredsOnClient(); }
        bool IMsRdpClientNonScriptable4.LaunchedViaClientShellInterface { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetLaunchedViaClientShellInterface(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetLaunchedViaClientShellInterface(); }
        bool IMsRdpClientNonScriptable4.TrustedZoneSite { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetTrustedZoneSite(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetTrustedZoneSite(); }
        #endregion
        #region IMsRdpClientNonScriptable5
        bool IMsRdpClientNonScriptable5.UseMultimon { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetUseMultimon(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetUseMultimon(); }
        uint IMsRdpClientNonScriptable5.RemoteMonitorCount { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorCount(); }
        void IMsRdpClientNonScriptable5.GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorsBoundingBox(out pLeft, out pTop, out pRight, out pBottom);
        bool IMsRdpClientNonScriptable5.RemoteMonitorLayoutMatchesLocal { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorLayoutMatchesLocal(); }
        bool IMsRdpClientNonScriptable5.DisableConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableConnectionBar(value); }
        bool IMsRdpClientNonScriptable5.DisableRemoteAppCapsCheck { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableRemoteAppCapsCheck(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetDisableRemoteAppCapsCheck(); }
        bool IMsRdpClientNonScriptable5.WarnAboutDirectXRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetWarnAboutDirectXRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetWarnAboutDirectXRedirection(); }
        bool IMsRdpClientNonScriptable5.AllowPromptingForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetAllowPromptingForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetAllowPromptingForCredentials(); }
        #endregion
        #region IMsRdpClientNonScriptable6
        void IMsRdpClientNonScriptable6.SendLocation2D(double latitude, double longitude) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable6>(this).SendLocation2D(latitude, longitude);
        void IMsRdpClientNonScriptable6.SendLocation3D(double latitude, double longitude, int altitude) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable6>(this).SendLocation3D(latitude, longitude, altitude);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClientNonScriptable7Proxy : IMsRdpClientNonScriptable7 //  : IMsRdpClientNonScriptable6
    {
        #region IMsTscNonScriptable
        BinaryString IMsTscNonScriptable.ClearTextPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetClearTextPassword(value); }
        BinaryString IMsTscNonScriptable.PortablePassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortablePassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortablePassword(); }
        BinaryString IMsTscNonScriptable.PortableSalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetPortableSalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetPortableSalt(); }
        BinaryString IMsTscNonScriptable.BinaryPassword { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinaryPassword(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinaryPassword(); }
        BinaryString IMsTscNonScriptable.BinarySalt { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).SetBinarySalt(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).GetBinarySalt(); }
        void IMsTscNonScriptable.ResetPassword() => ProxyObject.Unpack<MsRdpEx.Interop.IMsTscNonScriptable>(this).ResetPassword();
        #endregion
        #region IMsRdpClientNonScriptable
        void IMsRdpClientNonScriptable.NotifyRedirectDeviceChange(ulong wParam, long lParam) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).NotifyRedirectDeviceChange(wParam, lParam);
        void IMsRdpClientNonScriptable.SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable>(this).SendKeys(numKeys, in pbArrayKeyUp, in plKeyData);
        #endregion
        #region IMsRdpClientNonScriptable2
        nint IMsRdpClientNonScriptable2.UIParentWindowHandle { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).SetUIParentWindowHandle(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable2>(this).GetUIParentWindowHandle(); }
        #endregion
        #region IMsRdpClientNonScriptable3
        bool IMsRdpClientNonScriptable3.ShowRedirectionWarningDialog { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetShowRedirectionWarningDialog(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetShowRedirectionWarningDialog(); }
        bool IMsRdpClientNonScriptable3.PromptForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetPromptForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetPromptForCredentials(); }
        bool IMsRdpClientNonScriptable3.NegotiateSecurityLayer { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetNegotiateSecurityLayer(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetNegotiateSecurityLayer(); }
        bool IMsRdpClientNonScriptable3.EnableCredSspSupport { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetEnableCredSspSupport(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetEnableCredSspSupport(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDrives { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDrives(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDrives(); }
        bool IMsRdpClientNonScriptable3.RedirectDynamicDevices { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetRedirectDynamicDevices(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetRedirectDynamicDevices(); }
        IMsRdpDeviceCollection IMsRdpClientNonScriptable3.DeviceCollection { get => ProxyObject.Pack<IMsRdpDeviceCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDeviceCollection()); }
        IMsRdpDriveCollection IMsRdpClientNonScriptable3.DriveCollection { get => ProxyObject.Pack<IMsRdpDriveCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetDriveCollection()); }
        bool IMsRdpClientNonScriptable3.WarnAboutSendingCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutSendingCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutSendingCredentials(); }
        bool IMsRdpClientNonScriptable3.WarnAboutClipboardRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetWarnAboutClipboardRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetWarnAboutClipboardRedirection(); }
        BinaryString IMsRdpClientNonScriptable3.ConnectionBarText { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).SetConnectionBarText(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable3>(this).GetConnectionBarText(); }
        #endregion
        #region IMsRdpClientNonScriptable4
        RedirectionWarningType IMsRdpClientNonScriptable4.RedirectionWarningType { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetRedirectionWarningType(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetRedirectionWarningType(); }
        bool IMsRdpClientNonScriptable4.MarkRdpSettingsSecure { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetMarkRdpSettingsSecure(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetMarkRdpSettingsSecure(); }
        void IMsRdpClientNonScriptable4.set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).set_PublisherCertificateChain(in pVarCert);
        object IMsRdpClientNonScriptable4.PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPublisherCertificateChain(); }
        bool IMsRdpClientNonScriptable4.WarnAboutPrinterRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetWarnAboutPrinterRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetWarnAboutPrinterRedirection(); }
        bool IMsRdpClientNonScriptable4.AllowCredentialSaving { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetAllowCredentialSaving(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetAllowCredentialSaving(); }
        bool IMsRdpClientNonScriptable4.PromptForCredsOnClient { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetPromptForCredsOnClient(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetPromptForCredsOnClient(); }
        bool IMsRdpClientNonScriptable4.LaunchedViaClientShellInterface { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetLaunchedViaClientShellInterface(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetLaunchedViaClientShellInterface(); }
        bool IMsRdpClientNonScriptable4.TrustedZoneSite { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).SetTrustedZoneSite(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable4>(this).GetTrustedZoneSite(); }
        #endregion
        #region IMsRdpClientNonScriptable5
        bool IMsRdpClientNonScriptable5.UseMultimon { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetUseMultimon(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetUseMultimon(); }
        uint IMsRdpClientNonScriptable5.RemoteMonitorCount { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorCount(); }
        void IMsRdpClientNonScriptable5.GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorsBoundingBox(out pLeft, out pTop, out pRight, out pBottom);
        bool IMsRdpClientNonScriptable5.RemoteMonitorLayoutMatchesLocal { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetRemoteMonitorLayoutMatchesLocal(); }
        bool IMsRdpClientNonScriptable5.DisableConnectionBar { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableConnectionBar(value); }
        bool IMsRdpClientNonScriptable5.DisableRemoteAppCapsCheck { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetDisableRemoteAppCapsCheck(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetDisableRemoteAppCapsCheck(); }
        bool IMsRdpClientNonScriptable5.WarnAboutDirectXRedirection { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetWarnAboutDirectXRedirection(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetWarnAboutDirectXRedirection(); }
        bool IMsRdpClientNonScriptable5.AllowPromptingForCredentials { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).SetAllowPromptingForCredentials(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable5>(this).GetAllowPromptingForCredentials(); }
        #endregion
        #region IMsRdpClientNonScriptable6
        void IMsRdpClientNonScriptable6.SendLocation2D(double latitude, double longitude) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable6>(this).SendLocation2D(latitude, longitude);
        void IMsRdpClientNonScriptable6.SendLocation3D(double latitude, double longitude, int altitude) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable6>(this).SendLocation3D(latitude, longitude, altitude);
        #endregion
        #region IMsRdpClientNonScriptable7
        IMsRdpCameraRedirConfigCollection IMsRdpClientNonScriptable7.CameraRedirConfigCollection { get => ProxyObject.Pack<IMsRdpCameraRedirConfigCollection>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable7>(this).GetCameraRedirConfigCollection()); }
        void IMsRdpClientNonScriptable7.DisableDpiCursorScalingForProcess() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable7>(this).DisableDpiCursorScalingForProcess();
        IMsRdpClipboard IMsRdpClientNonScriptable7.Clipboard { get => ProxyObject.Pack<IMsRdpClipboard>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClientNonScriptable7>(this).GetClipboard()); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpCameraRedirConfigCollectionProxy : IMsRdpCameraRedirConfigCollection //
    {
        #region IMsRdpCameraRedirConfigCollection
        void IMsRdpCameraRedirConfigCollection.Rescan() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).Rescan();
        uint IMsRdpCameraRedirConfigCollection.Count { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).GetCount(); }
        IMsRdpCameraRedirConfig IMsRdpCameraRedirConfigCollection.get_ByIndex(uint index) => ProxyObject.Pack<IMsRdpCameraRedirConfig>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).get_ByIndex(index));
        IMsRdpCameraRedirConfig IMsRdpCameraRedirConfigCollection.get_BySymbolicLink(BinaryString SymbolicLink) => ProxyObject.Pack<IMsRdpCameraRedirConfig>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).get_BySymbolicLink(SymbolicLink));
        IMsRdpCameraRedirConfig IMsRdpCameraRedirConfigCollection.get_ByInstanceId(BinaryString InstanceId) => ProxyObject.Pack<IMsRdpCameraRedirConfig>(ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).get_ByInstanceId(InstanceId));
        void IMsRdpCameraRedirConfigCollection.AddConfig(BinaryString SymbolicLink, bool fRedirected) => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).AddConfig(SymbolicLink, fRedirected);
        bool IMsRdpCameraRedirConfigCollection.RedirectByDefault { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).SetRedirectByDefault(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).GetRedirectByDefault(); }
        bool IMsRdpCameraRedirConfigCollection.EncodeVideo { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).SetEncodeVideo(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).GetEncodeVideo(); }
        CameraRedirEncodingQuality IMsRdpCameraRedirConfigCollection.EncodingQuality { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).SetEncodingQuality(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfigCollection>(this).GetEncodingQuality(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpClipboardProxy : IMsRdpClipboard //
    {
        #region IMsRdpClipboard
        bool IMsRdpClipboard.CanSyncLocalClipboardToRemoteSession() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClipboard>(this).CanSyncLocalClipboardToRemoteSession();
        void IMsRdpClipboard.SyncLocalClipboardToRemoteSession() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClipboard>(this).SyncLocalClipboardToRemoteSession();
        bool IMsRdpClipboard.CanSyncRemoteClipboardToLocalSession() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClipboard>(this).CanSyncRemoteClipboardToLocalSession();
        void IMsRdpClipboard.SyncRemoteClipboardToLocalSession() => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpClipboard>(this).SyncRemoteClipboardToLocalSession();
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IRemoteDesktopClientProxy : IRemoteDesktopClient //  /* : IDispatch */
    {
        #region IRemoteDesktopClient
        void IRemoteDesktopClient.Connect() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).Connect();
        void IRemoteDesktopClient.Disconnect() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).Disconnect();
        void IRemoteDesktopClient.Reconnect(uint width, uint height) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).Reconnect(width, height);
        IRemoteDesktopClientSettings IRemoteDesktopClient.Settings { get => ProxyObject.Pack<IRemoteDesktopClientSettings>(ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).GetSettings()); }
        IRemoteDesktopClientActions IRemoteDesktopClient.Actions { get => ProxyObject.Pack<IRemoteDesktopClientActions>(ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).GetActions()); }
        IRemoteDesktopClientTouchPointer IRemoteDesktopClient.TouchPointer { get => ProxyObject.Pack<IRemoteDesktopClientTouchPointer>(ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).GetTouchPointer()); }
        void IRemoteDesktopClient.DeleteSavedCredentials(BinaryString serverName) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).DeleteSavedCredentials(serverName);
        void IRemoteDesktopClient.UpdateSessionDisplaySettings(uint width, uint height) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).UpdateSessionDisplaySettings(width, height);
        void IRemoteDesktopClient.attachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).attachEvent(eventName, callback);
        void IRemoteDesktopClient.detachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClient>(this).detachEvent(eventName, callback);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IRemoteDesktopClientSettingsProxy : IRemoteDesktopClientSettings //  /* : IDispatch */
    {
        #region IRemoteDesktopClientSettings
        void IRemoteDesktopClientSettings.ApplySettings(BinaryString RdpFileContents) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientSettings>(this).ApplySettings(RdpFileContents);
        BinaryString IRemoteDesktopClientSettings.RetrieveSettings() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientSettings>(this).RetrieveSettings();
        [return: MarshalAs(UnmanagedType.Struct)] object IRemoteDesktopClientSettings.GetRdpProperty(BinaryString propertyName) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientSettings>(this).GetRdpProperty(propertyName);
        void IRemoteDesktopClientSettings.SetRdpProperty(BinaryString propertyName, [MarshalAs(UnmanagedType.Struct)] object Value) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientSettings>(this).SetRdpProperty(propertyName, Value);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IRemoteDesktopClientActionsProxy : IRemoteDesktopClientActions //  /* : IDispatch */
    {
        #region IRemoteDesktopClientActions
        void IRemoteDesktopClientActions.SuspendScreenUpdates() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientActions>(this).SuspendScreenUpdates();
        void IRemoteDesktopClientActions.ResumeScreenUpdates() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientActions>(this).ResumeScreenUpdates();
        void IRemoteDesktopClientActions.ExecuteRemoteAction(RemoteActionType remoteAction) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientActions>(this).ExecuteRemoteAction(remoteAction);
        BinaryString IRemoteDesktopClientActions.GetSnapshot(SnapshotEncodingType snapshotEncoding, SnapshotFormatType snapshotFormat, uint snapshotWidth, uint snapshotHeight) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientActions>(this).GetSnapshot(snapshotEncoding, snapshotFormat, snapshotWidth, snapshotHeight);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IRemoteDesktopClientTouchPointerProxy : IRemoteDesktopClientTouchPointer //  /* : IDispatch */
    {
        #region IRemoteDesktopClientTouchPointer
        bool IRemoteDesktopClientTouchPointer.Enabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).SetEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).GetEnabled(); }
        bool IRemoteDesktopClientTouchPointer.EventsEnabled { set => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).SetEventsEnabled(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).GetEventsEnabled(); }
        uint IRemoteDesktopClientTouchPointer.PointerSpeed { set => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).SetPointerSpeed(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientTouchPointer>(this).GetPointerSpeed(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IRemoteDesktopClientEventsProxy : IRemoteDesktopClientEvents //  /* : IDispatch */
    {
        #region IRemoteDesktopClientEvents
        [PreserveSig] void IRemoteDesktopClientEvents.OnConnecting() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnConnecting();
        [PreserveSig] void IRemoteDesktopClientEvents.OnConnected() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnConnected();
        [PreserveSig] void IRemoteDesktopClientEvents.OnLoginCompleted() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnLoginCompleted();
        [PreserveSig] void IRemoteDesktopClientEvents.OnDisconnected(int disconnectReason, int ExtendedDisconnectReason, BinaryString disconnectErrorMessage) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnDisconnected(disconnectReason, ExtendedDisconnectReason, disconnectErrorMessage);
        [PreserveSig] void IRemoteDesktopClientEvents.OnStatusChanged(int statusCode, BinaryString statusMessage) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnStatusChanged(statusCode, statusMessage);
        [PreserveSig] void IRemoteDesktopClientEvents.OnAutoReconnecting(int disconnectReason, int ExtendedDisconnectReason, BinaryString disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnAutoReconnecting(disconnectReason, ExtendedDisconnectReason, disconnectErrorMessage, networkAvailable, attemptCount, maxAttemptCount);
        [PreserveSig] void IRemoteDesktopClientEvents.OnAutoReconnected() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnAutoReconnected();
        [PreserveSig] void IRemoteDesktopClientEvents.OnDialogDisplaying() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnDialogDisplaying();
        [PreserveSig] void IRemoteDesktopClientEvents.OnDialogDismissed() => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnDialogDismissed();
        [PreserveSig] void IRemoteDesktopClientEvents.OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnNetworkStatusChanged(qualityLevel, bandwidth, rtt);
        [PreserveSig] void IRemoteDesktopClientEvents.OnAdminMessageReceived(BinaryString adminMessage) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnAdminMessageReceived(adminMessage);
        [PreserveSig] void IRemoteDesktopClientEvents.OnKeyCombinationPressed(int keyCombination) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnKeyCombinationPressed(keyCombination);
        [PreserveSig] void IRemoteDesktopClientEvents.OnRemoteDesktopSizeChanged(int width, int height) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnRemoteDesktopSizeChanged(width, height);
        [PreserveSig] void IRemoteDesktopClientEvents.OnTouchPointerCursorMoved(int x, int y) => ProxyObject.Unpack<MsRdpEx.Interop.IRemoteDesktopClientEvents>(this).OnTouchPointerCursorMoved(x, y);
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpDeviceProxy : IMsRdpDevice //
    {
        #region IMsRdpDevice
        BinaryString IMsRdpDevice.DeviceInstanceId { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDevice>(this).GetDeviceInstanceId(); }
        BinaryString IMsRdpDevice.FriendlyName { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDevice>(this).GetFriendlyName(); }
        BinaryString IMsRdpDevice.DeviceDescription { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDevice>(this).GetDeviceDescription(); }
        bool IMsRdpDevice.RedirectionState { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDevice>(this).SetRedirectionState(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDevice>(this).GetRedirectionState(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpDriveProxy : IMsRdpDrive //
    {
        #region IMsRdpDrive
        BinaryString IMsRdpDrive.Name { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDrive>(this).GetName(); }
        bool IMsRdpDrive.RedirectionState { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDrive>(this).SetRedirectionState(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpDrive>(this).GetRedirectionState(); }
        #endregion
    }
    [DynamicInterfaceCastableImplementation]
    internal interface IMsRdpCameraRedirConfigProxy : IMsRdpCameraRedirConfig //
    {
        #region IMsRdpCameraRedirConfig
        BinaryString IMsRdpCameraRedirConfig.FriendlyName { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetFriendlyName(); }
        BinaryString IMsRdpCameraRedirConfig.SymbolicLink { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetSymbolicLink(); }
        BinaryString IMsRdpCameraRedirConfig.InstanceId { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetInstanceId(); }
        BinaryString IMsRdpCameraRedirConfig.ParentInstanceId { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetParentInstanceId(); }
        bool IMsRdpCameraRedirConfig.Redirected { set => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).SetRedirected(value); get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetRedirected(); }
        bool IMsRdpCameraRedirConfig.DeviceExists { get => ProxyObject.Unpack<MsRdpEx.Interop.IMsRdpCameraRedirConfig>(this).GetDeviceExists(); }
        #endregion
    }
}
