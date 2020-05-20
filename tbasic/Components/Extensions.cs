// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using Tbasic.Parsing;

namespace Tbasic
{
    internal static class Extensions
    {
        private const StringComparison ComparisonType = StringComparison.OrdinalIgnoreCase;

        public static bool EqualsIgnoreCase(this string initial, string other)
        {
            return initial.Equals(other, ComparisonType);
        }

        public static bool EndsWithIgnoreCase(this string initial, string other)
        {
            return initial.EndsWith(other, ComparisonType);
        }

        /// <summary>
        /// Removes the first instance of a char onward. If the char isn't found, returns the original string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemoveFromChar(this string str, char c)
        {
            int char_index = str.IndexOf(c);
            if (char_index > -1) {
                return str.Remove(char_index);
            }
            else {
                return str;
            }
        }

        /// <summary>
        /// Removes the first instance of a char and everything before it. If the char isn't found, returns an empty string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemoveToChar(this string str, char c)
        {
            int char_index = str.IndexOf(c);
            if (char_index < 0) {
                return string.Empty;
            }
            else {
                return str.Substring(0, char_index);
            }
        }

        internal static IEnumerable<TOutput> TB_ConvertAll<TInput, TOutput>(this IEnumerable<TInput> enumerable, Converter<TInput, TOutput> conversion)
        {
            foreach (TInput item in enumerable)
                yield return conversion(item);
        }

        internal static IEnumerable<string> TB_ToStrings(this IEnumerable<IEnumerable<char>> enumerable)
        {
            foreach (var item in enumerable)
                yield return item.ToString();
        }
        
        internal static IEnumerable<char> TB_Segment(this string source, int start, int count)
        {
            return StringSegment.Create(source, start, count);
        }

        /// <summary>
        /// Gets a character at an index safely, by returning -1 instead of throwing an IndexOutOfRange exception
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static int CharAt(this string s, int index)
        {
            if (index < 0 || index >= s.Length)
                return -1;
            return s[index];
        }

        internal static unsafe bool StartsWithFast(this string source, string test)
        {
            if (test.Length > source.Length)
                return false; // we don't have enough material

            fixed (char* lpsrc = source) fixed (char* lptest = test) {
                for (int i = 0; i < test.Length; ++i) {
                    if (lpsrc[i] != test[i]) {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static unsafe bool StartsWithFastIgnoreCase(this string source, string test)
        {
            if (test.Length > source.Length)
                return false; // we don't have enough material

            fixed (char* lpsrc = source) fixed (char* lptest = test) {
                for (int i = 0; i < test.Length; ++i) {
                    if (char.ToUpper(lpsrc[i]) != char.ToUpper(test[i])) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
