using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Tcpclient.Forms.Window_Manager
{
    public partial class SendText : Form
    {
        Tcpclient.Window_Manager.Details details;
        public SendText(Tcpclient.Window_Manager.Details d)
        {
            details = d;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!details.textBox3.Text.Contains("Active"))
            {
                switch (MessageBox.Show(this, "This window isn't active. Sending text to it requires it being on the foreground." +
                    "\r\nDo you want to activate the window before sending the text?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        details.Send(false, new string[] { "WinActivate", details.item.Name });
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            details.Send(false, new string[] { "SEND", textBox1.Text });
            details.button12_Click(sender, e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx");
            }
            catch (Exception ex) {
                MessageBox.Show(this, "Unable to open website in browser:\r\n" + ex.Message, this.Text, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
