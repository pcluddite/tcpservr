// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tbasic.Types;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Manages parameters and other data passed to a function or subroutine
    /// </summary>
    public partial class StackData : ICloneable
    {
        /// <summary>
        /// Adds a parameter to the end of the parameter list
        /// </summary>
        /// <param name="value">the Number to add</param>
        public void Add(Number value)
        {
            _params.Add( value);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<Number> collection)
        {
            foreach(Number value in collection) {
                _params.Add(value);
            }
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(params Number[] collection)
        {
            foreach(Number value in collection) {
                _params.Add(value);
            }
        }

        /// <summary>
        /// Assigns a new value to a parameter at a given index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">the new Number data to assign</param>
        public void Set(int index, Number value)
        {
            if ((uint)index >= (uint)_params.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            Contract.EndContractBlock();
            _params[index] =  value;
        }

        /// <summary>
        /// Adds a parameter to the end of the parameter list
        /// </summary>
        /// <param name="value">the bool to add</param>
        public void Add(bool value)
        {
            _params.Add( value);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<bool> collection)
        {
            foreach(bool value in collection) {
                _params.Add(value);
            }
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(params bool[] collection)
        {
            foreach(bool value in collection) {
                _params.Add(value);
            }
        }

        /// <summary>
        /// Assigns a new value to a parameter at a given index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">the new bool data to assign</param>
        public void Set(int index, bool value)
        {
            if ((uint)index >= (uint)_params.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            Contract.EndContractBlock();
            _params[index] =  value;
        }

        /// <summary>
        /// Adds a parameter to the end of the parameter list
        /// </summary>
        /// <param name="value">the object to add</param>
        public void Add(object value)
        {
            _params.Add(value);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<object> collection)
        {
            _params.AddRange(collection);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(params object[] collection)
        {
            _params.AddRange(collection);
        }

        /// <summary>
        /// Assigns a new value to a parameter at a given index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">the new object data to assign</param>
        public void Set(int index, object value)
        {
            if ((uint)index >= (uint)_params.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            Contract.EndContractBlock();
            _params[index] = value;
        }

        /// <summary>
        /// Adds a parameter to the end of the parameter list
        /// </summary>
        /// <param name="value">the Enum to add</param>
        public void Add(Enum value)
        {
            _params.Add(value);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<Enum> collection)
        {
            _params.AddRange(collection);
        }

        /// <summary>
        /// Adds a number of parameters to this collection
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(params Enum[] collection)
        {
            _params.AddRange(collection);
        }

        /// <summary>
        /// Assigns a new value to a parameter at a given index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="value">the new Enum data to assign</param>
        public void Set(int index, Enum value)
        {
            if ((uint)index >= (uint)_params.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            Contract.EndContractBlock();
            _params[index] = value;
        }
    }
}
