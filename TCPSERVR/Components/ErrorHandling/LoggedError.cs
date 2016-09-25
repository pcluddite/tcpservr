// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Security.Principal;
using System.Text;

namespace Tcpservr.Errors
{
    public class LoggedError
    {
        private const string DATETIME_FORMAT = "MM/dd/yyyy hh:mm:ss tt";
        
        public DateTime ErrorTime { get; private set; }
        public bool Dominant { get; private set; }
        public string Message { get; private set; }
        public string TargetSite { get; private set; }
        public bool Fatal { get; private set; }
        public string Sender { get; private set; }

        public LoggedError(string sender, Exception ex, bool dominant, bool fatal)
            : this(sender, ex.Message, ex.TargetSite.Name, dominant, fatal)
        {
        }

        public LoggedError(string sender, string msg, string method, bool dominant, bool fatal)
        {
            ErrorTime = DateTime.Now;
            Sender = sender;
            Message = msg;
            TargetSite = method;
            Dominant = dominant;
            Fatal = fatal;
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("".PadLeft(50, '='));
            sb.AppendLine("Date:\t" + ErrorTime.ToString(DATETIME_FORMAT));
            sb.AppendLine("Status:\t" + (Dominant ? "Master" : "Slave"));
            sb.AppendLine("User:\t" + WindowsIdentity.GetCurrent().Name);
            sb.AppendLine((Dominant ? "Client:" : "Pipe:") + "\t" + Sender);
            sb.AppendLine("Method:\t" + TargetSite);
            sb.AppendLine("Message:");
            sb.AppendLine(Message);
            return sb.ToString();
        }
    }
}