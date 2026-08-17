using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;
using BinaryString = MsRdpEx.Interop.BinaryString;

namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Runs the Microsoft RDP ActiveX control in an off-screen HWND with a direct
/// OLE client site and in-place site. The window is only the RDP session engine;
/// presentation and input belong to the Avalonia control.
/// </summary>
internal sealed class RdpActiveXSession : IDisposable
{
    private const string StaticWindowClass = "STATIC";
    private static readonly Guid IidIUnknown = new("00000000-0000-0000-C000-000000000046");
    private static readonly Guid IidIClassFactory = new("00000001-0000-0000-C000-000000000046");

    private const uint WsPopup = 0x80000000;
    private const uint WsVisible = 0x10000000;
    private const uint WsClipSiblings = 0x04000000;
    private const uint WsClipChildren = 0x02000000;
    private const uint WsTabStop = 0x00010000;
    private const uint WsExToolWindow = 0x00000080;
    private const uint WsExNoActivate = 0x08000000;

    private const uint SwpNoActivate = 0x0010;
    private const uint SwpShowWindow = 0x0040;

    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmRightButtonDown = 0x0204;
    private const uint WmRightButtonUp = 0x0205;
    private const uint WmMiddleButtonDown = 0x0207;
    private const uint WmMiddleButtonUp = 0x0208;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmXButtonDown = 0x020B;
    private const uint WmXButtonUp = 0x020C;
    private const uint WmMouseHorizontalWheel = 0x020E;
    private const uint WmMouseLeave = 0x02A3;
    private const uint WmSetCursor = 0x0020;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const uint WmSystemKeyDown = 0x0104;
    private const uint WmSystemKeyUp = 0x0105;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmNcDestroy = 0x0082;

    private const uint MapvkVkToVsc = 0;
    private const int HtClient = 1;
    private const NativeMouseButtons MouseButtonMask = NativeMouseButtons.Left |
                                                        NativeMouseButtons.Right |
                                                        NativeMouseButtons.Middle |
                                                        NativeMouseButtons.XButton1 |
                                                        NativeMouseButtons.XButton2;

    private static nint msRdpExModule;
    private static string? loadedLibraryPath;
    private static readonly object NativeActivationLock = new();
    private static RdpOleHostAttach? rdpOleHostAttach;
    private static RdpOleHostSetBounds? rdpOleHostSetBounds;
    private static RdpOleHostTranslateAccelerator? rdpOleHostTranslateAccelerator;
    private static RdpOleHostRelease? rdpOleHostRelease;
    private static MsRdpExInstanceHandle.OutputMirrorCaptureApi? outputMirrorCaptureApi;
    private static MsRdpExInstanceHandle.OutputMirrorGetFrameVersion? outputMirrorGetFrameVersion;
    private static readonly UIntPtr InputWindowSubclassId = new(1);

    private IMsRdpClient10? client;
    private RdpClientEventSubscription? eventSubscription;
    private readonly SubclassProc inputWindowSubclassProc;
    private MsRdpExInstanceHandle? instance;
    private nint hostWindow;
    private nint oleHost;
    private nint inputWindowSubclassHandle;
    private bool oleInitialized;
    private bool disposed;
    private bool remoteMouseDragActive;
    private int sessionDesktopWidth;
    private int sessionDesktopHeight;

    public bool IsReady => client is not null;
    public bool IsConnectionActive { get; private set; }
    public bool IsLoginCompleted { get; private set; }
    public nint HostWindowHandle => hostWindow;
    public nint InputWindowHandle => GetInputWindow();

    public event EventHandler? Ready;
    public event EventHandler? LoginCompleted;
    public event EventHandler<RdpRemoteDesktopSizeChangedEventArgs>? RemoteDesktopSizeChanged;
    public event EventHandler<RdpFocusReleasedEventArgs>? FocusReleased;
    public event EventHandler<RdpStatusChangedEventArgs>? StatusChanged;

    public RdpActiveXSession()
    {
        inputWindowSubclassProc = InputWindowSubclassProc;
    }

    public void Start(int width, int height, Guid classId, string axName, string? rdpExDll)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (client is not null)
            return;

        if (classId == Guid.Empty)
            throw new ArgumentException("An RDP ActiveX class identifier is required.", nameof(classId));
        if (string.IsNullOrWhiteSpace(axName))
            throw new ArgumentException("An RDP ActiveX name is required.", nameof(axName));

