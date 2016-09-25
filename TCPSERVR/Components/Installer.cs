using System;
using System.Diagnostics;
using System.Windows.Forms;
using Tcpservr.Components;
using Microsoft.Win32;
using System.IO;
using System.Text;
using Tcpservr.Core;
using System.Net;

namespace Tcpservr
{
    public class Installer
    {

        public static bool TryInstall(string[] args)
        {
            if (args.Length < 2) {
                return false;
            }
            string arg0 = args[0].ToLower();
            if (!(arg0.Equals("-install") || arg0.Equals("-qinstall"))) {
                return false;
            }
            Installer installer = new Installer(arg0.Equals("-qinstall"));
            string arg1 = args[1].ToLower();
            switch (arg1) {
                case "service":
                    installer.AddService();
                    break;
                case "firewall":
                    installer.AddFirewallException();
                    break;
                case "hklm":
                    installer.AddHKLM();
                    break;
                case "hkcu":
                    installer.AddHKCU();
                    break;
                default:
                    if (args.Length != 3) {
                        return false;
                    }
                    switch (arg1) {
                        case "address":
                            installer.SetAddress(args[2]);
                            break;
                        case "port":
                            installer.SetPort(args[2]);
                            break;
                        case "pass":
                            installer.SetPass(args[2]);
                            break;
                        case "history":
                            installer.SetHistoryLog(args[2]);
                            break;
                        default:
                            return false;
                    }
                    break;
            }
            return true;
        }

        public static bool TryRemove(string[] args)
        {
            if (args.Length < 2) {
                return false;
            }
            string arg0 = args[0].ToLower();
            if (!(arg0.Equals("-remove") || arg0.Equals("-qremove"))) {
                return false;
            }
            Installer installer = new Installer(arg0.Equals("-qremove"));
            string arg1 = args[1].ToLower();
            switch (arg1) {
                case "service":
                    installer.RemoveService();
                    break;
                case "firewall":
                    installer.RemoveFirewallException();
                    break;
                case "hklm":
                    installer.RemoveHKLM();
                    break;
                case "hkcu":
                    installer.RemoveHKCU();
                    break;
                default:
                    return false;
            }
            return true;
        }
        
        public bool IsQuiet { get; set; }

        public Installer(bool isQuiet)
        {
            IsQuiet = isQuiet;
        }

        private void ShowInfo(string info)
        {
            if (!IsQuiet) {
                MessageBox.Show(info, "TCPSERVR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowError(string info)
        {
            if (!IsQuiet) {
                MessageBox.Show(info, "TCPSERVR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetAddress(string address)
        {
            IPAddress ipaddr;
            if (IPAddress.TryParse(address, out ipaddr)) {
                ServerCore.Settings.EndpointIP.Address = ipaddr;
                ShowInfo("The new address has been set.");
            }
            else { 
                ShowError("The address was not valid.");
            }
        }

        public void SetPort(string port)
        {
            int iport;
            if (int.TryParse(port, out iport)) {
                try {
                    ServerCore.Settings.EndpointIP.Port = iport;
                    ShowInfo("The new port has been set.");
                    return;
                }
                catch(ArgumentOutOfRangeException) {
                }
            }
            ShowError("The port was not valid.");
        }

        public void SetPass(string pass)
        {
            /*using (TcpSecure security = new TcpSecure("Ave Maria Gratia")) {
                if (pass.Equals("")) {
                    file.IniWriteValue("Config", "PrivateKey", "");
                }
                else {
                    pass = Convert.ToBase64String(security.Encrypt(Encoding.UTF8.GetBytes(pass)));
                    file.IniWriteValue("Config", "PrivateKey", pass);
                }
            }*/
            ShowInfo("The new private key has been set.");
        }

        public void SetHistoryLog(string log)
        {
            bool do_log;
            if (bool.TryParse(log, out do_log)) {
                ServerCore.Settings.LogHistory = do_log;
                ShowInfo(string.Format("Command history will {0} be logged", do_log ? "now" : "no longer"));
            }
            else {
                ShowError("History logging should be set to 'true' or 'false'.");
            }
        }

        public void AddHKLM()
        {
            try {
                RegistryKey hklm = Registry.LocalMachine;
                hklm = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                hklm.SetValue("TCPSERVR", "\"" + Application.ExecutablePath + "\" -start");
                hklm.Close();    
                ShowInfo("TCPSERVR has been added to the Run list in HKEY_LOCAL_MACHINE");
            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
        }

        public void AddHKCU()
        {
            try {
                RegistryKey hkcu = Registry.CurrentUser;
                hkcu = hkcu.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                hkcu.SetValue("TCPSERVR", "\"" + Application.ExecutablePath + "\" -start");
                hkcu.Close();
                ShowInfo("TCPSERVR has been added to the Run list in HKEY_CURRENT_USER");
            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
        }

        public void AddService()
        {
            Process p = new Process();
            if (IsQuiet) {
                if (Application.ExecutablePath.Contains(" ")) {
                    return;
                }
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "create TCPSERVR binPath= \"cmd /c start " + Application.ExecutablePath + " --start\" type= own type= interact start= auto";
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
                p.StartInfo.Arguments = "start TCPSERVR";
                p.Start();
            }
            else {
                if (Application.ExecutablePath.Contains(" ")) {
                    ShowError("TCPSERVR cannot be in a path that includes spaces if you want to install it as a service.");
                    return;
                }
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "create TCPSERVR binPath= \"cmd /c start " + Application.ExecutablePath + " --start\" type= own type= interact start= auto";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
                if (MessageBox.Show("Installation returned:\n" + p.StandardOutput.ReadToEnd() + "\nWould you like to attempt to start the service?", "TCPSERVR",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    p.StartInfo.FileName = "sc";
                    p.StartInfo.Arguments = "start TCPSERVR";
                    p.Start();
                    ShowInfo("Attempted to start the service. Try to connect to the application to see if it succeeded.");
                }
            }
        }

        public void AddFirewallException()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh";
            p.StartInfo.Arguments = "advfirewall firewall add rule name=\"TCPSERVR\" program=\"" + Application.ExecutablePath + "\" dir=\"in\" action=\"allow\" enable=\"yes\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            ShowInfo("Installation returned:\n" + p.StandardOutput.ReadToEnd());
        }

        public void RemoveHKLM()
        {
            try {
                RegistryKey hklm = Registry.LocalMachine;
                hklm = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                hklm.DeleteValue("TCPSERVR", false);
                ShowInfo("TCPSERVR has been removed from the Run list in HKEY_LOCAL_MACHINE");
            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
        }

        public void RemoveHKCU()
        {
            try {
                RegistryKey hkcu = Registry.CurrentUser;
                hkcu = hkcu.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                hkcu.DeleteValue("TCPSERVR", false);
                ShowInfo("TCPSERVR has been removed from the Run list in HKEY_CURRENT_USER");
            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
        }

        public void RemoveService()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "sc";
            p.StartInfo.Arguments = "delete TCPSERVR";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            ShowInfo("Removal returned:\n" + p.StandardOutput.ReadToEnd());
        }

        public void RemoveFirewallException()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh";
            p.StartInfo.Arguments = "advfirewall firewall delete rule name=\"TCPSERVR\" dir=\"in\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            ShowInfo("Removal returned:\n" + p.StandardOutput.ReadToEnd());
        }
    }
}