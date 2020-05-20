// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tbasic.Errors;
using System.Diagnostics.Contracts;
using Tbasic.Components;

namespace Tbasic.Parsing
{
    public partial class DefaultScanner
    {
        /// <summary>
        /// Gets the escape character for this scanner
        /// </summary>
        protected virtual char EscapeCharacter { get; } = '\\';

        /// <summary>
        /// Finds the index of the last quote of a quoted and escaped string
        /// </summary>
        protected int IndexString(string fullstr, int index)
        {
            Contract.Requires(fullstr != null);
            Contract.Requires((uint)index < (uint)fullstr.Length);
            Contract.Requires(IsQuote(fullstr[index]));

            char quote = fullstr[index++]; // The first character should be the quote

            for (; index < fullstr.Length; index++) {
                char cur = fullstr[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw ThrowHelper.UnterminatedString();
                    default:
                        if (cur == EscapeCharacter) {
                            index++;
                            cur = fullstr[index];
                            if (index >= fullstr.Length) {
                                throw ThrowHelper.UnterminatedEscapeSequence();
                            }
                            switch (cur) {
                                case 'b':
                                case 't':
                                case 'n':
                                case 'f':
                                case 'r':
                                case '\"':
                                case '\'': break; // you're golden
                                case 'u':
                                    index += 4;
                                    if (index >= fullstr.Length) {
                                        throw ThrowHelper.UnterminatedUnicodeEscape();
                                    }
                                    break;
                                default:
                                    if (cur != EscapeCharacter) {
                                        throw ThrowHelper.UnknownEscapeSequence(cur);
                                    }
                                    break;
                            }
                        }
                        else if (cur == quote) { // We be dun
                            return index;
                        }
                        break;
                }
            }
            throw ThrowHelper.UnterminatedString();
        }

        /// <summary>
        /// Parses a quoted and escaped string
        /// </summary>
        protected int ReadString(string fullstr, int index, out string s_parsed)
        {
            Contract.Requires(fullstr != null);
            Contract.Requires((uint)index < (uint)fullstr.Length);
            Contract.Requires(IsQuote(fullstr[index]));

            char quote = fullstr[index++]; // The first character should be the quote

            StringBuilder sb = new StringBuilder();
            for (; index < fullstr.Length; index++) {
                char cur = fullstr[index];
                switch (cur) {
                    case '\n':
                    case '\r':
                        throw ThrowHelper.UnterminatedString();
                    default:
                        if (cur == EscapeCharacter) {
                            index++;
                            if (index >= fullstr.Length) {
                                throw ThrowHelper.UnterminatedEscapeSequence();
                            }
                            cur = fullstr[index];
                            switch (cur) {
                                case 'b': sb.Append('\b'); break;
                                case 't': sb.Append('\t'); break;
                                case 'n': sb.Append('\n'); break;
                                case 'f': sb.Append('\f'); break;
                                case 'r': sb.Append('\r'); break;
                                case '"': sb.Append('"'); break;
                                case '\'': sb.Append('\''); break;
                                case 'u':
                                    index += 4;
                                    if (index >= fullstr.Length) {
                                        throw ThrowHelper.UnterminatedUnicodeEscape();
                                    }
                                    sb.Append((char)ushort.Parse(fullstr.Substring(index - 3, 4), NumberStyles.HexNumber));
                                    break;
                                default:
                                    if (cur == EscapeCharacter) {
                                        sb.Append(cur);
                                    }
                                    else {
                                        throw ThrowHelper.UnknownEscapeSequence(cur);
                                    }
                                    break;
                            }
                            break;
                        }
                        else if (cur == quote) { // We be dun
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
        /// Gets the index of the closing group character without finding its arguments
        /// </summary>
        protected int IndexGroup(string fullstr, int index)
        {
            Contract.Requires(fullstr != null);
            Contract.Requires((uint)index < (uint)fullstr.Length);
            Contract.Requires(IsGroupChar(fullstr[index]));

            char c_open = fullstr[index]; // The first character should be the grouping character (i.e. '(' or '[')
            char c_close = c_open == '(' ? ')' : ']';

            int expected = 1; // We are expecting one closing character
            int c_index = index + 1; // We used the first character
            for (; c_index < fullstr.Length; c_index++) {
                char cur = fullstr[c_index];
                switch (cur) {
                    case ' ': // ignore spaces
                        continue;
                    case '\'':
                    case '\"': {
                            c_index = IndexString(fullstr, c_index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            c_index = IndexGroup(fullstr, c_index);
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

        /// <summary>
        /// Gets the arguments of a group from a string with the opening group character at the given index
        /// </summary>
        protected int ReadGroup(string fullstr, int index, out IList<IEnumerable<char>> args)
        {
            return ReadGroup(fullstr, index, ',', out args);
        }

        private int ReadGroup(string fullstr, int index, char separator, out IList<IEnumerable<char>> args)
        {
            Contract.Requires(fullstr != null);
            Contract.Requires((uint)index < (uint)fullstr.Length);
            Contract.Requires(IsGroupChar(fullstr[index]));

            List<IEnumerable<char>> result = new List<IEnumerable<char>>();
            char c_open = fullstr[index];
            char c_close = c_open == '(' ? ')' : ']';
            int expected = 0;
            int last = index;

            for (; index < fullstr.Length; index++) {
                char cur = fullstr[index];
                switch (cur) {
                    case '\'':
                    case '\"': {
                            index = IndexString(fullstr, index);
                        }
                        break;
                    case '(':
                    case '[':
                        if (cur == c_open) {
                            expected++;
                        }
                        else {
                            index = IndexGroup(fullstr, index);
                        }
                        break;
                    default:
                        if (cur == c_close) {
                            expected--;
                        }
                        break;
                }

                if ((expected == 1 && cur == separator) // The separators in between other parentheses are not ours.
                    || expected == 0) {
                    StringSegment _param = StringSegment.Create(fullstr, last + 1, index - last - 1).Trim();
                    if (!StringSegment.IsNullOrEmpty(_param)) {
                        result.Add(_param);
                    }
                    last = index;
                    if (expected == 0) { // fin
                        args = result;
                        return last;
                    }
                }
            }
            throw ThrowHelper.UnterminatedGroup();
        }
    }
}
