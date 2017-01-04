using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PerMilleLoadingFactorDto : DbItem
    {
        public string CoverCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}
