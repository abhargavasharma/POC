using System.Collections.Generic;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumCalculationResult
    {
        public IReadOnlyList<RiskPremiumCalculationResult> RiskPremiumCalculationResults { get; }

        public PremiumCalculationResult(IReadOnlyList<RiskPremiumCalculationResult> riskPremiumCalculationResults)
        {
            RiskPremiumCalculationResults = riskPremiumCalculationResults;
        }
    }
}