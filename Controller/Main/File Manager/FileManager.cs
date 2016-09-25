using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net;
using Tcpclient.Forms;
using System.IO;
using Tcpclient.Components;

namespace Tcpclient {
    public class FileManager {

        private Main main;

        public bool IsActive {
            get {
                return main.label4.Text.Equals("File Manager");
            }
            set {
                if (value) {
                    Activate();
                }
                else {
                    Deactivate();
                }
            }
        }

        private Main.ShowStatusTipMethod showStatusTip;
        private ResponseDelegate SetFileList;
        private ResponseDelegate SetDirList;
        
        public FileManager(Main mainForm) {
            this.main = mainForm;
            SetFileList = new ResponseDelegate(setListFile);
            SetDirList = new ResponseDelegate(setListDir);

            showStatusTip = new Main.ShowStatusTipMethod(main.ShowStatusTip);
            main.downloadButton.Click += new EventHandler(downloadButton_Click);
            main.uploadButton.Click += new EventHandler(uploadButton_Click);
            main.renameButton.Click += new EventHandler(renameButton_Click);
            main.copyButton.Click += new EventHandler(copyButton_Click);
            main.cutButton.Click += new EventHandler(cutButton_Click);

            main.newFolderButton.Click += new EventHandler(newFolder_Click);
            main.deleteButton.Click += new EventHandler(deleteButton_Click);
            main.recycleButton.Click += new EventHandler(recycleButton_Click);
            main.pasteButton.Click += new EventHandler(pasteButton_Click);
            main.goButton.Click += new EventHandler(goButton_Click);
            main.goButton.MouseDown += new MouseEventHandler(goButton_MouseDown);
            main.goButton.MouseUp += new MouseEventHandler(goButton_MouseUp);
            main.refreshButton.Click += new EventHandler(refreshButton_Click);

            main.fileMenu.Closing += new ToolStripDropDownClosingEventHandler(contextMenuStrip1_Closing);
            main.propertiesToolStripMenuItem.Click += new EventHandler(propertiesToolStripMenuItem_Click);
            main.downloadMenuItem.Click += new EventHandler(downloadButton_Click);
            main.renameToolStripMenuItem.Click += new EventHandler(renameButton_Click);
            main.deleteToolStripMenuItem.Click += new EventHandler(deleteButton_Click);
            main.copyToolStripMenuItem.Click += new EventHandler(copyButton_Click);
            main.cutToolStripMenuItem.Click += new EventHandler(cutButton_Click);
            main.extractToolStripMenuItem.Click += new EventHandler(extractToolStripMenuItem_Click);
            main.compressToolStripMenuItem.Click += new EventHandler(compressToolStripMenuItem_Click);
            main.openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);

            main.pasteToolStripMenuItem.Click += new EventHandler(pasteButton_Click);
            main.newFolderToolStripMenuItem.Click += new EventHandler(newFolder_Click);
            main.uploadToolStripMenuItem.Click += new EventHandler(uploadButton_Click);

