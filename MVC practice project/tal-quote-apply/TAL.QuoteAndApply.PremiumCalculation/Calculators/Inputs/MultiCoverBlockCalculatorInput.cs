using System.Collections.Generic;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class MultiCoverBlockCalculatorInput
    {
        public MultiCoverDiscountCalculatorInput MultiCoverDiscountCalculatorInput { get; }
        public IReadOnlyList<decimal> IncludedCoverBlockPremiums { get; }

        public MultiCoverBlockCalculatorInput(MultiCoverDiscountCalculatorInput multiCoverDiscountCalculatorInput, IReadOnlyList<decimal> includedCoverBlockPremiums)
        {
            MultiCoverDiscountCalculatorInput = multiCoverDiscountCalculatorInput;
            IncludedCoverBlockPremiums = includedCoverBlockPremiums;
        }
    }
}