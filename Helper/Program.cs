using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Tcpservr
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arg)
        {
            if (arg.Length < 1) {
                Environment.Exit(400);
            }

            if (arg[0].ToUpper().CompareTo("CHAT") == 0) {
                if (arg.Length != 2) {
                    Environment.Exit(400);
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new chat(arg[1]));
                Environment.Exit(200);
            }
            else if (arg[0].ToUpper().CompareTo("BLOCKMESSAGE") == 0) {
                if (arg.Length != 2) {
                    Environment.Exit(400);
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BlockInput(arg[1]));
                Environment.Exit(200);
            }
            else if (arg[0].ToUpper().CompareTo("UPDATE") == 0) {
                if (arg.Length != 3) {
                    Environment.Exit(400);
                }
                Updater updater = new Updater(arg[1], arg[2]);
                Environment.Exit(updater.Run());
            }
            else if (arg[0].ToUpper().CompareTo("MSGBOX") == 0) {
                if (arg.Length != 4) {
                    Environment.Exit(400);
                }
                int flag;
                if (!int.TryParse(arg[1], out flag)) {
                    Environment.Exit(400);
                }
                MsgBox msgbox = new MsgBox(flag, arg[2], arg[3]);
                msgbox.Show();
                Environment.Exit(200);
            }
        }
    }
}
