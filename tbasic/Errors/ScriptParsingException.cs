// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Errors
{
    /// <summary>
    /// Represents a generic script parsing error
    /// </summary>
    public class ScriptParsingException : TbasicRuntimeException
    {
        /// <summary>
        /// Initializes a new exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerEx"></param>
        public ScriptParsingException(string message, Exception innerEx = null)
            : base(message, innerEx)
        {
        }
    }

    /// <summary>
    /// An exception pertaining to reaching the end of the code
    /// </summary>
    public class EndOfCodeException : ScriptParsingException
    {
        /// <summary>
        /// Initializes a new exception
        /// </summary>
        /// <param name="msg"></param>
        public EndOfCodeException(string msg) :
            base(msg, new FormatException(msg))
        {
        }

        /// <summary>
        /// Initializes a new exception
        /// </summary>
        public EndOfCodeException()
            : this("End of code was not expected.")
        {
        }
    }

    /// <summary>
    /// Occours when a token was expected, but not found
    /// </summary>
    public class ExpectedTokenExceptiopn : ScriptParsingException
    {
        /// <summary>
        /// Initializes a new exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prependGeneric"></param>
        public ExpectedTokenExceptiopn(string msg, bool prependGeneric = true)
            : base(prependGeneric ? ("Expected token in expression: " + msg) : msg)
        {
        }
    }

    /// <summary>
    /// Occours when an invalid token was parsed
    /// </summary>
    public class InvalidTokenException : ScriptParsingException
    {
        /// <summary>
        /// Initializes a new exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prependGeneric"></param>
        public InvalidTokenException(string msg, bool prependGeneric = true)
            : base(prependGeneric ? ($"Unexpected token in expression [ {msg} ]") : msg)
        {
        }
    }

    /// <summary>
    /// Occours when an invalid or unexpected operator was parsed
    /// </summary>
    public class InvalidOperatorException : InvalidTokenException
    {
        /// <summary>
        /// Initializes a new exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prependGeneric"></param>
        public InvalidOperatorException(string msg, bool prependGeneric = true)
            : base(prependGeneric ? ("Invalid operator in expression: " + msg) : msg, prependGeneric: false)
        {
        }
    }
}
