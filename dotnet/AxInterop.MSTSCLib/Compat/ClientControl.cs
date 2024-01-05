using System;
using System.Runtime.InteropServices;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    [ComImport]
    [Guid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IMsTscAxEvents
    {
        [DispId(1)] void OnConnecting();
        [DispId(2)] void OnConnected();
        [DispId(3)] void OnLoginComplete();
        [DispId(4)] void OnDisconnected(int discReason);
        [DispId(5)] void OnEnterFullScreenMode();
        [DispId(6)] void OnLeaveFullScreenMode();
        [DispId(7)] void OnChannelReceivedData(string chanName, string data);
        [DispId(8)] void OnRequestGoFullScreen();
        [DispId(9)] void OnRequestLeaveFullScreen();
        [DispId(10)] void OnFatalError(int errorCode);
        [DispId(11)] void OnWarning(int warningCode);
        [DispId(12)] void OnRemoteDesktopSizeChange(int width, int height);
        [DispId(13)] void OnIdleTimeoutNotification();
        [DispId(14)] void OnRequestContainerMinimize();
        [DispId(15)] void OnConfirmClose(out bool pfAllowClose);
        [DispId(16)] void OnReceivedTSPublicKey(string publicKey, out bool pfContinueLogon);
        [DispId(17)] void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus);
        [DispId(18)] void OnAuthenticationWarningDisplayed();
        [DispId(19)] void OnAuthenticationWarningDismissed();
        [DispId(20)] void OnRemoteProgramResult(string bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable);
        [DispId(21)] void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation);
        [DispId(29)] void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute);
        [DispId(22)] void OnLogonError(int lError);
        [DispId(23)] void OnFocusReleased(int iDirection);
        [DispId(24)] void OnUserNameAcquired(string bstrUserName);
        [DispId(26)] void OnMouseInputModeChanged(bool fMouseModeRelative);
        [DispId(28)] void OnServiceMessageReceived(string serviceMessage);
        [DispId(30)] void OnConnectionBarPullDown();
        [DispId(32)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [DispId(35)] void OnDevicesButtonPressed();
        [DispId(33)] void OnAutoReconnected();
        [DispId(34)] void OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount);
    }
}
