// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Tcpservr.Errors
{
    /// <summary>
    /// Manages logged errors. This class should be thread safe.
    /// </summary>
    public class LoggedErrorCollection : IEnumerable<LoggedError>
    {
        private ConcurrentQueue<LoggedError> errors;
        private int capacity;

        public LoggedErrorCollection(int capacity)
        {
            this.capacity = capacity;
            errors = new ConcurrentQueue<LoggedError>();
        }

        public void Add(LoggedError error)
        {
            errors.Enqueue(error);
            if (errors.Count > capacity) {
                LoggedError oldest;
                errors.TryDequeue(out oldest);
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (LoggedError error in errors) {
                result.AppendLine(error.ToString());
            }
            return result.ToString();
        }

        public IEnumerator<LoggedError> GetEnumerator()
        {
            return errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}