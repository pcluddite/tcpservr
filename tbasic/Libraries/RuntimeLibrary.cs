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
using Tbasic.Runtime;
using Tbasic.Errors;

namespace Tbasic.Libraries
{
    internal class RuntimeLibrary : Library
    {
        public RuntimeLibrary(ObjectContext context)
        {
            Add("SizeOf", SizeOf);
            Add("Len", SizeOf);
            Add("IsStr", IsString);
            Add("IsInt", IsInt);
            Add("IsDouble", IsDouble);
            Add("IsBool", IsBool);
            Add("IsDefined", IsDefined);
            Add("IsByte", IsByte);
            Add("Str", ToString);
            Add("Double", ToDouble);
            Add("Int", ToInt);
            Add("Bool", ToBool);
            Add("Byte", ToByte);
            Add("Char", ToChar);
            AddLibrary(new StringLibrary());
            AddLibrary(new ArrayLibrary());
            context.SetConstant("@version", Executer.VERSION);
            context.SetConstant("@osversion", Environment.OSVersion.VersionString);
        }

        private void ToChar(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<char>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be char");
            }
        }

        private void ToString(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<string>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be string");
            }
        }

        private void ToBool(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<bool>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be bool");
            }
        }

        private void ToDouble(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<double>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be double");
            }
        }

        private void ToInt(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<int>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be int");
            }
        }

        private void ToByte(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            try {
                stackFrame.Data = stackFrame.GetParameter<byte>(1);
            }
            catch (InvalidCastException) {
                throw new TbasicException(ErrorClient.BadRequest, "Parameter cannot be byte");
            }
        }

        private void SizeOf(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            object obj = stackFrame.GetParameter(1);
            int len = -1;
            if (obj == null) {
                len = 0;
            }
            else if (obj is string) {
                len = obj.ToString().Length;
            }
            else if (obj is int) {
                len = sizeof(int);
            }
            else if (obj is double) {
                len = sizeof(double);
            }
            else if (obj is bool) {
                len = sizeof(bool);
            }
            else if (obj.GetType().IsArray) {
                len = ((object[])obj).Length;
            }
            else {
                throw new TbasicException(ErrorClient.Forbidden, "Object size cannot be determined");
            }
            stackFrame.Data = len;
        }

        private void IsInt(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1) is int;
        }

        private void IsString(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1) is string;
        }

        private void IsBool(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1) is bool;
        }

        private void IsDouble(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1) is byte;
        }

        private void IsDefined(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            string name = stackFrame.GetParameter<string>(1);
            ObjectContext context = stackFrame.Context.FindContext(name);
            stackFrame.Data = context != null;
        }

        private void IsByte(TFunctionData stackFrame)
        {
            stackFrame.AssertParamCount(2);
            stackFrame.Data = stackFrame.GetParameter(1) is byte;
        }
    }
}