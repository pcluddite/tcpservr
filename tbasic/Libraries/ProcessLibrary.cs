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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tbasic.Runtime;
using Tbasic.Errors;
using System.IO;

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
            Add("ProcBlockList", BlockedList);
            //Add("ProcBlock", ProcessBlock);
            //Add("ProcRedirect", ProcessRedirect);
            Add("ProcSetDebugger", ProcessSetDebugger);
            Add("ProcUnblock", Unblock);
            Add("ProcList", ProcessList);
        }

        private void ProcessExists(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            _sframe.Data = false;
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(_sframe.GetParameter<string>(1))) {
                    _sframe.Data = true;
                    break;
                }
            }
        }

        private void ProcessList(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            Process[] procs = Process.GetProcesses();
            if (procs.Length > 0) {
                object[][] _ret = new object[procs.Length][];
                for (int index = 0; index < _ret.Length; index++) {
                    _ret[index] = new object[] { procs[index].Id, procs[index].ProcessName };
                }
                _sframe.Data = _ret;
            }
            else {
                _sframe.Status = ErrorSuccess.NoContent;
            }
        }

        private void ProcessKill(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(_sframe.GetParameter<string>(1))) {
                    p.Kill();
                    return;
                }
            }
            _sframe.Status = ErrorClient.NotFound;
        }

        private void ProcessClose(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            foreach (Process p in Process.GetProcesses()) {
                if (p.ProcessName.EqualsIgnoreCase(_sframe.GetParameter<string>(1))) {
                    p.Close();
                    return;
                }
            }
            _sframe.Status = ErrorClient.NotFound;
        }

        private void BlockedList(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(1);
            var list = BlockedList(); // dicts currently are not supported 2/24/15
            if (list.Count == 0) {
                _sframe.Status = ErrorSuccess.NoContent;
            }
            else {
                string[][] _array = new string[list.Count][];
                int index = 0;
                foreach (var _kv in list) {
                    _array[index++] = new string[] { _kv.Key, _kv.Value }; // convert it to jagged array (like AutoIt) 2/23/15
                }
                _sframe.Data = _array;
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

        private void ProcessBlock(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 2) {
                _sframe.AddParameter(16);
                _sframe.AddParameter("The application you requested has been blocked");
                _sframe.AddParameter("Blocked");
            }
            _sframe.AssertParamCount(5);
            string name = _sframe.GetParameter<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", "\"" + Application.ExecutablePath + "\" -m \"" + _sframe.GetParameter(2) + "\" \"" + _sframe.GetParameter(3) + "\" \"" + _sframe.GetParameter(4) + "\"");
            }
        }

        private void ProcessRedirect(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            string name = _sframe.GetParameter<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            if (!File.Exists(_sframe.GetParameter<string>(2))) {
                throw new FileNotFoundException();
            }
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", "\"" + Application.ExecutablePath + "\" -r \"" + _sframe.GetParameter(2) + "\"");
            }
        }

        private void ProcessSetDebugger(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(3);
            string name = _sframe.GetParameter<string>(1);
            if (!Path.HasExtension(name)) {
                name += ".exe";
            }
            name = Path.GetFileName(name);
            if (!File.Exists(_sframe.GetParameter<string>(2))) {
                throw new FileNotFoundException();
            }
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Path.Combine(REG_EXEC_PATH, name))) {
                key.SetValue("Debugger", _sframe.GetParameter<string>(2));
            }
        }

        private void Unblock(TFunctionData _sframe)
        {
            _sframe.AssertParamCount(2);
            string name = _sframe.GetParameter<string>(1);
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
                _sframe.Status = -1; // -1 not found 2-24-15
            }
        }

        private void Run(TFunctionData _sframe)
        {
            if (_sframe.ParameterCount == 2) {
                _sframe.AddParameter("");
            }
            if (_sframe.ParameterCount == 3) {
                _sframe.AddParameter(Environment.CurrentDirectory);
            }
            if (_sframe.ParameterCount == 4) {
                _sframe.AddParameter(false);
            }
            _sframe.AssertParamCount(5);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = _sframe.GetParameter<string>(1);
            startInfo.Arguments = _sframe.GetParameter<string>(2);
            startInfo.WorkingDirectory = _sframe.GetParameter<string>(3);
            _sframe.Status = Run(startInfo, _sframe.GetParameter<bool>(4));
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