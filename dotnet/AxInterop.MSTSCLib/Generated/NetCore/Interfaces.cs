using System.Runtime.InteropServices;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    [ProxyGuid("327BB5CD-834E-4400-AEF2-B30E15E5D682", typeof(IMsTscAx_RedistProxy))]
    public interface IMsTscAx_Redist /* : IDispatch */
    {
        #region IMsTscAx_Redist
        #endregion
    }
    [ProxyGuid("8C11EFAE-92C3-11D1-BC1E-00C04FA31489", typeof(IMsTscAxProxy))]
    public interface IMsTscAx : IMsTscAx_Redist
    {
        #region IMsTscAx
        BinaryString Server { set; get; }
        BinaryString Domain { set; get; }
        BinaryString UserName { set; get; }
        BinaryString DisconnectedText { set; get; }
        BinaryString ConnectingText { set; get; }
        short Connected { get; }
        int DesktopWidth { set; get; }
        int DesktopHeight { set; get; }
        int StartConnected { set; get; }
        int HorizontalScrollBarVisible { get; }
        int VerticalScrollBarVisible { get; }
        BinaryString FullScreenTitle { set; }
        int CipherStrength { get; }
        BinaryString Version { get; }
        int SecuredSettingsEnabled { get; }
        IMsTscSecuredSettings SecuredSettings { get; }
        IMsTscAdvancedSettings AdvancedSettings { get; }
        IMsTscDebug Debugger { get; }
        void Connect();
        void Disconnect();
        void CreateVirtualChannels(BinaryString newVal);
        void SendOnVirtualChannel(BinaryString chanName, BinaryString ChanData);
        #endregion
    }
    [ProxyGuid("C9D65442-A0F9-45B2-8F73-D61D2DB8CBB6", typeof(IMsTscSecuredSettingsProxy))]
    public interface IMsTscSecuredSettings /* : IDispatch */
    {
        #region IMsTscSecuredSettings
        BinaryString StartProgram { set; get; }
        BinaryString WorkDir { set; get; }
        int FullScreen { set; get; }
        #endregion
    }
    [ProxyGuid("809945CC-4B3B-4A92-A6B0-DBF9B5F2EF2D", typeof(IMsTscAdvancedSettingsProxy))]
    public interface IMsTscAdvancedSettings /* : IDispatch */
    {
        #region IMsTscAdvancedSettings
        int Compress { set; get; }
        int BitmapPeristence { set; get; }
        int allowBackgroundInput { set; get; }
        BinaryString KeyBoardLayoutStr { set; }
        BinaryString PluginDlls { set; }
        BinaryString IconFile { set; }
        int IconIndex { set; }
        int ContainerHandledFullScreen { set; get; }
        int DisableRdpdr { set; get; }
        #endregion
    }
    [ProxyGuid("209D0EB9-6254-47B1-9033-A98DAE55BB27", typeof(IMsTscDebugProxy))]
    public interface IMsTscDebug /* : IDispatch */
    {
        #region IMsTscDebug
        int HatchBitmapPDU { set; get; }
        int HatchSSBOrder { set; get; }
        int HatchMembltOrder { set; get; }
        int HatchIndexPDU { set; get; }
        int LabelMemblt { set; get; }
        int BitmapCacheMonitor { set; get; }
        int MallocFailuresPercent { set; get; }
        int MallocHugeFailuresPercent { set; get; }
        int NetThroughput { set; get; }
        BinaryString CLXCmdLine { set; get; }
        BinaryString CLXDll { set; get; }
        int RemoteProgramsHatchVisibleRegion { set; get; }
        int RemoteProgramsHatchVisibleNoDataRegion { set; get; }
        int RemoteProgramsHatchNonVisibleRegion { set; get; }
        int RemoteProgramsHatchWindow { set; get; }
        int RemoteProgramsStayConnectOnBadCaps { set; get; }
        uint ControlType { get; }
        bool DecodeGfx { set; }
        #endregion
    }
    [ProxyGuid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6", typeof(IMsTscAxEventsProxy))]
    public interface IMsTscAxEvents /* : IDispatch */
    {
        #region IMsTscAxEvents
        [PreserveSig] void OnConnecting();
        [PreserveSig] void OnConnected();
        [PreserveSig] void OnLoginComplete();
        [PreserveSig] void OnDisconnected(int discReason);
        [PreserveSig] void OnEnterFullScreenMode();
        [PreserveSig] void OnLeaveFullScreenMode();
        [PreserveSig] void OnChannelReceivedData(BinaryString chanName, BinaryString data);
        [PreserveSig] void OnRequestGoFullScreen();
        [PreserveSig] void OnRequestLeaveFullScreen();
        [PreserveSig] void OnFatalError(int errorCode);
        [PreserveSig] void OnWarning(int warningCode);
        [PreserveSig] void OnRemoteDesktopSizeChange(int width, int height);
        [PreserveSig] void OnIdleTimeoutNotification();
        [PreserveSig] void OnRequestContainerMinimize();
        [PreserveSig] void OnConfirmClose(out bool pfAllowClose);
        [PreserveSig] void OnReceivedTSPublicKey(BinaryString publicKey, out bool pfContinueLogon);
        [PreserveSig] void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus);
        [PreserveSig] void OnAuthenticationWarningDisplayed();
        [PreserveSig] void OnAuthenticationWarningDismissed();
        [PreserveSig] void OnRemoteProgramResult(BinaryString bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable);
        [PreserveSig] void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation);
        [PreserveSig] void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute);
        [PreserveSig] void OnLogonError(int lError);
        [PreserveSig] void OnFocusReleased(int iDirection);
        [PreserveSig] void OnUserNameAcquired(BinaryString bstrUserName);
        [PreserveSig] void OnMouseInputModeChanged(bool fMouseModeRelative);
        [PreserveSig] void OnServiceMessageReceived(BinaryString serviceMessage);
        [PreserveSig] void OnConnectionBarPullDown();
        [PreserveSig] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig] void OnDevicesButtonPressed();
        [PreserveSig] void OnAutoReconnected();
        [PreserveSig] void OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount);
        #endregion
    }
    [ProxyGuid("92B4A539-7115-4B7C-A5A9-E5D9EFC2780A", typeof(IMsRdpClientProxy))]
    public interface IMsRdpClient : IMsTscAx
    {
        #region IMsRdpClient
        int ColorDepth { set; get; }
        IMsRdpClientAdvancedSettings AdvancedSettings2 { get; }
        IMsRdpClientSecuredSettings SecuredSettings2 { get; }
        ExtendedDisconnectReasonCode ExtendedDisconnectReason { get; }
        bool FullScreen { set; get; }
        void SetVirtualChannelOptions(BinaryString chanName, int chanOptions);
        int GetVirtualChannelOptions(BinaryString chanName);
        ControlCloseStatus RequestClose();
        #endregion
    }
    [ProxyGuid("3C65B4AB-12B3-465B-ACD4-B8DAD3BFF9E2", typeof(IMsRdpClientAdvancedSettingsProxy))]
    public interface IMsRdpClientAdvancedSettings : IMsTscAdvancedSettings
    {
        #region IMsRdpClientAdvancedSettings
        int SmoothScroll { set; get; }
        int AcceleratorPassthrough { set; get; }
        int ShadowBitmap { set; get; }
        int TransportType { set; get; }
        int SasSequence { set; get; }
        int EncryptionEnabled { set; get; }
        int DedicatedTerminal { set; get; }
        int RDPPort { set; get; }
        int EnableMouse { set; get; }
        int DisableCtrlAltDel { set; get; }
        int EnableWindowsKey { set; get; }
        int DoubleClickDetect { set; get; }
        int MaximizeShell { set; get; }
        int HotKeyFullScreen { set; get; }
        int HotKeyCtrlEsc { set; get; }
        int HotKeyAltEsc { set; get; }
        int HotKeyAltTab { set; get; }
        int HotKeyAltShiftTab { set; get; }
        int HotKeyAltSpace { set; get; }
        int HotKeyCtrlAltDel { set; get; }
        int orderDrawThreshold { set; get; }
        int BitmapCacheSize { set; get; }
        int BitmapVirtualCacheSize { set; get; }
        int ScaleBitmapCachesByBPP { set; get; }
        int NumBitmapCaches { set; get; }
        int CachePersistenceActive { set; get; }
        BinaryString PersistCacheDirectory { set; }
        int brushSupportLevel { set; get; }
        int minInputSendInterval { set; get; }
        int InputEventsAtOnce { set; get; }
        int maxEventCount { set; get; }
        int keepAliveInterval { set; get; }
        int shutdownTimeout { set; get; }
        int overallConnectionTimeout { set; get; }
        int singleConnectionTimeout { set; get; }
        int KeyboardType { set; get; }
        int KeyboardSubType { set; get; }
        int KeyboardFunctionKey { set; get; }
        int WinceFixedPalette { set; get; }
        bool ConnectToServerConsole { set; get; }
        int BitmapPersistence { set; get; }
        int MinutesToIdleTimeout { set; get; }
        bool SmartSizing { set; get; }
        BinaryString RdpdrLocalPrintingDocName { set; get; }
        BinaryString RdpdrClipCleanTempDirString { set; get; }
        BinaryString RdpdrClipPasteInfoString { set; get; }
        BinaryString ClearTextPassword { set; }
        bool DisplayConnectionBar { set; get; }
        bool PinConnectionBar { set; get; }
        bool GrabFocusOnConnect { set; get; }
        BinaryString LoadBalanceInfo { set; get; }
        bool RedirectDrives { set; get; }
        bool RedirectPrinters { set; get; }
        bool RedirectPorts { set; get; }
        bool RedirectSmartCards { set; get; }
        int BitmapVirtualCache16BppSize { set; get; }
        int BitmapVirtualCache24BppSize { set; get; }
        int PerformanceFlags { set; get; }
        void set_ConnectWithEndpoint([In] [MarshalAs(UnmanagedType.Struct)] ref object A_1);
        bool NotifyTSPublicKey { set; get; }
        #endregion
    }
    [ProxyGuid("605BEFCF-39C1-45CC-A811-068FB7BE346D", typeof(IMsRdpClientSecuredSettingsProxy))]
    public interface IMsRdpClientSecuredSettings : IMsTscSecuredSettings
    {
        #region IMsRdpClientSecuredSettings
        int KeyboardHookMode { set; get; }
        int AudioRedirectionMode { set; get; }
        #endregion
    }
    [ProxyGuid("C1E6743A-41C1-4A74-832A-0DD06C1C7A0E", typeof(IMsTscNonScriptableProxy))]
    public interface IMsTscNonScriptable
    {
        #region IMsTscNonScriptable
        BinaryString ClearTextPassword { set; }
        BinaryString PortablePassword { set; get; }
        BinaryString PortableSalt { set; get; }
        BinaryString BinaryPassword { set; get; }
        BinaryString BinarySalt { set; get; }
        void ResetPassword();
        #endregion
    }
    [ProxyGuid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE", typeof(IMsRdpClientNonScriptableProxy))]
    public interface IMsRdpClientNonScriptable : IMsTscNonScriptable
    {
        #region IMsRdpClientNonScriptable
        void NotifyRedirectDeviceChange(ulong wParam, long lParam);
        void SendKeys(int numKeys, [In] ref bool pbArrayKeyUp, [In] ref int plKeyData);
        #endregion
    }
    [ProxyGuid("E7E17DC4-3B71-4BA7-A8E6-281FFADCA28F", typeof(IMsRdpClient2Proxy))]
    public interface IMsRdpClient2 : IMsRdpClient
    {
        #region IMsRdpClient2
        IMsRdpClientAdvancedSettings2 AdvancedSettings3 { get; }
        BinaryString ConnectedStatusText { set; get; }
        #endregion
    }
    [ProxyGuid("9AC42117-2B76-4320-AA44-0E616AB8437B", typeof(IMsRdpClientAdvancedSettings2Proxy))]
    public interface IMsRdpClientAdvancedSettings2 : IMsRdpClientAdvancedSettings
    {
        #region IMsRdpClientAdvancedSettings2
        bool CanAutoReconnect { get; }
        bool EnableAutoReconnect { set; get; }
        int MaxReconnectAttempts { set; get; }
        #endregion
    }
    [ProxyGuid("91B7CBC5-A72E-4FA0-9300-D647D7E897FF", typeof(IMsRdpClient3Proxy))]
    public interface IMsRdpClient3 : IMsRdpClient2
    {
        #region IMsRdpClient3
        IMsRdpClientAdvancedSettings3 AdvancedSettings4 { get; }
        #endregion
    }
    [ProxyGuid("19CD856B-C542-4C53-ACEE-F127E3BE1A59", typeof(IMsRdpClientAdvancedSettings3Proxy))]
    public interface IMsRdpClientAdvancedSettings3 : IMsRdpClientAdvancedSettings2
    {
        #region IMsRdpClientAdvancedSettings3
        bool ConnectionBarShowMinimizeButton { set; get; }
        bool ConnectionBarShowRestoreButton { set; get; }
        #endregion
    }
    [ProxyGuid("095E0738-D97D-488B-B9F6-DD0E8D66C0DE", typeof(IMsRdpClient4Proxy))]
    public interface IMsRdpClient4 : IMsRdpClient3
    {
        #region IMsRdpClient4
        IMsRdpClientAdvancedSettings4 AdvancedSettings5 { get; }
        #endregion
    }
    [ProxyGuid("FBA7F64E-7345-4405-AE50-FA4A763DC0DE", typeof(IMsRdpClientAdvancedSettings4Proxy))]
    public interface IMsRdpClientAdvancedSettings4 : IMsRdpClientAdvancedSettings3
    {
        #region IMsRdpClientAdvancedSettings4
        uint AuthenticationLevel { set; get; }
        #endregion
    }
    [ProxyGuid("17A5E535-4072-4FA4-AF32-C8D0D47345E9", typeof(IMsRdpClientNonScriptable2Proxy))]
    public interface IMsRdpClientNonScriptable2 : IMsRdpClientNonScriptable
    {
        #region IMsRdpClientNonScriptable2
        nint UIParentWindowHandle { set; get; }
        #endregion
    }
    [ProxyGuid("4EB5335B-6429-477D-B922-E06A28ECD8BF", typeof(IMsRdpClient5Proxy))]
    public interface IMsRdpClient5 : IMsRdpClient4
    {
        #region IMsRdpClient5
        IMsRdpClientTransportSettings TransportSettings { get; }
        IMsRdpClientAdvancedSettings5 AdvancedSettings6 { get; }
        BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        ITSRemoteProgram RemoteProgram { get; }
        IMsRdpClientShell MsRdpClientShell { get; }
        #endregion
    }
    [ProxyGuid("720298C0-A099-46F5-9F82-96921BAE4701", typeof(IMsRdpClientTransportSettingsProxy))]
    public interface IMsRdpClientTransportSettings /* : IDispatch */
    {
        #region IMsRdpClientTransportSettings
        BinaryString GatewayHostname { set; get; }
        uint GatewayUsageMethod { set; get; }
        uint GatewayProfileUsageMethod { set; get; }
        uint GatewayCredsSource { set; get; }
        uint GatewayUserSelectedCredsSource { set; get; }
        int GatewayIsSupported { get; }
        uint GatewayDefaultUsageMethod { get; }
        #endregion
    }
    [ProxyGuid("FBA7F64E-6783-4405-DA45-FA4A763DABD0", typeof(IMsRdpClientAdvancedSettings5Proxy))]
    public interface IMsRdpClientAdvancedSettings5 : IMsRdpClientAdvancedSettings4
    {
        #region IMsRdpClientAdvancedSettings5
        bool RedirectClipboard { set; get; }
        uint AudioRedirectionMode { set; get; }
        bool ConnectionBarShowPinButton { set; get; }
        bool PublicMode { set; get; }
        bool RedirectDevices { set; get; }
        bool RedirectPOSDevices { set; get; }
        int BitmapVirtualCache32BppSize { set; get; }
        #endregion
    }
    [ProxyGuid("FDD029F9-467A-4C49-8529-64B521DBD1B4", typeof(ITSRemoteProgramProxy))]
    public interface ITSRemoteProgram /* : IDispatch */
    {
        #region ITSRemoteProgram
        bool RemoteProgramMode { set; get; }
        void ServerStartProgram(BinaryString bstrExecutablePath, BinaryString bstrFilePath, BinaryString bstrWorkingDirectory, bool vbExpandEnvVarInWorkingDirectoryOnServer, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [ProxyGuid("D012AE6D-C19A-4BFE-B367-201F8911F134", typeof(IMsRdpClientShellProxy))]
    public interface IMsRdpClientShell /* : IDispatch */
    {
        #region IMsRdpClientShell
        void Launch();
        BinaryString RdpFileContents { set; get; }
        void SetRdpProperty(BinaryString szProperty, [MarshalAs(UnmanagedType.Struct)] object Value);
        [return: MarshalAs(UnmanagedType.Struct)] object GetRdpProperty(BinaryString szProperty);
        bool IsRemoteProgramClientInstalled { get; }
        bool PublicMode { set; get; }
        void ShowTrustedSitesManagementDialog();
        #endregion
    }
    [ProxyGuid("B3378D90-0728-45C7-8ED7-B6159FB92219", typeof(IMsRdpClientNonScriptable3Proxy))]
    public interface IMsRdpClientNonScriptable3 : IMsRdpClientNonScriptable2
    {
        #region IMsRdpClientNonScriptable3
        bool ShowRedirectionWarningDialog { set; get; }
        bool PromptForCredentials { set; get; }
        bool NegotiateSecurityLayer { set; get; }
        bool EnableCredSspSupport { set; get; }
        bool RedirectDynamicDrives { set; get; }
        bool RedirectDynamicDevices { set; get; }
        IMsRdpDeviceCollection DeviceCollection { get; }
        IMsRdpDriveCollection DriveCollection { get; }
        bool WarnAboutSendingCredentials { set; get; }
        bool WarnAboutClipboardRedirection { set; get; }
        BinaryString ConnectionBarText { set; get; }
        #endregion
    }
    [ProxyGuid("56540617-D281-488C-8738-6A8FDF64A118", typeof(IMsRdpDeviceCollectionProxy))]
    public interface IMsRdpDeviceCollection
    {
        #region IMsRdpDeviceCollection
        void RescanDevices(bool vboolDynRedir);
        IMsRdpDevice get_DeviceByIndex(uint index);
        IMsRdpDevice get_DeviceById(BinaryString devInstanceId);
        uint DeviceCount { get; }
        #endregion
    }
    [ProxyGuid("7FF17599-DA2C-4677-AD35-F60C04FE1585", typeof(IMsRdpDriveCollectionProxy))]
    public interface IMsRdpDriveCollection
    {
        #region IMsRdpDriveCollection
        void RescanDrives(bool vboolDynRedir);
        IMsRdpDrive get_DriveByIndex(uint index);
        uint DriveCount { get; }
        #endregion
    }
    [ProxyGuid("D43B7D80-8517-4B6D-9EAC-96AD6800D7F2", typeof(IMsRdpClient6Proxy))]
    public interface IMsRdpClient6 : IMsRdpClient5
    {
        #region IMsRdpClient6
        IMsRdpClientAdvancedSettings6 AdvancedSettings7 { get; }
        IMsRdpClientTransportSettings2 TransportSettings2 { get; }
        #endregion
    }
    [ProxyGuid("222C4B5D-45D9-4DF0-A7C6-60CF9089D285", typeof(IMsRdpClientAdvancedSettings6Proxy))]
    public interface IMsRdpClientAdvancedSettings6 : IMsRdpClientAdvancedSettings5
    {
        #region IMsRdpClientAdvancedSettings6
        bool RelativeMouseMode { set; get; }
        BinaryString AuthenticationServiceClass { get; set; }
        BinaryString PCB { get; set; }
        int HotKeyFocusReleaseLeft { set; get; }
        int HotKeyFocusReleaseRight { set; get; }
        bool EnableCredSspSupport { set; get; }
        uint AuthenticationType { get; }
        bool ConnectToAdministerServer { set; get; }
        #endregion
    }
    [ProxyGuid("67341688-D606-4C73-A5D2-2E0489009319", typeof(IMsRdpClientTransportSettings2Proxy))]
    public interface IMsRdpClientTransportSettings2 : IMsRdpClientTransportSettings
    {
        #region IMsRdpClientTransportSettings2
        uint GatewayCredSharing { set; get; }
        uint GatewayPreAuthRequirement { set; get; }
        BinaryString GatewayPreAuthServerAddr { set; get; }
        BinaryString GatewaySupportUrl { set; get; }
        BinaryString GatewayEncryptedOtpCookie { set; get; }
        uint GatewayEncryptedOtpCookieSize { set; get; }
        BinaryString GatewayUsername { set; get; }
        BinaryString GatewayDomain { set; get; }
        BinaryString GatewayPassword { set; }
        #endregion
    }
    [ProxyGuid("F50FA8AA-1C7D-4F59-B15C-A90CACAE1FCB", typeof(IMsRdpClientNonScriptable4Proxy))]
    public interface IMsRdpClientNonScriptable4 : IMsRdpClientNonScriptable3
    {
        #region IMsRdpClientNonScriptable4
        RedirectionWarningType RedirectionWarningType { set; get; }
        bool MarkRdpSettingsSecure { set; get; }
        void set_PublisherCertificateChain([In] [MarshalAs(UnmanagedType.Struct)] ref object pVarCert);
        object PublisherCertificateChain { [return: MarshalAs(UnmanagedType.Struct)] get; }
        bool WarnAboutPrinterRedirection { set; get; }
        bool AllowCredentialSaving { set; get; }
        bool PromptForCredsOnClient { set; get; }
        bool LaunchedViaClientShellInterface { set; get; }
        bool TrustedZoneSite { set; get; }
        #endregion
    }
    [ProxyGuid("B2A5B5CE-3461-444A-91D4-ADD26D070638", typeof(IMsRdpClient7Proxy))]
    public interface IMsRdpClient7 : IMsRdpClient6
    {
        #region IMsRdpClient7
        IMsRdpClientAdvancedSettings7 AdvancedSettings8 { get; }
        IMsRdpClientTransportSettings3 TransportSettings3 { get; }
        BinaryString GetStatusText(uint statusCode);
        IMsRdpClientSecuredSettings2 SecuredSettings3 { get; }
        ITSRemoteProgram2 RemoteProgram2 { get; }
        #endregion
    }
    [ProxyGuid("26036036-4010-4578-8091-0DB9A1EDF9C3", typeof(IMsRdpClientAdvancedSettings7Proxy))]
    public interface IMsRdpClientAdvancedSettings7 : IMsRdpClientAdvancedSettings6
    {
        #region IMsRdpClientAdvancedSettings7
        bool AudioCaptureRedirectionMode { set; get; }
        uint VideoPlaybackMode { set; get; }
        bool EnableSuperPan { set; get; }
        uint SuperPanAccelerationFactor { set; get; }
        bool NegotiateSecurityLayer { set; get; }
        uint AudioQualityMode { set; get; }
        bool RedirectDirectX { set; get; }
        uint NetworkConnectionType { set; get; }
        #endregion
    }
    [ProxyGuid("3D5B21AC-748D-41DE-8F30-E15169586BD4", typeof(IMsRdpClientTransportSettings3Proxy))]
    public interface IMsRdpClientTransportSettings3 : IMsRdpClientTransportSettings2
    {
        #region IMsRdpClientTransportSettings3
        uint GatewayCredSourceCookie { set; get; }
        BinaryString GatewayAuthCookieServerAddr { set; get; }
        BinaryString GatewayEncryptedAuthCookie { set; get; }
        uint GatewayEncryptedAuthCookieSize { set; get; }
        BinaryString GatewayAuthLoginPage { set; get; }
        #endregion
    }
    [ProxyGuid("25F2CE20-8B1D-4971-A7CD-549DAE201FC0", typeof(IMsRdpClientSecuredSettings2Proxy))]
    public interface IMsRdpClientSecuredSettings2 : IMsRdpClientSecuredSettings
    {
        #region IMsRdpClientSecuredSettings2
        BinaryString PCB { get; set; }
        #endregion
    }
    [ProxyGuid("92C38A7D-241A-418C-9936-099872C9AF20", typeof(ITSRemoteProgram2Proxy))]
    public interface ITSRemoteProgram2 : ITSRemoteProgram
    {
        #region ITSRemoteProgram2
        BinaryString RemoteApplicationName { set; }
        BinaryString RemoteApplicationProgram { set; }
        BinaryString RemoteApplicationArgs { set; }
        #endregion
    }
    [ProxyGuid("4F6996D5-D7B1-412C-B0FF-063718566907", typeof(IMsRdpClientNonScriptable5Proxy))]
    public interface IMsRdpClientNonScriptable5 : IMsRdpClientNonScriptable4
    {
        #region IMsRdpClientNonScriptable5
        bool UseMultimon { set; get; }
        uint RemoteMonitorCount { get; }
        void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);
        bool RemoteMonitorLayoutMatchesLocal { get; }
        bool DisableConnectionBar { set; }
        bool DisableRemoteAppCapsCheck { set; get; }
        bool WarnAboutDirectXRedirection { set; get; }
        bool AllowPromptingForCredentials { set; get; }
        #endregion
    }
    [ProxyGuid("FDD029F9-9574-4DEF-8529-64B521CCCAA4", typeof(IMsRdpPreferredRedirectionInfoProxy))]
    public interface IMsRdpPreferredRedirectionInfo
    {
        #region IMsRdpPreferredRedirectionInfo
        bool UseRedirectionServerName { set; get; }
        #endregion
    }
    [ProxyGuid("302D8188-0052-4807-806A-362B628F9AC5", typeof(IMsRdpExtendedSettingsProxy))]
    public interface IMsRdpExtendedSettings
    {
        #region IMsRdpExtendedSettings
        void set_Property(BinaryString bstrPropertyName, [In] [MarshalAs(UnmanagedType.Struct)] ref object pValue);
        [return: MarshalAs(UnmanagedType.Struct)] object get_Property(BinaryString bstrPropertyName);
        #endregion
    }
    [ProxyGuid("4247E044-9271-43A9-BC49-E2AD9E855D62", typeof(IMsRdpClient8Proxy))]
    public interface IMsRdpClient8 : IMsRdpClient7
    {
        #region IMsRdpClient8
        void SendRemoteAction(RemoteSessionActionType actionType);
        IMsRdpClientAdvancedSettings8 AdvancedSettings9 { get; }
        ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);
        #endregion
    }
    [ProxyGuid("89ACB528-2557-4D16-8625-226A30E97E9A", typeof(IMsRdpClientAdvancedSettings8Proxy))]
    public interface IMsRdpClientAdvancedSettings8 : IMsRdpClientAdvancedSettings7
    {
        #region IMsRdpClientAdvancedSettings8
        bool BandwidthDetection { set; get; }
        ClientSpec ClientProtocolSpec { set; get; }
        #endregion
    }
    [ProxyGuid("28904001-04B6-436C-A55B-0AF1A0883DC9", typeof(IMsRdpClient9Proxy))]
    public interface IMsRdpClient9 : IMsRdpClient8
    {
        #region IMsRdpClient9
        IMsRdpClientTransportSettings4 TransportSettings4 { get; }
        void SyncSessionDisplaySettings();
        void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor);
        void attachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        void detachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        #endregion
    }
    [ProxyGuid("011C3236-4D81-4515-9143-067AB630D299", typeof(IMsRdpClientTransportSettings4Proxy))]
    public interface IMsRdpClientTransportSettings4 : IMsRdpClientTransportSettings3
    {
        #region IMsRdpClientTransportSettings4
        uint GatewayBrokeringType { set; }
        #endregion
    }
    [ProxyGuid("7ED92C39-EB38-4927-A70A-708AC5A59321", typeof(IMsRdpClient10Proxy))]
    public interface IMsRdpClient10 : IMsRdpClient9
    {
        #region IMsRdpClient10
        ITSRemoteProgram3 RemoteProgram3 { get; }
        #endregion
    }
    [ProxyGuid("4B84EA77-ACEA-418C-881A-4A8C28AB1510", typeof(ITSRemoteProgram3Proxy))]
    public interface ITSRemoteProgram3 : ITSRemoteProgram2
    {
        #region ITSRemoteProgram3
        void ServerStartApp(BinaryString bstrAppUserModelId, BinaryString bstrArguments, bool vbExpandEnvVarInArgumentsOnServer);
        #endregion
    }
    [ProxyGuid("05293249-B28B-4BD8-BE64-1B2F496B910E", typeof(IMsRdpClientNonScriptable6Proxy))]
    public interface IMsRdpClientNonScriptable6 : IMsRdpClientNonScriptable5
    {
        #region IMsRdpClientNonScriptable6
        void SendLocation2D(double latitude, double longitude);
        void SendLocation3D(double latitude, double longitude, int altitude);
        #endregion
    }
    [ProxyGuid("71B4A60A-FE21-46D8-A39B-8E32BA0C5ECC", typeof(IMsRdpClientNonScriptable7Proxy))]
    public interface IMsRdpClientNonScriptable7 : IMsRdpClientNonScriptable6
    {
        #region IMsRdpClientNonScriptable7
        IMsRdpCameraRedirConfigCollection CameraRedirConfigCollection { get; }
        void DisableDpiCursorScalingForProcess();
        IMsRdpClipboard Clipboard { get; }
        #endregion
    }
    [ProxyGuid("AE45252B-AAAB-4504-B681-649D6073A37A", typeof(IMsRdpCameraRedirConfigCollectionProxy))]
    public interface IMsRdpCameraRedirConfigCollection
    {
        #region IMsRdpCameraRedirConfigCollection
        void Rescan();
        uint Count { get; }
        IMsRdpCameraRedirConfig get_ByIndex(uint index);
        IMsRdpCameraRedirConfig get_BySymbolicLink(BinaryString SymbolicLink);
        IMsRdpCameraRedirConfig get_ByInstanceId(BinaryString InstanceId);
        void AddConfig(BinaryString SymbolicLink, bool fRedirected);
        bool RedirectByDefault { set; get; }
        bool EncodeVideo { set; get; }
        CameraRedirEncodingQuality EncodingQuality { set; get; }
        #endregion
    }
    [ProxyGuid("2E769EE8-00C7-43DC-AFD9-235D75B72A40", typeof(IMsRdpClipboardProxy))]
    public interface IMsRdpClipboard
    {
        #region IMsRdpClipboard
        bool CanSyncLocalClipboardToRemoteSession();
        void SyncLocalClipboardToRemoteSession();
        bool CanSyncRemoteClipboardToLocalSession();
        void SyncRemoteClipboardToLocalSession();
        #endregion
    }
    [ProxyGuid("57D25668-625A-4905-BE4E-304CAA13F89C", typeof(IRemoteDesktopClientProxy))]
    public interface IRemoteDesktopClient /* : IDispatch */
    {
        #region IRemoteDesktopClient
        void Connect();
        void Disconnect();
        void Reconnect(uint width, uint height);
        IRemoteDesktopClientSettings Settings { get; }
        IRemoteDesktopClientActions Actions { get; }
        IRemoteDesktopClientTouchPointer TouchPointer { get; }
        void DeleteSavedCredentials(BinaryString serverName);
        void UpdateSessionDisplaySettings(uint width, uint height);
        void attachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        void detachEvent(BinaryString eventName, [MarshalAs(UnmanagedType.IDispatch)] object callback);
        #endregion
    }
    [ProxyGuid("48A0F2A7-2713-431F-BBAC-6F4558E7D64D", typeof(IRemoteDesktopClientSettingsProxy))]
    public interface IRemoteDesktopClientSettings /* : IDispatch */
    {
        #region IRemoteDesktopClientSettings
        void ApplySettings(BinaryString RdpFileContents);
        BinaryString RetrieveSettings();
        [return: MarshalAs(UnmanagedType.Struct)] object GetRdpProperty(BinaryString propertyName);
        void SetRdpProperty(BinaryString propertyName, [MarshalAs(UnmanagedType.Struct)] object Value);
        #endregion
    }
    [ProxyGuid("7D54BC4E-1028-45D4-8B0A-B9B6BFFBA176", typeof(IRemoteDesktopClientActionsProxy))]
    public interface IRemoteDesktopClientActions /* : IDispatch */
    {
        #region IRemoteDesktopClientActions
        void SuspendScreenUpdates();
        void ResumeScreenUpdates();
        void ExecuteRemoteAction(RemoteActionType remoteAction);
        BinaryString GetSnapshot(SnapshotEncodingType snapshotEncoding, SnapshotFormatType snapshotFormat, uint snapshotWidth, uint snapshotHeight);
        #endregion
    }
    [ProxyGuid("260EC22D-8CBC-44B5-9E88-2A37F6C93AE9", typeof(IRemoteDesktopClientTouchPointerProxy))]
    public interface IRemoteDesktopClientTouchPointer /* : IDispatch */
    {
        #region IRemoteDesktopClientTouchPointer
        bool Enabled { set; get; }
        bool EventsEnabled { set; get; }
        uint PointerSpeed { set; get; }
        #endregion
    }
    [ProxyGuid("079863B7-6D47-4105-8BFE-0CDCB360E67D", typeof(IRemoteDesktopClientEventsProxy))]
    public interface IRemoteDesktopClientEvents /* : IDispatch */
    {
        #region IRemoteDesktopClientEvents
        [PreserveSig] void OnConnecting();
        [PreserveSig] void OnConnected();
        [PreserveSig] void OnLoginCompleted();
        [PreserveSig] void OnDisconnected(int disconnectReason, int ExtendedDisconnectReason, BinaryString disconnectErrorMessage);
        [PreserveSig] void OnStatusChanged(int statusCode, BinaryString statusMessage);
        [PreserveSig] void OnAutoReconnecting(int disconnectReason, int ExtendedDisconnectReason, BinaryString disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount);
        [PreserveSig] void OnAutoReconnected();
        [PreserveSig] void OnDialogDisplaying();
        [PreserveSig] void OnDialogDismissed();
        [PreserveSig] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig] void OnAdminMessageReceived(BinaryString adminMessage);
        [PreserveSig] void OnKeyCombinationPressed(int keyCombination);
        [PreserveSig] void OnRemoteDesktopSizeChanged(int width, int height);
        [PreserveSig] void OnTouchPointerCursorMoved(int x, int y);
        #endregion
    }
    [ProxyGuid("60C3B9C8-9E92-4F5E-A3E7-604A912093EA", typeof(IMsRdpDeviceProxy))]
    public interface IMsRdpDevice
    {
        #region IMsRdpDevice
        BinaryString DeviceInstanceId { get; }
        BinaryString FriendlyName { get; }
        BinaryString DeviceDescription { get; }
        bool RedirectionState { set; get; }
        #endregion
    }
    [ProxyGuid("D28B5458-F694-47A8-8E61-40356A767E46", typeof(IMsRdpDriveProxy))]
    public interface IMsRdpDrive
    {
        #region IMsRdpDrive
        BinaryString Name { get; }
        bool RedirectionState { set; get; }
        #endregion
    }
    [ProxyGuid("09750604-D625-47C1-9FCD-F09F735705D7", typeof(IMsRdpCameraRedirConfigProxy))]
    public interface IMsRdpCameraRedirConfig
    {
        #region IMsRdpCameraRedirConfig
        BinaryString FriendlyName { get; }
        BinaryString SymbolicLink { get; }
        BinaryString InstanceId { get; }
        BinaryString ParentInstanceId { get; }
        bool Redirected { set; get; }
        bool DeviceExists { get; }
        #endregion
    }
}
