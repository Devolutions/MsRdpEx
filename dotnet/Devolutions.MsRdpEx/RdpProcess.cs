using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MsRdpEx
{
    public class RdpProcess
    {
        public IMsRdpExProcess iface;
        
        public RdpProcess(string[] args, string appName, string axName) {
            iface = Bindings.StartProcess(args, appName, axName);
        }

        public RdpProcess(ProcessStartInfo startInfo)
        {
            iface = Bindings.CreateProcess();

            this.FileName = startInfo.FileName;
            this.WorkingDirectory = startInfo.WorkingDirectory;
            this.SetArguments(startInfo.Arguments);
            SetEnvironmentBlock(startInfo.Environment);
        }

        public void SetArguments(string arguments)
        {
            iface.SetArguments(arguments);
        }

        public void SetArgumentVector(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string arg in args)
            {
                sb.AppendFormat("{0}\0", arg);
            }
            sb.Append("\0");

            string argumentBlock = sb.ToString();
            iface.SetArgumentBlock(argumentBlock);
        }

        private void SetEnvironmentBlock(IDictionary<string, string?> environment)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string?> envvar in environment)
            {
                sb.AppendFormat("{0}={1}\0", envvar.Key, envvar.Value);
            }
            sb.Append("\0");

            string environmentBlock = sb.ToString();
            iface.SetEnvironmentBlock(environmentBlock);
        }

        public string FileName
        {
            set { iface.SetFileName(value); }
        }

        public string WorkingDirectory
        {
            set { iface.SetWorkingDirectory(value); }
        }

        public static Process StartProcess(ProcessStartInfo startInfo)
        {
            RdpProcess rdpProcess = new RdpProcess(startInfo);
            return rdpProcess.Start();
        }

        public Process Start()
        {
            iface.StartWithInfo();
            uint processId = this.GetProcessId();
            return Process.GetProcessById((int)processId);
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
