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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tbasic.Win32;
using Tbasic.Errors;
using Forms = System.Windows.Forms;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    /// <summary>
    /// A library used to automate and manipulate key strokes, mouse clicks and other input
    /// </summary>
    public class AutoLibrary : Library
    {

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public AutoLibrary()
        {
            Add("MouseClick", MouseClick);
            Add("BlockInput", BlockInput);
            Add("MouseMove", MouseMove);
            Add("Send", Send);
            Add("VolumeUp", VolumeUp);
            Add("VolumeDown", VolumeDown);
            Add("VolumeMute", VolumeMute);
        }

        /// <summary>
        /// Mouse buttons
        /// </summary>
        public enum MouseButton : long
        {
            /// <summary>
            /// The left mouse button
            /// </summary>
            Left = MouseEvents.MOUSEEVENTF_LEFTDOWN | MouseEvents.MOUSEEVENTF_LEFTUP,
            /// <summary>
            /// The right mouse button
            /// </summary>
            Right = MouseEvents.MOUSEEVENTF_RIGHTDOWN | MouseEvents.MOUSEEVENTF_RIGHTUP
        }

        /// <summary>
        /// Clicks the mouse a specified number of times
        /// </summary>
        /// <param name="button">the mouse button to press</param>
        /// <param name="x">the final x position of the cursor</param>
        /// <param name="y">the final y position of the cursor</param>
        /// <param name="clicks">the number of clicks</param>
        /// <param name="delay">the delay to cursor motion (higher numbers are slower)</param>
        public static void MouseClick(MouseButton button, int x, int y, int clicks = 1, int delay = 1)
        {
            MouseMove(x, y, delay);
            long action = (long)button;
            for (int i = 0; i < clicks; i++) {
                User32.mouse_event(action, x, y, 0, 0);
            }
        }

        private void MouseClick(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 4) {
                _sframe.AddParameter(1);
                _sframe.AddParameter(1);
            }
            if (_sframe.ParameterCount == 5) {
                _sframe.AddParameter(5);
            }
            _sframe.AssertParamCount(6);

            int x = _sframe.GetParameter<int>(2),
                y = _sframe.GetParameter<int>(3),
                clicks = _sframe.GetParameter<int>(4),
                speed = _sframe.GetParameter<int>(5);

            MouseButton button;
            if (_sframe.GetFromEnum(1, "button", "LEFT", "RIGHT").EqualsIgnoreCase("LEFT")) {
                button = MouseButton.Left;
            }
            else {
                button = MouseButton.Right;
            }
            MouseClick(button, x, y, clicks, speed);
        }

        private void MouseMove(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 3) {
                _sframe.AddParameter(1);
            }
            _sframe.AssertParamCount(4);

            MouseMove(_sframe.GetParameter<int>(1),
                      _sframe.GetParameter<int>(2),
                      _sframe.GetParameter<int>(3));
        }

        /// <summary>
        /// Moves the mouse to a specified position on the screen
        /// </summary>
        /// <param name="endX">the final x position of the cursor</param>
        /// <param name="endY">the final y position of the cursor</param>
        /// <param name="delay">the delay to cursor motion (higher numbers are slower)</param>
        public static void MouseMove(double endX, double endY, int delay = 1)
        {
            if (delay > 0) {
                double startX = Cursor.Position.X,
                       startY = Cursor.Position.Y;

                double direction = startX < endX ? 1 : -1;
                double slope = (endY - startY) / (endX - startX);
                delay = (int)(Math.Sqrt(delay));

                double oldX = startX;
                for (double x = startX; !IsBetween(endX, oldX, x); x += direction) {
                    double y = slope * (x - startX) + startY;
                    int newX = (int)(x + 0.5),
                        newY = (int)(y + 0.5);
                    System.Threading.Thread.Sleep(delay);
                    oldX = x;
                    Cursor.Position = new Point(newX, newY);

                }
            }
            Cursor.Position = new Point((int)endX, (int)endY);
        }

        private static bool IsBetween(double x, double d1, double d2)
        {
            if (d1 < d2) {
                return (x <= d2) && (x >= d1);
            }
            else {
                return (x <= d1) && (x >= d2);
            }
        }

        /// <summary>
        /// Blocks user input
        /// </summary>
        /// <param name="blocked">true will block, false will unblock</param>
        /// <returns>true if the operation succeeded, otherwise false</returns>
        public static bool BlockInput(bool blocked = true)
        {
            return User32.BlockInput(blocked);
        }

        private void BlockInput(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            _sframe.SetAll(_sframe.GetParameter(0), _sframe.GetParameter(1).ToString().Replace("1", "true").Replace("0", "false"));
            if (!BlockInput(_sframe.GetParameter<bool>(1))) {
                _sframe.Status = ErrorClient.Forbidden;
            }
        }

        /// <summary>
        /// Sends keys to the active window
        /// </summary>
        /// <param name="keys">the formatted key string</param>
        public static void Send(string keys)
        {
            SendKeys.SendWait(keys);
        }

        private void Send(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            Send(_sframe.GetParameter<string>(1));
        }

        /// <summary>
        /// Press the volume up key a given number of times
        /// </summary>
        /// <param name="amnt">number of times to press the key</param>
        public static void VolumeUp(int amnt = 1)
        {
            for (int i = 0; i < amnt; i++) {
                User32.keybd_event((byte)Forms.Keys.VolumeUp, 0, 0, 0);
            }
        }

        private void VolumeUp(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 1) {
                _sframe.SetAll(_sframe.GetParameter(0), "1");
            }
            _sframe.AssertParamCount(2);
            VolumeUp(_sframe.GetParameter<int>(1));
        }

        /// <summary>
        /// Press the volume down key a given number of times
        /// </summary>
        /// <param name="amnt">number of times to press the key</param>
        public static void VolumeDown(int amnt = 1)
        {
            for (int i = 0; i < amnt; i++) {
                User32.keybd_event((byte)Forms.Keys.VolumeDown, 0, 0, 0);
            }
        }

        private void VolumeDown(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 1) {
                _sframe.SetAll(_sframe.GetParameter(0), "1");
            }
            _sframe.AssertParamCount(2);
            VolumeDown(_sframe.GetParameter<int>(1));
        }

        /// <summary>
        /// Toggle volume mute
        /// </summary>
        public static void VolumeMute()
        {
            User32.keybd_event((byte)Forms.Keys.VolumeMute, 0, 0, 0);
        }

        private void VolumeMute(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            VolumeMute();
        }
    }
}