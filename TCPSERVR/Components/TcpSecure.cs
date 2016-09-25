using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Tbasic.Errors;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Tcpservr.Components
{
    public class TcpSecure : IDisposable
    {

        private AesManaged rij;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        public TcpSecure(string key)
        {
            rij = new AesManaged();
            key = key.PadLeft(16, '0');
            encryptor = rij.CreateEncryptor(
                Encoding.UTF8.GetBytes(key),
                Encoding.UTF8.GetBytes("fq7XeXyqkbnmG4Ax")
                );
            decryptor = rij.CreateDecryptor(
                Encoding.UTF8.GetBytes(key),
                Encoding.UTF8.GetBytes("fq7XeXyqkbnmG4Ax")
                );
            IsDisposed = false;
        }

        ~TcpSecure()
        {
            Dispose();
        }

        public const int KEY_SIZE = 64;
        public const int SALT_SIZE = 32;

        private const int ITERATIONS = 4096;

        public static byte[] GetHash(string plainText)
        {
            byte[] key = new byte[KEY_SIZE];
            byte[] salt = new byte[SALT_SIZE];
            byte[] hash = new byte[salt.Length + key.Length];

            try {
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) {
                    using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(plainText, salt, ITERATIONS)) {
                        rng.GetBytes(salt);
                        key = rfc.GetBytes(key.Length);
                        salt.CopyTo(hash, 0);
                        key.CopyTo(hash, salt.Length);
                    }
                }
                return hash;
            }
            finally {
                Array.Clear(key, 0, key.Length);
                Array.Clear(salt, 0, salt.Length);
            }
        }

        public static bool HashEquals(byte[] hashAndSalt, string plainText)
        {
            if (hashAndSalt.Length < KEY_SIZE + SALT_SIZE || string.IsNullOrEmpty(plainText))
                return false;

            byte[] key = new byte[KEY_SIZE];
            byte[] salt = new byte[SALT_SIZE];
            byte[] otherKey = null;

            try {
                Array.Copy(hashAndSalt, 0, salt, 0, salt.Length);
                Array.Copy(hashAndSalt, salt.Length, key, 0, key.Length);

                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(plainText, salt, ITERATIONS)) {
                    otherKey = rfc.GetBytes(KEY_SIZE);
                    return CompareInConstantTime(key, otherKey);
                }
            }
            finally {
                Array.Clear(key, 0, key.Length);
                Array.Clear(salt, 0, salt.Length);
                if (otherKey != null)
                    Array.Clear(otherKey, 0, otherKey.Length);
            }
        }

        private static bool CompareInConstantTime<T>(T[] array1, T[] array2)
        {
            bool isEqual = true;
            for (int index = 0; index < array1.Length; ++index) {
                if (!array1[index].Equals(array2[index])) {
                    isEqual = false;
                }
            }
            return isEqual;
        }

        public CryptoStream CreateEncryptorStream(Stream stream)
        {
            return new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        }

        public CryptoStream CreateDecryptorStream(Stream stream)
        {
            return new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        }

        public byte[] Encrypt(byte[] data)
        {
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public byte[] Decrypt(byte[] data)
        {
            try {
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
            catch {
                throw new FunctionException(ErrorClient.Unauthorized, "Attempted to use an invalid encryption");
            }
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!IsDisposed) {
                encryptor.Dispose();
                decryptor.Dispose();
                rij.Clear();
                IsDisposed = true;
            }
        }
    }
}