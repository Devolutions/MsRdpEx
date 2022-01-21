using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using MSTSCLib;

using MsRdpEx;

namespace MsRdpEx.PowerShell
{
    public partial class RdpMain : Form
    {
        public RdpCoreApi coreApi;
        public RdpInstance rdpInstance;
        public RdpView rdpView;
        public List<RdpView> rdpViews = new List<RdpView>();

        public RdpMain()
        {
            InitializeComponent();

            this.coreApi = new RdpCoreApi();
            coreApi.Load();
        }

        public void Connect(string hostname, string username, string password)
        {
            if (this.InvokeRequired) {
                Action safe = delegate { Connect(hostname, username, password); };
                this.Invoke(safe);
                return;
            }

            RdpContext context = RdpContext.Instance();
            string rdpExDll = coreApi.MsRdpExDllPath;

            string axName = "mstsc";
            string appName = axName;

            RdpView rdpView = new RdpView(axName, rdpExDll);
            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = rdpView.rdpClient;

            this.rdpInstance = new RdpInstance((IMsRdpExInstance)rdp.GetOcx());
            rdpInstance.OutputMirrorEnabled = true;
            rdpInstance.VideoRecordingEnabled = true;

            rdp.Server = hostname;
            rdp.UserName = username;
            rdp.AdvancedSettings9.EnableCredSspSupport = true;
            IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp.GetOcx();
            secured.ClearTextPassword = password;
            IMsRdpExtendedSettings extendedSettings = (IMsRdpExtendedSettings)rdp.GetOcx();
            object boolValue = false;
            extendedSettings.set_Property("EnableHardwareMode", ref boolValue);
            Size DesktopSize = new Size(1024, 768);
            rdp.DesktopWidth = DesktopSize.Width;
            rdp.DesktopHeight = DesktopSize.Height;
            rdpView.ClientSize = DesktopSize;
            rdpView.Text = String.Format("{0} ({1})", rdp.Server, axName);

            rdp.Connect();
            rdpView.Show();

            this.rdpViews.Add(rdpView);
        }

        protected override void OnLoad(EventArgs e)
        {
            //Visible = false; 
            //ShowInTaskbar = false; 
            //WindowState = FormWindowState.Minimized;
            base.OnLoad(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Name = "RdpMain";
            this.Text = "Remote Desktop Client";
            this.ResumeLayout(false);
            this.Hide();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
