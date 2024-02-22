using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HomeBankingMindHub.Utils;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository 
                                 accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();

                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList(),

                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),

                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Type = c.Type.ToString(),
                            Color = c.Color.ToString(),
                            Number = c.Number,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            ThruDate = c.ThruDate
                        }).ToList()
                    };
                       clientsDTO.Add(newClientDTO);
                }
                   return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
               return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);

                if (client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),

                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)

                    }).ToList(),                   
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,                        
                        Type = c.Type.ToString(),
                        Color = c.Color.ToString(),                        
                        Number = c.Number,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,                        
                        ThruDate = c.ThruDate                        
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }        
        [HttpPost]
        public IActionResult Post([FromBody] LoginRequestDTO client)
        {
            try
            {
                if (client.FirstName.IsNullOrEmpty() || client.LastName.IsNullOrEmpty() || client.Email.IsNullOrEmpty() 
                    || client.Password.IsNullOrEmpty())
                {
                    return StatusCode(400, "Algún o algunos de los datos son erróneos, ingrese nuevamente los datos");
                }                          
                if (_clientRepository.FindByEmail(client.Email)!=null)
                {
                    return StatusCode(403,"El usuario ya existe");
                }

                string passwordHash = HomeUtils.GeneratePasswordHash(client.Password);

                var newClient = new Client
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Password = passwordHash               
                };
                _clientRepository.Save(newClient);

                string generatedAccountNumber;
                Account existingAccount;

                do
                {
                    generatedAccountNumber = HomeUtils.GenerateAccountNumber();
                    existingAccount = _accountRepository.FindByNumber(generatedAccountNumber);
                }
                while (existingAccount != null);

                var newAccount = new Account
                {
                    Number = generatedAccountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    Client = newClient
                };
                _accountRepository.Save(newAccount);
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current")]
        public IActionResult GetCurrent() 
        {
            try 
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty) 
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null) 
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),

                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),

                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/accounts")]
        public IActionResult GetCurrentAccounts()
        {
            try
            {                
                string email = User.FindFirst("Client")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    return Forbid(); 
                }
              
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid(); 
                }

                var accounts = client.Accounts.Select(ac => new AccountDTO
                {
                    Id = ac.Id,
                    Balance = ac.Balance,
                    CreationDate = ac.CreationDate,
                    Number = ac.Number
                }).ToList();
                return Ok(accounts); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
        [HttpPost("current/accounts")]
        public IActionResult CreateAccount() 
        {
            try 
            {               
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                if (client.Accounts.Count > 2)
                {
                    return Forbid();
                }

                string generatedAccountNumber;
                Account existingAccount;

                do
                {
                    generatedAccountNumber = HomeUtils.GenerateAccountNumber();
                    existingAccount = _accountRepository.FindByNumber(generatedAccountNumber);
                }
                while (existingAccount != null);
                var newAccount = new Account                
                {
                    Number = generatedAccountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                };              
                _accountRepository.Save(newAccount);
                _clientRepository.Update(client);
                return StatusCode(201, "Cuenta creada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("current/cards")]
        public IActionResult CreateCard(CardRequestDTO cardRequest)  
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                       
                CardType parsedType = (CardType)Enum.Parse(typeof(CardType),cardRequest.Type,true);

                CardColor parsedColor = (CardColor)Enum.Parse(typeof(CardColor),cardRequest.Color,true);

                if (client == null)
                {
                    return Forbid();
                }

                if (_cardRepository.FindByIdAndTypeAndColor(client.Id, parsedType, parsedColor) != null) 
                {
                    return Forbid();
                }

                int cantType = _cardRepository.CountByType(client.Id, parsedType);
                int cantColor = _cardRepository.CountByColor(client.Id, parsedColor);

                if (cantType >= 3)
                {
                    return Forbid();
                }

                if (cantColor>= 1)
                {
                    return Forbid();
                }

                string generatedCardNumber;
                Account existingCard;

                do
                {
                    generatedCardNumber = HomeUtils.GenerateRandomCardNumber();
                    existingCard = _accountRepository.FindByNumber(generatedCardNumber);
                }
                while (existingCard != null);

                var newCard = new Card
                { 
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = parsedType,
                    Color = parsedColor,
                    Number = generatedCardNumber,
                    Cvv = HomeUtils.GenerateRandomCvv(),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                    ClientId = client.Id
                };
                _cardRepository.Save(newCard);
                _clientRepository.Update(client);
                return StatusCode(201, "Tarjeta creada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

