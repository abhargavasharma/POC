namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class MultiCoverDiscountCoverCalculatorInput
    {
        public decimal Premium { get; }
        public bool IncludeInMultiCoverDiscount { get; }

        public MultiCoverDiscountCoverCalculatorInput(decimal premium, bool includeInMultiCoverDiscount)
        {
            Premium = premium;
            IncludeInMultiCoverDiscount = includeInMultiCoverDiscount;
        }
    }
}