using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tcpclient {
    public class Logger {

        public string Path { get; set; }

        public Logger(string path) {
            this.Path = path;
        }

        public void Log(Exception ex, string target) {
            this.Log(ex.Message, target);
        }
        
        public void Log(string message, string target) {
            try {
                File.AppendAllText(this.Path, DateTime.Now.ToString("[MM/dd/yyyy hh:mm:ss]") + message + " @ " + target + "\r\n");
            }
            catch {
            }
        }
    }
}
