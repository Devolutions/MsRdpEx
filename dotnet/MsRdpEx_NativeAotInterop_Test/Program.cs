using MsRdpEx.Interop;

BinaryString? value = "NativeAOT";
Console.WriteLine(typeof(MSTSCLib.IMsRdpClient).Assembly.FullName);
Console.WriteLine(typeof(IMsRdpClient).Assembly.FullName);
Console.WriteLine(value);
