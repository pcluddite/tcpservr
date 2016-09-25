using System;
using System.IO;
using System.Threading;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Runtime;
using Tcpservr.Libraries;
using Tcpservr.Messaging;
using Tcpservr.Threads;

namespace Tcpservr.Core
{
    public abstract class ServerMode
    {
        public TRuntime MainExecuter { get; protected set; } = new TRuntime();
        public MessageReceiver Receiver { get; protected set; } = null;

        public virtual bool Listening
        {
            get {
                return Receiver != null;
            }
        }

        public abstract string ClientString { get; }
        public abstract string PortString { get; }

        public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();

        public ThreadInfo CurrentThread
        {
            get {
                return ServerCore.Threads[Thread.CurrentThread.ManagedThreadId];
            }
        }

        protected internal void SetTask(string task)
        {
            if (CurrentThread != null) {
                CurrentThread.SetTask(task);
            }
        }

        protected virtual void LoadLibraries()
        {
            MainExecuter.Global.LoadStandardLibrary();
            MainExecuter.Global.AddLibrary(new ThreadLibrary(this));
            MainExecuter.Global.AddCommandLibrary(new ServerLibrary(this));
            MainExecuter.Global.AddCommandLibrary(new FileIOLibServer(this));
            MainExecuter.Global.AddCommandLibrary(new UserIOLibServer(this));
        }

        /// <summary>
        /// Gets the absolute path based on the directory this thread is using
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetRealPath(string path)
        {
            if (path == "\\" || path == "/") { // both separators are used on Windows, so check for them. Breaks UNIX compatability though.
                path = Path.GetPathRoot(CurrentDirectory);
            }
            else if (path.StartsWith(Path.DirectorySeparatorChar.ToString())) {
                path = Path.Combine(Path.GetPathRoot(CurrentDirectory), path.Substring(1));
            }
            if (!Path.IsPathRooted(path)) {
                path = Path.Combine(CurrentDirectory, path);
            }
            return Path.GetFullPath(path);
        }

        public abstract void HandleClient();

        public virtual void SendResponse(ServerOutputMessage response) // this reconstructs the message from scratch in order to find the id, so it should be avoided 6/10/16
        {
            SendResponse(MessageReceiver.ProcessMessage(Receiver.LastReceived), response);
        }

        public virtual void SendResponse(ServerInputMessage input, ServerOutputMessage response)
        {
            SetTask("Sending response");
            response.ID = input.ID; // set the ids the same
            Receiver.SendResponse(response);

            SetTask("Waiting for message");
            CurrentThread.CompleteReport(input);

            if (CurrentThread.ForceGarbageCollection) {
                GC.Collect();
                CurrentThread.ForceGarbageCollection = false;
            }
        }

        protected int currentLine = 0;

        public virtual ServerOutputMessage GetResponse(ServerInputMessage msg)
        {
            ServerOutputMessage response;
            try {
                StackData stackdat = MainExecuter.Execute(new Line(currentLine++, msg.Message));
                response = new ServerOutputMessage(stackdat.Status, stackdat.ReturnValue);
            }
            catch (Exception ex) {
                response = ProcessException(ex);
            }
            return response;
        }

        protected virtual ServerOutputMessage ProcessException(Exception ex)
        {
            CurrentThread.SetTask("Processing exception");
            FunctionException wrapped = FunctionException.WrapException(ex) as FunctionException;
            if (wrapped == null) {
                return new ServerOutputMessage(ErrorServer.GenericError, ex.Message, null); // we don't know specfics, send back a generic error
            }
            else {
                return new ServerOutputMessage(wrapped);
            }
        }
    }
}
