using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace MsRdpEx_AvaloniaApp;

public sealed class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            RdpLaunchOptions launchOptions = RdpLaunchOptions.Parse(desktop.Args ?? []);
            if (launchOptions.CanAutoConnect)
            {
                desktop.MainWindow = new MainWindow(
                    launchOptions.CreateConnectionSettings(),
                    launchOptions);
            }
            else
            {
                ConnectionDialog connectionDialog = new(launchOptions);
                connectionDialog.ConnectionRequested += settings =>
                {
                    MainWindow sessionWindow = new(settings, launchOptions);
                    desktop.MainWindow = sessionWindow;
                    sessionWindow.Show();
                    connectionDialog.Close();
                };
                desktop.MainWindow = connectionDialog;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
