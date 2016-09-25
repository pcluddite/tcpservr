using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;
using Tcpservr.Components.Win32;
using Tcpservr.Core;
using Tcpservr.Messaging;
using Tcpservr.Threads;

namespace Tcpservr.Libraries
{
    public class PipeLibrary : Library
    {
        private MasterMode core;

        public PipeLibrary(MasterMode core)
        {
            this.core = core;
            Add("PipeListAll", PipeListAll);
            Add("PipeExists", PipeExists);
            Add("PipeUse", PipeUse);
            Add("PipeCreate", PipeCreate);
            Add("PipeListUsers", PipeListUsers);
            Add("User", PipeSetUser);
        }

        public object PipeSetUser(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(false);
            }
            stackdat.AssertCount(3);

            var users = MasterPipe.GetCopyOfUsers();

            string spawnerpipe;
            if (!users.TryGetValue(stackdat.Get<string>(1), out spawnerpipe)) {
                throw new FunctionException(ErrorClient.NotFound, "User");
            }

            string username = stackdat.Get<string>(1);
            bool is_global = stackdat.Get<bool>(2);

            string userPipeName = MasterPipe.PIPENAME + '_' + username.Substring(username.LastIndexOf('\\') + 1); // gets the username for this pipe and prepends TCPSERVR
            string responseMsg;

            if (!PipeExists(userPipeName)) {
                StackData createMsg = new StackData(runtime.Options, stackdat.Parameters);
                createMsg.AddRange("PipeCreate", spawnerpipe, userPipeName);

                PipeCreate(runtime, createMsg);
                
                responseMsg = "The current pipe is set to a new pipe '" + userPipeName + "'";
            }
            else {
                responseMsg = "The current pipe is set to an existing pipe '" + userPipeName + "'";
            }

            core.CurrentPipeName = userPipeName;
            
            if (is_global) {
                ServerCore.GlobalPipe = userPipeName;
            }

            throw new FunctionException(ErrorSuccess.OK, responseMsg, prependGeneric: false);
        }

        public object PipeCreate(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);

            string pipename = stackdat.Get<string>(1);
            if (!PipeExists(pipename)) {
                throw new FunctionException(ErrorClient.NotFound, "Pipe");
            }

            ServerOutputMessage response = PipeUse(pipename, new ServerInputMessage("OPEN \"" + stackdat.Get<string>(2) + "\""), core.CurrentThread);
            
            stackdat.Status = response.Status;
            return response.Response;
        }

        public object PipeListUsers(TRuntime runtime, StackData stackdata)
        {
            stackdata.AssertCount(1);
            return ListConnectedUsers();
        }
        
        public static string[] ListConnectedUsers()
        {
            var dict_users = MasterPipe.GetCopyOfUsers();
            string[] arr_users = new string[dict_users.Count];

            int index = 0;
            foreach(var kv in dict_users) {
                arr_users[index++] = string.Format("{0}|{1}|{2}", kv.Key, kv.Value, GetPipeStatus(kv.Value));
            }

            return arr_users;
        }

        private static string GetPipeStatus(string pipename)
        {
            return PipeExists(pipename) ? "Alive" : "Broken";
        }

        private static IEnumerable<string> GetPipes()
        {
            return Kernel32.FindFiles(@"\\.\pipe\*");
        }
        
        public object PipeListAll(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            string[] pipes = GetPipes().ToArray();
            if (pipes.Length > 0) {
                return pipes;
            }
            else {
                stackdat.Status = ErrorSuccess.NoContent;
                return null;
            }
        }

        public object PipeUse(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            ServerOutputMessage response = PipeUse(stackdat.Get<string>(1), new ServerInputMessage(stackdat.Get<string>(2)), core.CurrentThread);
            stackdat.Status = response.Status;
            return response.Response;
        }

        public static ServerOutputMessage PipeUse(string pipe, ServerInputMessage stackdat, ThreadInfo currentThread = null)
        {
            ServerOutputMessage response;
            try {
                if (!PipeExists(pipe)) {
                    throw new FunctionException(ErrorServer.BadGateway, "Pipe does not exist");
                }

                int id = Thread.CurrentThread.ManagedThreadId;
                currentThread?.SetTask("Connecting to '" + pipe + "'");

                using (NamedPipeClientStream client = new NamedPipeClientStream(pipe)) {
                    client.Connect(5000);
                    client.ReadMode = PipeTransmissionMode.Message;

                    currentThread?.SetTask("Writing to '" + pipe + "'");

                    MessageReceiver receiver = new MessageReceiver(client);
                    receiver.SendMessage(stackdat);

                    byte[] data;
                    currentThread?.SetTask("Reading from '" + pipe + "'");
                    int len = receiver.Receive(out data);
                    if (len == -1) {
                        throw new FunctionException(ErrorServer.BadGateway, "An invalid message was received from the pipe and cannot be processed.", prependGeneric: false);
                    }
                    response = MessageReceiver.ProcessResponse(data);
                    if (!ErrorSuccess.IsSuccess(response.Status)) {
                        throw new FunctionException(response.Status, response.StatusMessage, prependGeneric: false);
                    }
                    return response;
                }
            }
            catch (FunctionException) {
                throw; // send it up the ladder
            }
            catch (IOException ex) {
                throw new FunctionException(ErrorServer.BadGateway, ex.Message, ex);
            }
            catch (Exception ex) {
                throw new FunctionException(ErrorServer.GenericError, ex.Message, ex);
            }
        }

        public static bool PipeExists(string pipe)
        {
            foreach (string s in GetPipes()) {
                if (s == pipe) {
                    return true;
                }
            }
            return false;
        }

        public object PipeExists(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return PipeExists(stackdat.Get<string>(1));
        }
    }
}
