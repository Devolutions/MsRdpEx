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
        public event Action? EnteredFullScreen;
        public event Action? LeftFullScreen;
        public event Action<BinaryString?, BinaryString?>? ChannelDataReceived;
        public event Action? FullScreenRequested;
        public event Action? LeaveFullScreenRequested;
        public event Action<int>? FatalError;
        public event Action<int>? Warning;
        public event Action<int, int>? RemoteDesktopSizeChanged;
        public event Action? IdleTimeoutNotification;
        public event Action? ContainerMinimizeRequested;
        public event EventHandler<RdpClientConfirmCloseEventArgs>? ConfirmClose;
        public event EventHandler<RdpClientPublicKeyEventArgs>? TSPublicKeyReceived;
        public event EventHandler<RdpClientLegacyAutoReconnectingEventArgs>? LegacyAutoReconnecting;
        public event Action? AuthenticationWarningDisplayed;
        public event Action? AuthenticationWarningDismissed;
        public event Action<BinaryString?, RemoteProgramResult, bool>? RemoteProgramResultReceived;
        public event Action<bool, uint>? RemoteProgramDisplayed;
        public event Action<bool, nint, RemoteWindowDisplayedAttribute>? RemoteWindowDisplayed;
        public event Action<int>? LogonError;
        public event Action<int>? FocusReleased;
        public event Action<BinaryString?>? UserNameAcquired;
        public event Action<bool>? MouseInputModeChanged;
        public event Action<BinaryString?>? ServiceMessageReceived;
        public event Action? ConnectionBarPullDown;
        public event Action<uint, int, int>? NetworkStatusChanged;
        public event Action? DevicesButtonPressed;
        public event Action? AutoReconnected;
        public event EventHandler<RdpClientDisconnectedEventArgs>? DisconnectedWithDetails;
        public event Action<int, string?>? StatusChanged;
        public event EventHandler<RdpClientAutoReconnectingEventArgs>? AutoReconnecting;
        public event Action? DialogDisplaying;
        public event Action? DialogDismissed;
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
        private void RaiseEnteredFullScreen() => EnteredFullScreen?.Invoke();
        private void RaiseLeftFullScreen() => LeftFullScreen?.Invoke();
        private void RaiseChannelDataReceived(BinaryString? channelName, BinaryString? data) => ChannelDataReceived?.Invoke(channelName, data);
        private void RaiseFullScreenRequested() => FullScreenRequested?.Invoke();
        private void RaiseLeaveFullScreenRequested() => LeaveFullScreenRequested?.Invoke();
        private void RaiseFatalError(int errorCode) => FatalError?.Invoke(errorCode);
        private void RaiseWarning(int warningCode) => Warning?.Invoke(warningCode);
        private void RaiseRemoteDesktopSizeChanged(int width, int height) => RemoteDesktopSizeChanged?.Invoke(width, height);
        private void RaiseIdleTimeoutNotification() => IdleTimeoutNotification?.Invoke();
        private void RaiseContainerMinimizeRequested() => ContainerMinimizeRequested?.Invoke();

        private bool RaiseConfirmClose()
        {
            var args = new RdpClientConfirmCloseEventArgs();
            ConfirmClose?.Invoke(this, args);
            return args.AllowClose;
        }

        private bool RaiseTSPublicKeyReceived(BinaryString? publicKey)
        {
            var args = new RdpClientPublicKeyEventArgs(publicKey);
            TSPublicKeyReceived?.Invoke(this, args);
            return args.ContinueLogon;
        }

        private AutoReconnectContinueState RaiseLegacyAutoReconnecting(int disconnectReason, int attemptCount)
        {
            var args = new RdpClientLegacyAutoReconnectingEventArgs(disconnectReason, attemptCount);
            LegacyAutoReconnecting?.Invoke(this, args);
            RaiseAutoReconnecting(disconnectReason, null, null, null, attemptCount, null);
            return args.ContinueStatus;
        }

        private void RaiseAuthenticationWarningDisplayed() => AuthenticationWarningDisplayed?.Invoke();
        private void RaiseAuthenticationWarningDismissed() => AuthenticationWarningDismissed?.Invoke();
        private void RaiseRemoteProgramResult(BinaryString? remoteProgram, RemoteProgramResult error, bool isExecutable) => RemoteProgramResultReceived?.Invoke(remoteProgram, error, isExecutable);
        private void RaiseRemoteProgramDisplayed(bool displayed, uint displayInformation) => RemoteProgramDisplayed?.Invoke(displayed, displayInformation);
        private void RaiseRemoteWindowDisplayed(bool displayed, nint windowHandle, RemoteWindowDisplayedAttribute windowAttribute) => RemoteWindowDisplayed?.Invoke(displayed, windowHandle, windowAttribute);
        private void RaiseLogonError(int error) => LogonError?.Invoke(error);
        private void RaiseFocusReleased(int direction) => FocusReleased?.Invoke(direction);
        private void RaiseUserNameAcquired(BinaryString? userName) => UserNameAcquired?.Invoke(userName);
        private void RaiseMouseInputModeChanged(bool mouseModeRelative) => MouseInputModeChanged?.Invoke(mouseModeRelative);
        private void RaiseServiceMessageReceived(BinaryString? serviceMessage) => ServiceMessageReceived?.Invoke(serviceMessage);
        private void RaiseConnectionBarPullDown() => ConnectionBarPullDown?.Invoke();
        private void RaiseNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => NetworkStatusChanged?.Invoke(qualityLevel, bandwidth, rtt);
        private void RaiseDevicesButtonPressed() => DevicesButtonPressed?.Invoke();
        private void RaiseAutoReconnected() => AutoReconnected?.Invoke();
        private void RaiseDisconnectedWithDetails(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage) =>
            DisconnectedWithDetails?.Invoke(this, new(disconnectReason, extendedDisconnectReason, disconnectErrorMessage));
        private void RaiseStatusChanged(int statusCode, string? statusMessage) => StatusChanged?.Invoke(statusCode, statusMessage);
        private void RaiseAutoReconnecting(int disconnectReason, int? extendedDisconnectReason, string? disconnectErrorMessage, bool? networkAvailable, int attemptCount, int? maxAttemptCount) =>
            AutoReconnecting?.Invoke(this, new(disconnectReason, extendedDisconnectReason, disconnectErrorMessage, networkAvailable, attemptCount, maxAttemptCount));
        private void RaiseDialogDisplaying() => DialogDisplaying?.Invoke();
        private void RaiseDialogDismissed() => DialogDismissed?.Invoke();
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
            public void OnEnterFullScreenMode() => subscription.RaiseEnteredFullScreen();
            public void OnLeaveFullScreenMode() => subscription.RaiseLeftFullScreen();
            public void OnChannelReceivedData(BinaryStringRef chanName, BinaryStringRef data) => subscription.RaiseChannelDataReceived(chanName, data);
            public void OnRequestGoFullScreen() => subscription.RaiseFullScreenRequested();
            public void OnRequestLeaveFullScreen() => subscription.RaiseLeaveFullScreenRequested();
            public void OnFatalError(int errorCode) => subscription.RaiseFatalError(errorCode);
            public void OnWarning(int warningCode) => subscription.RaiseWarning(warningCode);
            public void OnRemoteDesktopSizeChange(int width, int height) => subscription.RaiseRemoteDesktopSizeChanged(width, height);
            public void OnIdleTimeoutNotification() => subscription.RaiseIdleTimeoutNotification();
            public void OnRequestContainerMinimize() => subscription.RaiseContainerMinimizeRequested();
            public void OnConfirmClose(out bool pfAllowClose) => pfAllowClose = subscription.RaiseConfirmClose();
            public void OnReceivedTSPublicKey(BinaryStringRef publicKey, out bool pfContinueLogon) => pfContinueLogon = subscription.RaiseTSPublicKeyReceived(publicKey);
            public void OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus) =>
                pArcContinueStatus = subscription.RaiseLegacyAutoReconnecting(disconnectReason, attemptCount);
            public void OnAuthenticationWarningDisplayed() => subscription.RaiseAuthenticationWarningDisplayed();
            public void OnAuthenticationWarningDismissed() => subscription.RaiseAuthenticationWarningDismissed();
            public void OnRemoteProgramResult(BinaryStringRef bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable) =>
                subscription.RaiseRemoteProgramResult(bstrRemoteProgram, lError, vbIsExecutable);
            public void OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation) => subscription.RaiseRemoteProgramDisplayed(vbDisplayed, uDisplayInformation);
            public void OnRemoteWindowDisplayed(bool vbDisplayed, nint hwnd, RemoteWindowDisplayedAttribute windowAttribute) => subscription.RaiseRemoteWindowDisplayed(vbDisplayed, hwnd, windowAttribute);
            public void OnLogonError(int lError) => subscription.RaiseLogonError(lError);
            public void OnFocusReleased(int iDirection) => subscription.RaiseFocusReleased(iDirection);
            public void OnUserNameAcquired(BinaryStringRef bstrUserName) => subscription.RaiseUserNameAcquired(bstrUserName);
            public void OnMouseInputModeChanged(bool fMouseModeRelative) => subscription.RaiseMouseInputModeChanged(fMouseModeRelative);
            public void OnServiceMessageReceived(BinaryStringRef serviceMessage) => subscription.RaiseServiceMessageReceived(serviceMessage);
            public void OnConnectionBarPullDown() => subscription.RaiseConnectionBarPullDown();
            public void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt) => subscription.RaiseNetworkStatusChanged(qualityLevel, bandwidth, rtt);
            public void OnDevicesButtonPressed() => subscription.RaiseDevicesButtonPressed();
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
                    case 1: subscription.RaiseConnecting(); break;
                    case 2: subscription.RaiseConnected(); break;
                    case 3: subscription.RaiseLoginCompleted(); break;
                    case 4 when TryGetInt32(arguments, argumentCount, 0, out int disconnectReason): subscription.RaiseDisconnected(disconnectReason); break;
                    case 5: subscription.RaiseEnteredFullScreen(); break;
                    case 6: subscription.RaiseLeftFullScreen(); break;
                    case 7 when TryGetBinaryString(arguments, argumentCount, 1, out BinaryString? channelName) && TryGetBinaryString(arguments, argumentCount, 0, out BinaryString? channelData):
                        subscription.RaiseChannelDataReceived(channelName, channelData); break;
                    case 8: subscription.RaiseFullScreenRequested(); break;
                    case 9: subscription.RaiseLeaveFullScreenRequested(); break;
                    case 10 when TryGetInt32(arguments, argumentCount, 0, out int errorCode): subscription.RaiseFatalError(errorCode); break;
                    case 11 when TryGetInt32(arguments, argumentCount, 0, out int warningCode): subscription.RaiseWarning(warningCode); break;
                    case 12 when TryGetInt32(arguments, argumentCount, 1, out int width) && TryGetInt32(arguments, argumentCount, 0, out int height):
                        subscription.RaiseRemoteDesktopSizeChanged(width, height); break;
                    case 13: subscription.RaiseIdleTimeoutNotification(); break;
                    case 14: subscription.RaiseContainerMinimizeRequested(); break;
                    case 15: TrySetBoolean(arguments, argumentCount, 0, subscription.RaiseConfirmClose()); break;
                    case 16 when TryGetBinaryString(arguments, argumentCount, 1, out BinaryString? publicKey):
                        TrySetBoolean(arguments, argumentCount, 0, subscription.RaiseTSPublicKeyReceived(publicKey)); break;
                    case 17 when TryGetInt32(arguments, argumentCount, 2, out int legacyDisconnectReason) && TryGetInt32(arguments, argumentCount, 1, out int legacyAttemptCount):
                        TrySetAutoReconnectState(arguments, argumentCount, 0, subscription.RaiseLegacyAutoReconnecting(legacyDisconnectReason, legacyAttemptCount)); break;
                    case 18: subscription.RaiseAuthenticationWarningDisplayed(); break;
                    case 19: subscription.RaiseAuthenticationWarningDismissed(); break;
                    case 20 when TryGetBinaryString(arguments, argumentCount, 2, out BinaryString? remoteProgram) && TryGetInt32(arguments, argumentCount, 1, out int remoteProgramResult) && TryGetBoolean(arguments, argumentCount, 0, out bool isExecutable):
                        subscription.RaiseRemoteProgramResult(remoteProgram, (RemoteProgramResult)remoteProgramResult, isExecutable); break;
                    case 21 when TryGetBoolean(arguments, argumentCount, 1, out bool isDisplayed) && TryGetInt32(arguments, argumentCount, 0, out int displayInformation):
                        subscription.RaiseRemoteProgramDisplayed(isDisplayed, unchecked((uint)displayInformation)); break;
                    case 22 when TryGetInt32(arguments, argumentCount, 0, out int logonError): subscription.RaiseLogonError(logonError); break;
                    case 23 when TryGetInt32(arguments, argumentCount, 0, out int focusDirection): subscription.RaiseFocusReleased(focusDirection); break;
                    case 24 when TryGetBinaryString(arguments, argumentCount, 0, out BinaryString? userName): subscription.RaiseUserNameAcquired(userName); break;
                    case 26 when TryGetBoolean(arguments, argumentCount, 0, out bool mouseModeRelative): subscription.RaiseMouseInputModeChanged(mouseModeRelative); break;
                    case 28 when TryGetBinaryString(arguments, argumentCount, 0, out BinaryString? serviceMessage): subscription.RaiseServiceMessageReceived(serviceMessage); break;
                    case 29 when TryGetBoolean(arguments, argumentCount, 2, out bool isWindowDisplayed) && TryGetPointer(arguments, argumentCount, 1, out nint windowHandle) && TryGetInt32(arguments, argumentCount, 0, out int windowAttribute):
                        subscription.RaiseRemoteWindowDisplayed(isWindowDisplayed, windowHandle, (RemoteWindowDisplayedAttribute)windowAttribute); break;
                    case 30: subscription.RaiseConnectionBarPullDown(); break;
                    case 32 when TryGetInt32(arguments, argumentCount, 2, out int qualityLevel) && TryGetInt32(arguments, argumentCount, 1, out int bandwidth) && TryGetInt32(arguments, argumentCount, 0, out int rtt):
                        subscription.RaiseNetworkStatusChanged(unchecked((uint)qualityLevel), bandwidth, rtt); break;
                    case 33: subscription.RaiseAutoReconnected(); break;
                    case 34 when TryGetInt32(arguments, argumentCount, 3, out int reconnectReason) && TryGetBoolean(arguments, argumentCount, 2, out bool networkAvailable) && TryGetInt32(arguments, argumentCount, 1, out int reconnectAttemptCount) && TryGetInt32(arguments, argumentCount, 0, out int maxReconnectAttemptCount):
                        subscription.RaiseAutoReconnecting(reconnectReason, null, null, networkAvailable, reconnectAttemptCount, maxReconnectAttemptCount); break;
                    case 35: subscription.RaiseDevicesButtonPressed(); break;
                    case 750: subscription.RaiseConnecting(); break;
                    case 751: subscription.RaiseConnected(); break;
                    case 752: subscription.RaiseLoginCompleted(); break;
                    case 753 when TryGetInt32(arguments, argumentCount, 2, out int modernDisconnectReason) && TryGetInt32(arguments, argumentCount, 1, out int extendedDisconnectReason) && TryGetString(arguments, argumentCount, 0, out string? disconnectErrorMessage):
                        subscription.RaiseDisconnected(modernDisconnectReason);
                        subscription.RaiseDisconnectedWithDetails(modernDisconnectReason, extendedDisconnectReason, disconnectErrorMessage); break;
                    case 754 when TryGetInt32(arguments, argumentCount, 1, out int statusCode) && TryGetString(arguments, argumentCount, 0, out string? statusMessage):
                        subscription.RaiseStatusChanged(statusCode, statusMessage); break;
                    case 755 when TryGetInt32(arguments, argumentCount, 5, out int reconnectDisconnectReason) && TryGetInt32(arguments, argumentCount, 4, out int reconnectExtendedDisconnectReason) && TryGetString(arguments, argumentCount, 3, out string? reconnectErrorMessage) && TryGetBoolean(arguments, argumentCount, 2, out bool reconnectNetworkAvailable) && TryGetInt32(arguments, argumentCount, 1, out int reconnectAttempt) && TryGetInt32(arguments, argumentCount, 0, out int maxReconnectAttempt):
                        subscription.RaiseAutoReconnecting(reconnectDisconnectReason, reconnectExtendedDisconnectReason, reconnectErrorMessage, reconnectNetworkAvailable, reconnectAttempt, maxReconnectAttempt); break;
                    case 756: subscription.RaiseAutoReconnected(); break;
                    case 757: subscription.RaiseDialogDisplaying(); break;
                    case 758: subscription.RaiseDialogDismissed(); break;
                    case 759 when TryGetInt32(arguments, argumentCount, 2, out int modernQualityLevel) && TryGetInt32(arguments, argumentCount, 1, out int modernBandwidth) && TryGetInt32(arguments, argumentCount, 0, out int modernRtt):
                        subscription.RaiseNetworkStatusChanged(unchecked((uint)modernQualityLevel), modernBandwidth, modernRtt); break;
                    case 760 when TryGetString(arguments, argumentCount, 0, out string? adminMessage): subscription.RaiseAdminMessageReceived(adminMessage); break;
                    case 761 when TryGetInt32(arguments, argumentCount, 0, out int keyCombination): subscription.RaiseKeyCombinationPressed(keyCombination); break;
                    case 762 when TryGetInt32(arguments, argumentCount, 1, out int modernWidth) && TryGetInt32(arguments, argumentCount, 0, out int modernHeight):
                        subscription.RaiseRemoteDesktopSizeChanged(modernWidth, modernHeight); break;
                    case 800 when TryGetInt32(arguments, argumentCount, 1, out int x) && TryGetInt32(arguments, argumentCount, 0, out int y):
                        subscription.RaiseTouchPointerCursorMoved(x, y); break;
                }
            }

            private static unsafe bool TryGetInt32(NativeVariant* arguments, uint argumentCount, uint index, out int value)
            {
                if (arguments is null || index >= argumentCount)
                {
                    value = default;
                    return false;
                }

                switch (arguments[index].Type)
                {
                    case VariantType.Int16: value = (short)arguments[index].Content1; return true;
                    case VariantType.Int32:
                    case VariantType.OtherInt32: value = (int)arguments[index].Content1; return true;
                    case VariantType.UInt16: value = (ushort)arguments[index].Content1; return true;
                    case VariantType.UInt32:
                    case VariantType.OtherUInt32: value = unchecked((int)(uint)arguments[index].Content1); return true;
                    default: value = default; return false;
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

            private static unsafe bool TryGetBinaryString(NativeVariant* arguments, uint argumentCount, uint index, out BinaryString? value)
            {
                if (arguments is null || index >= argumentCount || arguments[index].Type != VariantType.BinaryString)
                {
                    value = default;
                    return false;
                }

                value = BinaryString.Marshaller.ConvertToManaged(arguments[index].Content1);
                return true;
            }

            private static unsafe bool TryGetString(NativeVariant* arguments, uint argumentCount, uint index, out string? value)
            {
                if (!TryGetBinaryString(arguments, argumentCount, index, out BinaryString? binaryString))
                {
                    value = default;
                    return false;
                }

                value = binaryString;
                return true;
            }

            private static unsafe bool TryGetPointer(NativeVariant* arguments, uint argumentCount, uint index, out nint value)
            {
                if (arguments is not null && index < argumentCount && (arguments[index].Type == VariantType.IntPtr || arguments[index].Type == VariantType.UIntPtr))
                {
                    value = arguments[index].Content1;
                    return true;
                }

                value = default;
                return false;
            }

            private static unsafe bool TrySetBoolean(NativeVariant* arguments, uint argumentCount, uint index, bool value)
            {
                if (arguments is null || index >= argumentCount || arguments[index].Type != (VariantType.ByRefModifier | VariantType.Boolean) || arguments[index].Content1 == 0)
                    return false;

                *(short*)arguments[index].Content1 = value ? VariantBool.TrueValue : VariantBool.FalseValue;
                return true;
            }

            private static unsafe bool TrySetAutoReconnectState(NativeVariant* arguments, uint argumentCount, uint index, AutoReconnectContinueState value)
            {
                if (arguments is null || index >= argumentCount || arguments[index].Type != (VariantType.ByRefModifier | VariantType.Int32) || arguments[index].Content1 == 0)
                    return false;

                *(int*)arguments[index].Content1 = (int)value;
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
    public sealed class RdpClientConfirmCloseEventArgs : EventArgs
    {
        public bool AllowClose { get; set; } = true;
    }

    [SupportedOSPlatform("windows")]
    public sealed class RdpClientPublicKeyEventArgs(BinaryString? publicKey) : EventArgs
    {
        public BinaryString? PublicKey { get; } = publicKey;
        public bool ContinueLogon { get; set; } = true;
    }

    [SupportedOSPlatform("windows")]
    public sealed class RdpClientLegacyAutoReconnectingEventArgs(int disconnectReason, int attemptCount) : EventArgs
    {
        public int DisconnectReason { get; } = disconnectReason;
        public int AttemptCount { get; } = attemptCount;
        public AutoReconnectContinueState ContinueStatus { get; set; }
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