        string libraryPath = ResolveLibraryPath(rdpExDll);

        try
        {
            int oleResult = OleInitialize(0);
            Marshal.ThrowExceptionForHR(oleResult);
            oleInitialized = true;

            int surfaceWidth = ClampDesktopDimension(width);
            int surfaceHeight = ClampDesktopDimension(height);
            nint executable = GetModuleHandleW(null);

            hostWindow = CreateWindowExW(
                WsExToolWindow | WsExNoActivate,
                StaticWindowClass,
                null,
                WsPopup | WsVisible | WsClipSiblings | WsClipChildren | WsTabStop,
                -32000,
                -32000,
                surfaceWidth,
                surfaceHeight,
                0,
                0,
                executable,
                0);

            if (hostWindow == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "The off-screen RDP host window could not be created.");

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

                Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", axName.Trim());
                msRdpExModule = msRdpExModule == 0 ? NativeLibrary.Load(normalizedPath) : msRdpExModule;
                loadedLibraryPath ??= normalizedPath;
                EnsureOleHostExports(msRdpExModule);
                control = CreateComInstance(msRdpExModule, classId);
            }

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

                    instance = new MsRdpExInstanceHandle(
                        control,
                        outputMirrorCaptureApi,
                        outputMirrorGetFrameVersion);
                }
                finally
                {
                    ComInterfaceMarshaller<object>.Free((void*)control);
                }
            }

            SubscribeToClientEvents(client);
            PublishStatus("RDP bitmap surface ready (MsRdpEx output mirror)." );
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
            ?? throw new InvalidOperationException("The off-screen RDP session is not ready yet.");

        if (string.IsNullOrWhiteSpace(settings.HostName))
            throw new ArgumentException("A host name is required.", nameof(settings));

        if (rdpClient.Connected != 0)
            throw new InvalidOperationException("Disconnect the current session before connecting again.");

        int desktopWidth = ClampDesktopDimension(width);
        int desktopHeight = ClampDesktopDimension(height);
        ResizeHost(desktopWidth, desktopHeight);
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
        advancedSettings.GrabFocusOnConnect = false;
        advancedSettings.allowBackgroundInput = 1;
        advancedSettings.RedirectClipboard = settings.RedirectClipboard;

        object? rawClient = ProxyObject.Unpack(rdpClient);
        IMsTscNonScriptable nonScriptable = ProxyObject.Pack<IMsTscNonScriptable>(rawClient)
            ?? throw new InvalidOperationException("The RDP control does not expose its credential interface.");

        EnableOutputMirror(rdpClient);

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
            ?? throw new InvalidOperationException("The off-screen RDP session is not ready yet.");
        if (rdpClient.Connected != 0)
            throw new InvalidOperationException("Disconnect the current session before connecting again.");

        int desktopWidth = ClampDesktopDimension(width);
        int desktopHeight = ClampDesktopDimension(height);
        ResizeHost(desktopWidth, desktopHeight);
        sessionDesktopWidth = desktopWidth;
        sessionDesktopHeight = desktopHeight;
        EnableOutputMirror(rdpClient);
        ConnectClient(rdpClient, null);
    }

    public T GetClient<T>() where T : class
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10 rdpClient = client
            ?? throw new InvalidOperationException("The off-screen RDP session is not ready yet.");
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

    public bool ResizeDisplay(int width, int height, double renderScaling)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        IMsRdpClient10? rdpClient = client;
        if (rdpClient is null)
            return false;

        int desktopWidth = MakeEven(ClampDesktopDimension(width));
        int desktopHeight = MakeEven(ClampDesktopDimension(height));
        ResizeHost(desktopWidth, desktopHeight);

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
        ResizeHost(hostWidth, hostHeight);

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

    public bool WithShadowBitmap(Action<nint, int, int, int> action)
    {
        return instance?.WithShadowBitmap(action) == true;
    }

    public bool CanCaptureOffThread => instance?.CanCaptureOffThread == true;

    public bool TryGetShadowBitmapFrameVersion(out uint version)
    {
        if (instance is not null)
            return instance.TryGetShadowBitmapFrameVersion(out version);

        version = 0;
        return false;
    }

    public void SendMouseMove(int x, int y, NativeMouseButtons buttons)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow == 0)
            return;

        SendMessageW(inputWindow, WmMouseMove, (nuint)(uint)buttons, MakeLParam(x, y));
        SynchronizeCursor(inputWindow);
    }

    public void SendMouseButton(
        uint message, int x, int y, NativeMouseButtons buttons, ushort xButton = 0)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow == 0)
            return;

        bool buttonDown = message is WmLeftButtonDown or WmRightButtonDown or WmMiddleButtonDown or WmXButtonDown;
        bool finalButtonUp = message is WmLeftButtonUp or WmRightButtonUp or WmMiddleButtonUp or WmXButtonUp &&
                             (buttons & MouseButtonMask) == NativeMouseButtons.None;

        if (buttonDown)
        {
            EnsureInputWindowSubclass(inputWindow);
            remoteMouseDragActive = true;
        }
        else if (finalButtonUp)
        {
            remoteMouseDragActive = false;
        }

        nuint wParam = (nuint)((uint)buttons | ((uint)xButton << 16));
        SendMessageW(inputWindow, message, wParam, MakeLParam(x, y));
        SynchronizeCursor(inputWindow);
    }

    public void SendMouseWheel(int x, int y, int delta, NativeMouseButtons buttons)
    {
        SendMouseWheelMessage(WmMouseWheel, x, y, delta, buttons);
    }

    public void SendMouseHorizontalWheel(int x, int y, int delta, NativeMouseButtons buttons)
    {
        SendMouseWheelMessage(WmMouseHorizontalWheel, x, y, delta, buttons);
    }

    public bool TryGetInputSize(out int width, out int height)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow == 0 || !GetClientRect(inputWindow, out NativeRect bounds))
        {
            width = 0;
            height = 0;
            return false;
        }

        width = bounds.Right - bounds.Left;
        height = bounds.Bottom - bounds.Top;
        return width > 0 && height > 0;
    }

    public void SendMouseLeave()
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow != 0)
            SendMessageW(inputWindow, WmMouseLeave, 0, 0);
    }

    public void SendKey(int virtualKey, bool keyUp, bool extended, bool systemKey)
    {
        uint scanCode = MapVirtualKeyW((uint)virtualKey, MapvkVkToVsc);
        SendKey(virtualKey, scanCode, keyUp, extended, systemKey);
    }

    public void SendKey(int virtualKey, uint scanCode, bool keyUp, bool extended, bool systemKey)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow == 0 || virtualKey == 0)
            return;

        nint lParam = 1 | ((nint)scanCode << 16);

        if (extended)
            lParam |= (nint)1 << 24;

        if (systemKey)
            lParam |= (nint)1 << 29;

        if (keyUp)
            lParam |= (nint)3 << 30;

        uint message = systemKey
            ? keyUp ? WmSystemKeyUp : WmSystemKeyDown
            : keyUp ? WmKeyUp : WmKeyDown;

        if (TryTranslateAccelerator(inputWindow, message, (nuint)virtualKey, lParam))
            return;

        SendMessageW(inputWindow, message, (nuint)virtualKey, lParam);
    }

    public void SendCharacter(char character)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow != 0)
            SendMessageW(inputWindow, WmChar, character, 0);
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

    private void ResizeHost(int width, int height)
    {
        if (hostWindow != 0)
        {
            SetWindowPos(hostWindow, 0, -32000, -32000, width, height, SwpNoActivate | SwpShowWindow);
        }

        if (oleHost != 0 && rdpOleHostSetBounds is not null)
        {
            NativeRect bounds = new()
            {
                Right = width,
                Bottom = height
            };
            int hr = rdpOleHostSetBounds(oleHost, ref bounds);
            if (hr < 0)
                PublishStatus($"RDP OLE host resize failed: 0x{hr:X8}");
        }
    }

    private void EnableOutputMirror(IMsRdpClient10 rdpClient)
    {
        object? rawClient = ProxyObject.Unpack(rdpClient);
        IMsRdpExtendedSettings extendedSettings = ProxyObject.Pack<IMsRdpExtendedSettings>(rawClient)
            ?? throw new InvalidOperationException("The RDP control does not expose MsRdpEx extended settings.");
        extendedSettings.SetProperty(new BinaryString("OutputMirrorEnabled"), true);
        extendedSettings.SetProperty(new BinaryString("EnableHardwareMode"), false);
        instance?.SetOutputMirrorEnabled(true);
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

    private static void EnsureOleHostExports(nint library)
    {
        rdpOleHostAttach ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostAttach>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_Attach"));
        rdpOleHostSetBounds ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostSetBounds>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_SetBounds"));
        rdpOleHostTranslateAccelerator ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostTranslateAccelerator>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_TranslateAccelerator"));
        rdpOleHostRelease ??= Marshal.GetDelegateForFunctionPointer<RdpOleHostRelease>(
            NativeLibrary.GetExport(library, "MsRdpEx_RdpOleHost_Release"));

        if (outputMirrorGetFrameVersion is null &&
            NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirror_GetFrameVersion", out nint export))
        {
            outputMirrorGetFrameVersion =
                Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorGetFrameVersion>(export);
        }

        outputMirrorCaptureApi ??= TryLoadOutputMirrorCaptureApi(library);
    }

    private static MsRdpExInstanceHandle.OutputMirrorCaptureApi? TryLoadOutputMirrorCaptureApi(nint library)
    {
        if (!NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_Create", out nint create) ||
            !NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_GetFrameVersion", out nint getFrameVersion) ||
            !NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_GetShadowBitmap", out nint getShadowBitmap) ||
            !NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_Lock", out nint lockCapture) ||
            !NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_Unlock", out nint unlockCapture) ||
            !NativeLibrary.TryGetExport(library, "MsRdpEx_OutputMirrorCapture_Release", out nint release))
        {
            return null;
        }

        return new MsRdpExInstanceHandle.OutputMirrorCaptureApi(
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureCreate>(create),
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureGetFrameVersion>(getFrameVersion),
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureGetShadowBitmap>(getShadowBitmap),
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureLock>(lockCapture),
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureUnlock>(unlockCapture),
            Marshal.GetDelegateForFunctionPointer<MsRdpExInstanceHandle.OutputMirrorCaptureRelease>(release));
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
            $"MsRdpEx.dll is required for Avalonia bitmap rendering. Expected a {architecture} native asset beside the application or under its runtimes folder.",
            candidates[0]);
    }

    private static void SynchronizeCursor(nint inputWindow)
    {
        // The off-screen input HWND never receives the WM_SETCURSOR Windows would
        // normally send before WM_MOUSEMOVE. Ask it to select its current remote
        // cursor explicitly so the pointer over the Avalonia surface stays in sync.
        SendMessageW(
            inputWindow,
            WmSetCursor,
            unchecked((nuint)inputWindow),
            MakeLParam(HtClient, (int)WmMouseMove));
    }

    private bool TryTranslateAccelerator(nint inputWindow, uint message, nuint wParam, nint lParam)
    {
        if (oleHost == 0 || rdpOleHostTranslateAccelerator is null)
            return false;

        NativePoint point = default;
        GetCursorPos(out point);
        NativeMessage nativeMessage = new()
        {
            Window = inputWindow,
            Message = message,
            WParam = wParam,
            LParam = lParam,
            Time = unchecked((uint)GetMessageTime()),
            Point = point
        };
        return rdpOleHostTranslateAccelerator(oleHost, ref nativeMessage) == 0;
    }

    private void SendMouseWheelMessage(uint message, int x, int y, int delta, NativeMouseButtons buttons)
    {
        nint inputWindow = GetInputWindow();
        if (inputWindow == 0)
            return;

        NativePoint point = new(x, y);
        ClientToScreen(inputWindow, ref point);
        nuint wParam = (nuint)((uint)(ushort)buttons | ((uint)(ushort)delta << 16));
        SendMessageW(inputWindow, message, wParam, MakeLParam(point.X, point.Y));
    }

    private nint GetInputWindow()
    {
        try
        {
            return instance?.GetInputWindow() ?? 0;
        }
        catch (COMException)
        {
            return 0;
        }
    }

    private void EnsureInputWindowSubclass(nint inputWindow)
    {
        if (inputWindow == 0 || inputWindowSubclassHandle == inputWindow)
            return;

        RemoveInputWindowSubclass();
        if (SetWindowSubclass(inputWindow, inputWindowSubclassProc, InputWindowSubclassId, UIntPtr.Zero))
            inputWindowSubclassHandle = inputWindow;
    }

    private nint InputWindowSubclassProc(
        nint window,
        uint message,
        nint wParam,
        nint lParam,
        UIntPtr subclassId,
        UIntPtr referenceData)
    {
        // The RDP input HWND captures on button-down. Avalonia then takes capture so
        // its visible bitmap surface keeps receiving pointer moves. That handoff must
        // not be interpreted by the hidden ActiveX control as a cancelled remote drag.
        if (message == WmCaptureChanged && remoteMouseDragActive && !disposed)
            return 0;

        if (message == WmNcDestroy && inputWindowSubclassHandle == window)
        {
            RemoveWindowSubclass(window, inputWindowSubclassProc, subclassId);
            inputWindowSubclassHandle = 0;
        }

        return DefSubclassProc(window, message, wParam, lParam);
    }

    private void RemoveInputWindowSubclass()
    {
        nint inputWindow = inputWindowSubclassHandle;
        if (inputWindow == 0)
            return;

        inputWindowSubclassHandle = 0;
        RemoveWindowSubclass(inputWindow, inputWindowSubclassProc, InputWindowSubclassId);
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

    private void DisposeNativeResources()
    {
        remoteMouseDragActive = false;
        RemoveInputWindowSubclass();

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

        instance?.Dispose();
        instance = null;

        if (oleHost != 0)
        {
            nint host = oleHost;
            oleHost = 0;
            rdpOleHostRelease?.Invoke(host);
        }

        client = null;
        IsConnectionActive = false;
        IsLoginCompleted = false;

        if (hostWindow != 0)
        {
            DestroyWindow(hostWindow);
            hostWindow = 0;
        }

        if (oleInitialized)
        {
            OleUninitialize();
            oleInitialized = false;
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

    private static nint MakeLParam(int x, int y)
    {
        return (nint)((uint)(ushort)x | ((uint)(ushort)y << 16));
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

    [Flags]
    internal enum NativeMouseButtons : uint
    {
        None = 0,
        Left = 0x0001,
        Right = 0x0002,
        Shift = 0x0004,
        Control = 0x0008,
        Middle = 0x0010,
        XButton1 = 0x0020,
        XButton2 = 0x0040
    }

    internal static class MouseMessages
    {
        public const uint LeftDown = WmLeftButtonDown;
        public const uint LeftUp = WmLeftButtonUp;
        public const uint RightDown = WmRightButtonDown;
        public const uint RightUp = WmRightButtonUp;
        public const uint MiddleDown = WmMiddleButtonDown;
        public const uint MiddleUp = WmMiddleButtonUp;
        public const uint XDown = WmXButtonDown;
        public const uint XUp = WmXButtonUp;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public nint Window;
        public uint Message;
        public nuint WParam;
        public nint LParam;
        public uint Time;
        public NativePoint Point;
        public uint Private;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int DllGetClassObject(ref Guid classId, ref Guid interfaceId, out nint classFactory);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostAttach(nint control, nint window, ref NativeRect bounds, out nint host);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostSetBounds(nint host, ref NativeRect bounds);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int RdpOleHostTranslateAccelerator(nint host, ref NativeMessage message);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void RdpOleHostRelease(nint host);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint SubclassProc(
        nint window,
        uint message,
        nint wParam,
        nint lParam,
        UIntPtr subclassId,
        UIntPtr referenceData);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern nint GetModuleHandleW(string? moduleName);

    [DllImport("ole32.dll", ExactSpelling = true)]
    private static extern int OleInitialize(nint reserved);

    [DllImport("ole32.dll", ExactSpelling = true)]
    private static extern void OleUninitialize();

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

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint SendMessageW(nint window, uint message, nuint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ClientToScreen(nint window, ref NativePoint point);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(nint window, out NativeRect bounds);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern uint MapVirtualKeyW(uint code, uint mapType);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int GetMessageTime();

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out NativePoint point);

    [DllImport("comctl32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern nint DefSubclassProc(nint window, uint message, nint wParam, nint lParam);

    [DllImport("comctl32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(
        nint window,
        SubclassProc subclassProc,
        UIntPtr subclassId,
        UIntPtr referenceData);

    [DllImport("comctl32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(nint window, SubclassProc subclassProc, UIntPtr subclassId);
}
