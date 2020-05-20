// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using Tbasic.Errors;

namespace Tbasic.Parsing
{
    /// <summary>
    /// A collection of Line objects sorted by line number
    /// </summary>
    public class LineCollection : IList<Line>, ICollection<Line>, ICloneable
    {
        private SortedList<int, Line> allLines = new SortedList<int, Line>();

        /// <summary>
        /// Initializes a new LineCollection
        /// </summary>
        public LineCollection()
        {
        }

        /// <summary>
        /// Initializes a new LineCollection from another collection
        /// </summary>
        /// <param name="lines">the collection to add to this LineCollection</param>
        public LineCollection(ICollection<Line> lines)
            : this()
        {
            foreach (Line line in lines) {
                allLines.Add(line.LineNumber, line);
            }
        }

        /// <summary>
        /// Extracts Line objects starting at a given index with specified start and stop conditions
        /// </summary>
        /// <param name="index">the index to begin parsing</param>
        /// <param name="startPredicate">the condition which confirms the start of the block</param>
        /// <param name="endPredicate">the condition which confirms the end of a block</param>
        /// <returns>a LineCollection of the extracted lines</returns>
        public LineCollection ParseBlock(int index, Predicate<Line> startPredicate, Predicate<Line> endPredicate)
        {
            return ParseBlock(index, this, startPredicate, endPredicate);
        }

        internal static LineCollection ParseBlock(int index, IList<Line> allLines, Predicate<Line> start, Predicate<Line> end)
        {
            LineCollection blockLines = new LineCollection();

            int expected = 0;
            for (; index < allLines.Count; index++) {
                Line current = allLines[index];
                if (start.Invoke(current)) {
                    expected++;
                }
                if (end.Invoke(current)) {
                    expected--;
                }
                blockLines.Add(current);
                if (expected == 0) {
                    return blockLines;
                }
            }

            throw ThrowHelper.UnterminatedBlock(blockLines[0].VisibleName);
        }

        /// <summary>
        /// Adds a Line and sorts it by its LineNumber
        /// </summary>
        /// <param name="item"></param>
        public void Add(Line item)
        {
            allLines.Add(item.LineNumber, item);
        }

        /// <summary>
        /// Removes all items from this collection
        /// </summary>
        public void Clear()
        {
            allLines.Clear();
        }

        /// <summary>
        /// Determines if this collection contains a a Line (this will return true when the collection contains a Line with the same LineNumber)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Line item)
        {
            return allLines.ContainsKey(item.LineNumber);
        }

        /// <summary>
        /// Copies all elements in this collection
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Line[] array, int arrayIndex)
        {
            allLines.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of Lines contained in this collection
        /// </summary>
        public int Count
        {
            get { return allLines.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this collection is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes multiple elements from this collection
        /// </summary>
        /// <param name="lines"></param>
        public void Remove(ICollection<Line> lines)
        {
            foreach (Line line in lines) {
                Remove(line);
            }
        }

        /// <summary>
        /// Removes a Line from the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Line item)
        {
            return allLines.Remove(item.LineNumber);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Line> GetEnumerator()
        {
            return allLines.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allLines.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the list index of a code line
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Line item)
        {
            return allLines.IndexOfKey(item.LineNumber);
        }

        /// <summary>
        /// Gets the list index of a code line from its line index
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public int IndexOf(int lineNumber)
        {
            return allLines.IndexOfKey(lineNumber);
        }

        /// <summary>
        /// Throws a NotImplementedException. This method is unsupported because items are automatically sorted.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Line item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a code line at its list index, not its line index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            allLines.RemoveAt(index);
        }

        /// <summary>
        /// Returns a line from its LineNumber
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public Line LineAt(int lineNumber)
        {
            return allLines[lineNumber];
        }

        /// <summary>
        /// Gets code at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Line this[int index]
        {
            get {
                return allLines.Values[index];
            }
            set {
                allLines.Values[index] = value;
            }
        }

        /// <summary>
        /// Returns a shallow copy of this collection
        /// </summary>
        /// <returns></returns>
        public LineCollection Clone()
        {
            return new LineCollection(allLines.Values);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}