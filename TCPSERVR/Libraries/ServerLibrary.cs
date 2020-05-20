// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;

using TCPSERVR.Core;
using TCPSERVR.Messaging;

namespace TCPSERVR.Libraries
{
    internal class ServerLibrary : Library
    {
        private ServerMode clientHandler;

        public ServerLibrary(ServerMode core)
        {
            clientHandler = core;
            Add("Update", Update);
            Add("Ver", Ver);
            Add("Hello", Hello);
            Add("Fuck", Fuck);
            Add("UsingPipe", UsingPipe);
            Add("Restart", Restart);
            Add("Stop", End);
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private void UnloadModule(string moduleName)
        {
            foreach (ProcessModule mod in Process.GetCurrentProcess().Modules) {
                if (mod.ModuleName == moduleName) {
                    FreeLibrary(mod.BaseAddress);
                }
            }
        }

        public static object Ver(TRuntime runtime, StackData stackdat)
        {
            return Program.VER;
        }

        public static object Hello(TRuntime runtime, StackData stackdat)
        {
            return "Hello.";
        }

        public static object Fuck(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount > 1 && stackdat.Get(1).ToString().Equals("you", StringComparison.CurrentCultureIgnoreCase)) {
                throw new FunctionException(ErrorClient.Conflict, "Between you and me");
            }
            else {
                throw new FunctionException(ErrorClient.BadRequest, "Requests must be formatted politely");
            }
        }

        public static object Update(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 1) {
                stackdat.Add(Path.Combine(ServerCore.ApplicationDirectory, "TCPSERVR2.EXE"));
            }
            stackdat.AssertCount(2);
            
            string fileName = Path.Combine(ServerCore.ApplicationDirectory, "UPDATER.EXE");
            //File.WriteAllBytes(fileName, HelperApplication);           /// TODO: Add helper application
            CommandLine args = new CommandLine("UPDATE", ServerCore.ExecutablePath, stackdat.Get<string>(1));
            using (Process p = new Process()) {
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = args.ToString();
                p.Start();
            }
            stackdat.Status = ErrorSuccess.Accepted;
            return "Update is being prepared. End the server and wait a few minutes for the update to complete.";
        }

        public object UsingPipe(TRuntime runtime, StackData stackdat)
        {
            MasterMode master = clientHandler as MasterMode;
            if (clientHandler == null)
                throw new NotImplementedException();

            if (stackdat.ParameterCount == 1) {
                return (master.UsingPipe + " " + master.CurrentPipeName).Trim();
            }
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(false);
            }
            stackdat.AssertCount(3);
            master.CurrentPipeName = stackdat.Get<string>(1);
            if (stackdat.Get<bool>(2)) {
                if (string.IsNullOrEmpty(master.CurrentPipeName)) {
                    ServerCore.GlobalPipe = null;
                }
                else {
                    ServerCore.GlobalPipe = master.CurrentPipeName;
                }
            }
            if (stackdat.Get<string>(1) == "") {
                master.CurrentPipeName = null;
                throw new FunctionException(ErrorSuccess.OK, "New wessages will not be piped"); // only way to set status message right now 6/7/16
            }
            else {
                master.CurrentPipeName = null;
                throw new FunctionException(ErrorSuccess.Accepted, "New messages will be sent through '" + stackdat.Get<string>(1) + "'");
            }
        }

        public object End(TRuntime runtime, StackData stackdat)
        {
            ServerOutputMessage response = new ServerOutputMessage(ErrorSuccess.Accepted, "Server will now exit. Do not reconnect.", null);
            clientHandler.SendResponse(response);
            clientHandler.Receiver.Dispose();
            ServerCore.StopMaster();
            Program.CleanResources();
            Environment.Exit(ErrorSuccess.OK);
            return null; // just satisfying the compiler
        }

        public object Restart(TRuntime runtime, StackData stackdat)
        {
            Program.CleanResources();
            if (File.Exists(Path.Combine(ServerCore.ApplicationDirectory, "TCPSERVR2.EXE"))) {
                try {
                    File.Delete(Path.Combine(ServerCore.ApplicationDirectory, "TCPSERVR2.EXE"));
                }
                catch (IOException ex) {
                    throw new FunctionException(ErrorServer.GenericError, ex.Message, ex);
                }
            }
            File.Copy(ServerCore.ExecutablePath, Path.Combine(ServerCore.ApplicationDirectory, "TCPSERVR2.EXE"));
            StackData restart = new StackData(runtime.Options);
            restart.Add("UPDATE");
            Update(runtime, restart);
            if (!ErrorSuccess.IsSuccess(restart.Status)) { // if not success
                throw new FunctionException(ErrorClient.Conflict);
            }
            ServerOutputMessage response = new ServerOutputMessage(ErrorSuccess.Accepted, "Attempting to restart. Please wait a few minutes to reconnect.");
            clientHandler.Receiver.SendResponse(response);
            clientHandler.Receiver.Dispose();
            ServerCore.StopMaster();
            Environment.Exit(ErrorSuccess.OK);
            return null;
        }
    }
}