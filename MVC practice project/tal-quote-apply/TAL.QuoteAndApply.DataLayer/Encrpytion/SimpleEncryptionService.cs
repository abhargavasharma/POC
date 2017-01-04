using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.DataLayer.Encrpytion
{
    public interface ISimpleEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }

    public class SimpleEncryptionService : ISimpleEncryptionService
    {
        private const string PasswordHash = "k*UK{xE3dvqMeK3N";
        private const string SaltKey = "?Xv2.[>]CLv`a;<X";
        private const string IVKey = "@9\"\\yH:7Zh* r6pt>a";

        private static byte[] KeyByteArray { get; set; }
        private static byte[] IVKeyByteArray { get; set; }

        public string Encrypt(string plainText)
        {
            KeyByteArray = KeyByteArray ?? new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            IVKeyByteArray = IVKeyByteArray ?? Encoding.ASCII.GetBytes(IVKey);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            
            using (var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros })
            {
                using (var encryptor = symmetricKey.CreateEncryptor(KeyByteArray, IVKeyByteArray))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = memoryStream.ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            KeyByteArray = KeyByteArray ?? new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            IVKeyByteArray = IVKeyByteArray ?? Encoding.ASCII.GetBytes(IVKey);

            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            using (var symmetricKey = new RijndaelManaged() {Mode = CipherMode.CBC, Padding = PaddingMode.None})
            {
                using (var decryptor = symmetricKey.CreateDecryptor(KeyByteArray, IVKeyByteArray))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)
                                .TrimEnd("\0".ToCharArray());
                        }
                    }
                }
            }

        }
    }
}
