namespace MsRdpEx_App
{
    partial class DvcDialog
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
            dvcTabs = new System.Windows.Forms.TabControl();
            infoPage = new System.Windows.Forms.TabPage();
            capabilitiesGrid = new System.Windows.Forms.PropertyGrid();
            execRunPage = new System.Windows.Forms.TabPage();
            panel1 = new System.Windows.Forms.Panel();
            pwshRadioButton = new System.Windows.Forms.RadioButton();
            powershellRadioButton = new System.Windows.Forms.RadioButton();
            processRadioButton = new System.Windows.Forms.RadioButton();
            batchRadioButton = new System.Windows.Forms.RadioButton();
            runRadioButton = new System.Windows.Forms.RadioButton();
            label5 = new System.Windows.Forms.Label();
            directoryInput = new System.Windows.Forms.TextBox();
            directoryLabel = new System.Windows.Forms.Label();
            cmdLabel = new System.Windows.Forms.Label();
            cmdInput = new System.Windows.Forms.TextBox();
            execRunButton = new System.Windows.Forms.Button();
            fileInput = new System.Windows.Forms.TextBox();
            fileLabel = new System.Windows.Forms.Label();
            sessionTab = new System.Windows.Forms.TabPage();
            shutdownForceFlag = new System.Windows.Forms.CheckBox();
            shutdownRebootFlag = new System.Windows.Forms.CheckBox();
            lockButton = new System.Windows.Forms.Button();
            shutdownButton = new System.Windows.Forms.Button();
            logoffButton = new System.Windows.Forms.Button();
            msgBoxTimeoutFlag = new System.Windows.Forms.CheckBox();
            msgBoxTimeoutInput = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            msgBoxResponseText = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            msgBoxShowButton = new System.Windows.Forms.Button();
            msgBoxResponseFlag = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            msgBoxKindSelect = new System.Windows.Forms.ComboBox();
            msgBoxMessageInput = new System.Windows.Forms.TextBox();
            msgBoxTitleInput = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            shutdownTimeoutFlag = new System.Windows.Forms.CheckBox();
            shutdownTimeoutInput = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            shutdownMessageInput = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            dvcTabs.SuspendLayout();
            infoPage.SuspendLayout();
            execRunPage.SuspendLayout();
            panel1.SuspendLayout();
            sessionTab.SuspendLayout();
            SuspendLayout();
            // 
            // dvcTabs
            // 
            dvcTabs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dvcTabs.Controls.Add(infoPage);
            dvcTabs.Controls.Add(execRunPage);
            dvcTabs.Controls.Add(sessionTab);
            dvcTabs.Location = new System.Drawing.Point(13, 15);
            dvcTabs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dvcTabs.Name = "dvcTabs";
            dvcTabs.SelectedIndex = 0;
            dvcTabs.Size = new System.Drawing.Size(595, 669);
            dvcTabs.TabIndex = 0;
            // 
            // infoPage
            // 
            infoPage.Controls.Add(capabilitiesGrid);
            infoPage.Location = new System.Drawing.Point(4, 34);
            infoPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            infoPage.Name = "infoPage";
            infoPage.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            infoPage.Size = new System.Drawing.Size(587, 631);
            infoPage.TabIndex = 0;
            infoPage.Text = "Info";
            infoPage.UseVisualStyleBackColor = true;
            // 
            // capabilitiesGrid
            // 
            capabilitiesGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            capabilitiesGrid.Location = new System.Drawing.Point(7, 8);
            capabilitiesGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            capabilitiesGrid.Name = "capabilitiesGrid";
            capabilitiesGrid.Size = new System.Drawing.Size(566, 610);
            capabilitiesGrid.TabIndex = 0;
            // 
            // execRunPage
            // 
            execRunPage.Controls.Add(panel1);
            execRunPage.Controls.Add(label5);
            execRunPage.Controls.Add(directoryInput);
            execRunPage.Controls.Add(directoryLabel);
            execRunPage.Controls.Add(cmdLabel);
            execRunPage.Controls.Add(cmdInput);
            execRunPage.Controls.Add(execRunButton);
            execRunPage.Controls.Add(fileInput);
            execRunPage.Controls.Add(fileLabel);
            execRunPage.Location = new System.Drawing.Point(4, 34);
            execRunPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            execRunPage.Name = "execRunPage";
            execRunPage.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            execRunPage.Size = new System.Drawing.Size(587, 631);
            execRunPage.TabIndex = 1;
            execRunPage.Text = "Run";
            execRunPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(pwshRadioButton);
            panel1.Controls.Add(powershellRadioButton);
            panel1.Controls.Add(processRadioButton);
            panel1.Controls.Add(batchRadioButton);
            panel1.Controls.Add(runRadioButton);
            panel1.Location = new System.Drawing.Point(72, 28);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(529, 193);
            panel1.TabIndex = 12;
            // 
            // pwshRadioButton
            // 
            pwshRadioButton.AutoSize = true;
            pwshRadioButton.Location = new System.Drawing.Point(17, 154);
            pwshRadioButton.Name = "pwshRadioButton";
            pwshRadioButton.Size = new System.Drawing.Size(231, 29);
            pwshRadioButton.TabIndex = 4;
            pwshRadioButton.Text = "PowerShell 7.x(pwsh.exe)";
            pwshRadioButton.UseVisualStyleBackColor = true;
            pwshRadioButton.CheckedChanged += runRadioButton_CheckedChanged;
            // 
            // powershellRadioButton
            // 
            powershellRadioButton.AutoSize = true;
            powershellRadioButton.Location = new System.Drawing.Point(17, 119);
            powershellRadioButton.Name = "powershellRadioButton";
            powershellRadioButton.Size = new System.Drawing.Size(274, 29);
            powershellRadioButton.TabIndex = 3;
            powershellRadioButton.Text = "PowerShell 5.x(powershell.exe)";
            powershellRadioButton.UseVisualStyleBackColor = true;
            powershellRadioButton.CheckedChanged += runRadioButton_CheckedChanged;
            // 
            // processRadioButton
            // 
            processRadioButton.AutoSize = true;
            processRadioButton.Location = new System.Drawing.Point(17, 49);
            processRadioButton.Name = "processRadioButton";
            processRadioButton.Size = new System.Drawing.Size(234, 29);
            processRadioButton.TabIndex = 2;
            processRadioButton.Text = "Process(CreateProcessW)";
            processRadioButton.UseVisualStyleBackColor = true;
            processRadioButton.CheckedChanged += runRadioButton_CheckedChanged;
            // 
            // batchRadioButton
            // 
            batchRadioButton.AutoSize = true;
            batchRadioButton.Location = new System.Drawing.Point(17, 84);
            batchRadioButton.Name = "batchRadioButton";
            batchRadioButton.Size = new System.Drawing.Size(155, 29);
            batchRadioButton.TabIndex = 1;
            batchRadioButton.Text = "Batch(cmd.exe)";
            batchRadioButton.UseVisualStyleBackColor = true;
            batchRadioButton.CheckedChanged += runRadioButton_CheckedChanged;
            // 
            // runRadioButton
            // 
            runRadioButton.AutoSize = true;
            runRadioButton.Checked = true;
            runRadioButton.Location = new System.Drawing.Point(17, 14);
            runRadioButton.Name = "runRadioButton";
            runRadioButton.Size = new System.Drawing.Size(174, 29);
            runRadioButton.TabIndex = 0;
            runRadioButton.TabStop = true;
            runRadioButton.Text = "Run(ShellExecute)";
            runRadioButton.UseVisualStyleBackColor = true;
            runRadioButton.CheckedChanged += runRadioButton_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(15, 28);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(51, 25);
            label5.TabIndex = 11;
            label5.Text = "Kind:";
            // 
            // directoryInput
            // 
            directoryInput.Location = new System.Drawing.Point(208, 507);
            directoryInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            directoryInput.Name = "directoryInput";
            directoryInput.Size = new System.Drawing.Size(393, 31);
            directoryInput.TabIndex = 6;
            // 
            // directoryLabel
            // 
            directoryLabel.AutoSize = true;
            directoryLabel.Location = new System.Drawing.Point(34, 507);
            directoryLabel.Name = "directoryLabel";
            directoryLabel.Size = new System.Drawing.Size(158, 25);
            directoryLabel.TabIndex = 5;
            directoryLabel.Text = "Working directory:";
            // 
            // cmdLabel
            // 
            cmdLabel.AutoSize = true;
            cmdLabel.Location = new System.Drawing.Point(30, 282);
            cmdLabel.Name = "cmdLabel";
            cmdLabel.Size = new System.Drawing.Size(33, 25);
            cmdLabel.TabIndex = 4;
            cmdLabel.Text = "---";
            // 
            // cmdInput
            // 
            cmdInput.Enabled = false;
            cmdInput.Location = new System.Drawing.Point(208, 279);
            cmdInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            cmdInput.Multiline = true;
            cmdInput.Name = "cmdInput";
            cmdInput.ReadOnly = true;
            cmdInput.Size = new System.Drawing.Size(393, 217);
            cmdInput.TabIndex = 3;
            // 
            // execRunButton
            // 
            execRunButton.Location = new System.Drawing.Point(34, 557);
            execRunButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            execRunButton.Name = "execRunButton";
            execRunButton.Size = new System.Drawing.Size(567, 58);
            execRunButton.TabIndex = 2;
            execRunButton.Text = "RUN";
            execRunButton.UseVisualStyleBackColor = true;
            execRunButton.Click += execRunButton_Click;
            // 
            // fileInput
            // 
            fileInput.Location = new System.Drawing.Point(208, 243);
            fileInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            fileInput.Name = "fileInput";
            fileInput.Size = new System.Drawing.Size(393, 31);
            fileInput.TabIndex = 1;
            // 
            // fileLabel
            // 
            fileLabel.AutoSize = true;
            fileLabel.Location = new System.Drawing.Point(30, 246);
            fileLabel.Name = "fileLabel";
            fileLabel.Size = new System.Drawing.Size(100, 25);
            fileLabel.TabIndex = 0;
            fileLabel.Text = "Command:";
            // 
            // sessionTab
            // 
            sessionTab.Controls.Add(shutdownTimeoutFlag);
            sessionTab.Controls.Add(shutdownTimeoutInput);
            sessionTab.Controls.Add(label6);
            sessionTab.Controls.Add(shutdownMessageInput);
            sessionTab.Controls.Add(label8);
            sessionTab.Controls.Add(shutdownForceFlag);
            sessionTab.Controls.Add(shutdownRebootFlag);
            sessionTab.Controls.Add(lockButton);
            sessionTab.Controls.Add(shutdownButton);
            sessionTab.Controls.Add(logoffButton);
            sessionTab.Controls.Add(msgBoxTimeoutFlag);
            sessionTab.Controls.Add(msgBoxTimeoutInput);
            sessionTab.Controls.Add(label7);
            sessionTab.Controls.Add(msgBoxResponseText);
            sessionTab.Controls.Add(label4);
            sessionTab.Controls.Add(msgBoxShowButton);
            sessionTab.Controls.Add(msgBoxResponseFlag);
            sessionTab.Controls.Add(label3);
            sessionTab.Controls.Add(msgBoxKindSelect);
            sessionTab.Controls.Add(msgBoxMessageInput);
            sessionTab.Controls.Add(msgBoxTitleInput);
            sessionTab.Controls.Add(label2);
            sessionTab.Controls.Add(label1);
            sessionTab.Location = new System.Drawing.Point(4, 34);
            sessionTab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            sessionTab.Name = "sessionTab";
            sessionTab.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            sessionTab.Size = new System.Drawing.Size(587, 631);
            sessionTab.TabIndex = 2;
            sessionTab.Text = "MsgBox/Session";
            sessionTab.UseVisualStyleBackColor = true;
            // 
            // shutdownForceFlag
            // 
            shutdownForceFlag.AutoSize = true;
            shutdownForceFlag.Location = new System.Drawing.Point(214, 485);
            shutdownForceFlag.Name = "shutdownForceFlag";
            shutdownForceFlag.Size = new System.Drawing.Size(81, 29);
            shutdownForceFlag.TabIndex = 18;
            shutdownForceFlag.Text = "Force";
            shutdownForceFlag.UseVisualStyleBackColor = true;
            // 
            // shutdownRebootFlag
            // 
            shutdownRebootFlag.AutoSize = true;
            shutdownRebootFlag.Location = new System.Drawing.Point(301, 485);
            shutdownRebootFlag.Name = "shutdownRebootFlag";
            shutdownRebootFlag.Size = new System.Drawing.Size(96, 29);
            shutdownRebootFlag.TabIndex = 17;
            shutdownRebootFlag.Text = "Reboot";
            shutdownRebootFlag.UseVisualStyleBackColor = true;
            // 
            // lockButton
            // 
            lockButton.Location = new System.Drawing.Point(17, 328);
            lockButton.Name = "lockButton";
            lockButton.Size = new System.Drawing.Size(191, 62);
            lockButton.TabIndex = 16;
            lockButton.Text = "Lock";
            lockButton.UseVisualStyleBackColor = true;
            lockButton.Click += lockButton_Click;
            // 
            // shutdownButton
            // 
            shutdownButton.Location = new System.Drawing.Point(17, 485);
            shutdownButton.Name = "shutdownButton";
            shutdownButton.Size = new System.Drawing.Size(191, 130);
            shutdownButton.TabIndex = 15;
            shutdownButton.Text = "Shutdown";
            shutdownButton.UseVisualStyleBackColor = true;
            shutdownButton.Click += shutdownButton_Click;
            // 
            // logoffButton
            // 
            logoffButton.Location = new System.Drawing.Point(214, 328);
            logoffButton.Name = "logoffButton";
            logoffButton.Size = new System.Drawing.Size(191, 62);
            logoffButton.TabIndex = 14;
            logoffButton.Text = "Logoff";
            logoffButton.UseVisualStyleBackColor = true;
            logoffButton.Click += logoffButton_Click;
            // 
            // msgBoxTimeoutFlag
            // 
            msgBoxTimeoutFlag.AutoSize = true;
            msgBoxTimeoutFlag.Location = new System.Drawing.Point(17, 132);
            msgBoxTimeoutFlag.Name = "msgBoxTimeoutFlag";
            msgBoxTimeoutFlag.Size = new System.Drawing.Size(103, 29);
            msgBoxTimeoutFlag.TabIndex = 13;
            msgBoxTimeoutFlag.Text = "Timeout";
            msgBoxTimeoutFlag.UseVisualStyleBackColor = true;
            // 
            // msgBoxTimeoutInput
            // 
            msgBoxTimeoutInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            msgBoxTimeoutInput.Location = new System.Drawing.Point(126, 129);
            msgBoxTimeoutInput.Name = "msgBoxTimeoutInput";
            msgBoxTimeoutInput.Size = new System.Drawing.Size(357, 31);
            msgBoxTimeoutInput.TabIndex = 12;
            msgBoxTimeoutInput.Text = "5";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(489, 132);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(77, 25);
            label7.TabIndex = 11;
            label7.Text = "seconds";
            // 
            // msgBoxResponseText
            // 
            msgBoxResponseText.AutoSize = true;
            msgBoxResponseText.Location = new System.Drawing.Point(147, 239);
            msgBoxResponseText.Name = "msgBoxResponseText";
            msgBoxResponseText.Size = new System.Drawing.Size(33, 25);
            msgBoxResponseText.TabIndex = 9;
            msgBoxResponseText.Text = "---";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(17, 239);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(124, 25);
            label4.TabIndex = 8;
            label4.Text = "Last response:";
            // 
            // msgBoxShowButton
            // 
            msgBoxShowButton.Location = new System.Drawing.Point(201, 190);
            msgBoxShowButton.Name = "msgBoxShowButton";
            msgBoxShowButton.Size = new System.Drawing.Size(365, 34);
            msgBoxShowButton.TabIndex = 7;
            msgBoxShowButton.Text = "Show";
            msgBoxShowButton.UseVisualStyleBackColor = true;
            msgBoxShowButton.Click += msgBoxShowButton_Click;
            // 
            // msgBoxResponseFlag
            // 
            msgBoxResponseFlag.AutoSize = true;
            msgBoxResponseFlag.Location = new System.Drawing.Point(17, 190);
            msgBoxResponseFlag.Name = "msgBoxResponseFlag";
            msgBoxResponseFlag.Size = new System.Drawing.Size(178, 29);
            msgBoxResponseFlag.TabIndex = 6;
            msgBoxResponseFlag.Text = "Wait for response";
            msgBoxResponseFlag.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(17, 90);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(51, 25);
            label3.TabIndex = 5;
            label3.Text = "Kind:";
            // 
            // msgBoxKindSelect
            // 
            msgBoxKindSelect.FormattingEnabled = true;
            msgBoxKindSelect.Items.AddRange(new object[] { "Default", "Ok", "OkCancel", "AbortRetryIgnore", "YesNoCancel", "YesNo", "RetryCancel", "CancelTryContinue", "Help" });
            msgBoxKindSelect.Location = new System.Drawing.Point(109, 87);
            msgBoxKindSelect.Name = "msgBoxKindSelect";
            msgBoxKindSelect.Size = new System.Drawing.Size(457, 33);
            msgBoxKindSelect.TabIndex = 4;
            // 
            // msgBoxMessageInput
            // 
            msgBoxMessageInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            msgBoxMessageInput.Location = new System.Drawing.Point(109, 50);
            msgBoxMessageInput.Name = "msgBoxMessageInput";
            msgBoxMessageInput.Size = new System.Drawing.Size(457, 31);
            msgBoxMessageInput.TabIndex = 3;
            // 
            // msgBoxTitleInput
            // 
            msgBoxTitleInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            msgBoxTitleInput.Location = new System.Drawing.Point(109, 13);
            msgBoxTitleInput.Name = "msgBoxTitleInput";
            msgBoxTitleInput.Size = new System.Drawing.Size(457, 31);
            msgBoxTitleInput.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(17, 50);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(86, 25);
            label2.TabIndex = 1;
            label2.Text = "Message:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(17, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 25);
            label1.TabIndex = 0;
            label1.Text = "Title:";
            // 
            // shutdownTimeoutFlag
            // 
            shutdownTimeoutFlag.AutoSize = true;
            shutdownTimeoutFlag.Location = new System.Drawing.Point(214, 520);
            shutdownTimeoutFlag.Name = "shutdownTimeoutFlag";
            shutdownTimeoutFlag.Size = new System.Drawing.Size(103, 29);
            shutdownTimeoutFlag.TabIndex = 23;
            shutdownTimeoutFlag.Text = "Timeout";
            shutdownTimeoutFlag.UseVisualStyleBackColor = true;
            // 
            // shutdownTimeoutInput
            // 
            shutdownTimeoutInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            shutdownTimeoutInput.Location = new System.Drawing.Point(323, 518);
            shutdownTimeoutInput.Name = "shutdownTimeoutInput";
            shutdownTimeoutInput.Size = new System.Drawing.Size(160, 31);
            shutdownTimeoutInput.TabIndex = 22;
            shutdownTimeoutInput.Text = "5";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(489, 524);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(77, 25);
            label6.TabIndex = 21;
            label6.Text = "seconds";
            // 
            // shutdownMessageInput
            // 
            shutdownMessageInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            shutdownMessageInput.Location = new System.Drawing.Point(306, 562);
            shutdownMessageInput.Name = "shutdownMessageInput";
            shutdownMessageInput.Size = new System.Drawing.Size(260, 31);
            shutdownMessageInput.TabIndex = 20;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(214, 562);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(86, 25);
            label8.TabIndex = 19;
            label8.Text = "Message:";
            // 
            // DvcDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(622, 699);
            Controls.Add(dvcTabs);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "DvcDialog";
            Text = "DvcDialog";
            dvcTabs.ResumeLayout(false);
            infoPage.ResumeLayout(false);
            execRunPage.ResumeLayout(false);
            execRunPage.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            sessionTab.ResumeLayout(false);
            sessionTab.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl dvcTabs;
        private System.Windows.Forms.TabPage infoPage;
        private System.Windows.Forms.TabPage execRunPage;
        private System.Windows.Forms.PropertyGrid capabilitiesGrid;
        private System.Windows.Forms.Button execRunButton;
        private System.Windows.Forms.TextBox fileInput;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.TabPage sessionTab;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox directoryInput;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.Label cmdLabel;
        private System.Windows.Forms.TextBox cmdInput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton pwshRadioButton;
        private System.Windows.Forms.RadioButton powershellRadioButton;
        private System.Windows.Forms.RadioButton processRadioButton;
        private System.Windows.Forms.RadioButton batchRadioButton;
        private System.Windows.Forms.RadioButton runRadioButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox msgBoxKindSelect;
        private System.Windows.Forms.TextBox msgBoxMessageInput;
        private System.Windows.Forms.TextBox msgBoxTitleInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label msgBoxResponseText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button msgBoxShowButton;
        private System.Windows.Forms.CheckBox msgBoxResponseFlag;
        private System.Windows.Forms.CheckBox msgBoxTimeoutFlag;
        private System.Windows.Forms.TextBox msgBoxTimeoutInput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button lockButton;
        private System.Windows.Forms.Button shutdownButton;
        private System.Windows.Forms.Button logoffButton;
        private System.Windows.Forms.CheckBox shutdownForceFlag;
        private System.Windows.Forms.CheckBox shutdownRebootFlag;
        private System.Windows.Forms.CheckBox shutdownTimeoutFlag;
        private System.Windows.Forms.TextBox shutdownTimeoutInput;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox shutdownMessageInput;
        private System.Windows.Forms.Label label8;
    }
}