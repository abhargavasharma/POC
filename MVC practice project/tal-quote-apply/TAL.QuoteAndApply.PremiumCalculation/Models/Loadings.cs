namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class Loadings
    {
        public decimal PercentageLoading { get; }
        public decimal PerMilleLoading { get; }

        public Loadings(decimal percentageLoading, decimal perMilleLoading)
        {
            PercentageLoading = percentageLoading;
            PerMilleLoading = perMilleLoading;
        }
    }
}