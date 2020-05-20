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
using System.IO;
using System.Security;

namespace Tbasic.Errors
{
    /// <summary>
    /// An exception that has been associated with a status code
    /// </summary>
    public class TbasicException : Exception
    {
        /// <summary>
        /// Gets the status code for this exception
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// Gets whether a generic message was prepended to the server message
        /// </summary>
        public bool GenericPrepended { get; private set; }

        /// <summary>
        /// Constructs a new CustomException with a given status code
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="innerException">the inner exception</param>
        public TbasicException(int status, Exception innerException = null)
            : this(status, GetGenericMessage(status), false, innerException)
        {
        }

        /// <summary>
        /// Constructs a new CustomException with a given status code and message
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="msg">the message for this exception</param>
        /// /// <param name="innerException">the inner exception</param>
        public TbasicException(int status, string msg, Exception innerException = null)
            : this(status, msg, true, innerException)
        {
        }

        /// <summary>
        /// Constructs a new CustomException with a given status code and message
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="msg">the message for this exception</param>
        /// /// <param name="innerException">the inner exception</param>
        public TbasicException(int status, string msg, TbasicException innerException)
            : this(status, msg, !innerException.GenericPrepended, innerException)
        {
        }

        /// <summary>
        /// Constructs a new CustomException with a given status code and message
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="msg">the message for this exception</param>
        /// <param name="prependGeneric">true to add the generic message, false otherwise</param>
        /// <param name="innerException">the inner exception</param>
        public TbasicException(int status, string msg, bool prependGeneric, Exception innerException = null)
            : base(prependGeneric ? GetGenericMessage(status) + ": " + msg : msg, innerException)
        {
            Status = status;
            GenericPrepended = prependGeneric;
        }

        /// <summary>
        /// Gets a generic message for an exception
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetGenericMessage(int status)
        {
            switch (status) {
                // success
                case ErrorSuccess.OK:
                    return "OK";
                case ErrorSuccess.Created:
                    return "Created";
                case ErrorSuccess.Accepted:
                    return "Accepted";
                case ErrorSuccess.NonAuthoritative:
                    return "Non-Authoritative Information";
                case ErrorSuccess.NoContent:
                    return "No Content";
                case ErrorSuccess.Warnings:
                    return "Warning";
                // client errors
                case ErrorClient.BadRequest:
                    return "Bad Request";
                case ErrorClient.Unauthorized:
                    return "Unauthorized";
                case ErrorClient.Forbidden:
                    return "Forbidden";
                case ErrorClient.NotFound:
                    return "Not Found";
                case ErrorClient.Conflict:
                    return "Conflict";
                case ErrorClient.Locked:
                    return "Locked";
                // server errors
                case ErrorServer.GenericError:
                    return "Internal Error";
                case ErrorServer.NotImplemented:
                    return "Not Implemented";
                case ErrorServer.BadGateway:
                    return "Bad Gateway";
                case ErrorServer.NoMemory:
                    return "Insufficient Memory";
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts some common exceptions into a TbasicException. If it cannot be converted, the original exception is returned
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception WrapException(Exception ex)
        {
            if (ex is ArgumentException || ex is FormatException) {
                return new TbasicException(ErrorClient.BadRequest, ex.Message, ex);
            }
            else if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is DriveNotFoundException) {
                return new TbasicException(ErrorClient.NotFound, ex);
            }
            else if (ex is UnauthorizedAccessException || ex is SecurityException || ex is InvalidOperationException || ex is InvalidCastException) {
                return new TbasicException(ErrorClient.Forbidden, ex.Message, ex);
            }
            else if (ex is NotImplementedException) {
                return new TbasicException(ErrorServer.NotImplemented, ex.Message, ex);
            }
            else if (ex is IOException) {
                return new TbasicException(ErrorClient.Locked, ex.Message, ex);
            }
            return ex;
        }
    }
}