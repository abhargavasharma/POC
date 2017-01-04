using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.Product.Models.Definition
{ 

    public class PremiumTypeDefinition
    {
        public PremiumTypeDefinition(PremiumType premiumType, string name, int? maximumEntryAgeNextBirthday, bool isUserSelectable = true)
        {
            PremiumType = premiumType;
            Name = name;
            MaximumEntryAgeNextBirthday = maximumEntryAgeNextBirthday;
            IsUserSelectable = isUserSelectable;
        }

        public PremiumType PremiumType { get; private set; } 
        public string Name { get; private set; }
        public int? MaximumEntryAgeNextBirthday { get; private set; }
        public bool IsUserSelectable { get; private set; }
    }
}
