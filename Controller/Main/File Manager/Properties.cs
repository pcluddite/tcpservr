using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class Properties : Form {
        private NetworkStream socket;
        private string directory;
        private string filename;
        private bool isDirectory;
        public Properties(IPEndPoint ep, string file, bool dir) {
            TcpClient client = new TcpClient();
            client.Connect(ep);
            socket = client.GetStream();
            isDirectory = dir;
            filename = file.Remove(0, file.LastIndexOf('\\') + 1);
            directory = file.Remove(file.LastIndexOf('\\'));
            InitializeComponent();
        }

        private void Properties_Load(object sender, EventArgs e) {
            backgroundWorker1.RunWorkerAsync();
            Enabled = false;
            TMessage tMsg = new TMessage(socket);
            tMsg.Send("GetFileInfo", directory + "\\" + filename);
            if (isDirectory) {
                label4.Text = "Item(s)";
            }
            nameTextBox.Text = filename;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(socket);
                SetInfoDelegate setFileInfo = new SetInfoDelegate(SetFileInfo);
                int len;
                while ((len = response.Receive()) != 0) {
                    if (len == -1) {
                        continue;
                    }

                    if (response.Status == 200 || response.Status == 204) {
                        TMessage nMsg = new TMessage(socket);
                        string path = "";
                        Invoke(new MethodInvoker(delegate { 
                            path = directory + "\\" + filename;
                        }));
                        if (response.RespondedTo.StartsWith("GETFILEINFO")) {
                            Invoke(setFileInfo, response);
                            continue;
                        }
                        else if (response.RespondedTo.StartsWith("SETCREATEDDATE")) {
                            nMsg.Send("SetModifiedDate", path, modifiedTextBox.Text);
                            continue;
                        }
                        else if (response.RespondedTo.StartsWith("SETMODIFIEDDATE")) {
                            nMsg.Send("SetAccessDate", path, accessedTextBox.Text);
                            continue;
                        }
                        else if (response.RespondedTo.StartsWith("SETACCESSDATE")) {
                            Invoke(new MethodInvoker(delegate {
                                nMsg.Send("SetFileAttributes", path, attributes);
                            }));
                            continue;
                        }
                        else if (response.RespondedTo.StartsWith("SETFILEATTRIBUTES")) {
                            if (nameTextBox.Text.Equals(filename)) {
                                Invoke(new MethodInvoker(delegate {
                                    MessageBox.Show(this, "File properties have been saved!", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }));
                                break;
                            }
                            else {
                                nMsg.Send("Ren", path, nameTextBox.Text);
                                continue;
                            }
                        }
                        else if (response.RespondedTo.StartsWith("REN")) {
                            Invoke(new MethodInvoker(delegate {
                                MessageBox.Show(this, "File properties have been saved!", "Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                            break;
                        }
                    }

                    MessageBoxIcon icon = MessageBoxIcon.Information;

                    if (response.Status >= 400) {
                        icon = MessageBoxIcon.Warning;
                    }
                    if (response.Status >= 500) {
                        icon = MessageBoxIcon.Error;
                    }

                    MessageBox.Show(response.Status + " " + response.Message,
                        icon.ToString(),
                        MessageBoxButtons.OK, icon);

                    Invoke(new MethodInvoker(delegate { Enabled = true; }));
                }
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation")) {
                    return;
                }
                Program.Log(ex, "Properties.Receive");
            }
        }

        private delegate void SetInfoDelegate(TResponse response);

        private void SetFileInfo(TResponse response) {
            Enabled = true;
            string[] fi = response.Message.Replace("\r\n", "\n").Split('\n');
            for (int i = 0; i < fi.Length - 1; i++) {
                int o;
                while (!int.TryParse(fi[i].Substring(0, 1), out o)) {
                    fi[i] = fi[i].Remove(0, 1);
                }
            }
            createdTextBox.Text = fi[0];
            modifiedTextBox.Text = fi[1];
            accessedTextBox.Text = fi[2];
            sizeTextBox.Text = fi[3];
            if (!isDirectory) {
                SimpleSize();
            }
            if (fi[4].Length >= 12) {
                fi[4] = fi[4].Remove(0, 12);
            }
            else {
                return;
            }
            archivedCheck.Checked = fi[4].Contains('a');
            compressedCheck.Checked = fi[4].Contains('c');
            encryptedCheck.Checked = fi[4].Contains('e');
            hiddenCheck.Checked = fi[4].Contains('h');
            readOnlyCheck.Checked = fi[4].Contains('r');
            systemCheck.Checked = fi[4].Contains('s');
        }

        private void SimpleSize() {
            double size = double.Parse(sizeTextBox.Text);
            int unit = 0;
            double converted = size;
            while (converted > 1024) {
                converted = converted / 1024;
                unit++;
                if (unit > 2) {
                    break;
                }
            }
            if (converted < 1) {
                converted += size % 1024;
            }
            sizeTextBox.Text = Math.Round(converted, 2).ToString();
            switch (unit) {
                default:
                    label4.Text = "Bytes";
                    break;
                case 1:
                    label4.Text = "KB";
                    break;
                case 2:
                    label4.Text = "MB";
                    break;
                case 3:
                    label4.Text = "GB";
                    break;
            }
        }

        private string attributes;
        private void button1_Click(object sender, EventArgs e) {
            attributes = "";
            attributes += archivedCheck.Checked     ? "a" : "";
            attributes += compressedCheck.Checked   ? "c" : "";
            attributes += encryptedCheck.Checked    ? "e" : "";
            attributes += hiddenCheck.Checked       ? "h" : "";
            attributes += readOnlyCheck.Checked     ? "r" : "";
            attributes += systemCheck.Checked       ? "s" : "";
            TMessage tMsg = new TMessage(socket);
            tMsg.Send("SetCreatedDate", directory + "\\" + filename, createdTextBox.Text);
        }

        private void Properties_FormClosing(object sender, FormClosingEventArgs e) {
            socket.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Close();
        }
    }
}
