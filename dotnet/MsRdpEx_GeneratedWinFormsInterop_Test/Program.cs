using AxMSTSCLib;
using MSTSCLib;
using GeneratedWinFormsHost = MsRdpEx.WinForms.GeneratedRdpClientHost;
using GeneratedWinFormsHostOptions = MsRdpEx.WinForms.GeneratedRdpClientHostOptions;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            VerifyLegacyAliases();
            return 0;
        }

        if (args is ["--host-activation"])
        {
            VerifyHostActivation();
            return 0;
        }

        if (args is ["--host-activation-rdpclient10-first"])
        {
            VerifyRdpClient10FirstHostActivation();
            return 0;
        }

        Console.Error.WriteLine("Usage: MsRdpEx_GeneratedWinFormsInterop_Test [--host-activation|--host-activation-rdpclient10-first]");
        return 64;
    }

    private static void VerifyLegacyAliases()
    {
        var confirmClose = new RdpClientConfirmCloseEventArgs();
        confirmClose.pfAllowClose = false;
        if (confirmClose.AllowClose)
            throw new InvalidOperationException("pfAllowClose did not update AllowClose.");

        var publicKey = new RdpClientPublicKeyEventArgs(null);
        publicKey.pfContinueLogon = false;
        if (publicKey.ContinueLogon)
            throw new InvalidOperationException("pfContinueLogon did not update ContinueLogon.");

        var reconnect = new RdpClientLegacyAutoReconnectingEventArgs(42, 3);
        reconnect.pArcContinueStatus = AutoReconnectContinueState.autoReconnectContinueStop;
        if (reconnect.ContinueStatus != AutoReconnectContinueState.autoReconnectContinueStop)
            throw new InvalidOperationException("pArcContinueStatus did not update ContinueStatus.");

        if (typeof(RdpClientConfirmCloseEventArgs).Assembly.GetName().Name != "Interop.MSTSCLib.Generated")
            throw new InvalidOperationException("Generated WinForms resolved duplicate event-argument types.");

        Console.WriteLine(typeof(AxMsRdpClient9NotSafeForScripting).Assembly.FullName);
        Console.WriteLine(typeof(RdpClientConfirmCloseEventArgs).Assembly.FullName);
        Console.WriteLine("Generated WinForms legacy aliases succeeded.");
    }

    private static void VerifyHostActivation()
    {
        string rdpExDll = Path.Combine(AppContext.BaseDirectory, "MsRdpEx.dll");
        if (!File.Exists(rdpExDll))
            throw new FileNotFoundException("The package's win-x64 MsRdpEx.dll was not copied to the test output.", rdpExDll);

        using var form = new Form();

        using var generatedHost = new GeneratedWinFormsHost(new GeneratedWinFormsHostOptions
        {
            ClassId = new Guid("A0C63C30-F08D-4AB4-907C-34905D770C7D"),
            AxName = "mstsc",
            RdpExDll = rdpExDll,
        });
        form.Controls.Add(generatedHost);
        generatedHost.CreateControl();
        Console.WriteLine("Generated host activation succeeded.");
        form.Controls.Remove(generatedHost);

        VerifyRdpClient10HostActivation(form, rdpExDll);
        Console.WriteLine("Host activation succeeded without starting a remote connection.");
    }

    private static void VerifyRdpClient10FirstHostActivation()
    {
        string rdpExDll = GetRdpExDllPath();
        var client = RdpClientFactory.CreateClient10();
        Console.WriteLine($"Direct RdpClientFactory activation succeeded (Version={client.Version}).");

        using var form = new Form();
        VerifyRdpClient10HostActivation(form, rdpExDll);
        Console.WriteLine("RdpClient10-first host activation succeeded without starting a remote connection.");
    }

    private static string GetRdpExDllPath()
    {
        string rdpExDll = Path.Combine(AppContext.BaseDirectory, "MsRdpEx.dll");
        if (!File.Exists(rdpExDll))
            throw new FileNotFoundException("The package's win-x64 MsRdpEx.dll was not copied to the test output.", rdpExDll);

        return rdpExDll;
    }

    private static void VerifyRdpClient10HostActivation(Form form, string rdpExDll)
    {
        using var rdpClient10 = new AxMsRdpClient10
        {
            axName = "mstsc",
            rdpExDll = rdpExDll,
        };
        var initializableRdpClient10 = (System.ComponentModel.ISupportInitialize)rdpClient10;
        initializableRdpClient10.BeginInit();
        form.Controls.Add(rdpClient10);
        form.CreateControl();
        initializableRdpClient10.EndInit();
        rdpClient10.CreateControl();
        Console.WriteLine($"AxMsRdpClient10 activation succeeded (Version={rdpClient10.Version}).");
    }
}
