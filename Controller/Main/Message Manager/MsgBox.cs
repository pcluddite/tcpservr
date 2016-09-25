using System;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Tcpclient.Forms {
    public partial class msgbox : Form {
        Socket client;
        int icon = 1;
        public msgbox(Socket Client) {
            client = Client;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            int flag = 0;
            flag = (icon == 1) ? 64 : flag;
            flag = (icon == 2) ? 32 : flag;
            flag = (icon == 3) ? 48 : flag;
            flag = (icon == 4) ? 16 : flag;
            flag = (comboBox1.SelectedIndex == 1) ? flag + 1 : flag;
            flag = (comboBox1.SelectedIndex == 2) ? flag + 2 : flag;
            flag = (comboBox1.SelectedIndex == 3) ? flag + 3 : flag;
            flag = (comboBox1.SelectedIndex == 4) ? flag + 4 : flag;
            flag = (comboBox1.SelectedIndex == 5) ? flag + 5 : flag;
            TMessage tMsg = new TMessage(client);
            tMsg.Send(new string[] { "msgbox", flag.ToString(), textBox2.Text, textBox1.Text });
            Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e) {
            icon++;
            icon = icon % 5;
            switch (icon) {
                case 0:
                    pictureBox2.Image = null;
                    break;
                case 1:
                    pictureBox2.Image = global::Tcpclient.Properties.Resources.info;
                    break;
                case 2:
                    pictureBox2.Image = global::Tcpclient.Properties.Resources.quest;
                    break;
                case 3:
                    pictureBox2.Image = global::Tcpclient.Properties.Resources.warn;
                    break;
                case 4:
                    pictureBox2.Image = global::Tcpclient.Properties.Resources.error;
                    break;
            }
        }

        private void msgbox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
    }
}