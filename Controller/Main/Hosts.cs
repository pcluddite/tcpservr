using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tcpclient {
    public partial class Hosts : Form {
        private string curdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);

        public Hosts() {
            InitializeComponent();
        }

        private void Hosts_Load(object sender, EventArgs e) {
            foreach (var comp in Settings.GetAllComputers()) {
                ListViewItem item = new ListViewItem(new string[] {
                    comp["name"], comp["address"], comp["port"]
                });
                item.Name = comp["name"];
                listView1.Items.Add(item);
            }
            try {
                listView1.Items[Settings.GetDefault()].Selected = true;
            }
            catch {
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            configButton.Enabled = connectButton.Enabled =
                removeButton.Enabled = editToolStripMenuItem.Enabled =
                connectToolStripMenuItem.Enabled = (listView1.SelectedItems.Count == 1);
        }


        private void button5_Click(object sender, EventArgs e) {
            listView1_MouseDoubleClick(sender, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listView1.SelectedItems.Count != 0) {
                listView1.Enabled = addMachineButton.Enabled =
                    configButton.Enabled = connectButton.Enabled =
                    removeButton.Enabled = editToolStripMenuItem.Enabled =
                    connectToolStripMenuItem.Enabled = false;
                textBox4.Visible = label4.Visible =
                    progressBar1.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (MessageBox.Show(this, "Are you sure you want to remove the settings for this machine?", Text, 
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                if (listView1.SelectedItems.Count > 0) {
                    if (listView1.Items.Count == 1) {
                        MessageBox.Show("At least one server should remain on the list in order to connect.",
                            Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else {
                        Settings set = new Settings(listView1.SelectedItems[0].Name);
                        set.Destroy();
                        int index = listView1.SelectedItems[0].Index;
                        listView1.Items.Remove(listView1.SelectedItems[0]);
                        if (index == 0) {
                            index = listView1.Items.Count - 1;
                        }
                        if (index == 0) {
                            index = 1;
                        }
                        if (listView1.Items[index - 1] != null) {
                            listView1.Items[index - 1].Selected = true;
                        }
                    }
                }
            }
        }

        private void Hosts_FormClosing(object sender, FormClosingEventArgs e) {
            if (listView1.SelectedItems.Count > 0) {
                Settings.SetDefault(listView1.SelectedItems[0].Name);
            }
            Settings.Save();
        }

        private void button4_Click(object sender, EventArgs e) {
            if (File.Exists(Application.StartupPath + "\\Console.exe")) {
                try {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\Console.exe");
                }
                catch (Exception ex) {
                    MessageBox.Show(this,
                    "An error occoured in trying to start the console application:\r\n" +
                    ex.Message,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                MessageBox.Show(this,
                    "The console application cannot be found!",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        Main m;
        bool error = false;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            error = false;
            try {
                string hostText = ""; int port = 0; Hosts hosts = null; string name = "";
                Invoke(new MethodInvoker(delegate {
                    hostText = listView1.SelectedItems[0].SubItems[1].Text;
                    port = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
                    hosts = this;
                    name = listView1.SelectedItems[0].SubItems[0].Text;
                }));

                IPAddress hostAddr;
                if (!IPAddress.TryParse(hostText, out hostAddr)) {
                    backgroundWorker1.ReportProgress(0, "Finding host " + hostText + "...");
                    try {
                        var addresses =
                            from address in Dns.GetHostAddresses(hostText)
                            where address.AddressFamily != AddressFamily.InterNetworkV6
                            select address;
                        hostAddr = addresses.ElementAt(0);
                    }
                    catch {
                        error = true;
                        backgroundWorker1.ReportProgress(1, "No such host with the name '" + hostText + "' could be found. Make sure the spelling is correct and that the server is online.");
                        return;
                    }
                }
                backgroundWorker1.ReportProgress(0, "Connecting to " + hostAddr + ":" + port + "...");
                Main f = new Main(hostText, port, hosts, name);
                Invoke(new MethodInvoker(delegate {
                    m = f;
                }));
            }
            catch (Exception ex) {
                error = true;
                backgroundWorker1.ReportProgress(1, PrintException(ex));
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            if (e.ProgressPercentage == 0) {
                label4.Text = e.UserState.ToString();
            }
            else {
                MessageBox.Show(this, e.UserState.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string PrintException(Exception ex) {
            Type type = ex.GetType();
            if (ex.Message.Contains("target machine actively refused")) {
                return "The server is not accepting any connections at that port.";
            }
            else if (type == typeof(SocketException)) {
                return "Unable to connect to the remote host. Make sure the server is online.";
            }
            else {
                return "A network exception occoured:\r\n" + ex.Message;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            textBox4.Visible = label4.Visible = progressBar1.Visible = false;
            listView1.Enabled = addMachineButton.Enabled = 
                configButton.Enabled = connectButton.Enabled = 
                removeButton.Enabled = editToolStripMenuItem.Enabled =
                connectToolStripMenuItem.Enabled = true;
            if (error) {
                return;
            }
            Hide();
            m.Show();
        }

        About aboutBox = null;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (aboutBox == null || aboutBox.IsDisposed) {
                aboutBox = new About();
            }
            if (aboutBox.Visible) {
                aboutBox.BringToFront();
            }
            else {
                aboutBox.Show();
            }
        }

        private void addMachineButton_Click(object sender, EventArgs e) {
            AddMachine addMachine = new AddMachine(this, "", "", "");
            addMachine.Show();
        }

        private void configButton_Click(object sender, EventArgs e) {
            AddMachine configure = new AddMachine(this, listView1.SelectedItems[0].Text,
                                                        listView1.SelectedItems[0].SubItems[1].Text,
                                                        listView1.SelectedItems[0].SubItems[2].Text);
            configure.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }
    }
}