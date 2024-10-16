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
            this.label1 = new System.Windows.Forms.Label();
            this.execRunCommandTextBox = new System.Windows.Forms.TextBox();
            this.execRunButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.messageBoxTab = new System.Windows.Forms.TabPage();
            this.sessionTab = new System.Windows.Forms.TabPage();
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
            this.dvcTabs.Size = new System.Drawing.Size(1074, 638);
            this.dvcTabs.TabIndex = 0;
            // 
            // infoPage
            // 
            this.infoPage.Controls.Add(this.capabilitiesGrid);
            this.infoPage.Location = new System.Drawing.Point(4, 29);
            this.infoPage.Name = "infoPage";
            this.infoPage.Padding = new System.Windows.Forms.Padding(3);
            this.infoPage.Size = new System.Drawing.Size(1066, 605);
            this.infoPage.TabIndex = 0;
            this.infoPage.Text = "Info";
            this.infoPage.UseVisualStyleBackColor = true;
            // 
            // capabilitiesGrid
            // 
            this.capabilitiesGrid.Location = new System.Drawing.Point(6, 6);
            this.capabilitiesGrid.Name = "capabilitiesGrid";
            this.capabilitiesGrid.Size = new System.Drawing.Size(1054, 654);
            this.capabilitiesGrid.TabIndex = 0;
            // 
            // execRunPage
            // 
            this.execRunPage.Controls.Add(this.execRunButton);
            this.execRunPage.Controls.Add(this.execRunCommandTextBox);
            this.execRunPage.Controls.Add(this.label1);
            this.execRunPage.Location = new System.Drawing.Point(4, 29);
            this.execRunPage.Name = "execRunPage";
            this.execRunPage.Padding = new System.Windows.Forms.Padding(3);
            this.execRunPage.Size = new System.Drawing.Size(1066, 605);
            this.execRunPage.TabIndex = 1;
            this.execRunPage.Text = "Run";
            this.execRunPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Command:";
            // 
            // execRunCommandTextBox
            // 
            this.execRunCommandTextBox.Location = new System.Drawing.Point(25, 41);
            this.execRunCommandTextBox.Name = "execRunCommandTextBox";
            this.execRunCommandTextBox.Size = new System.Drawing.Size(1014, 26);
            this.execRunCommandTextBox.TabIndex = 1;
            // 
            // execRunButton
            // 
            this.execRunButton.Location = new System.Drawing.Point(25, 425);
            this.execRunButton.Name = "execRunButton";
            this.execRunButton.Size = new System.Drawing.Size(1014, 61);
            this.execRunButton.TabIndex = 2;
            this.execRunButton.Text = "RUN";
            this.execRunButton.UseVisualStyleBackColor = true;
            this.execRunButton.Click += new System.EventHandler(this.execRunButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(468, 672);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 26);
            this.textBox1.TabIndex = 1;
            // 
            // messageBoxTab
            // 
            this.messageBoxTab.Location = new System.Drawing.Point(4, 29);
            this.messageBoxTab.Name = "messageBoxTab";
            this.messageBoxTab.Padding = new System.Windows.Forms.Padding(3);
            this.messageBoxTab.Size = new System.Drawing.Size(1066, 605);
            this.messageBoxTab.TabIndex = 2;
            this.messageBoxTab.Text = "Message Box";
            this.messageBoxTab.UseVisualStyleBackColor = true;
            // 
            // sessionTab
            // 
            this.sessionTab.Location = new System.Drawing.Point(4, 29);
            this.sessionTab.Name = "sessionTab";
            this.sessionTab.Padding = new System.Windows.Forms.Padding(3);
            this.sessionTab.Size = new System.Drawing.Size(1066, 605);
            this.sessionTab.TabIndex = 3;
            this.sessionTab.Text = "Session";
            this.sessionTab.UseVisualStyleBackColor = true;
            // 
            // DvcDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 723);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dvcTabs);
            this.Name = "DvcDialog";
            this.Text = "DvcDialog";
            this.dvcTabs.ResumeLayout(false);
            this.infoPage.ResumeLayout(false);
            this.execRunPage.ResumeLayout(false);
            this.execRunPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl dvcTabs;
        private System.Windows.Forms.TabPage infoPage;
        private System.Windows.Forms.TabPage execRunPage;
        private System.Windows.Forms.PropertyGrid capabilitiesGrid;
        private System.Windows.Forms.Button execRunButton;
        private System.Windows.Forms.TextBox execRunCommandTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage messageBoxTab;
        private System.Windows.Forms.TabPage sessionTab;
    }
}