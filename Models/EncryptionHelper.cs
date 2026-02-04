using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BulkUploader.Models
{
     public static class EncryptionHelper
    {
        private static readonly string Key = "YourSecureEncryptionKey123456"; // Must be 32 characters

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key.Length > 32 ? Key.Substring(0, 32) : Key.PadRight(32, ' '));
                aes.IV = new byte[16];
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    return null;

                // Fix URL encoding issues
                string decodedText = Uri.UnescapeDataString(encryptedText).Replace(" ", "+");

                // Ensure valid Base64 padding
                int mod4 = decodedText.Length % 4;
                if (mod4 > 0)
                {
                    decodedText += new string('=', 4 - mod4);
                }

                byte[] encryptedBytes = Convert.FromBase64String(decodedText);

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(Key.PadRight(32, ' '));
                    aes.IV = new byte[16];

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch (FormatException ex)
            {
                throw new Exception("Decryption failed: Invalid Base64 string. Raw input: " + encryptedText, ex);
            }
        }

    }
}
