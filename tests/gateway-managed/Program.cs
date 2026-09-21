using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MsRdpEx;

internal static class Program
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibraryW(string path);

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }

    private static void ExpectUnsupported(Action action)
    {
        try { action(); }
        catch (NotSupportedException) { return; }
        throw new InvalidOperationException("Older DLL did not report NotSupportedException");
    }

    private static int Main(string[] args)
    {
        try {
            Check(args.Length == 2, "Usage: GatewaySettingsTest <absolute-dll-path> <default|off|older>");
            Check(Path.IsPathRooted(args[0]), "Use an absolute native DLL path");
            bool older = args[1] == "older";
            Check(older || args[1] == "default" || args[1] == "off", "Unknown test mode");
            bool expected = args[1] != "off";
            Environment.SetEnvironmentVariable("MSRDPEX_GATEWAY_UNIQUE_BINDING", older || !expected ? "0" : null);
            Environment.SetEnvironmentVariable("MSRDPEX_GATEWAY_DIAGNOSTICS", null);
            Environment.SetEnvironmentVariable("MSRDPEX_LOG_ENABLED", "0");
            Environment.SetEnvironmentVariable("MSRDPEX_HOOK_ENABLED", "1");
            // Keep the DLL loaded until process exit while managed COM wrappers exist.
            Check(LoadLibraryW(args[0]) != IntPtr.Zero, "Native DLL load failed: " + Marshal.GetLastWin32Error());
            var first = new RdpCoreApi();
            var second = new RdpCoreApi();
            first.Load();
            Check(string.Equals(Path.GetFullPath(first.MsRdpExDllPath), Path.GetFullPath(args[0]),
                StringComparison.OrdinalIgnoreCase), "Loaded a different native DLL");
            if (older) {
                ExpectUnsupported(() => { bool ignored = first.GatewayIsolationEnabled; });
                ExpectUnsupported(() => first.GatewayIsolationEnabled = false);
                first.AxHookEnabled = false;
                first.AxHookEnabled = true;
            } else {
                Check(first.GatewayIsolationEnabled == expected, "Incorrect initial property value");
                first.GatewayIsolationEnabled = false;
                Check(!second.GatewayIsolationEnabled, "State not shared between core objects");
                Environment.SetEnvironmentVariable("MSRDPEX_GATEWAY_UNIQUE_BINDING", "1");
                first.AxHookEnabled = false; first.AxHookEnabled = true;
                Check(!second.GatewayIsolationEnabled, "Hook toggle reset the property");
                Task.Run(() => second.GatewayIsolationEnabled = true).GetAwaiter().GetResult();
                Check(first.GatewayIsolationEnabled, "State not shared across threads");
                first.GatewayIsolationEnabled = false;
                Check(!second.GatewayIsolationEnabled, "Final opt-out failed");
            }
            first.AxHookEnabled = false;
            first.Unload();
            Console.WriteLine("PASS managed gateway settings: " + args[1] + " (" + RuntimeInformation.ProcessArchitecture + ")");
            return 0;
        } catch (Exception error) {
            Console.Error.WriteLine(error);
            return 1;
        }
    }
}
