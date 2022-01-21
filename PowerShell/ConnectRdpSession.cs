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
    [Cmdlet(VerbsCommunications.Connect,"RdpSession")]

    public class ConnectRdpSessionCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Hostname;

        [Parameter(Position = 1, Mandatory = true)]
        public string Username;

        [Parameter(Position = 2, Mandatory = true)]
        public string Password;

        protected override void ProcessRecord()
        {
            RdpContext rdpContext = RdpContext.Instance();
            rdpContext.Connect(Hostname, Username, Password);
        }
    }
}
