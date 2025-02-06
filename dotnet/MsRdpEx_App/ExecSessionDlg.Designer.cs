namespace MsRdpEx_App
{
    partial class ExecSessionDlg
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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            sessionIdText = new System.Windows.Forms.Label();
            sessionKindText = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            sessionStatusText = new System.Windows.Forms.Label();
            stdinInput = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            cancelPendingText = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            stdoutInput = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            stderrInput = new System.Windows.Forms.TextBox();
            logInput = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            sendStdinButton = new System.Windows.Forms.Button();
            eofCheckBox = new System.Windows.Forms.CheckBox();
            crCheckBox = new System.Windows.Forms.CheckBox();
            lfCheckBox = new System.Windows.Forms.CheckBox();
            cancelButton = new System.Windows.Forms.Button();
            abortButton = new System.Windows.Forms.Button();
            label9 = new System.Windows.Forms.Label();
            abortCodeInput = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(11, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(99, 25);
            label1.TabIndex = 0;
            label1.Text = "Session ID:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 37);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(51, 25);
            label2.TabIndex = 1;
            label2.Text = "Kind:";
            // 
            // sessionIdText
            // 
            sessionIdText.AutoSize = true;
            sessionIdText.Location = new System.Drawing.Point(116, 12);
            sessionIdText.Name = "sessionIdText";
            sessionIdText.Size = new System.Drawing.Size(44, 25);
            sessionIdText.TabIndex = 2;
            sessionIdText.Text = "<?>";
            // 
            // sessionKindText
            // 
            sessionKindText.AutoSize = true;
            sessionKindText.Location = new System.Drawing.Point(116, 37);
            sessionKindText.Name = "sessionKindText";
            sessionKindText.Size = new System.Drawing.Size(44, 25);
            sessionKindText.TabIndex = 3;
            sessionKindText.Text = "<?>";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(257, 12);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(64, 25);
            label3.TabIndex = 4;
            label3.Text = "Status:";
            // 
            // sessionStatusText
            // 
            sessionStatusText.AutoSize = true;
            sessionStatusText.Location = new System.Drawing.Point(401, 12);
            sessionStatusText.Name = "sessionStatusText";
            sessionStatusText.Size = new System.Drawing.Size(84, 25);
            sessionStatusText.TabIndex = 5;
            sessionStatusText.Text = "starting...";
            // 
            // stdinInput
            // 
            stdinInput.Enabled = false;
            stdinInput.Location = new System.Drawing.Point(11, 117);
            stdinInput.Multiline = true;
            stdinInput.Name = "stdinInput";
            stdinInput.Size = new System.Drawing.Size(777, 97);
            stdinInput.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 89);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(107, 25);
            label4.TabIndex = 7;
            label4.Text = "Input(stdin):";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(257, 37);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(138, 25);
            label5.TabIndex = 8;
            label5.Text = "Cancel pending:";
            // 
            // cancelPendingText
            // 
            cancelPendingText.AutoSize = true;
            cancelPendingText.Location = new System.Drawing.Point(401, 37);
            cancelPendingText.Name = "cancelPendingText";
            cancelPendingText.Size = new System.Drawing.Size(33, 25);
            cancelPendingText.TabIndex = 9;
            cancelPendingText.Text = "no";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 268);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(135, 25);
            label6.TabIndex = 11;
            label6.Text = "Output(stdout):";
            // 
            // stdoutInput
            // 
            stdoutInput.Location = new System.Drawing.Point(11, 296);
            stdoutInput.Multiline = true;
            stdoutInput.Name = "stdoutInput";
            stdoutInput.ReadOnly = true;
            stdoutInput.Size = new System.Drawing.Size(777, 146);
            stdoutInput.TabIndex = 10;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = System.Drawing.Color.Brown;
            label7.Location = new System.Drawing.Point(13, 451);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(116, 25);
            label7.TabIndex = 13;
            label7.Text = "Error(stdout):";
            // 
            // stderrInput
            // 
            stderrInput.ForeColor = System.Drawing.Color.Brown;
            stderrInput.Location = new System.Drawing.Point(12, 479);
            stderrInput.Multiline = true;
            stderrInput.Name = "stderrInput";
            stderrInput.ReadOnly = true;
            stderrInput.Size = new System.Drawing.Size(777, 146);
            stderrInput.TabIndex = 12;
            // 
            // logInput
            // 
            logInput.ForeColor = System.Drawing.Color.MidnightBlue;
            logInput.Location = new System.Drawing.Point(13, 661);
            logInput.Multiline = true;
            logInput.Name = "logInput";
            logInput.ReadOnly = true;
            logInput.Size = new System.Drawing.Size(777, 146);
            logInput.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.Color.MidnightBlue;
            label8.Location = new System.Drawing.Point(13, 628);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(107, 25);
            label8.TabIndex = 15;
            label8.Text = "Session log:";
            // 
            // sendStdinButton
            // 
            sendStdinButton.Enabled = false;
            sendStdinButton.Location = new System.Drawing.Point(678, 220);
            sendStdinButton.Name = "sendStdinButton";
            sendStdinButton.Size = new System.Drawing.Size(112, 34);
            sendStdinButton.TabIndex = 16;
            sendStdinButton.Text = "Send";
            sendStdinButton.UseVisualStyleBackColor = true;
            sendStdinButton.Click += sendStdinButton_Click;
            // 
            // eofCheckBox
            // 
            eofCheckBox.AutoSize = true;
            eofCheckBox.ForeColor = System.Drawing.Color.Firebrick;
            eofCheckBox.Location = new System.Drawing.Point(13, 220);
            eofCheckBox.Name = "eofCheckBox";
            eofCheckBox.Size = new System.Drawing.Size(70, 29);
            eofCheckBox.TabIndex = 17;
            eofCheckBox.Text = "EOF";
            eofCheckBox.UseVisualStyleBackColor = true;
            // 
            // crCheckBox
            // 
            crCheckBox.AutoSize = true;
            crCheckBox.Location = new System.Drawing.Point(90, 220);
            crCheckBox.Name = "crCheckBox";
            crCheckBox.Size = new System.Drawing.Size(60, 29);
            crCheckBox.TabIndex = 18;
            crCheckBox.Text = "CR";
            crCheckBox.UseVisualStyleBackColor = true;
            // 
            // lfCheckBox
            // 
            lfCheckBox.AutoSize = true;
            lfCheckBox.Location = new System.Drawing.Point(156, 220);
            lfCheckBox.Name = "lfCheckBox";
            lfCheckBox.Size = new System.Drawing.Size(55, 29);
            lfCheckBox.TabIndex = 19;
            lfCheckBox.Text = "LF";
            lfCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.BackColor = System.Drawing.Color.PeachPuff;
            cancelButton.Enabled = false;
            cancelButton.Location = new System.Drawing.Point(696, 12);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(92, 46);
            cancelButton.TabIndex = 21;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // abortButton
            // 
            abortButton.BackColor = System.Drawing.Color.LightCoral;
            abortButton.Location = new System.Drawing.Point(696, 64);
            abortButton.Name = "abortButton";
            abortButton.Size = new System.Drawing.Size(92, 43);
            abortButton.TabIndex = 22;
            abortButton.Text = "Abort";
            abortButton.UseVisualStyleBackColor = false;
            abortButton.Click += abortButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(519, 70);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(106, 25);
            label9.TabIndex = 23;
            label9.Text = "Abort code:";
            // 
            // abortCodeInput
            // 
            abortCodeInput.Location = new System.Drawing.Point(631, 70);
            abortCodeInput.Name = "abortCodeInput";
            abortCodeInput.Size = new System.Drawing.Size(59, 31);
            abortCodeInput.TabIndex = 24;
            abortCodeInput.Text = "1";
            // 
            // ExexSessionDlg
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 837);
            Controls.Add(abortCodeInput);
            Controls.Add(label9);
            Controls.Add(abortButton);
            Controls.Add(cancelButton);
            Controls.Add(lfCheckBox);
            Controls.Add(crCheckBox);
            Controls.Add(eofCheckBox);
            Controls.Add(sendStdinButton);
            Controls.Add(label8);
            Controls.Add(logInput);
            Controls.Add(label7);
            Controls.Add(stderrInput);
            Controls.Add(label6);
            Controls.Add(stdoutInput);
            Controls.Add(cancelPendingText);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(stdinInput);
            Controls.Add(sessionStatusText);
            Controls.Add(label3);
            Controls.Add(sessionKindText);
            Controls.Add(sessionIdText);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "ExexSessionDlg";
            Text = "NowProto Exec Session";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label sessionIdText;
        private System.Windows.Forms.Label sessionKindText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label sessionStatusText;
        private System.Windows.Forms.TextBox stdinInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label cancelPendingText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox stdoutInput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stderrInput;
        private System.Windows.Forms.TextBox logInput;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button sendStdinButton;
        private System.Windows.Forms.CheckBox eofCheckBox;
        private System.Windows.Forms.CheckBox crCheckBox;
        private System.Windows.Forms.CheckBox lfCheckBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox abortCodeInput;
    }
}