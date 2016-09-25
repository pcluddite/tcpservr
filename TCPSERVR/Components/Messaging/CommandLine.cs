// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcpservr.Messaging
{
    public class CommandLine : IList<string>, ICloneable
    {
        private List<string> args = new List<string>();

        public string this[int index]
        {
            get {
                return args[index];
            }

            set {
                args[index] = value;
            }
        }

        public int Count
        {
            get {
                return args.Count;
            }
        }

        public CommandLine(string line)
        {
            ParseArguments(line);
        }

        public CommandLine(params string[] cmdArgs)
        {
            args.AddRange(cmdArgs);
        }

        public CommandLine(IEnumerable<object> _sframe)
        {
            ParseArguments(_sframe);
        }

        private void ParseArguments(IEnumerable<object> _sframe)
        {
            foreach(object o in _sframe) {
                args.Add(o.ToString());
            }
        }

        /// <summary>
        /// Arguments are parsed just like the windows command line
        /// </summary>
        /// <param name="line"></param>
        private void ParseArguments(string line)
        {
            if (args.Count > 0) {
                args.Clear();
            }

            char[] chars = line.ToCharArray();
            bool inquote = false;

            for (int index = 0; index < chars.Length; ++index) {
                if (chars[index] == '"')
                    inquote = !inquote;
                if (!inquote && chars[index] == ' ')
                    chars[index] = '\n';
            }

            args.AddRange(new string(chars).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

            for (int index = 0; index < args.Count; ++index) {
                args[index] = Unquote(args[index]);
            }
        }

        public void Add(string item)
        {
            args.Add(item);
        }

        public void Clear()
        {
            args.Clear();
        }

        public bool Contains(string item)
        {
            return args.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            args.CopyTo(array, arrayIndex);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return args.GetEnumerator();
        }

        public int IndexOf(string item)
        {
            return args.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            args.Insert(index, item);
        }

        public bool Remove(string item)
        {
            return args.Remove(item);
        }

        public void RemoveAt(int index)
        {
            args.RemoveAt(index);
        }

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

        public CommandLine Clone()
        {
            CommandLine clone = new CommandLine("");
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
