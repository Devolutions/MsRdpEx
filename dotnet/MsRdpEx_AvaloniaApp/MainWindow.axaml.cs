using Avalonia.Controls;
using Devolutions.MsRdpEx.Avalonia;

namespace MsRdpEx_AvaloniaApp;

public sealed partial class MainWindow : Window
{
    private RdpConnectionSettings? pendingSettings;

    public MainWindow()
    {
        InitializeComponent();
    }

    internal MainWindow(RdpConnectionSettings settings, RdpLaunchOptions? launchOptions = null) : this()
    {
        ArgumentNullException.ThrowIfNull(settings);

        pendingSettings = settings;
        if (launchOptions is not null)
        {
            RdpHost.ClassId = launchOptions.ClassId;
            RdpHost.AxName = launchOptions.AxName;
            RdpHost.RdpExDll = launchOptions.RdpExDll;
        }
        Title = $"{settings.HostName} - MsRdpEx Avalonia RDP";

        RdpHost.ClientReady += OnClientReady;
        Closing += OnWindowClosing;
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
