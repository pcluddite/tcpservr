// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Microsoft.Win32;
using System.Collections.Generic;

namespace Tbasic.Components
{
    internal static class WinRegistry
    {
        public static object Read(string keyPath, string name, object defaultValue = null)
        {
            using (RegistryKey key = OpenKey(keyPath)) {
                return key.GetValue(name, defaultValue);
            }
        }

        public static void Write(string keyPath, string name, object val)
        {
            using (RegistryKey key = OpenKey(keyPath, write: true)) {
                key.SetValue(name, val);
            }
        }

        public static void Write(string keyPath, string name, object val, RegistryValueKind kind)
        {
            using (RegistryKey key = OpenKey(keyPath, write: true)) {
                key.SetValue(name, val, kind);
            }
        }

        public static RegistryKey CreateKey(string keyPath)
        {
            using (RegistryKey baseKey = GetPathRoot(keyPath)) {
                return baseKey.CreateSubKey(RemovePathRoot(keyPath));
            }
        }
        
        public static void CopyKey(string keyPath, string newKeyPath)
        {
            using (RegistryKey src = OpenKey(keyPath), dest = CreateKey(keyPath)) {
                CopyKey(src, dest);
            }
        }

        public static void CopyKey(RegistryKey src, RegistryKey dest)
        {
            string[] names = src.GetValueNames();
            for(int index = 0; index < names.Length; ++index) {
                dest.SetValue(names[index], src.GetValue(names[index]), src.GetValueKind(names[index]));
            }

            string[] keys = src.GetSubKeyNames();
            for(int index = 0; index < names.Length; ++index) {
                CopyKey(src.OpenSubKey(keys[index]), dest.CreateSubKey(keys[index]));
            }
        }

        private static RegistryKey GetPathRoot(string key)
        {
            key = key.RemoveFromChar('\\').ToUpper();
            switch(key) {
                case "HKEY_CURRENT_USER":
                case "HKCU":
                    return Registry.CurrentUser;
                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    return Registry.LocalMachine;
                case "HKEY_CLASSES_ROOT":
                case "HKCR":
                    return Registry.ClassesRoot;
                case "HKEY_CURRENT_CONFIG":
                case "HKCC":
                    return Registry.CurrentConfig;
                case "HKEY_USERS":
                    return Registry.Users;
            }
            return null;
        }

        private static string RemovePathRoot(string key)
        {
            return key.RemoveToChar('\\');
        }

        public static RegistryValueKind GetValueKind(string key, string valuename)
        {
            using (RegistryKey root = GetPathRoot(key), subkey = root.OpenSubKey(RemovePathRoot(key))) {
                return subkey.GetValueKind(valuename);
            }
        }

        public static RegistryKey OpenKey(string path, bool write = false)
        {
            using (RegistryKey key = GetPathRoot(path)) {
                return key.OpenSubKey(RemovePathRoot(path), write);
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

        public static void Rename(string keyPath, string valueName, string newName)
        {
            using (RegistryKey key = OpenKey(keyPath, write: true)) {
                key.SetValue(newName, key.GetValue(valueName), key.GetValueKind(valueName));
                key.DeleteValue(valueName);
            }
        }

        public static void RenameKey(string keyPath, string newKeyPath)
        {
            using (RegistryKey src = OpenKey(keyPath), dest = CreateKey(keyPath)) {
                CopyKey(src, dest);
                DeleteKey(newKeyPath);
            }
        }

        public static void Delete(string keyPath, string valueName)
        {
            using (var key = OpenKey(keyPath, write: true)) {
                key.DeleteValue(valueName, throwOnMissingValue: true);
            }
        }

        public static void DeleteKey(string keyPath)
        {
            using (var key = OpenParentKey(keyPath, write: true)) {
                key.DeleteSubKeyTree(RemovePathRoot(keyPath), throwOnMissingSubKey: true);
            }
        }

        public static object[][] EnumerateValues(string keyPath)
        {
            using (var key = OpenKey(keyPath)) {
                string[] valuenames = key.GetValueNames();
                List<object[]> values = new List<object[]>(valuenames.Length);

                foreach(string name in valuenames)
                    values.Add(new object[] { name, key.GetValue(name) });

                return values.ToArray();
            }
        }

        public static string[] EnumeratKeys(string keyPath)
        {
            using (var key = OpenKey(keyPath)) {
                return key.GetSubKeyNames();
            }
        }
    }
}
