using System;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Globalization;
using Tcpclient.Components;
namespace Tcpclient.Forms.Registry_Editor {
    public partial class QWord : Form {
        NetworkStream client;
        Form1 form;
        string Key;
        string ValueData;

        public QWord(string key, string valueName, string valueData, NetworkStream Client, Form1 Form, bool enable) {
            InitializeComponent();
            Key = key;
            ValueData = valueData.Remove(0, 2);
            ValueData = ValueData.Remove(ValueData.IndexOf("("));
            ValueData = ValueData.Replace(")", "").Trim();
            form = Form;
            client = Client;
            textBox1.Text = valueName;
            textBox1.ReadOnly = !enable;

            textBox2.Text = Int64.Parse(ValueData, NumberStyles.AllowHexSpecifier).ToString("x16");
            textBox2.Text = textBox2.Text.TrimStart('0');
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox2.Text.Equals("")) {
                textBox2.Text = "0";
            }
            Int64 num;
            try {
                if (radioButton1.Checked) {
                    num = Int64.Parse(textBox2.Text, NumberStyles.AllowHexSpecifier);
                }
                else {
                    num = Int64.Parse(textBox2.Text);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TMessage tMsg = new TMessage(client);
            tMsg.Send("RegWrite", Key, textBox1.Text, num.ToString(), "QWORD");
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            if (textBox2.Text.Equals("")) {
                textBox2.Text = "0";
            }
            if (radioButton1.Checked) {
                textBox2.MaxLength = 16;
                textBox2.Text = Int64.Parse(textBox2.Text).ToString("x16").TrimStart('0');
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            if (textBox2.Text.Equals("")) {
                textBox2.Text = "0";
            }
            if (radioButton2.Checked) {
                textBox2.MaxLength = 20;
                textBox2.Text = Int64.Parse(textBox2.Text, NumberStyles.AllowHexSpecifier).ToString();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
            int i;
            if (!int.TryParse(e.KeyChar.ToString(), out i) && e.KeyChar != '\b') {
                e.Handled = true;
                if (radioButton1.Checked && (e.KeyChar == 'a' || e.KeyChar == 'b' || e.KeyChar == 'c' || e.KeyChar == 'd' || e.KeyChar == 'e' || e.KeyChar == 'f'
                    || e.KeyChar == 'A' || e.KeyChar == 'B' || e.KeyChar == 'C' || e.KeyChar == 'D' || e.KeyChar == 'E' || e.KeyChar == 'F')) {
                    e.Handled = false;
                }
            }
        }
    }
}
