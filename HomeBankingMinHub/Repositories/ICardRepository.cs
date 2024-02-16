using HomeBankingMindHub.Models;


namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        Card FindByIdAndTypeAndColor(long clientId, CardType type, CardColor color);
        int CountByType(long clientId, CardType type);
        int CountByColor(long clientId, CardColor color);
    }
}
