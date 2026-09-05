using System.Security.Cryptography;
using System.Text;
using System.Xml.Schema;
namespace AuraMart.Identity.Application;
public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    public static bool VerifyOldHash_SHA256(string password, string storedHash)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // Chuyển password thành byte array
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // Chuyển ngược lại thành chuỗi Hex để so sánh
            var builder = new System.Text.StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            string computedHash = builder.ToString();

            // So sánh không phân biệt hoa thường
            return computedHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
    public static bool VerifyOldHash_SHA256_Base64(string password, string storedHash)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // 1. Chuyển password người dùng nhập thành byte array
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // 2. Mã hóa mảng byte đó thành chuỗi Base64
            string computedHash = Convert.ToBase64String(bytes);

            // 3. So sánh trực tiếp (Base64 có phân biệt hoa thường nên dùng Equals chuẩn)
            return computedHash == storedHash;
        }
    }
}
