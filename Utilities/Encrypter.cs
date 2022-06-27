using System.Security.Cryptography;
using System.Text;

namespace Practice.Utilities
{
    public static class Encrypter
    {
        public static string EncryptSHA256(string input)
        {
            using var sha = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha.ComputeHash(inputBytes);
            var hashStrBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashStrBuilder.Append(b.ToString("x2"));
            }
            return hashStrBuilder.ToString();
        }
    }
}
