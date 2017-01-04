using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PlanMinimumCoverAmountForMultiPlanDiscountDto : DbItem
    {
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal MinimumCoverAmount { get; set; }
    }
}
