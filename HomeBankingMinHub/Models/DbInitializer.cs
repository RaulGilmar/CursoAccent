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
                var accountsTatiana = context.Clients.FirstOrDefault(c => c.Email == "Tati88@gmail.com");

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

             if (!context.Transactions.Any()) 
              {
                  var account1 = context.Accounts.FirstOrDefault(c => c.Number == "ALE001");

                if (account1 != null)
                {
                    var transactions = new Transaction[]
                      {
                         new Transaction
                         {

                              AccountId = account1.Id,
                              Amount = 30000,
                              Date = DateTime.Now.AddHours(-4),
                              Description = "Transferencia Recibida",
                              Type = TransactionType.CREDIT.ToString()
                         },

                         new Transaction
                         {

                              AccountId = account1.Id,
                              Amount = -5000,
                              Date = DateTime.Now.AddHours(-3),
                              Description = "Transferencia Enviada",
                              Type = TransactionType.DEBIT.ToString()
                         },

                           new Transaction
                         {

                              AccountId = account1.Id,
                              Amount = -10000,
                              Date = DateTime.Now.AddHours(-2),
                              Description = "Compra en tienda Mercado Libre",
                              Type = TransactionType.DEBIT.ToString()
                         },



                      };

                    foreach (Transaction transaction in transactions) 
                     {
                        context.Transactions.Add(transaction);
                     }

                    context.SaveChanges();

                }
            } 

             if (!context.Loans.Any()) 
             {
                var loans = new Loan[]
                {
                      new Loan
                        {
                            Name = "Hipotecario",
                            MaxAmount = 10000000,
                            Payments = "12,24,36,48,60"

                        },
                       new Loan
                        {
                            Name = "Personal",
                            MaxAmount = 600000,
                            Payments = "6,12,24,36"

                        },
                        new Loan
                        {
                            Name = "Automotriz",
                            MaxAmount = 2000000,
                            Payments = "12,24,36,48"

                        }
                                                                           
                };
                    
                foreach (var loan in loans) 
                {
                    context.Loans.Add(loan);        
                }

                context.SaveChanges();

                var client1 = context.Clients.FirstOrDefault(c => c.Email == "m.ale@gmail.com");

                if (client1 != null) 
                {
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");

                    if (loan1 != null) 
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 8000000,
                            Payments = "60",
                            ClientId = client1.Id,
                            LoanId = loan1.Id
                        };

                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");

                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 100000,
                            Payments = "12",
                            ClientId = client1.Id,
                            LoanId = loan2.Id
                        };

                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");

                    if (loan3 != null) 
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 80000,
                            Payments = "24",
                            ClientId = client1.Id,
                            LoanId = loan3.Id
                        };

                        context.ClientLoans.Add(clientLoan3);

                    }

                    context.SaveChanges();

                }
                var client2 = context.Clients.FirstOrDefault(c => c.Email == "Tati88@gmail.com");

                if (client2 != null) 
                {
                    var loan4 = context.Loans.FirstOrDefault(l => l.Name == "Personal");

                    if (loan4 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 600000,
                            Payments = "36",
                            ClientId = client2.Id,
                            LoanId = loan4.Id
                        };

                        context.ClientLoans.Add(clientLoan2);
                    }

                    context.SaveChanges();

                }
            }


        }
    }
}
