using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Tcpclient.Components;

namespace Tcpclient.Window_Manager {
    public partial class Details : Form {
        bool receiving = false;
        public ListViewItem item;
        TcpClient client = new TcpClient();
        int quality = 50;
        Forms.Window_Manager.SendText t;
        Main mainForm;
        public Details(ListViewItem item, IPEndPoint EP, int Quality, Main m) {
            mainForm = m;
            t = new Tcpclient.Forms.Window_Manager.SendText(this);
            quality = Quality;
            this.item = item;
            client.Connect(EP);
            InitializeComponent();
        }

        private void Details_Load(object sender, EventArgs e) {
            textBox1.Text = item.Text;
            int state = int.Parse(item.SubItems[1].Text);
            string stateS = "";
            if ((state & 1) == 1) { stateS += "Existing"; }
            if ((state & 2) == 2) { stateS += ", Visible"; button5.Text = "Hide"; } else { button5.Text = "Show"; }
            if ((state & 4) == 4) { stateS += ", Enabled"; }
            if ((state & 8) == 8) { stateS += ", Active"; }
            if ((state & 16) == 16) { stateS += ", Minimized"; }
            if ((state & 32) == 32) { stateS += ", Maximized"; }
            textBox3.Text = stateS;
            numericUpDown4.Value = decimal.Parse(item.SubItems[3].Text.Split('x')[0]);
            numericUpDown3.Value = decimal.Parse(item.SubItems[3].Text.Split('x')[1]);
            numericUpDown1.Value = decimal.Parse(item.SubItems[2].Text.Split(',')[0].Replace("(", ""));
            numericUpDown2.Value = decimal.Parse(item.SubItems[2].Text.Split(',')[1].Replace(")", ""));
            backgroundWorker1.RunWorkerAsync();
            WinPicture(item.Name);
        }

