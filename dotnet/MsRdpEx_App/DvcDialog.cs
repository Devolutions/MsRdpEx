using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devolutions.NowClient;
using Devolutions.NowProto.Messages;
using Devolutions.NowProto.Capabilities;
using Devolutions.NowProto.Exceptions;

namespace MsRdpEx_App
{
    public partial class DvcDialog : Form
    {
        public NowClient nowClient = null;

        class CapabilitiesView
        {
            public static CapabilitiesView FromUpdate(NowMsgChannelCapset capabilities)
            {
                return new CapabilitiesView
                {
                    Run = capabilities.ExecCapset.HasFlag(NowCapabilityExec.Run),
                    Batch = capabilities.ExecCapset.HasFlag(NowCapabilityExec.Batch),
                    Process = capabilities.ExecCapset.HasFlag(NowCapabilityExec.Process),
                    Shell = capabilities.ExecCapset.HasFlag(NowCapabilityExec.Shell),
                    WinPs = capabilities.ExecCapset.HasFlag(NowCapabilityExec.WinPs),
                    Pwsh = capabilities.ExecCapset.HasFlag(NowCapabilityExec.Pwsh),
                };
            }

            public bool Run { get; private init; }
            public bool Batch { get; private init; }
            public bool Process { get; private init; }
            public bool Shell { get; private init; }
            public bool WinPs { get; private init; }
            public bool Pwsh { get; private init; }
            public bool AppleScript { get; private init; }
        }

        public DvcDialog()
        {
            InitializeComponent();
            msgBoxKindSelect.SelectedIndex = 0;
        }

        public void OnDvcConnected(INowTransport transport)
        {
            Debug.WriteLine("OnDvcConnected triggered.");
            Task.Run(async () =>
            {
                // Connect and negotiate capabilities.
                var client = await NowClient.Connect(transport);
                Debug.WriteLine("NOW channel has been connected!");
                OnNowClientConnected(client);

            });
        }

        public void OnNowClientConnected(NowClient client)
        {
            nowClient = client;
            var capabilitiesView = CapabilitiesView.FromUpdate(nowClient.Capabilities);

            // Set capabilities grid when connected
            if (InvokeRequired)
            {
                capabilitiesGrid.Invoke(() => { capabilitiesGrid.SelectedObject = capabilitiesView; });
            }
            else
            {
                capabilitiesGrid.SelectedObject = capabilitiesView;
            }
        }


