// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tcpservr.Terminal
{
    public class Settings
    {
        public static Dictionary<string, string> Variables = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        public static string ApplicationPath
        {
            get {
                return Application.StartupPath;
            }
        }

        public static string SettingsPath
        {
            get {
                return Path.Combine(ApplicationPath, "console.config");
            }
        }

        public static int Timeout
        {
            get {
                int timeout;
                if (int.TryParse(GetValue("timeout"), out timeout)) {
                    return timeout;
                }
                return 5000;
            }
        }

        public static void RemoveValue(string name)
        {
            Variables.Remove(name);
        }

        public static string GetValue(string name, string defaultValue = "")
        {
            string value;
            if (!Variables.TryGetValue(name, out value)) {
                return defaultValue;
            }
            return value;
        }

        public static int GetIntValue(string name, int defaultValue = default(int))
        {
            int value;
            if (!int.TryParse(GetValue(name), out value)) {
                return defaultValue;
            }
            return value;
        }

        public static bool GetBoolValue(string name, bool defaultValue = default(bool))
        {
            bool value;
            if (!bool.TryParse(GetValue(name), out value)) {
                return defaultValue;
            }
            return value;
        }

        public static void SetValue(string name, string value)
        {
            Variables[name] = value;
        }

        public static string Host
        {
            get {
                return GetValue("host", "localhost");
            }
            set {
                SetValue("host", value);
            }
        }

        public static int Port
        {
            get {
                return GetIntValue("port", 2200);
            }
            set {
                SetValue("port", value.ToString());
            }
        }

        public static string Color
        {
            get {
                return GetValue("color", "08");
            }
            set {
                SetValue("color", value);
            }
        }

        public static string Prompt
        {
            get {
                return GetValue("prompt", "$P$G");
            }
            set {
                SetValue("prompt", value);
            }
        }

        public static bool GetHost(out IPEndPoint result)
        {
            IPAddress host;
            if (!IPAddress.TryParse(Host, out host)) {
                Console.Write("Finding host {0}...", Host, Port);
                try {
                    var addresses =
                        from address in Dns.GetHostAddresses(Host)
                        where address.AddressFamily != AddressFamily.InterNetworkV6
                        select address;
                    host = addresses.ElementAt(0);
                    Console.WriteLine("OK!");
                }
                catch {
                    Console.WriteLine("FAILED!");
                    Console.WriteLine("Hostname could not be resolved.");
                    result = null;
                    return false;
                }
            }
            result = new IPEndPoint(host, Port);
            return true;
        }

        public static void Save()
        {
            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(SettingsPath))) {
                writer.WriteStartObject();
                JsonSerializer serializer = new JsonSerializer();
                writer.WritePropertyName("variables");
                serializer.Serialize(writer, Variables);
                writer.WriteEndObject();
            }
        }

        public static void Load()
        {
            using (JsonReader reader = new JsonTextReader(new StreamReader(SettingsPath))) {
                reader.Read();
                Variables = (Dictionary<string, string>)reader.Value;
            }
        }

        public static string ReplaceVariables(string line)
        {
            string ret = line;

            foreach (var kv in Variables) {
                ret = Regex.Replace(ret, "%" + kv.Key + "%", kv.Value, RegexOptions.IgnoreCase);
            }

            return ret;
        }

        public static bool SetColor(string hex)
        {
            if (hex.Length != 2 || hex[1] == hex[0])
                return false;

            int bgcolor = Convert.ToInt32(hex[0].ToString(), 16);
            int fgcolor = Convert.ToInt32(hex[1].ToString(), 16);

            if ((bgcolor < 0 || bgcolor > 15) || (fgcolor < 0 || fgcolor > 15))
                return false;

            Console.BackgroundColor = (ConsoleColor)bgcolor;
            Console.ForegroundColor = (ConsoleColor)fgcolor;
            return true;
        }

        public static string GetPrompt(string remoteDir)
        {
            StringBuilder sb = new StringBuilder(Prompt.Length); // definitely not a guarantee, but it's a start
            for (int index = 0; index < Prompt.Length; ++index) {
                char c = Prompt[index];
                if (c == '$') {
                    if (index + 1 < Prompt.Length) {
                        c = Prompt[++index];
                        switch (char.ToUpper(c)) {
                            case 'A': sb.Append('&'); break;
                            case 'B': sb.Append('|'); break;
                            case 'C': sb.Append('('); break;
                            case 'D': sb.Append(DateTime.Today.Date.ToShortDateString()); break;
                            case 'E': sb.Append((char)27); break;
                            case 'F': sb.Append(')'); break;
                            case 'G': sb.Append('>'); break;
                            case 'H': sb.Append('\b'); break;
                            case 'L': sb.Append('<'); break;
                            case 'N': {
                                    if (remoteDir.Length > 0)
                                        sb.Append(Path.GetPathRoot(remoteDir));
                                }
                                break;
                            case 'P': sb.Append(remoteDir); break;
                            case 'Q': sb.Append('='); break;
                            case 'S': sb.Append(' '); break;
                            case 'T': DateTime.Now.TimeOfDay.ToString("hh:mm:ss"); break;
                            case 'V': sb.Append(Program.VER); break;
                            case '_': sb.AppendLine(); break;
                            case '$': sb.Append('$'); break;
                        }
                    }
                }
                else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}