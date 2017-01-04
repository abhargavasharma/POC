namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PlanPremiumAndDiscounts
    {
        public decimal Premium { get; }
        public decimal MultiPlanDiscount { get; }
        public decimal MultiPlanDiscountFactor { get; }
        public decimal MultiCoverDiscount { get; }

        public PlanPremiumAndDiscounts(decimal premium, decimal multiPlanDiscount, decimal multiPlanDiscountFactor, decimal multiCoverDiscount)
        {
            Premium = premium;
            MultiPlanDiscount = multiPlanDiscount;
            MultiPlanDiscountFactor = multiPlanDiscountFactor;
            MultiCoverDiscount = multiCoverDiscount;
        }
    }
}