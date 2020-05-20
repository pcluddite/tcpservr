// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Tbasic.Parsing
{
    /// <summary>
    /// Defines a set of methods and properties for a line of Tbasic code
    /// </summary>
    [DebuggerDisplay("[{LineNumber}, {Text}]")]
    public struct Line : IComparable<Line>, IEquatable<Line>, IEnumerable<char>
    {
        private string visibleName;
        private string text;

        /// <summary>
        /// Gets a value indicating whether this line is formatted like a function
        /// </summary>
        public bool IsFunction { get; private set; }

        /// <summary>
        /// Gets or sets the line number
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the text of this line
        /// </summary>
        public string Text
        {
            get {
                return text;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                Contract.EndContractBlock();

                text = value;
                FindAndSetName();
            }
        }

        /// <summary>
        /// Gets or sets the name of the line displayed in exceptions
        /// </summary>
        public string VisibleName
        {
            get {
                return visibleName ?? Name;
            }
            set {
                visibleName = value;
            }
        }

        /// <summary>
        /// Gets the name that is retreived from the ObjectContext libraries
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a line of Tbasic code
        /// </summary>
        /// <param name="id">The id of the line. This should be the line number.</param>
        /// <param name="line">The text of the line</param>
        public Line(int id, string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            Contract.EndContractBlock();

            LineNumber = id;
            visibleName = null;
            text = line.Trim(); // Ignore leading and trailing whitespace.
            IsFunction = false;
            Name = string.Empty;
            FindAndSetName();
        }

        internal static Line CreateLineNoTrim(int id, string line)
        {
            return new Line() { LineNumber = id, Text = line };
        }
        
        private void FindAndSetName()
        {
            int paren = Text.IndexOf('(');
            int space = Text.IndexOf(' ');
            IsFunction = false;
            if (paren < 0 && space < 0) { // no paren or space, the name is the who line
                Name = Text;
            }
            else if (paren < 0 && space > 0) { // no paren, but there's a space
                Name = Text.Remove(space);
            }
            else if (space < 0 && paren > 0) { // no space, but there's a paren
                Name = Text.Remove(paren);
                IsFunction = true;
            }
            else if (space < paren) { // the space is before the paren, so that's where the name is
                Name = Text.Remove(space);
            }
            else {
                Name = Text.Remove(paren);
                IsFunction = true; // it's formatted like a function
            }
        }

        /// <summary>
        /// Returns the text that this line represents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Compares this Line to another Line by comparing their LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Line other)
        {
            return LineNumber.CompareTo(other.LineNumber);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            Line? l = other as Line?;
            if (l != null)
                return Equals(l);
            return false;
        }

        /// <summary>
        /// Hash code for the LineNumber
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return LineNumber.GetHashCode() ^ Text.GetHashCode() ^ VisibleName.GetHashCode();
        }

        /// <summary>
        /// Determines if two Line objects share the same LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Line other)
        {
            return LineNumber == other.LineNumber && Text == other.Text && VisibleName == other.VisibleName;
        }

        /// <summary>
        /// Gets the enumerator for this type
        /// </summary>
        public IEnumerator<char> GetEnumerator()
        {
            return Text.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Text.GetEnumerator();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(text != null);
            Contract.Invariant(Name != null);
        }
    }
}