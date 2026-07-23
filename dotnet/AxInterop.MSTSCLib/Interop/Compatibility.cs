using System;
using System.Runtime.InteropServices;
using MSTSCLib;

namespace MsRdpEx.Interop.Compatibility
{
    [ComImport]
    [Guid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IMsTscAxEvents
    {
        #region IMsTscAxEvents
        [PreserveSig, DispId(1)] void OnConnecting();
        [PreserveSig, DispId(2)] void OnConnected();
        [PreserveSig, DispId(3)] void OnLoginComplete();
        [PreserveSig, DispId(4)] void OnDisconnected(int discReason);
        [PreserveSig, DispId(5)] void OnEnterFullScreenMode();
        [PreserveSig, DispId(6)] void OnLeaveFullScreenMode();
        [PreserveSig, DispId(7)] void OnChannelReceivedData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString chanName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString data);
        [PreserveSig, DispId(8)] void OnRequestGoFullScreen();
        [PreserveSig, DispId(9)] void OnRequestLeaveFullScreen();
        [PreserveSig, DispId(10)] void OnFatalError(int errorCode);
        [PreserveSig, DispId(11)] void OnWarning(int warningCode);
        [PreserveSig, DispId(12)] void OnRemoteDesktopSizeChange(int width, int height);
        [PreserveSig, DispId(13)] void OnIdleTimeoutNotification();
        [PreserveSig, DispId(14)] void OnRequestContainerMinimize();
        [PreserveSig, DispId(15)] void OnConfirmClose(out bool pfAllowClose);
        [PreserveSig, DispId(16)] void OnReceivedTSPublicKey([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString publicKey, out bool pfContinueLogon);
        [PreserveSig, DispId(17)] void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus);
        [PreserveSig, DispId(18)] void OnAuthenticationWarningDisplayed();
        [PreserveSig, DispId(19)] void OnAuthenticationWarningDismissed();
        [PreserveSig, DispId(20)] void OnRemoteProgramResult([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable);
        [PreserveSig, DispId(21)] void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation);
        [PreserveSig, DispId(29)] void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute);
        [PreserveSig, DispId(22)] void OnLogonError(int lError);
        [PreserveSig, DispId(23)] void OnFocusReleased(int iDirection);
        [PreserveSig, DispId(24)] void OnUserNameAcquired([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString bstrUserName);
        [PreserveSig, DispId(26)] void OnMouseInputModeChanged(bool fMouseModeRelative);
        [PreserveSig, DispId(28)] void OnServiceMessageReceived([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString serviceMessage);
        [PreserveSig, DispId(30)] void OnConnectionBarPullDown();
        [PreserveSig, DispId(32)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig, DispId(35)] void OnDevicesButtonPressed();
        [PreserveSig, DispId(33)] void OnAutoReconnected();
        [PreserveSig, DispId(34)] void OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount);
        #endregion
    }

    [ComImport]
    [Guid("079863B7-6D47-4105-8BFE-0CDCB360E67D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IRemoteDesktopClientEvents
    {
        #region IRemoteDesktopClientEvents
        [PreserveSig, DispId(750)] void OnConnecting();
        [PreserveSig, DispId(751)] void OnConnected();
        [PreserveSig, DispId(752)] void OnLoginCompleted();
        [PreserveSig, DispId(753)] void OnDisconnected(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString disconnectErrorMessage);
        [PreserveSig, DispId(754)] void OnStatusChanged(int statusCode, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString statusMessage);
        [PreserveSig, DispId(755)] void OnAutoReconnecting(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount);
        [PreserveSig, DispId(756)] void OnAutoReconnected();
        [PreserveSig, DispId(757)] void OnDialogDisplaying();
        [PreserveSig, DispId(758)] void OnDialogDismissed();
        [PreserveSig, DispId(759)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [PreserveSig, DispId(760)] void OnAdminMessageReceived([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] BinaryString adminMessage);
        [PreserveSig, DispId(761)] void OnKeyCombinationPressed(int keyCombination);
        [PreserveSig, DispId(762)] void OnRemoteDesktopSizeChanged(int width, int height);
        [PreserveSig, DispId(800)] void OnTouchPointerCursorMoved(int x, int y);
        #endregion
    }
}
