using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;
using Tcpservr.Core;
using Tcpservr.Errors;
using Tcpservr.Libraries;
using Tcpservr.Messaging;

namespace Tcpservr
{
    public class Program
    {
        public const string VER = "TCPSERVR [Version 2.0.2016]";

        public static void Main(string[] args)
        {
            if (File.Exists(Application.StartupPath + "\\tbasic.dll")) {
                Start(args);
            }
            else {
                DisplayError("Could not find file '" + Application.StartupPath + "\\tbasic.dll'. Ensure this file exists in the same directory as TCPSERVR.");
            }
        }

        public static void Start(string[] args)
        {
            if (args.Length == 1) {
                if (args[0].Equals("--start", StringComparison.OrdinalIgnoreCase)) {
#if !THROW_ERRORS
                    try {
#endif
                        ServerCore.BeginMaster();
#if !THROW_ERRORS
                    }
                    catch (Exception masterEx) {
                        SocketException socketEx = masterEx as SocketException;
                        if (socketEx != null && socketEx.SocketErrorCode == SocketError.AddressAlreadyInUse) {
                            try {
                                ServerCore.BeginSlave();
                            }
                            catch (Exception salveEx) {
                                WriteError(new LoggedError("--", salveEx, dominant: false, fatal: true));
                            }
                        }
                        else {
                            WriteError(new LoggedError("--", masterEx, dominant: true, fatal: true));
                        }
                    }
#endif
                }
                else if (args[0].Equals("--stop", StringComparison.OrdinalIgnoreCase)) {
                    ServerOutputMessage response = SendPipeMessage(new ServerInputMessage("QUIT"));
                    if (response != null)
                        DisplayInfo(response.StatusMessage);
                }
                else if (args[0].Equals("--listusers", StringComparison.OrdinalIgnoreCase)) {
                    ServerOutputMessage response = SendPipeMessage(new ServerInputMessage("GETUSERS"));
                    if (response != null) {
                        if (response.Status == ErrorSuccess.NoContent) {
                            DisplayInfo("No users are currently connected to the server.");
                        }
                        else {
                            DisplayInfo(response.Response.ToString());
                        }
                    }
                }
                else if (File.Exists(args[0])) {
                    try {
                        TRuntime exec = new TRuntime();
                        using (StreamReader reader = new StreamReader(File.OpenRead(args[0]))) {
                            exec.Execute(reader);
                        }
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) {
                        DisplayError(ex.Message);
                    }
                }
                else {
                    DisplayError("Unknown argument '" + args[0] + "'");
                }
            }
            else if (args.Length > 1) {
                if (Installer.TryInstall(args) || Installer.TryRemove(args)) {
                    return;
                }
                else if (args[0].Equals("-R", StringComparison.OrdinalIgnoreCase)) {
                    Process p = new Process();
                    p.StartInfo.FileName = args[1];
                    if (args.Length > 3) {
                        CommandLine cmd = new CommandLine();
                        for (int i = 3; i < args.Length; i++) {
                            cmd.Add(args[i]);
                        }
                        p.StartInfo.Arguments = cmd.ToString();
                    }
                    p.Start();
                }
                else if (args.Length > 3 && args[0].Equals("-M", StringComparison.OrdinalIgnoreCase)) {
                    int flag;
                    if (!int.TryParse(args[1], out flag)) {
                        DisplayError("Argument two was not a valid flag");
                    }
                    UserIOLibrary.MsgBox(buttons: flag, prompt: args[2], title: args[3]);
                }
            }
            else {
                DisplayError("The arguments passed to this program were invalid.");
            }
        }

        public static void DisplayError(string message)
        {
            MessageBox.Show(message, VER, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void DisplayInfo(string message)
        {
            MessageBox.Show(message, VER, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static ServerOutputMessage SendPipeMessage(ServerInputMessage pmsg, string pipename = MasterPipe.PIPENAME)
        {
            try {
                return PipeLibrary.PipeUse(pipename, pmsg);
            }
            catch (FunctionException ex) {
                if (ex.Status == ErrorServer.BadGateway) {
                    DisplayError("No instance of TCPSERVR is running that can be stopped.");
                }
                else {
                    DisplayError(ex.Message);
                }
                return null;
            }
        }

        public static void CleanResources()
        {
            string[] resources = new string[] {
                Path.GetTempPath() + "\\thelper.exe",
                Path.GetTempPath() + "\\7za.exe",
                Application.StartupPath + "\\Updater.exe"
            };
            foreach (string path in resources) {
                try {
                    if (File.Exists(path)) {
                        File.Delete(path);
                    }
                }
                catch (Exception ex) {
                    WriteError(new LoggedError("CleanResources", ex, dominant: false, fatal: false));
                }
            }
        }

        public static void WriteError(LoggedError error)
        {
            try {
                using (StreamWriter sw = new StreamWriter(Path.Combine(ServerCore.ApplicationDirectory, "DUMP.TXT"), append: true)) {
                    sw.WriteLine(error);
                }
            }
            catch {
                // No need to throw an exception. We're already crashing.
            }
        }
    }
}