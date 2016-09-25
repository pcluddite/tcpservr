using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Pipes;
using System.Security.Principal;

namespace Tcpservr
{
    public partial class chat : Form {
        delegate void SetTextCallback(string text);
        StringBuilder allText;
        StringBuilder newText;
        bool validClose;

        public chat(string msg) {
            InitializeComponent();
            allText = new StringBuilder();
            validClose = false;
            newText = new StringBuilder();
            SetText(msg);
        }

        private void chat_Load(object sender, EventArgs e) {
            //int width = Screen.PrimaryScreen.Bounds.Width;
            //int height = Screen.PrimaryScreen.Bounds.Height;
            //this.Location = new Point(width - 400, height - 330);
            this.Width = 400;
            this.Height = 330;
            this.Activate();
            this.BringToFront();
            this.Focus();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            try {
                SetTextCallback d = new SetTextCallback(SetText);
                PipeSecurity ps = new PipeSecurity();
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                ps.SetAccessRule(
                    new PipeAccessRule(account, PipeAccessRights.ReadWrite,
                        System.Security.AccessControl.AccessControlType.Allow));

                NamedPipeServerStream server = new NamedPipeServerStream("CHATPIPE",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None, 16384, 16384, ps);
                while (true) {
                    server.WaitForConnection();

                    TReceiver receiver = new TReceiver(server);
                    byte[] data;
                    int len = receiver.Receive(out data);
                    if (len == -1) {
                        server.Disconnect();
                        continue;
                    }
                    TMessage msg = new TMessage();
                    msg.Process(data);

                    TResponse response = new TResponse(msg);

                    switch (msg.Args[0].ToUpper()) {
                        case "CLOSE":
                            response.Process(202, "Closing chat. Do not reconnect.");
                            server.Write(response.Data, 0, response.Length);
                            server.WaitForPipeDrain();
                            server.Disconnect();
                            return;
                        case "GETTEXT":
                            response.Process(200, allText.ToString());
                            newText = new StringBuilder();
                            break;
                        case "GETNEWTEXT":
                            response.Process(200, newText.ToString());
                            newText = new StringBuilder();
                            break;
                        case "SETTEXT":
                            if (msg.Args.Length == 2) {
                                Invoke(d, msg.Args[1]);
                                response.Process(200, "Text added");
                            }
                            else {
                                response.Process(400, "Bad Request: 2 argument(s) expected");
                            }
                            break;
                        default:
                            response.Process(501, "Not Implemented: " + msg.Args[0].ToUpper());
                            break;
                    }
                    server.Write(response.Data, 0, response.Length);
                    server.WaitForPipeDrain();
                    server.Disconnect();
                }
            }
            catch {
            }
        }

        private void SetText(string text) {
            if (!text.Equals("")) {
                allText.AppendLine("Client: " + text);
                newText.AppendLine("Client: " + text);
                AppendText(richTextBox1, Color.Blue, "\r\n" + text);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!richTextBox2.Text.Trim().Equals("")) {
                allText.AppendLine("Server: " + richTextBox2.Text);
                newText.AppendLine("Server: " + richTextBox2.Text);
                AppendText(richTextBox1, Color.Green, "\r\n" + richTextBox2.Text);
                richTextBox2.Clear();
            }
        }

        void AppendText(RichTextBox box, Color color, string text) {
            int start = box.TextLength;
            box.AppendText(text);
            int end = box.TextLength;
            box.Select(start, end - start);
            box.SelectionColor = color;
            box.SelectionLength = 0;
            box.SelectionStart = richTextBox1.Text.Length;
            box.ScrollToCaret();
            box.Refresh();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            validClose = true;
            Close();
        }

        private void chat_FormClosing(object sender, FormClosingEventArgs e) {
            if (!validClose) {
                e.Cancel = true;
            }
        }
    }
}
