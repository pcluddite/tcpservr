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
using System;

namespace Tbasic.Parsing
{
    /// <summary>
    /// Defines a set of methods and properties for a line of Tbasic code
    /// </summary>
    public class Line : IComparable<Line>, IEquatable<Line>
    {
        private bool? isFunc;
        private string visibleName;
        private string name;
        private string text;

        /// <summary>
        /// Gets a value indicating whether this line is formatted like a function
        /// </summary>
        public bool IsFunction
        {
            get {
                if (isFunc == null) {
                    FindAndSetName();
                }
                return isFunc.Value;
            }
        }

        /// <summary>
        /// Gets or sets the line number
        /// </summary>
        public uint LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the text of this line
        /// </summary>
        public string Text
        {
            get {
                return text;
            }
            set {
                text = value;
                name = null;
                isFunc = null;
            }
        }

        /// <summary>
        /// Gets or sets the name of the line displayed in exceptions
        /// </summary>
        public string VisibleName
        {
            get {
                if (visibleName == null) {
                    return Name;
                }
                return visibleName;
            }
            set {
                visibleName = value;
            }
        }

        /// <summary>
        /// Retrieves the name that is retreived from the ObjectContext libraries
        /// </summary>
        public string Name
        {
            get {
                if (name == null) { // This way we don't have to do this every time
                    FindAndSetName();
                }
                return name;
            }
        }

        /// <summary>
        /// Initializes a line of Tbasic code
        /// </summary>
        /// <param name="id">The id of the line. This should be the line number.</param>
        /// <param name="line">The text of the line</param>
        public Line(uint id, string line)
        {
            LineNumber = id;
            Text = line.Trim(); // Ignore leading and trailing whitespace.
            VisibleName = Name;
        }

        /// <summary>
        /// Initializes a line of Tbasic code carring the same information as another Tbasic.Line
        /// </summary>
        /// <param name="line"></param>
        public Line(Line line)
        {
            LineNumber = line.LineNumber;
            Text = line.Text;
            VisibleName = line.Name;
        }

        private void FindAndSetName()
        {
            int paren = Text.IndexOf('(');
            int space = Text.IndexOf(' ');
            isFunc = false;
            if (paren < 0 && space < 0) { // no paren or space, the name is the who line
                name = Text;
            }
            else if (paren < 0 && space > 0) { // no paren, but there's a space
                name = Text.Remove(space);
            }
            else if (space < 0 && paren > 0) { // no space, but there's a paren
                name = Text.Remove(paren);
                isFunc = true;
            }
            else if (space < paren) { // the space is before the paren, so that's where the name is
                name = Text.Remove(space);
            }
            else {
                name = Text.Remove(paren);
                isFunc = true; // it's formatted like a function
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
        /// Compares this Tbasic.Line to another Tbasic.Line by comparing their LineNumber
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
            if (other is Line) {
                return this.Equals((Line)other);
            }
            return base.Equals(other);
        }

        /// <summary>
        /// Hash code for the LineNumber
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return LineNumber.GetHashCode();
        }

        /// <summary>
        /// Determines if two Tbasic.Line objects share the same LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Line other)
        {
            return other.LineNumber == LineNumber;
        }
    }
}