// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Tbasic.Parsing
{
    /// <summary>
    /// A class for parsing statements, where each argument is an escaped string separated by a space
    /// </summary>
    public class Statement : IList<string>, ICloneable
    {
        private List<string> args = new List<string>();

        /// <summary>
        /// Gets or sets the argument at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get {
                return args[index];
            }

            set {
                args[index] = value;
            }
        }

        /// <summary>
        /// Gets the argument count
        /// </summary>
        public int Count
        {
            get {
                return args.Count;
            }
        }

        /// <summary>
        /// Constructs and parses a statement
        /// </summary>
        public Statement(IScanner scanner)
        {
            if (scanner == null)
                throw new ArgumentNullException(nameof(scanner));
            Contract.EndContractBlock();
            ParseArguments(scanner);
        }
        
        private void ParseArguments(IScanner scanner)
        {
            if (args.Count > 0)
                args.Clear();

            if (scanner.Length == 0)
                return;
            
            scanner.SkipWhiteSpace();
            Add(scanner.Next().ToString());
            scanner.SkipWhiteSpace();

            IEnumerable<char> arg;
            while(scanner.NextWordOrString(out arg)) {
                Add(arg.ToString());
                scanner.SkipWhiteSpace();
            }
        }

        /// <summary>
        /// Adds an argument to this command line
        /// </summary>
        /// <param name="item"></param>
        public void Add(string item)
        {
            args.Add(item);
        }

        /// <summary>
        /// Clears all arguments
        /// </summary>
        public void Clear()
        {
            args.Clear();
        }

        /// <summary>
        /// Determines if the command line contains an argument
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(string item)
        {
            return args.Contains(item);
        }

        /// <summary>
        /// Copies this collection into an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            args.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return args.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of a given argument
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(string item)
        {
            return args.IndexOf(item);
        }

        /// <summary>
        /// Inserts an argument at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, string item)
        {
            args.Insert(index, item);
        }

        /// <summary>
        /// Removes an argument from the command line
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(string item)
        {
            return args.Remove(item);
        }

        /// <summary>
        /// Removes an argument at a given index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            args.RemoveAt(index);
        }

        /// <summary>
        /// Converts this command line object to its string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string arg in this) {
                if (arg.Contains(' ') || arg.Contains('\"')) {
                    sb.Append(Quote(arg));
                }
                else {
                    sb.Append(arg);
                }
                sb.Append(' ');
            }
            return sb.ToString(0, sb.Length - 1); // removes last space
        }

        private static bool IsInQuotes(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 1) // null, empty string, and strings with one char cannot be in quotes
                return false;
            return str[0] == '\"' && str[str.Length - 1] == '\"';
        }

        private static string Quote(string str)
        {
            return '\"' + str.Replace("\"", "\"\"") + '\"';
        }

        private static string Unquote(string str)
        {
            if (IsInQuotes(str)) {
                return str.Substring(1, str.Length - 2).Replace("\"\"", "\"");
            }
            else {
                return str;
            }
        }

        private Statement()
        {
        }

        /// <summary>
        /// Clones this command line
        /// </summary>
        /// <returns></returns>
        public Statement Clone()
        {
            Statement clone = new Statement();
            clone.args.AddRange(args);
            return clone;
        }

        #region explicitly implemented

        object ICloneable.Clone()
        {
            return Clone();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return args.GetEnumerator();
        }

        bool ICollection<string>.IsReadOnly
        {
            get {
                return ((ICollection<string>)args).IsReadOnly;
            }
        }

        #endregion
    }
}
