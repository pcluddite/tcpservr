// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Errors
{
    /// <summary>
    /// Occours when any definable symbol cannot be defined by the preprocessor due to syntax
    /// </summary>
    public class InvalidDefinitionException : ScriptParsingException
    {
        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerEx"></param>
        public InvalidDefinitionException(string message, Exception innerEx = null) : base(message, innerEx)
        {
        }

        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type">the type of symbol that could not be defined</param>
        /// <param name="innerEx"></param>
        public InvalidDefinitionException(string message, string type, Exception innerEx = null)
            : base(string.Format("Invalid {0} definition: {1}", type, message), innerEx)
        {
        }
    }
}
