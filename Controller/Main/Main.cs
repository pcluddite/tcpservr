using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Tcpclient.Components;
using tbas = TbasicOld.Components;

namespace Tcpclient
{
    public partial class Main : Form {
        Hosts hosts;
        string host;
        int port;
        string name;
        public TcpClient mainClient;
        public TcpClient messageClient;
        public bool isWorking = false;
        private FileManager fileMan;
        private Tcpclient.Forms.Pipes pipesForm;

        #region Form

        public Main(string Host, int Port, Hosts Form, string Name) {
            Program.Settings = new Settings(Name);
            name = Name;
            port = Port;
            host = Host;
            hosts = Form;
            mainClient = new TcpClient(host, port);
            messageClient = new TcpClient(host, port);
            pipesForm = new Forms.Pipes(this);
            if (Directory.Exists("Downloaded Files")) {
                Directory.Move("Downloaded Files", "Temp");
            }
            if (!Directory.Exists("Temp")) {
                Directory.CreateDirectory("Temp");
            }
            InitializeComponent();
            try {
                string key = Program.Settings.Get("PrivateKey");
                if (key != null) {
                    using (tbas.TcpSecure security = new tbas.TcpSecure("7CyveqhEMtSG4eVm")) {
                        Program.Password = Encoding.UTF8.GetString(security.Decrypt(Convert.FromBase64String(key)));
                    }
                }
            }
            catch {
            }
        }

        private void Main_Load(object sender, EventArgs e) {
            Text = name + " - TcpServr Controller";
            fileMan = new FileManager(this);
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            linkLabel2.Links.Add(0, linkLabel2.Text.Length, "http://www.visualpharm.com/");
            linkLabel4.Links.Add(0, linkLabel4.Text.Length, "http://tatice.deviantart.com/");
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
            timer1.Start();
            Enabled = false;
            if (!Program.Password.Equals("")) {
                forgetPasswordToolStripMenuItem.Visible = true;
            }
            SendCommand(false, "HELLO");
        }

        bool closeHost = true;
        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            mainClient.Client.Shutdown(SocketShutdown.Both);
            mainClient.Close();
            messageClient.Client.Shutdown(SocketShutdown.Both);
            messageClient.Close();
            if (closeHost) { 
                hosts.Close();
            }
        }

        #endregion

