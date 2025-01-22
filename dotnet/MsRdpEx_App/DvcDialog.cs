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

namespace MsRdpEx_App
{
    public partial class DvcDialog : Form
    {
        public NowClient nowClient = null;
        public ExecSession currentExecSession = null;

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
                capabilitiesGrid.Invoke(() =>
                {
                    capabilitiesGrid.SelectedObject = capabilitiesView;
                });
            }
            else
            {
                capabilitiesGrid.SelectedObject = capabilitiesView;
            }
        }

        static int current_session_id = -1;

        private void OnSessionStarted(uint sessionId)
        {
            Debug.WriteLine("Session has started!");
        }

        private void OnSessionStdout(uint sessionId, ArraySegment<byte> outData, bool last)
        {
            string textToAppend = string.Empty;

            try
            {
                if (outData.Count != 0)
                {
                    textToAppend = Encoding.UTF8.GetString(outData.ToArray());
                }

            }
            catch (Exception)
            {
                textToAppend = $"<RAW BYTES ({outData.Count})>";
            }

            if (last)
            {
                textToAppend += "<EOF>";
            }

            if (InvokeRequired)
            {
                stdoutTextBox.Invoke(() =>
                {
                    stdoutTextBox.AppendText(textToAppend);
                });
            }
            else
            {
                stdoutTextBox.AppendText(textToAppend);
            }
        }

        private void OnSessionStderr(uint sessionId, ArraySegment<byte> outData, bool last)
        {
            string textToAppend = string.Empty;

            try
            {
                if (outData.Count != 0)
                {
                    textToAppend = Encoding.UTF8.GetString(outData.ToArray());
                }

            }
            catch (Exception)
            {
                textToAppend = $"<RAW BYTES ({outData.Count})>";
            }

            if (last)
            {
                textToAppend += "<EOF>";
            }

            if (InvokeRequired)
            {
                stderrTextBox.Invoke(() =>
                {
                    stderrTextBox.AppendText(textToAppend);
                });
            }
            else
            {
                stderrTextBox.AppendText(textToAppend);
            }
        }

        private async void execRunButton_Click(object sender, EventArgs e)
        {
            if (execKindComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an execution kind", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (nowClient == null)
            {
                MessageBox.Show("Now client is not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            stdinTextBox.Clear();
            stdoutTextBox.Clear();
            stderrTextBox.Clear();
            execLogTextBox.Clear();

            // ShellExecute
            if (execKindComboBox.SelectedIndex == 0)
            {
                var execParams = new ExecRunParams(execRunCommandTextBox.Text)
                {
                    OnStarted = OnSessionStarted,
                    OnStdout = OnSessionStdout,
                    OnStderr = OnSessionStderr
                };

                currentExecSession = await nowClient.ExecRun(execParams);
                return;
            }

            // Process
            if (execKindComboBox.SelectedIndex == 1)
            {
                var execParams = new ExecProcessParams(execRunCommandTextBox.Text)
                {
                    OnStarted = OnSessionStarted,
                    OnStdout = OnSessionStdout,
                    OnStderr = OnSessionStderr
                };

                if (execDirectoryTextBox.Text.Length != 0)
                {
                    execParams.Directory(execDirectoryTextBox.Text);
                }

                if (execArgsTextBox.Text.Length != 0)
                {
                    execParams.Parameters(execArgsTextBox.Text);
                }

                currentExecSession = await nowClient.ExecProcess(execParams);
                return;
            }

            // Batch
            if (execKindComboBox.SelectedIndex == 2)
            {
                var execParams = new ExecBatchParams(execRunCommandTextBox.Text)
                {
                    OnStarted = OnSessionStarted,
                    OnStdout = OnSessionStdout,
                    OnStderr = OnSessionStderr
                };

                if (execDirectoryTextBox.Text.Length != 0)
                {
                    execParams.Directory(execDirectoryTextBox.Text);
                }


                currentExecSession = await nowClient.ExecBatch(execParams);
                return;
            }

            // Powershell
            if (execKindComboBox.SelectedIndex == 3)
            {
                var execParams = new ExecWinPsParams(execRunCommandTextBox.Text)
                {
                    OnStarted = OnSessionStarted,
                    OnStdout = OnSessionStdout,
                    OnStderr = OnSessionStderr
                };

                if (execDirectoryTextBox.Text.Length != 0)
                {
                    execParams.Directory(execDirectoryTextBox.Text);
                }

                currentExecSession = await nowClient.ExecWinPs(execParams);
                return;
            }

            // Powershell
            if (execKindComboBox.SelectedIndex == 4)
            {
                var execParams = new ExecPwshParams(execRunCommandTextBox.Text)
                {
                    OnStarted = OnSessionStarted,
                    OnStdout = OnSessionStdout,
                    OnStderr = OnSessionStderr
                };

                if (execDirectoryTextBox.Text.Length != 0)
                {
                    execParams.Directory(execDirectoryTextBox.Text);
                }

                currentExecSession = await nowClient.ExecPwsh(execParams);
                return;
            }

            MessageBox.Show("Unknown execution kind", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void execRunPage_Click(object sender, EventArgs e)
        {

        }

        private async void sendStdinButton_Click(object sender, EventArgs e)
        {
            if (currentExecSession == null)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = stdinTextBox.Text;

            if (execSendCrlfCheckBox.Checked)
            {
                data += "\r\n";
            }

            await currentExecSession.SendStdin(Encoding.UTF8.GetBytes(data), sendEofCheckbox.Checked);
        }

        private async void abortButton_Click(object sender, EventArgs e)
        {
            if (currentExecSession == null)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                await currentExecSession.Abort(1);
            }
            finally
            {
                currentExecSession = null;
            }

        }

        private async void cancelButton_Click(object sender, EventArgs e)
        {
            if (currentExecSession == null)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                await currentExecSession.Cancel();
                Debug.WriteLine("Task cancelled!");
                currentExecSession = null;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }
}
