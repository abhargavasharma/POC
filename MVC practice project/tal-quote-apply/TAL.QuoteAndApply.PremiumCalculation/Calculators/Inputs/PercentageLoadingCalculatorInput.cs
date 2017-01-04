namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class PercentageLoadingCalculatorInput
    {
        public decimal Premium { get; }

        /// <summary>
        /// As whole number not part of 1, eg: 10 not .10
        /// </summary>
        public decimal PercentageLoading { get; }

        public decimal PercentageLoadingFactor { get; }

        public PercentageLoadingCalculatorInput(decimal premium, decimal percentageLoading, decimal percentageLoadingFactor)
        {
            Premium = premium;
            PercentageLoading = percentageLoading;
            PercentageLoadingFactor = percentageLoadingFactor;
        }
    }
}
