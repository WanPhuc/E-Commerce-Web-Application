using System.Security.Cryptography;
using System.Text;
using System.Xml.Schema;
namespace WebBanHang.Helpers;
public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashOfInput = HashPassword(password);
        return StringComparer.Ordinal.Compare(hashOfInput, hashedPassword) == 0;
    }
}