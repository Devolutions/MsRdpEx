
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
            btnConnect = new System.Windows.Forms.Button();
            lblComputer = new System.Windows.Forms.Label();
            lblUserName = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            grpLogonSettings = new System.Windows.Forms.GroupBox();
            txtPassword = new System.Windows.Forms.TextBox();
            txtUserName = new System.Windows.Forms.TextBox();
            txtComputer = new System.Windows.Forms.TextBox();
            grpClientSettings = new System.Windows.Forms.GroupBox();
            cboLaunchMode = new System.Windows.Forms.ComboBox();
            lblLaunchMode = new System.Windows.Forms.Label();
            cboRdpClient = new System.Windows.Forms.ComboBox();
            lblRdpClient = new System.Windows.Forms.Label();
            grpLogonSettings.SuspendLayout();
            grpClientSettings.SuspendLayout();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new System.Drawing.Point(462, 500);
            btnConnect.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(125, 44);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblComputer
            // 
            lblComputer.AutoSize = true;
            lblComputer.Location = new System.Drawing.Point(27, 42);
            lblComputer.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblComputer.Name = "lblComputer";
            lblComputer.Size = new System.Drawing.Size(96, 25);
            lblComputer.TabIndex = 3;
            lblComputer.Text = "Computer:";
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Location = new System.Drawing.Point(27, 94);
            lblUserName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new System.Drawing.Size(100, 25);
            lblUserName.TabIndex = 4;
            lblUserName.Text = "User name:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(27, 146);
            lblPassword.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(91, 25);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password:";
            // 
            // grpLogonSettings
            // 
            grpLogonSettings.Controls.Add(txtPassword);
            grpLogonSettings.Controls.Add(txtUserName);
            grpLogonSettings.Controls.Add(txtComputer);
            grpLogonSettings.Controls.Add(lblComputer);
            grpLogonSettings.Controls.Add(lblPassword);
            grpLogonSettings.Controls.Add(lblUserName);
            grpLogonSettings.Location = new System.Drawing.Point(20, 23);
            grpLogonSettings.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            grpLogonSettings.Name = "grpLogonSettings";
            grpLogonSettings.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            grpLogonSettings.Size = new System.Drawing.Size(567, 219);
            grpLogonSettings.TabIndex = 6;
            grpLogonSettings.TabStop = false;
            grpLogonSettings.Text = "Logon settings";
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(212, 140);
            txtPassword.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            txtPassword.MaxLength = 128;
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(301, 31);
            txtPassword.TabIndex = 8;
            txtPassword.Text = "vmbox";
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            txtUserName.Location = new System.Drawing.Point(212, 88);
            txtUserName.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            txtUserName.MaxLength = 128;
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new System.Drawing.Size(301, 31);
            txtUserName.TabIndex = 7;
            txtUserName.Text = "vmbox";
            // 
            // txtComputer
            // 
            txtComputer.Location = new System.Drawing.Point(212, 37);
            txtComputer.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            txtComputer.MaxLength = 128;
            txtComputer.Name = "txtComputer";
            txtComputer.Size = new System.Drawing.Size(301, 31);
            txtComputer.TabIndex = 6;
            txtComputer.Text = "192.168.1.103";
            // 
            // grpClientSettings
            // 
            grpClientSettings.Controls.Add(cboLaunchMode);
            grpClientSettings.Controls.Add(lblLaunchMode);
            grpClientSettings.Controls.Add(cboRdpClient);
            grpClientSettings.Controls.Add(lblRdpClient);
            grpClientSettings.Location = new System.Drawing.Point(22, 256);
            grpClientSettings.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            grpClientSettings.Name = "grpClientSettings";
            grpClientSettings.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            grpClientSettings.Size = new System.Drawing.Size(565, 233);
            grpClientSettings.TabIndex = 7;
            grpClientSettings.TabStop = false;
            grpClientSettings.Text = "RDP Client Settings";
            // 
            // cboLaunchMode
            // 
            cboLaunchMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboLaunchMode.FormattingEnabled = true;
            cboLaunchMode.Items.AddRange(new object[] { "embedded view", "external program" });
            cboLaunchMode.Location = new System.Drawing.Point(210, 125);
            cboLaunchMode.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            cboLaunchMode.Name = "cboLaunchMode";
            cboLaunchMode.Size = new System.Drawing.Size(301, 33);
            cboLaunchMode.TabIndex = 3;
            // 
            // lblLaunchMode
            // 
            lblLaunchMode.AutoSize = true;
            lblLaunchMode.Location = new System.Drawing.Point(25, 131);
            lblLaunchMode.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblLaunchMode.Name = "lblLaunchMode";
            lblLaunchMode.Size = new System.Drawing.Size(123, 25);
            lblLaunchMode.TabIndex = 2;
            lblLaunchMode.Text = "Launch mode:";
            // 
            // cboRdpClient
            // 
            cboRdpClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboRdpClient.FormattingEnabled = true;
            cboRdpClient.Items.AddRange(new object[] { "mstsc", "msrdc" });
            cboRdpClient.Location = new System.Drawing.Point(210, 60);
            cboRdpClient.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            cboRdpClient.Name = "cboRdpClient";
            cboRdpClient.Size = new System.Drawing.Size(301, 33);
            cboRdpClient.TabIndex = 1;
            // 
            // lblRdpClient
            // 
            lblRdpClient.AutoSize = true;
            lblRdpClient.Location = new System.Drawing.Point(25, 65);
            lblRdpClient.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblRdpClient.Name = "lblRdpClient";
            lblRdpClient.Size = new System.Drawing.Size(118, 25);
            lblRdpClient.TabIndex = 0;
            lblRdpClient.Text = "Client Engine:";
            // 
            // MainDlg
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(607, 567);
            Controls.Add(grpClientSettings);
            Controls.Add(grpLogonSettings);
            Controls.Add(btnConnect);
            Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            Name = "MainDlg";
            Text = "Remote Desktop Client";
            grpLogonSettings.ResumeLayout(false);
            grpLogonSettings.PerformLayout();
            grpClientSettings.ResumeLayout(false);
            grpClientSettings.PerformLayout();
            ResumeLayout(false);
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
