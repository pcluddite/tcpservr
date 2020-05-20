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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Tbasic.Components
{
    internal sealed class StringSegment : IEnumerable<char>, IEquatable<StringSegment>, IEquatable<string>
    {
        public static readonly StringSegment Empty = new StringSegment(string.Empty);

        private string full;
        private int offset;
        private int len;

        public int Length
        {
            get {
                return len;
            }
        }

        public int Offset
        {
            get {
                return offset;
            }
        }

        public string FullString
        {
            get {
                return full;
            }
        }

        public StringSegment(string fullStr)
            : this(fullStr, 0)
        {
        }

        public StringSegment(string fullStr, int offset)
            : this(fullStr, offset, fullStr.Length - offset)
        {
        }

        public StringSegment(string fullStr, int offset, int count)
        {
            if (count > fullStr.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));
            full = fullStr;
            this.offset = offset;
            len = count;
        }

        public char this[int index]
        {
            get {
                if (index >= len || index < 0)
                    throw new IndexOutOfRangeException();
                return GetCharAt(index);
            }
        }

        private char GetCharAt(int index)
        {
            return full[offset + index];
        }

        public string Substring(int startIndex)
        {
            return Substring(startIndex, len - startIndex);
        }

        public string Substring(int startIndex, int length)
        {
            if (length > len - startIndex)
                throw new ArgumentOutOfRangeException(nameof(length));
            return full.Substring(startIndex + offset, length);
        }

        public StringSegment Subsegment(int startIndex)
        {
            return new StringSegment(full, offset + startIndex);
        }

        public StringSegment Subsegment(int startIndex, int length)
        {
            return new StringSegment(full, offset + startIndex, length);
        }

        public int IndexOf(char value)
        {
            return full.IndexOf(value, offset, len) - offset;
        }

        public int IndexOf(char value, int start)
        {
            return full.IndexOf(value, offset + start, len) - offset;
        }

        public int IndexOf(string value)
        {
            return full.IndexOf(value, offset, len) - offset;
        }

        public int IndexOf(string value, int start)
        {
            return full.IndexOf(value, offset + start, len - start) - offset;
        }

        public int IndexOf(string value, int start, StringComparison comparisonType)
        {
            return full.IndexOf(value, offset + start, len - start, comparisonType) - offset;
        }

        public bool StartsWith(string value)
        {
            return StartsWith(value, ignoreCase: false);
        }

        public bool StartsWith(string value, bool ignoreCase)
        {
            return StartsWith(value, 0, ignoreCase);
        }

        public bool StartsWith(string value, int startIndex, bool ignoreCase)
        {
            unsafe
            {
                int len = value.Length;
                fixed (char* aptr = full) fixed (char* bptr = value)
                {
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
        }

        public StringSegment Remove(int index)
        {
            return new StringSegment(full, offset, index);
        }

        public StringSegment Trim()
        {
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

        public string ToUpper()
        {
            return ToString().ToUpper();
        }

        public string ToLower()
        {
            return ToString().ToLower();
        }

        public override string ToString()
        {
            return full.Substring(offset, len);
        }

        public static bool IsNullOrEmpty(StringSegment segment)
        {
            return segment == null || segment.FullString == null || segment.Length == 0;
        }

        public int SkipWhiteSpace(int start = 0)
        {
            for (int index = start; index < Length; ++index) {
                if (!char.IsWhiteSpace(this[index])) {
                    return index;
                }
            }
            return -1;
        }

        public bool Equals(StringSegment other)
        {
            return Equals(this, other);
        }

        public bool Equals(string other)
        {
            if (other == null) {
                return false;
            }
            return this.SequenceEqual(other);
        }

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

        public static bool operator==(StringSegment first, StringSegment second)
        {
            return Equals(first, second);
        }

        public static bool operator !=(StringSegment first, StringSegment second)
        {
            return !Equals(first, second);
        }


        public override int GetHashCode()
        {
            return full.GetHashCode() ^ len ^ offset; // TODO: Optimize this so there aren't many collisions 6/16/16
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new StringSegEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class StringSegEnumerator : IEnumerator<char>
        {
            private StringSegment seg;
            private int curr = -1;

            public StringSegEnumerator(StringSegment segment)
            {
                seg = segment;
            }

            public char Current
            {
                get {
                    return seg.GetCharAt(curr);
                }
            }

            object IEnumerator.Current
            {
                get {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++curr < seg.len;
            }

            public void Reset()
            {
                curr = -1;
            }
        }
    }
}
