namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class PerMilleLoadingCalculatorInput
    {
        public decimal CoverAmount { get; }
        public decimal PerMilleLoading { get; }
        public decimal PerMilleLoadingFactor { get; set; }
        public decimal FactorB { get; }

        public PerMilleLoadingCalculatorInput(decimal coverAmount, decimal perMilleLoading, decimal perMilleLoadingFactor, decimal factorB)
        {
            CoverAmount = coverAmount;
            PerMilleLoading = perMilleLoading;
            PerMilleLoadingFactor = perMilleLoadingFactor;
            FactorB = factorB;
        }
    }
}