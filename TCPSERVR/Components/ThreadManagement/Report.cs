using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcpservr.Threads
{
    public class Report
    {
        private List<KeyValuePair<string, bool>> tasks = new List<KeyValuePair<string, bool>>();

        public string CurrentTask
        {
            get {
                return tasks[tasks.Count - 1].Key;
            }
        }

        public bool IsCompleted
        {
            get {
                return tasks[tasks.Count - 1].Value;
            }
        }

        private string cmd;

        /// <summary>
        /// Creates a report for a command
        /// </summary>
        /// <param name="cmd"></param>
        public Report(string cmd)
        {
            this.cmd = cmd;
        }

        /// <summary>
        /// Removes a task from this report
        /// </summary>
        /// <param name="taskName"></param>
        public void RemoveTask(string taskName)
        {
            for(int index = 0; index < taskName.Length; ++index) {
                if (tasks[index].Key == taskName) {
                    tasks.RemoveAt(index);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the current task of the command
        /// </summary>
        /// <param name="task"></param>
        public void SetTask(string task)
        {
            if (tasks.Count > 0) {
                Complete();
            }
            tasks.Add(new KeyValuePair<string, bool>(task, false));
        }

        /// <summary>
        /// Indicates that the last task was completed
        /// </summary>
        public void Complete()
        {
            tasks[tasks.Count - 1] = new KeyValuePair<string, bool>(tasks[tasks.Count - 1].Key, true);
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("|\n|{0,-25}{1}\n|", " ", cmd + " Command Report");
            foreach (var v in tasks) {
                sb.AppendFormat("\n{0,-50}{1}", v.Key + "...", v.Value ? "Completed" : "Working");
            }
            return sb.ToString();
        }
    }
}
