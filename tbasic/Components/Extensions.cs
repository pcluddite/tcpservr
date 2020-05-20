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

        public static int IndexOfIgnoreCase(this string initial, string other, int start)
        {
            return initial.IndexOf(other, start, ComparisonType);
        }

        public static int SkipWhiteSpace(this string str, int start = 0)
        {
            for(int index = start; index < str.Length; ++index) {
                if (!char.IsWhiteSpace(str[index])) {
                    return index;
                }
            }
            return -1;
        }
    }
}
