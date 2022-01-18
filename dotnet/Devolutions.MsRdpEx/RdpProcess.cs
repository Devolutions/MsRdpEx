using System;

namespace MsRdpEx
{
    public class RdpProcess
    {
        public IMsRdpExProcess iface;
        
        public RdpProcess(string appName, string[] args) {
            iface = Bindings.StartProcess(appName, args);
        }

        public void Stop(UInt32 exitCode)
        {
            iface.Stop(exitCode);
        }

        public void Wait(UInt32 milliseconds)
        {
            iface.Wait(milliseconds);
        }

        public void GetExitCode(out UInt32 exitCode)
        {
            iface.GetExitCode(out exitCode);
        }
    }
}
