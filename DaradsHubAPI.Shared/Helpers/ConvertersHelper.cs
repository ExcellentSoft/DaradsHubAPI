using System;
using System.IO;
using System.Text;

namespace DaradsHubAPI.Shared.Helpers
{

    public static class ConvertersHelper
    {
        public static string ConvertByteToString(byte[] value)
        {
            string result = Encoding.UTF8.GetString(value, 0, value.Length);
            return result.Trim();

        }

        public static bool ConvertByteToBoolean(byte[] value)
        {
            bool returnValue = false;
            string result = Encoding.UTF8.GetString(value, 0, value.Length);
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Trim().ToLower() == "true")
                {
                    returnValue = true;
                }
            }
            return returnValue;

        }

        public static byte[] ConvertStringToByte(string value)
        {
            var result = Encoding.UTF8.GetBytes(value);
            return result;

        }
        public static byte[] HexadecimalStringToByteArray(string input)
        {
            var outputLength = input.Length / 2;
            var output = new byte[outputLength];
            using (var sr = new StringReader(input))
            {
                for (var i = 0; i < outputLength; i++)
                    output[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return output;
        }
    }
}
