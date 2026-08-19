using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Runtime.InteropServices;
using System.Text;
using static Devolutions.MsRdpEx.Avalonia.RdpActiveXSession;

namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Decides whether a key about to be forwarded to the remote session is
/// claimed by the hosting application instead. Return true to keep the key
/// local: it passes through to normal application processing (accelerators,
/// menu shortcuts) and is not sent remotely. Implementations should answer
/// consistently for a key press and its matching release.
/// </summary>
/// <param name="virtualKey">The Win32 virtual-key code.</param>
/// <param name="keyUp">True for the key release, false for the key press.</param>
/// <param name="systemKey">True when the key is a system (Alt) key message.</param>
public delegate bool RdpLocalKeyFilter(int virtualKey, bool keyUp, bool systemKey);

/// <summary>
/// A pure Avalonia RDP surface. Pixels are copied from MsRdpEx's shadow DIB
/// into a WriteableBitmap; no native HWND participates in the Avalonia visual
/// tree.
/// </summary>
public class RdpClientView : UserControl, IDisposable
{
    /// <summary>
    /// The Microsoft RDP Client Control version 11 class identifier.
    /// </summary>
    public static readonly Guid DefaultClassId = new("A0C63C30-F08D-4AB4-907C-34905D770C7D");

    private const int WhKeyboardLl = 13;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmSystemKeyDown = 0x0104;
    private const uint WmSystemKeyUp = 0x0105;
    private const uint LlkhfExtended = 0x01;
    private const uint CfHDrop = 15;
    private const uint GmemMoveable = 0x0002;
    private const uint GmemZeroInit = 0x0040;

    private readonly object frameLock = new();
    private readonly DispatcherTimer mouseDragTimer;
    private readonly DispatcherTimer resizeTimer;
    private readonly DispatcherTimer staCaptureTimer;
    private readonly LowLevelKeyboardProc keyboardHookProc;
    private RdpActiveXSession? session;
    private WriteableBitmap? frameBitmap;
    private WriteableBitmap? backFrameBitmap;
    private Rect frameDestination;
    private NativeMouseButtons pressedButtons;
    private readonly Dictionary<int, ForwardedKey> forwardedKeys = [];
    private readonly HashSet<int> localShortcutKeys = [];
    private readonly HashSet<int> claimedLocalKeys = [];
    private int lastRemoteX;
    private int lastRemoteY;
    private bool dynamicResolutionEnabled;
    private PixelSize pendingDesktopSize;
    private double pendingRenderScaling = 1.0;
    private TopLevel? topLevel;
    private nint keyboardHook;
    private bool sessionDisconnectedFired;
    private CancellationTokenSource? captureCancellation;
    private Task? captureTask;
    private int captureGeneration;
    private int invalidateQueued;
    private int captureFailureReported;
    private int resizeRetryCount;
    private PixelSize initialFrameTarget;
    private long initialFrameDeadline;
    private bool initialLoginCompleted;
    private bool waitingForInitialFrame;
    private bool isViewOnly;
    private bool viewportChangedQueued;
    private PixelSize viewportPixelSize;
    private uint staFrameVersion;
    private bool staHasFrameVersion;
    private bool disposed;

