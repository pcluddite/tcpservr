// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;

namespace Tbasic.Win32 {
    internal enum WindowState : int {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9
    }

    [Flags]
    internal enum WindowFlag : int {
        None = 0x00,
        Existing = 0x01,
        Visible = 0x02,
        Enable = 0x04,
        Active = 0x08,
        Minimized = 0x10,
        Maximized = 0x20
    }
}
