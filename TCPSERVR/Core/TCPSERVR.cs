using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Runtime;
using Tcpservr.Components;
using Tcpservr.Errors;
using Tcpservr.Libraries;
using Tcpservr.Messaging;
using Tcpservr.Threads;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tcpservr
{
    public class TCPSERVR
    {
        private static readonly string[] MASTERWORDS = new string[] { "USER", "PIPESETUSER", "PIPELISTUSERS" };

        public string ApplicationDirectory
        {
            get {
                return Program.ApplicationPath;
            }
        }

        public string ExecutablePath
        {
            get {
                return Application.ExecutablePath;
            }
        }

        public Executer MainExecuter { get; set; } = new Executer();

        public ThreadCollection Threads { get; set; } = new ThreadCollection();
        public LoggedErrorCollection ErrorLog { get; set; } = new LoggedErrorCollection(10);
        public ServerSettings Settings { get; private set; } = new ServerSettings();
        
        public ThreadInfo CurrentThread
        {
            get {
                ThreadInfo thread;
                if (!Threads.TryGetValue(Thread.CurrentThread.ManagedThreadId, out thread)) {
                    return null;
                }
                return thread;
            }
        }

        public string CurrentDirectory
        {
            get {
                return CurrentThread.CurrentDirectory;
            }
            set {
                CurrentThread.CurrentDirectory = value;
            }
        }
                
        protected internal void SetTask(string task)
        {
            if (CurrentThread != null) {
                CurrentThread.SetTask(task);
            }
        }

        public string GetRealPath(string path)
        {
            if (path.Equals(Path.DirectorySeparatorChar.ToString())) {
                path = Path.GetPathRoot(CurrentThread.CurrentDirectory);
            }
            else if (path.StartsWith(Path.DirectorySeparatorChar.ToString())) {
                path = Path.Combine(Path.GetPathRoot(CurrentThread.CurrentDirectory), path.Remove(0, 1));
            }
            if (!Path.IsPathRooted(path)) {
                path = Path.Combine(CurrentThread.CurrentDirectory, path);
            }
            return Path.GetFullPath(path);
        }

        public TcpListener listener;

        public string PrivateKey;

        public bool IsSecure
        {
            get {
                return PrivateKey != null;
            }
        }
        
        public MasterPipe MasterPipe { get; private set; }

        public bool IsDominant { get; set; }
        
        public string GlobalPipe = "";
        public bool UsingGlobalPipe = false;

        public TCPSERVR(bool dominant)
        {
            IsDominant = dominant;
            Directory.SetCurrentDirectory(ApplicationDirectory);
        }

        public void Start()
        {
            MainExecuter.Global.LoadStandardLibrary();
            MainExecuter.Global.AddCommandLibrary(new PipeLibrary(this));
            MainExecuter.Global.AddCommandLibrary(new ServerLibrary(this));
            MainExecuter.Global.AddCommandLibrary(new FileIOLibServer(this));
            MainExecuter.Global.AddCommandLibrary(new UserIOLibServer(this));
            MasterPipe = new MasterPipe(this);

            listener = new TcpListener(Settings.EndpointIP);
            PrivateKey = Settings.GetPrivateKey();

            Thread cleaner = new Thread(new ThreadStart(Program.CleanResources));
            cleaner.Start();

            listener.Start();
            MasterPipe.BeginStart();

            while (true) {
                TcpClient client = null;
                try {
                    client = listener.AcceptTcpClient();
                }
                catch (SocketException ex) {
                    if (ex.SocketErrorCode != SocketError.Interrupted) // interupted sockets can be ignored
                        throw;
                }
                ThreadInfo tInfo = new ThreadInfo(client.Client, Settings.LogHistory, new Thread(HandleClient));
                Threads.Add(tInfo);
                tInfo.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            ThreadInfo thread = CurrentThread;
            TcpSecure security = null;
            thread.CurrentPipeName = GlobalPipe;
            thread.UsingPipe = UsingGlobalPipe;
#if !THROW_ERRORS
            try
#endif
            {
                Stream stream = ((TcpClient)client).GetStream();

                MessageReceiver receiver;
                receiver = new MessageReceiver(stream);

                byte[] data;
                int len;
                while ((len = receiver.Receive(out data)) != 0) {
                    // If it's not a valid message, maybe it's http
                    if (len < 0) {
                        Http http = new Http(receiver.Writer, Encoding.UTF8.GetString(data), Settings.DownloadFile);
                        http.SendWebpage();
                        break;
                    }

                    ServerInputMessage input;
                    ServerOutputMessage response;

                    try {
                        input = MessageReceiver.ProcessMessage(data);
                    }
                    catch (JsonException) {
                        response = new ServerOutputMessage(ErrorClient.BadRequest, "The request was poorly formatted", null);
                        receiver.SendResponse(response);
                        continue;
                    }

                    thread.CreateReport(input);

                    TFunctionData tMsg = input.ToTFunctionData(MainExecuter);

                    SetTask("Processing " + tMsg.Name);

                    try {
                        switch (tMsg.Name.ToUpper()) {
                            #region USINGPIPE
                            case "USINGPIPE":
                                if (tMsg.Count == 1) {
                                    response = new ServerOutputMessage(ErrorSuccess.OK, (thread.UsingPipe + " " + thread.CurrentPipeName).Trim());
                                    goto Write;
                                }
                                if (tMsg.Count == 2) {
                                    tMsg.Add(false);
                                }
                                tMsg.AssertArgs(3);
                                thread.CurrentPipeName = tMsg.Get<string>(1);
                                if (tMsg.Get<bool>(2)) {
                                    GlobalPipe = thread.CurrentPipeName;
                                    UsingGlobalPipe = !GlobalPipe.Equals("");
                                }
                                if (tMsg.Get<string>(1) == "") {
                                    thread.UsingPipe = false;
                                    response = new ServerOutputMessage(ErrorSuccess.OK, "New wessages will not be piped", null);
                                }
                                else {
                                    thread.UsingPipe = true;
                                    response = new ServerOutputMessage(ErrorSuccess.Accepted, "New messages will be sent through '" + tMsg.Get<string>(1) + "'");
                                }
                                goto Write;
                            #endregion
                            #region END
                            case "END":
                                response = new ServerOutputMessage(ErrorSuccess.Accepted, "Server will now exit. Do not reconnect.", null);
                                response.ID = input.ID;
                                receiver.SendResponse(response);
                                receiver.Dispose();
                                listener.Stop();
                                Program.CleanResources();
                                Environment.Exit(ErrorSuccess.OK);
                                break;
                            #endregion
                            #region RESTART
                            case "RESTART":
                                Program.CleanResources();
                                if (File.Exists(Path.Combine(ApplicationDirectory, "TCPSERVR2.EXE"))) {
                                    try {
                                        File.Delete(Path.Combine(ApplicationDirectory, "TCPSERVR2.EXE"));
                                    }
                                    catch (IOException ex) {
                                        response = new ServerOutputMessage(ErrorServer.GenericError, ex.Message, null);
                                        goto Write;
                                    }
                                }
                                File.Copy(ExecutablePath, Path.Combine(ApplicationDirectory, "TCPSERVR2.EXE"));
                                TFunctionData restart = new TFunctionData(MainExecuter, "UPDATE");
                                ServerLibrary updater = new ServerLibrary(this);
                                updater.Update(restart);
                                if (!ErrorSuccess.IsSuccess(restart.Status)) { // if not success
                                    response = new ServerOutputMessage(ErrorClient.Conflict, null);
                                    goto Write;
                                }
                                response = new ServerOutputMessage(ErrorSuccess.Accepted, "Attempting to restart. Please wait a few minutes to reconnect.");
                                response.ID = input.ID;
                                receiver.SendResponse(response);
                                receiver.Dispose();
                                listener.Stop();
                                Environment.Exit(ErrorSuccess.OK);
                                break;
                                #endregion
                        }
                    }
                    catch (Exception ex) {
                        response = ProcessException(ex);
                        goto Write;
                    }

                    if (!thread.UsingPipe || MASTERWORDS.Contains(tMsg.Name)) {
                        SetTask("Retrieving response from master application");
                        response = GetResponse(input);
                    }
                    else {
                        SetTask("Retrieving response from slave");
                        try {
                            response = PipeLibrary.PipeUse(thread.CurrentPipeName, input, CurrentThread);
                        }
                        catch (TbasicException ex) {
                            if (ex.Status == ErrorServer.BadGateway) {
                                StringBuilder errorResponse = new StringBuilder();
                                errorResponse.AppendLine("Cannot operate on the current pipe: " + thread.CurrentPipeName);
                                errorResponse.AppendLine("Defaulting to main client");
                                thread.UsingPipe = false;
                                if (UsingGlobalPipe && thread.CurrentPipeName.Equals(GlobalPipe)) {
                                    UsingGlobalPipe = false;
                                    errorResponse.AppendLine("New connections will not use this pipe");
                                }
                                response = new ServerOutputMessage(ErrorServer.BadGateway, ex.Message, errorResponse.ToString());
                            }
                            else {
                                response = new ServerOutputMessage(ex);
                            }
                        }
                    }

                Write: // I know, labels are a sin (send me to hell) 5/29/16
                    SetTask("Sending response");
                    response.ID = input.ID; // set the ids the same
                    receiver.SendResponse(response);

                    SetTask("Waiting for message");
                    thread.CompleteReport(input);

                    if (thread.ForceGarbageCollection) {
                        GC.Collect();
                        thread.ForceGarbageCollection = false;
                    }
                }
                receiver.Dispose();
            }
#if !THROW_ERRORS
            catch (Exception ex) {
                SetTask("Failing: " + ex.Message);
                LoggedError error = new LoggedError(CurrentThread.Client + ":" + CurrentThread.Port, ex, true, false);
                if (!(ex is IOException)) {
                    Program.WriteError(error);
                }
                ErrorLog.Add(error);
            }
            finally
#endif
            {
                CurrentThread.End();
                if (security != null) {
                    security.Dispose();
                }
            }
        }

        private static uint currentLine = 0;

        public ServerOutputMessage GetResponse(ServerInputMessage msg)
        {
            ServerOutputMessage response;
            try {
                TFunctionData data = MainExecuter.Execute(new Line(unchecked(currentLine++), msg.Message));
                response = new ServerOutputMessage(data.Status, data.Data);
            }
            catch (Exception ex) {
                response = ProcessException(ex);
            }
            return response;
        }

        private ServerOutputMessage ProcessException(Exception ex)
        {
            CurrentThread.SetTask("Processing exception");
            TbasicException wrapped = TbasicException.WrapException(ex) as TbasicException;
            if (wrapped == null) {
                return new ServerOutputMessage(ErrorServer.GenericError, ex.Message, null);
            }
            else {
                return new ServerOutputMessage(wrapped);
            }
        }
    }
}