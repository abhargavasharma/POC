using System.Collections.Generic;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class MultiCoverDiscountCalculatorInput
    {
        public decimal MultiCoverDiscountFactor { get; }
        public IReadOnlyList<MultiCoverDiscountCoverCalculatorInput> Covers { get; }

        public MultiCoverDiscountCalculatorInput(decimal multiCoverDiscountFactor, IReadOnlyList<MultiCoverDiscountCoverCalculatorInput> covers)
        {
            MultiCoverDiscountFactor = multiCoverDiscountFactor;
            Covers = covers;
        }
    }
}