            main.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel4_LinkClicked);
            main.linkLabel4.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel4_LinkClicked);

            main.addressBar.KeyDown += new KeyEventHandler(addressBar_KeyDown);
        }

        void goButton_MouseUp(object sender, MouseEventArgs e) {
            main.goButton.Image = Properties.Resources.go;
        }

        private void goButton_MouseDown(object sender, MouseEventArgs e) {
            main.goButton.Image = Properties.Resources.go_selected;
        }

        private void addressBar_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                goButton_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        public void Activate() {
            main.listView1.View = View.LargeIcon;
            main.listView1.SelectedIndexChanged += new EventHandler(listView1_SelectedIndexChanged);
            main.listView1.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClick);
            main.listView1.MouseClick += new MouseEventHandler(listView1_MouseClick);
            main.listView1.ContextMenuStrip = main.fileManMenu;
            main.groupBox2.Visible = main.groupBox3.Visible = true;
            main.label4.Text = "File Manager";
            main.groupBox1.Text = "About";
            main.label6.Visible = true;
            main.addressBar.Visible = main.label5.Visible = main.goButton.Visible = true;
            main.listView1.Items.Clear();
            int roof = (main.addressBar.Location.Y + main.addressBar.Size.Height) + 10;
            main.listView1.Size = new Size(main.listView1.Size.Width, main.listView1.Size.Height - 89);
            main.listView1.Location = new Point(main.listView1.Location.X, roof);
            main.button2.Enabled = false;
            main.linkLabel2.Visible = main.linkLabel4.Visible = true;
        }

        public bool Update(TResponse response) {
            if (response.RespondedTo.StartsWith("GET")) {
                try {
                    File.WriteAllBytes(main.saveFileDialog1.FileName, response.ByteBase64);
                    main.Invoke(new MethodInvoker(delegate {
                        main.textBox3.Show();
                        if (openDownload) {
                            main.textBox3.Text = "The file has been downloaded and will be opened shortly.";
                        }
                        else {
                            main.textBox3.Text = "Download Successful";
                        }
                        main.timer2.Start();
                    }));
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, main.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                main.Invoke(new MethodInvoker(delegate {
                    main.progressBar1.Hide();
                    main.SetEnabledOriginal();
                    if (openDownload) {
                        openDownload = false;
                        using (System.Diagnostics.Process p = new System.Diagnostics.Process()) {
                            try {
                                p.StartInfo.FileName = main.saveFileDialog1.FileName;
                                p.Start();
                            }
                            catch {
                                MessageBox.Show("The file was downloaded, but it was unable to be opened. Check the 'Downloaded Files' folder to see the downloaded file.", main.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }));
                return true;
            }
            if (response.RespondedTo.StartsWith("PUT")) {
                main.Invoke(new MethodInvoker(delegate {
                    main.listView1.Enabled = main.groupBox2.Enabled =
                    main.groupBox3.Enabled = main.groupBox1.Enabled = true;
                }));
            }
            if (response.RespondedTo.StartsWith("CD")) { main.SendCommand(false, "PWD"); }
            if (response.RespondedTo.StartsWith("DIR")) {
                main.Invoke(SetDirList, response);
                return true;
            }
            if (response.RespondedTo.StartsWith("CMD")) {
                main.Invoke(SetFileList, response);
                return true;
            }
            if (response.RespondedTo.StartsWith("PWD")) {
                main.Invoke(new MethodInvoker(delegate { setPWD(response.Message); }));
                return true;
            }
            if (response.RespondedTo.StartsWith("7ZA")) {
                response.Original = new TMessage(response.Stream);
                response.Original.Process(response.Header);
                switch(response.Status) {
                    case 400:
                        response.Process(400, "7-Zip was unable to open due to bad syntax");
                        break;
                    case 500:
                        response.Process(500, "The archive was unable to be accessed due to an internal error");
                        break;
                    case 503:
                        response.Process(400, "The compression/extraction may not be complete.");
                        break;
                    default:
                        response.Process(200, "The archive has been handled");
                        break;
                }
                main.Invoke(showStatusTip, response);
                main.SendCommand(false, "PWD");
            }
            if (response.RespondedTo.StartsWith("COPY") ||
                response.RespondedTo.StartsWith("MOVE") ||
                response.RespondedTo.StartsWith("DEL") ||
                response.RespondedTo.StartsWith("RD") ||
                response.RespondedTo.StartsWith("PUT") ||
                response.RespondedTo.StartsWith("RECYCLE") ||
                response.RespondedTo.StartsWith("MD")) {
                    main.Invoke(showStatusTip, response);
                    main.SendCommand(false, "PWD");
            }
            return false;
        }

        public void Deactivate() {
            if (IsActive) {
                main.listView1.SelectedIndexChanged -= new EventHandler(listView1_SelectedIndexChanged);
                main.listView1.MouseDoubleClick -= new MouseEventHandler(listView1_MouseDoubleClick);
                main.listView1.MouseClick -= new MouseEventHandler(listView1_MouseClick);
                main.listView1.ContextMenuStrip = null;
                main.label6.Visible = false;
                main.linkLabel2.Visible = main.linkLabel4.Visible = false;
                main.listView1.Location = new Point(main.listView1.Location.X, 62);
                main.listView1.Size = new Size(main.listView1.Size.Width, main.listView1.Size.Height + 89);
                main.groupBox2.Visible = main.groupBox3.Visible = false;
                main.addressBar.Visible = main.goButton.Visible = main.label5.Visible = false;
                main.button2.Enabled = true;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            openToolStripMenuItem_Click(sender, e);
        }

        private bool openDownload = false;
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if (main.listView1.SelectedItems.Count == 1) {
                if (main.listView1.SelectedItems[0].ImageKey.Equals("folder")) {
                    main.SendCommand(true, "CD", main.listView1.SelectedItems[0].Text);
                }
                else {
                    openDownload = true;
                    main.saveFileDialog1.FileName = Application.StartupPath + "\\Temp\\" + main.listView1.SelectedItems[0].Text;
                    main.SendCommand(true, "GET", main.listView1.SelectedItems[0].Text);
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                main.listView1.ContextMenuStrip = null;
                main.fileMenu.Show(main.listView1, e.X, e.Y);
                main.extractToolStripMenuItem.Enabled =
                    (main.listView1.SelectedItems.Count == 1 && isExtractable(main.listView1.SelectedItems[0].Text));
            }
        }

        private bool isExtractable(string fileName) {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".zip")) {
                return true;
            }
            else if (fileName.EndsWith(".7z")) {
                return true;
            }
            else if (fileName.EndsWith(".tar")) {
                return true;
            }
            return false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (main.listView1.SelectedItems.Count == 1) {
                main.renameButton.Enabled =
                    main.copyButton.Enabled =
                    main.recycleButton.Enabled = main.cutButton.Enabled = true;
            }
            else {
                main.deleteButton.Enabled = main.renameButton.Enabled =
                    main.copyButton.Enabled = main.recycleButton.Enabled =
                    main.cutButton.Enabled = false;
            }
            main.deleteButton.Enabled = 
                main.downloadButton.Enabled = main.listView1.SelectedItems.Count > 0;
        }

        private void setListDir(TResponse response) {
            main.listView1.Items.Clear();
            if (response.Status == 203) {
                string list = "..\n" + response.Message.Replace("\r\n", "\n");
                foreach (string s in list.Split('\n')) {
                    if (s.Equals("")) {
                        continue;
                    }
                    ListViewItem item = new ListViewItem(s);
                    item.ImageKey = "folder";
                    main.listView1.Items.Add(item);
                }
            }
            else {
                ListViewItem item = new ListViewItem("..");
                item.ImageKey = "folder";
                main.listView1.Items.Add(item);
            }
            main.SendCommand(false, "CMD", "DIR", "/A:-D", "/B");
        }

        string currentDirectory = "";

        private void setPWD(string response) {
            currentDirectory = response;
            main.addressBar.Text = response;
            main.SendCommand(false, "DIR", "/A:D", "/B");
        }

        private delegate void ResponseDelegate(TResponse response);
        private void setListFile(TResponse response) {
            if (response.Status != 203) {
                return;
            }
            foreach (string s in response.Message.Replace("\r\n", "\n").Split('\n')) {
                string fileName = s.ToLower();
                if (fileName.Equals("")) {
                    continue;
                }
                ListViewItem item = new ListViewItem(s);
                item.ImageKey = "blank";
                if (fileName.EndsWith(".exe")) { item.ImageKey = "exe"; }
                if (fileName.EndsWith(".ini") || fileName.EndsWith(".inf") || fileName.EndsWith(".config") || fileName.EndsWith(".xml")) { item.ImageKey = "ini"; }
                if (fileName.EndsWith(".bat") || fileName.EndsWith(".cmd") || fileName.EndsWith(".nt")) { item.ImageKey = "bat"; }
                if (fileName.EndsWith(".txt")) { item.ImageKey = "txt"; }
                if (fileName.EndsWith(".doc") || fileName.EndsWith(".docx")) { item.ImageKey = "doc"; }
                if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx")) { item.ImageKey = "xls"; }
                if (fileName.EndsWith(".ppt") || fileName.EndsWith(".pptx")) { item.ImageKey = "ppt"; }
                if (fileName.EndsWith(".dll")) { item.ImageKey = "dll"; }
                if (fileName.EndsWith(".avi")) { item.ImageKey = "avi"; }
                if (fileName.EndsWith(".mp3")) { item.ImageKey = "mp3"; }
                if (fileName.EndsWith(".wma")) { item.ImageKey = "wma"; }
                if (fileName.EndsWith(".wmv")) { item.ImageKey = "wmv"; }
                if (fileName.EndsWith(".wav")) { item.ImageKey = "wav"; }
                if (fileName.EndsWith(".bmp") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".gif") || fileName.EndsWith(".png")) { item.ImageKey = "bmp"; }
                if (fileName.EndsWith(".jar")) { item.ImageKey = "java"; }
                if (fileName.EndsWith(".js")) { item.ImageKey = "script"; }
                if (fileName.EndsWith(".vbs")) { item.ImageKey = "script"; }
                if (fileName.EndsWith(".zip")) { item.ImageKey = "zip"; }
                main.listView1.Items.Add(item);
            }
            listView1_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void uploadButton_Click(object sender, EventArgs e) {
            if (main.openFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    string filename = main.openFileDialog1.FileName;
                    filename = filename.Remove(0, filename.LastIndexOf('\\') + 1);
                    if (fileExists(filename)) {
                        if (MessageBox.Show(main, "A file with the same name already exists. Would you like to upload the file with a different name?", main.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            == DialogResult.Yes) {
                                RunDialogue dialog = new RunDialogue(main, "New file name:", 
                                    "&Upload", "Upload file", filename, new RunDialogue.RunButtonClick(GetNewName), true);
                                dialog.ShowDialog();
                        }
                        return;
                    }
                    main.toolStripStatusLabel1.Text = "Uploading File...";
                    main.listView1.Enabled = main.groupBox2.Enabled =
                        main.groupBox3.Enabled = main.groupBox1.Enabled = false;
                    main.SendCommand(true, "PUT", filename,
                        Convert.ToBase64String(System.IO.File.ReadAllBytes(main.openFileDialog1.FileName)));
                }
                catch (Exception ex) {
                    MessageBox.Show(main, ex.Message, main.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GetNewName(string name) {
            main.toolStripStatusLabel1.Text = "Uploading File...";
            main.listView1.Enabled = main.groupBox2.Enabled =
                main.groupBox3.Enabled = main.groupBox1.Enabled = false;
            main.SendCommand(true, "PUT", name,
                Convert.ToBase64String(System.IO.File.ReadAllBytes(main.openFileDialog1.FileName)));
        }

        private bool fileExists(string filename) {
            foreach (ListViewItem item in listView1.Items) {
                if (item.Text.Equals(filename, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }
            return false;
        }

        private ListView listView1 {
            get {
                return main.listView1;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e) {
            main.SendCommand(true, "PWD");
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e) {
            string file = main.listView1.SelectedItems[0].Text;
            RunDialogue run = new RunDialogue(main, "Folder path:", "E&xtract", "Extract to folder",
                currentDirectory + "\\" + file.Remove(file.LastIndexOf('.')), 
                new RunDialogue.RunButtonClick(Extract), true);
            run.ShowDialog();
        }

        private void Extract(string fileName) {
            main.SendCommand(true, "7za", "x", "-ro" + fileName.Trim(), main.listView1.SelectedItems[0].Text);
        }

        private void compressToolStripMenuItem_Click(object sender, EventArgs e) {
            string file = main.listView1.SelectedItems[0].Text;
            int index = file.IndexOf('.');
            if (index > -1) {
                file = file.Remove(index);
            }
            RunDialogue run = new RunDialogue(main, "New Archive Name:", "&Compress", "Compress to archive",
                            file + ".zip", new RunDialogue.RunButtonClick(Compress), true);
            run.ShowDialog();
        }

        private void Compress(string fileName) {
            if (main.listView1.SelectedItems.Count == 0) {
                return;
            }
            if (!fileName.Contains('.')) {
                fileName += ".zip";
            }
            string[] fileList = new string[main.listView1.SelectedItems.Count];
            for (int i = 0; i < fileList.Length; i++) {
                fileList[i] = main.listView1.SelectedItems[i].Text;
            }
            string[] sevenZip = new string[] { "7za", "a", "-rtzip", fileName };
            string[] args = new string[sevenZip.Length + fileList.Length];
            sevenZip.CopyTo(args, 0);
            fileList.CopyTo(args, sevenZip.Length);
            main.SendCommand(true, args);
        }

        private void renameButton_Click(object sender, EventArgs e) {
            Forms.File_Manager.Rename r = new Tcpclient.Forms.File_Manager.Rename(main.mainClient.GetStream(), main, main.listView1.SelectedItems[0].Text);
            r.ShowDialog();
            main.SendCommand(false, "PWD");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void newFolder_Click(object sender, EventArgs e) {
            Forms.File_Manager.New n = new Tcpclient.Forms.File_Manager.New(main.mainClient.GetStream(), main);
            n.ShowDialog();
        }

        private void deleteButton_Click(object sender, EventArgs e) {
            string text;
            if (main.listView1.SelectedItems.Count > 1) {
                text = "Are you sure you want to delete " + main.listView1.SelectedItems.Count + " items?";
            }
            else {
                text = "Are you sure you want to delete '" + main.listView1.SelectedItems[0].Text + "'?";
            }
            if (MessageBox.Show(main, text + "\nThis cannot be undone!",
                main.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
                    return;
            }
            List<string> dirs = new List<string>();
            dirs.AddRange(new string[] { "RD", "/Q", "/S" });
            List<string> files = new List<string>();
            files.Add("DEL");
            foreach (ListViewItem item in main.listView1.SelectedItems) {
                if (item.ImageKey.Equals("folder")) {
                    dirs.Add(item.Text);
                }
                else {
                    files.Add(item.Text);
                }
            }
            if (dirs.Count - 3 > 0) {
                main.SendCommand(false, dirs.ToArray());
            }
            while (main.isWorking) ;
            if (files.Count - 1> 0) {
                main.SendCommand(false, files.ToArray());
            }
        }

        private void recycleButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show(main, "Are you sure you want to recycle '" + main.listView1.SelectedItems[0].Text + "' on the remote machine?",
                main.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes) {
                    main.SendCommand(true, "RECYCLE", main.listView1.SelectedItems[0].Text);
            }
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            main.listView1.ContextMenuStrip = main.fileManMenu;
        }

        private void downloadButton_Click(object sender, EventArgs e) {
            bool isDir = main.listView1.SelectedItems[0].ImageKey.Equals("folder");
            if (isDir || main.listView1.SelectedItems.Count > 1) {
                string selected = main.listView1.SelectedItems[0].Text;
                int period = selected.LastIndexOf('.');
                if (period > 1) {
                    selected = selected.Remove(period);
                }
                main.saveFileDialog1.FileName = selected + ".zip";
                main.saveFileDialog1.Filter = "Compressed Folders (*.zip)|*.zip|All Files (*.*)|*.*";
            }
            else {
                main.saveFileDialog1.FileName = main.listView1.SelectedItems[0].Text;
                int period = main.listView1.SelectedItems[0].Text.LastIndexOf('.');
                if (period > -1) {
                    string ext = main.saveFileDialog1.FileName.Substring(period);
                    main.saveFileDialog1.Filter = "*" + ext + "|*" + ext + "|All Files (*.*)|*.*";
                }
                else {
                    main.saveFileDialog1.Filter = "All Files (*.*)|*.*";
                }
            }
            if (main.saveFileDialog1.ShowDialog() == DialogResult.OK) {
                if (main.listView1.SelectedItems.Count == 1) {
                    if (isDir) {
                        main.SendCommand(true, "GETDIR", main.listView1.SelectedItems[0].Text);
                    }
                    else {
                        main.SendCommand(true, "GET", main.listView1.SelectedItems[0].Text);
                    }
                }
                else {
                    string[] args = new string[main.listView1.SelectedItems.Count + 1];
                    args[0] = "GETMULT";
                    for (int i = 0; i < main.listView1.SelectedItems.Count; i++) {
                        args[i + 1] = main.listView1.SelectedItems[i].Text;
                    }
                    main.SendCommand(true, args);
                }
                main.ShowProgress();
            }
        }

        private void button10_Click(object sender, EventArgs e) {
            if (main.listView1.SelectedItems.Count == 1) {
                listView1_MouseDoubleClick(sender, null);
            }
        }

        private string clipboard;
        private int clipBoardAction = 0;
        private void copyButton_Click(object sender, EventArgs e) {
            if (main.listView1.SelectedItems.Count == 1) {
                main.pasteButton.Enabled =
                    main.pasteToolStripMenuItem.Enabled = true;
                clipBoardAction = 1;
                clipboard = currentDirectory + "\\" + main.listView1.SelectedItems[0].Text;
            }
        }

        private void cutButton_Click(object sender, EventArgs e) {
            if (main.listView1.SelectedItems.Count == 1) {
                main.pasteButton.Enabled =
                    main.pasteToolStripMenuItem.Enabled = true;
                clipBoardAction = 2;
                clipboard = currentDirectory + "\\" + main.listView1.SelectedItems[0].Text;
            }
        }

        private void pasteButton_Click(object sender, EventArgs e) {
            if (clipBoardAction == 1) {
                main.SendCommand(true, "COPY", clipboard, currentDirectory);
            }
            else if (clipBoardAction == 2) {
                main.SendCommand(true, "MOVE", clipboard, currentDirectory);
                clipBoardAction = 0;
                main.pasteButton.Enabled =
                    main.pasteToolStripMenuItem.Enabled = false;
            }
        }

        private void goButton_Click(object sender, EventArgs e) {
            main.SendCommand(true, "CD", main.addressBar.Text);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            Forms.Properties p =
                new Forms.Properties((IPEndPoint)main.mainClient.Client.RemoteEndPoint,
                    currentDirectory + "\\" + main.listView1.SelectedItems[0].Text,
                    (main.listView1.SelectedItems[0].ImageKey == "folder"));
            p.ShowDialog();
            main.SendCommand(true, "PWD");
        }
    }
}
