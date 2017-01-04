using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class SmokerFactorDto : DbItem, IPremiumCalculationFactorResult
    {
        public bool Smoker { get; set; }
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}