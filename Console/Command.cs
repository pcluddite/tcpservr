// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Tcpservr.Messaging;

namespace Tcpservr.Terminal
{
    public static class Command
    {
        public static bool Connect(CommandLine tMsg, TcpClient client)
        {
            if (tMsg.Count == 1) {
                if (client.Client != null && client.Connected) {
                    Console.WriteLine("A server connection has already been established!");
                    return false;
                }
                try {
                    IPEndPoint ep;
                    if (!Settings.GetHost(out ep)) {
                        Console.WriteLine("The client is not connected.");
                        return false;
                    }
                    Console.Write("Connecting to {0}:{1}...", ep.Address, ep.Port);
                    client.Connect(ep);
                    Console.WriteLine("OK!");
                    Console.WriteLine("Connection established!");
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine("FAILED!");
                    PrintException(ex);
                }
            }
            else {
                Console.WriteLine("'CONNECT' does not take any arguments");
            }
            return false;
        }

        public static bool Disconnect(CommandLine tMsg, TcpClient client)
        {
            if (tMsg.Count == 1) {
                try {
                    client.Client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    Console.WriteLine("Disconnected from remote host.");
                    return true;
                }
                catch (Exception ex) {
                    PrintException(ex);
                }
            }
            else {
                Console.WriteLine("'DISCONNECT' does not take any arguments.");
            }
            return false;
        }

        public static void Set(CommandLine tMsg)
        {
            if (tMsg.Count == 2) {
                string[] var = tMsg[1].Split('=');
                if (var.Length == 2) {

                    if (string.IsNullOrEmpty(var[1])) {
                        Settings.RemoveValue(var[0]);
                    }
                    else {
                        Settings.SetValue(var[0], var[1]);
                    }
                }
                else {
                    Console.WriteLine(Settings.GetValue(tMsg[1]));
                }
            }
            else if (tMsg.Count == 1) {
                foreach (var kv in Settings.Variables) {
                    Console.WriteLine("{0}={1}", kv.Key, kv.Value);
                }
            }
            else {
                Console.WriteLine("'SET' had too many arguments.");
            }
        }

        public static void Exit(TcpClient client)
        {
            try {
                Settings.RemoveValue("lastdata");
                Settings.Save();
                if (client != null && client.Connected) {
                    client.Client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (Exception ex) {
                WriteError("An error occoured while exiting:\n{0}", ex.Message);
                Console.ReadKey(intercept: true);
            }
        }

        public static void WriteError(string format, params object[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Settings.SetColor(Settings.Color);
        }

        public static void PrintException(Exception ex)
        {
            if (ex is NullReferenceException || ex.Message.Contains("no address was supplied")) {
                Console.WriteLine("The client is not connected to a remote host.");
            }
            else if (ex is ObjectDisposedException) {
                Console.WriteLine("The client has been disconnected.");
            }
            else if (ex.Message.Contains("target machine actively refused")) {
                Console.WriteLine("The host machine is not accepting any connections at that port.");
            }
            else if (ex is SocketException) {
                Console.WriteLine("Unable to connect to the remote host.");
            }
            else {
                Console.WriteLine("A network exception occoured:\r\n" + ex.Message);
            }
        }

        public static void Help()
        {
            Console.WriteLine("List of Local Commands");
            Console.WriteLine();
            Console.WriteLine("{0,-25}{1,-50}", "cls", "Clears the screen");
            Console.WriteLine("{0,-25}{1,-50}", "color", "Changes the console color. (/? for more)");
            Console.WriteLine("{0,-25}{1,-50}", "connect", "Connects to the remote host");
            Console.WriteLine("{0,-25}{1,-50}", "disconnect", "Ends connection to the remote host");
            Console.WriteLine("{0,-25}{1,-50}", "exit", "Ends connection to the host (if any) and exits");
            Console.WriteLine("{0,-25}{1,-50}", "prompt", "Changes the console prompt (/? for more)");
            Console.WriteLine("{0,-25}{1,-50}", "saveas", "Saves the last data into a file (for byte messages)");
            Console.WriteLine("{0,-25}{1,-50}", "set host=HOST", "Sets the host computer");
            Console.WriteLine("{0,-25}{1,-50}", "set port=PORT", "Sets the socket port for the host");
            Console.WriteLine("{0,-25}{1,-50}", "set timeout=TIME", "Sets the time limit (in milliseconds) to read messages");
            Console.WriteLine("{0,-25}{1,-50}", "set VAR=VALUE", "Sets an environment variable to be used in %'s");
            Console.WriteLine();
            Console.WriteLine("These commands are local commands. For remote commands refer to documentation.");
        }

        public static void Color(CommandLine tMsg)
        {
            if (tMsg.Count == 2) {
                ConsoleColor fcurrent = Console.ForegroundColor;
                ConsoleColor bcurrent = Console.BackgroundColor;
                if (tMsg[1].Equals("/?")) {
                    try {
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.Arguments = "/c color /?";
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.Start();
                        Console.WriteLine(p.StandardOutput.ReadToEnd());
                    }
                    catch (Exception ex) {
                        PrintException(ex);
                    }
                    return;
                }
                if (!Settings.SetColor(tMsg[1])) {
                    Console.ForegroundColor = fcurrent;
                    Console.BackgroundColor = bcurrent;
                }
                else {
                    Settings.SetValue("color", tMsg[1]);
                }
            }
            else {
                Console.WriteLine("The syntax of the command was invalid.");
            }
        }

        public static string Prompt(CommandLine tMsg)
        {
            if (tMsg.Count > 1 && tMsg[1] == "/?") {
                try {
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = "/c prompt /?";
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    Console.WriteLine(p.StandardOutput.ReadToEnd());
                }
                catch (Exception ex) {
                    PrintException(ex);
                }
                return null;
            }
            return tMsg[1];
        }

        public static bool Pass(CommandLine tMsg)
        {
            if (tMsg.Count == 2) {
                if (string.IsNullOrEmpty(tMsg[1])) {
                    //Settings.Password = null;
                    Console.WriteLine("No key will be used to send and receive data.");
                    return true;
                }
                else {
                    //Settings.Password = tMsg[1];
                    Console.WriteLine("Data will now be sent and received with this key.");
                    return true;
                }
            }
            else {
                Console.WriteLine("The syntax of the command was invalid.");
            }
            return false;
        }

        public static void SaveAs(CommandLine tMsg, string lastData)
        {
            if (tMsg.Count != 2) {
                Console.WriteLine("The syntax of the command was incorrect.");
            }
            try {
                File.WriteAllBytes(tMsg[1], Convert.FromBase64String(lastData));
                Console.WriteLine("The last data received was saved as '{0}'.", tMsg[1]);
            }
            catch (Exception ex) {
                PrintException(ex);
            }
        }
    }
}
