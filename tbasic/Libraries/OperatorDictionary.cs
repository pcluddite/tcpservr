// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Tbasic.Types
{
    internal abstract class OperatorDictionary<T> : IList<T>
        where T : IOperator
    {
        protected List<T> operators;

        protected OperatorDictionary(int capacity = 10)
        {
            Contract.Requires(capacity >= 0);
            operators = new List<T>(capacity);
        }

        protected OperatorDictionary(OperatorDictionary<T> other)
        {
            operators = new List<T>(other.operators);
        }

        public abstract void LoadStandardOperators();

        public T this[int index]
        {
            get {
                return operators[index];
            }
            set {
                operators[index] = value;
            }
        }

        public int Count
        {
            get {
                return operators.Count;
            }
        }

        public void Clear()
        {
            operators.Clear();
        }

        public int IndexOf(T item)
        {
            return operators.IndexOf(item);
        }

        public int IndexOf(string op)
        {
            for (int i = 0; i < operators.Count; ++i) {
                if (operators[i].OperatorString == op) {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            operators.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            operators.RemoveAt(index);
        }

        public void Add(T item)
        {
            operators.Add(item);
        }

        public bool Contains(T item)
        {
            return operators.Contains(item);
        }

        public bool Contains(string key)
        {
            return IndexOf(key) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            operators.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return operators.Remove(item);
        }

        public void Remove(string key)
        {
            operators.RemoveAt(IndexOf(key));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return operators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return operators.GetEnumerator();
        }

        bool ICollection<T>.IsReadOnly
        {
            get {
                return ((IList<T>)operators).IsReadOnly;
            }
        }
    }
}
