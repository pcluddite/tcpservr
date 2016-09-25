using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor
{
    public partial class MultiString : Form
    {
        NetworkStream client;
        Form1 form;
        string Key;
        string ValueData;
        public MultiString(string key, string valueName, string valueData, NetworkStream Client, Form1 Form, bool enable)
        {
            InitializeComponent();
            Key = key;
            ValueData = valueData;
            form = Form;
            textBox1.ReadOnly = !enable;
            client = Client;
            textBox1.Text = valueName;
            textBox2.Text = valueData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TMessage tMsg = new TMessage(client);
            tMsg.Send("REGWRITE", Key, textBox1.Text, string.Join("\r\n", textBox2.Lines), "MULTISTRING");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
