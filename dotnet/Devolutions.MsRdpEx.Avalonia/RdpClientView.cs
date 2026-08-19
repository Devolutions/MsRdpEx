using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// How <see cref="RdpClientView"/> presents the remote desktop, mirroring the
/// mstsc system-menu display options.
/// </summary>
public enum RdpDisplayMode
{
    /// <summary>
    /// The session resolution follows the viewport (debounced
    /// UpdateSessionDisplaySettings when dynamic resolution is enabled). The
    /// remote desktop is always presented at 100%.
    /// </summary>
    FitToWindow,

    /// <summary>
    /// The control scales the remote desktop to the window size client-side
    /// (SmartSizing); scrollbars appear when the desktop is larger.
    /// </summary>
    SmartSizing,

    /// <summary>
    /// The control presents the remote desktop at a fixed zoom percentage
    /// (see <see cref="RdpClientView.ZoomLevel"/>).
    /// </summary>
    Zoom
}

/// <summary>
/// An Avalonia RDP surface that hosts the Microsoft RDP ActiveX control in a
/// real, visible Win32 child window (via <see cref="NativeControlHost"/>). The
/// control renders natively into that window and owns native keyboard and mouse
/// input — there is no off-screen HWND, no framebuffer copy, and no synthetic
/// input forwarding. Native content always draws above Avalonia visuals
/// (airspace): Avalonia overlays cannot appear on top of the RDP view.
/// </summary>
public class RdpClientView : NativeControlHost, IDisposable
{
    /// <summary>
    /// The Microsoft RDP Client Control version 11 class identifier.
    /// </summary>
    public static readonly Guid DefaultClassId = new("A0C63C30-F08D-4AB4-907C-34905D770C7D");

    private const string StaticWindowClass = "STATIC";
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsTabStop = 0x00010000;

    private static readonly TimeSpan ResizeDebounceInterval = TimeSpan.FromMilliseconds(250);

    private readonly DispatcherTimer resizeTimer;
    private RdpActiveXSession? session;
    private TopLevel? topLevel;
    private bool dynamicResolutionEnabled;
    private PixelSize pendingDesktopSize;
    private double pendingRenderScaling = 1.0;
    private PixelSize viewportPixelSize;
    private PixelSize lastSurfaceSize;
    private bool isViewOnly;
    private bool isFullScreen;
    private bool pendingFullScreen;
    private bool syncingWindowState;
    private WindowState previousWindowState = WindowState.Normal;
    private bool dynamicResolutionRequested;
    private RdpDisplayMode displayMode = RdpDisplayMode.FitToWindow;
    private int zoomLevel = 100;
    private bool disposed;

    public RdpClientView()
    {
        resizeTimer = new DispatcherTimer
        {
            Interval = ResizeDebounceInterval
        };
        resizeTimer.Tick += OnResizeTimerTick;
    }

    public bool IsClientReady => session?.IsReady == true;
    public bool IsConnectionActive => session?.IsConnectionActive == true;
    public bool IsLoginCompleted => session?.IsLoginCompleted == true;
    public nint HostWindowHandle => session?.HostWindowHandle ?? 0;
    public PixelSize ViewportPixelSize => GetViewportPixelSize();

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
    /// Gets or sets whether Windows/system shortcuts (Alt+Tab, Alt+F4, Windows
    /// keys) are sent to the remote session while the RDP control has focus.
    /// This maps to the control's own KeyboardHookMode, so keyboard handling
    /// stays entirely native with no low-level keyboard hook in this library.
    /// Set to false to keep those shortcuts local. Applied at connect time.
    /// </summary>
    public bool UseRemoteKeyboardShortcuts { get; set; } = true;

    /// <summary>
    /// Gets or sets whether input to the remote session is suppressed while the
    /// remote desktop remains visible. Implemented by disabling the hosted
    /// window, so Windows itself keeps all input away from the control.
    /// </summary>
    public bool IsViewOnly
    {
        get => isViewOnly;
        set
        {
            if (isViewOnly == value)
                return;

            isViewOnly = value;
            nint hostWindow = HostWindowHandle;
            if (hostWindow != 0)
                EnableWindow(hostWindow, !value);
        }
    }

