using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;
using BinaryString = MsRdpEx.Interop.BinaryString;

namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Runs the Microsoft RDP ActiveX control in a caller-owned, visible HWND with a
/// direct OLE client site and in-place site. The control renders natively into
/// that window and receives native keyboard and mouse input; there is no
/// off-screen window, output mirror, or synthetic input forwarding.
/// </summary>
internal sealed class RdpActiveXSession : IDisposable
{
    private static readonly Guid IidIUnknown = new("00000000-0000-0000-C000-000000000046");
    private static readonly Guid IidIClassFactory = new("00000001-0000-0000-C000-000000000046");

    private const int GwChild = 5;

    private static nint msRdpExModule;
    private static string? loadedLibraryPath;
    private static readonly object NativeActivationLock = new();
    private static readonly object OleInitLock = new();
    private static int oleSessionCount;
    private static RdpOleHostAttach? rdpOleHostAttach;
    private static RdpOleHostSetBounds? rdpOleHostSetBounds;
    private static RdpOleHostSetUiActive? rdpOleHostSetUiActive;
    private static RdpOleHostSetFrameActive? rdpOleHostSetFrameActive;
    private static RdpOleHostSetFrameWindow? rdpOleHostSetFrameWindow;
    private static RdpOleHostRelease? rdpOleHostRelease;

    private IMsRdpClient10? client;
    private RdpClientEventSubscription? eventSubscription;
    private nint hostWindow;
    private nint oleHost;
    private bool oleScopeEntered;
    private bool oleInitializedByUs;
    private bool oleUninitializePending;
    private bool disposed;
    private int sessionDesktopWidth;
    private int sessionDesktopHeight;

    public bool IsReady => client is not null;
    public bool IsConnectionActive { get; private set; }
    public bool IsLoginCompleted { get; private set; }
    public nint HostWindowHandle => hostWindow;

    /// <summary>
    /// Gets or sets whether Windows/system shortcuts (Alt+Tab, Windows keys) are
    /// sent to the remote session while the control has focus. Mapped to the
    /// control's own KeyboardHookMode, so keyboard handling stays entirely
    /// native. Applied when connecting; set before <see cref="Connect"/>.
    /// </summary>
    public bool UseRemoteKeyboardShortcuts { get; set; } = true;

    public event EventHandler? Ready;
    public event EventHandler? LoginCompleted;
    public event EventHandler<RdpRemoteDesktopSizeChangedEventArgs>? RemoteDesktopSizeChanged;
    public event EventHandler<RdpFocusReleasedEventArgs>? FocusReleased;
    public event EventHandler<RdpStatusChangedEventArgs>? StatusChanged;
    public event EventHandler? Disconnected;

    /// <summary>
    /// Activates the RDP ActiveX in-place in <paramref name="hostWindow"/>. The
    /// caller owns the window and must keep it alive until <see cref="Dispose"/>.
    /// </summary>
    public void Start(nint hostWindow, int width, int height, Guid classId, string axName, string? rdpExDll)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (client is not null)
            return;

        if (hostWindow == 0 || !IsWindow(hostWindow))
            throw new ArgumentException("A valid host window handle is required.", nameof(hostWindow));
        if (classId == Guid.Empty)
            throw new ArgumentException("An RDP ActiveX class identifier is required.", nameof(classId));
        if (string.IsNullOrWhiteSpace(axName))
            throw new ArgumentException("An RDP ActiveX name is required.", nameof(axName));

        string libraryPath = ResolveLibraryPath(rdpExDll);

