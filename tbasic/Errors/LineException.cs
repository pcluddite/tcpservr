// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Text;

namespace Tbasic.Errors
{
    /// <summary>
    /// Represents a parsing exception that occoured on a specific line
    /// </summary>
    public class LineException : TbasicRuntimeException
    {
        /// <summary>
        /// The line at which the error occoured
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// The function or command that caused the error
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the detail message that is a part of the exception message
        /// </summary>
        public string DetailMessage { get; private set; }

        /// <summary>
        /// Constructs a new instance of this class
        /// </summary>
        /// <param name="line">the line number at which the error occoured</param>
        /// <param name="name">the function or command that caused the error</param>
        /// <param name="innerException">the exception that occoured</param>
        public LineException(int line, string name, Exception innerException)
            : base(string.Format("An error occoured at '{0}' on line {1}\n", name, line) + GetMessage(innerException), innerException)
        {
            Line = line;
            Name = name;
            if (!(innerException is LineException))
                DetailMessage = innerException.Message;
        }
        
        private static string GetMessage(Exception ex)
        {
            StringBuilder msg = new StringBuilder();

            LineException current = ex as LineException;
            string details = ex.Message;
            while (current != null) { // traverse the exception until we find the actual error
                details = current.DetailMessage;
                msg.AppendFormat("\tat '{0}' on line {1}\n", current.Name, current.Line);
                current = current.InnerException as LineException;
            }
            msg.Append("\nDetail:\n");
            msg.AppendFormat("{0}", details);
            return msg.ToString();
        }
    }
}