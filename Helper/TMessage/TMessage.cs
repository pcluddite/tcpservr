using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tcpservr {
    public class TMessage {

        private byte[] raw;
        private string[] cmd;

        public bool IsEncrypted {
            get {
                return Encoding.UTF8.GetString(raw, 0, 3).Equals("ENC");
            }
        }

        /// <summary>
        /// The raw byte data of the message including the leading 7 bytes
        /// </summary>
        public byte[] RawData {
            get {
                return raw;
            }
        }

        /// <summary>
        /// The byte data of the message excluding the leading 7 bytes
        /// </summary>
        public byte[] Data {
            get {
                return this.RawData.Skip(7).ToArray();
            }
        }

        private string original;

        /// <summary>
        /// The data as a string without symbols
        /// </summary>
        public string DataString {
            get {
                return RemoveSymbols(original);
            }
        }

        /// <summary>
        /// The data as a string including the symbols
        /// </summary>
        public string UnformattedDataString {
            get {
                return original;
            }
        }

        /// <summary>
        /// The arguments of the message excluding quotes
        /// </summary>
        public string[] Args {
            get {
                return cmd;
            }
        }

        /// <summary>
        /// The arguments of the message including quotes
        /// </summary>
        public string[] ArgsOriginal {
            get {
                return ParseCmd(original, true);
            }
        }

        /// <summary>
        /// The length of the message excluding the preceding 7 bytes
        /// </summary>
        public int Length {
            get {
                return this.Data.Length;
            }
        }

        /// <summary>
        /// Temporary storage for 3rd party use
        /// </summary>
        public object Storage { get; set; }

        private DateTime startTime;

        /// <summary>
        /// Gets the time the TMessage object was created
        /// </summary>
        public DateTime CreationTime {
            get {
                return startTime;
            }
        }

        /// <summary>
        /// Gets or sets an ID for this message
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Initialize a new TMessage object
        /// </summary>
        public TMessage() {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// Initialize a new TMessage object
        /// </summary>
        /// <param name="id">An identification number to assign the message</param>
        public TMessage(int id) {
            this.ID = id;
            startTime = DateTime.Now;
        }

        /// <summary>
        /// Process raw byte data
        /// </summary>
        /// <param name="data">The byte array to process (including the preceding 7 bytes)</param>
        public void Process(byte[] data) {
            Process(data, false);
        }

        /// <summary>
        /// Process raw byte data
        /// </summary>
        /// <param name="data">The byte array to process</param>
        /// <param name="addPrecedingBytes">Sepcify whether or not the preceding bytes should be prepended</param>
        public void Process(byte[] data, bool addPrecedingBytes) {
            if (addPrecedingBytes) {
                List<byte> newData = new List<byte>();
                newData.AddRange(TReceiver.TCP);
                newData.AddRange(BitConverter.GetBytes(data.Length));
                newData.AddRange(data);
                data = newData.ToArray();
            }
            raw = new byte[data.Length];
            data.CopyTo(raw, 0);
            original = Encoding.UTF8.GetString(Data, 0, Data.Length);
            cmd = ParseCmd(original, false);
        }

        /// <summary>
        /// Process an array of arguments
        /// </summary>
        /// <param name="args">The argument array</param>
        public void Process(params object[] args) {
            for (int i = 0; i < args.Length; i++) {
                args[i] = AddSymbols(args[i].ToString());
            }
            Process(buildString(args));
        }

        private string buildString(object[] args) {
            int i = 1;
            StringBuilder sb = new StringBuilder(args[0].ToString());
            while (i < args.Length) {
                if (args[i].ToString().Contains(' ') || args[i].ToString().Equals("")) {
                    sb.Append(" \"" + args[i] + "\"");
                }
                else {
                    sb.Append(" " + args[i]);
                }
                i++;
            }
            return sb.ToString();
        }

        private string AddSymbols(string s) {
            return s.Replace("@", "@at").Replace("\"", "@quote").Replace("|", "@pipe").Replace("\r\n", "@break");
        }

        /// <summary>
        /// Process a string message
        /// </summary>
        /// <param name="message">The message string without any byte header</param>
        public void Process(string message) {
            List<byte> data = new List<byte>();
            data.AddRange(TReceiver.TCP);
            data.AddRange(BitConverter.GetBytes(message.Length));
            data.AddRange(Encoding.UTF8.GetBytes(message));
            this.Process(data.ToArray());
        }

        private string[] ParseCmd(string msg, bool keepQuotes) {
            string[] cmd = ParseArguments(msg, keepQuotes);
            for (int i = 0; i < cmd.Length; i++) {
                cmd[i] = RemoveSymbols(cmd[i]);
            }
            return cmd;
        }

        private string RemoveSymbols(string s) {
            return s.Replace("@break", "\r\n").Replace("@pipe", "|").Replace("@quote", "\"").Replace("@at", "@");
        }

        private string[] ParseArguments(string commandLine) {
            return ParseArguments(commandLine, false);
        }

        private string[] ParseArguments(string commandLine, bool keepQuotes) {
            char[] parmChars = commandLine.ToCharArray();
            bool inQuote = false;
            for (int index = 0; index < parmChars.Length; index++) {
                if (parmChars[index] == '"')
                    inQuote = !inQuote;
                if (!inQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            if (keepQuotes) {
                return (new string(parmChars)).Split('\n');
            }
            else {
                return (new string(parmChars)).Replace("\"", "").Split('\n');
            }
        }

        /// <summary>
        /// Appends new arguments to the message
        /// </summary>
        /// <param name="args">The new arguments to append</param>
        public void AppendArguments(params string[] args) {
            string[] newArgs = new string[args.Length + cmd.Length];
            cmd.CopyTo(newArgs, 0);
            args.CopyTo(newArgs, cmd.Length);
            Process(newArgs);
        }

        /// <summary>
        /// Assigns new data to an argument
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="data">The new string data to assign</param>
        public void SetArgument(int index, string data) {
            cmd[index] = data;
            Process(cmd);
        }

        /// <summary>
        /// Copies the TMessage
        /// </summary>
        /// <returns>A new TMessage object with the same data</returns>
        public TMessage Clone() {
            TMessage result = new TMessage(ID);
            result.Storage = Storage;
            result.startTime = startTime;
            result.Process(raw);
            return result;
        }
    }
}
