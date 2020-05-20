// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Tbasic.Errors;
using Tbasic.Runtime;

namespace Tbasic.Types
{
    internal class UnaryOpDictionary : OperatorDictionary<UnaryOperator>
    {
        public UnaryOpDictionary()
        {
        }

        public UnaryOpDictionary(UnaryOpDictionary other)
            : base(other)
        {
        }

        public override void LoadStandardOperators()
        {
            operators.Add(new UnaryOperator("NEW", New, evaluate: false));
            operators.Add(new UnaryOperator("+", Plus));
            operators.Add(new UnaryOperator("-", Minus));
            operators.Add(new UnaryOperator("NOT", Not));
            operators.Add(new UnaryOperator("~", BitNot));
        }

        private static object New(TRuntime runtime, object value)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            FunctionEvaluator eval = value as FunctionEvaluator;
            if (eval == null)
                throw ThrowHelper.InvalidTypeInExpression(value?.GetType().Name, "function");
            
            TClass prototype;
            if (!runtime.Context.TryGetClass(eval.Expression.ToString(), out prototype))
                throw new UndefinedObjectException($"The class {eval.Expression} is undefined");

            StackData stackdat = new StackData(runtime.Options, eval.Parameters.TB_ToStrings());
            stackdat.Name = eval.Expression.ToString();
            stackdat.EvaluateAll(runtime);
            return prototype.GetInstance(runtime, stackdat);
        }

        private static object Plus(TRuntime runtime, object value)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return +Number.Convert(value, runtime.Options);
        }

        private static object Minus(TRuntime runtime, object value)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return -Number.Convert(value, runtime.Options);
        }

        private static object Not(TRuntime runtime, object value)
        {
            return !Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }

        private static object BitNot(TRuntime runtime, object value)
        {
            return ~Convert.ToUInt64(value, CultureInfo.InvariantCulture);
        }
    }
}
