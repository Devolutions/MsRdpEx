using System.Reflection;
using System.Runtime.InteropServices;
using MsRdpEx.Interop;

internal static class Program
{
    private const int SuccessExitCode = 0;
    private const int SkippedExitCode = 77;
    private const int UsageErrorExitCode = 64;
    private const int ClassNotRegistered = unchecked((int)0x80040154);
    private const string GeneratedInteropAssemblyName = "Interop.MSTSCLib.Generated";

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length == 0)
            return VerifyPackageConsumer();

        if (args is ["--com-integration"])
        {
#if MSRDPEX_MODERN_COM_RUNTIME
            int packageResult = VerifyPackageConsumer();
            return packageResult == SuccessExitCode ? VerifyComIntegration() : packageResult;
#else
            Console.Error.WriteLine("--com-integration requires a package under test that includes the modern COM runtime APIs.");
            return UsageErrorExitCode;
#endif
        }

        Console.Error.WriteLine("Usage: MsRdpEx_NativeAotInterop_Test [--com-integration]");
        Console.Error.WriteLine($"--com-integration returns {SkippedExitCode} when a required Microsoft RDP Client Control is not registered.");
        return UsageErrorExitCode;
    }

    private static int VerifyPackageConsumer()
    {
        BinaryString? value = "NativeAOT";
        AssemblyName generatedInteropAssembly = typeof(MSTSCLib.IMsRdpClient).Assembly.GetName();
        AssemblyName generatedComAssembly = typeof(IMsRdpClient).Assembly.GetName();

        if (generatedInteropAssembly.Name != GeneratedInteropAssemblyName ||
            generatedComAssembly.Name != GeneratedInteropAssemblyName)
        {
            throw new InvalidOperationException(
                $"Package consumer: expected {GeneratedInteropAssemblyName}, but resolved {generatedInteropAssembly.Name} and {generatedComAssembly.Name}.");
        }

        Console.WriteLine($"Package consumer: compatibility interop = {generatedInteropAssembly.FullName}");
        Console.WriteLine($"Package consumer: generated COM interop = {generatedComAssembly.FullName}");
        Console.WriteLine($"Package consumer: binary string = {value}");
        Console.WriteLine("Package consumer: succeeded.");
        return SuccessExitCode;
    }

#if MSRDPEX_MODERN_COM_RUNTIME
    private static int VerifyComIntegration()
    {
        Console.WriteLine("COM integration: activating Microsoft RDP Client Control version 11.");

        try
        {
            VerifyActivation("version 10", MSTSCLib.RdpClientFactory.CreateClient9);
            var client = MSTSCLib.RdpClientFactory.CreateClient10();
            Console.WriteLine("COM integration: activation succeeded.");

            object proxy = client;
            if (proxy is not MSTSCLib.ProxyObject)
                throw new InvalidOperationException("COM integration: activation did not return a generated COM proxy.");

            Console.WriteLine("COM integration: generated COM proxy creation succeeded.");
            Console.WriteLine($"COM integration: proxy invocation succeeded (Version={client.Version}).");

            using (MSTSCLib.RdpClientEvents.Subscribe(client))
            {
                Console.WriteLine("COM integration: event subscription (Advise) succeeded.");
            }

            Console.WriteLine("COM integration: event subscription disposal (Unadvise) succeeded.");
            VerifyActivation("version 12", MSTSCLib.RdpClientFactory.CreateClient11);
            Console.WriteLine("COM integration: succeeded without starting a remote connection.");
            return SuccessExitCode;
        }
        catch (COMException exception) when (exception.HResult == ClassNotRegistered)
        {
            Console.Error.WriteLine($"COM integration: SKIPPED: a required Microsoft RDP Client Control is not registered (HRESULT 0x{exception.HResult:X8}).");
            Console.Error.WriteLine($"COM integration: returning {SkippedExitCode}; no remote connection was started.");
            return SkippedExitCode;
        }
    }

    private static void VerifyActivation<T>(string version, Func<T> createClient) where T : MSTSCLib.IMsRdpClient
    {
        var client = createClient();
        Console.WriteLine($"COM integration: {version} activation and proxy invocation succeeded (Version={client.Version}).");
    }
#endif
}
