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
            int packageResult = VerifyPackageConsumer();
            return packageResult == SuccessExitCode ? VerifyComIntegration() : packageResult;
        }

        Console.Error.WriteLine("Usage: MsRdpEx_NativeAotInterop_Test [--com-integration]");
        Console.Error.WriteLine($"--com-integration returns {SkippedExitCode} when the Microsoft RDP Client Control version 11 is not registered.");
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

    private static int VerifyComIntegration()
    {
        Console.WriteLine("COM integration: activating Microsoft RDP Client Control version 11.");

        try
        {
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
            Console.WriteLine("COM integration: succeeded without starting a remote connection.");
            return SuccessExitCode;
        }
        catch (COMException exception) when (exception.HResult == ClassNotRegistered)
        {
            Console.Error.WriteLine($"COM integration: SKIPPED: Microsoft RDP Client Control version 11 is not registered (HRESULT 0x{exception.HResult:X8}).");
            Console.Error.WriteLine($"COM integration: returning {SkippedExitCode}; no remote connection was started.");
            return SkippedExitCode;
        }
    }
}
