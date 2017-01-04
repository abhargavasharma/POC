using System.Collections.Generic;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class PlanCalculatorInput
    {
        public IReadOnlyList<CoverPremiumCalculationResult> CoverCalculationResults { get; }
        public MultiCoverDiscountCalculatorInput MultiCoverDiscountCalculatorInput { get; }
        public decimal MultiPlanDiscountFactor { get; }

        public PlanCalculatorInput(MultiCoverDiscountCalculatorInput multiCoverDiscountCalculatorInput, IReadOnlyList<CoverPremiumCalculationResult> coverCalculationResults, decimal multiPlanDiscountFactor)
        {
            MultiCoverDiscountCalculatorInput = multiCoverDiscountCalculatorInput;
            CoverCalculationResults = coverCalculationResults;
            MultiPlanDiscountFactor = multiPlanDiscountFactor;
        }
    }
}