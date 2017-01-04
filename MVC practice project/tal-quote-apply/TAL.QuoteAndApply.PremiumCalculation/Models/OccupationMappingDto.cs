using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationMappingDto : DbItem
    {
        public string OccupationClass { get; set; }
        public string OccupationGroup { get; set; }
        public int BrandId { get; set; }
    }
}