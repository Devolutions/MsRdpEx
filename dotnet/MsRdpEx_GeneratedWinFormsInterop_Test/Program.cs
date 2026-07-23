using System.Runtime.InteropServices;
using System.ComponentModel;
using MsRdpEx;
using MsRdpEx.WinForms;
using MSTSCLib;

internal static class Program
{
    private const int SuccessExitCode = 0;
    private const int SkippedExitCode = 77;
    private const int UsageErrorExitCode = 64;
    private const int ClassNotRegistered = unchecked((int)0x80040154);
    private static readonly Guid Client10ClassId = new("A0C63C30-F08D-4AB4-907C-34905D770C7D");

    [STAThread]
    private static int Main(string[] args)
    {
        VerifyAliases();

        if (args.Length == 0)
            return SuccessExitCode;

        if (args is ["--com-integration"])
            return VerifyHostActivation();

        Console.Error.WriteLine("Usage: MsRdpEx_GeneratedWinFormsInterop_Test [--com-integration]");
        return UsageErrorExitCode;
    }

    private static void VerifyAliases()
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

        Console.WriteLine(typeof(GeneratedRdpClientHost).Assembly.FullName);
        Console.WriteLine(typeof(RdpClientConfirmCloseEventArgs).Assembly.FullName);
        Console.WriteLine("Generated WinForms legacy aliases succeeded.");
    }

    private static int VerifyHostActivation()
    {
        try
        {
            Application.OleRequired();
            VerifyHostActivation(new GeneratedRdpClientHostOptions { ClassId = Client10ClassId }, "system control");
            VerifyHostActivation(new GeneratedRdpClientHostOptions
            {
                ClassId = Client10ClassId,
                AxName = "mstsc",
                RdpExDll = Path.Combine(AppContext.BaseDirectory, "MsRdpEx.dll"),
            }, "MsRdpEx control");
            Console.WriteLine("No remote connection was started.");
            return SuccessExitCode;
        }
        catch (COMException exception) when (exception.HResult == ClassNotRegistered)
        {
            Console.Error.WriteLine($"Generated WinForms host integration skipped: RDP control is not registered (HRESULT 0x{exception.HResult:X8}).");
            return SkippedExitCode;
        }
    }

    private static void VerifyHostActivation(GeneratedRdpClientHostOptions options, string name)
    {
        using var form = new Form();
        using var host = new GeneratedRdpClientHost(options) { Dock = DockStyle.Fill };
        var initialization = (ISupportInitialize)host;
        initialization.BeginInit();
        form.Controls.Add(host);
        initialization.EndInit();
        form.CreateControl();
        host.CreateControl();

        IMsRdpClient10 client = host.GetClient<IMsRdpClient10>();
        Console.WriteLine($"Generated WinForms {name} activation succeeded (Server={client.Server}).");
        if (!string.IsNullOrEmpty(options.RdpExDll))
            Console.WriteLine($"Generated WinForms MsRdpEx instance succeeded (SessionId={RdpInstance.FromOcx(host.GetOcx()).SessionId}).");
    }
}
