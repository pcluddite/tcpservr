namespace TCPHOST
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizedNotActiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notActiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableCloseButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.killWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visibleWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizedWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizedWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secondsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.freezeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeDetailsToolStripMenuItem,
            this.toolStripSeparator2,
            this.hideToolStripMenuItem,
            this.maximizeToolStripMenuItem,
            this.minimizeToolStripMenuItem,
            this.restoreToolStripMenuItem,
            this.showToolStripMenuItem,
            this.disableCloseButtonToolStripMenuItem,
            this.toolStripSeparator1,
            this.killWindowToolStripMenuItem,
            this.closeWindowToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(184, 214);
            // 
            // changeDetailsToolStripMenuItem
            // 
            this.changeDetailsToolStripMenuItem.Name = "changeDetailsToolStripMenuItem";
            this.changeDetailsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.changeDetailsToolStripMenuItem.Text = "View Details";
            this.changeDetailsToolStripMenuItem.Click += new System.EventHandler(this.changeDetailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(180, 6);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.maximizeToolStripMenuItem.Text = "Maximize";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.minimizeToolStripMenuItem.Text = "Minimize";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.maximizedToolStripMenuItem,
            this.minimizedToolStripMenuItem,
            this.minimizedNotActiveToolStripMenuItem,
            this.notActiveToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.defaultToolStripMenuItem.Text = "Default";
            this.defaultToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // maximizedToolStripMenuItem
            // 
            this.maximizedToolStripMenuItem.Name = "maximizedToolStripMenuItem";
            this.maximizedToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.maximizedToolStripMenuItem.Text = "Maximized";
            this.maximizedToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // minimizedToolStripMenuItem
            // 
            this.minimizedToolStripMenuItem.Name = "minimizedToolStripMenuItem";
            this.minimizedToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.minimizedToolStripMenuItem.Text = "Minimized";
            this.minimizedToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // minimizedNotActiveToolStripMenuItem
            // 
            this.minimizedNotActiveToolStripMenuItem.Name = "minimizedNotActiveToolStripMenuItem";
            this.minimizedNotActiveToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.minimizedNotActiveToolStripMenuItem.Text = "Minimized Not Active";
            // 
            // notActiveToolStripMenuItem
            // 
            this.notActiveToolStripMenuItem.Name = "notActiveToolStripMenuItem";
            this.notActiveToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.notActiveToolStripMenuItem.Text = "Not Active";
            this.notActiveToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // disableCloseButtonToolStripMenuItem
            // 
            this.disableCloseButtonToolStripMenuItem.Name = "disableCloseButtonToolStripMenuItem";
            this.disableCloseButtonToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.disableCloseButtonToolStripMenuItem.Text = "Disable Close Button";
            this.disableCloseButtonToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(180, 6);
            // 
            // killWindowToolStripMenuItem
            // 
            this.killWindowToolStripMenuItem.Name = "killWindowToolStripMenuItem";
            this.killWindowToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.killWindowToolStripMenuItem.Text = "Kill Window";
            this.killWindowToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // closeWindowToolStripMenuItem
            // 
            this.closeWindowToolStripMenuItem.Name = "closeWindowToolStripMenuItem";
            this.closeWindowToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.closeWindowToolStripMenuItem.Text = "Close Window";
            this.closeWindowToolStripMenuItem.Click += new System.EventHandler(this.closeWindowToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(12, 32);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(350, 342);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            this.columnHeader1.Width = 121;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Window State";
            this.columnHeader2.Width = 98;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Position";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Size";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.refreshRateToolStripMenuItem,
            this.pictureQualityToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(374, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.existingToolStripMenuItem,
            this.visibleWindowsToolStripMenuItem,
            this.minimizedWindowsToolStripMenuItem,
            this.maximizedWindowsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // existingToolStripMenuItem
            // 
            this.existingToolStripMenuItem.Name = "existingToolStripMenuItem";
            this.existingToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.existingToolStripMenuItem.Text = "All Windows";
            this.existingToolStripMenuItem.ToolTipText = "Show all existing windows";
            this.existingToolStripMenuItem.Click += new System.EventHandler(this.existingToolStripMenuItem_Click);
            // 
            // visibleWindowsToolStripMenuItem
            // 
            this.visibleWindowsToolStripMenuItem.Checked = true;
            this.visibleWindowsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.visibleWindowsToolStripMenuItem.Name = "visibleWindowsToolStripMenuItem";
            this.visibleWindowsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.visibleWindowsToolStripMenuItem.Text = "Visible Windows";
            this.visibleWindowsToolStripMenuItem.ToolTipText = "Show only visible windows";
            this.visibleWindowsToolStripMenuItem.Click += new System.EventHandler(this.visibleWindowsToolStripMenuItem_Click);
            // 
            // minimizedWindowsToolStripMenuItem
            // 
            this.minimizedWindowsToolStripMenuItem.Name = "minimizedWindowsToolStripMenuItem";
            this.minimizedWindowsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.minimizedWindowsToolStripMenuItem.Text = "Minimized Windows";
            this.minimizedWindowsToolStripMenuItem.ToolTipText = "Show only minimized windows";
            this.minimizedWindowsToolStripMenuItem.Click += new System.EventHandler(this.minimizedWindowsToolStripMenuItem_Click);
            // 
            // maximizedWindowsToolStripMenuItem
            // 
            this.maximizedWindowsToolStripMenuItem.Name = "maximizedWindowsToolStripMenuItem";
            this.maximizedWindowsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.maximizedWindowsToolStripMenuItem.Text = "Maximized Windows";
            this.maximizedWindowsToolStripMenuItem.ToolTipText = "Show only maximized windows";
            this.maximizedWindowsToolStripMenuItem.Click += new System.EventHandler(this.maximizedWindowsToolStripMenuItem_Click);
            // 
            // refreshRateToolStripMenuItem
            // 
            this.refreshRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secondToolStripMenuItem,
            this.secondsToolStripMenuItem,
            this.secondsToolStripMenuItem1,
            this.freezeToolStripMenuItem});
            this.refreshRateToolStripMenuItem.Name = "refreshRateToolStripMenuItem";
            this.refreshRateToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.refreshRateToolStripMenuItem.Text = "Refresh Rate";
            // 
            // secondToolStripMenuItem
            // 
            this.secondToolStripMenuItem.Name = "secondToolStripMenuItem";
            this.secondToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.secondToolStripMenuItem.Text = "1 Second";
            this.secondToolStripMenuItem.ToolTipText = "Ask for list of windows every 1 second";
            this.secondToolStripMenuItem.Click += new System.EventHandler(this.secondToolStripMenuItem_Click);
            // 
            // secondsToolStripMenuItem
            // 
            this.secondsToolStripMenuItem.Checked = true;
            this.secondsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.secondsToolStripMenuItem.Name = "secondsToolStripMenuItem";
            this.secondsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.secondsToolStripMenuItem.Text = "3 Seconds";
            this.secondsToolStripMenuItem.ToolTipText = "Ask for list of windows every 3 seconds";
            this.secondsToolStripMenuItem.Click += new System.EventHandler(this.secondsToolStripMenuItem_Click);
            // 
            // secondsToolStripMenuItem1
            // 
            this.secondsToolStripMenuItem1.Name = "secondsToolStripMenuItem1";
            this.secondsToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.secondsToolStripMenuItem1.Text = "5 Seconds";
            this.secondsToolStripMenuItem1.ToolTipText = "Ask for list of windows every 5 second";
            this.secondsToolStripMenuItem1.Click += new System.EventHandler(this.secondsToolStripMenuItem1_Click);
            // 
            // freezeToolStripMenuItem
            // 
            this.freezeToolStripMenuItem.Name = "freezeToolStripMenuItem";
            this.freezeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.freezeToolStripMenuItem.Text = "Freeze";
            this.freezeToolStripMenuItem.ToolTipText = "Stop asking for a list of windows";
            this.freezeToolStripMenuItem.Click += new System.EventHandler(this.freezeToolStripMenuItem_Click);
            // 
            // pictureQualityToolStripMenuItem
            // 
            this.pictureQualityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.pictureQualityToolStripMenuItem.Name = "pictureQualityToolStripMenuItem";
            this.pictureQualityToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
            this.pictureQualityToolStripMenuItem.Text = "Picture Quality";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Text = "50%";
            this.toolStripTextBox1.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolStripTextBox1.ToolTipText = "The quality (1-100) of downloaded screen shots\r\nLower quality means faster downlo" +
                "ad";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 386);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Window Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableCloseButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killWindowToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem changeDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizedNotActiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notActiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshRateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secondsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pictureQualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem existingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visibleWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizedWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizedWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem freezeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}