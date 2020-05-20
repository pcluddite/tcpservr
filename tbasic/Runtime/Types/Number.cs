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
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security;
using System.Runtime.CompilerServices;

namespace Tbasic.Runtime
{
    internal struct Number : IConvertible, IComparable, IComparable<Number>, IComparable<double>, IEquatable<Number>, IEquatable<double>
    {
        public double Value;

        public Number(decimal value)
        {
            Value = (double)value;
        }

        public Number(double value)
        {
            Value = value;
        }

        public bool HasFraction()
        {
            return Value % 1 != 0;
        }

        public int ToInt()
        {
            return (int)Value;
        }

        public object ToObject()
        {
            if (HasFraction())
                return Value;
            return ToInt();
        }

        public static bool TryParse(string s, out Number result)
        {
            double d;
            if (double.TryParse(s, out d)) {
                result = new Number(d);
                return true;
            }
            else {
                result = default(Number);
                return false;
            }
        }

        public static Number Parse(string s)
        {
            return double.Parse(s);
        }

        public static implicit operator Number(double d)
        {
            return new Number(d);
        }

        public static implicit operator double(Number n)
        {
            return n.Value;
        }

        #region IComparable

        public int CompareTo(object obj)
        {
            Number? n = obj as Number?;
            if (n != null)
                return CompareTo(n.Value);

            decimal? d = obj as decimal?;
            if (n != null)
                return CompareTo(n.Value);

            throw new ArgumentException(string.Format("can only compare types {0} or {1}", typeof(Number).Name, typeof(double).Name));
        }

        public int CompareTo(Number other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        #endregion

        #region IEquatable

        public bool Equals(Number other)
        {
            return Value == other.Value;
        }

        public bool Equals(double other)
        {
            return Value == other;
        }

        public override bool Equals(object obj)
        {
            Number? n = obj as Number?;
            if (n != null)
                return Equals(n.Value);

            double? d = obj as double?;
            if (d != null)
                return Equals(d.Value);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion

        #region boolean ops

        public static bool operator ==(Number left, Number right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(Number left, Number right)
        {
            return left.Value != right.Value;
        }

        public static bool operator <(Number left, Number right)
        {
            return left.Value < right.Value;
        }

        public static bool operator <=(Number left, Number right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >(Number left, Number right)
        {
            return left.Value > right.Value;
        }

        public static bool operator >=(Number left, Number right)
        {
            return left.Value >= right.Value;
        }

        #endregion

        #region arithmetic ops

        public static Number operator +(Number left, Number right)
        {
            return new Number(left.Value + right.Value);
        }

        public static Number operator -(Number left, Number right)
        {
            return new Number(left.Value - right.Value);
        }

        public static Number operator *(Number left, Number right)
        {
            return new Number(left.Value * right.Value);
        }

        public static Number operator /(Number left, Number right)
        {
            return new Number(left.Value / right.Value);
        }

        public static Number operator %(Number left, Number right)
        {
            return new Number(left.Value % right.Value);
        }

        #endregion

        #region IConvertable
        
        TypeCode IConvertible.GetTypeCode()
        {
            return Value.GetTypeCode();
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

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt32(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToUInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToInt64(provider);
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
            return Value.ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)Value).ToType(conversionType, provider);
        }

        #endregion
    }
}
