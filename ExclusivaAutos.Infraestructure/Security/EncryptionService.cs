using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ExclusivaAutos.Infraestructure.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(string encryptionKey, string encryptionIv)
        {
            _key = DeriveKey(encryptionKey, 32);
            _iv = DeriveKey(encryptionIv, 16);
        }


        private byte[] DeriveKey(string input, int requiredLength)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            if (requiredLength <= hash.Length)
            {
                var result = new byte[requiredLength];
                Array.Copy(hash, result, requiredLength);
                return result;
            }

            var finalHash = new byte[requiredLength];
            var position = 0;

            while (position < requiredLength)
            {
                var bytesToCopy = Math.Min(hash.Length, requiredLength - position);
                Array.Copy(hash, 0, finalHash, position, bytesToCopy);
                position += bytesToCopy;

                if (position < requiredLength)
                {
                    hash = sha256.ComputeHash(hash);
                }
            }

            return finalHash;
        }
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var buffer = Convert.FromBase64String(cipherText);
                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (FormatException)
            {

                return cipherText;
            }
            catch (CryptographicException)
            {
                return cipherText;
            }
        }
    } }
