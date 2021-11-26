
namespace MsRdpEx_App
{
    partial class RdpView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RdpView));
            this.m_MsRdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            this.m_MsRdpClient.axName = this.axName;
            ((System.ComponentModel.ISupportInitialize)(this.m_MsRdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // m_MsRdpClient
            // 
            this.m_MsRdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_MsRdpClient.Enabled = true;
            this.m_MsRdpClient.Location = new System.Drawing.Point(0, 0);
            this.m_MsRdpClient.Name = "m_MsRdpClient";
            this.m_MsRdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_MsRdpClient.OcxState")));
            this.m_MsRdpClient.Size = new System.Drawing.Size(1107, 503);
            this.m_MsRdpClient.TabIndex = 0;
            // 
            // RdpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 503);
            this.Controls.Add(this.m_MsRdpClient);
            this.Name = "RdpView";
            this.Text = "Remote Desktop Client";
            ((System.ComponentModel.ISupportInitialize)(this.m_MsRdpClient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxMSTSCLib.AxMsRdpClient9NotSafeForScripting m_MsRdpClient;
    }
}