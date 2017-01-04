namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingCategory
    {
        public string Id { get; }
        public string Name { get; }
        public int OrderId { get; }

        public UnderwritingCategory(string id, string name, int orderId)
        {
            Id = id;
            Name = name;
            OrderId = orderId;
        }
    }
}