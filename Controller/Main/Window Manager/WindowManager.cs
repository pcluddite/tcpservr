using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;

namespace Tcpclient {
    public partial class Main {
        
        private void timer1_Tick(object sender, EventArgs e) {
            if (isWorking) { return; }
            switch (comboBox2.SelectedIndex) {
                case 0:
                    SendCommand(false, "WinListInfo", "Existing");
                    break;
                case 1:
                    SendCommand(false, "WinListInfo", "Visible");
                    break;
                case 2:
                    SendCommand(false, "WinListInfo", "Minimized");
                    break;
                case 3:
                    SendCommand(false, "WinListInfo", "Maximized");
                    break;
            }
        }

        private void hideWindowManager() {
            if (label4.Text.CompareTo("Window Manager") == 0) {
                label1.Visible =
                label2.Visible =
                label3.Visible =
                button14.Visible =
                comboBox1.Visible =
                comboBox2.Visible = numericUpDown1.Visible = false;
                listView1.MouseDoubleClick -= new MouseEventHandler(listView1_MouseDoubleClickWM);
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!timer1.Enabled) { timer1.Start(); }
            listView1.View = View.Details;
            listView1.Items.Clear();
            listView1.MouseDoubleClick += new MouseEventHandler(listView1_MouseDoubleClickWM);

            fileMan.Deactivate();

            label1.Visible = label2.Visible =
                label3.Visible = button14.Visible = comboBox1.Visible =
                comboBox2.Visible = numericUpDown1.Visible = true;

            groupBox1.Text = "Windows";
            timer1_Tick(sender, e);
            label4.Text = "Window Manager";

            addressBar.Visible = goButton.Visible = label5.Visible = false;

            button1.Enabled = false;
        }

        Regex doubleBreak = new Regex("\n\n");

        private void setList(string text) {
            if (text.Equals("No Content")) {
                listView1.Items.Clear();
                toolStripStatusLabel1.Text = "There are no windows open on this user";
                return;
            }
            string[] win = doubleBreak.Split(text.Replace("\r\n", "\n"));
            foreach (string s in win) {
                string[] details = s.Split('\n');
                if (listView1.Items.ContainsKey(details[4])) {
                    listView1.Items[details[4]].Text = details[0];
                    listView1.Items[details[4]].SubItems[1].Text = details[1];
                    listView1.Items[details[4]].SubItems[2].Text = details[3].Remove(0, 10);
                    listView1.Items[details[4]].SubItems[3].Text = details[2].Remove(0, 6);
                }
                else {
                    ListViewItem item = new ListViewItem(details[0]);
                    item.Name = details[4];
                    item.SubItems.Add(details[1]);
                    item.SubItems.Add(details[3].Remove(0, 10));
                    item.SubItems.Add(details[2].Remove(0, 6));
                    listView1.Items.Add(item);
                }
            }
            ListView.ListViewItemCollection toRemove = listView1.Items;
            foreach (ListViewItem s in toRemove) {
                if (!text.Contains(s.Name)) {
                    listView1.Items.Remove(s);
                }
            }
        }

        private void listView1_MouseDoubleClickWM(object sender, MouseEventArgs e) {
            if (listView1.SelectedItems.Count == 1) {
                Window_Manager.Details d = new Tcpclient.Window_Manager.Details(listView1.SelectedItems[0], (IPEndPoint)mainClient.Client.RemoteEndPoint, quality, this);
                d.Show();
            }
        }

        int quality = 50;
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            quality = (int)numericUpDown1.Value;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            switch (comboBox1.SelectedIndex) {
                case 2:
                    if (!timer1.Enabled) { timer1.Start(); }
                    timer1.Interval = 1000; break;
                case 1:
                    if (!timer1.Enabled) { timer1.Start(); }
                    timer1.Interval = 3000; break;
                case 0:
                    if (!timer1.Enabled) { timer1.Start(); }
                    timer1.Interval = 5000; break;
                case 3: timer1.Stop(); break;
            }
        }
    }
}
