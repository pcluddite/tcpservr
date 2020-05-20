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
using Tbasic.Libraries;
using Tbasic.Operators;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Manages the variables, functions, and commands declared in a given context
    /// </summary>
    public class ObjectContext
    {
        #region Private Fields

        private ObjectContext _super;
        private Dictionary<string, object> _variables;
        private Dictionary<string, object> _constants;
        private Dictionary<string, BlockCreator> _blocks;
        internal BinOpDictionary _binaryOps; // this is internal so it can be accessed by the evalutator
        internal UnaryOpDictionary _unaryOps; // ^^^ dito
        private Library _functions;
        private Library _commands;

        private bool _bCollected = false;

        #endregion

        #region Constructors

        internal ObjectContext(ObjectContext superContext)
        {
            _super = superContext;
            _functions = new Library();
            _commands = new Library();
            _variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _constants = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _blocks = new Dictionary<string, BlockCreator>(StringComparer.OrdinalIgnoreCase);
            if (superContext == null) {
                _binaryOps = new BinOpDictionary();
                _unaryOps = new UnaryOpDictionary();
            }
            else {
                _binaryOps = superContext._binaryOps; // binary operators are always global (for now)
                _unaryOps = superContext._unaryOps; // ^^^ dito
            }
#if SHOW_OBJECTS
            Console.WriteLine();
            Console.WriteLine("ObjectContext initialized.");
            Console.WriteLine("\tHash:\t{0}", GetHashCode());
            if (_super != null) {
                Console.WriteLine("\tSuper Context:\t{0}", _super.GetHashCode());
            }
#endif
        }

        /// <summary>
        /// Loads the standard library of functions, statements, constants and code blocks
        /// </summary>
        public void LoadStandardLibrary()
        {
            _functions = new Library(
                new Library[] { 
                    new MathLibrary(this),
                    new RuntimeLibrary(this),
                    new UserIOLibrary(),
                    new AutoLibrary(),
                    new FileIOLibrary(),
                    new ProcessLibrary(),
                    new WindowLibrary(this),
                    new RegistryLibrary(),
                    new SystemLibrary()
                });
            _commands = new Library(
                new Library[] {
                    new StatementLibrary()
                });
            _blocks = new Dictionary<string, BlockCreator>(StringComparer.OrdinalIgnoreCase) {
                { "DO"    , (i,c) => new DoBlock(i,c) },
                { "WHILE" , (i,c) => new WhileBlock(i,c) },
                { "IF"    , (i,c) => new IfBlock(i,c) },
                { "SELECT", (i,c) => new SelectBlock(i,c) }
            };
            _binaryOps.LoadStandardOperators();
            _unaryOps.LoadStandardOperators();
        }

        /// <summary>
        /// Adds a library that should be parsed as functions to this context
        /// </summary>
        /// <param name="lib">the library to add</param>
        public void AddLibrary(Library lib)
        {
            _functions.AddLibrary(lib);
        }

        /// <summary>
        /// Adds a library that should be parsed as commands to this context
        /// </summary>
        /// <param name="lib">the library to add</param>
        public void AddCommandLibrary(Library lib)
        {
            _commands.AddLibrary(lib);
        }

        #endregion

        #region CreateSubContext

        /// <summary>
        /// Creates a sub-context nested in this one. The sub-context inherits all variables, functions, and commands of declared in this one.
        /// </summary>
        /// <returns>the new sub-context</returns>
        public ObjectContext CreateSubContext()
        {
            if (_bCollected) {
                throw ThrowHelper.ContextCleared();
            }
            return new ObjectContext(this); // They're not allowed to do this themselves so it won't be null
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Clears all variables, functions, and commands in this context and returns its super-context. If
        /// no super-context exists, the object will not be collected, and the same object is returned.
        /// </summary>
        /// <returns>the super context of this context</returns>
        public ObjectContext Collect()
        {
            if (_bCollected) {
                throw ThrowHelper.ContextCleared();
            }
            else if (_super == null) {
                return this; // You won't ever get rid of me
            }
            _bCollected = true;
#if SHOW_OBJECTS
            foreach (var v in _variables) {
                Console.WriteLine("{0} = {1} collected", v.Key, v.Value);
            }
            foreach (var v in _functions) {
                Console.WriteLine("{0} function collected", v.Key);
            }
            foreach (var v in _commands) {
                Console.WriteLine("{0} command collected", v.Key);
            }
            Console.WriteLine("{0} ObjectContext collected", GetHashCode());
#endif
            _commands.Clear();
            _functions.Clear();
            _variables.Clear(); // YOU'RE FREE!!!!!
            return _super;
        }

        #endregion

        #region FindContext

        /// <summary>
        /// Searches for the context in which a code block is declared. If the block cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the block name</param>
        /// <returns>the ObjectContext in which the block is declared</returns>
        public ObjectContext FindBlockContext(string name)
        {
            if (_blocks.ContainsKey(name)) {
                return this;
            }
            else if (_super == null) {
                return null;
            }
            else {
                return _super.FindBlockContext(name);
            }
        }

        /// <summary>
        /// Searches for the context in which a command is declared. If the command cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the command name</param>
        /// <returns>the ObjectContext in which the command is declared</returns>
        public ObjectContext FindCommandContext(string name)
        {
            if (_commands.ContainsKey(name)) {
                return this;
            }
            else if (_super == null) {
                return null;
            }
            else {
                return _super.FindCommandContext(name);
            }
        }

        /// <summary>
        /// Searches for the context in which a function is declared. If the function cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the function name</param>
        /// <returns>the ObjectContext in which the function is declared</returns>
        public ObjectContext FindFunctionContext(string name)
        {
            if (_functions.ContainsKey(name)) {
                return this;
            }
            else if (_super == null) {
                return null;
            }
            else {
                return _super.FindFunctionContext(name);
            }
        }

        /// <summary>
        /// Searches for the context in which a variable is declared. If the variable cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <returns>the ObjectContext in which the variable is declared</returns>
        public ObjectContext FindVariableContext(string name)
        {
            if (_variables.ContainsKey(name)) {
                return this;
            }
            else if (_super == null) {
                return null;
            }
            else {
                return _super.FindVariableContext(name);
            }
        }

        /// <summary>
        /// Searches for the context in which any name is declared (variable, constant, function, command or block). If no definition cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <returns>the ObjectContext in which the name is defined</returns>
        public ObjectContext FindContext(string name)
        {
            ObjectContext context = FindVariableContext(name);
            if (context != null) {
                return context;
            }
            context = FindConstantContext(name);
            if (context != null) {
                return context;
            }
            context = FindFunctionContext(name);
            if (context != null) {
                return context;
            }
            context = FindCommandContext(name);
            if (context != null) {
                return context;
            }
            return FindBlockContext(name);
        }

        /// <summary>
        /// Searches for the context in which a constant is declared. If the constant cannot be found, null is returned.
        /// </summary>
        /// <param name="name">the constant name</param>
        /// <returns>the ObjectContext in which the constant is declared</returns>
        public ObjectContext FindConstantContext(string name)
        {
            if (_constants.ContainsKey(name)) {
                return this;
            }
            else if (_super == null) {
                return null;
            }
            else {
                return _super.FindConstantContext(name);
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Gets a code block that has been declared in this context
        /// </summary>
        /// <param name="name">block name</param>
        /// <returns>the code block</returns>
        public BlockCreator GetBlock(string name)
        {
            BlockCreator block;
            if (_blocks.TryGetValue(name, out block)) {
                return block;
            }
            else if (_super == null) {
                throw ThrowHelper.UndefinedObject(name);
            }
            else {
                return _super.GetBlock(name);
            }
        }

        /// <summary>
        /// Gets a variable or constant that has been declared in this context
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns>the variable data</returns>
        public object GetVariable(string name)
        {
            object value;
            if (_constants.TryGetValue(name, out value)) {
                return value;
            }
            else if (_variables.TryGetValue(name, out value)) {
                return value;
            }
            else if (_super == null) {
                throw ThrowHelper.UndefinedObject(name);
            }
            else {
                return _super.GetVariable(name);
            }
        }

        /// <summary>
        /// Gets the value of an array variable at a given index
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <param name="indices">the index (or indices of a multidimensional array)</param>
        /// <returns>the value of the variable</returns>
        public object GetArrayAt(string name, params int[] indices)
        {
            object obj = GetVariable(name);
            if (indices != null && indices.Length > 0) {
                for (int n = 0; n < indices.Length; n++) {
                    object[] _aObj = obj as object[];
                    if (_aObj != null) {
                        if (indices[n] < _aObj.Length) {
                            obj = _aObj[indices[n]];
                        }
                        else {
                            throw ThrowHelper.IndexOutOfRange(Variable.GetFullName(name, indices), indices[n]);
                        }
                    }
                    else {
                        throw ThrowHelper.IndexUnavailable(Variable.GetFullName(name, indices));
                    }
                }
            }
            return obj;
        }
        
        /// <summary>
        /// Gets a function that has been declared in this context
        /// </summary>
        /// <param name="name">function name</param>
        /// <returns>the function delegate</returns>
        public TBasicFunction GetFunction(string name)
        {
            TBasicFunction func;
            if (_functions.TryGetValue(name, out func)) {
                return func;
            }
            else if (_super == null) {
                throw ThrowHelper.UndefinedObject(name);
            }
            else {
                return _super.GetFunction(name);
            }
        }

        /// <summary>
        /// Gets a command that has been declared in this context
        /// </summary>
        /// <param name="name">command name</param>
        /// <returns>the command delegate</returns>
        public TBasicFunction GetCommand(string name)
        {
            TBasicFunction func;
            if (_commands.TryGetValue(name, out func)) {
                return func;
            }
            else if (_super == null) {
                throw ThrowHelper.UndefinedObject(name);
            }
            else {
                return _super.GetCommand(name);
            }
        }

        /// <summary>
        /// Gets a binary operator if it exists, throws an ArgumentException otherwise
        /// </summary>
        /// <param name="strOp">the operator as a string</param>
        /// <exception cref="ArgumentException">thrown if the operator is undefined</exception>
        /// <returns></returns>
        public BinaryOperator GetBinaryOperator(string strOp)
        {
            BinaryOperator op;
            if (_binaryOps.TryGetValue(strOp, out op)) {
                return op;
            }
            else {
                throw ThrowHelper.OperatorUndefined(strOp);
            }
        }

        /// <summary>
        /// Gets a binary operator if it exists, throws an ArgumentException otherwise
        /// </summary>
        /// <param name="strOp">the operator as a string</param>
        /// <exception cref="ArgumentException">thrown if the operator is undefined</exception>
        /// <returns></returns>
        public UnaryOperator GetUnaryOperator(string strOp)
        {
            UnaryOperator op;
            if (_unaryOps.TryGetValue(strOp, out op)) {
                return op;
            }
            else {
                throw ThrowHelper.OperatorUndefined(strOp);
            }
        }

        #endregion

        #region Set

        internal void SetReturns(TFunctionData _sframe)
        {
            SetVariable("@lasterror", _sframe.Status);
            SetVariable("@lasterr", _sframe.Status);
            SetVariable("@err", _sframe.Status);
            SetVariable("@error", _sframe.Status);
        }

        internal void PersistReturns(TFunctionData _sframe)
        {
            ObjectContext context = FindVariableContext("@error");
            if (context != null)
                _sframe.Status = (int)context.GetVariable("@error");
        }

        /// <summary>
        /// Sets a code block in this context. If the block exists, it is set in
        /// the context in which it was declared. Otherwise, it is declared in this context.
        /// </summary>
        /// <param name="name">the constant name</param>
        /// <param name="block">a method that can be called to initialize the block</param>
        public void SetBlock(string name, BlockCreator block)
        {
            ObjectContext c = FindBlockContext(name);
            if (c == null) {
                _blocks.Add(name, block);
#if SHOW_OBJECTS
                Console.WriteLine("{1} declared in {0}", GetHashCode(), name);
#endif
            }
            else {
                c._blocks[name] = block;
#if SHOW_OBJECTS
                Console.WriteLine("{1} set in {0}", c.GetHashCode(), name);
#endif
            }
        }

        /// <summary>
        /// Sets a constant that will be declared in this context. Once a constant is set, it cannot be changed.
        /// </summary>
        /// <param name="name">the constant name</param>
        /// <param name="obj">the object data</param>
        public void SetConstant(string name, object obj)
        {
            ObjectContext context = FindVariableContext(name);
            if (context == null) {
                context = FindConstantContext(name);
                if (context == null) {
                    _constants.Add(name, obj); // since you can set constants only once, we don't need to use the context in which a constant was set
                }
                else {
                    throw ThrowHelper.ConstantChange();
                }
            }
            else {
                throw ThrowHelper.AlreadyDefinedAsType(name, "variable", "constant");
            }
        }

        /// <summary>
        /// Sets a variable in this context. If the variable exists, it is set in
        /// the context in which it was declared. Otherwise, it is declared in this context.
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <param name="obj">the object data</param>
        public void SetVariable(string name, object obj)
        {
            ObjectContext c = FindConstantContext(name);
            if (c != null) {
                throw ThrowHelper.ConstantChange();
            }
            c = FindVariableContext(name);
            if (c == null) {
                _variables.Add(name, obj);
#if SHOW_OBJECTS
                Console.WriteLine("{1} = {2} declared in {0}", GetHashCode(), name, obj);
#endif
            }
            else {
                c._variables[name] = obj;
#if SHOW_OBJECTS
                Console.WriteLine("{1} = {2} set in {0}", c.GetHashCode(), name, obj);
#endif
            }
        }
        
        /// <summary>
        /// Sets the value of an array variable at a given index
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <param name="value">the new value of the array at that index</param>
        /// <param name="indices">the index (or indices of a multidimensional array)</param>
        /// <returns>the value of the variable</returns>
        public void SetArrayAt(string name, object value, params int[] indices)
        {
            object[] array = GetVariable(name) as object[];
            if (indices != null && indices.Length > 0) {
                for (int n = 0; n < indices.Length - 1; ++n) {
                    if (array != null) {
                        if (indices[n] < array.Length) {
                            array = array[indices[n]] as object[];
                        }
                        else {
                            throw ThrowHelper.IndexOutOfRange(Variable.GetFullName(name, indices), indices[n]);
                        }
                    }
                    else {
                        throw ThrowHelper.IndexUnavailable(Variable.GetFullName(name, indices));
                    }
                }
            }
            array[indices[indices.Length - 1]] = value;
        }

        /// <summary>
        /// Sets a function in this context. If the function exists, it is set in
        /// the context in which it was declared. Otherwise, it is declared in this context.
        /// </summary>
        /// <param name="name">the function name</param>
        /// <param name="func">the delegate to the function</param>
        public void SetFunction(string name, TBasicFunction func)
        {
            ObjectContext c = FindFunctionContext(name);
            if (c == null) {
                _functions.Add(name, func);
#if SHOW_OBJECTS
                Console.WriteLine("{1} function declared in {0}", GetHashCode(), name);
#endif
            }
            else {
                c._functions[name] = func;
#if SHOW_OBJECTS
                Console.WriteLine("{1} function declared in {0}", c.GetHashCode(), name);
#endif
            }
        }

        /// <summary>
        /// Sets a command in this context. If the command exists, it is set in
        /// the context in which it was declared. Otherwise, it is declared in this context.
        /// </summary>
        /// <param name="name">the command name</param>
        /// <param name="func">the delegate to the command</param>
        public void SetCommand(string name, TBasicFunction func)
        {
            ObjectContext c = FindCommandContext(name);
            if (c == null) {
                _commands.Add(name, func);
#if SHOW_OBJECTS
                Console.WriteLine("{1} command declared in {1}", GetHashCode(), name);
#endif
            }
            else {
                c._commands[name] = func;
#if SHOW_OBJECTS
                Console.WriteLine("{0} command set in {1}", c.GetHashCode(), name);
#endif
            }
        }

        /// <summary>
        /// Defines a binary operator
        /// </summary>
        /// <param name="op">the operator</param>
        public void SetBinaryOperator(BinaryOperator op)
        {
            _binaryOps[op.OperatorString] = op;
        }

        /// <summary>
        /// Defines a unary operator
        /// </summary>
        /// <param name="op">the operator</param>
        public void SetUnaryOperator(UnaryOperator op)
        {
            _unaryOps[op.OperatorString] = op;
        }
        #endregion
    }
}
