using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using Tcpclient.Forms;
using Tcpclient.Components;

namespace Tcpclient {
    public partial class Main {

        private void chatToolStripMenuItem_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send("PipeExists", "CHATPIPE");
        }

        private void blockemToolStripMenuItem_Click(object sender, EventArgs e) {
            BlockMessage b = new BlockMessage(host, port);
            b.ShowDialog();
        }

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            MsgBox m = new MsgBox(messageClient.GetStream());
            m.ShowDialog();
        }

        private void saySomethingToolStripMenuItem_Click(object sender, EventArgs e) {
            Say s = new Say(messageClient.GetStream());
            s.ShowDialog();
        }

        private void trayTipToolStripMenuItem_Click(object sender, EventArgs e) {
            Tooltip t = new Tooltip(messageClient.GetStream());
            t.ShowDialog();
        }

        About a = null;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (a == null || a.IsDisposed) {
                a = new About();
            }
            if (a.Visible) {
                a.BringToFront();
            }
            else {
                a.Show();
            }
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e) {
            if (SendCommand(true, "END") == 1) {
                mainClient.Client.Shutdown(SocketShutdown.Both);
                messageClient.Client.Shutdown(SocketShutdown.Both);
                closeHost = false;
                Close();
                hosts.Show();
            }
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e) {
            if (SendCommand(true, "RESTART") == 1) {
                mainClient.Client.Shutdown(SocketShutdown.Both);
                messageClient.Client.Shutdown(SocketShutdown.Both);
                Application.Restart();
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            string text = null;
            if (sender == (object)runWithArgumentsToolStripMenuItem) {
                if (listView1.SelectedItems[0].Text.Contains(' ')) {
                    text = "\"" + listView1.SelectedItems[0].Text + "\"";
                }
                else {
                    text = listView1.SelectedItems[0].Text;
                }
            }
            RunDialogue r = new RunDialogue(this, "Run arguments:", "&Run", "Run", text, new RunDialogue.RunButtonClick(Run), false);
            r.ShowDialog();
        }

        private void Run(string arguments) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send("RUN " + arguments);
        }

        private void rawMessageToolStripMenuItem_Click(object sender, EventArgs e) {
            RunDialogue r = new RunDialogue(this, "Raw message:", "&Send", "Send Raw Message", null, new RunDialogue.RunButtonClick(Raw), false);
            r.ShowDialog();
        }

        private void Raw(string cmd) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send(cmd);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) {
            try {
                TResponse response = new TResponse(messageClient.GetStream());
                StringDelegate updatePipeList = new StringDelegate(UpdatePipeList);
                StringDelegate updateCurrentUser = new StringDelegate(UpdateCurrentUser);
                ShowStatusTipMethod showStatus = new ShowStatusTipMethod(ShowStatusTip);
                int len;
                while ((len = response.Receive()) != 0) {
                    isWorking = false;

                    if (len == -1) {
                        continue;
                    }

                    if (response.RespondedTo.Equals("WINLIST")) { 
                        continue;
                    }

                    if (response.RespondedTo.StartsWith("PIPEEXISTS") && response.Status == 200) {
                        if (response.Status == 200) {
                            Invoke(new MethodInvoker(delegate {
                                chat c = new chat(new TcpClient(host, port), name, bool.Parse(response.Message));
                                c.Show();
                            }));
                        }
                    }
                    else if (response.RespondedTo.StartsWith("PIPELISTUSERS") && response.Status == 200) {
                        Invoke(updatePipeList, response.Message);
                    }
                    else if (response.RespondedTo.Equals("USINGPIPE") && response.Status == 200) {
                        Invoke(updateCurrentUser, response.Message);
                    }
                    else {
                        Invoke(showStatus, response);
                    }
                }
            }
            catch (Exception ex) {
                if (ex.Message.Contains("blocking operation")) {
                    return;
                }
                Program.Log(ex, "SecondaryClient");
            }
        }

        public delegate void ShowStatusTipMethod(TResponse response);
        public void ShowStatusTip(TResponse response) {
            if (!receiveConfirmationMessagesToolStripMenuItem.Checked) {
                return;
            }
            if (response.Status == 204) {
                return;
            }
            textBox3.Show();
            if (response.Status >= 200 && response.Status < 300) {
                textBox3.ForeColor = Color.Blue;
            }
            else if (response.Status >= 400 && response.Status < 500) {
                textBox3.ForeColor = Color.DarkOrange;
            }
            else {
                textBox3.ForeColor = Color.Red;
            }
            textBox3.Text = response.Message;
            if (timer2.Enabled) {
                timer2.Stop();
            }
            timer2.Start();
        }

        private void UpdatePipeList(string message) {
            pipesForm.listView1.Items.Clear();
            if (!message.Equals("")) {
                foreach (string line in message.Replace("\r\n", "\n").Split('\n')) {
                    ListViewItem user = new ListViewItem();
                    string[] components = line.Split('|');
                    if (components.Length != 3) {
                        continue;
                    }
                    user.Text = components[0];
                    user.SubItems.Add(components[2].Equals("Alive") ? "True" : "False");
                    pipesForm.listView1.Items.Add(user);
                }
                TMessage msg = new TMessage(messageClient.GetStream());
                msg.Send("UsingPipe");
            }
            else {
                pipesForm.textBox1.Text = "N/A";
                pipesForm.ShowProgress(false);
            }
        }

        private void UpdateCurrentUser(string message) {
            if (message.StartsWith("False")) {
                pipesForm.textBox1.Text = "N/A";
            }
            else {
                pipesForm.textBox1.Text = message.Remove(0, 4).TrimStart();
            }
            pipesForm.ShowProgress(false);
        }

        private void unblockProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            RunDialogue r = new RunDialogue(this, "Process name:", "&Unblock", "Unblock Process", null,
                new RunDialogue.RunButtonClick(Unblock), false);
            r.ShowDialog();
        }

        private void Unblock(string process) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send("UNBLOCK", process);
        }

        private void closeProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            RunDialogue r = new RunDialogue(this, "Process name:", "&Close", "Close Process", null,
                new RunDialogue.RunButtonClick(CloseProcess), false);
            r.ShowDialog();
        }

        private void CloseProcess(string process) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send("PROCESSCLOSE", process);
        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            RunDialogue r = new RunDialogue(this, "Process name:", "&Kill", "Kill Process", null,
                new RunDialogue.RunButtonClick(KillProcess), false);
            r.ShowDialog();
        }

        private void KillProcess(string process) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            tMsg.Send("PROCESSKILL", process);
        }

        private void blockProcessToolStripMenuItem_Click(object sender, EventArgs e) {
            BlockApp block = new BlockApp(messageClient.GetStream());
            block.ShowDialog();
        }

        private void blockAllUserInputToolStripMenuItem_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(messageClient.GetStream());
            if (blockAllUserInputToolStripMenuItem.Checked) {
                tMsg.Send("BLOCKINPUT 1");
            }
            else {
                tMsg.Send("BLOCKINPUT 0");
            }
        }
    }
}
