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
        private string mstscExecutable = null;
        private string mstscAxLibrary = null;

        private string msrdcExecutable = null;
        private string msrdcAxLibrary = null;

        private string rdpFileName = null;

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

            rdpFileName = Environment.GetEnvironmentVariable("RDP_FILENAME");

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

            string mstscax = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\mstscax.dll");
            string rdclientax_global = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Remote Desktop\\rdclientax.dll");
            string rdclientax_local = Environment.ExpandEnvironmentVariables("%LocalAppData%\\Apps\\Remote Desktop\\rdclientax.dll");

            this.mstscAxLibrary = mstscax;
            this.mstscExecutable = Path.Combine(Path.GetDirectoryName(mstscAxLibrary), "mstsc.exe");

            if (File.Exists(rdclientax_global))
            {
                this.msrdcAxLibrary = rdclientax_global;
            }
            else if (File.Exists(rdclientax_local))
            {
                this.msrdcAxLibrary = rdclientax_local;
            }

            if (!string.IsNullOrEmpty(msrdcAxLibrary))
            {
                this.msrdcExecutable = Path.Combine(Path.GetDirectoryName(msrdcAxLibrary), "msrdc.exe");
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string axName = this.cboRdpClient.Text;
            bool externalMode = this.cboLaunchMode.SelectedIndex == 1;

            MsRdpExManager manager = MsRdpExManager.Instance;
            RdpCoreApi coreApi = manager.CoreApi;
            bool axHookEnabled = manager.AxHookEnabled;
            string rdpExDll = coreApi.MsRdpExDllPath;

            if (externalMode)
            {
                string filename = this.mstscExecutable;

                if (axName.Equals("msrdc"))
                {
                    filename = this.msrdcExecutable;
                }

                string workingDirectory = Path.GetDirectoryName(filename);

                List<string> args = new List<string>();
                args.Add(filename);

                if (this.rdpFileName != null)
                {
                    args.Add(this.rdpFileName);
                }

                string arguments = string.Join(' ', args.ToArray());

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = filename;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = workingDirectory;
                startInfo.Arguments = arguments;
                startInfo.Environment.Add("MSRDPEX_AXNAME", axName);
                startInfo.Environment.Add("MSRDPEX_LOG_ENABLED", "1");
                startInfo.Environment.Add("MSRDPEX_LOG_LEVEL", "TRACE");

                Process process = RdpProcess.StartProcess(startInfo);
                return;
            }

            Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", axName);

            RdpView rdpView = new RdpView(axName, rdpExDll);
            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = rdpView.rdpClient;

            if (axHookEnabled)
            {
                RdpInstance rdpInstance = new RdpInstance((IMsRdpExInstance)rdp.GetOcx());
                rdpInstance.OutputMirrorEnabled = true;
                rdpInstance.VideoRecordingEnabled = true;
            }

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

            if (axHookEnabled)
            {
                object corePropsVal = extendedSettings.get_Property("CoreProperties");
                IMsRdpExtendedSettings coreProps = (IMsRdpExtendedSettings)corePropsVal;

                object basePropsVal = extendedSettings.get_Property("BaseProperties");
                IMsRdpExtendedSettings baseProps = (IMsRdpExtendedSettings)basePropsVal;

                object ShellMarkRdpSecure = true;
                baseProps.set_Property("ShellMarkRdpSecure", ref ShellMarkRdpSecure);

                object DisableUDPTransport = true;
                coreProps.set_Property("DisableUDPTransport", ref DisableUDPTransport);

                object UsingSavedCreds = false;
                coreProps.set_Property("UsingSavedCreds", ref UsingSavedCreds);

                object DisableCTRLAltDel = false;
                coreProps.set_Property("DisableCTRLAltDel", ref DisableCTRLAltDel);

                object EnableCredSspSupport = true;
                coreProps.set_Property("EnableCredSspSupport", ref EnableCredSspSupport);

                object PromptForCredentials = false;
                baseProps.set_Property("PromptForCredentials", ref PromptForCredentials);

                object AllowCredentialSaving = false;
                baseProps.set_Property("AllowCredentialSaving", ref AllowCredentialSaving);
            }

            rdp.Connect();

            rdpView.Show();
        }
    }
}
