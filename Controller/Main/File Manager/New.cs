using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Tcpclient.Components;

namespace Tcpclient.Forms.File_Manager
{
    public partial class New : Form
    {
        NetworkStream client;
        Main form;
        public New(NetworkStream Client, Main Form)
        {
            client = Client;
            form = Form;
            InitializeComponent();
        }

        private void New_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TMessage tMsg = new TMessage(client);
            tMsg.Process(new string[] { "MD", textBox1.Text });
            tMsg.Send();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
