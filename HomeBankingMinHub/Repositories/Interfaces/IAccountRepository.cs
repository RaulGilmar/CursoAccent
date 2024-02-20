using HomeBankingMindHub.Models;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FindByNumber(string number);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        
    }
}
