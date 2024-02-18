using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAuthService
    {
        Task<string> Login(Client LoginRequestDTO);
        Task Logout();
    }
}
