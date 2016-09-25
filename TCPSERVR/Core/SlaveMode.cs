using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using Tbasic.Errors;
using Tcpservr.Errors;
using Tcpservr.Libraries;
using Tcpservr.Messaging;
using Tcpservr.Threads;

namespace Tcpservr.Core
{
    public class SlaveMode : ServerMode
    {
        public static ConcurrentBag<string> Pipes { get; private set; } = new ConcurrentBag<string>();

        public bool BreakRequest { get; set; } = false;

        public NamedPipeServerStream Server { get; private set; } = null;

        public override string ClientString
        {
            get {
                return PipeName;
            }
        }

        public override string PortString
        {
            get {
                return "--";
            }
        }

        public string PipeName { get; private set; }

        public SlaveMode(string pipeName)
        {
            PipeName = pipeName;
            LoadLibraries();
        }

        protected override void LoadLibraries()
        {
            base.LoadLibraries();
            MainExecuter.Global.AddCommandLibrary(new SlaveLibrary(this));
        }

        public override void HandleClient()
        {
            ThreadInfo thread = CurrentThread;
            Pipes.Add(PipeName);
            try {
                while (!BreakRequest) {
                    while (PipeLibrary.PipeExists(PipeName)) {
                        Thread.Sleep(10);
                    }
                    using (NamedPipeServerStream server = new NamedPipeServerStream(PipeName,
                        PipeDirection.InOut, 254, PipeTransmissionMode.Message, PipeOptions.None, 4096, 4096, ServerCore.GetGlobalAccessLevel())) {
                        server.WaitForConnection();
                        Server = server;

                        Receiver = new MessageReceiver(server, dispose: false);

                        byte[] data;
                        int len = Receiver.Receive(out data);
                        if (len == -1) {
                            Program.WriteError(new LoggedError(PipeName, "An invalid message was received", "receiver.PRead", dominant: false, fatal: false));
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
                            Receiver.SendResponse(response);
                            continue;
                        }

                        thread.CreateReport(input);
                        
                        thread.SetTask("Processing " + input.Name.ToUpper());

                        try {
                            response = GetResponse(input);
                        }
                        catch(FunctionException ex) {
                            response = new ServerOutputMessage(ex.Status, ex.Message, null);
                        }
                        catch (Exception ex) {
                            response = new ServerOutputMessage(ErrorServer.GenericError, ex.Message, null);
                        }

                        SendResponse(input, response);
                    }
                }
            }
            catch (Exception ex) {
                Program.WriteError(new LoggedError(PipeName, ex, dominant: false, fatal: false));
            }
            finally {
                if (Receiver != null) {
                    Receiver = null;
                }
                Server = null;
                thread.End();
            }
        }

        public override void SendResponse(ServerInputMessage input, ServerOutputMessage response)
        {
            SetTask("Sending response");
            Receiver.SendResponse(response);
            SetTask("Waiting for pipe drain");
            Server.WaitForPipeDrain();
            Server.Disconnect();
            SetTask("Waiting for data");
            CurrentThread.CompleteReport(input);
        }
    }
}