
namespace MsRdpEx_App
{
    partial class MainDlg
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblComputer = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.grpLogonSettings = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtComputer = new System.Windows.Forms.TextBox();
            this.grpClientSettings = new System.Windows.Forms.GroupBox();
            this.cboRdpClient = new System.Windows.Forms.ComboBox();
            this.lblRdpClient = new System.Windows.Forms.Label();
            this.lblLaunchMode = new System.Windows.Forms.Label();
            this.cboLaunchMode = new System.Windows.Forms.ComboBox();
            this.grpLogonSettings.SuspendLayout();
            this.grpClientSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(277, 260);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblComputer
            // 
            this.lblComputer.AutoSize = true;
            this.lblComputer.Location = new System.Drawing.Point(16, 22);
            this.lblComputer.Name = "lblComputer";
            this.lblComputer.Size = new System.Drawing.Size(55, 13);
            this.lblComputer.TabIndex = 3;
            this.lblComputer.Text = "Computer:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(16, 49);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(61, 13);
            this.lblUserName.TabIndex = 4;
            this.lblUserName.Text = "User name:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(16, 76);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password:";
            // 
            // grpLogonSettings
            // 
            this.grpLogonSettings.Controls.Add(this.txtPassword);
            this.grpLogonSettings.Controls.Add(this.txtUserName);
            this.grpLogonSettings.Controls.Add(this.txtComputer);
            this.grpLogonSettings.Controls.Add(this.lblComputer);
            this.grpLogonSettings.Controls.Add(this.lblPassword);
            this.grpLogonSettings.Controls.Add(this.lblUserName);
            this.grpLogonSettings.Location = new System.Drawing.Point(12, 12);
            this.grpLogonSettings.Name = "grpLogonSettings";
            this.grpLogonSettings.Size = new System.Drawing.Size(340, 114);
            this.grpLogonSettings.TabIndex = 6;
            this.grpLogonSettings.TabStop = false;
            this.grpLogonSettings.Text = "Logon settings";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(127, 73);
            this.txtPassword.MaxLength = 128;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(182, 20);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(127, 46);
            this.txtUserName.MaxLength = 128;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(182, 20);
            this.txtUserName.TabIndex = 7;
            // 
            // txtComputer
            // 
            this.txtComputer.Location = new System.Drawing.Point(127, 19);
            this.txtComputer.MaxLength = 128;
            this.txtComputer.Name = "txtComputer";
            this.txtComputer.Size = new System.Drawing.Size(182, 20);
            this.txtComputer.TabIndex = 6;
            // 
            // grpClientSettings
            // 
            this.grpClientSettings.Controls.Add(this.cboLaunchMode);
            this.grpClientSettings.Controls.Add(this.lblLaunchMode);
            this.grpClientSettings.Controls.Add(this.cboRdpClient);
            this.grpClientSettings.Controls.Add(this.lblRdpClient);
            this.grpClientSettings.Location = new System.Drawing.Point(13, 133);
            this.grpClientSettings.Name = "grpClientSettings";
            this.grpClientSettings.Size = new System.Drawing.Size(339, 121);
            this.grpClientSettings.TabIndex = 7;
            this.grpClientSettings.TabStop = false;
            this.grpClientSettings.Text = "RDP Client Settings";
            // 
            // cboRdpClient
            // 
            this.cboRdpClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRdpClient.FormattingEnabled = true;
            this.cboRdpClient.Items.AddRange(new object[] {
            "mstsc",
            "msrdc"});
            this.cboRdpClient.Location = new System.Drawing.Point(126, 31);
            this.cboRdpClient.Name = "cboRdpClient";
            this.cboRdpClient.Size = new System.Drawing.Size(182, 21);
            this.cboRdpClient.TabIndex = 1;
            // 
            // lblRdpClient
            // 
            this.lblRdpClient.AutoSize = true;
            this.lblRdpClient.Location = new System.Drawing.Point(15, 34);
            this.lblRdpClient.Name = "lblRdpClient";
            this.lblRdpClient.Size = new System.Drawing.Size(72, 13);
            this.lblRdpClient.TabIndex = 0;
            this.lblRdpClient.Text = "Client Engine:";
            // 
            // lblLaunchMode
            // 
            this.lblLaunchMode.AutoSize = true;
            this.lblLaunchMode.Location = new System.Drawing.Point(15, 68);
            this.lblLaunchMode.Name = "lblLaunchMode";
            this.lblLaunchMode.Size = new System.Drawing.Size(75, 13);
            this.lblLaunchMode.TabIndex = 2;
            this.lblLaunchMode.Text = "Launch mode:";
            // 
            // cboLaunchMode
            // 
            this.cboLaunchMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLaunchMode.FormattingEnabled = true;
            this.cboLaunchMode.Items.AddRange(new object[] {
            "embedded view",
            "external program"});
            this.cboLaunchMode.Location = new System.Drawing.Point(126, 65);
            this.cboLaunchMode.Name = "cboLaunchMode";
            this.cboLaunchMode.Size = new System.Drawing.Size(182, 21);
            this.cboLaunchMode.TabIndex = 3;
            // 
            // MainDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 295);
            this.Controls.Add(this.grpClientSettings);
            this.Controls.Add(this.grpLogonSettings);
            this.Controls.Add(this.btnConnect);
            this.Name = "MainDlg";
            this.Text = "Remote Desktop Client";
            this.grpLogonSettings.ResumeLayout(false);
            this.grpLogonSettings.PerformLayout();
            this.grpClientSettings.ResumeLayout(false);
            this.grpClientSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblComputer;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox grpLogonSettings;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtComputer;
        private System.Windows.Forms.GroupBox grpClientSettings;
        private System.Windows.Forms.ComboBox cboRdpClient;
        private System.Windows.Forms.Label lblRdpClient;
        private System.Windows.Forms.ComboBox cboLaunchMode;
        private System.Windows.Forms.Label lblLaunchMode;
    }
}
