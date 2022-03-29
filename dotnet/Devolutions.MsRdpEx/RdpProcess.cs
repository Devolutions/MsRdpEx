using System;

namespace MsRdpEx
{
    public class RdpProcess
    {
        public IMsRdpExProcess iface;
        
        public RdpProcess(string[] args, string appName, string axName) {
            iface = Bindings.StartProcess(args, appName, axName);
        }

        public string FileName
        {
            set { iface.SetFileName(value); }
        }

        public string WorkingDirectory
        {
            set { iface.SetWorkingDirectory(value); }
        }

        public void Stop(UInt32 exitCode)
        {
            iface.Stop(exitCode);
        }

        public void Wait(UInt32 milliseconds)
        {
            iface.Wait(milliseconds);
        }

        public uint GetProcessId()
        {
            return iface.GetProcessId();
        }

        public uint GetExitCode()
        {
            return iface.GetExitCode();
        }
    }
}
