using System;
using System.IO;
using System.Drawing;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

using MSTSCLib;

using MsRdpEx;

namespace MsRdpEx.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start,"RdpClient")]
    public class StartRdpClientCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            RdpContext rdpContext = RdpContext.Instance();
        }
    }
}
