using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class Pipes : Form {

        private Main mainForm;

        public Pipes(Main mainForm) {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void Pipes_Load(object sender, EventArgs e) {
            
        }

        private void button1_Click(object sender, EventArgs e) {
            Hide();
        }

        private void Pipes_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Hide();
        }

        private void Pipes_VisibleChanged(object sender, EventArgs e) {
            if (Visible) {
                TMessage msg = new TMessage(mainForm.messageClient.GetStream());
                msg.Send("PipeListUsers", "/s");
                ShowProgress(true);
            }
        }

        public delegate void ShowProgressMethod(bool show);
        public void ShowProgress(bool show) {
            label1.Enabled = textBox1.Enabled = listView1.Enabled = 
                button3.Enabled = button1.Enabled = button2.Enabled = !show;
            if (show) {
                progressBar1.Location = new Point((ClientRectangle.Width - progressBar1.Width) / 2,
                                                  (ClientRectangle.Height - progressBar1.Height) / 2);
                progressBar1.Show();
            }
            else {
                button2.Enabled = listView1.SelectedItems.Count == 1;
                progressBar1.Hide();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            button2.Enabled = listView1.SelectedItems.Count == 1;
        }

        private void button2_Click(object sender, EventArgs e) {
            TMessage changeMsg = new TMessage(mainForm.mainClient.GetStream());
            changeMsg.Process("USER", listView1.SelectedItems[0].Text, "True");
            changeMsg.Send();
            changeMsg.Stream = mainForm.messageClient.GetStream();
            changeMsg.Send();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e) {
            TMessage changeMsg = new TMessage(mainForm.mainClient.GetStream());
            changeMsg.Process("UsingPipe", "", "True");
            changeMsg.Send();
            changeMsg.Stream = mainForm.messageClient.GetStream();
            changeMsg.Send();
            Hide();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listView1.SelectedItems.Count == 1) {
                button2_Click(sender, e);
            }
        }
    }
}
