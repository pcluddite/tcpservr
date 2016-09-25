using System;
using System.Text;
using System.Windows.Forms;
using tbas = TbasicOld.Components;

namespace Tcpclient {
    public partial class Pass : Form {

        bool isGood {
            get {
                return !Program.Password.Equals("");
            }
        }

        public Pass() {
            InitializeComponent();
            textBox1.Focus();
            textBox1.Select();
        }

        private void button1_Click(object sender, EventArgs e) {
            Program.Password = textBox1.Text;
            if (checkBox1.Checked) {
                using (tbas.TcpSecure security = new tbas.TcpSecure("7CyveqhEMtSG4eVm")) {
                    Program.Settings.Set("PrivateKey",
                        Convert.ToBase64String(security.Encrypt(Encoding.UTF8.GetBytes(textBox1.Text)))
                        );
                }
            }
            Close();
        }

        private void Pass_FormClosing(object sender, FormClosingEventArgs e) {
            if (!isGood) {
                if (MessageBox.Show(this, "Are you sure you want to continue without a password?\r\nThe program will be forced to exit.",
                this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No) {
                    e.Cancel = true;
                }
                else {
                    Program.Log("You did not set a password. Application was forced to exit.", "Password.Set()");
                    Program.Password = null;
                }
            }
        }

        private void Pass_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    Program.Password = "";
                    Close();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    button1_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e) {
            pictureBox2.Image = Properties.Resources.go_selected;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e) {
            pictureBox2.Image = Properties.Resources.go;
        }
    }
}
