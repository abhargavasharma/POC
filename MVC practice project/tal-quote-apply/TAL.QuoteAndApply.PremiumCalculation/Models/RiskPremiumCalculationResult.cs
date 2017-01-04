using System.Collections.Generic;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class RiskPremiumCalculationResult
    {
        public IReadOnlyList<PlanPremiumCalculationResult> PlanPremiumCalculationResults { get; private set; }

        public int RiskId { get; }
        public decimal RiskPremium { get; }
        public decimal MultiPlanDiscount { get; }

        public RiskPremiumCalculationResult(int riskId, decimal riskPremium, decimal multiPlanDiscount, 
            IReadOnlyList<PlanPremiumCalculationResult> planPremiumCalculationResults)
        {
            RiskId = riskId;
            RiskPremium = riskPremium;
            MultiPlanDiscount = multiPlanDiscount;
            PlanPremiumCalculationResults = planPremiumCalculationResults;
        }
    }
}