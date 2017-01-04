using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class WaitingPeriodFactorDto : DbItem, IPremiumCalculationFactorResult
    {
        public int WaitingPeriod { get; set; }
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}