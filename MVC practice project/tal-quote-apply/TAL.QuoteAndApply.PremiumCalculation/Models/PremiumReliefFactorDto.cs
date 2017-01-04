using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumReliefFactorDto : DbItem
    {
        public bool Selected { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}
