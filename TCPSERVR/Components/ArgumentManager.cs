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

namespace Tcpservr.Components
{
    public class ArgumentManager : IDictionary<string, string>
    {
        private Dictionary<string, string> args;

        public ArgumentManager(string[] args)
            : this(args, StringComparer.OrdinalIgnoreCase)
        {
        }

        public ArgumentManager(string[] _args, StringComparer comparer)
        {
            args = new Dictionary<string, string>(comparer);
            ParseArguments(_args);
        }

        private void ParseArguments(string[] _args)
        {
            // do something here
        }

        public string this[string key]
        {
            get {
                return args[key];
            }

            set {
                args[key] = value;
            }
        }

        public int Count
        {
            get {
                return args.Count;
            }
        }

        public ICollection<string> Keys
        {
            get {
                return args.Keys;
            }
        }

        public ICollection<string> Values
        {
            get {
                return args.Values;
            }
        }

        public void Add(string key, string value)
        {
            args.Add(key, value);
        }

        public void Clear()
        {
            args.Clear();
        }

        public bool ContainsKey(string key)
        {
            return args.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return args.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return args.TryGetValue(key, out value);
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>)args).Remove(item);
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            ((ICollection<KeyValuePair<string, string>>)args).Add(item);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return args.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>)args).Contains(item);
        }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get {
                return ((ICollection<KeyValuePair<string, string>>)args).IsReadOnly;
            }
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, string>>)args).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
