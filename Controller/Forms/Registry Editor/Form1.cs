using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor {
    public partial class Form1 : Form {
        TcpClient client;
        string Host;
        public string newKeyName = "";
        int nameLen;
        public Form1(string host, int port) {
            client = new TcpClient(host, port);
            InitializeComponent();
            Host = host;
            nameLen = host.Length + 1;
            backgroundWorker1.RunWorkerAsync();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            if (!treeView1.SelectedNode.FullPath.Equals(treeView1.TopNode.FullPath)) {
                treeView1.SelectedNode.Nodes.Clear();
                toolStripStatusLabel1.Text = treeView1.SelectedNode.FullPath;
                TMessage tMsg = new TMessage(client.GetStream());
                tMsg.Send("RegEnumKeys", e.Node.FullPath.Remove(0, nameLen));
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(client.GetStream());
                SetTreeView setValues = new SetTreeView(SetValueNames);
                SetTreeView setKeys = new SetTreeView(SetKeyNames);
                SetTreeView refresh = new SetTreeView(Refresh);
                while (response.Receive() != 0) {
                    if (!response.Status.ToString().StartsWith("2")) {
                        MessageBox.Show(response.Message + "\r\n(An error code " + response.Status + " in response to command '" + response.Name.ToUpper() + "'.)", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    switch (response.Name.ToUpper()) {
                        case "REGENUMVALUES":
                            Invoke(setValues, response);
                            break;
                        case "REGENUMKEYS":
                            Invoke(setKeys, response);
                            break;
                        case "REGRENAMEKEY":
                            Invoke(new MethodInvoker(delegate { treeView1.SelectedNode.Text = newKeyName; }));
                            break;
                        default:
                            MessageBox.Show(response.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Invoke(refresh, response);
                            break;
                    }
                }            
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation")) { return; }
                Program.Log(ex, "RegistrySocket");
            }
        }

        private void Refresh(TResponse response) {
            treeView1_AfterSelect(null, new TreeViewEventArgs(treeView1.SelectedNode));
        }

        private delegate void SetTreeView(TResponse response);
        private void SetValueNames(TResponse response) {
            listView1.Items.Clear();
            foreach (string s in response.Message.Replace("\r\n", "\n").Split('\n')) {
                if (!s.Trim().Equals("")) {
                    TMessage entry = new TMessage();
                    entry.Process(s);
                    string[] subItems = entry.Args;
                    if (subItems.Length < 4) {
                        Program.Log("Cannot process message received by the RegistrySocket", "Registry.Update()");
                        continue;
                    }
                    try {
                        ListViewItem item = new ListViewItem(subItems[1]);
                        switch (entry.Args[2].ToLower()) {
                            case "dword": subItems[2] = "REG_DWORD"; break;
                            case "qword": subItems[2] = "REG_QWORD"; break;
                            case "string": subItems[2] = "REG_SZ"; break;
                            case "multistring": subItems[2] = "REG_MULTI_SZ"; break;
                            case "expandstring": subItems[2] = "REG_EXPAND_SZ"; break;
                            case "binary": subItems[2] = "REG_BINARY"; break;
                        }
                        if (subItems[2].ToLower().Contains("word") || subItems[2].ToLower().Contains("binary")) { item.ImageIndex = 1; }
                        if (subItems[2].ToLower().Contains("word")) {
                            subItems[3] = "0x" + Convert.ToString(int.Parse(subItems[3]), 16).PadLeft(8, '0').ToUpper() + " (" + subItems[3] + ")";
                        }
                        if (subItems[2].ToLower().Contains("sz")) { item.ImageIndex = 2; }
                        item.SubItems.Add(subItems[2]);
                        item.SubItems.Add(subItems[3]);
                        listView1.Items.Add(item);
                    }
                    catch (Exception ex) {
                        Program.Log(ex, "Registry.Update()");
                    }
                }
            }
        }


        private void SetKeyNames(TResponse response) {
            string path = response.Header.Remove(0, response.Name.Length + 4).Replace("\"", "").Trim();
            string[] keys = response.Message.Replace("\r\n", "\n").Split('\n');
            foreach (string key in keys) {
                if (!key.Trim().Equals("")) {
                    EnumeratePath(path + "\\" + key);
                }
            }
            TMessage tMsg = new TMessage(client.GetStream());
            tMsg.Send("RegEnumValues", path, "/s");
        }

        private TreeNode GetRootNode() {
            foreach (TreeNode node in treeView1.Nodes) {
                if (node.Parent == null) {
                    return node;
                }
            }
            return null;
        }

        private void EnumeratePath(string xpath) {
            string[] path = xpath.Split('\\');
            TreeNode lastNode = GetRootNode();
            foreach (string key in path) {
                if (!lastNode.Nodes.ContainsKey(key)) {
                    lastNode.Nodes.Add(key, key);
                }
                lastNode = lastNode.Nodes[key];
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("REG_SZ") == 0) {
                String str = new String(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    listView1.SelectedItems[0].Text,
                    listView1.SelectedItems[0].SubItems[2].Text,
                    client.GetStream(),
                    this, "STRING", false);
                str.ShowDialog();
                return;
            }
            if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("REG_EXPAND_SZ") == 0) {
                String str = new String(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    listView1.SelectedItems[0].Text,
                    listView1.SelectedItems[0].SubItems[2].Text,
                    client.GetStream(),
                    this, "EXPANDSTRING", false);
                str.ShowDialog();
                return;
            }
            if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("REG_DWORD") == 0) {
                DWord dword = new DWord(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    listView1.SelectedItems[0].Text,
                    listView1.SelectedItems[0].SubItems[2].Text,
                    client.GetStream(),
                    this, false);
                dword.ShowDialog();
                return;
            }
            if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("REG_QWORD") == 0) {
                QWord qword = new QWord(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    listView1.SelectedItems[0].Text,
                    listView1.SelectedItems[0].SubItems[2].Text,
                    client.GetStream(),
                    this, false);
                qword.ShowDialog();
                return;
            }
            if (listView1.SelectedItems[0].SubItems[1].Text.CompareTo("REG_MULTI_SZ") == 0) {
                MultiString multi = new MultiString(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    listView1.SelectedItems[0].Text,
                    listView1.SelectedItems[0].SubItems[2].Text,
                    client.GetStream(),
                    this, false);
                multi.ShowDialog();
                return;
            }
            beta();
        }

        private void Form1_Load(object sender, EventArgs e) {
            BeginInvoke(new MethodInvoker(delegate {
                treeView1.TopNode.Text = Host;
                treeView1.TopNode.Expand();
            }));
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                listView1.ContextMenuStrip = null;
                contextMenuStrip2.Show(listView1, new Point(e.X, e.Y));
            }
        }

        private void contextMenuStrip2_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
            listView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e) {
            listView1_MouseDoubleClick(sender, new MouseEventArgs(MouseButtons.Left, 1, 5, 5, 0));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show(this, "Deleting certain registry values could cause system instability. Are you sure you want to permanently delete this value?", "Confirm Value Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.No) { return; }
            TMessage tMsg = new TMessage(client.GetStream());
            string path = treeView1.SelectedNode.FullPath.Remove(0, treeView1.Nodes[0].Text.Length + 1);
            tMsg.Send("REGDELETE", path, listView1.SelectedItems[0].Text);
        }

        private void stringValueToolStripMenuItem_Click(object sender, EventArgs e) {
            String str = new String(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    "", "", client.GetStream(),
                    this, "STRING", true);
            str.ShowDialog();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e) {
            Rename ren = new Rename(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                listView1.SelectedItems[0].Text, this, client.GetStream(), false);
            ren.ShowDialog();
        }

        private void binaryValueToolStripMenuItem_Click(object sender, EventArgs e) {
            beta();
        }

        void beta() {
            MessageBox.Show(this, "This value type is currently not supported.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dWORD32BitToolStripMenuItem_Click(object sender, EventArgs e) {
            DWord dword = new DWord(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    "", "0x00000000 (0)",
                    client.GetStream(),
                    this, true);
            dword.ShowDialog();
        }

        private void qWORD64bitValueToolStripMenuItem_Click(object sender, EventArgs e) {
            QWord qword = new QWord(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    "", "0x00000000 (0)",
                    client.GetStream(),
                    this, true);
            qword.ShowDialog();
        }

        private void multiStringValueToolStripMenuItem_Click(object sender, EventArgs e) {
            MultiString multi = new MultiString(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    "", "", client.GetStream(),
                    this, true);
            multi.ShowDialog();
        }

        private void expandableStringValueToolStripMenuItem_Click(object sender, EventArgs e) {
            String str = new String(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                    "", "", client.GetStream(),
                    this, "EXPANDSTRING", true);
            str.ShowDialog();
        }

        private void keyToolStripMenuItem_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client.GetStream());
            Key key = new Key(treeView1.SelectedNode.FullPath.Remove(0, nameLen),
                client.GetStream());
            key.ShowDialog();
            tMsg.Send("RegEnumKeys", treeView1.SelectedNode.FullPath.Remove(0, nameLen));
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client.GetStream());
            TreeNode parent = treeView1.SelectedNode.Parent;
            string parentPath = parent.FullPath.Remove(0, treeView1.TopNode.Text.Length + 1);
            if (parentPath.StartsWith("\\"))
                parentPath = parentPath.Remove(0, 1);

            if (MessageBox.Show(this, "Are you sure you want to delete this key and all of its subkeys?", "Confirm Key Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.No) { return; }
            tMsg.Send("REGDELETEKEY", treeView1.SelectedNode.FullPath.Remove(0, nameLen));
            treeView1.SelectedNode = parent;
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e) {
            TreeNode parent = treeView1.SelectedNode.Parent;
            Rename ren = new Rename(parent.FullPath.Remove(0, nameLen),
                treeView1.SelectedNode.Text, this, client.GetStream(), true);
            ren.ShowDialog();
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                contextMenuStrip3.Show(treeView1, e.X, e.Y);
            }
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e) {
            if (treeView1.SelectedNode != null) {
                treeView1.SelectedNode.Expand();
            }
        }

        private void copyKeyNameToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetData(DataFormats.StringFormat, toolStripStatusLabel1.Text.Remove(0, nameLen));
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.Close();
        }
    }
}
