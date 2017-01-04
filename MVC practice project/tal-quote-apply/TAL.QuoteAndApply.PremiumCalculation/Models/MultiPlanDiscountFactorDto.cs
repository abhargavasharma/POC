using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class MultiPlanDiscountFactorDto : DbItem
    {
        public int PlanCount { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}