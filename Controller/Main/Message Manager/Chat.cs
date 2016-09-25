using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Tcpclient.Components;

namespace Tcpclient {
    public partial class chat : Form {
        NetworkStream client;
        string name;
        bool waiting;
        bool exists;

        public chat(TcpClient Client, string Name, bool exists) {
            this.name = "Other: ";
            this.exists = exists;
            name = Name + ": ";
            client = Client.GetStream();
            InitializeComponent();
        }

        private void chat_Load(object sender, EventArgs e) {
            if (exists) {
                waiting = true;
                TMessage tMsg = new TMessage(client);
                tMsg.Send("PipeUse", "CHATPIPE", "GetText");
                button1.Text = "Send";

                timer1.Start();
            }
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(client);
                int len;
                while ((len = response.Receive()) != 0) {
                    waiting = false;
                    if (len == -1) {
                        continue;
                    }

                    if (response.Status == 502) {
                        Invoke(new MethodInvoker(delegate { 
                            showMessage("Waiting for the chat program to initialize on the server...");
                        }));
                    }
                    else if (response.RespondedTo.StartsWith("PIPEUSE CHATPIPE GETNEWTEXT", StringComparison.OrdinalIgnoreCase)) {
                        Invoke(new MethodInvoker(delegate { setText(response.ReplaceKeyWords()); }));
                    }
                    else if (response.RespondedTo.StartsWith("PIPEUSE CHATPIPE GETTEXT", StringComparison.OrdinalIgnoreCase)) {
                        Invoke(new MethodInvoker(delegate { setText(response.ReplaceKeyWords()); }));
                    }
                }
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation")) {
                    return;
                }
                MessageBox.Show("The chat application was forced to exit due to an error.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void setText(string text) {
            if (!text.Trim().Equals("")) {
                AppendText(richTextBox1, Color.Empty, "\r\n" + text);
                if (timer2.Enabled) {
                    timer2.Stop();
                }
            }
        }

        private void AppendText(RichTextBox box, Color color, string text) {
            int start = box.TextLength;
            box.AppendText(text);
            int end = box.TextLength;

            box.Select(start, end - start);

            box.SelectionColor = color;

            box.SelectionLength = 0;
            box.SelectionStart = richTextBox1.Text.Length;
            box.ScrollToCaret();
        }

        private void showMessage(string msg) {
            label1.Show();
            label1.Text = msg;
            timer2.Start();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (richTextBox2.Text.Trim().CompareTo("") != 0) {
                TMessage tMsg = new TMessage(client);
                if (button1.Text.Equals("Open Chat")) {
                    label1.Hide();
                    tMsg.Send("CHAT", richTextBox2.Text);
                    button1.Text = "Send";
                    timer1.Start();
                }
                else {
                    tMsg.Send("PipeUse", "CHATPIPE", "SetText", richTextBox2.Text);
                }
                richTextBox2.Clear();
            }
        }

        private void chat_FormClosing(object sender, FormClosingEventArgs e) {
            timer1.Stop();
            TMessage tMsg = new TMessage(client);
            tMsg.Send("PipeUse", "CHATPIPE", "Close");
            client.Close();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (!waiting) {
                TMessage tMsg = new TMessage(client);
                waiting = true;
                tMsg.Send("PipeUse", "CHATPIPE", "GetNewText");
            }
        }

        private void timer2_Tick(object sender, EventArgs e) {
            label1.Hide();
            timer2.Stop();
        }
    }
}
