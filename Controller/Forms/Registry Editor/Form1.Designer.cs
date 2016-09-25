namespace Tcpclient.Forms.Registry_Editor
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("HKEY_CLASSES_ROOT");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("HKEY_CURRENT_USER");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("HKEY_LOCAL_MACHINE");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("HKEY_USERS");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("HKEY_CURRENT_CONFIG");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Computer", 3, 3, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dWORD32BitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.keyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.stringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dWORD32bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qWORD64bitValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.multiStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.expandableStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.copyKeyNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(527, 462);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "Name";
            this.columnHeader0.Width = 121;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 117;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Data";
            this.columnHeader2.Width = 285;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(99, 26);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyToolStripMenuItem,
            this.toolStripSeparator1,
            this.stringValueToolStripMenuItem,
            this.binaryValueToolStripMenuItem,
            this.dWORD32BitToolStripMenuItem,
            this.qWORD64bitValueToolStripMenuItem,
            this.multiStringValueToolStripMenuItem,
            this.expandableStringValueToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // keyToolStripMenuItem
            // 
            this.keyToolStripMenuItem.Name = "keyToolStripMenuItem";
            this.keyToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.keyToolStripMenuItem.Text = "Key";
            this.keyToolStripMenuItem.Click += new System.EventHandler(this.keyToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // stringValueToolStripMenuItem
            // 
            this.stringValueToolStripMenuItem.Name = "stringValueToolStripMenuItem";
            this.stringValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.stringValueToolStripMenuItem.Text = "String Value";
            this.stringValueToolStripMenuItem.Click += new System.EventHandler(this.stringValueToolStripMenuItem_Click);
            // 
            // binaryValueToolStripMenuItem
            // 
            this.binaryValueToolStripMenuItem.Name = "binaryValueToolStripMenuItem";
            this.binaryValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.binaryValueToolStripMenuItem.Text = "Binary Value";
            this.binaryValueToolStripMenuItem.Click += new System.EventHandler(this.binaryValueToolStripMenuItem_Click);
            // 
            // dWORD32BitToolStripMenuItem
            // 
            this.dWORD32BitToolStripMenuItem.Name = "dWORD32BitToolStripMenuItem";
            this.dWORD32BitToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.dWORD32BitToolStripMenuItem.Text = "DWORD (32-bit) Value";
            this.dWORD32BitToolStripMenuItem.Click += new System.EventHandler(this.dWORD32BitToolStripMenuItem_Click);
            // 
            // qWORD64bitValueToolStripMenuItem
            // 
            this.qWORD64bitValueToolStripMenuItem.Name = "qWORD64bitValueToolStripMenuItem";
            this.qWORD64bitValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.qWORD64bitValueToolStripMenuItem.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem.Click += new System.EventHandler(this.qWORD64bitValueToolStripMenuItem_Click);
            // 
            // multiStringValueToolStripMenuItem
            // 
            this.multiStringValueToolStripMenuItem.Name = "multiStringValueToolStripMenuItem";
            this.multiStringValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.multiStringValueToolStripMenuItem.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem.Click += new System.EventHandler(this.multiStringValueToolStripMenuItem_Click);
            // 
            // expandableStringValueToolStripMenuItem
            // 
            this.expandableStringValueToolStripMenuItem.Name = "expandableStringValueToolStripMenuItem";
            this.expandableStringValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.expandableStringValueToolStripMenuItem.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem.Click += new System.EventHandler(this.expandableStringValueToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.ico");
            this.imageList1.Images.SetKeyName(1, "dword.ico");
            this.imageList1.Images.SetKeyName(2, "string.ico");
            this.imageList1.Images.SetKeyName(3, "computer.png");
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "HKEY_CLASSES_ROOT";
            treeNode1.Text = "HKEY_CLASSES_ROOT";
            treeNode2.Name = "HKEY_CURRENT_USER";
            treeNode2.Text = "HKEY_CURRENT_USER";
            treeNode3.Name = "HKEY_LOCAL_MACHINE";
            treeNode3.Text = "HKEY_LOCAL_MACHINE";
            treeNode4.Name = "HKEY_USERS";
            treeNode4.Text = "HKEY_USERS";
            treeNode5.Name = "HKEY_CURRENT_CONFIG";
            treeNode5.Text = "HKEY_CURRENT_CONFIG";
            treeNode6.ImageIndex = 3;
            treeNode6.Name = "Node0";
            treeNode6.SelectedImageIndex = 3;
            treeNode6.Text = "Computer";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(217, 462);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseClick);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.newToolStripMenuItem1,
            this.toolStripSeparator4,
            this.deleteToolStripMenuItem1,
            this.renameToolStripMenuItem1,
            this.toolStripSeparator5,
            this.copyKeyNameToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(160, 148);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyToolStripMenuItem1,
            this.toolStripSeparator3,
            this.stringValueToolStripMenuItem1,
            this.binaryValueToolStripMenuItem1,
            this.dWORD32bitValueToolStripMenuItem,
            this.qWORD64bitValueToolStripMenuItem1,
            this.multiStringValueToolStripMenuItem1,
            this.expandableStringValueToolStripMenuItem1});
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.newToolStripMenuItem1.Text = "New";
            // 
            // keyToolStripMenuItem1
            // 
            this.keyToolStripMenuItem1.Name = "keyToolStripMenuItem1";
            this.keyToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.keyToolStripMenuItem1.Text = "Key";
            this.keyToolStripMenuItem1.Click += new System.EventHandler(this.keyToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(197, 6);
            // 
            // stringValueToolStripMenuItem1
            // 
            this.stringValueToolStripMenuItem1.Name = "stringValueToolStripMenuItem1";
            this.stringValueToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.stringValueToolStripMenuItem1.Text = "String Value";
            this.stringValueToolStripMenuItem1.Click += new System.EventHandler(this.stringValueToolStripMenuItem_Click);
            // 
            // binaryValueToolStripMenuItem1
            // 
            this.binaryValueToolStripMenuItem1.Name = "binaryValueToolStripMenuItem1";
            this.binaryValueToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.binaryValueToolStripMenuItem1.Text = "Binary Value";
            this.binaryValueToolStripMenuItem1.Click += new System.EventHandler(this.binaryValueToolStripMenuItem_Click);
            // 
            // dWORD32bitValueToolStripMenuItem
            // 
            this.dWORD32bitValueToolStripMenuItem.Name = "dWORD32bitValueToolStripMenuItem";
            this.dWORD32bitValueToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.dWORD32bitValueToolStripMenuItem.Text = "DWORD (32-bit) Value";
            this.dWORD32bitValueToolStripMenuItem.Click += new System.EventHandler(this.dWORD32BitToolStripMenuItem_Click);
            // 
            // qWORD64bitValueToolStripMenuItem1
            // 
            this.qWORD64bitValueToolStripMenuItem1.Name = "qWORD64bitValueToolStripMenuItem1";
            this.qWORD64bitValueToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.qWORD64bitValueToolStripMenuItem1.Text = "QWORD (64-bit) Value";
            this.qWORD64bitValueToolStripMenuItem1.Click += new System.EventHandler(this.qWORD64bitValueToolStripMenuItem_Click);
            // 
            // multiStringValueToolStripMenuItem1
            // 
            this.multiStringValueToolStripMenuItem1.Name = "multiStringValueToolStripMenuItem1";
            this.multiStringValueToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.multiStringValueToolStripMenuItem1.Text = "Multi-String Value";
            this.multiStringValueToolStripMenuItem1.Click += new System.EventHandler(this.multiStringValueToolStripMenuItem_Click);
            // 
            // expandableStringValueToolStripMenuItem1
            // 
            this.expandableStringValueToolStripMenuItem1.Name = "expandableStringValueToolStripMenuItem1";
            this.expandableStringValueToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.expandableStringValueToolStripMenuItem1.Text = "Expandable String Value";
            this.expandableStringValueToolStripMenuItem1.Click += new System.EventHandler(this.expandableStringValueToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(156, 6);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // renameToolStripMenuItem1
            // 
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.renameToolStripMenuItem1.Text = "Rename";
            this.renameToolStripMenuItem1.Click += new System.EventHandler(this.renameToolStripMenuItem1_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(156, 6);
            // 
            // copyKeyNameToolStripMenuItem
            // 
            this.copyKeyNameToolStripMenuItem.Name = "copyKeyNameToolStripMenuItem";
            this.copyKeyNameToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.copyKeyNameToolStripMenuItem.Text = "Copy Key Name";
            this.copyKeyNameToolStripMenuItem.Click += new System.EventHandler(this.copyKeyNameToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(5, 7);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(760, 468);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 478);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(774, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(61, 17);
            this.toolStripStatusLabel1.Text = "Computer";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modifyToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(123, 76);
            this.contextMenuStrip2.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip2_Closed);
            // 
            // modifyToolStripMenuItem
            // 
            this.modifyToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
            this.modifyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.modifyToolStripMenuItem.Text = "Modify...";
            this.modifyToolStripMenuItem.Click += new System.EventHandler(this.modifyToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 500);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Registry Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem stringValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binaryValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dWORD32BitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qWORD64bitValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiStringValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandableStringValueToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem modifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem keyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem stringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem binaryValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dWORD32bitValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qWORD64bitValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem multiStringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem expandableStringValueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem copyKeyNameToolStripMenuItem;
    }
}