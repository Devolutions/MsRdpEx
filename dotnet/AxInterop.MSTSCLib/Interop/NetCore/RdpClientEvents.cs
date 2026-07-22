#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    [SupportedOSPlatform("windows")]
    public sealed partial class RdpClientEventSubscription : IDisposable
    {
        private readonly IConnectionPoint connectionPoint;
        private readonly RdpClientEventSink sink;
        private readonly int cookie;
        private bool disposed;

        internal RdpClientEventSubscription(IMsRdpClient client)
            : this(ProxyObject.Unpack(client) ?? throw new InvalidOperationException("The RDP client proxy is not initialized."), typeof(MsRdpEx.Interop.IMsTscAxEvents).GUID)
        {
        }

        internal RdpClientEventSubscription(IRemoteDesktopClient client)
            : this(ProxyObject.Unpack(client) ?? throw new InvalidOperationException("The Remote Desktop client proxy is not initialized."), typeof(MsRdpEx.Interop.IRemoteDesktopClientEvents).GUID)
        {
        }

        private RdpClientEventSubscription(object rawClient, Guid eventInterfaceId)
        {
            connectionPoint = GetConnectionPoint(rawClient, eventInterfaceId);
            sink = new RdpClientEventSink(this);
            connectionPoint.Advise(sink, out cookie);
        }

        public event Action? Connecting;
        public event Action? Connected;
        public event Action? LoginCompleted;
        public event Action<int>? Disconnected;
        public event Action<int>? FatalError;
        public event Action<int, int>? RemoteDesktopSizeChanged;
        public event Action? AutoReconnected;
        public event EventHandler<RdpClientDisconnectedEventArgs>? DisconnectedWithDetails;
        public event Action<int, string?>? StatusChanged;
        public event EventHandler<RdpClientAutoReconnectingEventArgs>? AutoReconnecting;
        public event Action? DialogDisplaying;
        public event Action? DialogDismissed;
        public event Action<uint, int, int>? NetworkStatusChanged;
        public event Action<string?>? AdminMessageReceived;
        public event Action<int>? KeyCombinationPressed;
        public event Action<int, int>? TouchPointerCursorMoved;

        public void Dispose()
        {
            if (disposed)
                return;

            connectionPoint.Unadvise(cookie);
            disposed = true;
        }

        private static unsafe IConnectionPoint GetConnectionPoint(object rawClient, Guid eventInterfaceId)
        {
            if (!ComWrappers.TryGetComInstance(rawClient, out nint unknown))
                throw new InvalidOperationException("Could not obtain an IUnknown pointer for the RDP client.");

            try
            {
                Guid containerId = typeof(IConnectionPointContainer).GUID;
                int hr = Marshal.QueryInterface(unknown, ref containerId, out nint container);
                Marshal.ThrowExceptionForHR(hr);

                try
                {
                    var connectionPointContainer = ComInterfaceMarshaller<IConnectionPointContainer>.ConvertToManaged((void*)container)!;
                    connectionPointContainer.FindConnectionPoint(in eventInterfaceId, out var connectionPoint);
                    return connectionPoint;
                }
                finally
                {
                    ComInterfaceMarshaller<IConnectionPointContainer>.Free((void*)container);
                }
            }
            finally
            {
                Marshal.Release(unknown);
            }
        }

        private void RaiseConnecting() => Connecting?.Invoke();
        private void RaiseConnected() => Connected?.Invoke();
        private void RaiseLoginCompleted() => LoginCompleted?.Invoke();
        private void RaiseDisconnected(int reason) => Disconnected?.Invoke(reason);
        private void RaiseFatalError(int errorCode) => FatalError?.Invoke(errorCode);
        private void RaiseRemoteDesktopSizeChanged(int width, int height) => RemoteDesktopSizeChanged?.Invoke(width, height);
        private void RaiseAutoReconnected() => AutoReconnected?.Invoke();
        private void RaiseDisconnectedWithDetails(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage) =>
            DisconnectedWithDetails?.Invoke(this, new(disconnectReason, extendedDisconnectReason, disconnectErrorMessage));
        private void RaiseStatusChanged(int statusCode, string? statusMessage) => StatusChanged?.Invoke(statusCode, statusMessage);
        private void RaiseAutoReconnecting(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage, bool? networkAvailable, int attemptCount, int? maxAttemptCount) =>
            AutoReconnecting?.Invoke(this, new(disconnectReason, extendedDisconnectReason, disconnectErrorMessage, networkAvailable, attemptCount, maxAttemptCount));
        private void RaiseDialogDisplaying() => DialogDisplaying?.Invoke();
        private void RaiseDialogDismissed() => DialogDismissed?.Invoke();
        private void RaiseNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => NetworkStatusChanged?.Invoke(qualityLevel, bandwidth, rtt);
        private void RaiseAdminMessageReceived(string? adminMessage) => AdminMessageReceived?.Invoke(adminMessage);
        private void RaiseKeyCombinationPressed(int keyCombination) => KeyCombinationPressed?.Invoke(keyCombination);
        private void RaiseTouchPointerCursorMoved(int x, int y) => TouchPointerCursorMoved?.Invoke(x, y);

        [GeneratedComClass]
        private sealed partial class RdpClientEventSink(RdpClientEventSubscription subscription) : MsRdpEx.Interop.IMsTscAxEvents, MsRdpEx.Interop.IRemoteDesktopClientEvents
        {
            public void OnConnecting() => subscription.RaiseConnecting();
            public void OnConnected() => subscription.RaiseConnected();
            public void OnLoginComplete() => subscription.RaiseLoginCompleted();
            public void OnDisconnected(int discReason) => subscription.RaiseDisconnected(discReason);
            public void OnEnterFullScreenMode() { }
            public void OnLeaveFullScreenMode() { }
            public void OnChannelReceivedData(BinaryStringRef chanName, BinaryStringRef data) { }
            public void OnRequestGoFullScreen() { }
            public void OnRequestLeaveFullScreen() { }
            public void OnFatalError(int errorCode) => subscription.RaiseFatalError(errorCode);
            public void OnWarning(int warningCode) { }
            public void OnRemoteDesktopSizeChange(int width, int height) => subscription.RaiseRemoteDesktopSizeChanged(width, height);
            public void OnIdleTimeoutNotification() { }
            public void OnRequestContainerMinimize() { }
            public void OnConfirmClose(out bool pfAllowClose) => pfAllowClose = true;
            public void OnReceivedTSPublicKey(BinaryStringRef publicKey, out bool pfContinueLogon) => pfContinueLogon = true;
            public void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus)
            {
                subscription.RaiseAutoReconnecting(disconnectReason, null, null, null, attemptCount, null);
                pArcContinueStatus = default;
            }
            public void OnAuthenticationWarningDisplayed() { }
            public void OnAuthenticationWarningDismissed() { }
            public void OnRemoteProgramResult(BinaryStringRef bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable) { }
            public void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation) { }
            public void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute) { }
            public void OnLogonError(int lError) { }
            public void OnFocusReleased(int iDirection) { }
            public void OnUserNameAcquired(BinaryStringRef bstrUserName) { }
            public void OnMouseInputModeChanged(bool fMouseModeRelative) { }
            public void OnServiceMessageReceived(BinaryStringRef serviceMessage) { }
            public void OnConnectionBarPullDown() { }
            public void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => subscription.RaiseNetworkStatusChanged(qualityLevel, bandwidth, rtt);
            public void OnDevicesButtonPressed() { }
            public void OnAutoReconnected() => subscription.RaiseAutoReconnected();
            public void OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount) =>
                subscription.RaiseAutoReconnecting(disconnectReason, null, null, networkAvailable, attemptCount, maxAttemptCount);
            public void OnLoginCompleted() => subscription.RaiseLoginCompleted();
            public void OnDisconnected(int disconnectReason, int extendedDisconnectReason, BinaryStringRef disconnectErrorMessage)
            {
                string? errorMessage = disconnectErrorMessage;
                subscription.RaiseDisconnected(disconnectReason);
                subscription.RaiseDisconnectedWithDetails(disconnectReason, extendedDisconnectReason, errorMessage);
            }
            public void OnStatusChanged(int statusCode, BinaryStringRef statusMessage) => subscription.RaiseStatusChanged(statusCode, statusMessage);
            public void OnAutoReconnecting(int disconnectReason, int extendedDisconnectReason, BinaryStringRef disconnectErrorMessage, bool networkAvailable, int attemptCount, int maxAttemptCount) =>
                subscription.RaiseAutoReconnecting(disconnectReason, extendedDisconnectReason, disconnectErrorMessage, networkAvailable, attemptCount, maxAttemptCount);
            public void OnDialogDisplaying() => subscription.RaiseDialogDisplaying();
            public void OnDialogDismissed() => subscription.RaiseDialogDismissed();
            public void OnAdminMessageReceived(BinaryStringRef adminMessage) => subscription.RaiseAdminMessageReceived(adminMessage);
            public void OnKeyCombinationPressed(int keyCombination) => subscription.RaiseKeyCombinationPressed(keyCombination);
            public void OnRemoteDesktopSizeChanged(int width, int height) => subscription.RaiseRemoteDesktopSizeChanged(width, height);
            public void OnTouchPointerCursorMoved(int x, int y) => subscription.RaiseTouchPointerCursorMoved(x, y);
            public void GetTypeInfoCount(nint pctinfo) { }
            public void GetTypeInfo(int iTInfo, int lcid, nint ppTInfo) { }
            public void GetIDsOfNames(nint riid, nint rgszNames, int cNames, int lcid, nint rgDispId) { }

            public unsafe void Invoke(int dispIdMember, nint riid, int lcid, short wFlags, nint pDispParams, nint pVarResult, nint pExcepInfo, nint puArgErr)
            {
                var arguments = pDispParams == 0 ? null : ((DispatchParameters*)pDispParams)->Arguments;
                uint argumentCount = pDispParams == 0 ? 0 : ((DispatchParameters*)pDispParams)->ArgumentCount;

                switch (dispIdMember)
                {
                    case 1:
                        subscription.RaiseConnecting();
                        break;
                    case 2:
                        subscription.RaiseConnected();
                        break;
                    case 3:
                        subscription.RaiseLoginCompleted();
                        break;
                    case 4 when TryGetInt32(arguments, argumentCount, 0, out int disconnectReason):
                        subscription.RaiseDisconnected(disconnectReason);
                        break;
                    case 10 when TryGetInt32(arguments, argumentCount, 0, out int errorCode):
                        subscription.RaiseFatalError(errorCode);
                        break;
                    case 12 when TryGetInt32(arguments, argumentCount, 1, out int width) &&
                                 TryGetInt32(arguments, argumentCount, 0, out int height):
                        subscription.RaiseRemoteDesktopSizeChanged(width, height);
                        break;
                    case 33:
                        subscription.RaiseAutoReconnected();
                        break;
                    case 750:
                        subscription.RaiseConnecting();
                        break;
                    case 751:
                        subscription.RaiseConnected();
                        break;
                    case 752:
                        subscription.RaiseLoginCompleted();
                        break;
                    case 753 when TryGetInt32(arguments, argumentCount, 2, out int modernDisconnectReason) &&
                                  TryGetInt32(arguments, argumentCount, 1, out int extendedDisconnectReason) &&
                                  TryGetString(arguments, argumentCount, 0, out string? disconnectErrorMessage):
                        subscription.RaiseDisconnected(modernDisconnectReason);
                        subscription.RaiseDisconnectedWithDetails(modernDisconnectReason, extendedDisconnectReason, disconnectErrorMessage);
                        break;
                    case 754 when TryGetInt32(arguments, argumentCount, 1, out int statusCode) &&
                                  TryGetString(arguments, argumentCount, 0, out string? statusMessage):
                        subscription.RaiseStatusChanged(statusCode, statusMessage);
                        break;
                    case 755 when TryGetInt32(arguments, argumentCount, 5, out int reconnectDisconnectReason) &&
                                  TryGetInt32(arguments, argumentCount, 4, out int reconnectExtendedDisconnectReason) &&
                                  TryGetString(arguments, argumentCount, 3, out string? reconnectErrorMessage) &&
                                  TryGetBoolean(arguments, argumentCount, 2, out bool networkAvailable) &&
                                  TryGetInt32(arguments, argumentCount, 1, out int attemptCount) &&
                                  TryGetInt32(arguments, argumentCount, 0, out int maxAttemptCount):
                        subscription.RaiseAutoReconnecting(reconnectDisconnectReason, reconnectExtendedDisconnectReason, reconnectErrorMessage, networkAvailable, attemptCount, maxAttemptCount);
                        break;
                    case 756:
                        subscription.RaiseAutoReconnected();
                        break;
                    case 757:
                        subscription.RaiseDialogDisplaying();
                        break;
                    case 758:
                        subscription.RaiseDialogDismissed();
                        break;
                    case 759 when TryGetInt32(arguments, argumentCount, 2, out int qualityLevel) &&
                                  TryGetInt32(arguments, argumentCount, 1, out int bandwidth) &&
                                  TryGetInt32(arguments, argumentCount, 0, out int rtt):
                        subscription.RaiseNetworkStatusChanged(unchecked((uint)qualityLevel), bandwidth, rtt);
                        break;
                    case 760 when TryGetString(arguments, argumentCount, 0, out string? adminMessage):
                        subscription.RaiseAdminMessageReceived(adminMessage);
                        break;
                    case 761 when TryGetInt32(arguments, argumentCount, 0, out int keyCombination):
                        subscription.RaiseKeyCombinationPressed(keyCombination);
                        break;
                    case 762 when TryGetInt32(arguments, argumentCount, 1, out int modernWidth) &&
                                  TryGetInt32(arguments, argumentCount, 0, out int modernHeight):
                        subscription.RaiseRemoteDesktopSizeChanged(modernWidth, modernHeight);
                        break;
                    case 800 when TryGetInt32(arguments, argumentCount, 1, out int x) &&
                                  TryGetInt32(arguments, argumentCount, 0, out int y):
                        subscription.RaiseTouchPointerCursorMoved(x, y);
                        break;
                }
            }

            private static unsafe bool TryGetInt32(NativeVariant* arguments, uint argumentCount, uint index, out int value)
            {
                if (arguments is null || index >= argumentCount)
                {
                    value = default;
                    return false;
                }

                var argument = arguments[index];
                switch (argument.Type)
                {
                    case VariantType.Int16:
                        value = (short)argument.Content1;
                        return true;
                    case VariantType.Int32:
                    case VariantType.OtherInt32:
                        value = (int)argument.Content1;
                        return true;
                    case VariantType.UInt16:
                        value = (ushort)argument.Content1;
                        return true;
                    case VariantType.UInt32:
                    case VariantType.OtherUInt32:
                        value = unchecked((int)(uint)argument.Content1);
                        return true;
                    default:
                        value = default;
                        return false;
                }
            }

            private static unsafe bool TryGetBoolean(NativeVariant* arguments, uint argumentCount, uint index, out bool value)
            {
                if (arguments is null || index >= argumentCount || arguments[index].Type != VariantType.Boolean)
                {
                    value = default;
                    return false;
                }

                value = (short)arguments[index].Content1 != 0;
                return true;
            }

            private static unsafe bool TryGetString(NativeVariant* arguments, uint argumentCount, uint index, out string? value)
            {
                if (arguments is null || index >= argumentCount || arguments[index].Type != VariantType.BinaryString)
                {
                    value = default;
                    return false;
                }

                value = arguments[index].Content1 == 0 ? null : Marshal.PtrToStringBSTR(arguments[index].Content1);
                return true;
            }

