using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

using MSTSCLib;

using MsRdpEx;

namespace MsRdpEx.PowerShell
{
    public class RdpContext
    {
        static RdpContext instance = null;
        static Thread thread = null;

        static RdpMain rdpMain = null;

        public static RdpContext Instance() {
            if (instance == null) {
                instance = new RdpContext();
            }
            return instance;
        }

        public RdpContext()
        {
            RdpContext.StartThread();
            System.Threading.Thread.Sleep(250);
        }

        public static Thread StartThread()
        {
            thread = new Thread(new ThreadStart(MainThread));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return thread;
        }

        static void MainThread()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            rdpMain = new RdpMain();
            Application.Run(rdpMain);
        }

        public void Connect(string hostname, string username, string password)
        {
            rdpMain.Connect(hostname, username, password);
        }
    }
}
