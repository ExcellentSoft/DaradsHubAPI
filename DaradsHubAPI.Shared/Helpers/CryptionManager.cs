using DaradsHubAPI.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DaradsHubAPI.Shared.Helpers
{
#nullable disable
    public class CryptionManagerMD5
    {//MD5

        private static string EncryptionKey = "F";
        private readonly AppSettings appSettings;
        public CryptionManagerMD5(IOptions<AppSettings> settings)
        {
            appSettings = settings.Value;
            EncryptionKey = appSettings.EncryptionKey;
        }
        public static string Encrypt(string unencryptedText)
        {
            if (string.IsNullOrEmpty(unencryptedText))
                throw new AppException("You cannot encrypt an empty text");

            var hashmd5 = new MD5CryptoServiceProvider();
            var hashedKeyBytes = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = hashedKeyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var unencryptedBytes = Encoding.UTF8.GetBytes(unencryptedText);
            var encryptedBytes = tdes.CreateEncryptor()
                                     .TransformFinalBlock(unencryptedBytes, 0, unencryptedBytes.Length);

            tdes.Clear();

            return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new AppException("You cannot decrypt an empty text");

            var hashmd5 = new MD5CryptoServiceProvider();
            var hashedKeyBytes = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = hashedKeyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = tdes.CreateDecryptor()
                                     .TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            tdes.Clear();

            return Encoding.UTF8.GetString(decryptedBytes);
        }

    }

    public class CryptionManagerSHA512
    {
        //SHA512

        private static string EncryptionKey = "E";
        private readonly AppSettings appSettings;
        public CryptionManagerSHA512(IOptions<AppSettings> settings)
        {
            appSettings = settings.Value;
            EncryptionKey = appSettings.EncryptionKey;
        }
        public static string EncrpyptSHA512String(string unhashedVal)
        {// Byte[] EncryptedSHA512 = sha512.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(PasswordSHA512, securityCode)));
            //sha512.Clear();
            SHA512 sham = new SHA512Managed();
            byte[] hash = sham.ComputeHash(Encoding.ASCII.GetBytes(unhashedVal));

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
        public static string EncrpyptSHA512ToBase4(string plainText)
        {
            SHA512 sham = new SHA512Managed();
            byte[] hash = sham.ComputeHash(Encoding.ASCII.GetBytes(plainText));
            return Convert.ToBase64String(hash);
        }
        public static string DecryptSHA512String(string encrptedValue)
        {
            return "";
        }
    }
}
