using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using MSTSCLib;

using MsRdpEx;

namespace MsRdpEx_App
{
    public partial class MainDlg : Form
    {
        public MainDlg()
        {
            InitializeComponent();
            this.cboRdpClient.SelectedIndex = 1;
            this.cboLaunchMode.SelectedIndex = 0;
            LoadEnvironment();
        }

        private void LoadEnvironment()
        {
            string rdpHostname = Environment.GetEnvironmentVariable("RDP_HOSTNAME");
            string rdpUsername = Environment.GetEnvironmentVariable("RDP_USERNAME");
            string rdpPassword = Environment.GetEnvironmentVariable("RDP_PASSWORD");

            if (rdpHostname != null)
            {
                this.txtComputer.Text = rdpHostname;
            }

            if (rdpUsername != null)
            {
                this.txtUserName.Text = rdpUsername;
            }

            if (rdpPassword != null)
            {
                this.txtPassword.Text = rdpPassword;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string axName = this.cboRdpClient.Text;
            bool externalMode = this.cboLaunchMode.SelectedIndex == 1;

            if (externalMode)
            {
                string appName = axName;
                string[] args = new string[0];
                Bindings.StartProcess(appName, args);
                return;
            }

            var process = Process.GetCurrentProcess();
            string processFileName = process.MainModule.FileName;
            string processFileDir = Path.GetDirectoryName(processFileName);
            string axNameEx = Path.Combine(processFileDir, "MsRdpEx.dll");

            if (File.Exists(axNameEx))
            {
                axName = axNameEx;
            }

            IMsRdpExCoreApi coreApi = Bindings.GetCoreApi();
            coreApi.Load();

            RdpView rdpView = new RdpView(axName);
            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = rdpView.rdpClient;

            rdp.Server = this.txtComputer.Text;
            rdp.UserName = this.txtUserName.Text;
            rdp.AdvancedSettings9.EnableCredSspSupport = true;
            IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp.GetOcx();
            secured.ClearTextPassword = this.txtPassword.Text;
            IMsRdpExtendedSettings extendedSettings = (IMsRdpExtendedSettings)rdp.GetOcx();
            object boolValue = false;
            extendedSettings.set_Property("EnableHardwareMode", ref boolValue);
            Size DesktopSize = new Size(1024, 768);
            rdp.DesktopWidth = DesktopSize.Width;
            rdp.DesktopHeight = DesktopSize.Height;
            rdpView.ClientSize = DesktopSize;
            rdpView.Text = String.Format("{0} ({1})", rdp.Server, axName);
            rdp.Connect();

            object corePropsVal = extendedSettings.get_Property("CoreProperties");
            IMsRdpExtendedSettings coreProps = (IMsRdpExtendedSettings)corePropsVal;

            object basePropsVal = extendedSettings.get_Property("BaseProperties");
            IMsRdpExtendedSettings baseProps = (IMsRdpExtendedSettings)basePropsVal;

            //object transportPropsVal = extendedSettings.get_Property("TransportProperties");
            //IMsRdpExtendedSettings transportProps = (IMsRdpExtendedSettings)transportPropsVal;

            object strVal = "MySmartCardReader";
            coreProps.set_Property("SmartCardReaderName", ref strVal);
            string readerName = (string) coreProps.get_Property("SmartCardReaderName");

            rdpView.Show();
        }
    }
}
