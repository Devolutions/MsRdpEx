using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Devolutions.MsRdpEx.Avalonia;

namespace MsRdpEx_AvaloniaApp;

public sealed partial class MainWindow : Window
{
    // Matches the mstsc Zoom submenu.
    private static readonly int[] ZoomLevels = [25, 50, 75, 100, 125, 150, 200, 300, 400];

    private RdpConnectionSettings? pendingSettings;
    private string hostName = "RDP";
    private bool syncingDisplayMode;

    public MainWindow()
    {
        InitializeComponent();
    }

    internal MainWindow(RdpConnectionSettings settings, RdpLaunchOptions? launchOptions = null) : this()
    {
        ArgumentNullException.ThrowIfNull(settings);

        pendingSettings = settings;
        hostName = settings.HostName;
        if (launchOptions is not null)
        {
            RdpHost.ClassId = launchOptions.ClassId;
            RdpHost.AxName = launchOptions.AxName;
            RdpHost.RdpExDll = launchOptions.RdpExDll;
        }
        Title = $"{settings.HostName} - MsRdpEx Avalonia RDP";
        TitleText.Text = Title;

        BuildSessionMenu();

        RdpHost.ClientReady += OnClientReady;
        RdpHost.StatusChanged += OnStatusChanged;
        RdpHost.Disconnected += OnDisconnected;
        RdpHost.DisplayModeChanged += OnDisplayModeChanged;
        RdpHost.EnteredFullScreen += OnFullScreenChanged;
        RdpHost.LeftFullScreen += OnFullScreenChanged;
        PropertyChanged += OnWindowPropertyChanged;
        Closing += OnWindowClosing;
    }

    /// <summary>
    /// Builds the mstsc-style session context menu: window controls, display
    /// modes (Full screen, Smart sizing, Zoom submenu), and Close.
    /// </summary>
    private void BuildSessionMenu()
    {
        MenuItem minimizeItem = new() { Header = "Mi_nimize", InputGesture = null };
        minimizeItem.Click += (_, _) => WindowState = WindowState.Minimized;

        MenuItem maximizeItem = new() { Header = "Ma_ximize" };
        maximizeItem.Click += (_, _) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        MenuItem fullScreenItem = new()
        {
            Header = "_Full screen",
            ToggleType = MenuItemToggleType.CheckBox,
            InputGesture = new KeyGesture(Key.Pause, KeyModifiers.Control | KeyModifiers.Alt)
        };
        fullScreenItem.Click += (_, _) =>
        {
            RdpHost.FullScreen = !RdpHost.FullScreen;
            TitleBar.IsVisible = !RdpHost.FullScreen;
        };

        MenuItem smartSizingItem = new() { Header = "_Smart sizing", ToggleType = MenuItemToggleType.CheckBox };
        smartSizingItem.Click += OnSmartSizingClicked;

        MenuItem zoomItem = new() { Header = "_Zoom" };
        foreach (int level in ZoomLevels)
        {
            MenuItem levelItem = new()
            {
                Header = $"{level}%",
                ToggleType = MenuItemToggleType.Radio,
                Tag = level
            };
            levelItem.Click += OnZoomLevelClicked;
            zoomItem.Items.Add(levelItem);
        }

        MenuItem closeItem = new() { Header = "_Close", InputGesture = new KeyGesture(Key.F4, KeyModifiers.Alt) };
        closeItem.Click += (_, _) => Close();

        SessionMenu.Items.Add(minimizeItem);
        SessionMenu.Items.Add(maximizeItem);
        SessionMenu.Items.Add(new Separator());
        SessionMenu.Items.Add(fullScreenItem);
        SessionMenu.Items.Add(smartSizingItem);
        SessionMenu.Items.Add(zoomItem);
        SessionMenu.Items.Add(new Separator());
        SessionMenu.Items.Add(closeItem);

        SessionMenu.Opening += (_, _) => SyncSessionMenu();
        SyncSessionMenu();
    }

