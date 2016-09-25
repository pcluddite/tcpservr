using System;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Globalization;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor
{
    public partial class DWord : Form
    {
        NetworkStream client;
        Form1 form;
        string Key;
        string ValueData;
        string ValueName;

        public DWord(string key, string valueName, string valueData, NetworkStream Client, Form1 Form, bool enable)
        {
            InitializeComponent();
            Key = key;
            ValueData = valueData.Remove(0,2).Remove(8);
            ValueName = valueName;
            form = Form;
            client = Client;
            textBox1.Text = valueName;
            textBox1.ReadOnly = !enable;
            textBox2.Text = ValueData.TrimStart('0');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TMessage tMsg = new TMessage(client);
            if (textBox2.Text.Equals("")) { textBox2.Text = "0"; }
            int num;
            try {
                if (radioButton1.Checked) { 
                    num = int.Parse(textBox2.Text, NumberStyles.AllowHexSpecifier);
                }
                else { 
                    num = int.Parse(textBox2.Text);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tMsg.Send("RegWrite", Key, textBox1.Text, num.ToString(), "DWORD");
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals("")) { 
                textBox2.Text = "0";
            }
            if (radioButton1.Checked) {
                textBox2.MaxLength = 8;
                textBox2.Text = int.Parse(textBox2.Text).ToString("x8").TrimStart('0');
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals("")) { 
                textBox2.Text = "0";
            }
            if (radioButton2.Checked) {
                textBox2.MaxLength = 10;
                textBox2.Text = int.Parse(textBox2.Text, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            int i;
            if (!int.TryParse(e.KeyChar.ToString(), out i) && e.KeyChar != '\b')
            {
                e.Handled = true;
                if (radioButton1.Checked && (e.KeyChar == 'a' || e.KeyChar == 'b' || e.KeyChar == 'c' || e.KeyChar == 'd' || e.KeyChar == 'e' || e.KeyChar == 'f'
                    || e.KeyChar == 'A' || e.KeyChar == 'B' || e.KeyChar == 'C' || e.KeyChar == 'D' || e.KeyChar == 'E' || e.KeyChar == 'F'))
                {
                    e.Handled = false;
                }
            }
        }
    }
}
