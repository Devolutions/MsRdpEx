using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devolutions.NowClient;
using Devolutions.NowProto.Exceptions;

namespace MsRdpEx_App
{
    public partial class ExecSessionDlg : Form
    {
        public ExecSessionDlg()
        {
            InitializeComponent();
        }

        private void SetGuiStateFinished()
        {
            stdinInput.Enabled = false;
            stdinInput.ReadOnly = true;
            crCheckBox.Enabled = false;
            lfCheckBox.Enabled = false;
            eofCheckBox.Enabled = false;
            sendStdinButton.Enabled = false;
            cancelButton.Enabled = false;
            abortButton.Enabled = false;
        }

        public async Task ProcessExecSession(ExecSession session, string kind)
        {
            sessionKindText.Text = kind;
            execSession = session;

            try
            {
                var code = await execSession.GetResult();
                logInput.Text += $"Finished with exit code {code}\n";
                sessionStatusText.Text = "finished";
            }
            catch (NowStatusException exception)
            {
                logInput.Text += $"Server error: {exception}";
                sessionStatusText.Text = "failed";
            }
            catch (NowClientException clientException)
            {
                logInput.Text += $"Client error: {clientException}";
                sessionStatusText.Text = "failed";
            }
            catch (Exception sessionException)
            {
                logInput.Text += $"Unexpected error: {sessionException}";
                sessionStatusText.Text = "failed";
            }

            SetGuiStateFinished();
        }

        private void AdjustControlsStarted()
        {
            stdinInput.Enabled = true;
            sessionStatusText.Text = "running";
            stdinInput.Enabled = true;
            sendStdinButton.Enabled = true;
            cancelButton.Enabled = true;
        }

        public void OnSessionStarted()
        {
            if (InvokeRequired)
            {
                Invoke(AdjustControlsStarted);
                return;
            }

            AdjustControlsStarted();
        }

        public void OnSessionStdout(ArraySegment<byte> outData, bool last)
        {
            if (stdoutInput.InvokeRequired)
            {
                stdoutInput.Invoke(() => OnSessionDataIn(stdoutInput, outData, last));
            }
            else
            {
                OnSessionDataIn(stdoutInput, outData, last);
            }
        }

        public void OnSessionStderr(ArraySegment<byte> outData, bool last)
        {
            if (stderrInput.InvokeRequired)
            {
                stderrInput.Invoke(() => OnSessionDataIn(stderrInput, outData, last));
            }
            else
            {
                OnSessionDataIn(stderrInput, outData, last);
            }
        }

        private void OnSessionDataIn(TextBox target, ArraySegment<byte> outData, bool last)
        {
            var textToAppend = string.Empty;

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

            target.AppendText(textToAppend);
        }

        private async void sendStdinButton_Click(object sender, EventArgs e)
        {
            var str = stdinInput.Text;

            if (crCheckBox.Checked)
            {
                str += "\r";
            }

            if (lfCheckBox.Checked)
            {
                str += "\n";
            }

            var data = Encoding.UTF8.GetBytes(str);

            await execSession.SendStdin(data, eofCheckBox.Checked);

            if (eofCheckBox.Checked)
            {
                sendStdinButton.Enabled = false;
                stdinInput.Enabled = false;
                stdinInput.ReadOnly = true;
                crCheckBox.Enabled = false;
                lfCheckBox.Enabled = false;
                eofCheckBox.Enabled = false;
            }

            logInput.Text += $"Sent stdin data bytes (data) \n";
        }

        private ExecSession execSession = null;

        private async void abortButton_Click(object sender, EventArgs e)
        {
            var abortCode = uint.Parse(abortCodeInput.Text);
            await execSession.Abort(abortCode);

            logInput.Text += $"Aborted with code {abortCode}\n";
            sessionStatusText.Text = "aborted";
            SetGuiStateFinished();
        }

        private async void cancelButton_Click(object sender, EventArgs e)
        {
            logInput.Text += "Sending cancel request...\n";
            try
            {
                await execSession.Cancel();
            } catch (NowStatusException exception)
            {
                logInput.Text += $"Cancel failed: {exception}\n";
            }

            logInput.Text += "Session cancelled\n";
            sessionStatusText.Text = "cancelled";
        }
    }
}
