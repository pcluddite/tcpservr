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
using System.Text;
using System.Text.RegularExpressions;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    internal class StringLibrary : Library
    {
        public StringLibrary()
        {
            Add("StrContains", StringContains);
            Add("StrIndexOf", StringIndexOf);
            Add("StrLastIndexOf", StringLastIndexOf);
            Add("StrUpper", StringUpper);
            Add("StrCompare", StringCompare);
            Add("StrLower", StringLower);
            Add("StrLeft", StringLeft);
            Add("StrRight", StringRight);
            Add("StrTrim", Trim);
            Add("StrTrimStart", TrimStart);
            Add("StrTrimEnd", TrimEnd);
            Add("StrSplit", StringSplit);
            Add("StrToChars", ToCharArray);
            Add("CharsToStr", CharsToString);
            Add("StrInStr", Substring);
        }

        private void CharsToString(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            StringBuilder hanz = new StringBuilder();
            foreach (char c in stackFrame.GetParameter<char[]>(1)) {
                hanz.Append(c);
            }
            stackFrame.Data = hanz.ToString();
        }

        private void ToCharArray(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter<string>(1).ToCharArray();
        }

        private void StringSplit(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(3);
            stackFrame.Data = Regex.Split(stackFrame.GetParameter(1).ToString(), stackFrame.GetParameter(2).ToString());
        }

        private void Trim(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1).ToString().Trim();
        }

        private void TrimStart(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1).ToString().TrimStart();
        }

        private void TrimEnd(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1).ToString().TrimEnd();
        }

        private void StringContains(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(3);
            stackFrame.Data = stackFrame.GetParameter<string>(1).Contains(stackFrame.GetParameter<string>(2));
        }

        private void StringCompare(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(3);
            stackFrame.Data = stackFrame.GetParameter<string>(1).CompareTo(stackFrame.GetParameter<string>(2));
        }

        private void StringIndexOf(TFunctionData stackFrame)
        {
            if (stackFrame.ParameterCount == 3) {
                stackFrame.AddParameter(0);
            }
            if (stackFrame.ParameterCount == 4) {
                stackFrame.AddParameter(stackFrame.GetParameter<string>(1).Length);
            }
            stackFrame.AssertParamCount(5);
            char? cObj = stackFrame.GetParameter(2) as char?;
            if (cObj == null) {
                stackFrame.Data = stackFrame.GetParameter<string>(1).IndexOf(stackFrame.GetParameter<string>(2), stackFrame.GetParameter<int>(3), stackFrame.GetParameter<int>(4));
            }
            else {
                stackFrame.Data = stackFrame.GetParameter<string>(1).IndexOf(cObj.Value, stackFrame.GetParameter<int>(3), stackFrame.GetParameter<int>(4));
            }
        }

        private void StringLastIndexOf(TFunctionData stackFrame)
        {
            if (stackFrame.ParameterCount == 3) {
                stackFrame.AddParameter(0);
            }
            if (stackFrame.ParameterCount == 4) {
                stackFrame.AddParameter(stackFrame.GetParameter<string>(1).Length);
            }
            stackFrame.AssertParamCount(5);
            char? cObj = stackFrame.GetParameter(2) as char?;
            if (cObj == null) {
                stackFrame.Data = stackFrame.GetParameter<string>(1).LastIndexOf(stackFrame.GetParameter<string>(2), stackFrame.GetParameter<int>(3), stackFrame.GetParameter<int>(4));
            }
            else {
                stackFrame.Data = stackFrame.GetParameter<string>(1).LastIndexOf(cObj.Value, stackFrame.GetParameter<int>(3), stackFrame.GetParameter<int>(4));
            }
        }

        private void StringUpper(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter<string>(1).ToUpper();
        }

        private void StringLower(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter<string>(1).ToLower();
        }

        private void StringLeft(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(3);
            stackFrame.Data = stackFrame.GetParameter<string>(1).Substring(stackFrame.GetParameter<int>(2));
        }

        private void StringRight(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(3);
            stackFrame.Data = stackFrame.GetParameter<string>(1).Remove(stackFrame.GetParameter<int>(2));
        }

        private void Substring(TFunctionData stackFrame)
        {
            if (stackFrame.ParameterCount == 3) {
                stackFrame.Data = stackFrame.GetParameter<string>(1).Substring(
                                    stackFrame.GetParameter<int>(2)
                                    );
            }
            else {
                stackFrame.AssertParamCount(4);
                stackFrame.Data = stackFrame.GetParameter<string>(1).Substring(
                                    stackFrame.GetParameter<int>(2), stackFrame.GetParameter<int>(3)
                                    );
            }
        }
    }
}