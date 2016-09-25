using System.IO;
using tbas = TbasicOld.Components;

namespace Tcpclient.Components {
    public class TMessage : tbas.TMessage {

        public TMessage()
            : base() {
        }

        public TMessage(int id)
            : base(id) {
        }

        public TMessage(Stream stream) :
            base(stream) {
        }

        public override void Send() {
            if (!Program.Password.Equals("")) {
                ProcessEncrypt(DataString, Program.Password);
            }
            base.Send();
        }
    }
}
