using System;
using System.Windows.Forms;
using System.Net.Sockets;
using Tcpclient.Components;

namespace Tcpclient.Forms.File_Manager
{
    public partial class Rename : Form {
        NetworkStream client;
        Main form;
        string old;
        public Rename(NetworkStream Client, Main Form, string title) {
            InitializeComponent();
            form = Form;
            client = Client;
            old = title;
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client);
            tMsg.Process("REN", old, textBox1.Text);
            tMsg.Send();
            this.Close();
        }

        private void Rename_Load(object sender, EventArgs e) {
            BeginInvoke(new MethodInvoker(delegate { textBox1.Text = old; textBox1.SelectAll(); }));
        }
    }
}
