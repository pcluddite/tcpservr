using System;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;
using Tcpservr.Core;
using Tcpservr.Messaging;

namespace Tcpservr.Libraries
{
    public class SlaveLibrary : Library
    {
        private SlaveMode slave;

        public SlaveLibrary(SlaveMode slave)
        {
            this.slave = slave;
            Add("StopPipe", Break);
            Add("PipeListChildren", GetChildPipes);
            Add("StopChild", EndChild);
        }

        public object Break(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            slave.BreakRequest = true;
            throw new FunctionException(ErrorSuccess.Accepted, "Pipe is being broken. Do not reconnect.");
        }

        public static object GetChildPipes(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            stackdat.Status = ErrorSuccess.OK;
            return SlaveMode.Pipes.ToArray();
        }

        public object EndChild(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            ServerOutputMessage response = new ServerOutputMessage(ErrorSuccess.Accepted, "Ending child instance of the application.", null);
            slave.SendResponse(response);
            slave.Server.Disconnect();
            Environment.Exit(ErrorSuccess.OK);
            return null;
        }
    }
}
