using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IClientLoanRepository
    {
        IEnumerable<ClientLoan> FindByClientId(long clientId);
        void Save(ClientLoan clientLoan);
    }
}