    /// <summary>
    /// Gets or sets fullscreen mode. The containing window covers the screen
    /// and the RDP control is switched to fullscreen rendering with its
    /// floating connection bar (auto-hiding at the top edge, with pin,
    /// minimize, restore, and close buttons, labeled with the server name).
    /// The user can also toggle fullscreen from the session with
    /// Ctrl+Alt+Break or the connection bar's restore button; both flow back
    /// through this property. Exiting restores the window state the session
    /// had before entering fullscreen.
    /// </summary>
    public bool FullScreen
    {
        get => isFullScreen || pendingFullScreen;
        set
        {
            if (value)
                EnterFullScreen();
            else
                ExitFullScreen();
        }
    }

    public event EventHandler? ClientReady;
    public event EventHandler? LoginCompleted;
    public event EventHandler<RdpRemoteDesktopSizeChangedEventArgs>? RemoteDesktopSizeChanged;
    public event EventHandler<RdpFocusReleasedEventArgs>? FocusReleased;
    public event EventHandler<RdpStatusChangedEventArgs>? StatusChanged;
    public event EventHandler? Disconnected;
    public event EventHandler? ViewportPixelSizeChanged;

    /// <summary>Raised after the control entered fullscreen rendering.</summary>
    public event EventHandler? EnteredFullScreen;

    /// <summary>Raised after the control left fullscreen rendering.</summary>
    public event EventHandler? LeftFullScreen;

    /// <summary>Raised when <see cref="DisplayMode"/> or <see cref="ZoomLevel"/> changes.</summary>
    public event EventHandler? DisplayModeChanged;

    /// <summary>
    /// Gets or sets how the remote desktop is presented, like the display
    /// options in the mstsc system menu. Defaults to
    /// <see cref="RdpDisplayMode.FitToWindow"/> when connecting without an
    /// explicit resolution (dynamic resolution).
    /// </summary>
    public RdpDisplayMode DisplayMode
    {
        get => displayMode;
        set => SetDisplayMode(value, zoomLevel);
    }

    /// <summary>
    /// Gets or sets the control's native zoom percentage (25–400). Only
    /// meaningful in <see cref="RdpDisplayMode.Zoom"/>; setting a value other
    /// than 100 switches to that mode, and setting 100 switches back to
    /// <see cref="RdpDisplayMode.FitToWindow"/>, mirroring mstsc's Zoom menu.
    /// </summary>
    public int ZoomLevel
    {
        get => zoomLevel;
        set => SetDisplayMode(value == 100 ? RdpDisplayMode.FitToWindow : RdpDisplayMode.Zoom, value);
    }

