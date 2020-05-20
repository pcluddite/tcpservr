// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Drawing;
using Tbasic.Win32;

namespace Tbasic.Components
{
    /// <summary>
    /// Class for getting a screen shot of the whole screen or a specific window
    /// </summary>
    internal static class ScreenCapture
    {
        public static Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public static Image CaptureWindow(IntPtr hWnd)
        {
            IntPtr hDest = IntPtr.Zero,
                   hSource = IntPtr.Zero,
                   hBitmap = IntPtr.Zero,
                   hBmpOld;
            try {
                RECT rect;
                if (!User32.GetWindowRect(hWnd, out rect))
                    return null;

                int nWidth = rect.Right - rect.Left,
                    nHeight = rect.Bottom - rect.Top;

                hSource = User32.GetWindowDC(hWnd);
                hDest = GDI32.CreateCompatibleDC(hSource);
                hBitmap = GDI32.CreateCompatibleBitmap(hSource, nWidth, nHeight);
                hBmpOld = GDI32.SelectObject(hSource, hBitmap);

                if (!GDI32.BitBlt(hDest, 0, 0, nWidth, nHeight, hSource, 0, 0, GDI32.SRCCOPY))
                    return null;

                GDI32.SelectObject(hDest, hBmpOld);
                return Image.FromHbitmap(hBitmap);
            }
            finally {
                // clean up unmanaged resources
                GDI32.DeleteDC(hDest);
                User32.ReleaseDC(hWnd, hSource);
                GDI32.DeleteObject(hBitmap);
            }
        }
    }
}
