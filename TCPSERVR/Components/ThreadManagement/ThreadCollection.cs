// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tbasic.Errors;

namespace TCPSERVR.Threads
{
    /// <summary>
    /// Manages a collection of threads. This class should be thread-safe.
    /// </summary>
    public class ThreadCollection : IDictionary<int, ThreadInfo>, IEnumerable<KeyValuePair<int, ThreadInfo>>
    {
        private ConcurrentDictionary<int, ThreadInfo> threads = new ConcurrentDictionary<int, ThreadInfo>();
        private object locker = new object();

        public ICollection<int> Keys
        {
            get {
                return threads.Keys;
            }
        }

        public ICollection<ThreadInfo> Values
        {
            get {
                return threads.Values;
            }
        }

        public int Count
        {
            get {
                return threads.Count;
            }
        }

        public ThreadInfo this[int key]
        {
            get {
                return GetThread(key);
            }
            set {
                threads[key] = value;
            }
        }

        public void Add(ThreadInfo thread)
        {
            if (!threads.TryAdd(thread.Thread.ManagedThreadId, thread))
                throw new FunctionException(ErrorClient.Conflict, "Unable to add thread to collection");
        }

        public ThreadInfo GetThread(int id)
        {
            ThreadInfo info;
            if (!threads.TryGetValue(id, out info)) {
                throw new FunctionException(ErrorClient.NotFound, "Thread");
            }
            return info;
        }

        public void Remove(int id)
        {
            ThreadInfo info;
            if (!threads.TryGetValue(id, out info))
                throw new FunctionException(ErrorClient.NotFound, "Thread");

            if (!threads.TryRemove(id, out info))
                throw new FunctionException(ErrorClient.Conflict, "Unable to remove thread from collection");
        }

        public bool Contains(int id)
        {
            return threads.ContainsKey(id);
        }

        public string GetManagedThreadList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("List of Managed Threads\r\n");
            sb.AppendLine(string.Format("{0, -6}{1, -18}{2,-17}{3,-18}{4}", "ID", "State", "Client", "Start", "End"));
            int running = 0;

            foreach (var t in GetSafeCopy()) {
                if (t.Value.Thread.ThreadState == System.Threading.ThreadState.Running)
                    running++;

                string endTime = "-";

                if (t.Value.EndTime != DateTime.MaxValue) {
                    endTime = t.Value.EndTime.ToString("MM/dd/yy hh:mm:ss");
                }
                sb.AppendLine(string.Format("{0, 4}  {1, -18}{2,-17}{3,-18}{4}",
                    t.Key, t.Value.ThreadState, t.Value.Client,
                    t.Value.StartTime.ToString("MM/dd/yy hh:mm:ss"), endTime));
            }
            if (running == 1) {
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,6} thread is running.", running));
            }
            else {
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,6} threads are running.", running));
            }
            return sb.ToString();
        }

        public static string GetProcessThreadList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("List of all threads\r\n");
            sb.AppendLine(string.Format("    {0, -10}{1, -17}{2,-15}", "ID", "State", "Processor Time"));
            int running = 0;
            foreach (ProcessThread t in Process.GetCurrentProcess().Threads) {
                if (t.ThreadState == System.Diagnostics.ThreadState.Running)
                    ++running;
                sb.AppendLine(string.Format("    {0, -10}{1, -17}{2,-15}", t.Id, t.ThreadState, t.StartTime));
            }
            if (running == 1) {
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,6} thread is running.", running));
            }
            else {
                sb.AppendLine();
                sb.AppendLine(string.Format("{0,6} threads are running.", running));
            }
            return sb.ToString();
        }

        public void Clean()
        {
            List<int> toRemove = new List<int>();
            foreach (var v in GetSafeCopy()) {
                if (v.Value.ThreadState == System.Threading.ThreadState.Stopped ||
                    v.Value.ThreadState == System.Threading.ThreadState.Aborted) {
                    toRemove.Add(v.Key);
                }
            }

            foreach (int id in toRemove) {
                ThreadInfo info;
                threads.TryRemove(id, out info);
            }
        }

        private Dictionary<int, ThreadInfo> GetSafeCopy()
        {
            lock(locker) {
                return new Dictionary<int, ThreadInfo>(threads);
            }
        }

        public IEnumerator<KeyValuePair<int, ThreadInfo>> GetEnumerator()
        {
            return threads.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(int key)
        {
            return threads.ContainsKey(key);
        }

        public void Add(int key, ThreadInfo value)
        {
            if (!threads.TryAdd(key, value))
                throw new FunctionException(ErrorClient.Conflict, "Unable to add thread to collection");
        }

        bool IDictionary<int, ThreadInfo>.Remove(int key)
        {
            ThreadInfo info;
            return threads.TryRemove(key, out info);
        }

        public bool TryGetValue(int key, out ThreadInfo value)
        {
            return threads.TryGetValue(key, out value);
        }

        public void Clear()
        {
            threads.Clear();
        }

        void ICollection<KeyValuePair<int, ThreadInfo>>.Add(KeyValuePair<int, ThreadInfo> item)
        {
            ((IDictionary<int, ThreadInfo>)threads).Add(item);
        }

        bool ICollection<KeyValuePair<int, ThreadInfo>>.Contains(KeyValuePair<int, ThreadInfo> item)
        {
            return ((ICollection<KeyValuePair<int, ThreadInfo>>)threads).Contains(item);
        }

        void ICollection<KeyValuePair<int, ThreadInfo>>.CopyTo(KeyValuePair<int, ThreadInfo>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, ThreadInfo>>)threads).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<int, ThreadInfo>>.Remove(KeyValuePair<int, ThreadInfo> item)
        {
            return ((ICollection<KeyValuePair<int, ThreadInfo>>)threads).Remove(item);
        }

        bool ICollection<KeyValuePair<int, ThreadInfo>>.IsReadOnly
        {
            get {
                return ((ICollection<KeyValuePair<int, ThreadInfo>>)threads).IsReadOnly;
            }
        }
    }
}