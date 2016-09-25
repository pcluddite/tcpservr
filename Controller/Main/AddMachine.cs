using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tcpclient {
    public partial class AddMachine : Form {
        private Hosts hostForm;
        private string name, address, port;
        private bool isUpdate;
        private ListViewItem item;

        public AddMachine(Hosts hosts, string name, string address, string port) {
            InitializeComponent();
            hostForm = hosts;
            this.name = name;
            this.address = address;
            this.port = port;
            isUpdate = (item = GetItem(name)) != null;
        }

        private void AddMachine_Load(object sender, EventArgs e) {
            textBox1.Text = name;
            textBox2.Text = address;
            textBox3.Text = port;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!checkStuff()) {
                return;
            }
            if (!textBox1.Text.Equals(name) && GetItem(textBox1.Text) != null) {
                textBox1.Focus();
                textBox1.SelectAll();
                toolTip1.Show("The computer name must be unique.", textBox1);
            }
            else {
                if (isUpdate) {
                    Settings comp = new Settings(item.Text);
                    comp.SetAttribute("name", textBox1.Text);
                    comp.SetAttribute("address", textBox2.Text);
                    comp.SetAttribute("port", textBox3.Text);
                    item.Text = textBox1.Text;
                    item.SubItems[1].Text = textBox2.Text;
                    item.SubItems[2].Text = textBox3.Text;
                }
                else {
                    Settings.AddComputer(textBox1.Text, textBox2.Text, textBox3.Text);
                    item = new ListViewItem(textBox1.Text);
                    item.SubItems.Add(textBox2.Text);
                    item.SubItems.Add(textBox3.Text);
                    hostForm.listView1.Items.Add(item);
                }
                Close();
            }
        }

        private ListViewItem GetItem(string name) {
            foreach (ListViewItem item in hostForm.listView1.Items) {
                if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                    return item;
                }
            }
            return null;
        }

        private bool checkStuff() {
            int temp;
            if (textBox1.Text.Trim().Equals("")) {
                textBox1.Focus();
                textBox1.SelectAll();
                toolTip1.Show("Please enter a name for the server.", textBox1);
                return false;
            }
            else if (textBox2.Text.Trim().Equals("")) {
                textBox2.Focus();
                textBox2.SelectAll();
                toolTip1.Show("Please enter a valid IP Address or DNS host name.", textBox2);
                return false;
            }
            else if (textBox3.Text.Trim().Equals("")) {
                textBox3.Focus();
                textBox3.SelectAll();
                toolTip1.Show("Please enter the port number of the host machine.", textBox3);
                return false;
            }
            else if (!int.TryParse(textBox3.Text, out temp)) {
                textBox3.Focus();
                textBox3.SelectAll();
                toolTip1.Show("The port number must be a valid integer.", textBox3);
                return false;
            }
            else {
                return true;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
