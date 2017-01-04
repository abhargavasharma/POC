namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverPremiumCalculationResult
    {
        public string CoverCode { get; }
        public decimal BasePremium { get; }
        public decimal AdditionalPremium { get; }
        public decimal LoadingPremium { get; }

        public CoverPremiumCalculationResult(string coverCode, decimal basePremium, decimal additionalPremium)
        {
            CoverCode = coverCode;
            BasePremium = basePremium;
            AdditionalPremium = additionalPremium;
            LoadingPremium = 0;
        }

        public CoverPremiumCalculationResult(string coverCode, decimal basePremium, decimal additionalPremium, decimal loadingPremium)
            :this(coverCode, basePremium, additionalPremium)
        {
            LoadingPremium = loadingPremium;
        }

        public CoverPremiumCalculationResult WithLoadingPremium(decimal loadingPremium)
        {
            return new CoverPremiumCalculationResult(this.CoverCode, this.BasePremium, this.AdditionalPremium, loadingPremium);
        }

        public CoverPremiumCalculationResult WithBasePremium(decimal basePremium)
        {
            return new CoverPremiumCalculationResult(this.CoverCode, basePremium, this.AdditionalPremium, this.LoadingPremium);
        }

        public decimal TotalPremium
        {
            get
            {
                return BasePremium + AdditionalPremium + LoadingPremium;
            }
        }
    }
}