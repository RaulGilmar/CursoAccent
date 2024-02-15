namespace HomeBankingMindHub.Utils
{
    public static class AccountUtils
    {
        public static string GenerateAccountNumber() 
        {
            Random random = new Random();
            return $"VIN-{random.Next(00000001, 99999999)}";
        }

    }
}
