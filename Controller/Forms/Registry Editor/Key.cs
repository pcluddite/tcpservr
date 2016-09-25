using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor {
    public partial class Key : Form {
        string key;
        NetworkStream client;
        public Key(string Key, NetworkStream Client) {
            InitializeComponent();
            key = Key;
            client = Client;
        }

        private void button2_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client);
            tMsg.Process("REGCREATEKEY", key, textBox1.Text);
            tMsg.Send();
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
