using System;
using System.Windows.Forms;
using System.Net.Sockets;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor
{
    public partial class String : Form
    {
        NetworkStream client;
        Form1 form;
        string Key;
        string ValueData;
        string type;
        public String(string key, string valueName, string valueData, NetworkStream Client, Form1 Form, string Type, bool enable)
        {
            type = Type;
            InitializeComponent();
            Key = key;
            ValueData = valueData;
            form = Form;
            client = Client;
            textBox1.Text = valueName;
            textBox1.ReadOnly = !enable;
            textBox2.Text = valueData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TMessage tMsg = new TMessage(client);
            tMsg.Send("RegWrite", Key, textBox1.Text, textBox2.Text, type);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
