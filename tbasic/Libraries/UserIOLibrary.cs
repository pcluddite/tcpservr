// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Microsoft.VisualBasic;
using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Runtime;
using Tbasic.Types;

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

        private object ConsoleWriteline(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Console.WriteLine(stackdat.Get(1));
            return null;
        }

        private object ConsoleWrite(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Console.Write(stackdat.Get(1));
            return null;
        }

        private object ConsoleRead(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return Console.Read();
        }

        private object ConsoleReadLine(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return Console.ReadLine();
        }

        private object ConsoleReadKey(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return Console.ReadKey().KeyChar;
        }

        private object ConsolePause(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return Console.ReadKey(true).KeyChar;
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

        private object Input(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.AddRange("Tbasic", -1, -1);
            }
            if (stackdat.ParameterCount == 3) {
                stackdat.AddRange(-1, -1);
            }
            if (stackdat.ParameterCount == 4) {
                stackdat.AddRange(-1);
            }
            stackdat.AssertCount(5);

            int x = stackdat.Get<int>(3),
                y = stackdat.Get<int>(4);

            string resp = InputBox(stackdat.Get<string>(1), stackdat.Get<string>(2), "", x, y);

            if (string.IsNullOrEmpty(resp)) { 
                stackdat.Status = ErrorSuccess.NoContent; // -1 no input 2/24
                return null;
            }
            else {
                return resp;
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

        private object TrayTip(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(""); // title
                stackdat.Add(0); // icon
                stackdat.Add(5000); // timeout
            }
            else if (stackdat.ParameterCount == 3) {
                stackdat.Add(0); // icon
                stackdat.Add(5000); // timeout
            }
            else if (stackdat.ParameterCount == 4) {
                stackdat.Add(5000); // timeout
            }
            stackdat.AssertCount(5);
            TrayTip(text: stackdat.Get<string>(1), title: stackdat.Get<string>(2), icon: stackdat.Get<ToolTipIcon>(3), timeout: stackdat.Get<int>(4));
            return null;
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

        private object MsgBox(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 3) {
                stackdat.Add("");
            }
            stackdat.AssertCount(4);

            int flag = stackdat.Get<int>(1);
            string text = stackdat.Get<string>(2),
                   title = stackdat.Get<string>(3);

            return MsgBox(buttons: flag, prompt: text, title: title);
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

        private object Say(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            Say(stackdat.Get<string>(1));
            return null;
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
