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

    public event EventHandler? ClientReady;
    public event EventHandler? LoginCompleted;
    public event EventHandler<RdpRemoteDesktopSizeChangedEventArgs>? RemoteDesktopSizeChanged;
    public event EventHandler<RdpFocusReleasedEventArgs>? FocusReleased;
    public event EventHandler<RdpStatusChangedEventArgs>? StatusChanged;
    public event EventHandler? Disconnected;
    public event EventHandler? ViewportPixelSizeChanged;

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
        dynamicResolutionEnabled = !hasDesktopWidth;
        int desktopWidth = settings.DesktopWidth > 0 ? settings.DesktopWidth : viewportSize.Width;
        int desktopHeight = settings.DesktopHeight > 0 ? settings.DesktopHeight : viewportSize.Height;
        activeSession.UseRemoteKeyboardShortcuts = UseRemoteKeyboardShortcuts;
        activeSession.Connect(settings, desktopWidth, desktopHeight);
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
        dynamicResolutionEnabled = manageDynamicResolution;
        activeSession.UseRemoteKeyboardShortcuts = UseRemoteKeyboardShortcuts;
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

    /// <summary>
    /// Moves native keyboard focus to the hosted RDP control, as if the user
    /// had clicked into the remote desktop.
    /// </summary>
    public void FocusSession()
    {
        VerifyAccess();
        session?.FocusSession();
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
        }

        base.OnAttachedToVisualTree(e);

        // Reparenting to a new top level reuses the live session without
        // re-running StartSession, so re-apply the frame window and the
        // activation state on every attach, not just at session creation.
        UpdateOleFrameState();
        if (topLevel is Window attachedWindow)
            session?.SetFrameActive(attachedWindow.IsActive);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        resizeTimer.Stop();

        // The frame window belongs to the top level we are leaving; clear it so
        // a session surviving a reparent never reports a stale (or destroyed)
        // window to the OLE host.
        session?.SetFrameWindow(0);

        if (topLevel is Window window)
        {
            window.Activated -= OnWindowActivated;
            window.Deactivated -= OnWindowDeactivated;
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
