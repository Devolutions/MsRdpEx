
namespace MsRdpEx
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
            this.grpLogonSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(277, 140);
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
            this.grpLogonSettings.Size = new System.Drawing.Size(340, 122);
            this.grpLogonSettings.TabIndex = 6;
            this.grpLogonSettings.TabStop = false;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(103, 73);
            this.txtPassword.MaxLength = 128;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(206, 20);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(103, 46);
            this.txtUserName.MaxLength = 128;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(206, 20);
            this.txtUserName.TabIndex = 7;
            // 
            // txtComputer
            // 
            this.txtComputer.Location = new System.Drawing.Point(103, 19);
            this.txtComputer.MaxLength = 128;
            this.txtComputer.Name = "txtComputer";
            this.txtComputer.Size = new System.Drawing.Size(206, 20);
            this.txtComputer.TabIndex = 6;
            // 
            // MainDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 171);
            this.Controls.Add(this.grpLogonSettings);
            this.Controls.Add(this.btnConnect);
            this.Name = "MainDlg";
            this.Text = "Remote Desktop Client";
            this.grpLogonSettings.ResumeLayout(false);
            this.grpLogonSettings.PerformLayout();
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
    }
}

