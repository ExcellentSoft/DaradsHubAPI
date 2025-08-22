using DaradsHubAPI.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DaradsHubAPI.Shared.Extentions
{
    public static class StringExtensions
    {
        public static string Encrypt(this string textToEncrypt)
        {
            return CryptionManagerMD5.Encrypt(textToEncrypt);
        }

        public static string Decrypt(this string textToDecrypt)
        {
            return CryptionManagerMD5.Decrypt(textToDecrypt);
        }

        public static string EncryptSHA512(this string textToEncrypt)
        {

            return CryptionManagerSHA512.EncrpyptSHA512String(textToEncrypt);
        }
        public static string EncryptSHA512ToBase64(this string textToEncrypt)
        {

            return CryptionManagerSHA512.EncrpyptSHA512ToBase4(textToEncrypt);
        }

        public static bool IsValidEmail(this string email)
        {
            var e = new EmailAddressAttribute();
            return !string.IsNullOrEmpty(email) && e.IsValid(email);
        }
        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            bool isPhoneNo = false;

            isPhoneNo = Regex.Match(phoneNumber, @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$").Success;

            return isPhoneNo;
        }

        public static bool IsValidNumber(this string text)
        {
            bool isNumber = Regex.IsMatch(text, @"^[1-9]\d{0,2}(\.\d{3})*(,\d+)?$", RegexOptions.IgnoreCase);
            return isNumber;
        }
    }
}