#pragma warning disable CS0649 // Fields are populated by the COM DISPPARAMS structure.
            private unsafe struct DispatchParameters
            {
                public NativeVariant* Arguments;
                public nint NamedArguments;
                public uint ArgumentCount;
                public uint NamedArgumentCount;
            }
#pragma warning restore CS0649
        }
    }

    [SupportedOSPlatform("windows")]
    public sealed class RdpClientDisconnectedEventArgs(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage) : EventArgs
    {
        public int DisconnectReason { get; } = disconnectReason;
        public int? ExtendedDisconnectReason { get; } = extendedDisconnectReason;
        public string? DisconnectErrorMessage { get; } = disconnectErrorMessage;
    }

    [SupportedOSPlatform("windows")]
    public sealed class RdpClientAutoReconnectingEventArgs(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage, bool? networkAvailable, int attemptCount, int? maxAttemptCount) : EventArgs
    {
        public int DisconnectReason { get; } = disconnectReason;
        public int? ExtendedDisconnectReason { get; } = extendedDisconnectReason;
        public string? DisconnectErrorMessage { get; } = disconnectErrorMessage;
        public bool? NetworkAvailable { get; } = networkAvailable;
        public int AttemptCount { get; } = attemptCount;
        public int? MaxAttemptCount { get; } = maxAttemptCount;
    }

    [SupportedOSPlatform("windows")]
    public static class RdpClientEvents
    {
        public static RdpClientEventSubscription Subscribe(this IMsRdpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            return new RdpClientEventSubscription(client);
        }

        public static RdpClientEventSubscription Subscribe(this IRemoteDesktopClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            return new RdpClientEventSubscription(client);
        }
    }
}

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("B196B284-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal unsafe partial interface IConnectionPointContainer
    {
        void EnumConnectionPoints(out nint connectionPoints);
        void FindConnectionPoint(in Guid eventInterfaceId, out IConnectionPoint connectionPoint);
    }

    [GeneratedComInterface]
    [Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal unsafe partial interface IConnectionPoint
    {
        void GetConnectionInterface(out Guid eventInterfaceId);
        void GetConnectionPointContainer(out nint connectionPointContainer);
        void Advise(IMsTscAxEvents sink, out int cookie);
        void Unadvise(int cookie);
        void EnumConnections(out nint connections);
    }
}
