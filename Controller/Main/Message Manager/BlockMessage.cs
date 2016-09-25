using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class BlockMessage : Form {
        NetworkStream socket;

        public BlockMessage(string host, int port) {
            TcpClient client = new TcpClient();
            client.Connect(host, port);
            socket = client.GetStream();

            InitializeComponent();

            backgroundWorker1.RunWorkerAsync();
            Send("PipeExists", "BLOCKMESSAGEPIPE");
        }

        private void button1_Click(object sender, EventArgs e) {
            if (button1.Text.Equals("&Block")) {
                Send("BlockMessage", textBox1.Text);
                button1.Text = "Un&block";
                button2.Show();
            }
            else {
                Send("PipeUse", "BLOCKMESSAGEPIPE", "Close");
                button1.Text = "&Block";
                button2.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Send("PipeUse", "BLOCKMESSAGEPIPE", "SetText", textBox1.Text);
        }

        private void Send(params object[] args) {
            if (isWorking) {
                MessageBox.Show(this, "A message is currenlty being sent.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                TMessage tMsg = new TMessage(socket);
                tMsg.Send(args);
                isWorking = true;
            }
        }

        private delegate void SetMessageMethod(string text);
        private void SetBlockMessage(string text) {
            this.Enabled = true;
            button1.Text = "Unblock";
            button2.Show();
            textBox1.Text = text;
        }

        private bool isWorking = false;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(socket);
                SetMessageMethod setMessage = new SetMessageMethod(SetBlockMessage);
                int len;
                while ((len = response.Receive()) != 0) {
                    isWorking = false;
                    if (len == -1) {
                        continue;
                    }
                    if (response.RespondedTo.StartsWith("PIPEEXISTS")) {
                        if (bool.Parse(response.Message)) {
                            Send("PIPEUSE", "BLOCKMESSAGEPIPE", "GETTEXT");
                        }
                    }
                    if (response.RespondedTo.StartsWith("GETTEXT")) {
                        Invoke(setMessage, response.Message);
                    }
                }
            }
            catch (Exception ex) {
                Program.Log(ex.Message, "BlockMessage.Form");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (!IsDisposed) {
                Close();
            }
        }

        private void BlockMessage_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
    }
}
