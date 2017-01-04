using System.Collections.Generic;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PlanPremiumCalculationResult
    {
        public string PlanCode { get; }
        public decimal TotalPremium { get; }
        public decimal MultiPlanDiscount { get; }
        public decimal MultiCoverDiscount { get;  }
        public decimal MultiPlanDiscountFactor { get; }

        public IReadOnlyList<CoverPremiumCalculationResult> CoverPremiumCalculationResults { get; }

        public PlanPremiumCalculationResult(string planCode, decimal totalPremium, decimal multiPlanDiscount, decimal multiPlanDiscountFactor, decimal multiCoverDiscount, IReadOnlyList<CoverPremiumCalculationResult> coverPremiumCalculationResults)
        {
            PlanCode = planCode;
            TotalPremium = totalPremium;
            MultiPlanDiscount = multiPlanDiscount;
            MultiCoverDiscount = multiCoverDiscount;
            MultiPlanDiscountFactor = multiPlanDiscountFactor;
            CoverPremiumCalculationResults = coverPremiumCalculationResults;
        }
    }
}