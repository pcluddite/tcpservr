// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using Tbasic.Errors;
using Tbasic.Runtime;
using System.Globalization;

namespace Tbasic.Types
{
    /// <summary>
    /// Represents a generic number (this is a double at its core)
    /// </summary>
    public partial struct Number : IConvertible, IComparable, IComparable<Number>, IComparable<double>, IEquatable<Number>, IEquatable<double>
    {
        /// <summary>
        /// Gets or sets the value this Number represetns
        /// </summary>
        public double Value;
        /// <summary>
        /// Gets the size of the type that represents this number
        /// </summary>
        public const int SIZE = sizeof(double);

        /// <summary>
        /// Constructs a new number
        /// </summary>
        /// <param name="value"></param>
        public Number(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Determines if this number has a fractional part
        /// </summary>
        /// <returns></returns>
        public bool HasFraction()
        {
            return Value % 1 != 0;
        }

        /// <summary>
        /// Converts this number to an integer
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            return (int)Value;
        }

        /// <summary>
        /// Converts this number to whichever primitive type seems most appropriate (either int or double)
        /// </summary>
        /// <returns></returns>
        public object ToPrimitive()
        {
            if (HasFraction())
                return Value;
            return ToInt();
        }

        /// <summary>
        /// Tries to parse a string as a number
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Number result)
        {
            double d;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) {
                result = new Number(d);
                return true;
            }
            else {
                result = default(Number);
                return false;
            }
        }

        /// <summary>
        /// Parses a string as a number
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Number Parse(string s)
        {
            return double.Parse(s);
        }

        /// <summary>
        /// Determines if an object can be stored in a Number
        /// </summary>
        /// <param name="o"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static bool IsNumber(object o, ExecuterOption opts)
        {
            double d;
            return TypeConvert.TryConvert(o, out d, opts);
        }

        /// <summary>
        /// Attempts to convert an object to a Number. Returns null if no conversion is possible.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static Number? AsNumber(object o, ExecuterOption opts)
        {
            if (o == null) {
                if (opts.HasFlag(ExecuterOption.NullIsZero)) {
                    return 0;
                }
                else {
                    return null;
                }
            }
            Number? n = o as Number?;
            if (n != null)
                return n;
            double d;
            if (TypeConvert.TryConvert(o, out d, opts)) {
                return d;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Converts an object to a Number
        /// </summary>
        public static Number Convert(object o, ExecuterOption opts)
        {
            Number? n = AsNumber(o, opts);
            if (n == null)
                throw ThrowHelper.InvalidTypeInExpression(o?.GetType().Name ?? "null", nameof(Number));
            return n.Value;
        }

        /// <summary>
        /// Implicitly converts a double to a Number
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator Number(double d)
        {
            return new Number(d);
        }

        /// <summary>
        /// Implicitly converts a Number to a double
        /// </summary>
        /// <param name="num"></param>
        public static implicit operator double(Number num)
        {
            return num.Value;
        }

        /// <summary>
        /// Implicitly converts a double to an Integer
        /// </summary>
        /// <param name="n"></param>
        public static implicit operator Number(int n)
        {
            return new Number(n);
        }

        /// <summary>
        /// Converts this number to an integer
        /// </summary>
        /// <param name="n"></param>
        public static explicit operator int(Number n)
        {
            if (n.HasFraction())
                throw new InvalidCastException("Number contains a fractional part");
            return (int)n.Value;
        }
        
        /// <summary>
        /// Converts this number to a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (HasFraction())
                return Value.ToString();
            return ((long)Value).ToString();
        }

        #region IComparable

        /// <summary>
        /// Compares this Number with another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Number? n = obj as Number?;
            if (n != null)
                return CompareTo(n.Value);

            double? d = obj as double?;
            if (n != null)
                return CompareTo(d.Value);

            throw new ArgumentException($"Cannot compare types {nameof(Number)} and \"{obj?.GetType().Name}\"");
        }

        /// <summary>
        /// Compares this Number to another Number
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Number other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this Number to a double
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines if this number is equal to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Number other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// Determines if this number is equal to a double
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(double other)
        {
            return Value == other;
        }
        
        /// <summary>
        /// Determines if these two objects are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Number? n = obj as Number?;
            if (n != null)
                return Equals(n.Value);

            double? d = obj as double?;
            if (d != null)
                return Equals(d.Value);

            return false;
        }

        /// <summary>
        /// Gets the hash code for this number
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion
    }
}
