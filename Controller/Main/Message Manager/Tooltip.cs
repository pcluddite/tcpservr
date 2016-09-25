using System;
using System.Windows.Forms;
using System.Net.Sockets;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class Tooltip : Form {
        NetworkStream socket;
        public Tooltip(NetworkStream Socket) {
            socket = Socket;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(socket);
            tMsg.Process(
                "TrayTip", textBox3.Text, textBox2.Text, textBox1.Text, currentImage.ToString()
            );
            tMsg.Send();
        }

        private int currentImage = 1;
        private void pictureBox2_Click(object sender, EventArgs e) {
            currentImage = (currentImage + 1) % 4;
            switch (currentImage) {
                case 0:
                    pictureBox2.Image = null;
                    break;
                case 1:
                    pictureBox2.Image = Tcpclient.Properties.Resources.info;
                    break;
                case 2:
                    pictureBox2.Image = Tcpclient.Properties.Resources.warn;
                    break;
                case 3:
                    pictureBox2.Image = Tcpclient.Properties.Resources.error;
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
