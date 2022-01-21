using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

using MsRdpEx;

namespace MsRdpEx.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start,"RdpProcess")]
    public class StartRdpProcessCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            RdpCoreApi coreApi = new RdpCoreApi();
            string rdpExDll = coreApi.MsRdpExDllPath;

            coreApi.Load();

            string axName = "mstsc";
            string appName = axName;
            string[] args = new string[0];
            RdpProcess rdpProcess = new RdpProcess(args, appName, axName);

            WriteObject(rdpProcess);
        }
    }
}
