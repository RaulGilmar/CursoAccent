using HomeBankingMindHub.dtos;

using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;
using HomeBankingMinHub.Models.Model;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;

using System.Collections.Generic;

using System.Linq;



namespace HomeBankingMindHub.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class ClientsController : ControllerBase

    {

        private IClientRepository _clientRepository;



        public ClientsController(IClientRepository clientRepository)

        {

            _clientRepository = clientRepository;

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

                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO

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

                            Type = c.Type,
                            Color = c.Color,
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

                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO

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
                        
                        Type = c.Type,
                        Color = c.Color,                        
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
        public IActionResult Post([FromBody] ClientPost model)
        {
            try
            {
                if (model.FirstName.IsNullOrEmpty() || model.LastName.IsNullOrEmpty() || model.Email.IsNullOrEmpty() )
                {
                    return BadRequest("Algún o algunos de los datos son erróneos, ingrese nuevamente los datos");
                }

               
                var newClient = new Client
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Email                };

                _clientRepository.Save(newClient);

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }

}

