// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tbasic.Errors;
using Tbasic.Types;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Class for evaluating a function
    /// </summary>
    internal class FunctionEvaluator : IExpressionEvaluator
    {
        #region Properties

        [ContractPublicPropertyName(nameof(Expression))]
        private string _name;

        [ContractPublicPropertyName(nameof(Parameters))]
        private IList<IEnumerable<char>> _params;
        /// <summary>
        /// Gets or sets the expression to be evaluated
        /// </summary>
        /// <value></value>
        public IEnumerable<char> Expression
        {
            get {
                return _name;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                Contract.EndContractBlock();
                _name = value.ToString();
            }
        }
        
        /// <summary>
        /// Gets or sets the context in which this function should be run. This is the global context by default.
        /// </summary>
        public ObjectContext CurrentContext { get; set; }

        public TRuntime Runtime { get; set; }

        public IList<IEnumerable<char>> Parameters
        {
            get {
                return _params;
            }
        }

        #endregion

        public FunctionEvaluator(TRuntime runtime, string name, IList<IEnumerable<char>> parameters)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            Contract.EndContractBlock();

            Runtime = runtime;
            CurrentContext = runtime.Context;
            _name = name;
            _params = parameters;
        }
        
        public object Evaluate()
        {
            return ExecuteFunction(_name, _params);
        }
        
        public override string ToString()
        {
            return Expression.ToString();
        }
        
        private object ExecuteFunction(string name, IList<IEnumerable<char>> l_params)
        {
            CallData func;
            if (CurrentContext.TryGetFunction(name, out func)) {
                StackData stackdat = new StackData(Runtime.Options, l_params.TB_ToStrings());
                stackdat.Name = name; // if this isn't before evaluation, EvaluateAll() won't eval properly 8/30/16
                if (func.ShouldEvaluate) {
                    stackdat.EvaluateAll(Runtime);
                }
                Runtime.ExecuteInContext(CurrentContext, func.Function, stackdat: stackdat);
                return stackdat.ReturnValue;
            }
            else {
                throw ThrowHelper.UndefinedFunction(name);
            }
        }
    }
}
