using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository,
                                        ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        [HttpPost]
        public IActionResult Post([FromBody] TransferRequestDTO transferRequestDTO)
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
                //Validar que los parámetros ingresados no estén vacios.
                if (transferRequestDTO.FromAccountNumber == string.Empty || transferRequestDTO.ToAccountNumber == string.Empty)
                {
                    return StatusCode(403,"Números de cuenta de origen o destino no proporcionado");
                }
                //Validar que se proporcione un monto de transferencia + y mayor a 1
                if (transferRequestDTO.Amount < 1)
                {
                    return StatusCode(403,"El monto mínimo para transferir es $1");
                }
                //Validar monto máximo que se puede transferir 
                if (transferRequestDTO.Amount > 10000000)
                {
                    return StatusCode(403,"Para transferencias superiores a 10.000.000 comunicarse con el Banco");
                }
                //Validar que el haya un máximo de 2 decimales
                string amountDec = transferRequestDTO.Amount.ToString();
                int decPointIndex = amountDec.IndexOf('.');

                if (decPointIndex != -1 && (amountDec.Length - decPointIndex) > 3) 
                {
                    return StatusCode(403,"El monto no puede tener más de dos decimales.");
                }
                //Validar que no se realice una transferencia a la cuenta propia.
                if (transferRequestDTO.FromAccountNumber == transferRequestDTO.ToAccountNumber)
                {
                    return StatusCode(403,"Prohibido");
                }

                //Validar que se proporcione una descripción
                if (transferRequestDTO.Description == string.Empty)
                {
                    return StatusCode(403, "La descripción no puede estar vacía.");
                }
                // Validar que exista la cuenta de origen
                Account fromAccount = _accountRepository.FindByNumber(transferRequestDTO.FromAccountNumber);
                if (fromAccount == null)
                {
                    return StatusCode(403, "Cuenta de origen inexistente");
                }
                //Validar que tenga fondos
                if (fromAccount.Balance < transferRequestDTO.Amount)
                {
                    return StatusCode(403, "Fondos insuficientes");
                }
                //Validar que exista la cuenta de destino
                Account toAccount = _accountRepository.FindByNumber(transferRequestDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return StatusCode(403, "Cuenta de destino inexistente");
                }
                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.DEBIT,
                    Amount = transferRequestDTO.Amount * -1,
                    Description = transferRequestDTO.Description + " " + toAccount.Number,
                    AccountId = fromAccount.Id,
                    Date = DateTime.Now,
                });

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = transferRequestDTO.Amount,
                    Description = transferRequestDTO.Description + " " + fromAccount.Number,
                    AccountId = toAccount.Id,
                    Date = DateTime.Now,
                });
                fromAccount.Balance = fromAccount.Balance - transferRequestDTO.Amount;

                _accountRepository.Save(fromAccount);

                toAccount.Balance = toAccount.Balance + transferRequestDTO.Amount;

                _accountRepository.Save(toAccount);

                return Created("Transferencia realizada", fromAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
            