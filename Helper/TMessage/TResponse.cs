using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tcpservr {
    public class TResponse {

        /// <summary>
        /// Initialize a new TResponse
        /// </summary>
        /// <param name="original">The message to which this is a response</param>
        public TResponse(TMessage original) {
            this.Original = original;
        }

        /// <summary>
        /// The length of the response
        /// </summary>
        public int Length {
            get {
                return this.Data.Length;
            }
        }

        /// <summary>
        /// The full text of the response
        /// </summary>
        public string DataString {
            get {
                return Encoding.UTF8.GetString(raw.Skip(7).ToArray());
            }
        }

        /// <summary>
        /// The response header
        /// </summary>
        public string Header {
            get {
                return DataString.Replace("\r\n", "\n").Split('\n')[0];
            }
        }

        /// <summary>
        /// The response body
        /// </summary>
        public string Message {
            get {
                return this.DataString.Remove(0, this.Header.Length).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the raw data of the message
        /// </summary>
        public byte[] Data {
            get {
                return raw;
            }
            set {
                this.Process(value);
            }
        }

        /// <summary>
        /// The status code of the response
        /// </summary>
        public int Status {
            get {
                return int.Parse(this.Header.Split(' ')[0]);
            }
        }

        /// <summary>
        /// Gets or sets the orignal message
        /// </summary>
        public TMessage Original { get; set; }

        /// <summary>
        /// Processes data that has been encrypted
        /// </summary>
        /// <param name="data">The data to be encrypted (excluding the preceding 7 bytes)</param>
        public void ProcessEncrypted(byte[] data) {
            List<byte> newData = new List<byte>();
            newData.AddRange(TReceiver.ENC);
            newData.AddRange(BitConverter.GetBytes(data.Length));
            newData.AddRange(data);
            raw = newData.ToArray();
        }

        /// <summary>
        /// Processes a new response
        /// </summary>
        /// <param name="status">The response status</param>
        /// <param name="message">The response body</param>
        public void Process(int status, string message) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(status + " " + this.Original.DataString);
            sb.Append(message);
            string text = sb.ToString().TrimEnd();
            List<byte> data = new List<byte>();
            data.AddRange(TReceiver.TCP);
            data.AddRange(BitConverter.GetBytes(text.Length));
            data.AddRange(Encoding.UTF8.GetBytes(text));
            raw = data.ToArray();
        }

        private byte[] raw;

        /// <summary>
        /// Processes a new response
        /// </summary>
        /// <param name="data">The response data as a string (starting with the status code followed by a space)</param>
        public void Process(string data) {
            if (data.Trim().CompareTo("") == 0) {
                data = "204 No Content";
            }
            this.Process(int.Parse(data.Remove(3, data.Length - 3)), data.Remove(0, 4));
        }

        /// <summary>
        /// Processes a new response
        /// </summary>
        /// <param name="data">The data including the 7 leading bytes</param>
        public void Process(byte[] data) {
            raw = new byte[data.Length];
            data.CopyTo(raw, 0);
            if (Original == null) {
                TMessage msg = new TMessage();
                string text = Encoding.UTF8.GetString(data.Skip(7).ToArray());
                text = text.Replace("\r\n", "\n").Split('\n')[0].Remove(0, 3).TrimStart();
                msg.Process(text);
            }
        }

        public void Process(byte[] data, bool isEncrypted) {
            List<byte> newData = new List<byte>();
            
        }

        /// <summary>
        /// Clones a TResponse object
        /// </summary>
        /// <returns>A new TResponse object with the same value</returns>
        public TResponse Clone() {
            TMessage newMsg = Original.Clone();
            TResponse newResponse = new TResponse(newMsg);
            newResponse.Process(Status, Message);
            return newResponse;
        }
    }
}