        try
        {
            // Every successful OleInitialize (S_OK or S_FALSE) increments the
            // thread's OLE initialization count and must be balanced once.
            // OleUninitialize is deferred when other sessions remain and, when
            // this call got S_FALSE, deferred to a later S_OK-initialized
            // session so OLE initialized by the hosting application (Avalonia's
            // OleContext never uninitializes) is not torn down underneath it.
            int oleResult = OleInitialize(0);
            Marshal.ThrowExceptionForHR(oleResult);
            oleInitializedByUs = oleResult == 0; // S_OK
            EnterOleScope();
            oleScopeEntered = true;

            int surfaceWidth = ClampDesktopDimension(width);
            int surfaceHeight = ClampDesktopDimension(height);

            nint control;
            lock (NativeActivationLock)
            {
                string normalizedPath = Path.GetFullPath(libraryPath);
                if (loadedLibraryPath is not null &&
                    !string.Equals(loadedLibraryPath, normalizedPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"RdpClientView already loaded MsRdpEx.dll from '{loadedLibraryPath}'. " +
                        "All views in one process must use the same native library.");
                }

                control = WithAxNameEnvironment(axName.Trim(), () =>
                {
                    msRdpExModule = msRdpExModule == 0 ? NativeLibrary.Load(normalizedPath) : msRdpExModule;
                    loadedLibraryPath ??= normalizedPath;
                    EnsureOleHostExports(msRdpExModule);
                    return CreateComInstance(msRdpExModule, classId);
                });
            }

            this.hostWindow = hostWindow;

            unsafe
            {
                try
                {
                    NativeRect bounds = new()
                    {
                        Right = surfaceWidth,
                        Bottom = surfaceHeight
                    };
                    int hr = rdpOleHostAttach!(control, hostWindow, ref bounds, out oleHost);
                    Marshal.ThrowExceptionForHR(hr);

                    object rawClient = ComInterfaceMarshaller<object>.ConvertToManaged((void*)control)
                        ?? throw new InvalidOperationException("The ActiveX host returned an empty RDP control instance.");

                    client = ProxyObject.Pack<IMsRdpClient10>(rawClient)
                        ?? throw new InvalidOperationException("The hosted control does not implement IMsRdpClient10.");
                }
                finally
                {
                    ComInterfaceMarshaller<object>.Free((void*)control);
                }
            }

            // The control is hosted in a real on-screen window: UI-activate it so
            // it owns native focus and keyboard handling like any windowed RDP
            // client. Skipped gracefully when the loaded MsRdpEx.dll predates the
            // SetUiActive export; in-place activation still renders content.
            if (rdpOleHostSetUiActive is not null)
            {
                int hr = rdpOleHostSetUiActive(oleHost, 1);
                if (hr < 0)
                    PublishStatus($"RDP UI activation failed: 0x{hr:X8}");
            }

            SubscribeToClientEvents(client);
            PublishStatus("RDP native surface ready.");
            Ready?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            DisposeNativeResources();
            throw;
        }
    }

    public void Connect(RdpConnectionSettings settings, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10 rdpClient = client
            ?? throw new InvalidOperationException("The RDP session is not ready yet.");

        if (string.IsNullOrWhiteSpace(settings.HostName))
            throw new ArgumentException("A host name is required.", nameof(settings));

        if (rdpClient.Connected != 0)
            throw new InvalidOperationException("Disconnect the current session before connecting again.");

        int desktopWidth = ClampDesktopDimension(width);
        int desktopHeight = ClampDesktopDimension(height);
        ResizeSurface(desktopWidth, desktopHeight);
        sessionDesktopWidth = desktopWidth;
        sessionDesktopHeight = desktopHeight;

        if (!string.IsNullOrWhiteSpace(settings.RdpFileContents))
            rdpClient.MsRdpClientShell.RdpFileContents = new BinaryString(settings.RdpFileContents);

        rdpClient.Server = new BinaryString(settings.HostName.Trim());
        rdpClient.UserName = new BinaryString(settings.UserName.Trim());
        rdpClient.Domain = new BinaryString(settings.Domain.Trim());
        rdpClient.DesktopWidth = desktopWidth;
        rdpClient.DesktopHeight = desktopHeight;
        rdpClient.ColorDepth = 32;

        IMsRdpClientAdvancedSettings8 advancedSettings = rdpClient.AdvancedSettings9;
        advancedSettings.EnableCredSspSupport = true;
        advancedSettings.SmartSizing = false;
        advancedSettings.RedirectClipboard = settings.RedirectClipboard;

        IMsRdpClientSecuredSettings2 securedSettings = rdpClient.SecuredSettings3;
        securedSettings.KeyboardHookMode = UseRemoteKeyboardShortcuts ? 1 : 0;

        object? rawClient = ProxyObject.Unpack(rdpClient);
        IMsTscNonScriptable nonScriptable = ProxyObject.Pack<IMsTscNonScriptable>(rawClient)
            ?? throw new InvalidOperationException("The RDP control does not expose its credential interface.");

        if (string.IsNullOrEmpty(settings.Password))
            nonScriptable.ResetPassword();
        else
            nonScriptable.ClearTextPassword = new BinaryString(settings.Password);

        ConnectClient(rdpClient, settings.HostName.Trim());
    }

    public void ConnectConfigured(int width, int height)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10 rdpClient = client
            ?? throw new InvalidOperationException("The RDP session is not ready yet.");
        if (rdpClient.Connected != 0)
            throw new InvalidOperationException("Disconnect the current session before connecting again.");

        int desktopWidth = ClampDesktopDimension(width);
        int desktopHeight = ClampDesktopDimension(height);
        ResizeSurface(desktopWidth, desktopHeight);
        sessionDesktopWidth = desktopWidth;
        sessionDesktopHeight = desktopHeight;
        ConnectClient(rdpClient, null);
    }

    public T GetClient<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10 rdpClient = client
            ?? throw new InvalidOperationException("The RDP session is not ready yet.");
        object? rawClient = ProxyObject.Unpack(rdpClient);
        return ProxyObject.Pack<T>(rawClient)
            ?? throw new InvalidCastException($"The hosted RDP control does not implement {typeof(T).FullName}.");
    }

    public void Disconnect()
    {
        if (client is not null && client.Connected != 0)
        {
            PublishStatus("Disconnecting...");
            client.Disconnect();
        }
    }

    /// <summary>
    /// Reports the real top-level window as the OLE frame window so that
    /// control-owned dialogs (certificate warnings, credential prompts) are
    /// parented to a visible window.
    /// </summary>
    public void SetFrameWindow(nint frameWindow)
    {
        if (disposed || oleHost == 0 || rdpOleHostSetFrameWindow is null)
            return;

        int hr = rdpOleHostSetFrameWindow(oleHost, frameWindow);
        if (hr < 0)
            PublishStatus($"RDP OLE frame window update failed: 0x{hr:X8}");
    }

    /// <summary>
    /// Forwards top-level window activation to the control, matching standard
    /// OLE in-place hosting (AxHost) behavior.
    /// </summary>
    public void SetFrameActive(bool active)
    {
        if (disposed || oleHost == 0 || rdpOleHostSetFrameActive is null)
            return;

        int hr = rdpOleHostSetFrameActive(oleHost, active ? 1 : 0);
        if (hr < 0)
            PublishStatus($"RDP OLE frame activation failed: 0x{hr:X8}");
    }

    /// <summary>
    /// Moves the control's in-place rectangle to match the host window's client
    /// area. This only repositions the control; use
    /// <see cref="UpdateSessionDisplaySettings"/> to renegotiate the remote
    /// desktop size.
    /// </summary>
    public bool ResizeSurface(int width, int height)
    {
        if (disposed || oleHost == 0)
            return false;

        int surfaceWidth = Math.Max(width, 1);
        int surfaceHeight = Math.Max(height, 1);
        NativeRect bounds = new()
        {
            Right = surfaceWidth,
            Bottom = surfaceHeight
        };

        if (rdpOleHostSetBounds is null)
            return false;

        int hr = rdpOleHostSetBounds(oleHost, ref bounds);
        if (hr < 0)
        {
            PublishStatus($"RDP OLE host resize failed: 0x{hr:X8}");
            return false;
        }

        return true;
    }

    public bool UpdateSessionDisplaySettings(
        uint desktopWidth,
        uint desktopHeight,
        uint physicalWidth,
        uint physicalHeight,
        uint orientation,
        uint desktopScaleFactor,
        uint deviceScaleFactor)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10? rdpClient = client;
        if (rdpClient is null)
            return false;

        int hostWidth = MakeEven(ClampDesktopDimension(checked((int)desktopWidth)));
        int hostHeight = MakeEven(ClampDesktopDimension(checked((int)desktopHeight)));
        ResizeSurface(hostWidth, hostHeight);

        if (!IsLoginCompleted || rdpClient.Connected == 0)
            return false;

        try
        {
            rdpClient.UpdateSessionDisplaySettings(
                (uint)hostWidth,
                (uint)hostHeight,
                physicalWidth,
                physicalHeight,
                orientation,
                desktopScaleFactor,
                deviceScaleFactor);
            sessionDesktopWidth = hostWidth;
            sessionDesktopHeight = hostHeight;
            return true;
        }
        catch (COMException exception)
        {
            PublishStatus($"Dynamic resolution update failed: {exception.Message}");
            return false;
        }
    }

    /// <summary>
    /// Applies a debounced dynamic-resolution update for the current host size.
    /// </summary>
    public bool ResizeDisplay(int width, int height, double renderScaling)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10? rdpClient = client;
        if (rdpClient is null)
            return false;

        int desktopWidth = MakeEven(ClampDesktopDimension(width));
        int desktopHeight = MakeEven(ClampDesktopDimension(height));

        if (!IsLoginCompleted || rdpClient.Connected == 0)
            return false;

        if (desktopWidth == sessionDesktopWidth && desktopHeight == sessionDesktopHeight)
            return true;

        uint desktopScaleFactor = GetDesktopScaleFactor(renderScaling);
        uint deviceScaleFactor = desktopScaleFactor switch
        {
            < 125 => 100,
            < 200 => 140,
            _ => 180
        };

        try
        {
            rdpClient.UpdateSessionDisplaySettings(
                (uint)desktopWidth,
                (uint)desktopHeight,
                (uint)desktopWidth,
                (uint)desktopHeight,
                0,
                desktopScaleFactor,
                deviceScaleFactor);
            sessionDesktopWidth = desktopWidth;
            sessionDesktopHeight = desktopHeight;
            return true;
        }
        catch (COMException exception)
        {
            PublishStatus($"Dynamic resolution update failed: {exception.Message}");
            return false;
        }
    }

    /// <summary>
    /// Moves native keyboard focus to the hosted RDP control.
    /// </summary>
    public void FocusSession()
    {
        if (disposed || hostWindow == 0)
            return;

        // The control's in-place window is the first child of the host window;
        // focusing it lets mstscax route focus to its own input window.
        nint controlWindow = GetWindow(hostWindow, GwChild);
        if (controlWindow != 0)
            SetFocus(controlWindow);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;

        try
        {
            Disconnect();
        }
        catch (COMException)
        {
            // The ActiveX control may already be tearing down.
        }

        DisposeNativeResources();
    }

    private static void EnsureOleHostExports(nint library)
    {
        rdpOleHostAttach ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostAttach>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_Attach"));
        rdpOleHostSetBounds ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostSetBounds>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_SetBounds"));
        rdpOleHostRelease ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostRelease>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_Release"));

        if (rdpOleHostSetUiActive is null &&
            NativeLibrary.TryGetExport(library, "MsRdpEx_RdpOleHost_SetUiActive", out nint setUiActive))
        {
            rdpOleHostSetUiActive =
                Marshal.GetDelegateForFunctionPointer<RdpOleHostSetUiActive>(setUiActive);
        }

        if (rdpOleHostSetFrameActive is null &&
            NativeLibrary.TryGetExport(library, "MsRdpEx_RdpOleHost_SetFrameActive", out nint setFrameActive))
        {
            rdpOleHostSetFrameActive =
                Marshal.GetDelegateForFunctionPointer<RdpOleHostSetFrameActive>(setFrameActive);
        }

        if (rdpOleHostSetFrameWindow is null &&
            NativeLibrary.TryGetExport(library, "MsRdpEx_RdpOleHost_SetFrameWindow", out nint setFrameWindow))
        {
            rdpOleHostSetFrameWindow =
                Marshal.GetDelegateForFunctionPointer<RdpOleHostSetFrameWindow>(setFrameWindow);
        }
    }

    // MSRDPEX_AXNAME is a process-global read by MsRdpEx's DllGetClassObject.
    // Scope it to the activation call and restore the previous value so that
    // other in-process RDP activators never observe a stale name. Callers must
    // hold NativeActivationLock; components activating RDP controls outside
    // this lock must set and restore the variable themselves.
    internal static T WithAxNameEnvironment<T>(string axName, Func<T> action)
    {
        string? previousAxName = Environment.GetEnvironmentVariable("MSRDPEX_AXNAME");
        Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", axName);
        try
        {
            return action();
        }
        finally
        {
            Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", previousAxName);
        }
    }

    internal static void EnterOleScope()
    {
        lock (OleInitLock)
        {
            oleSessionCount++;
        }
    }

    // Test-only: resets the process-wide session count so the OLE scope rules
    // can be exercised deterministically in randomized test order.
    internal static void ResetOleScopeForTesting()
    {
        lock (OleInitLock)
        {
            oleSessionCount = 0;
        }
    }

    // Returns true when the caller must balance its OleInitialize now: when
    // it is the last remaining session and either it performed the S_OK
    // initialization itself or it is inheriting a balance deferred by an
    // earlier session that got S_FALSE (which must not run while Avalonia may
    // still own OLE). Sessions are assumed to live on the same (UI) thread,
    // matching OleInitialize's per-thread semantics.
    internal static bool ExitOleScope(bool initializedByUs)
    {
        lock (OleInitLock)
        {
            oleSessionCount--;
            return oleSessionCount == 0 && initializedByUs;
        }
    }

    // Hands this session's OleInitialize balance to an existing older session
    // that got S_FALSE, so that session's dispose performs the balancing
    // OleUninitialize instead of this one. Called when a new session that owns
    // the OLE initialization is created while foreign-initialized sessions are
    // still alive; without it the new session's dispose would tear down OLE
    // underneath the surviving sessions.
    internal void MarkOleUninitializePending()
    {
        oleUninitializePending = true;
    }

    private static string ResolveLibraryPath(string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            string explicitPath = Path.IsPathFullyQualified(configuredPath)
                ? configuredPath
                : Path.GetFullPath(configuredPath, AppContext.BaseDirectory);
            if (File.Exists(explicitPath))
                return explicitPath;

            throw new FileNotFoundException("The configured MsRdpEx.dll could not be found.", explicitPath);
        }

        string architecture = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant()
        };
        string[] candidates =
        [
            Path.Combine(AppContext.BaseDirectory, "MsRdpEx.dll"),
            Path.Combine(AppContext.BaseDirectory, "runtimes", $"win-{architecture}", "native", "MsRdpEx.dll")
        ];

        foreach (string candidate in candidates)
        {
            if (File.Exists(candidate))
                return candidate;
        }

        throw new FileNotFoundException(
            $"MsRdpEx.dll is required to host the RDP ActiveX control. Expected a {architecture} native asset beside the application or under its runtimes folder.",
            candidates[0]);
    }

    private void SubscribeToClientEvents(IMsRdpClient rdpClient)
    {
        eventSubscription = rdpClient.Subscribe();
        eventSubscription.Connecting += () =>
        {
            IsConnectionActive = true;
            PublishStatus("Connecting...");
        };
        eventSubscription.Connected += () => PublishStatus("Transport connected. Signing in...");
        eventSubscription.LoginCompleted += () =>
        {
            IsLoginCompleted = true;
            PublishStatus("Connected.");
            LoginCompleted?.Invoke(this, EventArgs.Empty);
        };
        eventSubscription.RemoteDesktopSizeChanged += (width, height) =>
            RemoteDesktopSizeChanged?.Invoke(this, new RdpRemoteDesktopSizeChangedEventArgs(width, height));
        eventSubscription.FocusReleased += direction =>
            FocusReleased?.Invoke(this, new RdpFocusReleasedEventArgs(direction));
        eventSubscription.DisconnectedWithDetails += (_, args) =>
        {
            IsConnectionActive = false;
            IsLoginCompleted = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
            string detail = string.IsNullOrWhiteSpace(args.DisconnectErrorMessage)
                ? $"reason {args.DisconnectReason}"
                : args.DisconnectErrorMessage;
            PublishStatus($"Disconnected: {detail}");
        };
        eventSubscription.FatalError += errorCode => PublishStatus($"RDP fatal error: {errorCode}");
        eventSubscription.LogonError += errorCode => PublishStatus($"RDP logon error: {errorCode}");
        eventSubscription.AuthenticationWarningDisplayed += () => PublishStatus("Waiting for certificate confirmation...");
    }

    private void PublishStatus(string message)
    {
        StatusChanged?.Invoke(this, new RdpStatusChangedEventArgs(message));
    }

    private void ConnectClient(IMsRdpClient10 rdpClient, string? hostName)
    {
        IsConnectionActive = true;
        IsLoginCompleted = false;

        try
        {
            PublishStatus(string.IsNullOrWhiteSpace(hostName)
                ? "Connecting..."
                : $"Connecting to {hostName}...");
            rdpClient.Connect();
        }
        catch
        {
            IsConnectionActive = false;
            throw;
        }
    }

    private void DisposeNativeResources()
    {
        try
        {
            eventSubscription?.Dispose();
        }
        catch (COMException)
        {
            // The connection point can already be gone during control teardown.
        }
        finally
        {
            eventSubscription = null;
        }

        // The OLE host closes, deactivates, and de-sites the control on this
        // (UI) thread before any managed wrapper is released.
        if (oleHost != 0)
        {
            nint host = oleHost;
            oleHost = 0;
            rdpOleHostRelease?.Invoke(host);
        }

        // Release the managed COM wrapper deterministically where the runtime
        // supports it. The generated interop wraps the control through
        // ComInterfaceMarshaller (StrategyBasedComWrappers), which offers no
        // deterministic release; because the OLE host above has already closed
        // and de-sited the control, that wrapper's final GC release is inert.
        // A built-in RCW (legacy interop path) is released explicitly here.
        object? rawClient = client is not null ? ProxyObject.Unpack(client) : null;
        client = null;
        IsConnectionActive = false;
        IsLoginCompleted = false;

        if (rawClient is not null && Marshal.IsComObject(rawClient))
            Marshal.FinalReleaseComObject(rawClient);

        // The host window is owned by the caller (NativeControlHost) and is
        // destroyed by it after the OLE host is released.
        hostWindow = 0;

        if (oleScopeEntered)
        {
            oleScopeEntered = false;
            if (oleUninitializePending)
            {
                // This session inherited the OleInitialize balance deferred by
                // an earlier session that got S_FALSE and is now the last one.
                oleUninitializePending = false;
                ExitOleScope(true);
                OleUninitialize();
            }
            else
            {
                bool initializedByUs = oleInitializedByUs;
                oleInitializedByUs = false;
                if (ExitOleScope(initializedByUs))
                    OleUninitialize();
            }
        }
    }

    private static unsafe nint CreateComInstance(nint library, Guid classId)
    {
        nint export = NativeLibrary.GetExport(library, "DllGetClassObject");
        DllGetClassObject getClassObject = Marshal.GetDelegateForFunctionPointer<DllGetClassObject>(export);

        Guid classFactoryId = IidIClassFactory;
        int hr = getClassObject(ref classId, ref classFactoryId, out nint factory);
        Marshal.ThrowExceptionForHR(hr);

        try
        {
            Guid unknownId = IidIUnknown;
            nint instance;
            nint* vtable = *(nint**)factory;
            var createInstance = (delegate* unmanaged[Stdcall]<nint, nint, Guid*, nint*, int>)vtable[3];
            hr = createInstance(factory, 0, &unknownId, &instance);
            Marshal.ThrowExceptionForHR(hr);
            return instance;
        }
        finally
        {
            Marshal.Release(factory);
        }
    }

    private static int MakeEven(int value)
    {
        return value - (value % 2);
    }

    private static int ClampDesktopDimension(int value)
    {
        // Modern Microsoft RDP controls accept desktop dimensions from 200 to
        // 8192 pixels. Clamp explicitly so small panes and extreme HiDPI layouts
        // have deterministic behavior instead of failing later with E_INVALIDARG.
        return Math.Clamp(value, 200, 8192);
    }

    private static uint GetDesktopScaleFactor(double renderScaling)
    {
        int percentage = Math.Clamp((int)Math.Round(renderScaling * 100), 100, 500);
        return percentage switch
        {
            < 113 => 100,
            < 138 => 125,
            < 163 => 150,
            < 188 => 175,
            < 213 => 200,
            < 238 => 225,
            < 275 => 250,
            < 350 => 300,
            < 450 => 400,
            _ => 500
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllGetClassObject(ref Guid classId, ref Guid interfaceId, out nint classFactory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostAttach(nint control, nint window, ref NativeRect bounds, out nint host);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostSetBounds(nint host, ref NativeRect bounds);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostSetUiActive(nint host, int active);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostSetFrameActive(nint host, int active);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostSetFrameWindow(nint host, nint frameWindow);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void RdpOleHostRelease(nint host);

    [DllImport("ole32.dll", ExactSpelling = true)]
    private static extern int OleInitialize(nint reserved);

    [DllImport("ole32.dll", ExactSpelling = true)]
    private static extern void OleUninitialize();

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetWindow(nint window, int command);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint SetFocus(nint window);
}
