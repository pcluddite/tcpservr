// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections.Generic;
using System.IO;
using Tbasic.Runtime;
using Tbasic.Types;

namespace Tbasic.Parsing
{
    /// <summary>
    /// An interface for scanning lines before they are executed
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Gets a collection of the functions defined within the script
        /// </summary>
        ICollection<FunctionBlock> Functions { get; }
        /// <summary>
        /// Gets a collection of the classes defined within he script
        /// </summary>
        ICollection<TClass> Classes { get; }
        /// <summary>
        /// Gets the lines that should be executed
        /// </summary>
        LineCollection Lines { get; }
        /// <summary>
        /// Preprocesses all ines in the reader and declares all functions and classes
        /// </summary>
        /// <param name="runtime">the current runtime, which may not yet be fully initialized</param>
        /// <param name="reader">the reader for the script</param>
        /// <returns>the preprocessor</returns>
        IPreprocessor Preprocess(TRuntime runtime, TextReader reader);
    }
}
