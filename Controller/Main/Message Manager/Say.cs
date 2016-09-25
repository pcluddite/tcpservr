using System;
using System.Windows.Forms;
using System.Net.Sockets;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class Say : Form {
        NetworkStream socket;
        public Say(NetworkStream Socket) {
            socket = Socket;
            InitializeComponent();
        }

        bool dontClose = false;
        private void button1_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(socket);
            tMsg.Process("SAY", textBox1.Text);
            tMsg.Send();
            dontClose = true;
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void SAPI_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }

        private void SAPI_FormClosing(object sender, FormClosingEventArgs e) {
            if (dontClose) {
                dontClose = false;
                e.Cancel = true;
            }
        }
    }
}
