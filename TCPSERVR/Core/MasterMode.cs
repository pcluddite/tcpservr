// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Tbasic.Errors;

using TCPSERVR.Errors;
using TCPSERVR.Libraries;
using TCPSERVR.Messaging;
using TCPSERVR.Threads;

namespace TCPSERVR.Core
{
    public class MasterMode : ServerMode
    {
        /// <summary>
        /// A list of words that should always use the master application, and not be redirect to a slave
        /// </summary>
        private static readonly string[] MASTERWORDS = new string[] { "USER", "PIPESETUSER", "PIPELISTUSERS", "USINGPIPE" };

        public TcpClient Client { get; private set; } = null;

        public override string ClientString
        {
            get {
                return ((IPEndPoint)Client.Client.RemoteEndPoint).Address.ToString();
            }
        }

        public override string PortString
        {
            get {
                return ((IPEndPoint)Client.Client.RemoteEndPoint).Port.ToString();
            }
        }

        public string CurrentPipeName { get; set; } = null;

        public bool UsingPipe
        {
            get {
                return CurrentPipeName != null;
            }
        }

        public MasterMode(TcpClient client)
        {
            Client = client;
            LoadLibraries();
        }

        protected override void LoadLibraries()
        {
            base.LoadLibraries();
            MainExecuter.Global.AddCommandLibrary(new PipeLibrary(this));
        }

        public override void HandleClient()
        {
            if (Listening)
                throw new InvalidOperationException("Client is already being handled");

            ThreadInfo thread = CurrentThread;
            CurrentPipeName = ServerCore.GlobalPipe;
            
            try {
                Stream stream = Client.GetStream();
                Receiver = new MessageReceiver(stream);

                byte[] data;
                int len;
                while ((len = Receiver.Receive(out data)) != 0) {
                    // If it's not a valid message, maybe it's http
                    if (len < 0) {
                        try {
                            Http http = new Http(Receiver.Writer, Encoding.UTF8.GetString(data), ServerCore.Settings.DownloadFile);
                            http.SendWebpage();
                        }
                        catch {
                            // No harm no foul. Just ignore the message and log the error.
                            ServerCore.ErrorLog.Add(new LoggedError(thread.Client, "An invalid message was received and ignored", "HTTP", dominant: true, fatal: false));
                        }
                        break;
                    }

                    ServerInputMessage input;
                    ServerOutputMessage response;

                    try {
                        input = MessageReceiver.ProcessMessage(data);
                    }
                    catch (JsonException) {
                        response = new ServerOutputMessage(ErrorClient.BadRequest, "The request was poorly formatted", null);
                        Receiver.SendResponse(response);
                        continue;
                    }

                    thread.CreateReport(input);

                    SetTask("Processing " + input.Name);

                    if (!UsingPipe || MASTERWORDS.Contains(input.Name, StringComparer.OrdinalIgnoreCase)) {
                        SetTask("Retrieving response from master application");
                        response = GetResponse(input);
                    }
                    else {
                        SetTask("Retrieving response from slave");
                        try {
                            response = PipeLibrary.PipeUse(CurrentPipeName, input, CurrentThread);
                        }
                        catch (FunctionException ex) {
                            if (ex.Status == ErrorServer.BadGateway) {
                                StringBuilder errorResponse = new StringBuilder();
                                errorResponse.AppendLine("Cannot operate on the current pipe: " + CurrentPipeName);
                                errorResponse.AppendLine("Defaulting to main client");
                                if (CurrentPipeName == ServerCore.GlobalPipe) {
                                    ServerCore.GlobalPipe = null;
                                    errorResponse.AppendLine("New connections will not use this pipe");
                                }
                                CurrentPipeName = null;
                                response = new ServerOutputMessage(ex);
                                response.StatusMessage = errorResponse.ToString();
                            }
                            else {
                                response = new ServerOutputMessage(ex);
                            }
                        }
                    }

                    SendResponse(input, response);
                }
            }
#if !THROW_ERRORS
            catch (Exception ex) {
                SetTask("Failing: " + ex.Message);
                LoggedError error = new LoggedError(thread.Client + ":" + thread.Port, ex, true, false);
                if (!(ex is IOException)) {
                    Program.WriteError(error);
                }
                ServerCore.ErrorLog.Add(error);
            }
#endif
            finally {
                if (Receiver != null) {
                    Receiver.Dispose();
                    Receiver = null;
                }
                thread.End();
            }
        }
    }
}
