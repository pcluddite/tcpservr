// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Microsoft.Win32;
using System;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    /// <summary>
    /// Library for interacting with Windows registry.
    /// </summary>
    internal class RegistryLibrary : Library
    {
        public RegistryLibrary()
        {
            Add("RegEnumKeys", RegEnumKeys);
            Add("RegEnumValues", RegEnumValues);
            Add("RegRenameKey", RegRenameKey);
            Add("RegRename", RegRename);
            Add("RegDelete", RegDelete);
            Add("RegDeleteKey", RegDeleteKey);
            Add("RegCreateKey", RegCreateKey);
            Add("RegRead", RegRead);
            Add("RegWrite", RegWrite);
        }
        
        private object RegValueKind(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            return WinRegistry.GetValueKind(stackdat.Get<string>(1), stackdat.Get<string>(2)).ToString();
        }

        private object RegRead(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(atLeast: 3, atMost: 4);

            if (stackdat.ParameterCount == 3)
                stackdat.Add((object)null);
            
            return WinRegistry.Read(stackdat.Get<string>(1), stackdat.Get<string>(2), stackdat.Get<string>(3));
        }

        private object RegDelete(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            WinRegistry.Delete(stackdat.Get<string>(1), stackdat.Get<string>(2));
            return null;
        }

        private object RegRename(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(4);
            WinRegistry.Rename(stackdat.Get<string>(1), stackdat.Get<string>(2), stackdat.Get<string>(3));
            return null;
        }

        private object RegDeleteKey(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            WinRegistry.DeleteKey(stackdat.Get<string>(1));
            return null;
        }

        private object RegRenameKey(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            WinRegistry.RenameKey(stackdat.Get<string>(1), stackdat.Get<string>(2));
            return null;
        }

        private object RegCreateKey(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            WinRegistry.RenameKey(stackdat.Get<string>(1), stackdat.Get<string>(2));
            stackdat.Status = ErrorSuccess.Created;
            return null;
        }
        
        private object RegEnumValues(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);

            object[][] values = WinRegistry.EnumerateValues(stackdat.Get<string>(1));
            if (values.Length == 0) {
                stackdat.Status = ErrorSuccess.NoContent;
                return null;
            }
            else {
                return values;
            }
        }

        private static object RegEnumKeys(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return WinRegistry.EnumeratKeys(stackdat.Get<string>(1));
        }

        private object RegWrite(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(5);

            object value = stackdat.Get(3);
            RegistryValueKind kind = stackdat.Get<RegistryValueKind>(4);

            switch (kind) {
                case RegistryValueKind.Binary:
                    value = Convert.FromBase64String(stackdat.Get<string>(3));
                    break;
                case RegistryValueKind.MultiString:
                    string strval = value as string;
                    if (value is string) {
                        value = stackdat.Get<string>(3).Replace("\r\n", "\n").Split('\n');
                    }
                    else if (value is string[]) {
                        value = stackdat.Get<string[]>(3);
                    }
                    else {
                        throw new ArgumentException("Parameter is not a valid multi-string");
                    }
                    break;
                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                case RegistryValueKind.String:
                    // natively supported
                    break;
                default:
                    throw new ArgumentException("Registry value of type '" + kind + "' is unsupported");
            }
            WinRegistry.Write(stackdat.Get<string>(1), stackdat.Get<string>(2), value, kind);
            return null;
        }
    }
}