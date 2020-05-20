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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
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

        private static RegistryKey GetRootKey(string key)
        {
            key = key.ToUpper();
            if (key.StartsWith("HKEY_CURRENT_USER")) {
                return Registry.CurrentUser;
            }
            else if (key.StartsWith("HKEY_CLASSES_ROOT")) {
                return Registry.ClassesRoot;
            }
            else if (key.StartsWith("HKEY_LOCAL_MACHINE")) {
                return Registry.LocalMachine;
            }
            else if (key.StartsWith("HKEY_USERS")) {
                return Registry.Users;
            }
            else if (key.StartsWith("HKEY_CURRENT_CONFIG")) {
                return Registry.CurrentConfig;
            }
            return null;
        }

        private static string RemoveKeyRoot(string key)
        {
            int indexOfRoot = key.IndexOf('\\');
            if (indexOfRoot < 0) {
                return "";
            }
            string ret = key.Remove(0, indexOfRoot);
            while (ret.StartsWith("\\")) {
                ret = ret.Remove(0, 1);
            }
            return ret;
        }

        private void RegValueKind(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            using (RegistryKey key = OpenKey(_sframe.GetParameter<string>(1), false)) {
                _sframe.Data = key.GetValueKind(_sframe.GetParameter<string>(2)).ToString();
            }
        }

        private RegistryValueKind RegValueKind(string key, string value)
        {
            RegistryKey keyBase = GetRootKey(key);
            using (keyBase = keyBase.OpenSubKey(RemoveKeyRoot(key))) {
                RegistryValueKind kind = keyBase.GetValueKind(value);
                return kind;
            }
        }

        private void RegRead(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);

            object ret = RegRead(_sframe.GetParameter<string>(1), _sframe.GetParameter<string>(2));

            if (ret == null) {
                _sframe.Status = ErrorClient.NotFound;
            }
            else {
                _sframe.Data = ret;
            }
        }

        public object RegRead(string key, string value)
        {
            return RegistryUtilities.Read(GetRootKey(key), RemoveKeyRoot(key), value, null);
        }

        private void RegDelete(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            RegistryKey key = GetRootKey(_sframe.GetParameter<string>(1));
            using (key = key.OpenSubKey(RemoveKeyRoot(_sframe.GetParameter<string>(1)), true)) {
                key.DeleteValue(_sframe.GetParameter<string>(2), true);
            }
        }

        private void RegRename(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(4);
            RegistryKey key = GetRootKey(_sframe.GetParameter<string>(1));
            using (key = key.OpenSubKey(RemoveKeyRoot(_sframe.GetParameter<string>(1)), true)) {
                key.SetValue(_sframe.GetParameter<string>(3), key.GetValue(_sframe.GetParameter<string>(2)), key.GetValueKind(_sframe.GetParameter<string>(2)));
                key.DeleteValue(_sframe.GetParameter<string>(2), true);
            }
        }

        private void RegDeleteKey(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            using (RegistryKey key = GetRootKey(_sframe.GetParameter<string>(1))) {
                key.DeleteSubKeyTree(RemoveKeyRoot(_sframe.GetParameter<string>(1)));
            }
        }

        private void RegRenameKey(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            using (RegistryKey key = OpenParentKey(_sframe.GetParameter<string>(1), true)) {
                RegistryUtilities.RenameSubKey(key, RemoveKeyRoot(_sframe.GetParameter<string>(1)), _sframe.GetParameter<string>(2));
            }
        }

        private void RegCreateKey(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            using (RegistryKey key = OpenKey(_sframe.GetParameter<string>(1), true)) {
                key.CreateSubKey(_sframe.GetParameter<string>(2));
                _sframe.Status = ErrorSuccess.Created;
            }
        }
        
        private void RegEnumValues(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);

            List<object[]> values = new List<object[]>();

            using (RegistryKey key = OpenKey(_sframe.GetParameter<string>(1), false)) {
                foreach (string valueName in key.GetValueNames()) {
                    values.Add(new object[] { valueName, key.GetValue(valueName) });
                }
            }
            if (values.Count == 0) {
                _sframe.Status = ErrorSuccess.NoContent;
            }
            else {
                _sframe.Data = values.ToArray();
            }
        }

        public static RegistryKey OpenKey(string path, bool write)
        {
            using (RegistryKey key = GetRootKey(path)) {
                return key.OpenSubKey(RemoveKeyRoot(path), write);
            }
        }

        public static RegistryKey OpenParentKey(string path, bool write)
        {
            int indexOfLast = path.LastIndexOf('\\');
            if (indexOfLast > -1) {
                path = path.Substring(0, indexOfLast);
            }
            return OpenKey(path, write);
        }

        private static void RegEnumKeys(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            using (RegistryKey key = OpenKey(_sframe.GetParameter<string>(1), false)) {
                _sframe.Data = key.GetSubKeyNames();
            }
        }

        private void RegWrite(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(5);

            object value = _sframe.GetParameter(3);

            RegistryValueKind kind;
            switch (_sframe.GetParameter<string>(4).ToLower()) {
                case "binary":
                    kind = RegistryValueKind.Binary;
                    List<string> slist = new List<string>();
                    slist.AddRange(_sframe.GetParameter<string>(3).Split(' '));
                    value = slist.ConvertAll(s => Convert.ToByte(s, 16)).ToArray();
                    break;
                case "dword":
                    kind = RegistryValueKind.DWord;
                    break;
                case "expandstring":
                    kind = RegistryValueKind.ExpandString;
                    break;
                case "multistring":
                    kind = RegistryValueKind.MultiString;
                    if (value is string) {
                        value = _sframe.GetParameter<string>(3).Replace("\r\n", "\n").Split('\n');
                    }
                    else if (value is string[]) {
                        value = _sframe.GetParameter<string[]>(3);
                    }
                    else {
                        throw new ArgumentException("Parameter is not a valid multi-string");
                    }
                    break;
                case "qword":
                    kind = RegistryValueKind.QWord;
                    break;
                case "string":
                    kind = RegistryValueKind.String;
                    break;
                default:
                    throw new ArgumentException("Unknown registry type '" + _sframe.GetParameter(4) + "'");
            }

            using (RegistryKey key = OpenKey(_sframe.GetParameter<string>(1), true)) {
                key.SetValue(_sframe.GetParameter<string>(2), value, kind);
            }
        }
    }
}