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
using System.Globalization;
using System.Text;
using Tbasic.Parsing;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Operators;

namespace Tbasic.Runtime
{
    /// <summary>
    /// General purpose evaluator for functions, variables, math, booleans, etc.
    /// </summary>
    internal partial class Evaluator : IEvaluator
    {
        private LinkedList<object> _tokens = new LinkedList<object>();
        private StringSegment _expression = StringSegment.Empty;
        private bool _parsed;
        
        public Evaluator(Executer exec)
        {
            CurrentExecution = exec;
        }
        
        public Evaluator(StringSegment expression, Executer exec)
        {
            CurrentExecution = exec;
            Expression = expression;
        }

        #region Properties
        
        public StringSegment Expression
        {
            get { return _expression; }
            set {
                _expression = value.Trim();
                _parsed = false;
                _tokens.Clear();
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get; set; }

        public bool ShouldParse
        {
            get {
                return !_parsed;
            }
            set {
                if (_parsed && value) {
                    _tokens.Clear();
                    _parsed = !value;
                }
            }
        }

        #endregion
        
        #region Evaluate

        public object Evaluate()
        {
            if (StringSegment.IsNullOrEmpty(Expression)) 
                return 0;
            
            if (!_parsed) {
                Scanner scanner = new Scanner(_expression);
                while (!scanner.EndOfStream)
                    NextToken(scanner);
                _parsed = true;
            }

            return ConvertToObject(EvaluateList());
        }
        
        public bool EvaluateBool()
        {
            return Convert.ToBoolean(Evaluate());
        }
        
        public int EvaluateInt()
        {
            return Convert.ToInt32(Evaluate());
        }

        public double EvaluateDouble()
        {
            return Convert.ToDouble(Evaluate());
        }
        
        public decimal EvaluateDecimal()
        {
            return Convert.ToDecimal(Evaluate());
        }
        
        public long EvaluateLong()
        {
            return Convert.ToInt64(Evaluate());
        }

        #endregion
        
        private int NextToken(Scanner scanner)
        {
            scanner.SkipWhiteSpace();

            int startIndex = scanner.IntPosition;

            // check group
            if (scanner.Next("(")) {
                return AddObjectToExprList("(", startIndex, scanner);
            }

            // check string
            string str_parsed;
            if (scanner.NextString(out str_parsed)) {
                return AddObjectToExprList(str_parsed, startIndex, scanner);
            }

            // check unary op
            UnaryOperator unaryOp;
            if (scanner.NextUnaryOp(CurrentContext._unaryOps, _tokens.Last?.Value, out unaryOp)) {
                return AddObjectToExprList(unaryOp, startIndex, scanner);
            }

            // check function
            Function func;
            if (scanner.NextFunction(CurrentExecution, out func)) {
                return AddObjectToExprList(func, startIndex, scanner);
            }

            // check null
            if (scanner.Next("null")) {
                return AddObjectToExprList("null", startIndex, scanner);
            }

            // check variable
            Variable variable;
            if (scanner.NextVariable(CurrentExecution, out variable)) {
                return AddObjectToExprList(variable, startIndex, scanner);
            }

            // check hexadecimal
            int hex;
            if (scanner.NextHexadecimal(out hex)) {
                return AddObjectToExprList(hex, startIndex, scanner);
            }

            // check boolean
            bool b;
            if (scanner.NextBool(out b)) {
                return AddObjectToExprList(b, startIndex, scanner);
            }

            // check numeric
            Number num;
            if (scanner.NextPositiveNumber(out num)) {
                return AddObjectToExprList(num.ToObject(), startIndex, scanner);
            }

            // check binary operator
            BinaryOperator binOp;
            if (scanner.NextBinaryOp(CurrentContext._binaryOps, out binOp)) {
                return AddObjectToExprList(binOp, startIndex, scanner);
            }

            // couldn't be parsed

            if (CurrentContext.FindFunctionContext(_expression.ToString()) == null) {
                throw new ArgumentException("Invalid expression '" + _expression + "'");
            }
            else {
                throw new FormatException("Poorly formed function call");
            }

            /*if (mRet.Index != nIdx) {
                throw new ArgumentException(
                    "Invalid token in expression '" + expr.Substring(nIdx, mRet.Index - nIdx).Trim() + "'"
                );
            }*/
        }

        private int AddObjectToExprList(object val, int startIndex, Scanner scanner)
        {
            if (Equals(val, "(")) {

                scanner.IntPosition = GroupParser.IndexGroup(_expression, startIndex) + 1;

                Evaluator eval = new Evaluator(
                    _expression.Subsegment(startIndex + 1, scanner.IntPosition - startIndex - 2),
                    CurrentExecution // share the wealth
                );
                _tokens.AddLast(eval);
            }
            else {
                _tokens.AddLast(val);
            }

            return scanner.IntPosition;
        }
        
        private object EvaluateList()
        {
            LinkedList<object> list = new LinkedList<object>(_tokens);

            // evaluate unary operators

            LinkedListNode<object> x = list.First;
            while (x != null) {
                UnaryOperator? op = x.Value as UnaryOperator?;
                if (op != null) {
                    x.Value = PerformUnaryOp(op.Value, x.Previous?.Value, x.Next.Value);
                    list.Remove(x.Next);
                }
                x = x.Next;
            }

            // queue and evaluate binary operators
            BinaryOpQueue opqueue = new BinaryOpQueue(list);
            
            x = list.First.Next; // skip the first operand
            while (x != null) {
                BinaryOperator? op = x.Value as BinaryOperator?;
                if (op == null) {
                    throw ThrowHelper.MissingBinaryOp(x.Previous.Value, x.Value);
                }
                else {
                    if (x.Next == null)
                        throw new ArgumentException("Expression cannot end in a binary operation [" + x.Value + "]");
                }
                x = x.Next.Next; // skip the operand
            }

            BinOpNodePair nodePair;
            while (opqueue.Dequeue(out nodePair)) {
                nodePair.Node.Previous.Value = PerformBinaryOp(
                    nodePair.Operator,
                    nodePair.Node.Previous.Value,
                    nodePair.Node.Next.Value
                    );
                list.Remove(nodePair.Node.Next);
                list.Remove(nodePair.Node);
            }

            IEvaluator expr = list.First.Value as IEvaluator;
            if (expr == null) {
                return list.First.Value;
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
        /// <param name="exec">the current execution</param>
        /// <returns></returns>
        public static object Evaluate(StringSegment expressionString, Executer exec)
        {
            Evaluator expression = new Evaluator(expressionString, exec);
            return expression.Evaluate();
        }

        internal static bool TryParse<T>(object input, out T result)
        {
            try {
                result = (T)input;
                return true;
            }
            catch (InvalidCastException) {
                IConvertible convertible = input as IConvertible;
                if (convertible == null) {
                    result = default(T);
                    return false;
                }
                else {
                    try {
                        result = (T)convertible.ToType(typeof(T), CultureInfo.CurrentCulture);
                        return true;
                    }
                    catch {
                        result = default(T);
                        return false;
                    }
                }
            }
        }

        public static object PerformUnaryOp(UnaryOperator op, object left, object right)
        {
            object operand = op.Side == UnaryOperator.OperandSide.Left ? left : right;
            IEvaluator tempv = operand as IEvaluator;
            if (tempv != null) {
                operand = tempv.Evaluate();
            }

            try {
                return op.ExecuteOperator(operand);
            }
            catch(InvalidCastException) when (operand is IOperator) {
                throw new ArgumentException("Unary operand cannot be " + operand.GetType().Name);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException) {
                throw new ArgumentException("Unary operator '" + op.OperatorString + "' not defined.");
            }
        }

        /// <summary>
        /// This routine will actually execute an operation and return its value
        /// </summary>
        /// <param name="op">Operator Information</param>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>v1 (op) v2</returns>
        public static object PerformBinaryOp(BinaryOperator op, object left, object right)
        {
            IEvaluator tv = left as IEvaluator;
            if (tv != null) {
                left = tv.Evaluate();
            }

            try {
                switch (op.OperatorString) { // short circuit evaluation 1/6/16
                    case "AND":
                        if (Convert.ToBoolean(left, CultureInfo.CurrentCulture)) {
                            tv = right as IEvaluator;
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
                            tv = right as IEvaluator;
                            if (tv != null) {
                                right = tv.Evaluate();
                            }
                            if (Convert.ToBoolean(right, CultureInfo.CurrentCulture)) {
                                return true;
                            }
                        }
                        return false;
                }

                tv = right as IEvaluator;
                if (tv != null) {
                    right = tv.Evaluate();
                }
                return op.ExecuteOperator(left, right);
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException) {
                throw new FormatException(string.Format(
                        "Operator '{0}' cannot be applied to objects of type '{1}' and '{2}'",
                        op.OperatorString, GetTypeName(right), GetTypeName(left)
                    ));
            }
        }

        public static string GetTypeName(object value)
        {
            Type t = value.GetType();
            if (t.IsArray) {
                return "object array";
            }
            else {
                return t.Name.ToLower();
            }
        }

        public static string ConvertToString(object obj)
        {
            if (obj == null) {
                return "";
            }
            string str_obj = obj as string;
            if (str_obj != null) {
                return FormatString(str_obj);
            }
            else if (obj.GetType().IsArray) {
                StringBuilder sb = new StringBuilder("{ ");
                object[] _aObj = (object[])obj;
                if (_aObj.Length > 0) {
                    for (int i = 0; i < _aObj.Length - 1; i++) {
                        sb.AppendFormat("{0}, ", ConvertToString(_aObj[i]));
                    }
                    sb.AppendFormat("{0} ", ConvertToString(_aObj[_aObj.Length - 1]));
                }
                sb.Append("}");
                return sb.ToString();
            }
            return obj.ToString();
        }

        private static string FormatString(string str)
        {
            StringBuilder sb = new StringBuilder();
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
            return "\"" + sb + "\"";
        }

        public static object ConvertToObject(object _oObj)
        {
            if (_oObj == null) {
                return 0;
            }
            int? _iObj = _oObj as int?;
            if (_iObj != null)
                return _iObj.Value;

            double? _dObj = _oObj as double?;
            if (_dObj != null) {
                Number n = new Number(_dObj.Value);
                if (!n.HasFraction())
                    return n.ToInt();
            }

            decimal? _mObj = _oObj as decimal?;
            if (_mObj != null) {
                Number n = new Number(_dObj.Value);
                if (!n.HasFraction())
                    return n.ToInt();
            }

            IntPtr? _pObj = _oObj as IntPtr?;
            if (_pObj != null) {
                return ConvertToObject(_pObj.Value);
            }

            return _oObj;
        }

        public static object ConvertToObject(IntPtr ptr)
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
    }
}
