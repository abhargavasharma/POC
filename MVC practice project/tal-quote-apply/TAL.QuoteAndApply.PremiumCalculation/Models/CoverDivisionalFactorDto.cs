using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverDivisionalFactorDto : DbItem
    {
        public string CoverCode { get; set; }
        public int BrandId { get; set; }
        public int DivisionalFactor { get; set; }
    }
}
