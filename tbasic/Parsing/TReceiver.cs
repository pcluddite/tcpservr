using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Security;

namespace Tbasic.Components {
    public class TReceiver : IDisposable {

        public static readonly byte[] TCP = Encoding.UTF8.GetBytes("TCP");
        public static readonly byte[] ENC = Encoding.UTF8.GetBytes("ENC");

        private Stream writeStream;
        private Stream readStream;

        public Stream GetWriteStream() {
            return writeStream;
        }

        public Stream GetReadStream() {
            return readStream;
        }

        public TReceiver(Stream stream) {
            writeStream = readStream = stream;
        }

        public TReceiver(Stream writer, Stream reader) {
            writeStream = writer;
            readStream = reader;
        }

        public int Receive(out byte[] data) {
            data = new byte[512];
            int len = readStream.Read(data, 0, data.Length);

            string check = Encoding.UTF8.GetString(data, 0, len);
            if (len < 7 || (!check.StartsWith("TCP") && !check.StartsWith("ENC"))) {
                return -1;
            }

            List<byte> allData = new List<byte>();
            allData.AddRange(data.Take(len));
            int size = BitConverter.ToInt32(allData.Skip(3).Take(4).ToArray(), 0);

            while (allData.Count - 7 < size) {
                data = new byte[255];
                len = readStream.Read(data, 0, data.Length);
                allData.AddRange(data.Take(len));
            }
            data = allData.ToArray();
            return data.Length;
        }

        public TResponse Receive() {
            byte[] data;
            int len = Receive(out data);
            if (len == -1) {
                return null;
            }
            TResponse response = new TResponse();
            response.Process(data);
            return response;
        }

        public void Send(byte[] data) {
            writeStream.Write(data, 0, data.Length);
        }

        public void Dispose() {
            writeStream.Dispose();
            readStream.Dispose();
        }
    }
}
