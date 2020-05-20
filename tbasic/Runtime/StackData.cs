// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Types;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Manages parameters and other data passed to a function or subroutine
    /// </summary>
    public partial class StackData : ICloneable
    {
        [ContractPublicPropertyName(nameof(Parameters))]
        private List<object> _params = new List<object>();

        /// <summary>
        /// Gets a list of the parameters passed to the function
        /// </summary>
        public IList<object> Parameters
        {
            get {
                return _params;
            }
        }

        /// <summary>
        /// Gets or sets the options for this StackData
        /// </summary>
        public ExecuterOption Options { get; set; }
        
        /// <summary>
        /// Gets the Tbasic function as text
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the name of the function (the first parameter)
        /// </summary>
        public string Name
        {
            get {
                if (_params.Count > 0) {
                    return (_params[0] ?? string.Empty).ToString();
                }
                else {
                    return string.Empty;
                }
            }
            set {
                if (_params.Count == 0) {
                    _params.Add(value);
                }
                else {
                    _params.Insert(0, value);
                }
            }
        }

        /// <summary>
        /// Gets the number of parameters in this collection
        /// </summary>
        public int ParameterCount
        {
            get {
                return _params.Count;
            }
        }
        
        /// <summary>
        /// Gets or sets the status that the function returned. Default is ErrorSuccess.OK
        /// </summary>
        public int Status { get; set; } = ErrorSuccess.OK;

        /// <summary>
        /// Gets or sets the return data for the function
        /// </summary>
        public object ReturnValue { get; set; } = null;

        /// <summary>
        /// Constructs this object
        /// </summary>
        /// <param name="options">the runtime options that are currently enforced</param>
        public StackData(ExecuterOption options)
        {
            Options = options;
        }

        /// <summary>
        /// Constructs this object
        /// </summary>
        /// <param name="parameters">the parameters of the function</param>
        /// <param name="options">the execution that called the function</param>
        public StackData(ExecuterOption options, IEnumerable<object> parameters)
            : this(options)
        {
            _params.AddRange(parameters);
        }

        /// <summary>
        /// Constructs this object
        /// </summary>
        /// <param name="text">the line that executed this function, this will be parsed like the Windows Command Prompt</param>
        /// <param name="runtime">the execution that called the function</param>
        public StackData(TRuntime runtime, string text)
            : this(runtime.Options)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();
            Statement line = new Statement(runtime.Scanner.Scan(text));
            Text = text;
            AddRange(line);
        }
        
        /// <summary>
        /// Throws an ArgumentException if the number of parameters does not match a specified count
        /// </summary>
        /// <param name="count">the number of parameters expected</param>
        /// <exception cref="ArgumentException">thrown if argument count is not the same as the parameter</exception>
        public void AssertCount(int count)
        {
            Contract.Requires(count >= 0);
            if (_params.Count != count) {
                throw new ArgumentException(string.Format("{0} does not take {1} parameter{2}", Name.ToUpper(), _params.Count - 1,
                _params.Count == 2 ? "" : "s"));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the number of parameters does not match a count in a specified range
        /// </summary>
        /// <param name="atLeast">the least number of arguments this function takes</param>
        /// <param name="atMost">the most number of arguments this function takes</param>
        /// <exception cref="ArgumentException">thrown if argument count is not the same as the parameter</exception>
        public void AssertCount(int atLeast, int atMost)
        {
            Contract.Requires(atLeast >= 0 && atMost >= 0);
            Contract.Requires(atLeast < atMost);
            if (_params.Count < atLeast || _params.Count > atMost) {
                throw new ArgumentException(string.Format("{0} does not take {1} parameter{2}", Name.ToUpper(), _params.Count - 1,
                _params.Count == 2 ? "" : "s"));
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the number of parameters is not at least a certain value
        /// </summary>
        /// <param name="atLeast">the least number of arguments this function takes</param>
        /// <exception cref="ArgumentException">thrown if argument count is not at least a certain number</exception>
        public void AssertAtLeast(int atLeast)
        {
            Contract.Requires(atLeast >= 0);
            if (_params.Count < atLeast) {
                throw new ArgumentException(string.Format("{0} must have at least {1} parameter{2}", Name.ToUpper(), atLeast - 1,
                atLeast == 2 ? "" : "s"));
            }
        }

        /// <summary>
        /// Returns the parameter at an index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException">thrown if the argument is out of range</exception>
        /// <returns></returns>
        public object Get(int index)
        {
            if ((uint)index >= (uint)_params.Count)
                throw new ArgumentOutOfRangeException();
            Contract.EndContractBlock();
            return _params[index];
        }

        /// <summary>
        /// Returns the parameter as an integer if the integer is within a given range.
        /// </summary>
        /// <param name="index">the index of the argument</param>
        /// <param name="lower">the inclusive lower bound</param>
        /// <param name="upper">the inclusive upper bound</param>
        /// <exception cref="ArgumentOutOfRangeException">thrown if the paramater is out of range</exception>
        /// <returns></returns>
        public int GetFromRange(int index, int lower, int upper)
        {
            Contract.Requires(lower >= 0 && upper >= 0);
            Contract.Requires(lower < upper);
            int n = Get<int>(index);
            if (n < lower || n > upper) {
                throw new ArgumentOutOfRangeException(string.Format("Parameter {0} expected to be integer between {1} and {2}", index, lower, upper));
            }
            return n;
        }

        /// <summary>
        /// Gets a parameter at a given index as a specified type
        /// </summary>
        /// <typeparam name="T">the type to convert the object</typeparam>
        /// <param name="index">the zero-based index of the parameter</param>
        /// <exception cref="InvalidCastException">object was not able to be converted to given type</exception>
        /// <returns></returns>
        public T Get<T>(int index)
        {
            T ret;
            if (TypeConvert.TryConvert(Get(index), out ret, Options))
                return ret;
            throw ThrowHelper.InvalidParamType(index, typeof(T).Name);
        }

        internal object Convert(int index, Type type)
        {
            object ret;
            if (TypeConvert.TryConvert(Get(index), type, out ret, Options))
                return ret;
            throw ThrowHelper.InvalidParamType(index, type.Name);
        }

        /// <summary>
        /// Gets an argument from a set of predefined values
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="typeName">The type the argument represents</param>
        /// <param name="values">Acceptable string values</param>
        /// <exception cref="ArgumentException">thrown if the argument is not in the acceptable list of values</exception>
        /// <returns></returns>
        public string GetEnum(int index, string typeName, params string[] values)
        {
            string arg = Get<string>(index);
            foreach (string val in values) {
                if (val.EqualsIgnoreCase(arg)) {
                    return arg;
                }
            }
            throw ThrowHelper.InvalidParamType(index, typeName);
        }
                
        /// <summary>
        /// Forces the (re-)evaluation of a string parameter. This is useful for statements, whose parameters don't get evaluated automatically.
        /// </summary>
        public object Evaluate(int index, TRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            string param = Get(index) as string;
            if (param != null)
                _params[index] = ExpressionEvaluator.Evaluate(param, runtime);
            return _params[index];
        }
        
        /// <summary>
        /// Forces the (re-)evaluation of a string parameter. This is useful for statements, whose parameters don't get evaluated automatically. This will replace the old parameter value on success.
        /// </summary>
        public T Evaluate<T>(int index, TRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            string param = Get(index) as string;
            if (param != null)
                _params[index] = ExpressionEvaluator.Evaluate(param, runtime);
            return Get<T>(index);
        }

        /// <summary>
        /// Forces the (re-)evaluation of all prarameters excluding the name. If the parameter is not a string, it's value is kept. This is useful for statements, whose parameters don't get evaluated automatically.
        /// </summary>
        public void EvaluateAll(TRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            ExpressionEvaluator eval = new ExpressionEvaluator(runtime);
            for (int index = 1; index < _params.Count; ++index) {
                string param = _params[index] as string;
                if (param != null) {
                    _params[index] = ExpressionEvaluator.Evaluate(param, runtime);
                }
            }
        }

        /// <summary>
        /// Clones this
        /// </summary>
        /// <returns>A new object with the same data</returns>
        public StackData Clone()
        {
            StackData clone = new StackData(Options);
            clone.Text = Text;
            if (_params == null) {
                clone._params = new List<object>();
            }
            else {
                clone._params.AddRange(_params);
            }
            clone.Status = Status;
            clone.ReturnValue = ReturnValue;
            return clone;
        }

        /// <summary>
        /// Copies all properties of another into this one
        /// </summary>
        /// <param name="other"></param>
        public void CopyFrom(StackData other)
        {
            StackData clone = other.Clone();
            Options = clone.Options;
            Text = clone.Text;
            _params = clone._params;
            Status = clone.Status;
            ReturnValue = clone.ReturnValue;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