    // Reflects the current window/display state into the menu items every time
    // the menu opens (fullscreen may also change via Ctrl+Alt+Break or the
    // connection bar).
    private void SyncSessionMenu()
    {
        syncingDisplayMode = true;
        try
        {
            SetChecked("_Full screen", RdpHost.FullScreen);
            SetChecked("_Smart sizing", RdpHost.DisplayMode == RdpDisplayMode.SmartSizing);

            foreach (MenuItem levelItem in ZoomItems())
            {
                bool isHundred = levelItem.Tag is int tag && tag == 100;
                levelItem.IsChecked = levelItem.Tag is int level && level == RdpHost.ZoomLevel &&
                    (RdpHost.DisplayMode == RdpDisplayMode.Zoom ||
                     (isHundred && RdpHost.DisplayMode == RdpDisplayMode.FitToWindow));
            }
        }
        finally
        {
            syncingDisplayMode = false;
        }
    }

    private void OnSmartSizingClicked(object? sender, RoutedEventArgs e)
    {
        if (syncingDisplayMode)
            return;

        RdpHost.DisplayMode = RdpHost.DisplayMode == RdpDisplayMode.SmartSizing
            ? RdpDisplayMode.FitToWindow
            : RdpDisplayMode.SmartSizing;
    }

    private void OnZoomLevelClicked(object? sender, RoutedEventArgs e)
    {
        if (syncingDisplayMode || sender is not MenuItem { Tag: int level })
            return;

        // 100% is presented as a Zoom entry but means "fit to window".
        RdpHost.ZoomLevel = level;
    }

    private void OnDisplayModeChanged(object? sender, EventArgs e)
    {
        if (SessionMenu.IsOpen)
            SyncSessionMenu();
    }

    private void SetChecked(string header, bool isChecked)
    {
        foreach (object? entry in SessionMenu.Items)
        {
            if (entry is MenuItem item && item.Header as string == header)
                item.IsChecked = isChecked;
        }
    }

    private IEnumerable<MenuItem> ZoomItems()
    {
        foreach (object? entry in SessionMenu.Items)
        {
            if (entry is MenuItem { Header: "_Zoom" } zoomItem)
            {
                foreach (object? level in zoomItem.Items)
                {
                    if (level is MenuItem levelItem)
                        yield return levelItem;
                }
            }
        }
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        // The session is gone (sign-out, network drop, or the fullscreen
        // connection bar's Close button); close the window like mstsc does.
        pendingSettings = null;
        Close();
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && !RdpHost.FullScreen)
            BeginMoveDrag(e);
    }

    private void OnMinimizeClicked(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnMaximizeRestoreClicked(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnFullScreenChanged(object? sender, EventArgs e)
    {
        UpdateTitleBarVisibility();
    }

    private void OnWindowPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == WindowStateProperty)
            UpdateTitleBarVisibility();
    }

    private void UpdateTitleBarVisibility()
    {
        // A custom title bar would otherwise consume a strip of the monitor
        // above the native RDP HWND. The floating mstsc connection bar remains
        // the only chrome in fullscreen. Auto-sized grid row collapses when
        // the bar is hidden.
        TitleBar.IsVisible = WindowState != WindowState.FullScreen && !RdpHost.FullScreen;
    }

    private void OnStatusChanged(object? sender, RdpStatusChangedEventArgs e)
    {
        Title = $"{hostName} - {e.Message}";
        TitleText.Text = Title;
    }

    private void OnClientReady(object? sender, EventArgs e)
    {
        if (pendingSettings is null)
            return;

        RdpConnectionSettings settings = pendingSettings;
        pendingSettings = null;

        try
        {
            RdpHost.Connect(settings);
        }
        catch (Exception exception)
        {
            Title = $"{settings.HostName} - Connection failed: {exception.Message}";
        }
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        pendingSettings = null;

        try
        {
            RdpHost.Dispose();
        }
        catch
        {
            // The native session engine may already be in its teardown path.
        }
    }
}
