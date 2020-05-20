// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections.Generic;

namespace Tbasic.Parsing
{
    /// <summary>
    /// A delegate for creating scanner objects
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public delegate IScanner CreateScannerDelegate<T>(T buffer) where T : IList<char>;

    /// <summary>
    /// A static class with some scanners
    /// </summary>
    public static class Scanners
    {
        /// <summary>
        /// Gets the default BASIC scanner
        /// </summary>
        public static readonly IScanner Default = new DefaultScanner(string.Empty);
        /// <summary>
        /// Gets a scanner better suited for a command line interface
        /// </summary>
        public static readonly IScanner Terminal = new TerminalScanner(string.Empty);
    }
}
