using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Components;

namespace Tbasic
{
    internal partial class BlockInput : Form {
        bool validClose = false;

        public BlockInput(string Msg) {
            validClose = true;
            InitializeComponent();
            label1.Text = Msg;
        }

        private void blockinput_Load(object sender, EventArgs e) {
            this.Location = new Point(0, 0);
            this.Size = new System.Drawing.Size(
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            label1.Location = new Point(((this.Size.Width - label1.Width) / 2), (this.Size.Height - label1.Height) / 2);
            ForceToFront();
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            ActivateMethod d = new ActivateMethod(ForceToFront);
            hasInvoked = true;
            while (true) {
                try {
                    Thread.Sleep(500);
                    if (hasInvoked) {
                        d.BeginInvoke(new AsyncCallback(isFinished), null);
                        hasInvoked = false;
                    }
                }
                catch {
                }
            }
        }

        bool hasInvoked;
        private void isFinished(IAsyncResult ar) {
            hasInvoked = ar.IsCompleted;
        }

        private delegate void ActivateMethod();
        private void ForceToFront() {
            this.BringToFront();
            this.Activate();
            this.Focus();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) {
            try {
                PipeSecurity ps = new PipeSecurity();
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                ps.SetAccessRule(
                    new PipeAccessRule(account, PipeAccessRights.ReadWrite,
                        System.Security.AccessControl.AccessControlType.Allow));

                NamedPipeServerStream server = new NamedPipeServerStream("BLOCKMESSAGEPIPE",
                    PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None, 16384, 16384, ps);

                SetTextMethod setText = new SetTextMethod(SetText);
                ActivateMethod activator = new ActivateMethod(ForceToFront);

                while (true) {
                    server.WaitForConnection();
                    TReceiver receiver = new TReceiver(server, server);
                    byte[] data;
                    int len = receiver.Receive(out data);
                    if (len == -1) {
                        server.Write(
                            Encoding.UTF8.GetBytes("400 Invalid message"), 0, "400 Invalid message".Length);
                        server.Disconnect();
                        continue;
                    }

                    TMessage msg = new TMessage();
                    msg.Process(data);
                    string text = msg.Text;

                    TResponse response = new TResponse(msg);

                    switch (msg.Arguments[0].ToUpper()) {
                        case "CLOSE":
                            response.Process(202, "Exiting block message");
                            server.Write(response.Data, 0, response.Length);
                            server.WaitForPipeDrain();
                            server.Disconnect();
                            server.Dispose();
                            return;
                        case "SETTEXT":
                            if (msg.Arguments.Length != 2) {
                                response.Process(400, "Bad Request");
                                break;
                            }
                            Invoke(setText, msg.Arguments[1]);
                            response.Process(200, "Text changed");
                            break;
                        case "GETTEXT":
                            response.Process(200, label1.Text);
                            break;
                        case "ACTIVATE":
                            Invoke(activator);
                            response.Process(200, "Activated");
                            break;
                        default:
                            response.Process(501, "Not Implemented: " + msg.Arguments[0].ToUpper());
                            break;
                    }
                    server.Write(response.Data, 0, response.Length);
                    server.WaitForPipeDrain();
                    server.Disconnect();
                }
            }
            catch (Exception ex) {
                new LoggedError("BlockInput", ex, false, false).Write();
            }
        }

        private delegate void SetTextMethod(string text);
        private void SetText(string text) {
            label1.Text = text;
            label1.Location = new Point(((this.Size.Width - label1.Width) / 2), (this.Size.Height - label1.Height) / 2);
        }

        private void blockinput_FormClosing(object sender, FormClosingEventArgs e) {
            if (!this.validClose) {
                e.Cancel = true;
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            validClose = true;
            this.Close();
        }
    }
}
