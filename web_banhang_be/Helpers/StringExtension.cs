using System.Security.Cryptography;
using System.Text;

namespace WebBanHang.Helpers
{
    public static class StringExtension
    {
        public static string ToSha256Hash(this string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
