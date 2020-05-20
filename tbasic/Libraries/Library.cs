// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tbasic.Types;

namespace Tbasic.Libraries
{
    /// <summary>
    /// A library for storing and processing Tbasic functions
    /// </summary>
    public partial class Library : IDictionary<string, CallData>
    {
        private Dictionary<string, CallData> lib = new Dictionary<string, CallData>(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        public Library()
        {
        }

        /// <summary>
        /// Initializes a new Tbasic Library object incorporating the functions from another library
        /// </summary>
        public Library(Library other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            Contract.EndContractBlock();

            lib = new Dictionary<string, CallData>(other.lib);
        }

        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        /// <param name="libs">a collection of Library objects that should be incorporated into this one</param>
        public Library(IEnumerable<Library> libs)
        {
            if (libs == null)
                throw new ArgumentNullException(nameof(libs));
            Contract.EndContractBlock();

            foreach (Library lib in libs) 
                AddLibrary(lib);
        }

        /// <summary>
        /// Adds a Tbasic Library to this one
        /// </summary>
        /// <param name="other">the Tbasic Library</param>
        public void AddLibrary(Library other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            Contract.EndContractBlock();

            foreach (var kv_entry in other.lib)
                lib.Add(kv_entry.Key, kv_entry.Value);
        }

        /// <summary>
        /// Adds a function to this dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, TbasicFunction value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Contract.EndContractBlock();

            lib.Add(key, new CallData(value, evaluate: true));
        }

        /// <summary>
        /// Adds a function to this dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="evaluate">whether or not this function should have its parameters evaluated</param>
        public void Add(string key, TbasicFunction value, bool evaluate)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Contract.EndContractBlock();

            lib.Add(key, new CallData(value, evaluate));
        }

        /// <summary>
        /// Adds a function to this dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, CallData value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            Contract.EndContractBlock();

            lib.Add(key, value);
        }

        /// <summary>
        /// Adds any delegate you might get your hands on
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="evaluate">whether or not this function should have its parameters evaluated</param>
        /// <param name="requiredArgs">the required number of arguments</param>
        public void AddDelegate(string key, Delegate value, bool evaluate = true, int requiredArgs = -1)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Contract.EndContractBlock();

            TbasicFunction func = value as TbasicFunction;
            if (func != null)
                Add(key, func);
            if (value.Method.ReturnType == typeof(void))
                lib.Add(key, new CallData(value, requiredArgs < 0 ? CountParameters(value) : requiredArgs, evaluate));
        }

        private static int CountParameters(Delegate d)
        {
            Contract.Ensures(Contract.Result<int>() >= 0);
            return d.Method.GetParameters().Length;
        }

        /// <summary>
        /// Determines if a key exists in this dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return lib.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection of this dictionary's keys
        /// </summary>
        public ICollection<string> Keys
        {
            get { return lib.Keys; }
        }

        /// <summary>
        /// Removes a key and its associated value from the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return lib.Remove(key);
        }

        /// <summary>
        /// Tries to get a value from this dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out CallData value)
        {
            return lib.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets a collection of this dictionary's values
        /// </summary>
        public ICollection<CallData> Values
        {
            get { return lib.Values; }
        }

        /// <summary>
        /// Gets or sets a function at a given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CallData this[string key]
        {
            get {
                return lib[key];
            }
            set {
                lib[key] = value;
            }
        }

        /// <summary>
        /// Clears all items from this dictionary
        /// </summary>
        public void Clear()
        {
            lib.Clear();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, CallData>> GetEnumerator()
        {
            return ((IDictionary<string, CallData>)lib).GetEnumerator();
        }

        /// <summary>
        /// Returns the number of elements in this collection
        /// </summary>
        public int Count
        {
            get { return lib.Count; }
        }
        
        bool IDictionary<string, CallData>.TryGetValue(string key, out CallData value)
        {
            return ((IDictionary<string, CallData>)lib).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, CallData>)lib).GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, CallData>>.IsReadOnly
        {
            get {
                return ((IDictionary<string, CallData>)lib).IsReadOnly;
            }
        }

        void ICollection<KeyValuePair<string, CallData>>.CopyTo(KeyValuePair<string, CallData>[] array, int arrayIndex)
        {
            ((IDictionary<string, CallData>)lib).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, CallData>>.Remove(KeyValuePair<string, CallData> item)
        {
            return ((IDictionary<string, CallData>)lib).Remove(item);
        }

        void ICollection<KeyValuePair<string, CallData>>.Add(KeyValuePair<string, CallData> item)
        {
            ((IDictionary<string, CallData>)lib).Add(item);
        }

        bool ICollection<KeyValuePair<string, CallData>>.Contains(KeyValuePair<string, CallData> item)
        {
            return ((IDictionary<string, CallData>)lib).Contains(item);
        }
    }
}
