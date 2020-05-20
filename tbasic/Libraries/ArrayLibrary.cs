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
using System.Linq;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    internal class ArrayLibrary : Library
    {
        public ArrayLibrary()
        {
            Add("ArrayContains", ArrayContains);
            Add("ArrayIndexOf", ArrayIndexOf);
            Add("ArrayLastIndexOf", ArrayLastIndexOf);
            //Add("ArrayResize", ArrayResize);
        }

        private void ArrayContains(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter<object[]>(1).Contains(stackFrame.GetParameter(2));
        }

        private void ArrayIndexOf(TFunctionData stackFrame)
        {
            object[] arr = stackFrame.GetParameter<object[]>(1);
            if (stackFrame.ParameterCount == 3) {
                stackFrame.AddParameter(0);
            }
            if (stackFrame.ParameterCount == 4) {
                stackFrame.AddParameter(arr.Length);
            }
            stackFrame.AssertParamCount(5);
            object o = stackFrame.GetParameter(2);
            int i = stackFrame.GetParameter<int>(3);
            int count = stackFrame.GetParameter<int>(5);
            for (; i < arr.Length && i < count; i++) {
                if (arr[i] == o) {
                    stackFrame.Data = i;
                    return;
                }
            }
            stackFrame.Data = -1;
        }

        private void ArrayLastIndexOf(TFunctionData stackFrame)
        {
            object[] arr = stackFrame.GetParameter<object[]>(1);
            if (stackFrame.ParameterCount == 3) {
                stackFrame.AddParameter(0);
            }
            if (stackFrame.ParameterCount == 4) {
                stackFrame.AddParameter(arr.Length);
            }
            stackFrame.AssertParamCount(5);
            int i = stackFrame.GetParameter<int>(3);
            object o = stackFrame.GetParameter(2);
            int count = stackFrame.GetParameter<int>(5);
            for (; i >= 0 && i > count; i--) {
                if (arr[i] == o) {
                    stackFrame.Data = i;
                    return;
                }
            }
            stackFrame.Data = -1;
        }
    }
}