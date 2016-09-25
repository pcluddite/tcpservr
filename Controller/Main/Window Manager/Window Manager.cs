using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TCPHOST
{
    public partial class Form1 : Form
    {
        bool receiving = false;
        TcpClient client;
        Socket socket;
        int quality = 50;

        public Form1(string host, int port)
        {
            client = new TcpClient(host, port);
            socket = client.Client;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            timer1.Start();
            timer1_Tick(sender, e);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                byte[] data = new byte[255];
                int bytes;
                while ((bytes = socket.Receive(data)) != 0)
                {
                    #region Get all data
                    if (Encoding.UTF8.GetString(data.Take(3).ToArray()).CompareTo("TCP") != 0)
                    {
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            Program.ProcessInvalid(data, Text, bytes, this);
                        }));
                        continue;
                    }
                    List<byte> allData = new List<byte>();
                    allData.AddRange(data.Take(bytes));
                    int size = BitConverter.ToInt32(allData.Skip(3).Take(4).ToArray(), 0);
                    while (allData.Count - 7 < size)
                    {
                        data = new byte[client.Available];
                        socket.Receive(data);
                        allData.AddRange(data);
                    }
                    bytes = size;
                    data = allData.Skip(7).ToArray();
                    #endregion
                    receiving = false;
                    string text = Encoding.UTF8.GetString(data, 0, bytes).Trim();
                    string code = Regex.Split(text, "\r\n")[0].Split(' ')[0];
                    string respondedTo = Regex.Split(text, "\r\n")[0].Split(' ')[1];
                    string response = text.Remove(0, Regex.Split(text, "\r\n")[0].Length).Trim();
                    if (!code.StartsWith("2"))
                    {
                        MessageBox.Show(response + "\r\n\r\n(An error code " + code + " in response to command '" + respondedTo + "'.)", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    if (respondedTo.CompareTo("WINLISTINFO") == 0)
                    {
                        if (response.CompareTo("") == 0) { continue; }
                        BeginInvoke(new MethodInvoker(delegate { setList(response); }));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("blocking operation")) { return; }
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setList(string text)
        {
            string[] win = Regex.Split(text, "\r\n\r\n");
            foreach (string s in win)
            {
                string[] details = Regex.Split(s, "\r\n");
                if (listView1.Items.ContainsKey(details[4]))
                {
                    listView1.Items[details[4]].Text = details[0];
                    listView1.Items[details[4]].SubItems[1].Text = details[1];
                    listView1.Items[details[4]].SubItems[2].Text = details[3].Remove(0, 10);
                    listView1.Items[details[4]].SubItems[3].Text = details[2].Remove(0, 6);
                }
                else
                {
                    ListViewItem item = new ListViewItem(details[0]);
                    item.Name = details[4];
                    item.SubItems.Add(details[1]);
                    item.SubItems.Add(details[3].Remove(0,10));
                    item.SubItems.Add(details[2].Remove(0,6));
                    listView1.Items.Add(item);
                }
            }
            ListView.ListViewItemCollection toRemove = listView1.Items;
            foreach (ListViewItem s in toRemove)
            {
                if (!text.Contains(s.Name))
                {
                    listView1.Items.Remove(s);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!receiving)
            {
                string command = "WinListInfo Visible";
                if (existingToolStripMenuItem.Checked) { command = "WinListInfo Existing"; }
                if (visibleWindowsToolStripMenuItem.Checked) { command = "WinListInfo Visible"; }
                if (minimizedWindowsToolStripMenuItem.Checked) { command = "WinListInfo Minimized"; }
                if (maximizedWindowsToolStripMenuItem.Checked) { command = "WinListInfo Maximized"; }
                socket.Send(Program.sendPrepare(command));
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(listView1, e.Location);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                client.Close();
            } catch { }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }
            ToolStripDropDownItem item = (ToolStripDropDownItem)sender;
            string handle = listView1.SelectedItems[0].Name;
            switch (item.Text)
            {
                case "Close Window":
                    socket.Send(Program.sendPrepare("WinClose " + handle));
                    break;
                case "Kill Window":
                    socket.Send(Program.sendPrepare("WinKill " + handle));
                    break;
                case "Disable Close Button":
                    socket.Send(Program.sendPrepare("RemoveX \"" + handle + "\""));
                    break;
            }
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string handle = listView1.SelectedItems[0].Name;
            switch (sender.ToString())
            {
                case "Hide":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " SW_HIDE"));
                    break;
                case "Maximize":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " SW_MAXIMIZE"));
                    break;
                case "Minimize":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " SW_MINIMIZE"));
                    break;
                case "Restore":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " SW_RESTORE"));
                    break;
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string handle = listView1.SelectedItems[0].Name;
            switch (sender.ToString())
            {
                case "Show":
                    contextMenuStrip1.Close();
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " SW_SHOW"));
                    break;
                case "Default":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " sw_showdefault"));
                    break;
                case "Maximized":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " sw_showmaximize"));
                    break;
                case "Restore":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " sw_showminimized"));
                    break;
                case "Not Active":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " sw_shownoactivate"));
                    break;
                case "Normal":
                    client.Client.Send(Program.sendPrepare("WinSetState " + handle + " sw_shownormal"));
                    break;
            }
        }

        private void changeDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Window_Manager.Details d = new TCPHOST.Window_Manager.Details(this, (IPEndPoint)client.Client.RemoteEndPoint, quality);
            d.Show();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                if (int.TryParse(toolStripTextBox1.Text.Replace("%", ""), out quality))
                {
                    Window_Manager.Details d = new TCPHOST.Window_Manager.Details(this, (IPEndPoint)client.Client.RemoteEndPoint, quality);
                    d.Show();
                }
            }
        }

        #region Menu
        #region View Menu
        private void existingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            visibleWindowsToolStripMenuItem.Checked = 
                minimizedWindowsToolStripMenuItem.Checked = 
                maximizedWindowsToolStripMenuItem.Checked = false;
            existingToolStripMenuItem.Checked = true;
        }

        private void visibleWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            existingToolStripMenuItem.Checked =
                minimizedWindowsToolStripMenuItem.Checked =
                maximizedWindowsToolStripMenuItem.Checked = false;
            visibleWindowsToolStripMenuItem.Checked = true;
        }

        private void minimizedWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            existingToolStripMenuItem.Checked =
                maximizedWindowsToolStripMenuItem.Checked = 
                visibleWindowsToolStripMenuItem.Checked = false;
            minimizedWindowsToolStripMenuItem.Checked = true;
        }

        private void maximizedWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            existingToolStripMenuItem.Checked =
                minimizedWindowsToolStripMenuItem.Checked =
                visibleWindowsToolStripMenuItem.Checked = false;
            maximizedWindowsToolStripMenuItem.Checked = true;
        }
        #endregion

        #region Refresh Menu
        private void secondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            secondsToolStripMenuItem.Checked = 
                secondsToolStripMenuItem1.Checked = 
                freezeToolStripMenuItem.Checked = false;
            secondToolStripMenuItem.Checked = true;
            if (!timer1.Enabled) { timer1.Start(); }
            timer1.Interval = 1000;
        }

        private void secondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            secondToolStripMenuItem.Checked =
                   secondsToolStripMenuItem1.Checked =
                   freezeToolStripMenuItem.Checked = false;
            secondsToolStripMenuItem.Checked = true;
            if (!timer1.Enabled) { timer1.Start(); }
            timer1.Interval = 3000;
        }

        private void secondsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            secondToolStripMenuItem.Checked =
                   secondsToolStripMenuItem.Checked =
                   freezeToolStripMenuItem.Checked = false;
            secondsToolStripMenuItem1.Checked = true;
            if (!timer1.Enabled) { timer1.Start(); }
            timer1.Interval = 5000;
        }

        private void freezeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            secondToolStripMenuItem.Checked =
                   secondsToolStripMenuItem.Checked =
                   secondsToolStripMenuItem1.Checked = false;
            freezeToolStripMenuItem.Checked = true;
            timer1.Stop();
        }
        #endregion
        #endregion
    }
}
