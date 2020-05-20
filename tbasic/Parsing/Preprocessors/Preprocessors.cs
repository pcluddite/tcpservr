// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======

namespace Tbasic.Parsing
{
    /// <summary>
    /// A static class with some preprocessors
    /// </summary>
    public static class Preprocessors
    {
        /// <summary>
        /// The default BASIC preprocessor
        /// </summary>
        public static readonly IPreprocessor Default = new DefaultPreprocessor();
        /// <summary>
        /// A preprocessor better suited for command line environments
        /// </summary>
        public static readonly IPreprocessor Terminal = new TerminalPreprocessor();
    }
}
