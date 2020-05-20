// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Tbasic.Components.Win32
{
    internal class Impersonate
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        public static int RunAs(string user, string pass, Delegate method, params object[] parms)
        {
            string domain = ".";

            int indexOfSlash = user.IndexOf('\\');
            if (indexOfSlash > 0) { // separate user from domain
                domain = user.Remove(indexOfSlash);
                user = user.Substring(indexOfSlash + 1);
            }

            return RunAs(user, pass, domain, method, parms);
        }

        public static int RunAs(string user, string pass, string domain, Delegate method, params object[] parms)
        {
            WindowsIdentity identity = null;
            WindowsImpersonationContext wic = null;

            if (string.IsNullOrEmpty(domain))
                domain = ".";

            try {
                IntPtr token;
                if (LogonUser(user, domain, pass, 0x09, 0x00, out token)) {
                    identity = new WindowsIdentity(token);
                    wic = identity.Impersonate(); // begin the impersonation

                    method.DynamicInvoke(parms);

                    return 0;
                }
                else {
                    return Marshal.GetLastWin32Error();
                }
            }
            finally {
                wic?.Undo(); // stop impersonation
            }
        }
    }
}
