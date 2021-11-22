using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MSTSCLib;

namespace MsRdpEx
{
    public partial class MainDlg : Form
    {
        public MainDlg()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            RdpView rdpView = new RdpView();
            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = rdpView.m_MsRdpClient;
            rdp.Server = "RDP-SERVER";
            rdp.UserName = "Administrator";
            rdp.Domain = "DOMAIN";
            rdp.AdvancedSettings9.EnableCredSspSupport = true;
            IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp.GetOcx();
            secured.ClearTextPassword = "Password123!";
            rdp.Connect();
            rdpView.Show();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

        }
    }
}
