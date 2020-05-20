// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Types
{
	public partial struct Number 
	{
        /// <summary>
        /// Determines if one number is equal to another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator ==(Number left, Number right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Determines if one number is not equal to another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator !=(Number left, Number right)
        {
            return left.Value != right.Value;
        }

        /// <summary>
        /// Determines if one number is greater than another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator >(Number left, Number right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Determines if one number is less than another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator <(Number left, Number right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Determines if one number is greater than or equal to another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator >=(Number left, Number right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Determines if one number is less than or equal to another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a boolean value indicating if the expression is true of false</returns>
        public static bool operator <=(Number left, Number right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Adds one number to another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a Number with the resulting value</returns>
        public static Number operator +(Number left, Number right)
        {
            return new Number(left.Value + right.Value);
        }

        /// <summary>
        /// Subtracts one number from another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a Number with the resulting value</returns>
        public static Number operator -(Number left, Number right)
        {
            return new Number(left.Value - right.Value);
        }

        /// <summary>
        /// Multiplies one number by another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a Number with the resulting value</returns>
        public static Number operator *(Number left, Number right)
        {
            return new Number(left.Value * right.Value);
        }

        /// <summary>
        /// Divides one number by another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a Number with the resulting value</returns>
        public static Number operator /(Number left, Number right)
        {
            return new Number(left.Value / right.Value);
        }

        /// <summary>
        /// Performs a modulus operation on one number using another number
        /// </summary>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>a Number with the resulting value</returns>
        public static Number operator %(Number left, Number right)
        {
            return new Number(left.Value % right.Value);
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToBoolean(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToChar(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSByte(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToByte(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt64(provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt16(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt32(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt64(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToSingle(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDouble(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDecimal(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDateTime(provider);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToString(provider);
        }

        TypeCode IConvertible.GetTypeCode()
        {
            return Value.GetTypeCode();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Value).ToType(conversionType, provider);
        }
	}
}

