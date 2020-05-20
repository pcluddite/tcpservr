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
using Microsoft.VisualBasic;
using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    /// <summary>
    /// A library for basic user input and output operations
    /// </summary>
    public class UserIOLibrary : Library
    {
        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public UserIOLibrary()
        {
            Add("TrayTip", TrayTip);
            Add("MsgBox", MsgBox);
            Add("Say", Say);
            Add("Input", Input);
            Add("StdRead", ConsoleRead);
            Add("StdReadLine", ConsoleReadLine);
            Add("StdReadKey", ConsoleReadKey);
            Add("StdWrite", ConsoleWrite);
            Add("StdWriteLine", ConsoleWriteline);
            Add("StdPause", ConsolePause);
        }

        private void ConsoleWriteline(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            Console.WriteLine(_sframe.GetParameter(1));
        }

        private void ConsoleWrite(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            Console.Write(_sframe.GetParameter(1));
        }

        private void ConsoleRead(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            _sframe.Data = Console.Read();
        }

        private void ConsoleReadLine(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            _sframe.Data = Console.ReadLine();
        }

        private void ConsoleReadKey(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            _sframe.Data = Console.ReadKey().KeyChar;
        }

        private void ConsolePause(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            _sframe.Data = Console.ReadKey(true).KeyChar;
        }

        /// <summary>
        /// Prompts the user to input data
        /// </summary>
        /// <param name="prompt">the text for the message prompt</param>
        /// <param name="title">the title of the message box</param>
        /// <param name="defaultResponse">the default response</param>
        /// <param name="x">the x position of the window</param>
        /// <param name="y">the y position of the window</param>
        /// <returns></returns>
        public static string InputBox(string prompt, string title = "", string defaultResponse = "", int x = -1, int y = -1)
        {
            return Interaction.InputBox(prompt, title, defaultResponse, x, y);
        }

        private void Input(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 2) {
                _sframe.SetAll(
                    _sframe.GetParameter(0), _sframe.GetParameter(1),
                    "Input", -1, -1
                    );
            }
            if (_sframe.ParameterCount == 3) {
                _sframe.SetAll(
                    _sframe.GetParameter(0), _sframe.GetParameter(1),
                    _sframe.GetParameter(2), -1, -1);
            }
            if (_sframe.ParameterCount == 4) {
                _sframe.SetAll(
                    _sframe.GetParameter(0), _sframe.GetParameter(1),
                    _sframe.GetParameter(2), _sframe.GetParameter(3), -1);
            }
            _sframe.AssertParamCount(5);

            int x = _sframe.GetParameter<int>(3),
                y = _sframe.GetParameter<int>(4);

            string resp = InputBox(_sframe.GetParameter<string>(1), _sframe.GetParameter<string>(2), "", x, y);

            if (string.IsNullOrEmpty(resp)) { 
                _sframe.Status = ErrorSuccess.NoContent; // -1 no input 2/24
            }
            else {
                _sframe.Data = resp;
            }
        }

        /// <summary>
        /// Creates a notification balloon
        /// </summary>
        /// <param name="text">the text of the balloon</param>
        /// <param name="title">the title of the balloon</param>
        /// <param name="icon">the balloon icon</param>
        /// <param name="timeout">the length of time the balloon should be shown (this may not be honored by the OS)</param>
        public static void TrayTip(string text, string title = "", ToolTipIcon icon = ToolTipIcon.None, int timeout = 5000)
        {
            Thread t = new Thread(MakeTrayTip);
            t.Start(new object[] { timeout, icon, text, title });
        }

        private void TrayTip(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 2) {
                _sframe.AddParameter(""); // title
                _sframe.AddParameter(0); // icon
                _sframe.AddParameter(5000); // timeout
            }
            else if (_sframe.ParameterCount == 3) {
                _sframe.AddParameter(0); // icon
                _sframe.AddParameter(5000); // timeout
            }
            else if (_sframe.ParameterCount == 4) {
                _sframe.AddParameter(5000); // timeout
            }
            _sframe.AssertParamCount(5);
            TrayTip(text: _sframe.GetParameter<string>(1), title: _sframe.GetParameter<string>(2), icon: _sframe.GetParameter<ToolTipIcon>(3), timeout: _sframe.GetParameter<int>(4));
        }

        private static void MakeTrayTip(object param)
        {
            try {
                object[] cmd = (object[])param;
                using (NotifyIcon tray = new NotifyIcon()) {
                    tray.Icon = Properties.Resources.blank;
                    tray.Visible = true;
                    int timeout = (int)cmd[0];
                    ToolTipIcon icon;
                    switch ((int)cmd[1]) {
                        case 1: icon = ToolTipIcon.Info; break;
                        case 2: icon = ToolTipIcon.Warning; break;
                        case 3: icon = ToolTipIcon.Error; break;
                        default: icon = ToolTipIcon.None; break;
                    }
                    tray.ShowBalloonTip(timeout, (string)cmd[3], (string)cmd[2], icon);
                    Thread.Sleep(timeout);
                    tray.Visible = false;
                }
            }
            catch {
            }
        }

        /// <summary>
        /// Creates a message box
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="buttons"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string MsgBox(object prompt, int buttons = (int)MsgBoxStyle.ApplicationModal, object title = null)
        {
            return Interaction.MsgBox(prompt, (MsgBoxStyle)buttons, title).ToString();
        }

        private void MsgBox(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 3) {
                _sframe.AddParameter("");
            }
            _sframe.AssertParamCount(4);

            int flag = _sframe.GetParameter<int>(1);
            string text = _sframe.GetParameter<string>(2),
                   title = _sframe.GetParameter<string>(3);

            _sframe.Data = MsgBox(buttons: flag, prompt: text, title: title);
        }

        /// <summary>
        /// Converts text to synthesized speech
        /// </summary>
        /// <param name="text">the text to speak</param>
        public static void Say(string text)
        {
            Thread t = new Thread(Say);
            t.Start(text);
        }

        private void Say(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            Say(_sframe.GetParameter<string>(1));
        }

        private static void Say(object text)
        {
            try {
                using (SpeechSynthesizer ss = new SpeechSynthesizer()) {
                    ss.Speak(text.ToString());
                }
            }
            catch {
            }
        }
    }
}
