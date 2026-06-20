using System.Security.Cryptography;
using System.Text;

public static class GlobalStaticService
{
    public static string HashData(string plainText, string key)
    {
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var computedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToBase64String(computedBytes);
    }
}