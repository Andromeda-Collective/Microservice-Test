using System.Security.Cryptography;
using System.Text;

namespace User_Service.Common.Utilities.Implementations;

public static class HashHelper
{
    public static string Hash(string value)
    {
        byte[] valueBytes = Encoding.UTF8.GetBytes(value);
        byte[] hashBytes = SHA256.HashData(valueBytes);


        string hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hashHex;
    }
}