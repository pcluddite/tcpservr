// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Runtime
{
    /// <summary>
    /// A list of flags to define the runtime environment
    /// </summary>
    [Flags]
    public enum ExecuterOption : int
    {
        /// <summary>
        /// No special options (default)
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Use strict type checking
        /// </summary>
        Strict = 0x01,
        /// <summary>
        /// Enforce the string type, don't convert from it (this does nothing if Strict is set)
        /// </summary>
        EnforceStrings = 0x02,
        /// <summary>
        /// FunctionExceptions should be thrown instead of simply setting the status code
        /// </summary>
        ThrowErrors = 0x04,
        /// <summary>
        /// Nulls should implicitly be converted to zero (this does nothing if Strict is set)
        /// </summary>
        NullIsZero = 0x08
    }
}
