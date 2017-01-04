using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class PremiumTypeResponse
    {
        public PremiumType PremiumType { get; set; }
        public string Name { get; set; }
        public int? MaximumEntryAgeNextBirthday { get; set; }
        public bool IsUserSelectable { get; set; }
    }
}
