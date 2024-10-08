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
using System.Runtime.InteropServices;

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
            this.cboRdpClient.SelectedIndex = 0;
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

        [StructLayout(LayoutKind.Sequential)]
        public struct CryptProtectPromptStruct
        {
            public int Size;
            public int Flags;
            public IntPtr Window;
            public string Message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DataBlob
        {
            public int Size;
            public IntPtr Data;
        }

        public const int CRYPTPROTECT_LOCAL_MACHINE = 0x00000004;
        public const int CRYPTPROTECT_UI_FORBIDDEN = 0x00000001;
        public const int CRYPTPROTECT_AUDIT = 0x00000010;

        [DllImport("crypt32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CryptProtectData(
            ref DataBlob dataIn,
            IntPtr description,
            IntPtr optionalEntropy,
            IntPtr reserved,
            IntPtr promptStruct,
            int flags,
            out DataBlob dataOut);

        [DllImport("crypt32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CryptUnprotectData(
            ref DataBlob dataIn,
            IntPtr description,
            IntPtr optionalEntropy,
            IntPtr reserved,
            IntPtr promptStruct,
            int flags,
            out DataBlob dataOut);

        public static byte[] TsCryptEncryptString(string inputString)
        {
            DataBlob inputBlob;
            DataBlob outputBlob;
            byte[] outputData = null;

            byte[] stringBytes = Encoding.Unicode.GetBytes(inputString);
            byte[] inputData = new byte[stringBytes.Length + 2];
            Buffer.BlockCopy(stringBytes, 0, inputData, 0, stringBytes.Length);

            inputBlob.Size = inputData.Length;
            inputBlob.Data = Marshal.AllocHGlobal(inputData.Length);
            Marshal.Copy(inputData, 0, inputBlob.Data, inputBlob.Size);

            if (CryptProtectData(ref inputBlob, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero, CRYPTPROTECT_UI_FORBIDDEN, out outputBlob))
            {
                outputData = new byte[outputBlob.Size];
                Marshal.Copy(outputBlob.Data, outputData, 0, outputBlob.Size);
            }

            Marshal.FreeHGlobal(inputBlob.Data);
            Marshal.FreeHGlobal(outputBlob.Data);

            return outputData;
        }

        public static string TsCryptDecryptString(byte[] inputBytes)
        {
            DataBlob inputBlob;
            DataBlob outputBlob;
            byte[] outputData = null;

            inputBlob.Size = inputBytes.Length;
            inputBlob.Data = Marshal.AllocHGlobal(inputBytes.Length);
            Marshal.Copy(inputBytes, 0, inputBlob.Data, inputBlob.Size);

            if (CryptUnprotectData(ref inputBlob, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero, CRYPTPROTECT_UI_FORBIDDEN, out outputBlob))
            {
                outputData = new byte[outputBlob.Size];
                Marshal.Copy(outputBlob.Data, outputData, 0, outputBlob.Size);
            }

            Marshal.FreeHGlobal(inputBlob.Data);
            Marshal.FreeHGlobal(outputBlob.Data);

            if (outputData != null)
            {
                return Encoding.Unicode.GetString(outputData).TrimEnd((Char)0);
            }

            return null;
        }

        public static string EncryptAuthCookieString(string cookieString)
        {
            byte[] cookieBytes = TsCryptEncryptString(cookieString);

            if (cookieBytes != null)
            {
                return Convert.ToBase64String(cookieBytes);
            }

            return null;
        }

        public static string DecryptAuthCookieString(string cookieString)
        {
            return TsCryptDecryptString(Convert.FromBase64String(cookieString));
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr SysAllocStringByteLen(IntPtr psz, uint len);

        public static void SetLoadBalanceInfo(IMsRdpClientAdvancedSettings advancedSettings, string loadBalanceInfo)
        {
            loadBalanceInfo += "\r\n";
            byte[] bytes = Encoding.UTF8.GetBytes(loadBalanceInfo);
            uint byteLen = (uint)bytes.Length;

            IntPtr bstrPtr = SysAllocStringByteLen(IntPtr.Zero, byteLen);
            Marshal.Copy(bytes, 0, bstrPtr, (int)byteLen);

            IMsRdpClientAdvancedSettingsLB lbSettings = (IMsRdpClientAdvancedSettingsLB)advancedSettings;
            lbSettings.LoadBalanceInfo = bstrPtr;

            Marshal.ZeroFreeBSTR(bstrPtr);
        }

        private void ParseRdpFile(string filename, AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp)
        {
            IMsRdpExtendedSettings extendedSettings = (IMsRdpExtendedSettings)rdp.GetOcx();
            IMsRdpPreferredRedirectionInfo redirectionInfo = (IMsRdpPreferredRedirectionInfo)rdp.GetOcx();
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
                                SetLoadBalanceInfo((IMsRdpClientAdvancedSettings)advancedSettings, value);
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

                            case "gatewayaccesstoken":
                                {
                                    string encryptedAuthCookie = EncryptAuthCookieString(value);
                                    transportSettings.GatewayEncryptedAuthCookie = encryptedAuthCookie; // "Cookie based authentication"
                                    transportSettings.GatewayEncryptedAuthCookieSize = (uint)encryptedAuthCookie.Length;
                                }
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
                                transportSettings.GatewayCredSharing = iValue;
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

                            case "use redirection server name":
                                redirectionInfo.UseRedirectionServerName = bValue;
                                break;

                            case "allowbackgroundinput":
                                advancedSettings.allowBackgroundInput = bValue ? 1 : 0;
                                break;

                            case "enablemousejiggler":
                                extendedSettings.set_Property("EnableMouseJiggler", bValue);
                                break;

                            case "mousejigglerinterval":
                                extendedSettings.set_Property("MouseJigglerInterval", iValue);
                                break;

                            case "mousejigglermethod":
                                extendedSettings.set_Property("MouseJigglerMethod", iValue);
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
                string executableFileName = this.mstscExecutable;

                if (axName.Equals("msrdc"))
                {
                    executableFileName = this.msrdcExecutable;
                }

                string workingDirectory = Path.GetDirectoryName(executableFileName);

                List<string> args = new List<string>();
                args.Add('"' + executableFileName + '"');

                if (this.rdpFileName != null)
                {
                    args.Add(this.rdpFileName);
                }

                string arguments = string.Join(" ", args.ToArray());

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = executableFileName;
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

            Guid sessionId = Guid.Empty;
            RdpDvcPlugin wtsPlugin = new RdpDvcPlugin(rdpView);

            if (axHookEnabled)
            {
                RdpInstance rdpInstance = new RdpInstance((IMsRdpExInstance)rdp.GetOcx());
                rdpInstance.OutputMirrorEnabled = false;
                rdpInstance.VideoRecordingEnabled = false;
                rdpInstance.WTSPlugin = wtsPlugin;

                sessionId = rdpInstance.SessionId;
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
            rdp.AdvancedSettings.allowBackgroundInput = 1;
            Size DesktopSize = new Size(1920, 1080);
            rdp.DesktopWidth = DesktopSize.Width;
            rdp.DesktopHeight = DesktopSize.Height;
            rdpView.ClientSize = DesktopSize;
            rdpView.Text = String.Format("{0} ({1})", rdp.Server, axName);

            // https://learn.microsoft.com/en-us/windows/win32/termserv/dvc-plug-in-registration
            string pluginCLSID = "7009F103-4B7E-48E2-81BC-46AB3FC1B64C";
            pluginCLSID = sessionId.ToString("D");
            string pluginDlls = String.Format("{0}:{{{1}}}", rdpExDll, pluginCLSID);
            rdp.AdvancedSettings.PluginDlls = pluginDlls;

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

            if (File.Exists(this.rdpFileName))
            {
                ParseRdpFile(this.rdpFileName, rdp);
            }

            rdp.Connect();

            rdpView.Show();
        }
    }
}
