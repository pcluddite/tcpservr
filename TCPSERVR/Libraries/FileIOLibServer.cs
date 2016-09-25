// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Tbasic.Libraries;
using Tbasic.Runtime;
using Tcpservr.Core;
using Tcpservr.Messaging;

namespace Tcpservr.Libraries
{
    public class FileIOLibServer : Library
    {
        private ServerMode core;

        public FileIOLibServer(ServerMode tcpservr)
        {
            core = tcpservr;
            Add("CD", Cd);
            Add("PWD", Pwd);
            Add("RECYCLE", Recycle);
            // these commands assume Windows. Portability may be considered later 6/6/16
            Add("DIR", Dir);
            Add("LS", Dir);
            Add("MOVE", Mv);
            Add("MV", Mv);
            Add("COPY", Cp);
            Add("CP", Cp);
            Add("DEL", Rm);
            Add("RM", Rm);
            Add("REN", Ren);
            Add("MKDIR", Mkdir);
            Add("MD", Mkdir);
        }

        public object Cd(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount != 2 && stackdat.ParameterCount != 1) {
                stackdat.AssertCount(2);
            }
            else if (stackdat.ParameterCount == 2) {
                CommandLine line = new CommandLine(stackdat.Parameters); // parse it as a command
                core.CurrentDirectory = core.GetRealPath(line[1]);
            }
            return core.CurrentDirectory;
        }

        public object Pwd(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return core.CurrentDirectory;
        }

        public object Recycle(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            FileIOLibrary.Recycle(stackdat.Get<string>(1));
            return null;
        }

        public object Dir(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("DIR", stackdat);
        }

        public object Mv(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("MOVE", stackdat);
        }

        public object Cp(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("COPY", stackdat);
        }

        public object Rm(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("DEL", stackdat);
        }

        public object Mkdir(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("MD", stackdat);
        }

        public object Rd(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("RD", stackdat);
        }

        public object Ren(TRuntime runtime, StackData stackdat)
        {
            return RunCmdCommand("REN", stackdat);
        }

        private string RunCmdCommand(string cmd, StackData stackdat)
        {
            CommandLine line = new CommandLine(stackdat.Parameters);
            line[0] = cmd;
            return Shell(line.ToString());
        }

        /// <summary>
        /// Redirects standard output and ignores the command's exit code
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal string Shell(string cmd)
        {
            string output;
            int error = FileIOLibrary.Shell(cmd, core.CurrentDirectory, out output);
            /*if (error != 0) {
                throw new FunctionException(ErrorServer.BadGateway, "Redirected standard output returned error code: " + error + Environment.NewLine +
                                                                  output, prependGeneric: false);
            }*/
            return output;
        }
    }
}
