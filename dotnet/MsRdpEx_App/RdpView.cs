using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MsRdpEx_App
{
    public partial class RdpView : Form
    {
        public string axName = "latest";

        public RdpView(string axName)
        {
            this.axName = axName;
            InitializeComponent();
        }

        private System.ComponentModel.IContainer components = null;
        public AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdpClient;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RdpView));
            this.rdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            this.rdpClient.axName = this.axName;
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // rdpClient
            // 
            this.rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpClient.Enabled = true;
            this.rdpClient.Location = new System.Drawing.Point(0, 0);
            this.rdpClient.Name = "rdpClient";
            this.rdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdpClient.OcxState")));
            this.rdpClient.Size = new System.Drawing.Size(1107, 503);
            this.rdpClient.TabIndex = 0;
            // 
            // RdpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 503);
            this.Controls.Add(this.rdpClient);
            this.Name = "RdpView";
            this.Text = "Remote Desktop Client";
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).EndInit();
            this.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
