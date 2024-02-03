using HomeBankingMindHub.Models;

namespace HomeBankingMinHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "m.ale@gmail.com", FirstName="Alejandro", LastName="Martinez", Password="abc"},
                    new Client { Email = "mariap@gmail.com", FirstName="Maria", LastName="Pola", Password="cde"},
                    new Client { Email = "Tati88@gmail.com", FirstName="Tatiana", LastName="Castillo", Password="efg"},
                    new Client { Email = "Joseb@gmail.com", FirstName="Jose", LastName="Bora", Password="hij"}
                };

                context.Clients.AddRange(clients);

                //guardamos
                context.SaveChanges();
            }
        }
    }
}
