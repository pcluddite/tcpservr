using System;
using System.IO;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Security.Principal;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Tcpservr
{
    public class Updater {

        public string OriginalAppPath { get; set; }
        public string UpdateAppPath { get; set; }

        public Updater(string path1, string path2) {
            this.OriginalAppPath = path1;
            this.UpdateAppPath = path2;
        }

        public int Run() {
            if (!File.Exists(UpdateAppPath)) {
                return 404;
            }
            try {
                new Thread(new ThreadStart(listen)).Start();

                while (ProcessExists("tcpservr")) {
                    Thread.Sleep(10);
                }
                if (File.Exists(OriginalAppPath)) {
                    File.Delete(OriginalAppPath);
                }
                File.Move(UpdateAppPath, OriginalAppPath);
                Process.Start(new ProcessStartInfo(OriginalAppPath, "-start"));
                return 202;
            }
            catch {
                return 500;
            }
        }

        private void listen() {
            try {
                PipeSecurity ps = new PipeSecurity();
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                ps.SetAccessRule(
                    new PipeAccessRule(account, PipeAccessRights.ReadWrite,
                        System.Security.AccessControl.AccessControlType.Allow));

                NamedPipeServerStream server = new NamedPipeServerStream("TCPSERVR-Updater",
                    PipeDirection.InOut, 254, PipeTransmissionMode.Message, PipeOptions.None, 16384, 16384, ps);

                while (true) {
                    server.WaitForConnection();
                    TReceiver receiver = new TReceiver(server);
                    byte[] data;
                    int len = receiver.Receive(out data);
                    if (len == -1) {
                        server.Write(
                            Encoding.UTF8.GetBytes("400 Invalid message"), 0, "400 Invalid message".Length);
                        server.WaitForPipeDrain();
                        server.Disconnect();
                        continue;
                    }
                    TMessage msg = new TMessage();
                    msg.Process(data);

                    TResponse response = new TResponse(msg);
                    response.Process(501, "Not Implemented: " + msg.Args[0].ToUpper());
                    if (msg.Args[0].ToLower().CompareTo("exit") == 0) {
                        response.Process(202, "Exiting updater...");
                        server.Write(response.Data, 0, response.Length);
                        server.WaitForPipeDrain();
                        server.Disconnect();
                        Environment.Exit(3);
                    }
                    server.Write(response.Data, 0, response.Length);
                    server.WaitForPipeDrain();
                    server.Disconnect();
                }
            }
            catch {
            }
        }

        static bool ProcessExists(string name) {
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName == name)
                    return true;
            }
            return false;
        }
    }
}
