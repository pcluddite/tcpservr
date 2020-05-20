// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Parsing
{
    internal static unsafe class MatchFast
    {
        public static int MatchIdentifier(string buff)
        {
            int end = 0;
            if (char.IsLetter(buff[end]) || buff[end] == '_') {
                return FindAcceptableFuncChars(buff, ++end);
            }
            else {
                return -1;
            }
        }
        
        public static int MatchVariable(string buff)
        {
            if (buff.CharAt(0) == '$' || buff.CharAt(0) == '@') { // it's a macro
                if (buff.CharAt(1) == -1)
                    return -1;
                int end = FindAcceptableFuncChars(buff, 1);
                if (end != 0) {
                    return end;
                }
            }
            return -1;
        }

        public static int MatchNumber(string buff)
        {
            int end = FindConsecutiveDigits(buff, 0);
            if (end == 0)
                return -1; // nothing was found

            if (buff.CharAt(end) == '.')
                end = FindConsecutiveDigits(buff, ++end);
            
            if (buff.CharAt(end) == 'e' || buff.CharAt(end) == 'E') {
                if (buff.CharAt(++end) == '-' || buff.CharAt(end) == '+')
                    ++end;

                end = FindConsecutiveDigits(buff, end);
            }

            return end;
        }

        private static unsafe int FindConsecutiveDigits(string buff, int start)
        {
            fixed (char* lpseg = buff) {
                int len = buff.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!char.IsDigit(lpseg[index])) {
                        return index;
                    }
                }
                return index;
            }
        }

        public static int MatchHex(string buff)
        {
            int end = 0;
            if (buff.CharAt(end) != '0')
                return -1;
            if (buff.CharAt(++end) != 'x' && buff.CharAt(end) != 'X')
                return -1;

            end = FindConsecutiveHex(buff, ++end);

            return end;
        }

        private static unsafe int FindConsecutiveHex(string buff, int start)
        {
            fixed (char* lpseg = buff) {
                int len = buff.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!IsHexDigit(lpseg[index])) {
                        return index;
                    }
                }
                return index;
            }
        }

        private static bool IsHexDigit(char c)
        {
            return char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }


        private static unsafe int FindAcceptableFuncChars(string buff, int start)
        {
            fixed (char* lpseg = buff) {
                int len = buff.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!char.IsLetterOrDigit(lpseg[index]) && lpseg[index] != '_') {
                        return index;
                    }
                }
                return index;
            }
        }

        internal static unsafe bool StartsWithFast(string s, string pattern, bool ignoreCase)
        {
            if (s == null || pattern == null)
                return false;
            if (pattern.Length > s.Length)
                return false;
            
            fixed(char* lpstr = s) fixed(char* lppat = pattern) {
                for (int i = 0, len = pattern.Length; i < len; ++i) {
                    char a, b;
                    if (ignoreCase) {
                        a = char.ToLower(lpstr[i]);
                        b = char.ToLower(lppat[i]);
                    }
                    else {
                        a = lpstr[i];
                        b = lppat[i];
                    }
                    if (a != b)
                        return false;
                }
            }
            return true;
        }
    }
}
