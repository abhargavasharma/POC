namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public interface ICoverFactors
    {
        string CoverCode { get; }
        int BrandId { get; }
        bool Active { get; }
        /// <summary>
        /// The cover Has Rates and is included in the multi cover discount
        /// </summary>
        bool IsRateableCover { get; }

        Loadings Loadings { get; }

        bool PercentageLoadingSupported { get; }
        bool PerMilleLoadingSupported { get; }
    }
}