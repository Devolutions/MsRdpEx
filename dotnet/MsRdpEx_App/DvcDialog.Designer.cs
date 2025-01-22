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
            this.dvcTabs = new System.Windows.Forms.TabControl();
            this.infoPage = new System.Windows.Forms.TabPage();
            this.capabilitiesGrid = new System.Windows.Forms.PropertyGrid();
            this.execRunPage = new System.Windows.Forms.TabPage();
            this.execRunButton = new System.Windows.Forms.Button();
            this.execRunCommandTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.messageBoxTab = new System.Windows.Forms.TabPage();
            this.sessionTab = new System.Windows.Forms.TabPage();
            this.execArgsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.execDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.stdinTextBox = new System.Windows.Forms.TextBox();
            this.sendStdinButton = new System.Windows.Forms.Button();
            this.execKindComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.stdoutTextBox = new System.Windows.Forms.TextBox();
            this.stderrTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.execLogTextBox = new System.Windows.Forms.TextBox();
            this.sendEofCheckbox = new System.Windows.Forms.CheckBox();
            this.abortButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.execSendCrlfCheckBox = new System.Windows.Forms.CheckBox();
            this.dvcTabs.SuspendLayout();
            this.infoPage.SuspendLayout();
            this.execRunPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // dvcTabs
            // 
            this.dvcTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dvcTabs.Controls.Add(this.infoPage);
            this.dvcTabs.Controls.Add(this.execRunPage);
            this.dvcTabs.Controls.Add(this.messageBoxTab);
            this.dvcTabs.Controls.Add(this.sessionTab);
            this.dvcTabs.Location = new System.Drawing.Point(12, 12);
            this.dvcTabs.Name = "dvcTabs";
            this.dvcTabs.SelectedIndex = 0;
            this.dvcTabs.Size = new System.Drawing.Size(1074, 800);
            this.dvcTabs.TabIndex = 0;
            // 
            // infoPage
            // 
            this.infoPage.Controls.Add(this.capabilitiesGrid);
            this.infoPage.Location = new System.Drawing.Point(4, 29);
            this.infoPage.Name = "infoPage";
            this.infoPage.Padding = new System.Windows.Forms.Padding(3);
            this.infoPage.Size = new System.Drawing.Size(1066, 767);
            this.infoPage.TabIndex = 0;
            this.infoPage.Text = "Info";
            this.infoPage.UseVisualStyleBackColor = true;
            // 
            // capabilitiesGrid
            // 
            this.capabilitiesGrid.Location = new System.Drawing.Point(6, 6);
            this.capabilitiesGrid.Name = "capabilitiesGrid";
            this.capabilitiesGrid.Size = new System.Drawing.Size(1054, 743);
            this.capabilitiesGrid.TabIndex = 0;
            // 
            // execRunPage
            // 
            this.execRunPage.Controls.Add(this.execSendCrlfCheckBox);
            this.execRunPage.Controls.Add(this.cancelButton);
            this.execRunPage.Controls.Add(this.abortButton);
            this.execRunPage.Controls.Add(this.sendEofCheckbox);
            this.execRunPage.Controls.Add(this.execLogTextBox);
            this.execRunPage.Controls.Add(this.label8);
            this.execRunPage.Controls.Add(this.stderrTextBox);
            this.execRunPage.Controls.Add(this.stdoutTextBox);
            this.execRunPage.Controls.Add(this.label7);
            this.execRunPage.Controls.Add(this.label6);
            this.execRunPage.Controls.Add(this.label5);
            this.execRunPage.Controls.Add(this.execKindComboBox);
            this.execRunPage.Controls.Add(this.sendStdinButton);
            this.execRunPage.Controls.Add(this.stdinTextBox);
            this.execRunPage.Controls.Add(this.label4);
            this.execRunPage.Controls.Add(this.execDirectoryTextBox);
            this.execRunPage.Controls.Add(this.label3);
            this.execRunPage.Controls.Add(this.label2);
            this.execRunPage.Controls.Add(this.execArgsTextBox);
            this.execRunPage.Controls.Add(this.execRunButton);
            this.execRunPage.Controls.Add(this.execRunCommandTextBox);
            this.execRunPage.Controls.Add(this.label1);
            this.execRunPage.Location = new System.Drawing.Point(4, 29);
            this.execRunPage.Name = "execRunPage";
            this.execRunPage.Padding = new System.Windows.Forms.Padding(3);
            this.execRunPage.Size = new System.Drawing.Size(1066, 767);
            this.execRunPage.TabIndex = 1;
            this.execRunPage.Text = "Run";
            this.execRunPage.UseVisualStyleBackColor = true;
            this.execRunPage.Click += new System.EventHandler(this.execRunPage_Click);
            // 
            // execRunButton
            // 
            this.execRunButton.Location = new System.Drawing.Point(31, 587);
            this.execRunButton.Name = "execRunButton";
            this.execRunButton.Size = new System.Drawing.Size(471, 153);
            this.execRunButton.TabIndex = 2;
            this.execRunButton.Text = "RUN";
            this.execRunButton.UseVisualStyleBackColor = true;
             this.execRunButton.Click += new System.EventHandler(this.execRunButton_Click);
            // 
            // execRunCommandTextBox
            // 
            this.execRunCommandTextBox.Location = new System.Drawing.Point(148, 69);
            this.execRunCommandTextBox.Name = "execRunCommandTextBox";
            this.execRunCommandTextBox.Size = new System.Drawing.Size(354, 26);
            this.execRunCommandTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Command/File:";
            // 
            // messageBoxTab
            // 
            this.messageBoxTab.Location = new System.Drawing.Point(4, 29);
            this.messageBoxTab.Name = "messageBoxTab";
            this.messageBoxTab.Padding = new System.Windows.Forms.Padding(3);
            this.messageBoxTab.Size = new System.Drawing.Size(1066, 767);
            this.messageBoxTab.TabIndex = 2;
            this.messageBoxTab.Text = "Message Box";
            this.messageBoxTab.UseVisualStyleBackColor = true;
            // 
            // sessionTab
            // 
            this.sessionTab.Location = new System.Drawing.Point(4, 29);
            this.sessionTab.Name = "sessionTab";
            this.sessionTab.Padding = new System.Windows.Forms.Padding(3);
            this.sessionTab.Size = new System.Drawing.Size(1066, 767);
            this.sessionTab.TabIndex = 3;
            this.sessionTab.Text = "Session";
            this.sessionTab.UseVisualStyleBackColor = true;
            // 
            // execArgsTextBox
            // 
            this.execArgsTextBox.Location = new System.Drawing.Point(148, 103);
            this.execArgsTextBox.Name = "execArgsTextBox";
            this.execArgsTextBox.Size = new System.Drawing.Size(354, 26);
            this.execArgsTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Args:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Directory:";
            // 
            // execDirectoryTextBox
            // 
            this.execDirectoryTextBox.Location = new System.Drawing.Point(148, 145);
            this.execDirectoryTextBox.Name = "execDirectoryTextBox";
            this.execDirectoryTextBox.Size = new System.Drawing.Size(354, 26);
            this.execDirectoryTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Stdin:";
            // 
            // stdinTextBox
            // 
            this.stdinTextBox.Location = new System.Drawing.Point(31, 224);
            this.stdinTextBox.Multiline = true;
            this.stdinTextBox.Name = "stdinTextBox";
            this.stdinTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stdinTextBox.Size = new System.Drawing.Size(342, 337);
            this.stdinTextBox.TabIndex = 8;
            // 
            // sendStdinButton
            // 
            this.sendStdinButton.Location = new System.Drawing.Point(379, 286);
            this.sendStdinButton.Name = "sendStdinButton";
            this.sendStdinButton.Size = new System.Drawing.Size(111, 275);
            this.sendStdinButton.TabIndex = 9;
            this.sendStdinButton.Text = "Send";
            this.sendStdinButton.UseVisualStyleBackColor = true;
             this.sendStdinButton.Click += new System.EventHandler(this.sendStdinButton_Click);
            // 
            // execKindComboBox
            // 
            this.execKindComboBox.FormattingEnabled = true;
            this.execKindComboBox.Items.AddRange(new object[] {
            "ShellExecute",
            "Process",
            "Batch",
            "PowerShell (5.x)",
            "Pwsh (7.x)"});
            this.execKindComboBox.Location = new System.Drawing.Point(148, 26);
            this.execKindComboBox.Name = "execKindComboBox";
            this.execKindComboBox.Size = new System.Drawing.Size(354, 28);
            this.execKindComboBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Kind:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(508, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Stdout:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(512, 224);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "Stderr:";
            // 
            // stdoutTextBox
            // 
            this.stdoutTextBox.Location = new System.Drawing.Point(512, 46);
            this.stdoutTextBox.Multiline = true;
            this.stdoutTextBox.Name = "stdoutTextBox";
            this.stdoutTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stdoutTextBox.Size = new System.Drawing.Size(530, 168);
            this.stdoutTextBox.TabIndex = 14;
            // 
            // stderrTextBox
            // 
            this.stderrTextBox.Location = new System.Drawing.Point(516, 247);
            this.stderrTextBox.Multiline = true;
            this.stderrTextBox.Name = "stderrTextBox";
            this.stderrTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stderrTextBox.Size = new System.Drawing.Size(530, 175);
            this.stderrTextBox.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(512, 437);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 20);
            this.label8.TabIndex = 16;
            this.label8.Text = "Exec log:";
            // 
            // execLogTextBox
            // 
            this.execLogTextBox.Location = new System.Drawing.Point(516, 460);
            this.execLogTextBox.Multiline = true;
            this.execLogTextBox.Name = "execLogTextBox";
            this.execLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.execLogTextBox.Size = new System.Drawing.Size(530, 175);
            this.execLogTextBox.TabIndex = 17;
            // 
            // sendEofCheckbox
            // 
            this.sendEofCheckbox.AutoSize = true;
            this.sendEofCheckbox.Location = new System.Drawing.Point(380, 224);
            this.sendEofCheckbox.Name = "sendEofCheckbox";
            this.sendEofCheckbox.Size = new System.Drawing.Size(110, 24);
            this.sendEofCheckbox.TabIndex = 18;
            this.sendEofCheckbox.Text = "Send EOF";
            this.sendEofCheckbox.UseVisualStyleBackColor = true;
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(516, 641);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(255, 99);
            this.abortButton.TabIndex = 19;
            this.abortButton.Text = "Abort";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(787, 641);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(255, 99);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // execSendCrlfCheckBox
            // 
            this.execSendCrlfCheckBox.AutoSize = true;
            this.execSendCrlfCheckBox.Location = new System.Drawing.Point(380, 254);
            this.execSendCrlfCheckBox.Name = "execSendCrlfCheckBox";
            this.execSendCrlfCheckBox.Size = new System.Drawing.Size(119, 24);
            this.execSendCrlfCheckBox.TabIndex = 21;
            this.execSendCrlfCheckBox.Text = "Send CRLF";
            this.execSendCrlfCheckBox.UseVisualStyleBackColor = true;
            // 
            // DvcDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 824);
            this.Controls.Add(this.dvcTabs);
            this.Name = "DvcDialog";
            this.Text = "DvcDialog";
            this.dvcTabs.ResumeLayout(false);
            this.infoPage.ResumeLayout(false);
            this.execRunPage.ResumeLayout(false);
            this.execRunPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl dvcTabs;
        private System.Windows.Forms.TabPage infoPage;
        private System.Windows.Forms.TabPage execRunPage;
        private System.Windows.Forms.PropertyGrid capabilitiesGrid;
        private System.Windows.Forms.Button execRunButton;
        private System.Windows.Forms.TextBox execRunCommandTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage messageBoxTab;
        private System.Windows.Forms.TabPage sessionTab;
        private System.Windows.Forms.TextBox stdoutTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox execKindComboBox;
        private System.Windows.Forms.Button sendStdinButton;
        private System.Windows.Forms.TextBox stdinTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox execDirectoryTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox execArgsTextBox;
        private System.Windows.Forms.TextBox stderrTextBox;
        private System.Windows.Forms.TextBox execLogTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox sendEofCheckbox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.CheckBox execSendCrlfCheckBox;
    }
}