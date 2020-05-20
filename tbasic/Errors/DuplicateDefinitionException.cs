// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Errors
{
    /// <summary>
    /// Occours when an object (such as a function) has already been defined
    /// </summary>
    public class DuplicateDefinitionException : TbasicRuntimeException
    {
        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        /// <param name="name">the name of the object that has already been defined</param>
        public DuplicateDefinitionException(string name)
            : base(string.Format("'{0}' has already been defined.", name))
        {
        }
        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        /// <param name="msg">the message for this exception</param>
        /// <param name="innerException"></param>
        public DuplicateDefinitionException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }
}
