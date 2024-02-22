using System.Text;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Utils
{
    public static class HomeUtils
    {
        private static Random random = new Random();
        public static string GenerateAccountNumber()
        {
            return $"VIN-{random.Next(00000001, 99999999)}";
        }
        public static int GenerateRandomCvv()
        {
            return random.Next(001, 999);
        }
        public static string GenerateRandomCardNumber()
        {
            var groups = Enumerable.Range(0, 4).Select(_ => random.Next(0001, 9999).ToString());           
            return string.Join("-", groups);
        }
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

