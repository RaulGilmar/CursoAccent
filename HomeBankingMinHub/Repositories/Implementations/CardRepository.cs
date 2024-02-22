using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public Card FindByIdAndTypeAndColor(long clientId, CardType type, CardColor color)
        {
            return FindByCondition(card => card.ClientId == clientId && card.Type == type && card.Color == color)
                .FirstOrDefault();
        }
        public Card FindById(long id)
        {
            return FindByCondition(card => card.Id == id)
                  .FirstOrDefault();
        }
        public Card FindByNumber(string number)
        {
            return FindByCondition(card => card.Number == number)
                .FirstOrDefault();
        }
        public IEnumerable<Card> GetAllCards()
        {
            return FindAll()
                  .ToList();
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
        public int CountByType(long clientId, CardType type)
        {
            return FindByCondition(card => card.ClientId == clientId && card.Type == type).Count();
        }
        public int CountByColor(long clientId, CardColor color)
        {
            return FindByCondition(card => card.ClientId == clientId && card.Color == color).Count();
        }
    }
}
