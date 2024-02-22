using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController (ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository,
                                IClientRepository clientRepository, IAccountRepository accountRepository,
                                ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        [HttpGet] 
        public IActionResult Get() 
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                var loansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans) 
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(newLoanDTO);
                }
                return Ok(loansDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public IActionResult CreateLoan(LoanRequestDTO loanRequestDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;

                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                var loan = _loanRepository.FindById(loanRequestDTO.LoanId);

                if (loan == null)
                {
                    return StatusCode(404, "Prestamo no encontrado");
                }
                // Validar que el monto del préstamo no esté vacío y sea mayor a 1000 
                if (loanRequestDTO.Amount < 1000 || string.IsNullOrEmpty(loanRequestDTO.Payments))
                {
                    return StatusCode(400, "Datos de préstamo inválidos");
                }
                // Validar que el monto solicitado no sea mayor que el máximo a otorgar
                if (loanRequestDTO.Amount > loan.MaxAmount)
                {
                    return StatusCode(400, "El monto solicitado excede el máximo permitido");
                }
                
                string[] availablePayments = loan.Payments.Split(',');

                // Validar que la cantidad de cuotas exista
                if (!availablePayments.Contains(loanRequestDTO.Payments))
                {
                    return StatusCode(400, "La cantidad de cuotas seleccionada no está disponible");
                }
                // Validar que la cuenta de destino exista
                var account = _accountRepository.FindByNumber(loanRequestDTO.ToAccountNumber);
                if (account == null)
                {
                    return StatusCode(400, "La cuenta de destino no existe");
                }
                // Validar que la cuenta de destino pertenezca al cliente autenticado
                if (account.ClientId != client.Id)
                {
                    return Forbid();
                }

                var clientLoans = _clientLoanRepository.FindByClientId(client.Id);

                // Validar si el cliente ya tiene un préstamo del mismo tipo
                foreach (ClientLoan clientLoan in clientLoans)
                {
                    var existingLoan = _loanRepository.FindById(clientLoan.LoanId);
                    if (existingLoan.Name == loan.Name)
                    {
                        return StatusCode(400, $"El cliente ya tiene un préstamo de tipo {loan.Name}");
                    }
                }
                double loanAmount = loanRequestDTO.Amount * 1.20;

                var newclientLoan = new ClientLoan
                {
                    LoanId = loan.Id,
                    Amount = loanAmount,
                    Payments = loanRequestDTO.Payments,
                    ClientId = client.Id 
                };
                _clientLoanRepository.Save(newclientLoan);

                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    Type = TransactionType.CREDIT,
                    Amount = loanAmount,
                    Description = $"Préstamo {loan.Name} aprobado"
                };
                _transactionRepository.Save(transaction);

                account.Balance += loanAmount;
                _accountRepository.Save(account);

                return StatusCode(201, "Solicitud de préstamo creada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }        
        }
    }
}
    