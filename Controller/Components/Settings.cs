using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Tcpclient {
    public class Settings {
        private static XmlDocument settings = LoadDoc();

        private XmlNode data;
        public Settings(string name) {
            data = GetSettingsNode(name);
        }

        public void SetAttribute(string key, string value) {
            data.Attributes[key].Value = value;
        }

        public void Set(string key, string value) {
            XmlNode config = null;
            foreach (XmlNode n in data.SelectNodes("config")) {
                if (n.Attributes["key"].Value.Equals(key)) {
                    config = n;
                    break;
                }
            }
            if (config == null) {
                XmlElement e = CreateChild(data, "config");
                e.SetAttribute("key", key);
                e.SetAttribute("value", value);
            }
            else {
                config.Attributes["value"].Value = value;
            }
        }

        public void DeleteKey(string key) {
            foreach (XmlNode n in data.SelectNodes("config")) {
                if (n.Attributes["key"].Value.Equals(key)) {
                    data.RemoveChild(n);
                    break;
                }
            }
        }

        public string Get(string key) {
            foreach (XmlNode config in data.SelectNodes("config")) {
                if (config.Attributes["key"].Value.Equals(key)) {
                    return config.Attributes["value"].Value;
                }
            }
            return null;
        }

        public void Destroy() {
            settings.SelectSingleNode("tcpservr/endpoints").RemoveChild(data);
        }

        private static XmlElement CreateChild(XmlNode parent, string childName) {
            XmlElement child = settings.CreateElement(childName);
            parent.AppendChild(child);
            return child;
        }

        public static string GetDefault() {
            try {
                return settings.SelectSingleNode("tcpservr/current").InnerText;
            }
            catch {
                return null;
            }
        }

        public static void SetDefault(string name) {
            settings.SelectSingleNode("tcpservr/current").InnerText = name;
        }

        public static XmlNode GetSettingsNode(string name) {
            foreach (XmlNode n in settings.SelectNodes("tcpservr/endpoints/address")) {
                if (n.Attributes["name"].Value.Equals(name)) {
                    return n;
                }
            }
            return null;
        }

        public static List<Dictionary<string, string>> GetAllComputers() {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (XmlNode n in settings.SelectNodes("tcpservr/endpoints/address")) {
                Dictionary<string, string> attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                attributes.Add("name",      n.Attributes["name"].Value);
                attributes.Add("address",   n.Attributes["address"].Value);
                attributes.Add("port",      n.Attributes["port"].Value);
                result.Add(attributes);
            }
            return result;
        }

        public static void AddComputer(string name, string address, string port) {
            XmlElement endpoints = settings.CreateElement("endpoints");
            settings.DocumentElement.AppendChild(endpoints);
            XmlElement child = settings.CreateElement("address");
            child.SetAttribute("name", name);
            child.SetAttribute("address", address);
            child.SetAttribute("port", port);
            settings.SelectSingleNode("tcpservr/endpoints").AppendChild(child);
        }

        private static XmlDocument LoadDoc() {
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);
            XmlDocument doc = new XmlDocument();
            if (File.Exists(directory + "\\config.xml")) {
                try {
                    File.Move(directory + "\\config.xml", directory + "\\Data.xml");
                }
                catch {
                }
            }
            if (File.Exists(directory + "\\Data.xml")) {
                doc.Load(directory + "\\Data.xml");
            }
            else {
                doc.LoadXml("<tcpservr></tcpservr>");
            }
            if (doc.SelectSingleNode("tcpservr/current") == null) {
                XmlElement child = doc.CreateElement("current");
                child.InnerText = "LocalHost";
                doc.DocumentElement.AppendChild(child);
            }
            if (doc.SelectSingleNode("tcpservr/endpoints") == null) {
                XmlElement endpoints = doc.CreateElement("endpoints");
                doc.DocumentElement.AppendChild(endpoints);
                XmlElement child = doc.CreateElement("address");
                child.SetAttribute("name", "This Computer");
                child.SetAttribute("address", "LocalHost");
                child.SetAttribute("port", "2200");
                doc.SelectSingleNode("tcpservr/endpoints").AppendChild(child);
            }
            return doc;
        }

        public static void Save() {
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6);
            settings.Save(directory + "\\Data.xml");
        }
    }
}
