using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAxEvents : IDispatch
    {
        [DispId(1)] void OnConnecting();
        [DispId(2)] void OnConnected();
        [DispId(3)] void OnLoginComplete();
        [DispId(4)] void OnDisconnected(int discReason);
        [DispId(5)] void OnEnterFullScreenMode();
        [DispId(6)] void OnLeaveFullScreenMode();
        [DispId(7)] void OnChannelReceivedData(ReadOnlyBinaryStringRef chanName, ReadOnlyBinaryStringRef data);
        [DispId(8)] void OnRequestGoFullScreen();
        [DispId(9)] void OnRequestLeaveFullScreen();
        [DispId(10)] void OnFatalError(int errorCode);
        [DispId(11)] void OnWarning(int warningCode);
        [DispId(12)] void OnRemoteDesktopSizeChange(int width, int height);
        [DispId(13)] void OnIdleTimeoutNotification();
        [DispId(14)] void OnRequestContainerMinimize();
        [DispId(15)] void OnConfirmClose(out VariantBool pfAllowClose);
        [DispId(16)] void OnReceivedTSPublicKey(ReadOnlyBinaryStringRef publicKey, out VariantBool pfContinueLogon);
        [DispId(17)] void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus);
        [DispId(18)] void OnAuthenticationWarningDisplayed();
        [DispId(19)] void OnAuthenticationWarningDismissed();
        [DispId(20)] void OnRemoteProgramResult(ReadOnlyBinaryStringRef bstrRemoteProgram, RemoteProgramResult lError, VariantBool vbIsExecutable);
        [DispId(21)] void OnRemoteProgramDisplayed(VariantBool vbDisplayed, uint uDisplayInformation);
        [DispId(29)] void OnRemoteWindowDisplayed(VariantBool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute);
        [DispId(22)] void OnLogonError(int lError);
        [DispId(23)] void OnFocusReleased(int iDirection);
        [DispId(24)] void OnUserNameAcquired(ReadOnlyBinaryStringRef bstrUserName);
        [DispId(26)] void OnMouseInputModeChanged(VariantBool fMouseModeRelative);
        [DispId(28)] void OnServiceMessageReceived(ReadOnlyBinaryStringRef serviceMessage);
        [DispId(30)] void OnConnectionBarPullDown();
        [DispId(32)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [DispId(35)] void OnDevicesButtonPressed();
        [DispId(33)] void OnAutoReconnected();
        [DispId(34)] void OnAutoReconnecting2(int disconnectReason, VariantBool networkAvailable, int attemptCount, int maxAttemptCount);
    }

    public enum AutoReconnectContinueState
    {
        autoReconnectContinueAutomatic,
        autoReconnectContinueStop,
        autoReconnectContinueManual,
    }

    public enum RemoteProgramResult
    {
        remoteAppResultOk,
        remoteAppResultLocked,
        remoteAppResultProtocolError,
        remoteAppResultNotInWhitelist,
        remoteAppResultNetworkPathDenied,
        remoteAppResultFileNotFound,
        remoteAppResultFailure,
        remoteAppResultHookNotLoaded,
    }

    public enum RemoteWindowDisplayedAttribute
    {
        remoteAppWindowNone,
        remoteAppWindowDisplayed,
        remoteAppShellIconDisplayed,
    }

    [GeneratedComInterface]
    [Guid("327BB5CD-834E-4400-AEF2-B30E15E5D682")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAx_Redist : IDispatch
    {
    }

    [GeneratedComInterface]
    [Guid("8C11EFAE-92C3-11D1-BC1E-00C04FA31489")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAx : IMsTscAx_Redist
    {
        void SetServer(ReadOnlyBinaryStringRef value);
        BinaryString GetServer();
        void SetDomain(ReadOnlyBinaryStringRef value);
        BinaryString GetDomain();
        void SetUserName(ReadOnlyBinaryStringRef value);
        BinaryString GetUserName();
        void SetDisconnectedText(ReadOnlyBinaryStringRef value);
        BinaryString GetDisconnectedText();
        void SetConnectingText(ReadOnlyBinaryStringRef value);
        BinaryString GetConnectingText();
        short GetConnected();
        void SetDesktopWidth(int value);
        int GetDesktopWidth();
        void SetDesktopHeight(int value);
        int GetDesktopHeight();
        void SetStartConnected(int value);
        int GetStartConnected();
        int GetHorizontalScrollBarVisible();
        int GetVerticalScrollBarVisible();
        void SetFullScreenTitle(ReadOnlyBinaryStringRef value);
        int GetCipherStrength();
        BinaryString GetVersion();
        int GetSecuredSettingsEnabled();
        IMsTscSecuredSettings GetSecuredSettings();
        IMsTscAdvancedSettings GetAdvancedSettings();
        IMsTscDebug GetDebugger();
        void Connect();
        void Disconnect();
        void CreateVirtualChannels(ReadOnlyBinaryStringRef newVal);
        void SendOnVirtualChannel(ReadOnlyBinaryStringRef chanName, ReadOnlyBinaryStringRef ChanData);
    }

    [GeneratedComInterface]
    [Guid("92B4A539-7115-4B7C-A5A9-E5D9EFC2780A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient : IMsTscAx
    {
        void SetColorDepth(int value);
        int GetColorDepth();
        IMsRdpClientAdvancedSettings GetAdvancedSettings2();
        IMsRdpClientSecuredSettings GetSecuredSettings2();
        ExtendedDisconnectReasonCode GetExtendedDisconnectReason();
        void SetFullScreen(VariantBool value);
        VariantBool GetFullScreen();
        void SetVirtualChannelOptions(ReadOnlyBinaryStringRef chanName, int chanOptions);
        int GetVirtualChannelOptions(ReadOnlyBinaryStringRef chanName);
        ControlCloseStatus RequestClose();
    }

    public enum ExtendedDisconnectReasonCode
    {
        exDiscReasonNoInfo = 0,
        exDiscReasonAPIInitiatedDisconnect = 1,
        exDiscReasonAPIInitiatedLogoff = 2,
        exDiscReasonServerIdleTimeout = 3,
        exDiscReasonServerLogonTimeout = 4,
        exDiscReasonReplacedByOtherConnection = 5,
        exDiscReasonOutOfMemory = 6,
        exDiscReasonServerDeniedConnection = 7,
        exDiscReasonServerDeniedConnectionFips = 8,
        exDiscReasonServerInsufficientPrivileges = 9,
        exDiscReasonServerFreshCredsRequired = 10,
        exDiscReasonRpcInitiatedDisconnectByUser = 11,
        exDiscReasonLogoffByUser = 12,
        exDiscReasonShutdown = 25,
        exDiscReasonReboot = 26,
        exDiscReasonLicenseInternal = 256,
        exDiscReasonLicenseNoLicenseServer = 257,
        exDiscReasonLicenseNoLicense = 258,
        exDiscReasonLicenseErrClientMsg = 259,
        exDiscReasonLicenseHwidDoesntMatchLicense = 260,
        exDiscReasonLicenseErrClientLicense = 261,
        exDiscReasonLicenseCantFinishProtocol = 262,
        exDiscReasonLicenseClientEndedProtocol = 263,
        exDiscReasonLicenseErrClientEncryption = 264,
        exDiscReasonLicenseCantUpgradeLicense = 265,
        exDiscReasonLicenseNoRemoteConnections = 266,
        exDiscReasonLicenseCreatingLicStoreAccDenied = 267,
        exDiscReasonRdpEncInvalidCredentials = 768,
        exDiscReasonProtocolRangeStart = 4096,
        exDiscReasonProtocolRangeEnd = 32767,
    }

    public enum ControlCloseStatus
    {
        controlCloseCanProceed,
        controlCloseWaitForEvents,
    }

    [GeneratedComInterface]
    [Guid("E7E17DC4-3B71-4BA7-A8E6-281FFADCA28F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient2 : IMsRdpClient
    {
        IMsRdpClientAdvancedSettings2 GetAdvancedSettings3();
        void SetConnectedStatusText(ReadOnlyBinaryStringRef value);
        BinaryString GetConnectedStatusText();
    }

    [GeneratedComInterface]
    [Guid("91B7CBC5-A72E-4FA0-9300-D647D7E897FF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient3 : IMsRdpClient2
    {
        IMsRdpClientAdvancedSettings3 GetAdvancedSettings4();
    }

    [GeneratedComInterface]
    [Guid("095E0738-D97D-488B-B9F6-DD0E8D66C0DE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient4 : IMsRdpClient3
    {
        IMsRdpClientAdvancedSettings4 GetAdvancedSettings5();
    }

    [GeneratedComInterface]
    [Guid("4EB5335B-6429-477D-B922-E06A28ECD8BF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient5 : IMsRdpClient4
    {
        IMsRdpClientTransportSettings GetTransportSettings();
        IMsRdpClientAdvancedSettings5 GetAdvancedSettings6();
        BinaryString GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);
        ITSRemoteProgram GetRemoteProgram();
        IMsRdpClientShell GetMsRdpClientShell();
    }

    [GeneratedComInterface]
    [Guid("D43B7D80-8517-4B6D-9EAC-96AD6800D7F2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient6 : IMsRdpClient5
    {
        IMsRdpClientAdvancedSettings6 GetAdvancedSettings7();
        IMsRdpClientTransportSettings2 GetTransportSettings2();
    }

    [GeneratedComInterface]
    [Guid("B2A5B5CE-3461-444A-91D4-ADD26D070638")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient7 : IMsRdpClient6
    {
        IMsRdpClientAdvancedSettings7 GetAdvancedSettings8();
        IMsRdpClientTransportSettings3 GetTransportSettings3();
        BinaryString GetStatusText(uint statusCode);
        IMsRdpClientSecuredSettings2 GetSecuredSettings3();
        ITSRemoteProgram2 GetRemoteProgram2();
    }

    [GeneratedComInterface]
    [Guid("4247E044-9271-43A9-BC49-E2AD9E855D62")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient8 : IMsRdpClient7
    {
        void SendRemoteAction(RemoteSessionActionType actionType);
        IMsRdpClientAdvancedSettings8 GetAdvancedSettings9();
        ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);
    }

    public enum RemoteSessionActionType
    {
        RemoteSessionActionCharms,
        RemoteSessionActionAppbar,
        RemoteSessionActionSnap,
        RemoteSessionActionStartScreen,
        RemoteSessionActionAppSwitch,
        RemoteSessionActionActionCenter,
    }

    public enum ControlReconnectStatus
    {
        controlReconnectStarted,
        controlReconnectBlocked,
    }

    [GeneratedComInterface]
    [Guid("28904001-04B6-436C-A55B-0AF1A0883DC9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient9 : IMsRdpClient8
    {
        IMsRdpClientTransportSettings4 GetTransportSettings4();
        void SyncSessionDisplaySettings();
        void UpdateSessionDisplaySettings(uint DesktopWidth, uint DesktopHeight, uint PhysicalWidth, uint PhysicalHeight, uint Orientation, uint DesktopScaleFactor, uint DeviceScaleFactor);
        void AttachEvent(ReadOnlyBinaryStringRef eventName, IDispatch callback);
        void DetachEvent(ReadOnlyBinaryStringRef eventName, IDispatch callback);
    }

    [GeneratedComInterface]
    [Guid("7ED92C39-EB38-4927-A70A-708AC5A59321")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient10 : IMsRdpClient9
    {
        ITSRemoteProgram3 GetRemoteProgram3();
    }
}