        public int SendCommand(bool showWorking, params string[] s) {
            try {
                if (isWorking) {
                    if (showWorking) {
                        MessageBox.Show(this, "The stream is being used. The message has not yet been sent.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return -1;
                }
                else {
                    isWorking = true;
                    TMessage tMsg = new TMessage(mainClient.GetStream());
                    tMsg.Send(s);
                    return 1;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Program.Log(ex, "MainSocket.Send()");
                Close();
                return 0;
            }
        }

        delegate void StringDelegate(string s);

        private void button3_Click(object sender, EventArgs e) {
            Forms.Registry_Editor.Form1 form = new Tcpclient.Forms.Registry_Editor.Form1(host, port);
            form.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(mainClient.GetStream());
                StringDelegate setWindowList = new StringDelegate(setList);
                StringDelegate setToolStrip = new StringDelegate(setToolstrip);
                int len;
                while ((len = response.Receive()) != 0) {
                    isWorking = false;
                    if (len == -1) {
                        continue;
                    }

                    if (response.RespondedTo.StartsWith("HELLO")) {
                        Invoke(new MethodInvoker(delegate { 
                            Enabled = true;
                            Activate();
                            Focus();
                            BringToFront();
                            timer1_Tick(null, EventArgs.Empty);
                        }));
                    }

                    if (response.RespondedTo.StartsWith("WINLISTINFO")) {
                        if (response.Status == 200 || response.Status == 204) {
                            Invoke(setWindowList, response.Message);
                            continue;
                        }
                    }

                    if (!fileMan.Update(response)) {
                        Invoke(setToolStrip, response.Message.Replace("\r\n", "\n").Split('\n')[0]);
                    }
                }
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation")) { return; }
                Program.Log(ex, "MainSocket");
            }
        }

        #region File Manager

        public void ShowProgress() {
            SetControlsEnabled(false);
            progressBar1.Size = new System.Drawing.Size(this.Size.Width / 5, progressBar1.Size.Height);
            progressBar1.Location = new Point((this.ClientRectangle.Width - progressBar1.Width) / 2, (this.ClientRectangle.Height - progressBar1.Height) / 2);
            progressBar1.Visible = true;
        }
        
        private void button2_Click(object sender, EventArgs e) {
            bool timer1Enabled;
            if (timer1Enabled = timer1.Enabled) { 
                timer1.Stop();
            }
            if (SendCommand(true, "PWD") == 1) {
                listView1.View = View.LargeIcon;

                hideWindowManager();

                fileMan.Activate();
            }
            else {
                if (timer1Enabled) {
                    timer1.Start();
                }
            }
        }

        void setToolstrip(string text) {
            toolStripStatusLabel1.Text = text;
        }

        private Dictionary<string, bool> original;
        public void SetControlsEnabled(bool enabled) {
            original = new Dictionary<string, bool>();
            foreach (Control c in this.Controls) {
                original.Add(c.Name, c.Enabled);
                c.Enabled = enabled;
            }
        }

        public void SetEnabledOriginal() {
            if (original != null) {
                foreach (var v in original) {
                    Controls[v.Key].Enabled = v.Value;
                }
            }
        }


        #endregion

        private void timer2_Tick(object sender, EventArgs e) {
            textBox3.Hide();
            textBox3.ForeColor = Color.Blue;
            timer2.Stop();
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e) {
            textBox3.Hide();
            textBox3.ForeColor = Color.Blue;
            timer2.Stop();
        }

        private void button14_Click(object sender, EventArgs e) {
            GetScreen g = new GetScreen(new TcpClient(host, port), "GETSCREEN", (int)numericUpDown1.Value);
            g.Show();
        }

        private void button4_Click(object sender, EventArgs e) {
            bool timerEnabled = timer1.Enabled;
            if (timerEnabled) {
                timer1.Stop();
            }
            pipesForm.ShowDialog();
            if (timerEnabled) {
                timer1_Tick(sender, e);
                timer1.Start();
            }
            Activate();
        }

        void beta() {
            MessageBox.Show(this, "This function is not available in beta.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.Close();
        }

        private void webCamToolStripMenuItem_Click(object sender, EventArgs e) {
            GetScreen gs = new GetScreen(new TcpClient(host, port), "WEBCAMCAPTURE", (int)numericUpDown1.Value);
            gs.Show();
        }

        private void clearTemporaryFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show(this, "Are you sure you want to delete all temporary files?\n(This cannot be undone!)",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                    return;
            }
            List<string> errors = new List<string>();
            int count = -1;
            if (Directory.Exists(Application.StartupPath + "\\temp")) {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\temp");
                for (count = 0; count < files.Length; count++) {
                    try {
                        File.Delete(files[count]);
                    }
                    catch {
                        errors.Add(files[count].Substring(files[count].LastIndexOf('\\')));
                    }
                }
            }
            if (errors.Count > 0) {
                MessageBox.Show(this, "The following files could not be deleted:\n" +
                    string.Join("\n", errors.ToArray()), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (count > 0) {
                MessageBox.Show(this, "The temp folder has been cleared. " + count + " file(s) were deleted.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show(this, "There were no files to delete in the temp folder.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void forgetPasswordToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show(this, "Are you sure you want to forget this password?\nYou will be required to enter it again the next time you connect.",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                    Program.Settings.DeleteKey("PrivateKey");
                    forgetPasswordToolStripMenuItem.Visible = false;
            }
        }
    }
}
