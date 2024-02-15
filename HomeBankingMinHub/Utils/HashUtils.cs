using System.Text;
using System.Security.Cryptography;
using System.Text;

namespace HomeBankingMindHub.Utils
{
    public static class HashUtils
    {
        public static string GeneratePasswordHash(string password) 
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }
    }
}
