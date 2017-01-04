namespace RallyCardUpdater.Models
{
    public class CardId
    {
        public string Id { get; set; }
        public RallyCardType CardType { get; set; }

        public CardId(string id, RallyCardType cardType)
        {
            Id = id;
            CardType = cardType;
        }
    }
}