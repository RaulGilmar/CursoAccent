using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Models
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

            if (!context.Accounts.Any()) 
            {
                var accountsAlejandro = context.Clients.FirstOrDefault(c => c.Email == "m.ale@gmail.com");

                if (accountsAlejandro != null)
                {
                    var accountsA = new Account[]
                    {
                        new Account 
                        {
                            ClientId = accountsAlejandro.Id, 
                            CreationDate=DateTime.Now,
                            Number = string.Empty,
                            Balance = 0
                        }
                    };

                    context.Accounts.AddRange(accountsA);                                        
                }
                var accountsMaria = context.Clients.FirstOrDefault(c => c.Email == "mariap@gmail.com");

                if (accountsMaria != null)
                {
                    var accountsM = new Account[]
                    {
                        new Account
                        {
                            ClientId = accountsMaria.Id,
                            CreationDate=DateTime.Now,
                            Number = string.Empty,
                            Balance = 0
                        }
                    };

                    context.Accounts.AddRange(accountsM);
                }
                var accountsTatiana = context.Clients.FirstOrDefault(c => c.Email == "Tatiana");

                if (accountsTatiana != null)
                {
                    var accountsT = new Account[]
                    {
                        new Account
                        {
                            ClientId = accountsTatiana.Id,
                            CreationDate=DateTime.Now,
                            Number = string.Empty,
                            Balance = 0
                        }
                    };

                    context.Accounts.AddRange(accountsT);
                }
                var accountsJose = context.Clients.FirstOrDefault(c => c.Email == "Joseb@gmail.com");

                if (accountsJose != null)
                {
                    var accountsJ = new Account[]
                    {
                        new Account
                        {
                            ClientId = accountsJose.Id,
                            CreationDate=DateTime.Now,
                            Number = string.Empty,
                            Balance = 0
                        }
                    };

                    context.Accounts.AddRange(accountsJ);
                }




                context.SaveChanges();

            }
        }
    }
}
