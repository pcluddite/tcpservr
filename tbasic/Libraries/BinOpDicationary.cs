// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Globalization;
using Tbasic.Runtime;
using Tbasic.Errors;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Tbasic.Types
{
    internal class BinOpDictionary : OperatorDictionary<BinaryOperator>
    {
        public BinOpDictionary()
        {
        }

        public BinOpDictionary(BinOpDictionary other)
            : base(other)
        {
        }

        public override void LoadStandardOperators()
        {
            operators.Add(new BinaryOperator(".", 2, Dot, OperandPosition.Left)); // only evaluate the left operand
            operators.Add(new BinaryOperator("*", 5, Multiply));
            operators.Add(new BinaryOperator("/", 5, Divide));
            operators.Add(new BinaryOperator("%", 5, Modulo));
            operators.Add(new BinaryOperator("+", 6, Add));
            operators.Add(new BinaryOperator("-", 6, Subtract));
            operators.Add(new BinaryOperator(">>", 7, ShiftRight));
            operators.Add(new BinaryOperator("<<", 7, ShiftLeft));
            operators.Add(new BinaryOperator("<", 8, LessThan));
            operators.Add(new BinaryOperator("=<", 8, LessThanOrEqual));
            operators.Add(new BinaryOperator("<=", 8, LessThanOrEqual));
            operators.Add(new BinaryOperator(">", 8, GreaterThan));
            operators.Add(new BinaryOperator("=>", 8, GreaterThanOrEqual));
            operators.Add(new BinaryOperator(">=", 8, GreaterThanOrEqual));
            operators.Add(new BinaryOperator("==", 9, EqualTo));
            operators.Add(new BinaryOperator("~=", 9, SortaEquals));
            operators.Add(new BinaryOperator("<>", 9, NotEqualTo));
            operators.Add(new BinaryOperator("!=", 9, NotEqualTo));
            operators.Add(new BinaryOperator("&", 10, BitAnd));
            operators.Add(new BinaryOperator("^", 11, BitXor));
            operators.Add(new BinaryOperator("|", 12, BitOr));
            operators.Add(new BinaryOperator("&&", 13, NotImplemented)); // These are special cases that are evaluated with short circuit evalutaion 6/20/16
            operators.Add(new BinaryOperator("||", 14, NotImplemented));
            operators.Add(new BinaryOperator("=", 16, Set, OperandPosition.Right));
        }

        private static object Dot(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (left == null || right == null)
                throw new UndefinedObjectException("The dot operator does not accept null operands");
            Contract.EndContractBlock();
            
            TClass n = left as TClass;
            if (n == null)
                throw new TbasicRuntimeException("The dot operator cannot be used on primitive types");
            
            FunctionEvaluator fEval = right as FunctionEvaluator;
            if (fEval != null) {
                fEval.CurrentContext = n;
                return fEval.Evaluate();
            }
            VariableEvaluator vEval = right as VariableEvaluator;
            if (vEval == null) {
                throw ThrowHelper.InvalidExpression($"{n.Name}.{right}");
            }
            else {
                vEval.CurrentContext = n;
                return vEval;
            }
        }

        private static object Set(TRuntime runtime, object left, object right)
        {
            VariableEvaluator v = left as VariableEvaluator;
            if (v == null)
                throw new ArgumentException($"Cannot set the value of {left}");

            ObjectContext context = v.CurrentContext;
            context.SetVariable(v.Name, right);

            return right;
        }

        private static object Multiply(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) *
                   Number.Convert(right, runtime.Options);
        }

        private static object Divide(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) /
                   Number.Convert(right, runtime.Options);
        }

        private static object Modulo(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Convert.ToInt64(left, CultureInfo.InvariantCulture) %
                   Convert.ToInt64(right, CultureInfo.InvariantCulture);
        }

        private static object Add(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            string str1 = left as string,
                   str2 = right as string;
            if (str1 != null || str2 != null)
                return StringAdd(left, right, str1, str2);
            return Number.Convert(left, runtime.Options) +
                   Number.Convert(right, runtime.Options);
        }

        private static string StringAdd(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 + str2;
        }

        private static object Subtract(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) -
                   Number.Convert(right, runtime.Options);
        }

        private static object LessThan(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) <
                   Number.Convert(right, runtime.Options);
        }

        private static object LessThanOrEqual(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) <=
                   Number.Convert(right, runtime.Options);
        }

        private static object GreaterThan(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) >
                   Number.Convert(right, runtime.Options);
        }

        private static object GreaterThanOrEqual(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return Number.Convert(left, runtime.Options) >=
                   Number.Convert(right, runtime.Options);
        }

        private static object EqualTo(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return EqualToAsBool(runtime, left, right);
        }

        private static bool EqualToAsBool(TRuntime runtime, object left, object right) // separate method so that it won't be boxed and unboxed unnecessarily 8/8/16
        {
            if (left == right)
                return true;

            if (left == null || right == null)
                return false;

            return left.Equals(right);
        }

        private static void InitializeStrings(object left, object right, ref string str1, ref string str2)
        {
            if (str1 == null)
                str1 = ExpressionEvaluator.GetStringRepresentation(left);
            if (str2 == null)
                str2 = ExpressionEvaluator.GetStringRepresentation(right);
        }

        private static object SortaEquals(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            if (left == null ^ right == null) // exclusive or
                return false;
            if (left.GetType() == right.GetType())
                return EqualTo(runtime, left, right);

            string str_left = left as string;
            if (str_left != null)
                return StrSortaEqualsObj(str_left, right);

            string str_right = right as string;
            if (str_right != null)
                return StrSortaEqualsObj(str_right, left);

            return false;
        }

        private const ExecuterOption SortaEqualsOptions = ExecuterOption.None;

        private static bool StrSortaEqualsObj(string str_left, object right)
        {
            Number? n_right = Number.AsNumber(right, SortaEqualsOptions);
            if (n_right != null) {
                Number n_left;
                if (Number.TryParse(str_left, out n_left)) {
                    return n_left == n_right.Value;
                }
                bool b_left;
                if (bool.TryParse(str_left, out b_left)) {
                    return (n_right != 0) == b_left;
                }
            }

            string str_right = right as string;
            if (str_right != null)
                return false;
            
            bool b_right;
            if (bool.TryParse(str_right, out b_right)) {
                bool b_left;
                if (bool.TryParse(str_left, out b_left)) {
                    return b_left == b_right;
                }
                Number n_left;
                if (Number.TryParse(str_left, out n_left)) {
                    return (n_left != 0) == b_right;
                }
            }
            return false;
        }

        private static object NotEqualTo(TRuntime runtime, object left, object right)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            Contract.EndContractBlock();

            return !EqualToAsBool(runtime, left, right);
        }

        private static bool StrNotEqualTo(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 != str2;
        }

        private static object ShiftLeft(TRuntime runtime, object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.InvariantCulture) <<
                   Convert.ToInt32(right, CultureInfo.InvariantCulture);
        }

        private static object ShiftRight(TRuntime runtime, object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.InvariantCulture) >>
                   Convert.ToInt32(right, CultureInfo.InvariantCulture);
        }

        private static object BitAnd(TRuntime runtime, object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.InvariantCulture) &
                   Convert.ToUInt64(right, CultureInfo.InvariantCulture);
        }

        private static object BitXor(TRuntime runtime, object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.InvariantCulture) ^
                   Convert.ToUInt64(right, CultureInfo.InvariantCulture);
        }

        private static object BitOr(TRuntime runtime, object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.InvariantCulture) |
                   Convert.ToUInt64(right, CultureInfo.InvariantCulture);
        }

        private static object NotImplemented(TRuntime runtime, object left, object right)
        {
            throw new NotImplementedException();
        }
    }
}
