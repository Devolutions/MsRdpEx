using System.Runtime.InteropServices;
using MsRdpEx.Interop;

BinaryString? value = "NativeAOT";
Console.WriteLine(typeof(MSTSCLib.IMsRdpClient).Assembly.FullName);
Console.WriteLine(typeof(IMsRdpClient).Assembly.FullName);
Console.WriteLine(value);

if (args.Contains("--com-integration", StringComparer.Ordinal))
{
    try
    {
        var client = MSTSCLib.RdpClientFactory.CreateClient10();
        using var events = MSTSCLib.RdpClientEvents.Subscribe(client);
        Console.WriteLine(client.Version);
    }
    catch (COMException exception) when ((uint)exception.HResult == 0x80040154)
    {
        Console.WriteLine("SKIPPED: Microsoft RDP Client Control version 11 is not registered.");
    }
}
