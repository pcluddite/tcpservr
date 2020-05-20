// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tbasic.Components;
using Tbasic.Errors;
using System.Diagnostics.Contracts;

namespace Tbasic.Parsing
{
    /// <summary>
    /// Structure to obtain a group of characters between an opening character and a closing character
    /// </summary>
    public struct GroupParser
    {
        /// <summary>
        /// The opening character for this group
        /// </summary>
        public char Open { get; }
        
        /// <summary>
        /// The closing character for this group
        /// </summary>
        public char Close { get; }

        /// <summary>
        /// Initializes a new group parser
        /// </summary>
        public GroupParser(char open, char close)
        {
            Open = open; Close = close;
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
