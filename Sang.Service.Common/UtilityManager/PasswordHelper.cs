using System.Security.Cryptography;
using System.Text;

namespace Sang.Service.Common.UtilityManager
{
    public static class PasswordHelper
    {
        private const string Key = "a8h3GZ9KsNp5Rv2t";
        public static string HashPasswordWithChannelId(
            string password, int channelId) => Encrypt(password + channelId);

        public static bool VerifyPasswordWithChannelId(string password, int channelId, string hashedPassword)
        {
            string combinedString = password + channelId;
            bool passwordMatch = VerifyPassword(combinedString, hashedPassword);

            return passwordMatch;
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[16];

            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] encryptedBytes = memoryStream.ToArray();
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[16];

            using MemoryStream memoryStream = new MemoryStream(encryptedBytes);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
        public static bool VerifyPassword(string plainPassword,
                                          string encryptedPassword) => Encrypt(plainPassword) == encryptedPassword;
    }
}
