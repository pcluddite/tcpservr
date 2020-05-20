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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Operators
{
    internal partial class BinOpDictionary : IDictionary<string, BinaryOperator>
    {
        public BinaryOperator this[string key]
        {
            get {
                return binaryOps[key];
            }
            set {
                binaryOps[key] = value;
            }
        }

        public int Count
        {
            get {
                return binaryOps.Count;
            }
        }

        public ICollection<string> Keys
        {
            get {
                return binaryOps.Keys;
            }
        }

        public ICollection<BinaryOperator> Values
        {
            get {
                return binaryOps.Values;
            }
        }

        public void Add(string key, BinaryOperator value)
        {
            binaryOps.Add(key, value);
        }

        public void Clear()
        {
            binaryOps.Clear();
        }

        public bool Contains(KeyValuePair<string, BinaryOperator> item)
        {
            return binaryOps.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return binaryOps.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, BinaryOperator>> GetEnumerator()
        {
            return binaryOps.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return binaryOps.Remove(key);
        }

        public bool TryGetValue(string key, out BinaryOperator value)
        {
            return binaryOps.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return binaryOps.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, BinaryOperator>>.IsReadOnly
        {
            get {
                return ((ICollection<KeyValuePair<string, BinaryOperator>>)binaryOps).IsReadOnly;
            }
        }

        void ICollection<KeyValuePair<string, BinaryOperator>>.Add(KeyValuePair<string, BinaryOperator> item)
        {
            ((ICollection<KeyValuePair<string, BinaryOperator>>)binaryOps).Add(item);
        }

        void ICollection<KeyValuePair<string, BinaryOperator>>.CopyTo(KeyValuePair<string, BinaryOperator>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, BinaryOperator>>)binaryOps).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, BinaryOperator>>.Remove(KeyValuePair<string, BinaryOperator> item)
        {
            return ((ICollection<KeyValuePair<string, BinaryOperator>>)binaryOps).Remove(item);
        }
    }
}
