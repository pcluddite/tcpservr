using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Tcpservr.Messaging
{
    public class MessageReceiver : IDisposable
    {
        /// <summary>
        /// The byte prefix for a plaintext message
        /// </summary>
        public static readonly byte[] PlainPrefix = EncodeString("TB2");
        /// <summary>
        /// The byte prefix for an encrypted message
        /// </summary>
        public static readonly byte[] EncryptedPrefix = EncodeString("EN2");
        /// <summary>
        /// Gets the length of a message prefix
        /// </summary>
        public const int PREFIX_SIZE = 3;
        /// <summary>
        /// Gets the length of a message header size
        /// </summary>
        public const int HEADER_SIZE = PREFIX_SIZE + sizeof(int); // messsages are prepended with the prefix and size of message

        public Stream Writer { get; set; }
        public Stream Reader { get; set; }

        public bool IsSecure { get; set; }

        public bool IsDisposed { get; private set; }
        public bool ShouldDispose { get; set; }

        public MessageReceiver(Stream stream, bool dispose = true)
            : this(stream, stream, dispose)
        {
        }

        public MessageReceiver(Stream writer, Stream reader, bool dispose = true)
        {
            Writer = writer;
            Reader = reader;
            ShouldDispose = dispose;
        }

        ~MessageReceiver()
        {
            if (ShouldDispose) {
                Dispose();
            }
        }

        public byte[] LastReceived { get; private set; }

        public int Receive(out byte[] data)
        {
            data = new byte[1024]; // receive things in 1kb blocks
            int len = Reader.Read(data, 0, data.Length);

            if (len < HEADER_SIZE || !(IsPrefixed(PlainPrefix, data) || IsPrefixed(EncryptedPrefix, data))) {
                return -1; // a valid message was not found
            }

            int expectedSize = BitConverter.ToInt32(data, PREFIX_SIZE); // the size of data we expect to receive

            List<byte> received = new List<byte>(len);
            received.AddRange(data.Take(len)); // add valid data to all received data

            while(received.Count - HEADER_SIZE < expectedSize) {
                len = Reader.Read(data, 0, data.Length); // continuously read data until we get the expected size
                received.AddRange(data.Take(len));
            }

            LastReceived = data = received.Skip(HEADER_SIZE).ToArray();

            return data.Length;
        }

        public ServerOutputMessage ReceiveResponse()
        {
            byte[] data;
            int len = Receive(out data);
            if (len <= 0)
                return null;
            return ProcessResponse(data);
        }

        public static ServerOutputMessage ProcessResponse(byte[] data)
        {
            return JsonConvert.DeserializeObject<ServerOutputMessage>(DecodeString(data, 0, data.Length));
        }

        public ServerInputMessage ReceiveMessage()
        {
            byte[] data;
            int len = Receive(out data);
            if (len <= 0)
                return null;
            return ProcessMessage(data);
        }

        public static ServerInputMessage ProcessMessage(byte[] data)
        {
            return JsonConvert.DeserializeObject<ServerInputMessage>(DecodeString(data, 0, data.Length));
        }

        private static bool IsPrefixed(byte[] prefix, byte[] msg)
        {
            if (msg.Length < prefix.Length)
                return false; // can't possibly be prefixed if the prefix is longer

            for(int i = 0; i < prefix.Length; ++i) {
                if (prefix[i] != msg[i]) {
                    return false;
                }
            }
            return true;
        }

        public void SendMessage(ServerInputMessage msg)
        {
            Send(AddHeader(EncodeString(JsonConvert.SerializeObject(msg))));
        }

        public void SendResponse(ServerOutputMessage msg)
        {
            Send(AddHeader(EncodeString(JsonConvert.SerializeObject(msg))));
        }

        private void Send(byte[] data)
        {
            Writer.Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            if (!IsDisposed) {
                Writer?.Dispose(); // only need to dispose one
                if (Reader != Writer) {
                    Reader?.Dispose();
                }
                IsDisposed = true;
            }
        }

        public static byte[] EncodeString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string DecodeString(byte[] msg, int index, int count)
        {
            return Encoding.UTF8.GetString(msg, index, count);
        }

        internal static byte[] AddHeader(byte[] data, bool encrypt = false)
        {
            byte[] bytes = new byte[data.Length + sizeof(int) + PREFIX_SIZE];
            if (encrypt) {
                EncryptedPrefix.CopyTo(bytes, 0);
                // TODO: fancy encryption stuff 5/22/16
                throw new NotImplementedException();
            }
            else {
                PlainPrefix.CopyTo(bytes, 0);
                BitConverter.GetBytes(data.Length).CopyTo(bytes, PREFIX_SIZE);
                data.CopyTo(bytes, PREFIX_SIZE + sizeof(int));
                return bytes;
            }
        }
    }
}