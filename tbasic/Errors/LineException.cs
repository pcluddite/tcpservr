/**
 *  TBASIC
 *  Copyright (C) 2013-2016 Timothy Baxendale
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *  
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 **/
using System;
using System.Text;

namespace Tbasic.Errors
{
    /// <summary>
    /// Represents a parsing exception that occoured on a specific line
    /// </summary>
    public class LineException : ScriptParsingException
    {
        /// <summary>
        /// The line at which the error occoured
        /// </summary>
        public uint Line { get; private set; }
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
        public LineException(uint line, string name, Exception innerException)
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
            while (current != null) { // traverse the exception until we find the actual error
                msg.AppendFormat("\tat '{0}' on line {1}\n", current.Name, current.Line);
                current = (ex = current.InnerException) as LineException;
            }
            msg.Append("\nDetail:\n");
            msg.AppendFormat("{0}", ex.Message);
            return msg.ToString();
        }
    }
}