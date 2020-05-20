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
using System.Text.RegularExpressions;
using Tbasic.Components;
using Tbasic.Runtime;

namespace Tbasic.Operators
{
    internal partial class BinOpDictionary
    {
        private Dictionary<string, BinaryOperator> binaryOps = new Dictionary<string, BinaryOperator>(22 /* magic number of standard operators */, StringComparer.OrdinalIgnoreCase);

        public void LoadStandardOperators()
        {
            binaryOps.Add("*",   new BinaryOperator("*",   0, Multiply));
            binaryOps.Add("/",   new BinaryOperator("/",   0, Divide));
            binaryOps.Add("MOD", new BinaryOperator("MOD", 0, Modulo));
            binaryOps.Add("+",   new BinaryOperator("+",   1, Add));
            binaryOps.Add("-",   new BinaryOperator("-",   1, Subtract));
            binaryOps.Add(">>",  new BinaryOperator(">>",  2, ShiftRight));
            binaryOps.Add("<<",  new BinaryOperator("<<",  2, ShiftLeft));
            binaryOps.Add("<",   new BinaryOperator("<",   3, LessThan));
            binaryOps.Add("=<",  new BinaryOperator("=<",  3, LessThanOrEqual));
            binaryOps.Add("<=",  new BinaryOperator("<=",  3, LessThanOrEqual));
            binaryOps.Add(">",   new BinaryOperator(">",   3, GreaterThan));
            binaryOps.Add("=>",  new BinaryOperator("=>",  3, GreaterThanOrEqual));
            binaryOps.Add(">=",  new BinaryOperator(">=",  3, GreaterThanOrEqual));
            binaryOps.Add("==",  new BinaryOperator("==",  4, EqualTo));
            binaryOps.Add("=",   new BinaryOperator("=",   4, EqualTo));
            binaryOps.Add("<>",  new BinaryOperator("<>",  4, NotEqualTo));
            binaryOps.Add("!=",  new BinaryOperator("!=",  4, NotEqualTo));
            binaryOps.Add("&",   new BinaryOperator("&",   5, BitAnd));
            binaryOps.Add("^",   new BinaryOperator("^",   6, BitXor));
            binaryOps.Add("|",   new BinaryOperator("|",   7, BitOr));
            binaryOps.Add("AND", new BinaryOperator("AND", 8, NotImplemented)); // These are special cases that are evaluated with short circuit evalutaion 6/20/16
            binaryOps.Add("OR",  new BinaryOperator("OR",  9, NotImplemented));
        }

        /// <summary>
        /// This method gets the precedence of a binary operator
        /// </summary>
        /// <param name="strOp"></param>
        /// <returns></returns>
        public int OperatorPrecedence(string strOp)
        {
            return binaryOps[strOp].Precedence;
        }

        private static object Multiply(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) *
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object Divide(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) /
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object Modulo(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) %
                   Convert.ToInt64(right, CultureInfo.CurrentCulture);
        }

        private static object Add(object left, object right)
        {
            string str1 = left as string,
                   str2 = right as string;
            if (str1 != null || str2 != null)
                return StringAdd(left, right, str1, str2);
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) +
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static string StringAdd(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 + str2;
        }

        private static object Subtract(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) -
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object LessThan(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) <
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object LessThanOrEqual(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) <=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object GreaterThan(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) >
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object GreaterThanOrEqual(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) >=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static object EqualTo(object left, object right)
        {
            string str1 = left as string,
                   str2 = right as string;
            if (str1 != null || str2 != null)
                return StrEquals(left, right, str1, str2);
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) ==
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static bool StrEquals(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 == str2;
        }

        private static void InitializeStrings(object left, object right, ref string str1, ref string str2)
        {
            if (str1 == null)
                str1 = Evaluator.ConvertToString(left);
            if (str2 == null)
                str2 = Evaluator.ConvertToString(right);
        }

        private static object NotEqualTo(object left, object right)
        {
            return Convert.ToDouble(left, CultureInfo.CurrentCulture) !=
                   Convert.ToDouble(right, CultureInfo.CurrentCulture);
        }

        private static bool StrNotEqualTo(object left, object right, string str1, string str2)
        {
            InitializeStrings(left, right, ref str1, ref str2);
            return str1 != str2;
        }

        private static object ShiftLeft(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) <<
                   Convert.ToInt32(right, CultureInfo.CurrentCulture);
        }

        private static object ShiftRight(object left, object right)
        {
            return Convert.ToInt64(left, CultureInfo.CurrentCulture) >>
                   Convert.ToInt32(right, CultureInfo.CurrentCulture);
        }

        private static object BitAnd(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) &
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object BitXor(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) ^
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object BitOr(object left, object right)
        {
            return Convert.ToUInt64(left, CultureInfo.CurrentCulture) |
                   Convert.ToUInt64(right, CultureInfo.CurrentCulture);
        }

        private static object NotImplemented(object left, object right)
        {
            throw new NotImplementedException();
        }
    }
}