        private void button2_Click(object sender, EventArgs e) {
            if (Send(false, "WinMove", item.Name, numericUpDown1.Value.ToString(), numericUpDown2.Value.ToString())) {
                button12_Click(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (item.Text.CompareTo(textBox1.Text) != 0 && item.Text.Trim().CompareTo("") != 0) {
                if (Send(false, "WinSetTitle", item.Name, textBox1.Text)) {
                    button12_Click(sender, e);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (Send(false, "WinSize", item.Name, numericUpDown4.Value.ToString(), numericUpDown3.Value.ToString())) {
                button12_Click(sender, e);
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            if (Send(false, "WinActivate", item.Name)) {
                button12_Click(sender, e);
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            bool shouldDoIt;
            if (button5.Text.CompareTo("Hide") == 0) {
                shouldDoIt = Send(false, "WinSetState", item.Name, "SW_HIDE");
            }
            else {
                shouldDoIt = Send(false, "WinSetState", item.Name, "SW_SHOW");
            }
            if (shouldDoIt) {
                button12_Click(sender, e);
            }
        }

        private void button7_Click(object sender, EventArgs e) {
            if (Send(false, "WinSetState", item.Name, "SW_RESTORE")) {
                button12_Click(sender, e);
            }
        }

        private void button8_Click(object sender, EventArgs e) {
            if (Send(false, "WinSetState", item.Name, "SW_MAXIMIZE")) {
                button12_Click(sender, e);
            }
        }

        private void button10_Click(object sender, EventArgs e) {
            if (Send(false, "WinClose", item.Name)) {
                button12_Click(sender, e);
            }
        }

        private void button11_Click(object sender, EventArgs e) {
            if (Send(false, "WinKill", item.Name)) {
                button12_Click(sender, e);
            }
        }

        private void textBox1_Enter(object sender, EventArgs e) {
            this.AcceptButton = button1;
        }

        private void numericUpDown1_Enter(object sender, EventArgs e) {
            this.AcceptButton = button2;
        }

        private void numericUpDown4_Enter(object sender, EventArgs e) {
            this.AcceptButton = button3;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(client.GetStream());
                UpdateMethod update = new UpdateMethod(Update);
                PictureUpdateMethod pictureUpdate = new PictureUpdateMethod(UpdatePicture);
                int len;
                while ((len = response.Receive()) != 0) {
                    if (len == -1) {
                        continue;
                    }
                    receiving = false;

                    if (response.Status == 404) {
                        Invoke(new MethodInvoker(delegate {
                            Hide();
                            MessageBox.Show(this, "This window no longer exists", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                        break;
                    }
                    if (!response.Status.ToString().StartsWith("2")) {
                        MessageBox.Show(response.Message + "\n(An error code " + response.Status + " in response to command '" + response.RespondedTo + "'.)", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    string responded = response.RespondedTo;

                    if (response.RespondedTo.StartsWith("WININFO")) {
                        Invoke(update, response.Message);
                    }
                    if (response.RespondedTo.StartsWith("WINPICTURE")) {
                        if (response.ByteBase64 != null) {
                            Invoke(pictureUpdate, response.ByteBase64);
                        }
                        else {
                            MessageBox.Show("It appears that the stream is currently backed up. Please wait a moment for all messages to be received and processed.",
                                "Window Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex) {
                if (!ex.Message.Contains("blocking operation") && !ex.Message.Contains("disposed object")) {
                    MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void response_Updated(decimal value) {
            if (value > 100) { value = 100; }
            progressBar1.Value = (int)value;
        }

        private delegate void PictureUpdateMethod(byte[] img);
        private void UpdatePicture(byte[] img) {
            try {
                progressBar1.Hide();
                using (Stream s = new MemoryStream(img)) {
                    pictureBox1.Image = Bitmap.FromStream(s);
                }
            }
            catch {
                Program.Log("Could not load bytes into graphics", "WindowDetails.Refresh()");
            }
        }

        private void WinPicture(string hwnd) {
            if (Send("WinPicture", hwnd, quality.ToString())) {
                progressBar1.Value = 0;
                progressBar1.Show();
            }
        }

        delegate void UpdateMethod(string response);
        private void Update(string response) {
            string[] info = response.Replace("\r\n", "\n").Split('\n');
            if (info.Length < 5) {
                Program.Log("The response received was unable to be processed by the update method.", "WindowDetails.Refresh()");
                return;
            }
            textBox1.Text = info[0];
            WinPicture(info[4]);
            int state = int.Parse(info[1]);
            string stateS = "";
            if ((state & 1) == 1) { stateS += "Existing"; }
            if ((state & 2) == 2) { stateS += ", Visible"; button5.Text = "Hide"; } else { button5.Text = "Show"; }
            if ((state & 4) == 4) { stateS += ", Enabled"; }
            if ((state & 8) == 8) { stateS += ", Active"; }
            if ((state & 16) == 16) { stateS += ", Minimized"; }
            if ((state & 32) == 32) { stateS += ", Maximized"; }
            textBox3.Text = stateS;
            if (stateS.CompareTo("") == 0) {
                Hide();
                timer1.Start();
                mainForm.textBox3.Show();
                mainForm.textBox3.Text = "The window no longer exists";
            }
            numericUpDown4.Value = decimal.Parse(info[2].Remove(0, 6).Split('x')[0]);
            numericUpDown3.Value = decimal.Parse(info[2].Remove(0, 6).Split('x')[1]);
            numericUpDown1.Value = decimal.Parse(info[3].Remove(0, 10).Split(',')[0].Replace("(", ""));
            numericUpDown2.Value = decimal.Parse(info[3].Remove(0, 10).Split(',')[1].Replace(")", ""));
        }

        private void Details_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                if (!t.IsDisposed) {
                    t.Close();
                }
                client.Client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch {
            }
        }

        public void button12_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client.GetStream());
            receiving = true;
            tMsg.Send("WinInfo", item.Name);
        }

        public bool Send(params string[] args) {
            return Send(true, args);
        }

        public bool Send(bool warn, params string[] args) {
            if (receiving) {
                if (warn) {
                    MessageBox.Show(this, "The client is still awaiting a response from the server. Please wait.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }
            else {
                TMessage tMsg = new TMessage(client.GetStream());
                receiving = true;
                tMsg.Send(args);
                return true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            try {
                string filename = Path.GetTempFileName().Replace(".tmp", ".png");
                pictureBox1.Image.Save(filename);
                Thread t = new Thread(new ParameterizedThreadStart(openPicture));
                t.Start(filename);
            }
            catch {
                MessageBox.Show(this, "Could not save the image to the temp folder. Check your credentials and try again.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void openPicture(object path) {
            Process p = new Process();
            p.StartInfo.FileName = (string)path;
            try {
                p.Start();
            }
            catch {
                MessageBox.Show("Unable to open the screenshot in your default viewer. Check the application associated with the PNG extension.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            if (Send(false, "WinSetState", item.Name, "SW_MINIMIZE")) {
                button12_Click(sender, e);
            }

        }

        private void button9_Click(object sender, EventArgs e) {
            if (Send(false, "RemoveX", item.Name)) {
                button12_Click(sender, e);
            }
        }

        private void button13_Click(object sender, EventArgs e) {
            //send("send \"" + Program.replace(textBox2.Text) + "\"", false);
            button12_Click(sender, e);
        }

        private void button14_Click(object sender, EventArgs e) {
            if (t.Visible) { t.Activate(); }
            if (t.IsDisposed) { t = new Tcpclient.Forms.Window_Manager.SendText(this); }
            t.Show();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            mainForm.textBox3.Hide();
            Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.pictureBox1.Image == null) {
                MessageBox.Show(this, "There is no image to save.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                    MessageBox.Show(this, "The screen shot has been saved.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show(this, "Unable to save picture:\r\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.Close();
        }
    }
}