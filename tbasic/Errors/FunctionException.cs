// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.IO;
using System.Security;

namespace Tbasic.Errors
{
    /// <summary>
    /// An exception that occours within a Tbasic function or subroutine and has an associated a status code
    /// </summary>
    public class FunctionException : TbasicRuntimeException
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
        public FunctionException(int status, Exception innerException = null)
            : this(status, GetGenericMessage(status), false, innerException)
        {
        }

        /// <summary>
        /// Constructs a new CustomException with a given status code and message
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="msg">the message for this exception</param>
        /// /// <param name="innerException">the inner exception</param>
        public FunctionException(int status, string msg, Exception innerException = null)
            : this(status, msg, true, innerException)
        {
        }

        /// <summary>
        /// Constructs a new CustomException with a given status code and message
        /// </summary>
        /// <param name="status">the status code for this exception</param>
        /// <param name="msg">the message for this exception</param>
        /// /// <param name="innerException">the inner exception</param>
        public FunctionException(int status, string msg, FunctionException innerException)
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
        public FunctionException(int status, string msg, bool prependGeneric, Exception innerException = null)
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
        /// Converts some common exceptions into a FunctionException. If it cannot be converted, null is returned.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static FunctionException FromException(Exception ex)
        {
            if (ex is ArgumentException || ex is FormatException) {
                return new FunctionException(ErrorClient.BadRequest, ex.Message, ex);
            }
            else if (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is DriveNotFoundException) {
                return new FunctionException(ErrorClient.NotFound, ex);
            }
            else if (ex is UnauthorizedAccessException || ex is SecurityException || ex is InvalidOperationException || ex is InvalidCastException) {
                return new FunctionException(ErrorClient.Forbidden, ex.Message, ex);
            }
            else if (ex is NotImplementedException) {
                return new FunctionException(ErrorServer.NotImplemented, ex.Message, ex);
            }
            else if (ex is IOException) {
                return new FunctionException(ErrorClient.Locked, ex.Message, ex);
            }
            return null;
        }
    }
}