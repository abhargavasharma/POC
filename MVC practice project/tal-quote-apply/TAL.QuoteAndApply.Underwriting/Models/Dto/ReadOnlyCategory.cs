using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyCategory
    {
        public ReadOnlyCategory(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            OrderId = category.OrderId;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public int OrderId { get; private set; }
    }
}