using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms.Registry_Editor {
    public partial class Rename : Form {
        NetworkStream client;
        string name;
        string key;
        bool isKey = false;
        Form1 form;
        public Rename(string Key, string Name, Form1 Form, NetworkStream Client, bool IsKey) {
            InitializeComponent();
            key = Key;
            textBox1.Text = Name;
            client = Client;
            name = Name;
            isKey = IsKey;
            form = Form;
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            TMessage tMsg = new TMessage(client);
            if (isKey) {
                tMsg.Send(new string[] { "REGRENAMEKEY", key + "\\" + name, textBox1.Text });
                form.newKeyName = textBox1.Text;
                Close();
            }
            else {
                tMsg.Send(new string[] { "REGRENAME", key, name, textBox1.Text });
                tMsg.Send(new string[] { "REGENUMVALUES", key });
                Close();
            }
        }
    }
}