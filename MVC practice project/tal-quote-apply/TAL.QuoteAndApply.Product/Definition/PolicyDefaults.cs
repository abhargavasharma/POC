using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Product.Definition
{
    public class PolicyDefaults
    {
        public PremiumFrequency PremiumFrequency { get; }

        public int BrandId { get; }
        public string BrandKey { get; }
        public int OrganisationId { get; }

        public PolicyDefaults(PremiumFrequency premiumFrequency, int brandId, string brandKey, int organisationId)
        {
            PremiumFrequency = premiumFrequency;
            BrandId = brandId;
            //TODO consider reloading brand from the DB
            BrandKey = brandKey;
            OrganisationId = organisationId;
        }
    }
}