// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Tbasic.Parsing
{
    /// <summary>
    /// This class is used to avoid string copying. It keeps a reference to the original string and accesses only a segment of it. This class is immutable.
    /// </summary>
    internal sealed class StringSegment : IEnumerable<char>, IEquatable<StringSegment>, IEquatable<string>
    {
        /// <summary>
        /// Represents an empty string
        /// </summary>
        public static readonly StringSegment Empty = new StringSegment(string.Empty);

        [ContractPublicPropertyName(nameof(FullString))]
        private string full;

        [ContractPublicPropertyName(nameof(Offset))]
        private int offset;

        [ContractPublicPropertyName(nameof(Length))]
        private int len;

        /// <summary>
        /// Gets the length of this segment
        /// </summary>
        public int Length
        {
            get {
                return len;
            }
        }

        /// <summary>
        /// Gets the index of the segment in the complete string
        /// </summary>
        public int Offset
        {
            get {
                return offset;
            }
        }

        /// <summary>
        /// Gets the entire string referenced by this segment
        /// </summary>
        public string FullString
        {
            get {
                return full;
            }
        }

        /// <summary>
        /// Constructs a new string segment with a given string
        /// </summary>
        /// <param name="fullStr">the string for this segment</param>
        private StringSegment(string fullStr)
            : this(fullStr, 0)
        {
        }

        /// <summary>
        /// Constructs a new string segment with a given string
        /// </summary>
        /// <param name="fullStr">the entire string</param>
        /// <param name="offset">the index at which to start the segment</param>
        private StringSegment(string fullStr, int offset)
            : this(fullStr, offset, fullStr.Length - offset)
        {
        }

        /// <summary>
        /// Constructs a new string segment with a given string
        /// </summary>
        /// <param name="fullStr">the entire string</param>
        /// <param name="offset">the index at which to start the segment</param>
        /// <param name="count">the number of characters to include in this segment</param>
        private StringSegment(string fullStr, int offset, int count)
        {
            full = fullStr;
            this.offset = offset;
            len = count;
        }

        public static StringSegment Create(string fullStr, int offset, int count)
        {
            Contract.Requires(offset >= 0);

            if (count > fullStr.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            Contract.EndContractBlock();

            if (offset == fullStr.Length - 1)
                return Empty;
            return new StringSegment(fullStr, offset, count);
        }

        /// <summary>
        /// Gets a character at a given index of the segment
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get {
                if (index >= len || index < 0)
                    throw new IndexOutOfRangeException();
                Contract.EndContractBlock();
                return GetCharAt(index);
            }
        }

        private char GetCharAt(int index)
        {
            Contract.Requires(index >= 0 && index < len);
            return full[offset + index];
        }

        /// <summary>
        /// Create a substring within this segment
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public string Substring(int startIndex)
        {
            return Substring(startIndex, len - startIndex);
        }


        /// <summary>
        /// Create a substring within this segment
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string Substring(int startIndex, int count)
        {
            Contract.Requires(startIndex < len && startIndex >= 0);
            if (count > len - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));
            Contract.EndContractBlock();
            return full.Substring(startIndex + offset, count);
        }

        /// <summary>
        /// Just like Substring(), only returns a StringSegment instead of a string
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public StringSegment Subsegment(int startIndex)
        {
            return new StringSegment(full, offset + startIndex);
        }

        /// <summary>
        /// Just like Substring(), only returns a StringSegment instead of a string
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public StringSegment Subsegment(int startIndex, int count)
        {
            return new StringSegment(full, offset + startIndex, count);
        }

        /// <summary>
        /// Gets the first index of a character in a segment
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(char value)
        {
            Contract.Requires(full != null);
            return full.IndexOf(value, offset, len) - offset;
        }

        /// <summary>
        /// Gets the first index of a character in a segment
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int IndexOf(char value, int start)
        {
            Contract.Requires(full != null);
            return full.IndexOf(value, offset + start, len) - offset;
        }

        /// <summary>
        /// Gets the first index of a character in a segment
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(string value)
        {
            Contract.Requires(full != null);
            return full.IndexOf(value, offset, len) - offset;
        }

        /// <summary>
        /// Gets the first index of a character in a segment
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int IndexOf(string value, int start)
        {
            Contract.Requires(full != null);
            Contract.Requires(start < len && start >= 0);
            return full.IndexOf(value, offset + start, len - start) - offset;
        }

        /// <summary>
        /// Gets the first index of a character in a segment
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(string value, int start, StringComparison comparisonType)
        {
            Contract.Requires(full != null);
            Contract.Requires(start < len && start >= 0);
            return full.IndexOf(value, offset + start, len - start, comparisonType) - offset;
        }

        /// <summary>
        /// Checks if this segment starts with another string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StartsWith(string value)
        {
            return StartsWith(value, ignoreCase: false);
        }

        /// <summary>
        /// Checks if this segment starts with another string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public bool StartsWith(string value, bool ignoreCase)
        {
            return StartsWith(value, 0, ignoreCase);
        }

        /// <summary>
        /// Checks if this segment starts with another string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public unsafe bool StartsWith(string value, int startIndex, bool ignoreCase)
        {
            Contract.Requires(value != null);
            Contract.Requires(startIndex < Length && startIndex >= 0);
            int len = value.Length;
            fixed (char* aptr = full) fixed (char* bptr = value) {
                char* a = aptr + offset + startIndex;
                char* b = bptr;
                int index;
                if (!ignoreCase) {
                    index = FirstIndexOfNotEqual(a, b, len);
                }
                else {
                    index = FirstIndexOfNotEqualIgnoreCase(a, b, len);
                }
                return index == -1 || index == len;
            }

        }

        /// <summary>
        /// Returns a segment with all characters removed after a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public StringSegment Remove(int index)
        {
            return new StringSegment(full, offset, index);
        }

        /// <summary>
        /// Removes trailing and leading whitespace
        /// </summary>
        /// <returns></returns>
        public StringSegment Trim()
        {
            Contract.Requires(full != null);
            int new_start = SkipWhiteSpace();
            if (new_start == -1)
                return Empty;
            int new_end = len - 1;
            while (char.IsWhiteSpace(GetCharAt(new_end))) {
                --new_end;
            }
            int new_len = new_end - new_start + 1;
            if (new_start == offset && new_len == len) {
                return this; // there was no extra whitespace. do not create a new object.
            }
            else {
                return Subsegment(new_start, new_len);
            }
        }

        /// <summary>
        /// Converts this string to upper case
        /// </summary>
        /// <returns></returns>
        public string ToUpper()
        {
            return ToString().ToUpper();
        }

        /// <summary>
        /// Converts this string to lower case
        /// </summary>
        /// <returns></returns>
        public string ToLower()
        {
            return ToString().ToLower();
        }

        /// <summary>
        /// Converts this StringSegment object to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return full?.Substring(offset, len) ?? string.Empty;
        }

        /// <summary>
        /// Determines if this instance of a string segment is null or empty
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(StringSegment segment)
        {
            return segment == null || segment.FullString == null || segment.Length == 0;
        }

        internal int SkipWhiteSpace(int start = 0)
        {
            for (int index = start; index < Length; ++index) {
                if (!char.IsWhiteSpace(this[index])) {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines if this StringSegment is equal to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StringSegment other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Determines if this StringSegment is equal to another string
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            if (other == null) {
                return false;
            }
            return this.SequenceEqual(other);
        }

        /// <summary>
        /// Determines if this object is equal to another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            StringSegment seg = obj as StringSegment;
            if (seg != null) {
                return Equals(seg);
            }
            string str = obj as string;
            if (str != null) {
                return Equals(str);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Determines if two instances of a StringSegment have equal values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(StringSegment a, StringSegment b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            unsafe
            {
                fixed(char* afullptr = a.FullString) fixed (char* bfullptr = b.FullString)
                {
                    return EqualsHelper(afullptr + a.Offset, bfullptr + b.Offset, a.Length);
                }
            }
        }

        /// <summary>
        /// Determines if this StringSegment is equal to a string
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(StringSegment a, string b)
        {
            if ((object)a == null || (object)b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            unsafe
            {
                fixed (char* afullptr = a.FullString) fixed (char* bfullptr = b)
                {
                    return EqualsHelper(afullptr + a.Offset, bfullptr, a.Length);
                }
            }
        }

        private static unsafe bool EqualsHelper(char* aptr, char* bptr, int length)
        {
            return FirstIndexOfNotEqual(aptr, bptr, length) == -1;
        }

        private static unsafe int FirstIndexOfNotEqual(char* aptr, char* bptr, int length)
        {
            for (int index = 0; index < length; ++index) {
                if (aptr[index] != bptr[index]) {
                    return index;
                }
            }
            return -1;
        }

        private static unsafe int FirstIndexOfNotEqualIgnoreCase(char* aptr, char* bptr, int length)
        {
            for (int index = 0; index < length; ++index) {
                if (char.ToUpper(aptr[index]) != char.ToUpper(bptr[index])) {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines if two StringSegments are equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator==(StringSegment first, StringSegment second)
        {
            return Equals(first, second);
        }

        /// <summary>
        /// Determines if two StringSegments are not equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator !=(StringSegment first, StringSegment second)
        {
            return !Equals(first, second);
        }

        /// <summary>
        /// Casts a StringSegment to a string
        /// </summary>
        /// <param name="seg"></param>
        public static explicit operator string(StringSegment seg)
        {
            return seg.ToString();
        }

        /// <summary>
        /// Casts a string to a StringSegment
        /// </summary>
        /// <param name="str"></param>
        public static explicit operator StringSegment(string str)
        {
            return new StringSegment(str);
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return full.GetHashCode() ^ len ^ offset; // TODO: Optimize this so there aren't many collisions 6/16/16
        }

        /// <summary>
        /// Gets the enumerator for this StringSegment
        /// </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator()
        {
            for (int i = offset; i < len; ++i)
                yield return GetCharAt(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
