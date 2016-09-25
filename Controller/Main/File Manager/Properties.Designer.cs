namespace Tcpclient.Forms
{
    partial class Properties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Properties));
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sizeTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.createdTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.modifiedTextBox = new System.Windows.Forms.TextBox();
            this.accessedTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.archivedCheck = new System.Windows.Forms.CheckBox();
            this.compressedCheck = new System.Windows.Forms.CheckBox();
            this.encryptedCheck = new System.Windows.Forms.CheckBox();
            this.hiddenCheck = new System.Windows.Forms.CheckBox();
            this.readOnlyCheck = new System.Windows.Forms.CheckBox();
            this.systemCheck = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(69, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(205, 20);
            this.nameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "File Size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Created:";
            // 
            // sizeTextBox
            // 
            this.sizeTextBox.Location = new System.Drawing.Point(69, 38);
            this.sizeTextBox.Name = "sizeTextBox";
            this.sizeTextBox.ReadOnly = true;
            this.sizeTextBox.Size = new System.Drawing.Size(52, 20);
            this.sizeTextBox.TabIndex = 4;
            this.sizeTextBox.Text = "1.23";
            this.sizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(127, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Bytes";
            // 
            // createdTextBox
            // 
            this.createdTextBox.Location = new System.Drawing.Point(69, 64);
            this.createdTextBox.Name = "createdTextBox";
            this.createdTextBox.Size = new System.Drawing.Size(205, 20);
            this.createdTextBox.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Modified:";
            // 
            // modifiedTextBox
            // 
            this.modifiedTextBox.Location = new System.Drawing.Point(69, 90);
            this.modifiedTextBox.Name = "modifiedTextBox";
            this.modifiedTextBox.Size = new System.Drawing.Size(205, 20);
            this.modifiedTextBox.TabIndex = 8;
            // 
            // accessedTextBox
            // 
            this.accessedTextBox.Location = new System.Drawing.Point(69, 116);
            this.accessedTextBox.Name = "accessedTextBox";
            this.accessedTextBox.Size = new System.Drawing.Size(205, 20);
            this.accessedTextBox.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Accessed:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(75, 211);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(145, 211);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(68, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // archivedCheck
            // 
            this.archivedCheck.AutoSize = true;
            this.archivedCheck.Location = new System.Drawing.Point(59, 142);
            this.archivedCheck.Name = "archivedCheck";
            this.archivedCheck.Size = new System.Drawing.Size(62, 17);
            this.archivedCheck.TabIndex = 13;
            this.archivedCheck.Text = "Archive";
            this.archivedCheck.UseVisualStyleBackColor = true;
            // 
            // compressedCheck
            // 
            this.compressedCheck.AutoSize = true;
            this.compressedCheck.Location = new System.Drawing.Point(59, 165);
            this.compressedCheck.Name = "compressedCheck";
            this.compressedCheck.Size = new System.Drawing.Size(84, 17);
            this.compressedCheck.TabIndex = 14;
            this.compressedCheck.Text = "Compressed";
            this.compressedCheck.UseVisualStyleBackColor = true;
            // 
            // encryptedCheck
            // 
            this.encryptedCheck.AutoSize = true;
            this.encryptedCheck.Location = new System.Drawing.Point(59, 188);
            this.encryptedCheck.Name = "encryptedCheck";
            this.encryptedCheck.Size = new System.Drawing.Size(74, 17);
            this.encryptedCheck.TabIndex = 15;
            this.encryptedCheck.Text = "Encrypted";
            this.encryptedCheck.UseVisualStyleBackColor = true;
            // 
            // hiddenCheck
            // 
            this.hiddenCheck.AutoSize = true;
            this.hiddenCheck.Location = new System.Drawing.Point(166, 142);
            this.hiddenCheck.Name = "hiddenCheck";
            this.hiddenCheck.Size = new System.Drawing.Size(60, 17);
            this.hiddenCheck.TabIndex = 16;
            this.hiddenCheck.Text = "Hidden";
            this.hiddenCheck.UseVisualStyleBackColor = true;
            // 
            // readOnlyCheck
            // 
            this.readOnlyCheck.AutoSize = true;
            this.readOnlyCheck.Location = new System.Drawing.Point(166, 165);
            this.readOnlyCheck.Name = "readOnlyCheck";
            this.readOnlyCheck.Size = new System.Drawing.Size(76, 17);
            this.readOnlyCheck.TabIndex = 17;
            this.readOnlyCheck.Text = "Read Only";
            this.readOnlyCheck.UseVisualStyleBackColor = true;
            // 
            // systemCheck
            // 
            this.systemCheck.AutoSize = true;
            this.systemCheck.Location = new System.Drawing.Point(166, 188);
            this.systemCheck.Name = "systemCheck";
            this.systemCheck.Size = new System.Drawing.Size(60, 17);
            this.systemCheck.TabIndex = 18;
            this.systemCheck.Text = "System";
            this.systemCheck.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Properties
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(282, 241);
            this.Controls.Add(this.systemCheck);
            this.Controls.Add(this.readOnlyCheck);
            this.Controls.Add(this.hiddenCheck);
            this.Controls.Add(this.encryptedCheck);
            this.Controls.Add(this.compressedCheck);
            this.Controls.Add(this.archivedCheck);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.accessedTextBox);
            this.Controls.Add(this.modifiedTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.createdTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sizeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Properties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Properties_FormClosing);
            this.Load += new System.EventHandler(this.Properties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sizeTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox createdTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox modifiedTextBox;
        private System.Windows.Forms.TextBox accessedTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox archivedCheck;
        private System.Windows.Forms.CheckBox compressedCheck;
        private System.Windows.Forms.CheckBox encryptedCheck;
        private System.Windows.Forms.CheckBox hiddenCheck;
        private System.Windows.Forms.CheckBox readOnlyCheck;
        private System.Windows.Forms.CheckBox systemCheck;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

    }
}