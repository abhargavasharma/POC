using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class IncreasingClaimsFactorDto : DbItem
    {
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public int BenefitPeriod { get; set; }
        public bool IncreasingClaimsEnabled { get; set; }
        public decimal Factor { get; set; }
    }
}