    /// <summary>
    /// Applies a display-mode change, keeping the session's SmartSizing state,
    /// the debounced dynamic-resolution path, and the fullscreen renegotiation
    /// coherent.
    /// </summary>
    private void SetDisplayMode(RdpDisplayMode mode, int zoom)
    {
        zoom = Math.Clamp(zoom, 25, 400);
        if (mode == displayMode && zoom == zoomLevel)
            return;

        displayMode = mode;
        zoomLevel = zoom;

        dynamicResolutionEnabled = dynamicResolutionRequested &&
                                   mode == RdpDisplayMode.FitToWindow;

        RdpActiveXSession? activeSession = session;
        if (activeSession is not null && activeSession.IsReady)
        {
            ApplyDisplayMode(activeSession);

            if (mode == RdpDisplayMode.FitToWindow)
            {
                // Re-assert the session resolution for the current viewport so
                // a smart-sizing or zoom presentation snaps back to 100%. The
                // forced update also covers the size-unchanged case, where the
                // control must re-apply the resolution to drop scaling.
                PixelSize size = GetDesktopPixelSize();
                double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
                activeSession.ResizeDisplay(size.Width, size.Height, scaling, force: true);
            }
        }

        // The native child always fills the view; the scaling happens inside
        // the control, so its window rectangle must track the new layout.
        lastSurfaceSize = default;
        InvalidateArrange();
        DisplayModeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyDisplayMode(RdpActiveXSession activeSession)
    {
        // ZoomLevel (VT_UI4) and SmartSizing are mutually exclusive. mstscax
        // also rejects ZoomLevel until the session is logged on, and while
        // dynamic resolution is driving the desktop size.
        switch (displayMode)
        {
            case RdpDisplayMode.SmartSizing:
                activeSession.SmartSizing = true;
                break;
            case RdpDisplayMode.Zoom:
                activeSession.SmartSizing = false;
                if (activeSession.IsLoginCompleted &&
                    !activeSession.TrySetZoomLevel(zoomLevel) &&
                    zoomLevel != 100)
                {
                    ApplyZoomFallback(activeSession);
                }
                break;
            default:
                activeSession.SmartSizing = false;
                if (activeSession.IsLoginCompleted)
                    _ = activeSession.TrySetZoomLevel(100);
                break;
        }
    }

    // When ZoomLevel is rejected (E_FAIL on some hosts), shrink the session
    // desktop and smart-size it back to the viewport so the remote UI appears
    // magnified without a COM zoom write.
    private void ApplyZoomFallback(RdpActiveXSession activeSession)
    {
        PixelSize view = GetDesktopPixelSize();
        int width = Math.Max(200, view.Width * 100 / zoomLevel);
        int height = Math.Max(200, view.Height * 100 / zoomLevel);
        double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        activeSession.SmartSizing = true;
        activeSession.ResizeDisplay(width, height, scaling, force: true);
    }

    /// <summary>
    /// Gets a generated COM interface for advanced RDP client configuration.
    /// This is available after <see cref="ClientReady"/> has fired.
    /// </summary>
    public T GetClient<T>() where T : class
    {
        VerifyAccess();
        return (session ?? throw new InvalidOperationException("The RDP session is not ready yet."))
            .GetClient<T>();
    }

    public void Connect(RdpConnectionSettings settings)
    {
        VerifyAccess();
        ArgumentNullException.ThrowIfNull(settings);

        RdpActiveXSession activeSession = session
            ?? throw new InvalidOperationException("The RDP session is not ready yet.");
        bool hasDesktopWidth = settings.DesktopWidth > 0;
        bool hasDesktopHeight = settings.DesktopHeight > 0;
        if (hasDesktopWidth != hasDesktopHeight)
        {
            throw new ArgumentException(
                "DesktopWidth and DesktopHeight must either both be positive or both be zero for dynamic resolution.",
                nameof(settings));
        }

        PixelSize viewportSize = GetDesktopPixelSize();
        dynamicResolutionRequested = !hasDesktopWidth;
        dynamicResolutionEnabled = dynamicResolutionRequested &&
                                   displayMode == RdpDisplayMode.FitToWindow;
        int desktopWidth = settings.DesktopWidth > 0 ? settings.DesktopWidth : viewportSize.Width;
        int desktopHeight = settings.DesktopHeight > 0 ? settings.DesktopHeight : viewportSize.Height;
        activeSession.UseRemoteKeyboardShortcuts = UseRemoteKeyboardShortcuts;
        activeSession.Connect(settings, desktopWidth, desktopHeight);

        ApplyDisplayMode(activeSession);
    }

    /// <summary>
    /// Connects a client that the embedding application configured through
    /// <see cref="GetClient{T}"/>. The configured server, credentials, gateway,
    /// redirection, and security properties are left unchanged.
    /// </summary>
    public void ConnectConfigured(bool manageDynamicResolution = false)
    {
        VerifyAccess();

        RdpActiveXSession activeSession = session
            ?? throw new InvalidOperationException("The RDP session is not ready yet.");
        MSTSCLib.IMsRdpClient client = activeSession.GetClient<MSTSCLib.IMsRdpClient>();
        PixelSize viewportSize = GetDesktopPixelSize();
        int desktopWidth = client.DesktopWidth > 0 ? client.DesktopWidth : viewportSize.Width;
        int desktopHeight = client.DesktopHeight > 0 ? client.DesktopHeight : viewportSize.Height;
        dynamicResolutionRequested = manageDynamicResolution;
        dynamicResolutionEnabled = dynamicResolutionRequested &&
                                   displayMode == RdpDisplayMode.FitToWindow;
        activeSession.UseRemoteKeyboardShortcuts = UseRemoteKeyboardShortcuts;
        activeSession.ConnectConfigured(desktopWidth, desktopHeight);

        ApplyDisplayMode(activeSession);
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

    /// <summary>
    /// Moves native keyboard focus to the hosted RDP control, as if the user
    /// had clicked into the remote desktop.
    /// </summary>
    public void FocusSession()
    {
        VerifyAccess();
        session?.FocusSession();
    }

    private void EnterFullScreen()
    {
        if (isFullScreen)
            return;

        if (topLevel is not Window window)
        {
            // Not attached yet (or reparenting): apply once attached.
            pendingFullScreen = true;
            return;
        }

        pendingFullScreen = false;
        previousWindowState = window.WindowState == WindowState.FullScreen
            ? WindowState.Normal
            : window.WindowState;

        // The container covers the screen first; the control is then switched
        // to fullscreen rendering so it picks up the final screen geometry and
        // shows its connection bar.
        syncingWindowState = true;
        try
        {
            window.WindowState = WindowState.FullScreen;
        }
        finally
        {
            syncingWindowState = false;
        }

        isFullScreen = true;
        if (session is not null)
            session.FullScreen = true;
    }

    private void ExitFullScreen()
    {
        if (isFullScreen)
        {
            // Clear the flag first so OnLeaveFullScreenMode (raised by
            // setting FullScreen=false) does not re-enter this method.
            isFullScreen = false;
            if (session is not null)
                session.FullScreen = false;
        }

        if (topLevel is Window window && window.WindowState == WindowState.FullScreen)
        {
            syncingWindowState = true;
            try
            {
                window.WindowState = previousWindowState;
            }
            finally
            {
                syncingWindowState = false;
            }
        }
    }

    // Keeps the control's fullscreen mode in sync when the window state
    // changes without going through this view (taskbar restore, Win+Down,
    // OS-driven transitions).
    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (syncingWindowState || e.Property != Window.WindowStateProperty)
            return;

        if (e.GetNewValue<WindowState>() == WindowState.FullScreen)
        {
            if (!isFullScreen)
            {
                isFullScreen = true;
                if (session is not null)
                    session.FullScreen = true;
            }
        }
        else if (isFullScreen)
        {
            isFullScreen = false;
            if (session is not null)
                session.FullScreen = false;
        }
    }

    private void OnSessionFullScreenRequested(object? sender, EventArgs e)
    {
        // Ctrl+Alt+Break in windowed mode.
        EnterFullScreen();
    }

    private void OnSessionLeaveFullScreenRequested(object? sender, EventArgs e)
    {
        // Ctrl+Alt+Break or the connection bar's restore button in fullscreen.
        ExitFullScreen();
    }

    private void OnSessionEnteredFullScreen(object? sender, EventArgs e)
    {
        if (!isFullScreen)
            EnterFullScreen();
        EnteredFullScreen?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionLeftFullScreen(object? sender, EventArgs e)
    {
        // The connection-bar Restore button makes mstscax leave fullscreen
        // itself and raises OnLeaveFullScreenMode — not always
        // OnRequestLeaveFullScreen. Drive the container window back too.
        if (isFullScreen)
            ExitFullScreen();
        LeftFullScreen?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionContainerMinimizeRequested(object? sender, EventArgs e)
    {
        // The connection bar's minimize button.
        if (topLevel is Window window)
            window.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Permanently closes the hosted session and releases its native OLE resources.
    /// Visual-tree detaches alone intentionally preserve the session for docking and
    /// tab reparenting (NativeControlHost defers destruction); owners should call
    /// Dispose when the view is truly closed, on the Avalonia UI thread.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;

        VerifyAccess();
        disposed = true;
        resizeTimer.Stop();
        // Restore the window before tearing down so a closing session never
        // leaves a fullscreen shell behind.
        ExitFullScreen();
        TeardownSession();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        // The top level must be known before base.OnAttachedToVisualTree runs:
        // it creates the native control synchronously through
        // CreateNativeControlCore.
        topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            window.PropertyChanged += OnWindowPropertyChanged;
        }

        base.OnAttachedToVisualTree(e);

        // Reparenting to a new top level reuses the live session without
        // re-running StartSession, so re-apply the frame window and the
        // activation state on every attach, not just at session creation.
        UpdateOleFrameState();
        if (topLevel is Window attachedWindow)
            session?.SetFrameActive(attachedWindow.IsActive);

        if (pendingFullScreen)
        {
            pendingFullScreen = false;
            EnterFullScreen();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        resizeTimer.Stop();

        // A detached (reparenting) view cannot stay fullscreen: restore the
        // window and the control before leaving the visual tree.
        ExitFullScreen();
        pendingFullScreen = false;

        // The frame window belongs to the top level we are leaving; clear it so
        // a session surviving a reparent never reports a stale (or destroyed)
        // window to the OLE host.
        session?.SetFrameWindow(0);

        if (topLevel is Window window)
        {
            window.Activated -= OnWindowActivated;
            window.Deactivated -= OnWindowDeactivated;
            window.PropertyChanged -= OnWindowPropertyChanged;
        }

        topLevel = null;

        // NativeControlHost keeps the native control alive across reparenting
        // and destroys it (via DestroyNativeControlCore) only when the view is
        // truly unrooted; the session survives docking and tab moves.
        base.OnDetachedFromVisualTree(e);
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        nint window = CreateWindowExW(
            0,
            StaticWindowClass,
            null,
            WsChild | WsVisible | WsClipSiblings | WsTabStop,
            0,
            0,
            640,
            480,
            parent.Handle,
            0,
            GetModuleHandleW(null),
            0);

        if (window == 0)
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "The RDP host window could not be created.");
        }

        var handle = new RdpHostWindowHandle(window);

        if (disposed)
            return handle;

        try
        {
            StartSession(window);
        }
        catch (Exception exception)
        {
            PublishStatus($"RDP initialization failed: {exception.Message}");
        }

        return handle;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        // Release the OLE host while the window still exists; the base
        // implementation destroys the window itself afterwards.
        TeardownSession();
        base.DestroyNativeControlCore(control);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size arrangedSize = base.ArrangeOverride(finalSize);
        OnViewportChanged(arrangedSize);
        return arrangedSize;
    }

    private void StartSession(nint hostWindow)
    {
        if (session is not null)
            return;

        RdpActiveXSession activeSession = new()
        {
            UseRemoteKeyboardShortcuts = UseRemoteKeyboardShortcuts
        };
        activeSession.Ready += OnSessionReady;
        activeSession.LoginCompleted += OnSessionLoginCompleted;
        activeSession.RemoteDesktopSizeChanged += OnSessionRemoteDesktopSizeChanged;
        activeSession.FocusReleased += OnSessionFocusReleased;
        activeSession.StatusChanged += OnSessionStatusChanged;
        activeSession.Disconnected += OnSessionDisconnected;
        activeSession.FullScreenRequested += OnSessionFullScreenRequested;
        activeSession.LeaveFullScreenRequested += OnSessionLeaveFullScreenRequested;
        activeSession.EnteredFullScreen += OnSessionEnteredFullScreen;
        activeSession.LeftFullScreen += OnSessionLeftFullScreen;
        activeSession.ContainerMinimizeRequested += OnSessionContainerMinimizeRequested;

        session = activeSession;

        try
        {
            PixelSize desktopSize = GetDesktopPixelSize();
            activeSession.Start(hostWindow, desktopSize.Width, desktopSize.Height, ClassId, AxName, RdpExDll);

            if (isViewOnly)
                EnableWindow(hostWindow, false);
        }
        catch
        {
            TeardownSession();
            throw;
        }
    }

    private void TeardownSession()
    {
        RdpActiveXSession? activeSession = session;
        if (activeSession is null)
            return;

        session = null;
        activeSession.Ready -= OnSessionReady;
        activeSession.LoginCompleted -= OnSessionLoginCompleted;
        activeSession.RemoteDesktopSizeChanged -= OnSessionRemoteDesktopSizeChanged;
        activeSession.FocusReleased -= OnSessionFocusReleased;
        activeSession.StatusChanged -= OnSessionStatusChanged;
        activeSession.Disconnected -= OnSessionDisconnected;
        activeSession.FullScreenRequested -= OnSessionFullScreenRequested;
        activeSession.LeaveFullScreenRequested -= OnSessionLeaveFullScreenRequested;
        activeSession.EnteredFullScreen -= OnSessionEnteredFullScreen;
        activeSession.LeftFullScreen -= OnSessionLeftFullScreen;
        activeSession.ContainerMinimizeRequested -= OnSessionContainerMinimizeRequested;
        activeSession.Dispose();
    }

    // Reports the real top-level window to the OLE host so control-owned
    // dialogs (certificate warnings, credential prompts) are parented to a
    // visible window.
    private void UpdateOleFrameState()
    {
        if (session is null)
            return;

        if (topLevel?.TryGetPlatformHandle() is { } platformHandle)
            session.SetFrameWindow(platformHandle.Handle);
    }

    private void OnViewportChanged(Size logicalSize)
    {
        if (logicalSize.Width <= 0 || logicalSize.Height <= 0)
            return;

        double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        PixelSize pixelSize = GetPixelSize(logicalSize, scaling);

        // Keep the control's in-place rectangle tracking the host window
        // immediately; only the server-side resolution renegotiation is
        // debounced below.
        if (session is not null && pixelSize != lastSurfaceSize)
        {
            session.ResizeSurface(pixelSize.Width, pixelSize.Height);
            lastSurfaceSize = pixelSize;
        }

        if (pixelSize != viewportPixelSize)
        {
            viewportPixelSize = pixelSize;
            ViewportPixelSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        QueueDynamicResize(pixelSize, scaling);
    }

    private void QueueDynamicResize(PixelSize pixelSize, double scaling)
    {
        if (!dynamicResolutionEnabled || session is null || !session.IsConnectionActive)
            return;

        pendingDesktopSize = pixelSize;
        pendingRenderScaling = scaling;
        resizeTimer.Stop();
        resizeTimer.Start();
    }

    private void OnResizeTimerTick(object? sender, EventArgs e)
    {
        resizeTimer.Stop();

        // The resize is applied once, after the size has been stable for the
        // debounce interval, so dragging a window edge produces a single
        // UpdateSessionDisplaySettings round-trip instead of a resize storm.
        RdpActiveXSession? activeSession = session;
        if (activeSession is null || !dynamicResolutionEnabled)
            return;

        activeSession.ResizeDisplay(pendingDesktopSize.Width, pendingDesktopSize.Height, pendingRenderScaling);
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        session?.SetFrameActive(true);
    }

    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        session?.SetFrameActive(false);
    }

    private void OnSessionReady(object? sender, EventArgs e)
    {
        ClientReady?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionLoginCompleted(object? sender, EventArgs e)
    {
        // Reconcile the negotiated desktop with the real viewport once the
        // session is up: the initial desktop size came from the connect-time
        // bounds, which may have changed while connecting.
        if (dynamicResolutionEnabled)
        {
            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            QueueDynamicResize(GetPixelSize(Bounds.Size, scaling), scaling);
        }
        else if (session is not null)
        {
            ApplyDisplayMode(session);
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
        FocusReleased?.Invoke(this, e);

        // The control released keyboard focus at an edge of its tab order
        // (the user pressed Tab or Shift+Tab out of the remote session).
        // Move Avalonia focus so focus traversal continues in the hosting
        // application. Anchor the search at the view itself: clicks into the
        // native child window never update Avalonia's focused element, so the
        // default anchor would be stale.
        NavigationDirection direction = e.Direction >= 0
            ? NavigationDirection.Next
            : NavigationDirection.Previous;
        IInputElement? next = TopLevel.GetTopLevel(this)?.FocusManager?.FindNextElement(
            direction,
            new FindNextElementOptions { FocusedElement = this });
        next?.Focus(NavigationMethod.Tab);
    }

    private void OnSessionDisconnected(object? sender, EventArgs e)
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    private void OnSessionStatusChanged(object? sender, RdpStatusChangedEventArgs e)
    {
        PublishStatus(e.Message);
    }

    private void PublishStatus(string message)
    {
        StatusChanged?.Invoke(this, new RdpStatusChangedEventArgs(message));
    }

    private PixelSize GetViewportPixelSize()
    {
        double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        return GetPixelSize(Bounds.Size, scaling);
    }

    private PixelSize GetDesktopPixelSize()
    {
        PixelSize pixelSize = GetViewportPixelSize();
        return new PixelSize(
            Math.Clamp(pixelSize.Width, 200, 8192),
            Math.Clamp(pixelSize.Height, 200, 8192));
    }

    private static PixelSize GetPixelSize(Size logicalSize, double scaling)
    {
        return new PixelSize(
            Math.Max(1, (int)Math.Round(logicalSize.Width * scaling)),
            Math.Max(1, (int)Math.Round(logicalSize.Height * scaling)));
    }

    private sealed class RdpHostWindowHandle(nint handle) : INativeControlHostDestroyableControlHandle
    {
        public nint Handle { get; } = handle;

        public string HandleDescriptor => "HWND";

        public void Destroy() => DestroyWindow(Handle);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern nint GetModuleHandleW(string? moduleName);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    private static extern nint CreateWindowExW(
        uint extendedStyle,
        string className,
        string? windowName,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint module,
        nint parameter);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnableWindow(nint window, bool enable);
}
