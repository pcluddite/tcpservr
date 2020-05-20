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
using System.Collections.Generic;
using System.Collections;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    /// <summary>
    /// Delegate for processing a TBasic function
    /// </summary>
    /// <param name="stack">The StackFrame containing parameter and execution information</param>
    public delegate void TBasicFunction(TFunctionData stack);

    /// <summary>
    /// A library for storing and processing TBasic functions
    /// </summary>
    public class Library : IDictionary<string, TBasicFunction>
    {

        private Dictionary<string, TBasicFunction> lib = new Dictionary<string, TBasicFunction>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        public Library()
        {
        }

        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        /// <param name="libs">a collection of Library objects that should be incorporated into this one</param>
        public Library(ICollection<Library> libs)
        {
            foreach (Library lib in libs) 
                AddLibrary(lib);
        }

        /// <summary>
        /// Adds a Tbasic Library to this one
        /// </summary>
        /// <param name="lib">the Tbasic Library</param>
        public void AddLibrary(Library lib)
        {
            foreach (var kv_entry in lib)
                Add(kv_entry.Key, kv_entry.Value);
        }

        public void Add(string key, TBasicFunction value)
        {
            lib.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return lib.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return lib.Keys; }
        }

        public bool Remove(string key)
        {
            return lib.Remove(key);
        }

        public bool TryGetValue(string key, out TBasicFunction value)
        {
            return lib.TryGetValue(key, out value);
        }

        public ICollection<TBasicFunction> Values
        {
            get { return lib.Values; }
        }

        public TBasicFunction this[string key]
        {
            get
            {
                return lib[key];
            }
            set
            {
                lib[key] = value;
            }
        }

        void ICollection<KeyValuePair<string, TBasicFunction>>.Add(KeyValuePair<string, TBasicFunction> item)
        {
            ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).Add(item);
        }

        public void Clear()
        {
            lib.Clear();
        }

        bool ICollection<KeyValuePair<string, TBasicFunction>>.Contains(KeyValuePair<string, TBasicFunction> item)
        {
            return ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).Contains(item);
        }

        void ICollection<KeyValuePair<string, TBasicFunction>>.CopyTo(KeyValuePair<string, TBasicFunction>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return lib.Count; }
        }

        bool ICollection<KeyValuePair<string, TBasicFunction>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<string, TBasicFunction>>.Remove(KeyValuePair<string, TBasicFunction> item)
        {
            return ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, TBasicFunction>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<string, TBasicFunction>>)lib).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