        private async void execRunButton_Click(object sender, EventArgs e)
        {
            try
            {
                await RunSession();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RunSession()
        {
            if (nowClient == null)
            {
                MessageBox.Show("Now client is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ShellExecute
            if (runRadioButton.Checked)
            {
                // TODO: no callbacks for RUN kind
                var execParams = new ExecRunParams(fileInput.Text)
                {
                    OnStarted = (_) => { },
                    OnStdout = (_, data, last) => { },
                    OnStderr = (_, data, last) => { }
                };

                await nowClient.ExecRun(execParams);
                return;
            }

            // Process
            if (processRadioButton.Checked)
            {
                var sessionDlg = new ExecSessionDlg();

                var execParams = new ExecProcessParams(fileInput.Text)
                {
                    OnStarted = (_) => sessionDlg.OnSessionStarted(),
                    OnStdout = (_, data, last) => sessionDlg.OnSessionStdout(data, last),
                    OnStderr = (_, data, last) => sessionDlg.OnSessionStderr(data, last)
                };

                if (directoryInput.Text.Length != 0)
                {
                    execParams.Directory(directoryInput.Text);
                }

                if (cmdInput.Text.Length != 0)
                {
                    execParams.Parameters(cmdInput.Text);
                }

                var session = await nowClient.ExecProcess(execParams);
                sessionDlg.Show();
                await sessionDlg.ProcessExecSession(session, "process");

                return;
            }

            // Batch
            if (batchRadioButton.Checked)
            {
                var sessionDlg = new ExecSessionDlg();

                var execParams = new ExecBatchParams(cmdInput.Text)
                {
                    OnStarted = (_) => sessionDlg.OnSessionStarted(),
                    OnStdout = (_, data, last) => sessionDlg.OnSessionStdout(data, last),
                    OnStderr = (_, data, last) => sessionDlg.OnSessionStderr(data, last)
                };

                if (directoryInput.Text.Length != 0)
                {
                    execParams.Directory(directoryInput.Text);
                }

                var session = await nowClient.ExecBatch(execParams);
                sessionDlg.Show(this);
                await sessionDlg.ProcessExecSession(session, "batch");
                return;
            }

            // Powershell
            if (powershellRadioButton.Checked)
            {
                var sessionDlg = new ExecSessionDlg();

                var execParams = new ExecWinPsParams(cmdInput.Text)
                {
                    OnStarted = (_) => sessionDlg.OnSessionStarted(),
                    OnStdout = (_, data, last) => sessionDlg.OnSessionStdout(data, last),
                    OnStderr = (_, data, last) => sessionDlg.OnSessionStderr(data, last)
                };

                if (directoryInput.Text.Length != 0)
                {
                    execParams.Directory(directoryInput.Text);
                }

                var session = await nowClient.ExecWinPs(execParams);
                sessionDlg.Show(this);
                await sessionDlg.ProcessExecSession(session, "powershell");
                return;

            }

            // Powershell
            if (pwshRadioButton.Checked)
            {
                var sessionDlg = new ExecSessionDlg();

                var execParams = new ExecPwshParams(cmdInput.Text)
                {
                    OnStarted = (_) => sessionDlg.OnSessionStarted(),
                    OnStdout = (_, data, last) => sessionDlg.OnSessionStdout(data, last),
                    OnStderr = (_, data, last) => sessionDlg.OnSessionStderr(data, last)
                };

                if (directoryInput.Text.Length != 0)
                {
                    execParams.Directory(directoryInput.Text);
                }

                var session = await nowClient.ExecPwsh(execParams);
                sessionDlg.Show(this);
                await sessionDlg.ProcessExecSession(session, "pwsh");
                return;
            }
        }

        private void runRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var fileTextStr = "";
            var cmdTextStr = "";
            var directoryTextStr = "";

            if (runRadioButton.Checked)
            {
                fileTextStr = "Command:";
            }

            if (processRadioButton.Checked)
            {
                fileTextStr = "File:";
                cmdTextStr = "Parameters:";
                directoryTextStr = "Directory:";
            }

            if (batchRadioButton.Checked | powershellRadioButton.Checked | pwshRadioButton.Checked)
            {
                cmdTextStr = "Script:";
                directoryTextStr = "Directory:";
            }

            fileInput.Text = "";
            cmdInput.Text = "";
            directoryInput.Text = "";

            if (fileTextStr.Length != 0)
            {
                fileLabel.Text = fileTextStr;
                fileInput.Enabled = true;
                fileInput.ReadOnly = false;
            }
            else
            {
                fileLabel.Text = "---";
                fileInput.Enabled = false;
                fileInput.ReadOnly = true;
            }

            if (cmdTextStr.Length != 0)
            {
                cmdLabel.Text = cmdTextStr;
                cmdInput.Enabled = true;
                cmdInput.ReadOnly = false;
            }
            else
            {
                cmdLabel.Text = "---";
                cmdInput.Enabled = false;
                cmdInput.ReadOnly = true;
            }

            if (directoryTextStr.Length != 0)
            {
                directoryLabel.Text = directoryTextStr;
                directoryInput.Enabled = true;
                directoryInput.ReadOnly = false;
            }
            else
            {
                directoryLabel.Text = "---";
                directoryInput.Enabled = false;
                directoryInput.ReadOnly = true;
            }
        }

        private async void msgBoxShowButton_Click(object sender, EventArgs e)
        {
            var msgBoxParams = new MessageBoxParams(msgBoxMessageInput.Text);

            if (msgBoxTitleInput.Text.Length > 0)
            {
                msgBoxParams.Title(msgBoxTitleInput.Text);
            }

            if (msgBoxKindSelect.SelectedIndex > 0)
            {
                var kind = msgBoxKindSelect.SelectedIndex switch
                {
                    1 => NowMsgSessionMessageBoxReq.MessageBoxStyle.Ok,
                    2 => NowMsgSessionMessageBoxReq.MessageBoxStyle.OkCancel,
                    3 => NowMsgSessionMessageBoxReq.MessageBoxStyle.AbortRetryIgnore,
                    4 => NowMsgSessionMessageBoxReq.MessageBoxStyle.YesNoCancel,
                    5 => NowMsgSessionMessageBoxReq.MessageBoxStyle.YesNo,
                    6 => NowMsgSessionMessageBoxReq.MessageBoxStyle.RetryCancel,
                    7 => NowMsgSessionMessageBoxReq.MessageBoxStyle.CancelTryContinue,
                    8 => NowMsgSessionMessageBoxReq.MessageBoxStyle.Help,
                    _ => throw new InvalidOperationException("invalid combo box contents")
                };

                msgBoxParams.Style(kind);
            }

            if (msgBoxTimeoutFlag.Checked)
            {
                msgBoxParams.Timeout(
                    TimeSpan.FromSeconds(uint.Parse(msgBoxTimeoutInput.Text))
                );
            }

            if (msgBoxResponseFlag.Checked)
            {
                msgBoxResponseText.Text = "<waiting...>";

                var responseHandler = await nowClient.SessionMessageBox(msgBoxParams);
                var response = await responseHandler.GetResponse();
                msgBoxResponseText.Text = response.ToString();
            }
            else
            {
                msgBoxResponseText.Text = "---";
                await nowClient.SessionMessageBoxNoResponse(msgBoxParams);
            }

        }

        private async void lockButton_Click(object sender, EventArgs e)
        {
            await nowClient.SessionLock();
        }

        private async void logoffButton_Click(object sender, EventArgs e)
        {
            await nowClient.SessionLogoff();
        }

        private async void shutdownButton_Click(object sender, EventArgs e)
        {
            var shutdownParams = new SystemShutdownParams();

            if (shutdownForceFlag.Checked)
            {
                shutdownParams.Force();
            }

            if (shutdownRebootFlag.Checked)
            {
                shutdownParams.Reboot();
            }

            if (shutdownTimeoutFlag.Checked)
            {
                shutdownParams.Timeout(
                    TimeSpan.FromSeconds(uint.Parse(shutdownTimeoutInput.Text))
                );
            }

            if (shutdownMessageInput.Text.Length > 0)
            {
                shutdownParams.Message(shutdownMessageInput.Text);
            }


            await nowClient.SystemShutdown(shutdownParams);
        }
    }
}
