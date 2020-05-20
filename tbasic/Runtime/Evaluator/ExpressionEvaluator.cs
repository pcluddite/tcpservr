// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Types;

namespace Tbasic.Runtime
{
    /// <summary>
    /// General purpose evaluator for functions, variables, math, booleans, etc.
    /// </summary>
    internal partial class ExpressionEvaluator : IExpressionEvaluator
    {
        private static readonly object[] EmptyObjectArray = new object[0];

        private LinkedList<LinkedList<object>> subexpressions = null;
        private IEnumerable<char> _expression = string.Empty;
        
        public ExpressionEvaluator(TRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();
            Runtime = runtime;
        }
        
        public ExpressionEvaluator(IEnumerable<char> expression, TRuntime runtime)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();
            Runtime = runtime;
            Expression = expression;
        }

        #region Properties
        
        public IEnumerable<char> Expression
        {
            get { return _expression; }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                Contract.EndContractBlock();
                _expression = value;
                subexpressions = null;
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return Runtime.Context;
            }
            set {
                throw new NotImplementedException(); // this shouldn't be set
            }
        }

        public TRuntime Runtime { get; set; }

        public bool Parsed
        {
            get {
                return (subexpressions != null);
            }
            set {
                if (Parsed && !value) {
                    subexpressions = null;
                }
            }
        }

        #endregion

        public object Evaluate(IEnumerable<char> expr)
        {
            if (expr == null)
                throw new ArgumentNullException(nameof(expr));
            Contract.EndContractBlock();
            Expression = expr;
            return Evaluate();
        }

        public object Evaluate()
        {
            object[] results = EvaluateAll();
            if (results.Length == 1) {
                return results[0];
            }
            else {
                return results;
            }
        }

        private object[] EvaluateAll()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            if (Expression == null || Expression.ToString() == string.Empty) {
                return EmptyObjectArray;
            }
            else {
                List<object> lResults = new List<object>();

                if (!Parsed) {
                    subexpressions = new LinkedList<LinkedList<object>>();

                    IScanner scanner = Runtime.Scanner.Scan(_expression);
                    LinkedList<object> objects = new LinkedList<object>();

                    while (!scanner.EndOfStream) {
                        if (!NextToken(objects, scanner)) { // expression break time
                            subexpressions.AddLast(objects);
                            objects = new LinkedList<object>();
                        }
                    }
                    subexpressions.AddLast(objects);
                }

                foreach(LinkedList<object> objects in subexpressions) {
                    lResults.Add(ConvertToSimpleType(EvaluateList(objects), Runtime.Options));
                }

                return lResults.ToArray();
            }
        }

        public bool EvaluateBool()
        {
            return Convert.ToBoolean(Evaluate());
        }
        
        private bool NextToken(LinkedList<object> objects, IScanner scanner)
        {
            int startIndex = scanner.Position;
            object value;
            TokenType type = scanner.NextToken(Runtime, out value, objects.Last?.Value);
            switch(type) {
                case TokenType.Comment:
                    return false;
                case TokenType.ExpressionBreak:
                    return false;
                case TokenType.Undefined: // couldn't be parsed
                    if (CurrentContext.FindFunctionContext(_expression.ToString()) == null) {
                        throw new InvalidTokenException(scanner.Next()?.ToString());
                    }
                    else {
                        throw new FormatException("Poorly formed function call");
                    }
            }
            AddObjectToExprList(value, startIndex, scanner, objects);
            return true;
        }

        private void AddObjectToExprList(object val, int startIndex, IScanner scanner, LinkedList<object> objects)
        {
            if (val?.ToString() == "(") {
                scanner.Position = startIndex;
                scanner.SkipWhiteSpace();
                scanner.SkipGroup();

                ExpressionEvaluator eval = new ExpressionEvaluator(
                    scanner.Read(startIndex + 1, scanner.Position - startIndex - 2),
                    Runtime // share the wealth
                );
                objects.AddLast(eval);
            }
            else {
                objects.AddLast(val);
            }
            scanner.SkipWhiteSpace();
        }
        
        private object EvaluateList(LinkedList<object> _objects)
        {
            LinkedList<object> list = new LinkedList<object>(_objects);

            // evaluate unary operators

            LinkedListNode<object> x = list.First;
            while (x != null) {
                UnaryOperator? op = x.Value as UnaryOperator?;
                if (op != null) {
                    var node = x.Next;
                    x.Value = PerformUnaryOp(Runtime, op.Value, node.Value);
                    if (node != null)
                        list.Remove(node);
                }
                x = x.Next;
            }

            // queue and evaluate binary operators
            BinaryOpQueue opqueue = new BinaryOpQueue(list);
            
            x = list.First?.Next; // skip the first operand
            while (x != null) {
                BinaryOperator? op = x.Value as BinaryOperator?;
                if (op == null) {
                    throw ThrowHelper.MissingBinaryOp(x.Previous.Value, x.Value);
                }
                else {
                    if (x.Next == null)
                        throw new ArgumentException("Expression cannot end in a binary operation [" + x.Value + "]");
                }
                x = x.Next?.Next; // skip the operand
            }

            var nodePair = default(ValueTuple<BinaryOperator, LinkedListNode<object>>);
            while (opqueue.Dequeue(out nodePair)) {
                nodePair.Item2.Value = PerformBinaryOp(
                    Runtime,
                    nodePair.Item1,
                    nodePair.Item2.Previous?.Value,
                    nodePair.Item2.Next?.Value
                    );
                if (nodePair.Item2.Next != null)
                    list.Remove(nodePair.Item2.Next);
                if (nodePair.Item2.Previous != null)
                    list.Remove(nodePair.Item2.Previous);
            }

            IExpressionEvaluator expr = list.First.Value as IExpressionEvaluator;
            if (expr == null) {
                return list.First?.Value;
            }
            else {
                return expr.Evaluate();
            }
        }
        
        #region static methods

        /// <summary>
        /// Static version of the Expression Evaluator
        /// </summary>
        /// <param name="expressionString">expression to be evaluated</param>
        /// <param name="runtime">the current execution</param>
        /// <returns></returns>
        public static object Evaluate(IEnumerable<char> expressionString, TRuntime runtime)
        {
            if (expressionString == null)
                throw new ArgumentNullException(nameof(expressionString));
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            ExpressionEvaluator expression = new ExpressionEvaluator(expressionString, runtime);
            return expression.Evaluate();
        }

        public static object PerformUnaryOp(TRuntime runtime, UnaryOperator op, object operand)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            if (op.EvaluateOperand) {
                IExpressionEvaluator tempv = operand as IExpressionEvaluator;
                if (tempv != null)
                    operand = tempv.Evaluate();
            }

            try {
                return op.ExecuteOperator(runtime, operand);
            }
            catch(InvalidCastException) {
                throw new ArgumentException("Unary operand cannot be " + operand.GetType().Name);
            }
        }

        /// <summary>
        /// Performs a binary operation
        /// </summary>
        public static object PerformBinaryOp(TRuntime runtime, BinaryOperator op, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            if (op.EvaulatedOperand.HasFlag(OperandPosition.Left)) {
                IExpressionEvaluator tv = left as IExpressionEvaluator;
                if (tv != null)
                    left = tv.Evaluate();
            }

            try {
                if (op.EvaulatedOperand.HasFlag(OperandPosition.Right)) {
                    IExpressionEvaluator tv;
                    switch (op.OperatorString) { // short circuit evaluation 1/6/16
                        case "AND":
                            if (Convert.ToBoolean(left, CultureInfo.CurrentCulture)) {
                                tv = right as IExpressionEvaluator;
                                if (tv != null) {
                                    right = tv.Evaluate();
                                }
                                if (Convert.ToBoolean(right, CultureInfo.CurrentCulture)) {
                                    return true;
                                }
                            }
                            return false;
                        case "OR":
                            if (Convert.ToBoolean(left, CultureInfo.CurrentCulture)) {
                                return true;
                            }
                            else {
                                tv = right as IExpressionEvaluator;
                                if (tv != null) {
                                    right = tv.Evaluate();
                                }
                                if (Convert.ToBoolean(right, CultureInfo.CurrentCulture)) {
                                    return true;
                                }
                            }
                            return false;
                    }

                    tv = right as IExpressionEvaluator;
                    if (tv != null)
                        right = tv.Evaluate();
                }
                
                return op.ExecuteOperator(runtime, left, right);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException) {
                throw new FormatException(string.Format(
                        "Operator [{0}] cannot be applied to objects of type '{1}' and '{2}'",
                        op.OperatorString, GetTypeName(right), GetTypeName(left)
                    ));
            }
        }

        public static string GetTypeName(object value)
        {
            Contract.Requires(value != null);
            Type t = value.GetType();
            if (t.IsArray) {
                return "array";
            }
            else {
                return t.Name;
            }
        }

        public static string GetStringRepresentation(object obj, bool escapeStrings = false)
        {
            if (obj == null)
                return "";
            
            string str_obj = obj as string;
            if (str_obj != null) {
                if (escapeStrings) {
                    return ToEscapedString(str_obj);
                }
                else {
                    return str_obj;
                }
            }
            else if (obj.GetType().IsArray) {
                StringBuilder sb = new StringBuilder("{ ");
                object[] _aObj = (object[])obj;
                if (_aObj.Length > 0) {
                    for (int i = 0; i < _aObj.Length - 1; i++) {
                        sb.AppendFormat("{0}, ", GetStringRepresentation(_aObj[i], escapeStrings: true));
                    }
                    sb.AppendFormat("{0} ", GetStringRepresentation(_aObj[_aObj.Length - 1], escapeStrings: true));
                }
                sb.Append("}");
                return sb.ToString();
            }
            return obj.ToString();
        }

        private static string ToEscapedString(string str)
        {
            Contract.Requires(str != null);
            StringBuilder sb = new StringBuilder();
            sb.Append('\"');
            for (int index = 0; index < str.Length; index++) {
                char c = str[index];
                switch (c) {
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\'': sb.Append("\\'"); break;
                    default:
                        if (c < ' ') {
                            sb.Append("\\u");
                            sb.Append(Convert.ToString(c, 16).PadLeft(4, '0'));
                        }
                        else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('\"');
            return sb.ToString();
        }

        public static object ConvertToSimpleType(object obj, ExecuterOption opts)
        {
            if (obj == null) {
                if (opts.HasFlag(ExecuterOption.NullIsZero)) {
                    return new Number(0);
                }
                else {
                    return null;
                }
            }

            if (obj is bool)
                return obj;

            Number? _nObj = Number.AsNumber(obj, opts);
            if (_nObj != null) {
                return new Number(_nObj.Value);
            }

            return obj;
        }

        public static object ConvertToSimpleType(IntPtr ptr)
        {
            if (IntPtr.Size == sizeof(long)) { // 64-bit
                return ptr.ToInt64();
            }
            else { // 32-bit
                return ptr.ToInt32();
            }
        }

        #endregion

        public override string ToString()
        {
            return Expression.ToString();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Expression != null);
            Contract.Invariant(Runtime != null);
        }
    }
}
