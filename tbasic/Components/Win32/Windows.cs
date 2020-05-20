// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tbasic.Win32
{
    /// <summary>
    /// A managed wrapper for common win32 window functions
    /// </summary>
    internal class Windows
    {
        public static bool WinExists(IntPtr hwnd)
        {
            RECT rect;
            return User32.GetWindowRect(hwnd, out rect);
        }

        public static WindowFlag GetState(IntPtr hwnd)
        {
            IntPtr exists = User32.GetWindow(hwnd, 0);
            WindowFlag state = WindowFlag.Existing;
            if (exists == IntPtr.Zero) { return 0; }
            if (User32.IsWindowVisible(hwnd)) { state |= WindowFlag.Visible; }
            if (User32.IsWindowEnabled(hwnd)) { state |= WindowFlag.Enable; }
            if (User32.GetForegroundWindow() == hwnd) { state |= WindowFlag.Active; }
            WINDOWPLACEMENT plac;
            User32.GetWindowPlacement(hwnd, out plac);
            if (plac.showCmd == 2) { state |= WindowFlag.Minimized; }
            if (plac.showCmd == 3) { state |= WindowFlag.Maximized; }
            return state;
        }

        public static IEnumerable<IntPtr> List()
        {
            return (new WinLister()).List();
        }

        public static IEnumerable<IntPtr> List(WindowFlag flag)
        {
            var results =
                from hwnd in List()
                where (GetState(hwnd) & flag) == flag
                select hwnd;

            return results;
        }

        private class WinLister
        {
            private Stack<IntPtr> list;
            private static CallBackPtr callBackPtr;

            public WinLister()
            {
                list = new Stack<IntPtr>();
            }

            private bool Report(IntPtr hwnd, int lParam)
            {
                list.Push(hwnd);
                return true;
            }

            public IEnumerable<IntPtr> List()
            {
                list.Clear();
                callBackPtr = new CallBackPtr(Report);
                User32.EnumWindows(callBackPtr, 0);
                return list;
            }
        }
    }
}
