using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms {
    public partial class BlockApp : Form {
        NetworkStream client;
        public BlockApp(NetworkStream client) {
            this.client = client;
            InitializeComponent();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            label2.Enabled = textBox2.Enabled = checkBox1.Enabled = radioButton2.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) {
            textBox3.Enabled = textBox4.Enabled = comboBox1.Enabled = radioButton3.Checked;
        }

        private void button3_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client);
            if (radioButton1.Checked) {
                tMsg.Send("Block", textBox1.Text);
            }
            else if (radioButton2.Checked) {
                if (checkBox1.Checked) {
                    tMsg.Send("Block", textBox1.Text, "/d", textBox2.Text);
                }
                else {
                    tMsg.Send("Block", textBox1.Text, "/r", textBox2.Text);
                }
            }
            else if (radioButton3.Checked) {
                int flag;
                switch (comboBox1.SelectedIndex) {
                    case 1:
                        flag = 64;
                        break;
                    case 2:
                        flag = 32;
                        break;
                    case 3:
                        flag = 48;
                        break;
                    case 4:
                        flag = 16;
                        break;
                    default:
                        flag = 0;
                        break;
                }
                tMsg.Send("Block", textBox1.Text, "/m", flag.ToString(), textBox4.Text, textBox3.Text);
            }
            Close();
        }

        private void button4_Click(object sender, EventArgs e) {
            Close();
        }
    }
}