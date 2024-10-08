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
                default:
                {
                    Debug.WriteLine("Unknown UI update");
                    break;
                }
            }

        }

        private void execRunButton_Click(object sender, EventArgs e)
        {
            SendUiInteractionHandler handler = SendUiInteraction;
            if (handler != null)
            {
                var interaction = new JObject
                {
                    ["kind"] = "ExecRun",
                    ["cmd"] = execRunCommandTextBox.Text,
                    ["session_id"] = 0,
                };

                handler(interaction);
            }
        }
    }

    public delegate void SendUiInteractionHandler(JObject interaction);
}
