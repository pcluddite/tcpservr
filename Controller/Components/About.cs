using System.Windows.Forms;

namespace Tcpclient
{
    public partial class About : Form {
        public About() {
            InitializeComponent();
            linkLabel2.Links.Add(0, linkLabel2.Text.Length, "http://www.visualpharm.com/");
            linkLabel4.Links.Add(0, linkLabel4.Text.Length, "http://tatice.deviantart.com/");
        }

        private void About_Load(object sender, System.EventArgs e) {
            label3.Text = Program.VER_STRING;
            label1.Text = Program.COPYRIGHT;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void About_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
    }
}
