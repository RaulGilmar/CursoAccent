using HomeBankingMindHub.Models;


namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        Card FindByIdAndTypeAndColor(long clientId, CardType type, CardColor color);
        Card FindByNumber(string number);
        int CountByType(long clientId, CardType type);
        int CountByColor(long clientId, CardColor color);
    }
}
