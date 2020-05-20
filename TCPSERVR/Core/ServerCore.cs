// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;

using TCPSERVR.Errors;
using TCPSERVR.Libraries;
using TCPSERVR.Messaging;
using TCPSERVR.Threads;

namespace TCPSERVR.Core
{
    public static class ServerCore
    {
        public static string ApplicationDirectory
        {
            get {
                return Application.StartupPath;
            }
        }

        public static string ExecutablePath
        {
            get {
                return Application.ExecutablePath;
            }
        }

        public static ThreadCollection Threads { get; set; } = new ThreadCollection();
        public static LoggedErrorCollection ErrorLog { get; set; } = new LoggedErrorCollection(10);
        public static ServerSettings Settings { get; private set; } = new ServerSettings();

        public static string GlobalPipe { get; set; } = null;
        public static bool IsDominant { get; set; } = true;

        public static MasterPipe MasterPipe { get; private set; }
        public static TcpListener Listener { get; private set; } = null;

        public static bool Listening { get { return Listener != null; } }

        public static void BeginMaster()
        {
            if (Listening)
                throw new InvalidOperationException("Server is already listening");

            try {
                MasterPipe = new MasterPipe();
                Listener = new TcpListener(Settings.EndpointIP);

                Thread resCleaner = new Thread(Program.CleanResources);
                resCleaner.Start();

                MasterPipe.BeginStart();
                Listener.Start();

                while(Listening) {
                    TcpClient client = null;
                    try {
                        client = Listener.AcceptTcpClient();
                    }
                    catch (SocketException ex) {
                        if (ex.SocketErrorCode == SocketError.Interrupted) {
                            break;
                        }
                        else {
                            throw;
                        }
                    }

                    MasterMode master = new MasterMode(client);

                    ThreadInfo tInfo = new ThreadInfo(master, Settings.LogHistory);
                    Threads.Add(tInfo);
                    tInfo.Start();
                }
            }
            finally {
                Listener = null;
            }
        }

        public static void StopMaster()
        {
            Listener.Stop();
            Listener = null;
        }

        public static void BeginSlave()
        {
            try {
                using (NamedPipeClientStream client = new NamedPipeClientStream(MasterPipe.PIPENAME)) {
                    client.Connect(5000);
                    client.ReadMode = PipeTransmissionMode.Message;
                    string id = GetPipeId();
                    string name = WindowsIdentity.GetCurrent().Name;
                    int indexOfSlash = name.LastIndexOf('\\');
                    if (indexOfSlash < 0) {
                        name = name.Substring(indexOfSlash + 1);
                    }

                    using (MessageReceiver receiver = new MessageReceiver(client)) {

                        receiver.SendMessage(new ServerInputMessage(new CommandLine("REGISTER", name, id).ToString())); // tell the master application that we're here
                        client.WaitForPipeDrain();

                        byte[] data;
                        int len = receiver.Receive(out data);
                        if (len == -1) {
                            Program.WriteError(new LoggedError("--",
                                "An invalid message was received when attempting to contact the master application.",
                                "Receiver.PRead", dominant: false, fatal: true));
                            Environment.Exit(ErrorServer.GenericError);
                        }

                        ServerOutputMessage response = MessageReceiver.ProcessResponse(data);
                        if (response.Status == ErrorSuccess.OK) {
                            Thread t = new Thread(CheckForMaster);
                            t.Start();
                            StartChildSpawner(id);
                        }
                        else {
                            Program.WriteError(new LoggedError("--", "Submissive application returned: " + response.Status + " " + response.StatusMessage,
                                "Receiver.PRead",
                                dominant: false, fatal: true));
                        }
                    }
                }
            }
            catch (Exception ex) {
                Program.WriteError(new LoggedError("--", ex, dominant: false, fatal: true));
                Environment.Exit(ErrorServer.GenericError);
            }
        }

        private static void StartChildSpawner(string id)
        {
            try {
                using (NamedPipeServerStream server = new NamedPipeServerStream(id, PipeDirection.InOut, 1,
                    PipeTransmissionMode.Message, PipeOptions.None, 4096, 4096, GetGlobalAccessLevel())) {
                    while (true) {
                        server.WaitForConnection();

                        MessageReceiver receiver = new MessageReceiver(server, dispose: false);
                        byte[] data;
                        int len = receiver.Receive(out data);
                        if (len == -1) {
                            Program.WriteError(new LoggedError("--", "An invalid message was received", "receiver.PRead", dominant: false, fatal: false));
                            server.Disconnect();
                            continue;
                        }

                        ServerInputMessage input;
                        ServerOutputMessage response;

                        try {
                            input = MessageReceiver.ProcessMessage(data);
                        }
                        catch (JsonException) {
                            response = new ServerOutputMessage(ErrorClient.BadRequest, null);
                            receiver.SendResponse(response);
                            continue;
                        }

                        StackData pMsg = new StackData(ExecuterOption.Strict, new CommandLine(input.Message));

                        try {
                            if (pMsg.Name.Equals("OPEN", StringComparison.OrdinalIgnoreCase)) {
                                pMsg.AssertCount(2);

                                SlaveMode slave = new SlaveMode(pMsg.Get<string>(1));

                                ThreadInfo info = new ThreadInfo(slave, Settings.LogHistory);
                                Threads.Add(info);
                                info.Thread.Start();
                                response = new ServerOutputMessage(ErrorSuccess.OK, null);
                            }
                            else {
                                throw new FunctionException(ErrorServer.NotImplemented, pMsg.Name.ToUpper());
                            }
                        }
                        catch (FunctionException ex) {
                            response = new ServerOutputMessage(ex.Status, ex.Message, null);
                        }
                        catch (Exception ex) {
                            response = new ServerOutputMessage(ErrorServer.GenericError, ex.Message, null);
                        }
                        receiver.SendResponse(response);
                        server.WaitForPipeDrain();
                        server.Disconnect();
                    }
                }
            }
            catch (Exception ex) {
                Program.WriteError(new LoggedError("--", ex, dominant: false, fatal: true));
                Environment.Exit(ErrorServer.GenericError);
            }

        }

        public static PipeSecurity GetGlobalAccessLevel()
        {
            PipeSecurity ps = new PipeSecurity();
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            ps.SetAccessRule(new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
            return ps;
        }

        private static string GetPipeId()
        {
            string id;
            do {
                id = "tcpservr_pipe" + ((int)MathLibrary.Random(1000)).ToString("000");
            }
            while (PipeLibrary.PipeExists(id));
            return id;
        }

        private static void CheckForMaster()
        {
            while (PipeLibrary.PipeExists(MasterPipe.PIPENAME)) {
                Thread.Sleep(5000);
            }
            Environment.Exit(ErrorClient.NotFound);
        }
    }
}
