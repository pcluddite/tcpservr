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
using Tbasic.Runtime;
using System;
using Tbasic.Parsing;

namespace Tbasic {
    
    /// <summary>
    /// Delegate for initializing a CodeBlock
    /// </summary>
    /// <param name="index">index in the collection which the block starts</param>
    /// <param name="lines"></param>
    /// <returns></returns>
    public delegate CodeBlock BlockCreator(int index, LineCollection lines);

    /// <summary>
    /// Defines a set of methods and properties for a code block
    /// </summary>
    public abstract class CodeBlock {

        /// <summary>
        /// Gets or sets the line of code that begins the block
        /// </summary>
        public virtual Line Header { get; set; }

        /// <summary>
        /// Gets or sets the line of code that ends the block
        /// </summary>
        public virtual Line Footer { get; set; }

        /// <summary>
        /// Gets or sets all of the code lines of the main portion of the block
        /// </summary>
        public virtual LineCollection Body { get; set; }

        /// <summary>
        /// Gets the number of code lines in the block
        /// </summary>
        public virtual int Length {
            get {
                return Body.Count + 2; // There are 2 keywords, so the length is actually 2 more
            }
        }

        /// <summary>
        /// Sets the header as the first line in the collection, the last line as the footer, and the code in between as the body
        /// </summary>
        /// <param name="blockLines"></param>
        protected void LoadFromCollection(LineCollection blockLines) {
            LineCollection body = blockLines.Clone();
            Header = blockLines[0];
            Footer = blockLines[blockLines.Count - 1];
            body.Remove(Header);
            body.Remove(Footer);
            Body = body;
        }

        /// <summary>
        /// When overridden in a derived class, executes this block
        /// </summary>
        /// <param name="exec">the current execution</param>
        public abstract void Execute(Executer exec);
    }
}