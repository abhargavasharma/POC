namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverCalculationRequest : ICoverFactors
    {
        public string CoverCode { get; }
        public int BrandId { get; }
        public bool Active { get; }
        public bool IsRateableCover { get; }
        public bool PercentageLoadingSupported { get; }
        public bool PerMilleLoadingSupported { get; }
        public Loadings Loadings { get; }

        public CoverCalculationRequest(string coverCode, bool active, bool isRateableCover,  bool percentageLoadingSupported, bool perMilleLoadingSupported, Loadings loadings, int brandId)
        {
            CoverCode = coverCode;
            Active = active;
            IsRateableCover = isRateableCover;
            PercentageLoadingSupported = percentageLoadingSupported;
            PerMilleLoadingSupported = perMilleLoadingSupported;
            Loadings = loadings;
            BrandId = brandId;
        }

        public CoverCalculationRequest(CoverCalculationRequest ccr) 
            : this(ccr.CoverCode, ccr.Active, ccr.IsRateableCover, ccr.PercentageLoadingSupported, ccr.PerMilleLoadingSupported, ccr.Loadings, ccr.BrandId)
        { }
    }
}