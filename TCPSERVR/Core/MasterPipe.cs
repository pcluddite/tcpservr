// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Tbasic.Errors;
using Tbasic.Runtime;
using Tcpservr.Errors;
using Tcpservr.Libraries;
using Tcpservr.Messaging;
using Tcpservr.Threads;

namespace Tcpservr.Core
{
    public class MasterPipe
    {
        public const string PIPENAME = "TCPSERVR";
        private static ConcurrentDictionary<string, string> users = new ConcurrentDictionary<string, string>();

        private static object locker = new object();

        public MasterPipe()
        {
        }

        public void BeginStart()
        {
            Thread t = new Thread(Start);
            t.Start();
        }

        public void Start()
        {
            try {
                using (NamedPipeServerStream server = new NamedPipeServerStream(PIPENAME,
                    PipeDirection.InOut, 254, PipeTransmissionMode.Message, PipeOptions.None, 16384, 16384, ServerCore.GetGlobalAccessLevel())) {

                    while (true) {
                        server.WaitForConnection();

                        MessageReceiver receiver = new MessageReceiver(server);

                        byte[] data;
                        int len = receiver.Receive(out data);
                        if (len == -1) {
                            server.Disconnect();
                            continue;
                        }

                        ServerInputMessage pMsg;
                        ServerOutputMessage response;
                        try {
                            pMsg = MessageReceiver.ProcessMessage(data);
                        }
                        catch (JsonException) {
                            response = new ServerOutputMessage(ErrorClient.BadRequest, null);
                            receiver.SendResponse(response);
                            server.WaitForPipeDrain();
                            server.Disconnect();
                            continue;
                        }

                        StackData args = new StackData(ExecuterOption.Strict, new CommandLine(pMsg.Message));

                        try {
                            switch (args.Name.ToUpper()) {
                                case "REGISTER":
                                    args.AssertCount(3);
                                    string username = args.Get<string>(1);
                                    string newpipe = args.Get<string>(2);
                                    string oldpipe;
                                    if (users.TryGetValue(username, out oldpipe)) {
                                        if (!PipeLibrary.PipeExists(oldpipe)) {
                                            if (!users.TryUpdate(username, newpipe, oldpipe))
                                                throw new FunctionException(ErrorServer.GenericError, "Unable to update broken pipe with new pipe");
                                            response = new ServerOutputMessage(ErrorSuccess.OK, null);
                                        }
                                        else {
                                            throw new FunctionException(ErrorClient.Conflict, "Pipe already exists");
                                        }
                                    }
                                    else {
                                        if (users.TryAdd(username, newpipe))
                                            response = new ServerOutputMessage(ErrorSuccess.OK, null);
                                        throw new FunctionException(ErrorServer.GenericError, "Unable to register user");
                                    }
                                    break;
                                case "CHANGEPIPE":
                                    args.AssertCount(3);
                                    int id = args.Get<int>(1);
                                    ThreadInfo thread;
                                    if (!ServerCore.Threads.TryGetValue(id, out thread)) {
                                        throw new FunctionException(ErrorClient.NotFound, "Thread");
                                    }
                                    MasterMode master = thread.Mode as MasterMode; // so it isn't cleaned up while we're using it
                                    if (thread.Mode != null) {
                                        if (args.Get<string>(2).Equals("")) {
                                            master.CurrentPipeName = null;
                                        }
                                        else {
                                            master.CurrentPipeName = args.Get<string>(2);
                                        }
                                        response = new ServerOutputMessage(ErrorSuccess.OK, "Pipe has been set on thread " + id);
                                    }
                                    else {
                                        throw new FunctionException(ErrorClient.Forbidden, "Thread is either not master or client is not active. No pipe changed.");
                                    }
                                    break;
                                case "HELLO":
                                    response = new ServerOutputMessage(ErrorSuccess.OK, "Hello!");
                                    break;
                                case "QUIT":
                                    response = new ServerOutputMessage(ErrorSuccess.OK, "All instances of the application will now close.", null);
                                    receiver.SendResponse(response);
                                    server.WaitForPipeDrain();
                                    server.Disconnect();
                                    Environment.Exit(ErrorSuccess.OK);
                                    break;
                                case "GETUSERS":
                                    StringBuilder sb = new StringBuilder();
                                    foreach (var user in GetCopyOfUsers()) {
                                        sb.AppendFormat("{0}|{1}", user.Key, user.Value);
                                        sb.AppendLine();
                                    }
                                    if (sb.Length == 0) {
                                        response = new ServerOutputMessage(ErrorSuccess.NoContent, null);
                                    }
                                    else {
                                        response = new ServerOutputMessage(ErrorSuccess.OK, sb.ToString().Trim());
                                    }
                                    break;
                                default:
                                    throw new FunctionException(ErrorServer.NotImplemented, args.Name.ToUpper());
                            }
                        }
                        catch (FunctionException ex) {
                            response = new ServerOutputMessage(ex.Status, ex.Message);
                        }
                        receiver.SendResponse(response);
                        server.WaitForPipeDrain();
                        server.Disconnect();
                    }
                }
            }
            catch (Exception ex) {
                Program.WriteError(new LoggedError(PIPENAME, ex, dominant: true, fatal: false));
            }
        }

        public static Dictionary<string, string> GetCopyOfUsers()
        {
            lock (locker) {
                return new Dictionary<string, string>(users);
            }
        }
    }
}