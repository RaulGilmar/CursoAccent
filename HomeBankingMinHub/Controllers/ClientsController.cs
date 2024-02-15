using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Models.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HomeBankingMindHub.Utils;
using System.ComponentModel.DataAnnotations;




namespace HomeBankingMindHub.Controllers

{

    [Route("api/[controller]")]

    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;           
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
                    return BadRequest("Algún o algunos de los datos son erróneos, ingrese nuevamente los datos");
                }

                
               
                if (_clientRepository.FindByEmail(client.Email)!=null)
                {
                    return Forbid("El usuario ya existe");
                }

                string passwordHash = HashUtils.GeneratePasswordHash(client.Password);

                var newClient = new Client
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Password = passwordHash               
                };

                _clientRepository.Save(newClient);

                var newAccount = new Account
                {
                    Number = AccountUtils.GenerateAccountNumber(),
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
                    return StatusCode(403, "Prohibido");
                }

                var newAccount = new Account
                {
                    Number = AccountUtils.GenerateAccountNumber(),
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    Client = client
                };    
                
                _accountRepository.Save(newAccount);

                return StatusCode(201, "Cuenta creada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        

    }

}

