using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Tcpclient {

    public class Program {

        private static Logger log = new Logger(Application.StartupPath + "\\Error Log.log");
        public static string Password = "";
        public static Settings Settings = null;

        public const string VER_STRING = "TcpServr Controller [Version 1.5.2014]";
        public const string COPYRIGHT = "Copyright (c) 2011-2014 Timothy Baxendale";

        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Hosts());
        }

        public static void Log(Exception ex, string target) {
            log.Log(ex, target);
        }

        public static void Log(string message, string target) {
            log.Log(message, target);
        }
    }
}
