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
using Tbasic.Errors;
using Tbasic.Runtime;
using Tbasic.Components;

namespace Tbasic.Parsing
{
    /// <summary>
    /// A set of methods for parsing character groups such as strings and characters grouped by parentheses
    /// </summary>
    public static class GroupParser
    {
        /// <summary>
        /// Indexes a string without parsing it (i.e. retreives the index of the terminating quote)
        /// </summary>
        /// <param name="str_full">the full string containing the string to be checked</param>
        /// <param name="index">the index in the full string of the first quote of the string to be indexed</param>
        /// <returns>the index of the terminating quote in the full string</returns>
        public static int IndexString(string str_full, int index)
        {
            return IndexString(new StringSegment(str_full), index);
        }

        internal static int IndexString(StringSegment str_full, int index)
        {
            char quote = str_full[index++]; // The first character should be the quote

            for (; index < str_full.Length; index++) {
                char cur = str_full[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw ThrowHelper.UnterminatedString();
                    case '\\':
                        index++;
                        cur = str_full[index];
                        if (index >= str_full.Length) {
                            throw ThrowHelper.UnterminatedEscapeSequence();
                        }
                        switch (cur) {
                            case 'b':
                            case 't':
                            case 'n':
                            case 'f':
                            case 'r':
                            case '\"':
                            case '\\':
                            case '\'': break; // you're golden
                            case 'u':
                                index += 4;
                                if (index >= str_full.Length) {
                                    throw ThrowHelper.UnterminatedUnicodeEscape();
                                }
                                break;
                            default:
                                throw ThrowHelper.UnknownEscapeSequence(cur);
                        }
                        break;
                    default:
                        if (cur == quote) { // We be dun
                            return index;
                        }
                        break;
                }
            }
            throw ThrowHelper.UnterminatedString();
        }

        /// <summary>
        /// Indexes and builds a string, parsing any escape sequences properly
        /// </summary>
        /// <param name="str_full">the full string containing the string to be parsed</param>
        /// <param name="index">the index in the full string of the first quote of the string to be indexed</param>
        /// <param name="s_parsed">the parsed string (without quotes and escape sequences replaced)</param>
        /// <returns>the index of the terminating quote  in the full string</returns>
        public static int ReadString(string str_full, int index, out string s_parsed)
        {
            return ReadString(new StringSegment(str_full), index, out s_parsed);
        }

        internal static int ReadString(StringSegment str_full, int index, out string s_parsed)
        {
            char quote = str_full[index++]; // The first character should be the quote

            StringBuilder sb = new StringBuilder();
            for (; index < str_full.Length; index++) {
                char cur = str_full[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw ThrowHelper.UnterminatedString();
                    case '\\':
                        index++;
                        if (index >= str_full.Length) {
                            throw ThrowHelper.UnterminatedEscapeSequence();
                        }
                        cur = str_full[index];
                        switch (cur) {
                            case 'b': sb.Append('\b'); break;
                            case 't': sb.Append('\t'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'r': sb.Append('\r'); break;
                            case '\\': sb.Append('\\'); break;
                            case '"': sb.Append('"'); break;
                            case '\'': sb.Append('\''); break;
                            case 'u':
                                index += 4;
                                if (index >= str_full.Length) {
                                    throw ThrowHelper.UnterminatedUnicodeEscape();
                                }
                                sb.Append((char)ushort.Parse(str_full.Substring(index - 3, 4), NumberStyles.HexNumber));
                                break;
                            default:
                                throw ThrowHelper.UnknownEscapeSequence(cur);
                        }
                        break;
                    default:
                        if (cur == quote) { // We be dun
                            s_parsed = sb.ToString();
                            return index;
                        }
                        else {
                            sb.Append(cur);
                        }
                        break;
                }
            }
            throw ThrowHelper.UnterminatedString();
        }

        /// <summary>
        /// Indexes a group (set off by parentheses or brackets) without evaluating its operators
        /// </summary>
        /// <param name="s_full">the full string containing the group to be indexed</param>
        /// <param name="firstIndex">the index in the full string of the character of the group to be indexed</param>
        /// <returns>the index of the terminating grouping character in the full string</returns>
        public static int IndexGroup(string s_full, int firstIndex)
        {
            return IndexGroup(new StringSegment(s_full), firstIndex);
        }
        
        internal static int IndexGroup(StringSegment s_full, int firstIndex)
        {
            char c_open = s_full[firstIndex]; // The first character should be the grouping character (i.e. '(' or '[')
            char c_close = c_open == '(' ? ')' : ']';

            int expected = 1; // We are expecting one closing character
            int c_index = firstIndex + 1; // We used the first character
            for (; c_index < s_full.Length; c_index++) {
                char cur = s_full[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = IndexString(s_full, c_index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            c_index = IndexGroup(s_full, c_index);
                        }
                        break;
                    default:
                        if (cur == c_close) {
                            expected--;
                        }
                        break;
                }
                if (expected == 0) {
                    return c_index;
                }
            }
            throw ThrowHelper.UnterminatedGroup();
        }

        internal static int ReadGroup(StringSegment s_full, int c_index, Executer _curExec, out IList<object> _oParams)
        {
            List<object> result = new List<object>();
            char c_open = s_full[c_index];
            char c_close = c_open == '(' ? ')' : ']';
            int expected = 0;
            int last = c_index;

            for (; c_index < s_full.Length; c_index++) {
                char cur = s_full[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = IndexString(s_full, c_index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            c_index = IndexGroup(s_full, c_index);
                        }
                        break;
                    default:
                        if (cur == c_close) {
                            expected--;
                        }
                        break;
                }

                if ((expected == 1 && cur == ',') // The commas in between other parentheses are not ours.
                    || expected == 0) {
                    StringSegment _param = s_full.Subsegment(last + 1, c_index - last - 1).Trim();
                    if (!StringSegment.Equals(_param, "")) {
                        result.Add(Evaluator.Evaluate(_param, _curExec));
                    }
                    last = c_index;
                    if (expected == 0) { // fin
                        _oParams = result;
                        return last;
                    }
                }
            }
            throw ThrowHelper.UnterminatedGroup();
        }
    }
}
