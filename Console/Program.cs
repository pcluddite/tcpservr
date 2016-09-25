// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using Tcpservr.Messaging;

namespace Tcpservr.Terminal
{
    public class Program
    {
        public const string VER = "TCPSERVR Console [Version 2.0.2016]";
        public const string COPYRIGHT = "Copyright (c) 2011-2016 Tim Baxendale";

        public static void Main(string[] args)
        {
            string remoteDir = string.Empty;
            uint msgId = 0;

            Console.Title = "TCPSERVR Console";
            Console.WriteLine(VER);
            Console.WriteLine(COPYRIGHT);

            TcpClient client = new TcpClient();

            ServerInputMessage tMsg;
            ServerOutputMessage response = null;

            MessageReceiver receiver = null;

            while(true) {
                Console.WriteLine();
                if (client.Client == null || !client.Connected) {
                    remoteDir = string.Empty;
                }
                Console.Write(Settings.GetPrompt(remoteDir));

                string line = Console.ReadLine();

                tMsg = new ServerInputMessage(Settings.ReplaceVariables(line));

                switch (tMsg.Name.ToUpper()) {
                    #region Commands
                    case "EXIT":
                        Command.Exit(client);
                        return;
                    case "SET":
                        Command.Set(new CommandLine(tMsg.Message));
                        break;
                    case "CONNECT":
                        if (Command.Connect(new CommandLine(tMsg.Message), client)) {
                            receiver = new MessageReceiver(client.GetStream(), false);
                        }
                        break;
                    case "DISCONNECT":
                        if (Command.Disconnect(new CommandLine(tMsg.Message), client)) {
                            client = new TcpClient();
                            receiver = null;
                        }
                        break;
                    case "HELP":
                        Command.Help();
                        break;
                    case "CLS":
                        Console.Clear();
                        break;
                    case "COLOR":
                        Command.Color(new CommandLine(tMsg.Message));
                        break;
                    case "PROMPT":
                        string tmp = Command.Prompt(new CommandLine(tMsg.Message));
                        if (tmp != null) {
                            Settings.Prompt = tmp;
                        }
                        break;
                    case "SAVE":
                    case "SAVEAS":
                        //Command.SaveAs(new CommandLine(tMsg.Message), lastResponse);
                        break;
                    #endregion

                    default:
                        try {
                            tMsg.ID = unchecked(msgId++);
                            receiver.SendMessage(tMsg);
                            response = receiver.ReceiveResponse();

                            switch (tMsg.Name.ToUpper()) {
                                case "CD":
                                case "PWD":
                                    if (response.Status == 200) { // ok
                                        remoteDir = (string)response.Response;
                                    }
                                    break;
                            }

                            if (response.Status == 502) // bad gateway
                                remoteDir = "";

                            if (response != null)
                                WriteResponse(response);
                        }
                        catch (Exception ex) {
                            Command.PrintException(ex);
                        }
                        break;
                }
            }
        }

        private static void WriteResponse(ServerOutputMessage response)
        {
            //Console.WriteLine(response.ID);
            Console.WriteLine("{0} {1}", response.Status, response.StatusMessage);
            string resp = ObjectToString(response.Response);
            if (resp.EndsWith(Environment.NewLine)) {
                resp = resp.Substring(0, resp.Length - Environment.NewLine.Length); // a newline will be added regardless, and I hate double newlines 6/9/16
            }
            Console.WriteLine(resp);
        }

        private static string ObjectToString(object o)
        {
            if (o is string || o is int || o is double || o is decimal || o is char || o is long || o is bool)
                return o.ToString(); // It's a supported type. Serialize it normally.

            return JsonConvert.SerializeObject(o, Formatting.Indented); // Maybe json has a way of representing this? (good for arrays)
        }
    }
}
