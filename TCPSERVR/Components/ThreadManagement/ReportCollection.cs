using System;
using System.Collections.Generic;
using System.Linq;
using Tcpservr.Messaging;
using Tbasic.Errors;
using System.Collections;
using System.Collections.Concurrent;

namespace Tcpservr.Threads
{
    /// <summary>
    /// Manages a collection of reports of ServerInputMessages. This class is designed to be thread-safe.
    /// </summary>
    public class ReportCollection : IEnumerable<KeyValuePair<ServerInputMessage, Report>>
    {
        private ConcurrentDictionary<ServerInputMessage, Report> reports = new ConcurrentDictionary<ServerInputMessage, Report>();
        
        public void Clear()
        {
            reports.Clear();
        }

        public void AddReport(ServerInputMessage msg, Report report)
        {
            if (!reports.TryAdd(msg, report))
                throw new FunctionException(ErrorClient.Conflict, "Unable to add message report to collection");
        }

        public Report GetReport(ServerInputMessage msg)
        {
            Report report;
            if (!reports.TryGetValue(msg, out report)) {
                throw NotFoundException();
            }
            return report;
        }

        private static FunctionException NotFoundException()
        {
            return new FunctionException(ErrorClient.NotFound, "No report found for that message", false);
        }

        public Report GetReportFromId(int id)
        {
            foreach (var v in reports) {
                if (v.Key.ID == id && v.Value != null)
                    return v.Value;
            }
            throw NotFoundException();
        }
                
        public IEnumerator<KeyValuePair<ServerInputMessage, Report>> GetEnumerator()
        {
            return reports.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}