    public RdpClientView()
    {
        Focusable = true;
        ClipToBounds = true;
        keyboardHookProc = KeyboardHookProc;

        mouseDragTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        mouseDragTimer.Tick += OnMouseDragTimerTick;

        resizeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        resizeTimer.Tick += OnResizeTimerTick;

        staCaptureTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(33)
        };
        staCaptureTimer.Tick += OnStaCaptureTimerTick;
        GotFocus += OnControlGotFocus;
        LostFocus += OnControlLostFocus;
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnFileDragOver);
        AddHandler(DragDrop.DropEvent, OnFileDrop);
    }

    public bool IsClientReady => session?.IsReady == true;
    public bool IsConnectionActive => session?.IsConnectionActive == true;
    public bool IsLoginCompleted => session?.IsLoginCompleted == true;
    public nint HostWindowHandle => session?.HostWindowHandle ?? 0;
    public nint InputWindowHandle => session?.InputWindowHandle ?? 0;
    public PixelSize ViewportPixelSize => GetDesktopPixelSize();

    /// <summary>
    /// Gets or sets the RDP ActiveX class identifier. Set this before the view is attached.
    /// </summary>
    public Guid ClassId { get; set; } = DefaultClassId;

    /// <summary>
    /// Gets or sets the MsRdpEx ActiveX name. Set this before the view is attached.
    /// </summary>
    public string AxName { get; set; } = "mstsc";

    /// <summary>
    /// Gets or sets an optional path to MsRdpEx.dll. Relative paths are resolved from
    /// the application base directory. Set this before the view is attached.
    /// </summary>
    public string? RdpExDll { get; set; }

    /// <summary>
    /// Gets or sets whether Windows/system shortcuts are sent to the remote session.
    /// Set to false to keep Alt+Tab, Alt+F4, Windows keys, and similar shortcuts local.
    /// </summary>
    public bool UseRemoteKeyboardShortcuts { get; set; } = true;

    /// <summary>
    /// Gets or sets whether local filesystem drops are offered to the remote session
    /// through the Windows CF_HDROP clipboard format.
    /// </summary>
    public bool AllowFileDrop { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional filter that lets the hosting application claim
    /// keys before they are forwarded to the remote session. While the view is
    /// focused, the low-level keyboard hook forwards every key remotely; set
    /// this filter to keep the host's own accelerators (for example menu
    /// shortcuts) working. Claimed keys pass through to normal local
    /// processing and are never sent to the remote session.
    /// </summary>
    public RdpLocalKeyFilter? LocalKeyFilter { get; set; }

    /// <summary>
    /// Gets or sets whether pointer and keyboard input is suppressed while the
    /// remote desktop remains visible.
    /// </summary>
    public bool IsViewOnly
    {
        get => isViewOnly;
        set
        {
            if (isViewOnly == value)
                return;

            isViewOnly = value;
            if (value)
            {
                ReleaseForwardedKeys();
                ReleaseMouseButtons();
            }
        }
    }

    public event EventHandler? ClientReady;
    public event EventHandler? LoginCompleted;
    public event EventHandler<RdpRemoteDesktopSizeChangedEventArgs>? RemoteDesktopSizeChanged;
    public event EventHandler<RdpFocusReleasedEventArgs>? FocusReleased;
    public event EventHandler<RdpStatusChangedEventArgs>? StatusChanged;
    public event EventHandler? ViewportPixelSizeChanged;

    /// <summary>
    /// Creates the off-screen ActiveX control immediately. Calling this method is
    /// optional for normal visual-tree use, but lets an embedding application
    /// configure the generated COM interfaces before the view is attached.
    /// </summary>
    public void Initialize()
    {
        VerifyAccess();
        ObjectDisposedException.ThrowIf(disposed, this);
        if (session is not null)
            return;

        RdpActiveXSession activeSession = new();
        activeSession.Ready += OnSessionReady;
        activeSession.LoginCompleted += OnSessionLoginCompleted;
        activeSession.RemoteDesktopSizeChanged += OnSessionRemoteDesktopSizeChanged;
        activeSession.FocusReleased += OnSessionFocusReleased;
        activeSession.StatusChanged += OnSessionStatusChanged;
        activeSession.Disconnected += OnSessionDisconnected;

        // When this view is re-created after a previous view, the previous
        // session may own the OLE initialization while the new session got
        // S_FALSE (Avalonia already initialized OLE on this thread). Hand the
        // balance to the older session so disposing the new one never tears
        // down OLE underneath the survivor.
        session?.MarkOleUninitializePending();
        session = activeSession;

        try
        {
            PixelSize desktopSize = GetDesktopPixelSize();
            activeSession.Start(desktopSize.Width, desktopSize.Height, ClassId, AxName, RdpExDll);
        }
        catch
        {
            activeSession.Ready -= OnSessionReady;
            activeSession.LoginCompleted -= OnSessionLoginCompleted;
            activeSession.RemoteDesktopSizeChanged -= OnSessionRemoteDesktopSizeChanged;
            activeSession.FocusReleased -= OnSessionFocusReleased;
            activeSession.StatusChanged -= OnSessionStatusChanged;
            activeSession.Disconnected -= OnSessionDisconnected;
            activeSession.Dispose();
            session = null;
            throw;
        }
    }

    /// <summary>
    /// Gets a generated COM interface for advanced RDP client configuration.
    /// This is available after <see cref="ClientReady"/> has fired.
    /// </summary>
    public T GetClient<T>() where T : class
    {
        VerifyAccess();
        return (session ?? throw new InvalidOperationException("The RDP bitmap session is not ready yet."))
            .GetClient<T>();
    }

    public void Connect(RdpConnectionSettings settings)
    {
        VerifyAccess();

        RdpActiveXSession activeSession = session
            ?? throw new InvalidOperationException("The RDP bitmap session is not ready yet.");
        bool hasDesktopWidth = settings.DesktopWidth > 0;
        bool hasDesktopHeight = settings.DesktopHeight > 0;
        if (hasDesktopWidth != hasDesktopHeight)
        {
            throw new ArgumentException(
                "DesktopWidth and DesktopHeight must either both be positive or both be zero for dynamic resolution.",
                nameof(settings));
        }

        PixelSize viewportSize = GetDesktopPixelSize();
        dynamicResolutionEnabled = !hasDesktopWidth;
        int desktopWidth = settings.DesktopWidth > 0 ? settings.DesktopWidth : viewportSize.Width;
        int desktopHeight = settings.DesktopHeight > 0 ? settings.DesktopHeight : viewportSize.Height;
        PrepareInitialFrame(new PixelSize(
            Math.Clamp(desktopWidth, 200, 8192),
            Math.Clamp(desktopHeight, 200, 8192)));
        activeSession.Connect(settings, desktopWidth, desktopHeight);
    }

    /// <summary>
    /// Connects a client that the embedding application configured through
    /// <see cref="GetClient{T}"/>. The configured server, credentials, gateway,
    /// redirection, and security properties are left unchanged.
    /// </summary>
    public void ConnectConfigured(
        bool manageDynamicResolution = false,
        PixelSize? initialFrameSize = null)
    {
        VerifyAccess();

        RdpActiveXSession activeSession = session
            ?? throw new InvalidOperationException("The RDP bitmap session is not ready yet.");
        MSTSCLib.IMsRdpClient client = activeSession.GetClient<MSTSCLib.IMsRdpClient>();
        PixelSize viewportSize = GetDesktopPixelSize();
        int desktopWidth = client.DesktopWidth > 0 ? client.DesktopWidth : viewportSize.Width;
        int desktopHeight = client.DesktopHeight > 0 ? client.DesktopHeight : viewportSize.Height;
        dynamicResolutionEnabled = manageDynamicResolution;
        PixelSize targetSize = initialFrameSize ?? new PixelSize(desktopWidth, desktopHeight);
        PrepareInitialFrame(new PixelSize(
            Math.Clamp(targetSize.Width, 200, 8192),
            Math.Clamp(targetSize.Height, 200, 8192)));
        activeSession.ConnectConfigured(desktopWidth, desktopHeight);
    }

    public bool TryUpdateSessionDisplaySettings(
        uint desktopWidth,
        uint desktopHeight,
        uint physicalWidth,
        uint physicalHeight,
        uint orientation,
        uint desktopScaleFactor,
        uint deviceScaleFactor)
    {
        VerifyAccess();
        return session?.UpdateSessionDisplaySettings(
            desktopWidth,
            desktopHeight,
            physicalWidth,
            physicalHeight,
            orientation,
            desktopScaleFactor,
            deviceScaleFactor) == true;
    }

    public void Disconnect()
    {
        VerifyAccess();
        session?.Disconnect();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(Brushes.Black, Bounds);

        lock (frameLock)
        {
            if (frameBitmap is null)
            {
                frameDestination = default;
                return;
            }

            frameDestination = CalculateAspectFit(Bounds, frameBitmap.PixelSize);
            context.DrawImage(
                frameBitmap,
                new Rect(0, 0, frameBitmap.PixelSize.Width, frameBitmap.PixelSize.Height),
                frameDestination);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ObjectDisposedException.ThrowIf(disposed, this);
        topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is Window window)
        {
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            if (window.IsActive)
            {
                // Register before the first pointer press so a fast initial
                // keystroke cannot arrive before GotFocus installs the hook.
                InstallKeyboardHook();
            }
        }

        try
        {
            Initialize();
            UpdateOleFrameState();
            StartCaptureWorker();
        }
        catch (Exception exception)
        {
            PublishStatus($"RDP initialization failed: {exception.Message}");
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (topLevel is Window window)
        {
            window.Activated -= OnWindowActivated;
            window.Deactivated -= OnWindowDeactivated;
        }

        StopCaptureWorker();
        UninstallKeyboardHook();
        mouseDragTimer.Stop();
        resizeTimer.Stop();
        ReleaseForwardedKeys();
        ReleaseMouseButtons();
        // Deactivate the frame before dropping its window so the control is
        // not left believing a detached frame is still active.
        session?.SetFrameActive(false);
        session?.SetFrameWindow(0);
        topLevel = null;

        base.OnDetachedFromVisualTree(e);
    }

    // Reports the real top-level window to the OLE host so control-owned
    // dialogs (certificate warnings, credential prompts) are parented to a
    // visible window, and forwards only frame activation. Document activation
    // would focus the hidden ActiveX input HWND and steal physical input from
    // the Avalonia surface.
    private void UpdateOleFrameState()
    {
        if (session is null)
            return;

        if (topLevel?.TryGetPlatformHandle() is { } platformHandle)
            session.SetFrameWindow(platformHandle.Handle);

        if (topLevel is Window window)
            session.SetFrameActive(window.IsActive);

    }

    /// <summary>
    /// Permanently closes the hosted session and releases its native OLE resources.
    /// Visual-tree detaches alone intentionally preserve the session for docking and
    /// tab reparenting; owners should call Dispose when the view is truly closed.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;

        VerifyAccess();
        disposed = true;

        if (topLevel is Window window)
        {
            window.Activated -= OnWindowActivated;
            window.Deactivated -= OnWindowDeactivated;
        }

        StopCaptureWorker();
        UninstallKeyboardHook();
        mouseDragTimer.Stop();
        resizeTimer.Stop();
        ReleaseForwardedKeys();
        ReleaseMouseButtons();

        if (session is not null)
        {
            session.Ready -= OnSessionReady;
            session.LoginCompleted -= OnSessionLoginCompleted;
            session.RemoteDesktopSizeChanged -= OnSessionRemoteDesktopSizeChanged;
            session.FocusReleased -= OnSessionFocusReleased;
            session.StatusChanged -= OnSessionStatusChanged;
            session.Disconnected -= OnSessionDisconnected;
            session.Dispose();
            session = null;
        }

        lock (frameLock)
        {
            frameBitmap?.Dispose();
            frameBitmap = null;
            backFrameBitmap?.Dispose();
            backFrameBitmap = null;
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size arrangedSize = base.ArrangeOverride(finalSize);
        QueueViewportPixelSizeChanged(arrangedSize);
        QueueDynamicResize(arrangedSize);
        return arrangedSize;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (IsViewOnly)
            return;

        if (TryMapToRemote(e.GetPosition(this), out int x, out int y))
        {
            pressedButtons |= GetMouseButtonState(e.GetCurrentPoint(this).Properties);
            lastRemoteX = x;
            lastRemoteY = y;
            session?.SendMouseMove(x, y, pressedButtons | GetModifierButtons(e.KeyModifiers));
            e.Handled = true;
        }
        else if (pressedButtons == NativeMouseButtons.None)
        {
            session?.SendMouseLeave();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();

        if (IsViewOnly)
            return;

        if (!TryMapToRemote(e.GetPosition(this), out int x, out int y))
            return;

        PointerUpdateKind kind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        uint message;

        switch (kind)
        {
            case PointerUpdateKind.LeftButtonPressed:
                pressedButtons |= NativeMouseButtons.Left;
                message = MouseMessages.LeftDown;
                break;
            case PointerUpdateKind.RightButtonPressed:
                pressedButtons |= NativeMouseButtons.Right;
                message = MouseMessages.RightDown;
                break;
            case PointerUpdateKind.MiddleButtonPressed:
                pressedButtons |= NativeMouseButtons.Middle;
                message = MouseMessages.MiddleDown;
                break;
            case PointerUpdateKind.XButton1Pressed:
                pressedButtons |= NativeMouseButtons.XButton1;
                message = MouseMessages.XDown;
                break;
            case PointerUpdateKind.XButton2Pressed:
                pressedButtons |= NativeMouseButtons.XButton2;
                message = MouseMessages.XDown;
                break;
            default:
                return;
        }

        lastRemoteX = x;
        lastRemoteY = y;
        ushort xButton = kind is PointerUpdateKind.XButton1Pressed ? (ushort)1
            : kind is PointerUpdateKind.XButton2Pressed ? (ushort)2 : (ushort)0;
        session?.SendMouseButton(
            message, x, y, pressedButtons | GetModifierButtons(e.KeyModifiers), xButton);
        e.Pointer.Capture(this);
        mouseDragTimer.Start();
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (IsViewOnly)
        {
            ReleaseMouseButtons();
            return;
        }

        if (!TryMapToRemote(e.GetPosition(this), out int x, out int y, true))
            return;

        PointerUpdateKind kind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        uint message;

        switch (kind)
        {
            case PointerUpdateKind.LeftButtonReleased:
                pressedButtons &= ~NativeMouseButtons.Left;
                message = MouseMessages.LeftUp;
                break;
            case PointerUpdateKind.RightButtonReleased:
                pressedButtons &= ~NativeMouseButtons.Right;
                message = MouseMessages.RightUp;
                break;
            case PointerUpdateKind.MiddleButtonReleased:
                pressedButtons &= ~NativeMouseButtons.Middle;
                message = MouseMessages.MiddleUp;
                break;
            case PointerUpdateKind.XButton1Released:
                pressedButtons &= ~NativeMouseButtons.XButton1;
                message = MouseMessages.XUp;
                break;
            case PointerUpdateKind.XButton2Released:
                pressedButtons &= ~NativeMouseButtons.XButton2;
                message = MouseMessages.XUp;
                break;
            default:
                return;
        }

        lastRemoteX = x;
        lastRemoteY = y;
        ushort xButton = kind is PointerUpdateKind.XButton1Released ? (ushort)1
            : kind is PointerUpdateKind.XButton2Released ? (ushort)2 : (ushort)0;
        session?.SendMouseButton(
            message, x, y, pressedButtons | GetModifierButtons(e.KeyModifiers), xButton);

        if (pressedButtons == NativeMouseButtons.None)
        {
            mouseDragTimer.Stop();
            e.Pointer.Capture(null);

            if (!frameDestination.Contains(e.GetPosition(this)))
            {
                session?.SendMouseLeave();
            }
        }

        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (IsViewOnly)
            return;

        if (TryMapToRemote(e.GetPosition(this), out int x, out int y))
        {
            NativeMouseButtons buttons = pressedButtons | GetModifierButtons(e.KeyModifiers);
            int verticalDelta = Math.Clamp((int)Math.Round(e.Delta.Y * 120), short.MinValue, short.MaxValue);
            int horizontalDelta = Math.Clamp((int)Math.Round(e.Delta.X * 120), short.MinValue, short.MaxValue);

            if (verticalDelta != 0)
                session?.SendMouseWheel(x, y, verticalDelta, buttons);
            if (horizontalDelta != 0)
                session?.SendMouseHorizontalWheel(x, y, horizontalDelta, buttons);

            e.Handled = verticalDelta != 0 || horizontalDelta != 0;
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (pressedButtons == NativeMouseButtons.None)
        {
            session?.SendMouseLeave();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (IsViewOnly || !IsConnectionActive)
            return;

        // When the low-level hook is installed it already consulted
        // LocalKeyFilter for this event; only the fallback path evaluates it.
        if (TryGetVirtualKey(e.Key, out int virtualKey, out bool extended))
        {
            bool systemKey = IsSystemKey(e);
            if (keyboardHook == 0 && LocalKeyFilter is not null &&
                LocalKeyFilter(virtualKey, false, systemKey))
            {
                return;
            }

            session?.SendKey(virtualKey, false, extended, systemKey);
            forwardedKeys[virtualKey] = new ForwardedKey(0, extended, systemKey);
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (IsViewOnly || !IsConnectionActive)
            return;

        if (TryGetVirtualKey(e.Key, out int virtualKey, out bool extended))
        {
            if (keyboardHook == 0 && LocalKeyFilter is not null &&
                LocalKeyFilter(virtualKey, true, IsSystemKey(e)))
            {
                return;
            }

            bool systemKey = forwardedKeys.TryGetValue(virtualKey, out ForwardedKey forwarded)
                ? forwarded.SystemKey
                : IsSystemKey(e);
            session?.SendKey(virtualKey, true, extended, systemKey);
            forwardedKeys.Remove(virtualKey);
            e.Handled = true;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (!ContinuePhysicalMouseDrag())
            ReleaseMouseButtons();
    }

    private void OnControlLostFocus(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        UninstallKeyboardHook();
        ReleaseForwardedKeys();
        if (!ContinuePhysicalMouseDrag())
            ReleaseMouseButtons();
    }

    private void OnControlGotFocus(object? sender, FocusChangedEventArgs e)
    {
        InstallKeyboardHook();
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        session?.SetFrameActive(true);
        InstallKeyboardHook();
    }

    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        // Remove the process-wide hook immediately. Avalonia's IsActive and
        // logical-focus state can lag a native foreground-window transition;
        // leaving the hook installed during that interval can swallow input
        // intended for another local application.
        UninstallKeyboardHook();
        session?.SetFrameActive(false);

        // Avalonia retains logical keyboard focus while its window is
        // deactivated, so LostFocus never fires. Release forwarded input here
        // or keys and mouse buttons stay pressed in the remote session.
        ReleaseForwardedKeys();
        ReleaseMouseButtons();
    }

    private void InstallKeyboardHook()
    {
        if (keyboardHook != 0 || disposed)
            return;

        keyboardHook = SetWindowsHookExW(WhKeyboardLl, keyboardHookProc, GetModuleHandleW(null), 0);
        if (keyboardHook == 0)
            PublishStatus($"RDP keyboard hook could not be installed (Win32 {Marshal.GetLastWin32Error()}).");
    }

    private void UninstallKeyboardHook()
    {
        nint hook = keyboardHook;
        keyboardHook = 0;
        if (hook != 0)
            UnhookWindowsHookEx(hook);
    }

    // Keys are trapped and forwarded only while the view is focused in an
    // active window with a live connection. A focused but disconnected view
    // must let keys pass through so Tab and focus traversal keep working in
    // the host application.
    internal static bool ShouldForwardKeys(
        bool isViewOnly,
        bool focusWithin,
        bool windowActive,
        bool connectionActive,
        bool foregroundWindowMatches)
    {
        return !isViewOnly && focusWithin && windowActive && connectionActive && foregroundWindowMatches;
    }

    private bool IsTopLevelForegroundWindow()
    {
        nint topLevelWindow = topLevel?.TryGetPlatformHandle()?.Handle ?? 0;
        return topLevelWindow != 0 && GetForegroundWindow() == topLevelWindow;
    }

    private nint KeyboardHookProc(int code, nint wParam, nint lParam)
    {
        if (code < 0 || keyboardHook == 0 ||
            !ShouldForwardKeys(
                IsViewOnly,
                IsKeyboardFocusWithin,
                topLevel is not Window window || window.IsActive,
                session?.IsConnectionActive == true,
                IsTopLevelForegroundWindow()))
        {
            return CallNextHookEx(keyboardHook, code, wParam, lParam);
        }

        uint message = unchecked((uint)(nuint)wParam);
        bool keyDown = message is WmKeyDown or WmSystemKeyDown;
        bool keyUp = message is WmKeyUp or WmSystemKeyUp;
        if (!keyDown && !keyUp)
            return CallNextHookEx(keyboardHook, code, wParam, lParam);

        KeyboardHookData keyData = Marshal.PtrToStructure<KeyboardHookData>(lParam);
        int virtualKey = checked((int)keyData.VirtualKey);
        bool extended = (keyData.Flags & LlkhfExtended) != 0;
        bool systemKey = message is WmSystemKeyDown or WmSystemKeyUp;

        if (ShouldHandleShortcutLocally(virtualKey, keyDown, keyUp))
            return CallNextHookEx(keyboardHook, code, wParam, lParam);

        // The hosting application can claim its own accelerators before the
        // key is swallowed and forwarded to the remote session. Claimed keys
        // are tracked so a stateful filter does not run twice (the claim is
        // decisive for the press and its release).
        if (keyUp && claimedLocalKeys.Remove(virtualKey))
            return CallNextHookEx(keyboardHook, code, wParam, lParam);

        if (keyDown && LocalKeyFilter is not null && LocalKeyFilter(virtualKey, keyUp, systemKey))
        {
            claimedLocalKeys.Add(virtualKey);
            return CallNextHookEx(keyboardHook, code, wParam, lParam);
        }

        // VK_PACKET carries the UTF-16 code unit in scanCode. This is the path
        // used by Windows Unicode input injection and by RDM's IME bridge.
        if (keyDown && virtualKey == 0xE7)
        {
            session?.SendCharacter((char)keyData.ScanCode);
            return 1;
        }

        session?.SendKey(virtualKey, keyData.ScanCode, keyUp, extended, systemKey);

        if (keyDown)
            forwardedKeys[virtualKey] = new ForwardedKey(keyData.ScanCode, extended, systemKey);
        else
            forwardedKeys.Remove(virtualKey);

        return 1;
    }

    private bool ShouldHandleShortcutLocally(int virtualKey, bool keyDown, bool keyUp)
    {
        if (UseRemoteKeyboardShortcuts)
        {
            localShortcutKeys.Clear();
            return false;
        }

        if (keyUp && localShortcutKeys.Remove(virtualKey))
            return true;

        const int leftWindows = 0x5B;
        const int rightWindows = 0x5C;
        const int leftAlt = 0xA4;
        const int rightAlt = 0xA5;
        const int leftControl = 0xA2;
        const int rightControl = 0xA3;

        bool windowsKey = virtualKey is leftWindows or rightWindows;
        int altKey = forwardedKeys.ContainsKey(leftAlt) ? leftAlt : rightAlt;
        int controlKey = forwardedKeys.ContainsKey(leftControl) ? leftControl : rightControl;
        bool altPressed = forwardedKeys.ContainsKey(altKey);
        bool controlPressed = forwardedKeys.ContainsKey(controlKey);
        bool localCombination = keyDown &&
            ((altPressed && virtualKey is 0x09 or 0x1B or 0x73 or 0x20) ||
             (controlPressed && virtualKey == 0x1B));
        if (!windowsKey && !localCombination)
            return false;

        localShortcutKeys.Add(virtualKey);
        if (altPressed)
            localShortcutKeys.Add(altKey);
        if (controlPressed)
            localShortcutKeys.Add(controlKey);
        ReleaseForwardedKeys();
        return true;
    }

    private void OnFileDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = !IsViewOnly && AllowFileDrop && IsConnectionActive &&
            e.DataTransfer.Contains(DataFormat.File)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnFileDrop(object? sender, DragEventArgs e)
    {
        if (IsViewOnly || !AllowFileDrop || !IsConnectionActive)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        IStorageItem[]? items = e.DataTransfer.TryGetFiles();
        string[] paths = items?
            .Select(item => item.Path.LocalPath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToArray() ?? [];
        if (paths.Length == 0 || !await TrySetFileDropClipboardAsync(paths))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        Focus();
        await Task.Delay(200);
        SendPasteShortcut();
        e.DragEffects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private static async Task<bool> TrySetFileDropClipboardAsync(IReadOnlyList<string> paths)
    {
        string dropList = string.Join('\0', paths) + "\0\0";
        byte[] encodedPaths = Encoding.Unicode.GetBytes(dropList);
        int headerSize = Marshal.SizeOf<DropFiles>();
        nint memory = GlobalAlloc(GmemMoveable | GmemZeroInit, (nuint)(headerSize + encodedPaths.Length));
        if (memory == 0)
            return false;

        bool clipboardOwnsMemory = false;
        try
        {
            nint data = GlobalLock(memory);
            if (data == 0)
                return false;

            try
            {
                Marshal.StructureToPtr(new DropFiles
                {
                    Offset = (uint)headerSize,
                    Wide = 1
                }, data, false);
                Marshal.Copy(encodedPaths, 0, data + headerSize, encodedPaths.Length);
            }
            finally
            {
                GlobalUnlock(memory);
            }

            bool clipboardOpened = false;
            for (int attempt = 0; attempt < 8; attempt++)
            {
                if (OpenClipboard(0))
                {
                    clipboardOpened = true;
                    break;
                }
                await Task.Delay(15);
            }

            if (!clipboardOpened)
                return false;

            try
            {
                if (!EmptyClipboard() || SetClipboardData(CfHDrop, memory) == 0)
                    return false;
                clipboardOwnsMemory = true;
                return true;
            }
            finally
            {
                CloseClipboard();
            }
        }
        finally
        {
            if (!clipboardOwnsMemory)
                GlobalFree(memory);
        }
    }

    private void SendPasteShortcut()
    {
        const int control = 0x11;
        const int v = 0x56;
        session?.SendKey(control, false, false, false);
        session?.SendKey(v, false, false, false);
        session?.SendKey(v, true, false, false);
        session?.SendKey(control, true, false, false);
    }

    private void OnMouseDragTimerTick(object? sender, EventArgs e)
    {
        if (pressedButtons == NativeMouseButtons.None)
        {
            mouseDragTimer.Stop();
            return;
        }

        NativeMouseButtons physicalButtons = GetPhysicalMouseButtonState();
        if ((physicalButtons & pressedButtons) == NativeMouseButtons.None)
        {
            ReleaseMouseButtons();
            return;
        }

        if (!GetCursorPos(out NativePoint screenPoint))
            return;

        PixelPoint canvasOrigin = this.PointToScreen(default);
        double scaling = topLevel?.RenderScaling ?? 1.0;
        Point localPoint = new(
            (screenPoint.X - canvasOrigin.X) / scaling,
            (screenPoint.Y - canvasOrigin.Y) / scaling);

        if (!TryMapToRemote(localPoint, out int x, out int y, true))
            return;

        lastRemoteX = x;
        lastRemoteY = y;
        session?.SendMouseMove(x, y, pressedButtons);
    }

    private void StartCaptureWorker()
    {
        StopCaptureWorker();
        staFrameVersion = 0;
        staHasFrameVersion = false;
        Interlocked.Exchange(ref captureFailureReported, 0);

        // Older MsRdpEx builds do not expose the capture-only native handle.
        // Keep their apartment-bound instance calls on the Avalonia STA.
        if (session?.CanCaptureOffThread != true)
        {
            staCaptureTimer.Start();
            return;
        }

        int generation = Interlocked.Increment(ref captureGeneration);
        CancellationTokenSource cancellation = new();
        captureCancellation = cancellation;
        captureTask = Task.Run(() => RunCaptureWorkerAsync(generation, cancellation));
    }

    private void StopCaptureWorker()
    {
        staCaptureTimer.Stop();
        CancellationTokenSource? cancellation = captureCancellation;
        captureCancellation = null;
        Interlocked.Increment(ref captureGeneration);
        cancellation?.Cancel();

        Task? task = captureTask;
        captureTask = null;
        if (task is not null)
        {
            try
            {
                task.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException) when (cancellation?.IsCancellationRequested == true)
            {
            }
        }
    }

    private async Task RunCaptureWorkerAsync(
        int generation,
        CancellationTokenSource cancellation)
    {
        uint lastFrameVersion = 0;
        bool hasFrameVersion = false;

        try
        {
            CancellationToken cancellationToken = cancellation.Token;
            while (!cancellationToken.IsCancellationRequested &&
                   generation == Volatile.Read(ref captureGeneration) && !disposed)
            {
                TryCaptureChangedFrame(ref lastFrameVersion, ref hasFrameVersion);

                await Task.Delay(33, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            Interlocked.CompareExchange(ref captureCancellation, null, cancellation);
            cancellation.Dispose();
        }
    }

    private void OnStaCaptureTimerTick(object? sender, EventArgs e)
    {
        TryCaptureChangedFrame(ref staFrameVersion, ref staHasFrameVersion);
    }

    private void TryCaptureChangedFrame(ref uint lastFrameVersion, ref bool hasFrameVersion)
    {
        uint frameVersion = 0;
        // The frame-version export is optional for the STA compatibility path;
        // without it, retain the historical 30 fps copy behavior.
        bool versionAvailable = session?.TryGetShadowBitmapFrameVersion(out frameVersion) == true;
        bool frameChanged = !versionAvailable || !hasFrameVersion || frameVersion != lastFrameVersion;

        if (!frameChanged || !CaptureFrame())
            return;

        if (versionAvailable)
        {
            lastFrameVersion = frameVersion;
            hasFrameVersion = true;
        }

        Interlocked.Exchange(ref captureFailureReported, 0);
        QueueInvalidate();
    }

    private unsafe bool CaptureFrame()
    {
        bool captured = false;
        try
        {
            session?.WithShadowBitmap((source, width, height, sourceStride) =>
            {
                lock (frameLock)
                {
                    if (waitingForInitialFrame &&
                        (!initialLoginCompleted ||
                         (initialFrameTarget != new PixelSize(width, height) &&
                          Environment.TickCount64 < initialFrameDeadline)))
                    {
                        return;
                    }

                    EnsureBackFrameBitmap(width, height);

                    using ILockedFramebuffer destination = backFrameBitmap!.Lock();
                    int bytesPerRow = checked(width * 4);

                    for (int y = 0; y < height; y++)
                    {
                        byte* sourceRow = (byte*)source + (y * sourceStride);
                        byte* destinationRow = (byte*)destination.Address + (y * destination.RowBytes);
                        Buffer.MemoryCopy(sourceRow, destinationRow, destination.RowBytes, bytesPerRow);
                    }

                    (frameBitmap, backFrameBitmap) = (backFrameBitmap, frameBitmap);
                    waitingForInitialFrame = false;
                    captured = true;
                }
            });
        }
        catch (ObjectDisposedException) when (disposed || captureCancellation is null)
        {
            return false;
        }
        catch (COMException exception)
        {
            if (Interlocked.Exchange(ref captureFailureReported, 1) == 0)
            {
                Dispatcher.UIThread.Post(
                    () => PublishStatus($"RDP frame capture failed: {exception.Message}"),
                    DispatcherPriority.Background);
            }
            return false;
        }

        return captured;
    }

    private void QueueInvalidate()
    {
        if (disposed || Interlocked.Exchange(ref invalidateQueued, 1) != 0)
            return;

        Dispatcher.UIThread.Post(
            () =>
            {
                Interlocked.Exchange(ref invalidateQueued, 0);
                if (!disposed)
                    InvalidateVisual();
            },
            DispatcherPriority.Render);
    }

    private void OnResizeTimerTick(object? sender, EventArgs e)
    {
        resizeTimer.Stop();

        if (!dynamicResolutionEnabled || pendingDesktopSize.Width <= 0 || pendingDesktopSize.Height <= 0)
            return;

        bool applied = session?.ResizeDisplay(
            pendingDesktopSize.Width, pendingDesktopSize.Height, pendingRenderScaling) == true;
        if (!applied && session?.IsLoginCompleted == true && resizeRetryCount < 4)
        {
            resizeRetryCount++;
            resizeTimer.Interval = TimeSpan.FromMilliseconds(250 * (1 << resizeRetryCount));
            resizeTimer.Start();
        }
    }

    private void QueueDynamicResize(Size logicalSize)
    {
        if (!dynamicResolutionEnabled || session is null || logicalSize.Width <= 0 || logicalSize.Height <= 0)
            return;

        pendingRenderScaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        pendingDesktopSize = GetDesktopPixelSize(logicalSize, pendingRenderScaling);
        lock (frameLock)
        {
            if (waitingForInitialFrame)
                initialFrameTarget = pendingDesktopSize;
        }
        resizeRetryCount = 0;
        resizeTimer.Interval = TimeSpan.FromMilliseconds(250);
        resizeTimer.Stop();
        resizeTimer.Start();
    }

    private void QueueViewportPixelSizeChanged(Size logicalSize)
    {
        if (logicalSize.Width <= 0 || logicalSize.Height <= 0)
            return;

        double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        PixelSize currentSize = GetDesktopPixelSize(logicalSize, scaling);
        if (currentSize == viewportPixelSize)
            return;

        viewportPixelSize = currentSize;
        if (viewportChangedQueued)
            return;

        viewportChangedQueued = true;
        Dispatcher.UIThread.Post(
            () =>
            {
                viewportChangedQueued = false;
                if (!disposed)
                    ViewportPixelSizeChanged?.Invoke(this, EventArgs.Empty);
            },
            DispatcherPriority.Background);
    }

    private void PrepareInitialFrame(PixelSize targetSize)
    {
        lock (frameLock)
        {
            initialFrameTarget = targetSize;
            initialFrameDeadline = long.MaxValue;
            initialLoginCompleted = false;
            waitingForInitialFrame = true;
            frameDestination = default;
            frameBitmap?.Dispose();
            frameBitmap = null;
            backFrameBitmap?.Dispose();
            backFrameBitmap = null;
        }

        InvalidateVisual();
    }

    private void EnsureBackFrameBitmap(int width, int height)
    {
        if (backFrameBitmap?.PixelSize == new PixelSize(width, height))
            return;

        backFrameBitmap?.Dispose();
        backFrameBitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);
    }

    private PixelSize GetDesktopPixelSize()
    {
        double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        return GetDesktopPixelSize(Bounds.Size, scaling);
    }

    private static PixelSize GetDesktopPixelSize(Size logicalSize, double scaling)
    {
        int width = Math.Clamp((int)Math.Round(logicalSize.Width * scaling), 200, 8192);
        int height = Math.Clamp((int)Math.Round(logicalSize.Height * scaling), 200, 8192);
        width -= width % 2;
        height -= height % 2;
        return new PixelSize(width, height);
    }

    private bool TryMapToRemote(Point point, out int x, out int y, bool clampOutside = false)
    {
        x = 0;
        y = 0;

        PixelSize frameSize;
        lock (frameLock)
            frameSize = frameBitmap?.PixelSize ?? default;

        if (frameSize.Width <= 0 || frameSize.Height <= 0 ||
            frameDestination.Width <= 0 || frameDestination.Height <= 0 ||
            (!clampOutside && !frameDestination.Contains(point)))
        {
            return false;
        }

        int inputWidth = frameSize.Width;
        int inputHeight = frameSize.Height;
        session?.TryGetInputSize(out inputWidth, out inputHeight);

        x = Math.Clamp(
            (int)((point.X - frameDestination.X) * inputWidth / frameDestination.Width),
            0,
            inputWidth - 1);
        y = Math.Clamp(
            (int)((point.Y - frameDestination.Y) * inputHeight / frameDestination.Height),
            0,
            inputHeight - 1);
        return true;
    }

    private void ReleaseForwardedKeys()
    {
        foreach ((int virtualKey, ForwardedKey key) in forwardedKeys)
        {
            if (key.ScanCode != 0)
                session?.SendKey(virtualKey, key.ScanCode, true, key.Extended, key.SystemKey);
            else
                session?.SendKey(virtualKey, true, key.Extended, key.SystemKey);
        }

        forwardedKeys.Clear();
    }

    private void ReleaseMouseButtons()
    {
        NativeMouseButtons remaining = pressedButtons;
        pressedButtons = NativeMouseButtons.None;
        mouseDragTimer.Stop();

        if (remaining.HasFlag(NativeMouseButtons.Left))
        {
            remaining &= ~NativeMouseButtons.Left;
            session?.SendMouseButton(MouseMessages.LeftUp, lastRemoteX, lastRemoteY, remaining);
        }

        if (remaining.HasFlag(NativeMouseButtons.Right))
        {
            remaining &= ~NativeMouseButtons.Right;
            session?.SendMouseButton(MouseMessages.RightUp, lastRemoteX, lastRemoteY, remaining);
        }

        if (remaining.HasFlag(NativeMouseButtons.Middle))
        {
            remaining &= ~NativeMouseButtons.Middle;
            session?.SendMouseButton(MouseMessages.MiddleUp, lastRemoteX, lastRemoteY, remaining);
        }


        if (remaining.HasFlag(NativeMouseButtons.XButton1))
        {
            remaining &= ~NativeMouseButtons.XButton1;
            session?.SendMouseButton(MouseMessages.XUp, lastRemoteX, lastRemoteY, remaining, 1);
        }

        if (remaining.HasFlag(NativeMouseButtons.XButton2))
        {
            remaining &= ~NativeMouseButtons.XButton2;
            session?.SendMouseButton(MouseMessages.XUp, lastRemoteX, lastRemoteY, remaining, 2);
        }
    }

    private bool ContinuePhysicalMouseDrag()
    {
        if (pressedButtons == NativeMouseButtons.None ||
            (GetPhysicalMouseButtonState() & pressedButtons) == NativeMouseButtons.None)
        {
            return false;
        }

        mouseDragTimer.Start();
        return true;
    }

    private static NativeMouseButtons GetMouseButtonState(PointerPointProperties properties)
    {
        NativeMouseButtons buttons = NativeMouseButtons.None;
        if (properties.IsLeftButtonPressed)
            buttons |= NativeMouseButtons.Left;
        if (properties.IsRightButtonPressed)
            buttons |= NativeMouseButtons.Right;
        if (properties.IsMiddleButtonPressed)
            buttons |= NativeMouseButtons.Middle;
        if (properties.IsXButton1Pressed)
            buttons |= NativeMouseButtons.XButton1;
        if (properties.IsXButton2Pressed)
            buttons |= NativeMouseButtons.XButton2;
        return buttons;
    }

    private static NativeMouseButtons GetPhysicalMouseButtonState()
    {
        NativeMouseButtons buttons = NativeMouseButtons.None;
        if (GetAsyncKeyState(0x01) < 0)
            buttons |= NativeMouseButtons.Left;
        if (GetAsyncKeyState(0x02) < 0)
            buttons |= NativeMouseButtons.Right;
        if (GetAsyncKeyState(0x04) < 0)
            buttons |= NativeMouseButtons.Middle;
        if (GetAsyncKeyState(0x05) < 0)
            buttons |= NativeMouseButtons.XButton1;
        if (GetAsyncKeyState(0x06) < 0)
            buttons |= NativeMouseButtons.XButton2;
        return buttons;
    }

    private static NativeMouseButtons GetModifierButtons(KeyModifiers modifiers)
    {
        NativeMouseButtons buttons = NativeMouseButtons.None;
        if (modifiers.HasFlag(KeyModifiers.Shift))
            buttons |= NativeMouseButtons.Shift;
        if (modifiers.HasFlag(KeyModifiers.Control))
            buttons |= NativeMouseButtons.Control;
        return buttons;
    }

    private static bool IsSystemKey(KeyEventArgs e)
    {
        return e.KeyModifiers.HasFlag(KeyModifiers.Alt) || e.Key is Key.LeftAlt or Key.RightAlt;
    }

    private static Rect CalculateAspectFit(Rect bounds, PixelSize source)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0 || source.Width <= 0 || source.Height <= 0)
            return default;

        double scale = Math.Min(bounds.Width / source.Width, bounds.Height / source.Height);
        double width = source.Width * scale;
        double height = source.Height * scale;
        return new Rect(
            bounds.X + ((bounds.Width - width) / 2),
            bounds.Y + ((bounds.Height - height) / 2),
            width,
            height);
    }

    private void OnSessionReady(object? sender, EventArgs e)
    {
        ClientReady?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionLoginCompleted(object? sender, EventArgs e)
    {
        QueueDynamicResize(Bounds.Size);
        lock (frameLock)
        {
            // A non-conforming server should not leave the reusable control black
            // forever, but normal connections only reveal a correctly sized frame.
            initialFrameDeadline = Environment.TickCount64 + 5000;
            initialLoginCompleted = true;
        }
        LoginCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionRemoteDesktopSizeChanged(
        object? sender, RdpRemoteDesktopSizeChangedEventArgs e)
    {
        RemoteDesktopSizeChanged?.Invoke(this, e);
    }

    private void OnSessionFocusReleased(object? sender, RdpFocusReleasedEventArgs e)
    {
        ReleaseForwardedKeys();
        FocusReleased?.Invoke(this, e);
    }

    private void OnSessionDisconnected(object? sender, EventArgs e)
    {
        // The remote session is gone: drop the forwarded/claimed key state so
        // a stale modifier does not distort shortcut routing after a reconnect
        // and no orphaned release is sent to the next session.
        if (sessionDisconnectedFired)
            return;

        sessionDisconnectedFired = true;
        forwardedKeys.Clear();
        claimedLocalKeys.Clear();
        localShortcutKeys.Clear();
    }

    private void OnSessionStatusChanged(object? sender, RdpStatusChangedEventArgs e)
    {
        PublishStatus(e.Message);
    }

    private void PublishStatus(string message)
    {
        StatusChanged?.Invoke(this, new RdpStatusChangedEventArgs(message));
    }

    private static bool TryGetVirtualKey(Key key, out int virtualKey, out bool extended)
    {
        extended = false;

        if (key is >= Key.D0 and <= Key.D9)
        {
            virtualKey = 0x30 + (key - Key.D0);
            return true;
        }

        if (key is >= Key.A and <= Key.Z)
        {
            virtualKey = 0x41 + (key - Key.A);
            return true;
        }

        if (key is >= Key.NumPad0 and <= Key.NumPad9)
        {
            virtualKey = 0x60 + (key - Key.NumPad0);
            return true;
        }

        if (key is >= Key.F1 and <= Key.F24)
        {
            virtualKey = 0x70 + (key - Key.F1);
            return true;
        }

        virtualKey = key switch
        {
            Key.Back => 0x08,
            Key.Tab => 0x09,
            Key.Clear => 0x0C,
            Key.Return or Key.Enter => 0x0D,
            Key.Pause => 0x13,
            Key.CapsLock or Key.Capital => 0x14,
            Key.Escape => 0x1B,
            Key.Space => 0x20,
            Key.PageUp or Key.Prior => 0x21,
            Key.PageDown or Key.Next => 0x22,
            Key.End => 0x23,
            Key.Home => 0x24,
            Key.Left => 0x25,
            Key.Up => 0x26,
            Key.Right => 0x27,
            Key.Down => 0x28,
            Key.Snapshot or Key.PrintScreen => 0x2C,
            Key.Insert => 0x2D,
            Key.Delete => 0x2E,
            Key.LWin => 0x5B,
            Key.RWin => 0x5C,
            Key.Apps => 0x5D,
            Key.Multiply => 0x6A,
            Key.Add => 0x6B,
            Key.Separator => 0x6C,
            Key.Subtract => 0x6D,
            Key.Decimal => 0x6E,
            Key.Divide => 0x6F,
            Key.NumLock => 0x90,
            Key.Scroll => 0x91,
            Key.LeftShift => 0xA0,
            Key.RightShift => 0xA1,
            Key.LeftCtrl => 0xA2,
            Key.RightCtrl => 0xA3,
            Key.LeftAlt => 0xA4,
            Key.RightAlt => 0xA5,
            Key.OemSemicolon or Key.Oem1 => 0xBA,
            Key.OemPlus => 0xBB,
            Key.OemComma => 0xBC,
            Key.OemMinus => 0xBD,
            Key.OemPeriod => 0xBE,
            Key.OemQuestion or Key.Oem2 => 0xBF,
            Key.OemTilde or Key.Oem3 => 0xC0,
            Key.OemOpenBrackets or Key.Oem4 => 0xDB,
            Key.OemPipe or Key.Oem5 => 0xDC,
            Key.OemCloseBrackets or Key.Oem6 => 0xDD,
            Key.OemQuotes or Key.Oem7 => 0xDE,
            _ => 0
        };

        extended = key is Key.RightCtrl or Key.RightAlt or Key.Insert or Key.Delete or
            Key.Home or Key.End or Key.PageUp or Key.PageDown or
            Key.Left or Key.Up or Key.Right or Key.Down or Key.Divide or Key.NumLock;
        return virtualKey != 0;
    }

    private readonly record struct ForwardedKey(uint ScanCode, bool Extended, bool SystemKey);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardHookData
    {
        public uint VirtualKey;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public nuint ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DropFiles
    {
        public uint Offset;
        public NativePoint Point;
        public int NonClient;
        public int Wide;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint LowLevelKeyboardProc(int code, nint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int virtualKey);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern nint GetModuleHandleW(string? moduleName);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    private static extern nint SetWindowsHookExW(
        int hookType, LowLevelKeyboardProc callback, nint module, uint threadId);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hook);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint CallNextHookEx(nint hook, int code, nint wParam, nint lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetForegroundWindow();

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint GlobalAlloc(uint flags, nuint bytes);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint GlobalLock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint GlobalFree(nint memory);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(nint owner);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint SetClipboardData(uint format, nint memory);
}
