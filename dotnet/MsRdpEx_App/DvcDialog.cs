using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MsRdpEx_App
{
    public partial class DvcDialog : Form
    {
        public event SendUiInteractionHandler SendUiInteraction;

        class CapabilitiesView
        {
            public static CapabilitiesView FromUpdate(JObject update)
            {
                return new CapabilitiesView
                {
                    run = (bool)update["run"],
                    cmd = (bool)update["cmd"],
                    process = (bool)update["process"],
                    shell = (bool)update["shell"],
                    batch = (bool)update["batch"],
                    winps = (bool)update["winps"],
                    pwsh = (bool)update["pwsh"],
                    applescript = (bool)update["applescript"],
                };
            }

            public bool Run { get => run; }
            public bool Cmd { get => cmd; }
            public bool Process { get => process; }
            public bool Shell { get => shell; }
            public bool Batch { get => batch; }
            public bool WinPS { get => winps; }
            public bool Pwsh { get => pwsh; }
            public bool AppleScript { get => applescript; }


            bool run;
            bool cmd;
            bool process;
            bool shell;
            bool batch;
            bool winps;
            bool pwsh;
            bool applescript;
        }

        public DvcDialog()
        {
            InitializeComponent();
        }

        public void ProcessUpdate(JObject update)
        {
            switch ((string)update["kind"])
            {
                case "ShowCapabilities": {
                    var capabilities_view = CapabilitiesView.FromUpdate(update);

                    capabilitiesGrid.SelectedObject = capabilities_view;

                    break;
                }
                case "MessageBoxResult": {
                    var kind = (string)update["message_box_kind"];
                    MessageBox.Show(kind, "Remote message box result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                case "ExecLog": {
                    var session_id = (int)update["session_id"];
                    var info = (string)update["info"];
                    execLogTextBox.AppendText($"[{session_id}] {info}");
                    break;
                }
                case "ExecDataOut": {
                    var session_id = (int)update["session_id"];
                    var stderr = (bool)update["stderr"];
                    var data = (string)update["data"];

                    if (stderr)
                    {
                        stderrTextBox.AppendText(data);
                    }
                    else
                    {
                        stdoutTextBox.AppendText(data);
                    }

                    break;
                }
                default:
                {
                    Debug.WriteLine("Unknown UI update");
                    break;
                }
            }

        }

        static int current_session_id = -1;

        private void execRunButton_Click(object sender, EventArgs e)
        {
            SendUiInteractionHandler handler = SendUiInteraction;
            if (handler == null)
            {
                return;
            }

            if (execKindComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an execution kind", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            stdinTextBox.Clear();
            stdoutTextBox.Clear();
            stderrTextBox.Clear();
            execLogTextBox.Clear();

            // ShellExecute
            if (execKindComboBox.SelectedIndex == 0)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["exec_kind"] = "run",
                    ["file"] = execRunCommandTextBox.Text,
                    ["args"] = "",
                    ["directory"] = "",
                    ["session_id"] = ++current_session_id,
                };

                handler(interaction);
                return;
            }

            // Process
            if (execKindComboBox.SelectedIndex == 1)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["exec_kind"] = "process",
                    ["file"] = execRunCommandTextBox.Text,
                    ["args"] = execArgsTextBox.Text,
                    ["directory"] = execDirectoryTextBox.Text,
                    ["session_id"] = ++current_session_id,
                };

                handler(interaction);
                return;
            }

            // Batch
            if (execKindComboBox.SelectedIndex == 2)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["exec_kind"] = "cmd",
                    ["file"] = execRunCommandTextBox.Text,
                    ["args"] = "",
                    ["directory"] = "",
                    ["session_id"] = ++current_session_id,
                };

                handler(interaction);
                return;
            }

            // Powershell
            if (execKindComboBox.SelectedIndex == 3)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["exec_kind"] = "powershell",
                    ["file"] = execRunCommandTextBox.Text,
                    ["args"] = "",
                    ["directory"] = "",
                    ["session_id"] = ++current_session_id,
                };

                handler(interaction);
                return;
            }

            // Powershell
            if (execKindComboBox.SelectedIndex == 4)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["exec_kind"] = "pwsh",
                    ["file"] = execRunCommandTextBox.Text,
                    ["args"] = "",
                    ["directory"] = "",
                    ["session_id"] = ++current_session_id,
                };

                handler(interaction);
                return;
            }

            MessageBox.Show("Unknown execution kind", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void execRunPage_Click(object sender, EventArgs e)
        {

        }

        private void sendStdinButton_Click(object sender, EventArgs e)
        {
            if (current_session_id == -1)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SendUiInteractionHandler handler = SendUiInteraction;
            if (handler == null)
            {
                return;
            }

            var data = stdinTextBox.Text;

            if (execSendCrlfCheckBox.Checked)
            {
                data += "\r\n";
            }

            var interaction = new JObject
            {
                ["kind"] = "ExecStdin",
                ["session_id"] = current_session_id,
                ["data"] = data,
                ["eof"] = sendEofCheckbox.Checked,
            };

            handler(interaction);
            return;
        }

        private void abortButton_Click(object sender, EventArgs e)
        {
            if (current_session_id == -1)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SendUiInteractionHandler handler = SendUiInteraction;
            if (handler == null)
            {
                return;
            }

            var interaction = new JObject
            {
                ["kind"] = "ExecAbort",
                ["session_id"] = current_session_id,
                ["status"] = 1,
            };

            handler(interaction);
            return;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (current_session_id == -1)
            {
                MessageBox.Show("No session is active", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SendUiInteractionHandler handler = SendUiInteraction;
            if (handler == null)
            {
                return;
            }

            var interaction = new JObject
            {
                ["kind"] = "ExecCancel",
                ["session_id"] = current_session_id,
            };

            handler(interaction);
            return;
        }
    }

    public delegate void SendUiInteractionHandler(JObject interaction);
}
