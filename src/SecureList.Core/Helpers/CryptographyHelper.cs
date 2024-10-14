using System.Security.Cryptography;
using System.Text;

namespace SecureList.Core.Helpers;

public static class CryptographyHelper
{
    public static (string md5Hash, string sha1Hash, string sha256Hash) CalculateHashes(string password)
    {
        string md5Hash, sha1Hash, sha256Hash;

        using var md5 = MD5.Create();
        using var sha1 = SHA1.Create();
        using var sha256 = SHA256.Create();

        md5Hash = CalculateHash(password, md5);
        sha1Hash = CalculateHash(password, sha1);
        sha256Hash = CalculateHash(password, sha256);

        return (md5Hash, sha1Hash, sha256Hash);
    }

    private static string CalculateHash(string password, HashAlgorithm algorithm)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);
        byte[] hashBytes = algorithm.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes);
    }
}
