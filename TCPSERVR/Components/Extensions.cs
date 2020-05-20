// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace TCPSERVR.Components
{
    internal static class Extensions
    {
        public static string Center(this string initial, int finalLength, char padChar = ' ')
        {
            int start = (finalLength - initial.Length) / 2;
            if (start <= 0) {
                return initial;
            }
            int endLength = (finalLength - initial.Length);
            string line = string.Format("{0,-" + start + "}{1}{2," + endLength + "}", padChar, initial, padChar);
            return line;
        }

        public static string Clip(this string s, int maxSize)
        {
            if (s.Length < maxSize) {
                return s;
            }
            else {
                return s.Remove(maxSize - 5) + "[...]";
            }
        }
        
        public static string ToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null) {
                throw new ArgumentNullException("securePassword");
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static bool SecureStringEqual(this SecureString privateKey, SecureString testString)
        {
            if (testString == null) {
                throw new ArgumentNullException(nameof(testString));
            }
            if (privateKey == null) {
                throw new ArgumentNullException(nameof(privateKey));
            }

            if (testString.Length != privateKey.Length) {
                return false;
            }

            IntPtr ss_bstr1_ptr = IntPtr.Zero;
            IntPtr ss_bstr2_ptr = IntPtr.Zero;

            try {
                ss_bstr1_ptr = Marshal.SecureStringToBSTR(testString);
                ss_bstr2_ptr = Marshal.SecureStringToBSTR(privateKey);

                string str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                string str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                return str1.Equals(str2);
            }
            finally {
                if (ss_bstr1_ptr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                }

                if (ss_bstr2_ptr != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                }
            }
        }
    }
}