// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tbasic.Errors;
using Tbasic.Runtime;
using Tbasic.Types;

namespace Tbasic.Libraries
{
    internal class ProcessLibrary : Library
    {
        public ProcessLibrary()
        {
            Add("ProcStart", Run);
            Add("ProcClose", ProcessClose);
            Add("ProcKill", ProcessKill);
            Add("ProcExists", ProcessExists);
            Add("ProcBlockList", new TbasicFunction(BlockedList));
            //Add("ProcBlock", ProcessBlock);
            //Add("ProcRedirect", ProcessRedirect);
            Add("ProcSetDebugger", ProcessSetDebugger);
            Add("ProcUnblock", Unblock);
            Add("ProcList", ProcessList);
        }

        private object ProcessExists(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(stackdat.Get<string>(1))) {
                    return true;
                }
            }
            return false;
        }

        private object ProcessList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            Process[] procs = Process.GetProcesses();
            if (procs.Length > 0) {
                object[][] _ret = new object[procs.Length][];
                for (int index = 0; index < _ret.Length; index++) {
                    _ret[index] = new object[] { procs[index].Id, procs[index].ProcessName };
                }
                return _ret;
            }
            else {
                stackdat.Status = ErrorSuccess.NoContent;
                return null;
            }
        }

        private object ProcessKill(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(stackdat.Get<string>(1))) {
                    p.Kill();
                    return null;
                }
            }
            stackdat.Status = ErrorClient.NotFound;
            return null;
        }

        private object ProcessClose(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(stackdat.Get<string>(1))) {
                    p.Close();
                    return null;
                }
            }
            stackdat.Status = ErrorClient.NotFound;
            return null;
        }

        private object BlockedList(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            var list = BlockedList(); // dicts currently are not supported 2/24/15
            if (list.Count == 0) {
                stackdat.Status = ErrorSuccess.NoContent;
                return null;
            }
            else {
                string[][] _array = new string[list.Count][];
                int index = 0;
                foreach (var _kv in list) {
                    _array[index++] = new string[] { _kv.Key, _kv.Value }; // convert it to jagged array (like AutoIt) 2/23/15
                }
                return _array;
            }
        }

        private Dictionary<string, string> BlockedList()
        {
            using (RegistryKey imgKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options")) {
                Dictionary<string, string> blocked = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (string keyName in imgKey.GetSubKeyNames()) {
                    using (RegistryKey app = imgKey.OpenSubKey(keyName)) {
                        if (app.GetValueNames().Contains("Debugger")) {
                            blocked.Add(keyName, app.GetValue("Debugger") + "");
                        }
                    }
                }
                return blocked;
            }
        }

        private const string REG_EXEC_PATH = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\";

        private object ProcessBlock(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add(16);
                stackdat.Add("The application you requested has been blocked");
                stackdat.Add("Blocked");
            }
            stackdat.AssertCount(5);
            string name = stackdat.Get<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", "\"" + Application.ExecutablePath + "\" -m \"" + stackdat.Get(2) + "\" \"" + stackdat.Get(3) + "\" \"" + stackdat.Get(4) + "\"");
            }
            return null;
        }

        private object ProcessRedirect(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string name = stackdat.Get<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            if (!File.Exists(stackdat.Get<string>(2))) {
                throw new FileNotFoundException();
            }
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", "\"" + Application.ExecutablePath + "\" -r \"" + stackdat.Get(2) + "\"");
            }
            return null;
        }

        private object ProcessSetDebugger(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(3);
            string name = stackdat.Get<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            if (!File.Exists(stackdat.Get<string>(2))) {
                throw new FileNotFoundException();
            }
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", stackdat.Get<string>(2));
            }
            return null;
        }

        private object Unblock(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            string name = stackdat.Get<string>(1);
            if (!name.Contains(".")) {
                name += ".exe";
            }
            var blockedList = BlockedList();
            if (blockedList.ContainsKey(name)) {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(Path.Combine(REG_EXEC_PATH, name), true)) {
                    key.DeleteValue("Debugger");
                }
            }
            else {
                stackdat.Status = -1; // -1 not found 2-24-15
            }
            return null;
        }

        private object Run(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount == 2) {
                stackdat.Add("");
            }
            if (stackdat.ParameterCount == 3) {
                stackdat.Add(Environment.CurrentDirectory);
            }
            if (stackdat.ParameterCount == 4) {
                stackdat.Add(false);
            }
            stackdat.AssertCount(5);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = stackdat.Get<string>(1);
            startInfo.Arguments = stackdat.Get<string>(2);
            startInfo.WorkingDirectory = stackdat.Get<string>(3);
            stackdat.Status = Run(startInfo, stackdat.Get<bool>(4));
            return null;
        }

        private int Run(ProcessStartInfo info, bool wait)
        {
            using (Process p = new Process()) {
                p.StartInfo = info;
                p.Start();
                string result = null;
                if (p.StartInfo.RedirectStandardOutput) {
                    result = p.StandardOutput.ReadToEnd();
                }
                if (wait) {
                    p.WaitForExit();
                    return p.ExitCode;
                }
                return 0;
            }
        }
    }
}