using System;
using System.Threading;
using Tcpservr.Messaging;
using Tcpservr.Core;
using Tcpservr.Components;

namespace Tcpservr.Threads
{
    public class ThreadInfo
    {
        public bool LogHistory { get; set; }

        private WeakReference master;

        /// <summary>
        /// A weak reference to the ServerMode. Returns null if master has been collected.
        /// </summary>
        public ServerMode Mode {
            get {
                return master.IsAlive ? (ServerMode)master.Target : null;
            }
            private set {
                master.Target = value;
            }
        }

        public ThreadInfo(ServerMode mode, bool logHistory)
        {
            master = new WeakReference(mode);
            EndTime = DateTime.MaxValue;
            Thread = new Thread(mode.HandleClient);
            Client = mode.ClientString;
            Port = mode.PortString;
            Reports = new ReportCollection();
            report = new Report("Generic");
            LogHistory = logHistory;
        }

        public ReportCollection Reports { get; private set; }
        public Thread Thread { get; private set; }

        public string Client { get; private set; }
        public string Port { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public bool ForceGarbageCollection { get; set; }

        public string CurrentTask
        {
            get {
                return report.CurrentTask;
            }
        }

        public ThreadState ThreadState
        {
            get {
                return Thread.ThreadState;
            }
        }

        public int ID
        {
            get {
                return Thread.ManagedThreadId;
            }
        }

        private Report report;

        public void Start()
        {
            SetTask("Initializing");
            StartTime = DateTime.Now;
            Thread.Start();
        }

        public void SetTask(string task)
        {
            report.SetTask(task);
        }

        public void CreateReport(ServerInputMessage tMsg)
        {
            report = new Report(tMsg.Name.ToUpper());
            if (LogHistory) {
                Reports.AddReport(tMsg, report);
            }
        }

        public void CompleteReport(ServerInputMessage tMsg)
        {
            tMsg.Message = tMsg.Message.Clip(100);
            report.Complete();
            report = new Report("Generic");
        }

        public void Abort()
        {
            SetTask("Aborting");
            SetEndTimeToNow();
            Thread.Abort();
        }

        public void End()
        {
            SetTask("Ending");
            SetEndTimeToNow();
        }

        private void SetEndTimeToNow()
        {
            EndTime = DateTime.Now;
        }
    }
}