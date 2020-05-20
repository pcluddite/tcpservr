// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Text;
using System.Threading;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;
using TCPSERVR.Components;
using TCPSERVR.Core;
using TCPSERVR.Threads;

namespace TCPSERVR.Libraries
{
    public class ThreadLibrary : Library
    {
        private ServerMode core;

        public ThreadLibrary(ServerMode tcpservr)
        {
            core = tcpservr;
            Add("threadlist", ThreadList);
            Add("threadlistclean", ThreadListClean);
            Add("threadabort", ThreadAbort);
            Add("threadstate", ThreadState);
            Add("threaddelete", ThreadDelete);
            Add("processthreadlist", ProcessThreadList);
            Add("history", History);
            Add("getreport", GetReport);
            Add("clearhistory", ClearHistory);
            Add("historylog", HistoryLog);
        }

        public object HistoryLog(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(core.CurrentThread.ID);
            }
            if (stackdat.ParameterCount == 3) {
                stackdat.Add("CURRENT");
            }
            stackdat.AssertCount(4);
            int id = stackdat.Get<int>(2);
            ThreadInfo thread = ServerCore.Threads[id];
            bool threadLog = stackdat.Get<bool>(2);
            bool globalLog = stackdat.GetEnum(3, "'ALL' or 'CURRENT'", "ALL", "CURRENT").Equals("ALL", StringComparison.OrdinalIgnoreCase);
            thread.LogHistory = threadLog;
            if (globalLog) {
                ServerCore.Settings.LogHistory = true;
            }
            return null;
        }

        public object History(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 1) {
                stackdat.Add(Thread.CurrentThread.ManagedThreadId);
            }
            stackdat.AssertCount(2);
            int id = stackdat.Get<int>(1);
            ThreadInfo thread = ServerCore.Threads[id];
            if (!thread.LogHistory) {
                stackdat.Status = ErrorSuccess.NoContent;
                return null;
            }
            StringBuilder history = new StringBuilder('|' + ("Message History for Thread " + thread.ID).Center(70));
            history.AppendFormat("\n{0,-8}{1,-40}{2,-20}", "[ ID ]", "[ Message ]", "[ Received ]");
            foreach (var v in thread.Reports) {
                history.AppendFormat("\n{0,6}  {1,-40}{2,-20}", v.Key.ID, v.Key.Message, v.Key.CreationTime.ToString("HH:mm:ss MM/dd/yyyy"));
            }
            return history.ToString();
        }

        public static object ClearHistory(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 1) {
                stackdat.Add(Thread.CurrentThread.ManagedThreadId);
            }
            stackdat.AssertCount(2);
            int id = stackdat.Get<int>(1);
            ThreadInfo thread = ServerCore.Threads[id];
            thread.Reports.Clear();
            return null;
        }

        public static object GetReport(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(Thread.CurrentThread.ManagedThreadId);
            }
            stackdat.AssertCount(3);
            ThreadInfo thread = ServerCore.Threads[stackdat.Get<int>(2)];
            Report report = thread.Reports.GetReportFromId(stackdat.Get<int>(1));
            return report.ToString();
        }

        public static object ProcessThreadList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return ThreadCollection.GetProcessThreadList();
        }

        public static object ThreadList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return ServerCore.Threads.GetManagedThreadList();
        }

        public static object ThreadDelete(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            int id = stackdat.Get<int>(1);
            ThreadInfo thread = ServerCore.Threads[id];
            if (thread.ThreadState != System.Threading.ThreadState.Aborted &&
                thread.ThreadState != System.Threading.ThreadState.Stopped) {
                throw new FunctionException(ErrorClient.Conflict, "Cannot delete thread because thread is running");
            }
            ServerCore.Threads.Remove(id);
            return null;
        }

        public static object ThreadListClean(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            ServerCore.Threads.Clean();
            return null;
        }

        public static object ThreadAbort(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            int id = stackdat.Get<int>(1);
            ServerCore.Threads[id].Abort();
            throw new FunctionException(ErrorSuccess.Accepted, "Terminating thread " + id);
        }

        public static object ThreadState(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            int id = stackdat.Get<int>(1);
            return ServerCore.Threads[id].CurrentTask;
        }
    }
}