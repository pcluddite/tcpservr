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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tbasic.Operators
{
    internal partial class UnaryOpDictionary : IDictionary<string, UnaryOperator>
    {
        public UnaryOperator this[string key]
        {
            get {
                return unaryOps[key];
            }
            set {
                unaryOps[key] = value;
            }
        }

        public int Count
        {
            get {
                return unaryOps.Count;
            }
        }

        public ICollection<string> Keys
        {
            get {
                return unaryOps.Keys;
            }
        }

        public ICollection<UnaryOperator> Values
        {
            get {
                return unaryOps.Values;
            }
        }

        public void Add(string key, UnaryOperator value)
        {
            unaryOps.Add(key, value);
        }

        public void Clear()
        {
            unaryOps.Clear();
        }

        public bool Contains(KeyValuePair<string, UnaryOperator> item)
        {
            return unaryOps.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return unaryOps.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, UnaryOperator>> GetEnumerator()
        {
            return unaryOps.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return unaryOps.Remove(key);
        }

        public bool TryGetValue(string key, out UnaryOperator value)
        {
            return unaryOps.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return unaryOps.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, UnaryOperator>>.IsReadOnly
        {
            get {
                return ((ICollection<KeyValuePair<string, UnaryOperator>>)unaryOps).IsReadOnly;
            }
        }

        void ICollection<KeyValuePair<string, UnaryOperator>>.Add(KeyValuePair<string, UnaryOperator> item)
        {
            ((ICollection<KeyValuePair<string, UnaryOperator>>)unaryOps).Add(item);
        }

        void ICollection<KeyValuePair<string, UnaryOperator>>.CopyTo(KeyValuePair<string, UnaryOperator>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, UnaryOperator>>)unaryOps).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, UnaryOperator>>.Remove(KeyValuePair<string, UnaryOperator> item)
        {
            return ((ICollection<KeyValuePair<string, UnaryOperator>>)unaryOps).Remove(item);
        }
    }
}
