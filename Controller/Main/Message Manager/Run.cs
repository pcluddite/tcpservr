using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Tcpclient.Forms
{
    public partial class RunDialogue : Form {
        private Main mainForm;
        public delegate void RunButtonClick(string text);

        RunButtonClick onClick;

        public RunDialogue(Main main, string label1Text, string button1Text, string title, string textBoxText, RunButtonClick button1Click, bool selectAll) {
            mainForm = main;
            InitializeComponent();
            if (textBoxText != null) {
                textBox1.Text = textBoxText;
            }
            if (selectAll) {
                textBox1.SelectAll();
            }
            else {
                textBox1.Select(textBox1.Text.Length, 0);
            }
            Text = title;
            button1.Text = button1Text;
            label1.Text = label1Text;
            onClick = button1Click;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox1.Text.Equals("")) {
                toolTip1.ToolTipTitle = Text;
                toolTip1.Show("You must enter some text here", textBox1);
            }
            else {
                onClick.Invoke(textBox1.Text);
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
