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

        private void ParseRdpFile(string filename, AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp)
        {
            IMsRdpExtendedSettings extendedSettings = (IMsRdpExtendedSettings)rdp.GetOcx();
            var advancedSettings = rdp.AdvancedSettings9;
            var transportSettings = rdp.TransportSettings4;
            var remoteProgram = rdp.RemoteProgram2;

            object corePropsVal = extendedSettings.get_Property("CoreProperties");
            IMsRdpExtendedSettings coreProps = (IMsRdpExtendedSettings)corePropsVal;

            object basePropsVal = extendedSettings.get_Property("BaseProperties");
            IMsRdpExtendedSettings baseProps = (IMsRdpExtendedSettings)basePropsVal;

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                int sc1 = line.IndexOf(':');
                int sc2 = (sc1 > 0) ? sc1 + 2 : -1;
                if ((sc1 > 0) && (sc2 < line.Length) && (line[sc2] == ':'))
                {
                    string name = line.Substring(0, sc1);
                    char type = line[sc1 + 1];
                    string value = line.Substring(sc2 + 1, line.Length - sc2 - 1);

                    Debug.WriteLine("{0}:{1}:{2}", name, type, value);

                    if (type == 's')
                    {
                        switch (name.ToLower())
                        {
                            case "full address":
                                rdp.Server = value;
                                break;

                            case "alternate full address":
                                break;

                            case "loadbalanceinfo":
                                advancedSettings.LoadBalanceInfo = value;
                                break;

                            case "workspace id":
                                baseProps.set_Property("WorkspaceID", value);
                                break;

                            case "gatewayhostname":
                                transportSettings.GatewayHostname = value;
                                break;

                            case "remoteapplicationname":
                                remoteProgram.RemoteApplicationName = value;
                                break;

                            case "remoteapplicationprogram":
                                remoteProgram.RemoteApplicationProgram = value;
                                break;

                            case "remotedesktopname":
                                coreProps.set_Property("RemoteDesktopName", value);
                                break;

                            case "wvd endpoint pool":
                                coreProps.set_Property("HostPoolId", value);
                                break;

                            case "diagnosticserviceurl":
                                coreProps.set_Property("RDmiDiagnosticsUrl", value);
                                break;

                            case "hubdiscoverygeourl":
                                coreProps.set_Property("RDmiEHDiscoveryUrl", value);
                                break;

                            case "resourceprovider":
                                coreProps.set_Property("RDmiResourceProvider", value);
                                break;

                            case "armpath":
                                coreProps.set_Property("armPath", value);
                                break;

                            case "geo":
                                break;

                            case "kdcproxyurl":
                                extendedSettings.set_Property("KDCProxyURL", value);
                                break;
                        }
                    }
                    else if (type == 'i')
                    {
                        uint iValue = uint.Parse(value);
                        bool bValue = iValue == 0 ? false : true;

                        switch (name.ToLower())
                        {
                            case "authentication level":
                                advancedSettings.AuthenticationLevel = iValue;
                                break;

                            case "enablecredsspsupport":
                                advancedSettings.EnableCredSspSupport = bValue;
                                break;

                            case "promptcredentialonce":
                                break;

                            case "audiomode":
                                advancedSettings.AudioRedirectionMode = iValue;
                                break;

                            case "gatewayusagemethod":
                                transportSettings.GatewayUsageMethod = iValue;
                                break;

                            case "gatewayprofileusagemethod":
                                transportSettings.GatewayProfileUsageMethod = iValue;
                                break;

                            case "gatewaybrokeringtype":
                                transportSettings.GatewayBrokeringType = iValue;
                                break;

                            case "gatewaycredentialssource":
                                transportSettings.GatewayCredsSource = iValue;
                                break;
                        }
                    }
                }
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

                if (axName.Equals("msrdc"))
                {
                    startInfo.Environment.Add("MSRDPEX_RDCLIENTAX_DLL", this.msrdcAxLibrary);
                }
                else
                {
                    startInfo.Environment.Add("MSRDPEX_MSTSCAX_DLL", this.mstscAxLibrary);
                }

                Process process = RdpProcess.StartProcess(startInfo);
                return;
            }

            RdpView rdpView;

            if (axHookEnabled)
            {
                rdpView = new RdpView(axName, rdpExDll);
            }
            else
            {
                rdpView = new RdpView(axName, null);
            }

            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = rdpView.rdpClient;

            if (axHookEnabled)
            {
                RdpInstance rdpInstance = new RdpInstance((IMsRdpExInstance)rdp.GetOcx());
                rdpInstance.OutputMirrorEnabled = false;
                rdpInstance.VideoRecordingEnabled = false;

                Guid sessionId = rdpInstance.SessionId;
                Debug.WriteLine("SessionId: {0}", sessionId);
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

            try {
                object RequestUseNewOutputPresenter = true;
                extendedSettings.set_Property("RequestUseNewOutputPresenter", ref RequestUseNewOutputPresenter);
            } catch { }

            if (axHookEnabled)
            {
                object corePropsVal = extendedSettings.get_Property("CoreProperties");
                IMsRdpExtendedSettings coreProps = (IMsRdpExtendedSettings)corePropsVal;

                object basePropsVal = extendedSettings.get_Property("BaseProperties");
                IMsRdpExtendedSettings baseProps = (IMsRdpExtendedSettings)basePropsVal;

                object BandwidthAutodetect = false;
                coreProps.set_Property("BandwidthAutodetect", ref BandwidthAutodetect);

                object DisableUDPTransport = true;
                coreProps.set_Property("DisableUDPTransport", ref DisableUDPTransport);

                object EnableCredSspSupport = true;
                coreProps.set_Property("EnableCredSspSupport", ref EnableCredSspSupport);
            }

            ParseRdpFile(this.rdpFileName, rdp);

            rdp.Connect();

            rdpView.Show();
        }
    }
}
