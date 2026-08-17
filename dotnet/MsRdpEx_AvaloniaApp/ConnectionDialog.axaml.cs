using Avalonia.Controls;
using Devolutions.MsRdpEx.Avalonia;
using Avalonia.Interactivity;

namespace MsRdpEx_AvaloniaApp;

public sealed partial class ConnectionDialog : Window
{
    private int customResolutionIndex = -1;
    private int customDesktopWidth;
    private int customDesktopHeight;
    private string rdpFileContents = string.Empty;

    public ConnectionDialog()
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            HostNameTextBox.Focus();
            HostNameTextBox.SelectAll();
        };
    }

    internal ConnectionDialog(RdpLaunchOptions options) : this()
    {
        if (!string.IsNullOrWhiteSpace(options.HostName))
            HostNameTextBox.Text = options.HostName;

        UserNameTextBox.Text = options.UserName;
        PasswordTextBox.Text = options.Password;
        DomainTextBox.Text = options.Domain;
        rdpFileContents = options.RdpFileContents;
        ValidationTextBlock.Text = options.Error ?? string.Empty;

        int resolutionIndex = (options.DesktopWidth, options.DesktopHeight) switch
        {
            (1024, 768) => 1,
            (1280, 720) => 2,
            (1600, 900) => 3,
            (1920, 1080) => 4,
            _ => 0
        };

        if (resolutionIndex == 0 && options.DesktopWidth > 0 && options.DesktopHeight > 0)
        {
            customDesktopWidth = options.DesktopWidth;
            customDesktopHeight = options.DesktopHeight;
            ResolutionComboBox.Items.Add(new ComboBoxItem
            {
                Content = $"{customDesktopWidth} × {customDesktopHeight} (from RDP file)"
            });
            customResolutionIndex = ResolutionComboBox.ItemCount - 1;
            resolutionIndex = customResolutionIndex;
        }

        ResolutionComboBox.SelectedIndex = resolutionIndex;
    }

    public event Action<RdpConnectionSettings>? ConnectionRequested;

    private void OnConnectClicked(object? sender, RoutedEventArgs e)
    {
        string hostName = HostNameTextBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(hostName))
        {
            ValidationTextBlock.Text = "Enter the name of the remote computer.";
            HostNameTextBox.Focus();
            return;
        }

        string password = PasswordTextBox.Text ?? string.Empty;
        (int desktopWidth, int desktopHeight) = ResolutionComboBox.SelectedIndex switch
        {
            1 => (1024, 768),
            2 => (1280, 720),
            3 => (1600, 900),
            4 => (1920, 1080),
            var index when index == customResolutionIndex => (customDesktopWidth, customDesktopHeight),
            _ => (0, 0)
        };

        RdpConnectionSettings settings = new(
            hostName,
            UserNameTextBox.Text?.Trim() ?? string.Empty,
            password,
            DomainTextBox.Text?.Trim() ?? string.Empty,
            desktopWidth,
            desktopHeight,
            rdpFileContents);

        // Do not retain the visible password after handing it to the session window.
        PasswordTextBox.Text = string.Empty;
        password = string.Empty;
        ValidationTextBlock.Text = string.Empty;
        ConnectButton.IsEnabled = false;
        ConnectionRequested?.Invoke(settings);
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        PasswordTextBox.Text = string.Empty;
        Close();
    }
}
