// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Threading;
using Tbasic.Errors;
using Tbasic.Libraries;
using Tbasic.Runtime;

using TCPSERVR.Core;
using TCPSERVR.Errors;

namespace TCPSERVR.Libraries
{
    public class UserIOLibServer : Library
    {
        private ServerMode core;

        public UserIOLibServer(ServerMode tcpservr)
        {
            core = tcpservr;
            Add("ECHO", Echo);
            tcpservr.MainExecuter.Global.SetFunction("MsgBox", MsgBox); // override the original MsgBox() function
        }

        public static object Echo(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return stackdat.Get<string>(1);
        }

        public static object MsgBox(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 3) {
                stackdat.Add("");
            }
            stackdat.AssertCount(4);

            int flag = stackdat.Get<int>(1);
            string text = stackdat.Get<string>(2),
                   title = stackdat.Get<string>(3);

            // send the message box off into its own thread, so we're not waiting for a response
            Thread msgthread = new Thread(MsgBoxRunner);
            msgthread.Start(new object[] { flag, text, title });

            stackdat.Status = ErrorSuccess.Accepted;
            return null;
        }

        private static void MsgBoxRunner(object args)
        {
            try {
                object[] arr_args = (object[])args;
                UserIOLibrary.MsgBox(buttons: (int)arr_args[0], prompt: (string)arr_args[1], title: (string)arr_args[2]);
            }
            catch (Exception ex) { // so we don't disturb the entire application on something so trivial as this 6/9/16
                ServerCore.ErrorLog.Add(new LoggedError("MSGBOX_THREAD", ex, ServerCore.IsDominant, fatal: false));
            }
        }
    }
}
