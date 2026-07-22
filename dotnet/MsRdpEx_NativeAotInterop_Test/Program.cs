using System.Runtime.InteropServices;
using MsRdpEx.Interop;

BinaryString? value = "NativeAOT";
Console.WriteLine(typeof(MSTSCLib.IMsRdpClient).Assembly.FullName);
Console.WriteLine(typeof(IMsRdpClient).Assembly.FullName);
Console.WriteLine(value);

if (args.Contains("--com-integration", StringComparer.Ordinal))
{
    Activate("version 10", MSTSCLib.RdpClientFactory.CreateClient9);
    Activate("version 11", MSTSCLib.RdpClientFactory.CreateClient10);
    Activate("version 12", MSTSCLib.RdpClientFactory.CreateClient11);
}

static void Activate<T>(string version, Func<T> createClient) where T : MSTSCLib.IMsRdpClient
{
    try
    {
        var client = createClient();
        Console.WriteLine($"{version}: {client.Version}");
    }
    catch (COMException exception) when ((uint)exception.HResult == 0x80040154)
    {
        Console.WriteLine($"SKIPPED: Microsoft RDP Client Control {version} is not registered.");
    }
}
