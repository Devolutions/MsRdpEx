using System;
using System.Runtime.InteropServices;

namespace MSTSCLib
{
    [ComImport]
    [Guid("079863B7-6D47-4105-8BFE-0CDCB360E67D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IRemoteDesktopClientEvents
    {
        [DispId(750)] void OnConnecting();
        [DispId(751)] void OnConnected();
        [DispId(752)] void OnLoginCompleted();
        [DispId(753)] void OnDisconnected(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.BStr)] string disconnectErrorMessage);
        [DispId(754)] void OnStatusChanged(int statusCode, [MarshalAs(UnmanagedType.BStr)] string statusMessage);
        [DispId(755)] void OnAutoReconnecting(int disconnectReason, int ExtendedDisconnectReason, [MarshalAs(UnmanagedType.BStr)] string disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount);
        [DispId(756)] void OnAutoReconnected();
        [DispId(757)] void OnDialogDisplaying();
        [DispId(758)] void OnDialogDismissed();
        [DispId(759)] void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);
        [DispId(760)] void OnAdminMessageReceived([MarshalAs(UnmanagedType.BStr)] string adminMessage);
        [DispId(761)] void OnKeyCombinationPressed(int keyCombination);
        [DispId(762)] void OnRemoteDesktopSizeChanged(int width, int height);
        [DispId(800)] void OnTouchPointerCursorMoved(int x, int y);
    }
}
