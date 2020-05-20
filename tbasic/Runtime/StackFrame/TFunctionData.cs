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
using System.Text;
using Tbasic.Errors;
using Tbasic.Parsing;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Manages parameters and other data passed to a function or subroutine
    /// </summary>
    public class TFunctionData : ICloneable
    {
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
        /// The executer that called the function
        /// </summary>
        public Executer StackExecuter { get; private set; }

        /// <summary>
        /// Gets or sets the current context of stack executer
        /// </summary>
        public ObjectContext Context
        {
            get {
                return StackExecuter.Context;
            }
            set {
                StackExecuter.Context = value;
            }
        }

        /// <summary>
        /// The Tbasic function as text. This value cannot be changed after it has been set in the constructor.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The name of the function (the first parameter)
        /// </summary>
        public string Name
        {
            get {
                if (_params.Count > 0) {
                    return _params[0].ToString();
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
        public object Data { get; set; } = null;

        /// <summary>
        /// Constructs a StackFrame object
        /// </summary>
        /// <param name="exec">the execution that called the function</param>
        public TFunctionData(Executer exec)
        {
            StackExecuter = exec;
        }

        /// <summary>
        /// Constructs a StackFrame object
        /// </summary>
        /// <param name="text">the text to be processed (formatted as a shell command)</param>
        /// <param name="exec">the execution that called the function</param>
        public TFunctionData(Executer exec, string text)
            : this(exec)
        {
            SetAll(text);
        }

        /// <summary>
        /// Sets the data for this StackFrame object
        /// </summary>
        /// <param name="parameters">parameters to manage</param>
        public void SetAll(params object[] parameters)
        {
            if (parameters == null) {
                _params = new List<object>();
            }
            else {
                _params = new List<object>(parameters);
            }
        }

        internal void SetAll(List<object> parameters)
        {
            if (parameters == null) {
                _params = new List<object>();
            }
            else {
                _params = new List<object>(parameters);
            }
        }

        /// <summary>
        ///  Sets the data for this StackFrame object
        /// </summary>
        /// <param name="message">the text to be processed (formatted as a shell command)</param>
        public void SetAll(string message)
        {
            Text = message;
            _params = new List<object>(ParseArguments(message));
        }

        private string[] ParseArguments(string commandLine)
        {
            commandLine = commandLine.Trim(); // just a precaution
            List<string> args = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < commandLine.Length; index++) {
                switch (commandLine[index]) {
                    case ' ':
                        if (index > 0 && commandLine[index - 1] == ' ') {
                            continue; // ignore extra whitespace
                        }
                        else {
                            args.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        break;
                    case '\'':
                    case '"':
                        string parsed;
                        index = GroupParser.ReadString(commandLine, index, out parsed);
                        if (parsed.Length == 0) {
                            args.Add(parsed);
                        }
                        else {
                            sb.Append(parsed);
                        }
                        break;
                    default:
                        sb.Append(commandLine[index]);
                        break;
                }
            }
            if (sb.Length > 0) {
                args.Add(sb.ToString()); // Don't forget about me!
            }
            return args.ToArray();
        }

        /// <summary>
        /// Assigns new data to a parameter
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="data">The new string data to assign</param>
        public void SetParameter(int index, object data)
        {
            if (index < _params.Count) {
                _params[index] = data;
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the number of parameters does not match a specified count
        /// </summary>
        /// <param name="count">the number of parameters expected</param>
        /// <exception cref="ArgumentException">thrown if argument count is not the same as the parameter</exception>
        public void AssertParamCount(int count)
        {
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
        public void AssertParamCount(int atLeast, int atMost)
        {
            if (_params.Count < atLeast || _params.Count > atMost) {
                throw new ArgumentException(string.Format("{0} does not take {1} parameter{2}", Name.ToUpper(), _params.Count - 1,
                _params.Count == 2 ? "" : "s"));
            }
        }

        /// <summary>
        /// Returns the parameter at an index
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <exception cref="IndexOutOfRangeException">thrown if the argument is out of range</exception>
        /// <returns></returns>
        public object GetParameter(int index)
        {
            try {
                return _params[index];
            }
            catch(ArgumentOutOfRangeException ex) {
                throw new IndexOutOfRangeException(ex.Message);
            }
        }

        /// <summary>
        /// Returns the parameter as an integer if the integer is within a given range.
        /// </summary>
        /// <param name="index">the index of the argument</param>
        /// <param name="lower">the inclusive lower bound</param>
        /// <param name="upper">the inclusive upper bound</param>
        /// <exception cref="ArgumentOutOfRangeException">thrown if the paramater is out of range</exception>
        /// <returns></returns>
        public int GetFromIntRange(int index, int lower, int upper)
        {
            int n = GetParameter<int>(index);
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
        public T GetParameter<T>(int index)
        {
            T ret;
            if (Evaluator.TryParse(GetParameter(index), out ret)) {
                return ret;
            }
            throw new InvalidCastException(string.Format("Expected parameter {0} to be of type {1}", index, typeof(T).Name));
        }

        /// <summary>
        /// Gets an argument from a set of predefined values
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="typeName">The type the argument represents</param>
        /// <param name="values">Acceptable string values</param>
        /// <exception cref="ArgumentException">thrown if the argument is not in the acceptable list of values</exception>
        /// <returns></returns>
        public string GetFromEnum(int index, string typeName, params string[] values)
        {
            string arg = GetParameter<string>(index);
            foreach (string val in values) {
                if (val.EqualsIgnoreCase(arg)) {
                    return arg;
                }
            }
            throw new ArgumentException("expected parameter " + index + " to be of type " + typeName);
        }

        /// <summary>
        /// Adds a parameter to the end of this collection
        /// </summary>
        /// <param name="param"></param>
        public void AddParameter(object param)
        {
            _params.Add(param);
        }

        /// <summary>
        /// Clones this StackFrame
        /// </summary>
        /// <returns>A new StackFrame object with the same data</returns>
        public TFunctionData Clone()
        {
            TFunctionData clone = new TFunctionData(StackExecuter);
            clone.Text = Text;
            if (_params == null) {
                clone._params = new List<object>();
            }
            else {
                clone._params.AddRange(_params);
            }
            clone.Status = Status;
            clone.Data = Data;
            return clone;
        }

        /// <summary>
        /// Copies all properties of another StackFrame into this one
        /// </summary>
        /// <param name="other"></param>
        public void CopyFrom(TFunctionData other)
        {
            TFunctionData clone = other.Clone();
            StackExecuter = clone.StackExecuter;
            Text = clone.Text;
            _params = clone._params;
            Status = clone.Status;
            Data = clone.Data;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
