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

namespace Tbasic.Operators
{
    internal partial class UnaryOpDictionary
    {
        private Dictionary<string, UnaryOperator> unaryOps = new Dictionary<string, UnaryOperator>(22 /* magic number of standard operators */, StringComparer.OrdinalIgnoreCase);

        public void LoadStandardOperators()
        {
            unaryOps.Add("+", new UnaryOperator("+", Plus));
            unaryOps.Add("-", new UnaryOperator("-", Minus));
            unaryOps.Add("NOT ", new UnaryOperator("NOT ", Not));
            unaryOps.Add("~", new UnaryOperator("~", BitNot));
        }
        
        private static object Plus(object value)
        {
            return +Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }

        private static object Minus(object value)
        {
            return -Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }

        private static object Not(object value)
        {
            return !Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }

        private static object BitNot(object value)
        {
            return ~Convert.ToUInt64(value, CultureInfo.CurrentCulture);
        }
    }
}
