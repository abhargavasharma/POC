using System.Collections.Generic;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class RiskCalculatorInput
    {
        public IReadOnlyList<PlanPremiumCalculationResult> PlanResultsWithDiscounts { get; }

        public RiskCalculatorInput(IReadOnlyList<PlanPremiumCalculationResult> planResultsWithDiscounts)
        {
            PlanResultsWithDiscounts = planResultsWithDiscounts;
        }
    }
}