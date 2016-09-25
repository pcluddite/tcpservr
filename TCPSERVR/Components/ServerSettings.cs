// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Tcpservr.Core;

namespace Tcpservr
{
    public class ServerSettings
    {
        public static readonly string SettingsPath = Path.Combine(ServerCore.ApplicationDirectory, "Settings.ini");

        /// <summary>
        /// Gets or sets whether command history should be kept 
        /// </summary>
        [JsonProperty(PropertyName = "history")]
        public bool LogHistory { get; set; } = false;

        /// <summary>
        /// Gets or sets the endpoint that this server binds to
        /// </summary>
        [JsonProperty(PropertyName = "endpoint")]
        public IPEndPoint EndpointIP { get; set; } = new IPEndPoint(IPAddress.Any, 2200);

        /// <summary>
        /// File that can be downloaded if the server is accessed by HTTP
        /// </summary>
        [JsonProperty(PropertyName = "download")]
        public string DownloadFile { get; set; } = null;

        public ServerSettings()
        {
        }

        public string GetPrivateKey()
        {
            /*try {
                if (configExists) {
                    try {
                        using (TcpSecure security = new TcpSecure("Ave Maria Gratia")) {
                            string key = file.IniReadValue("Config", "PrivateKey");
                            if (key.Trim().Equals("")) {
                                return null;
                            }
                            key = Encoding.UTF8.GetString(security.Decrypt(Convert.FromBase64String(key)));
                            return key;
                        }
                    }
                    catch {
                        LoggedError error = new LoggedError("Initializer",
                            "The private key was unable to be read. Defaulted to unencrypted mode.", "GetPrivateKey", true, false);
                        error.Write();
                    }
                }
            }
            catch {
            }*/
            return null;
        }
    }
}
