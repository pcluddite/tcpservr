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
