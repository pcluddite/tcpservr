using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using tbas = TbasicOld.Components;

namespace Tcpclient.Components {
    public class TResponse : tbas.TResponse {
        public TResponse()
            : base() {
        }

        public TResponse(Stream stream)
            : base(stream) {
        }

        public TResponse(TMessage original)
            : base(original) {
        }

        public TResponse(Stream stream, TMessage original)
            : base(stream, original) {
        }


        private bool isFirst = true;
        public int Receive() {
            byte[] data;
            int len = Receive(Stream, out data);
            Process(data);
            bool promptPass = false;
            if (IsEncrypted) {
                try {
                    Process(Decrypt(data, Program.Password));
                }
                catch {
                    promptPass = true;
                }
            }
            while (promptPass || Status == 401) {
                promptPass = false;
                if (isFirst) {
                    isFirst = false;
                }
                else {
                    MessageBox.Show(
                        "The encryption was not recognized. The password may be incorrect.",
                        "Password Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                Pass passDialog = new Pass();
                passDialog.ShowDialog();
                if (Program.Password == null) {
                    Stream.Close();
                    Environment.Exit(500);
                }
                else {
                    TMessage testMsg = new TMessage(Stream);
                    testMsg.ProcessEncrypt("HELLO", Program.Password);
                    testMsg.Send();
                    len = Receive(Stream, out data);
                    try {
                        Process(Decrypt(data, Program.Password));
                    }
                    catch {
                        promptPass = true;
                    }
                }
            }
            return len;
        }

        private static int Receive(Stream stream, out byte[] data) {
            tbas.TReceiver receiver = new tbas.TReceiver(stream);
            return receiver.Receive(out data);
        }

        private static byte[] Decrypt(byte[] data, string pass) {
            using (tbas.TcpSecure security = new tbas.TcpSecure(pass)) {
                byte[] decrypted = security.Decrypt(data.Skip(7).ToArray());
                byte[] newData = new byte[7 + decrypted.Length];
                tbas.TReceiver.TCP.CopyTo(newData, 0);
                BitConverter.GetBytes(decrypted.Length).CopyTo(newData, 3);
                decrypted.CopyTo(newData, 7);
                return newData;
            }
        }

        private static byte[] Encrypt(string message, string pass) {
            using (tbas.TcpSecure tcpSecure = new tbas.TcpSecure(pass)) {
                byte[] encrypted = tcpSecure.Encrypt(Encoding.UTF8.GetBytes(message));
                byte[] data = new byte[7 + encrypted.Length];
                Encoding.UTF8.GetBytes("ENC").CopyTo(data, 0);
                BitConverter.GetBytes(encrypted.Length).CopyTo(data, 3);
                encrypted.CopyTo(data, 7);
                return data;
            }
        }
    }
}
