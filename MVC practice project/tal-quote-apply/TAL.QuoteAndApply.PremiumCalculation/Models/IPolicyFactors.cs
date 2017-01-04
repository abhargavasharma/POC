using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public interface IPolicyFactors
    {
        PremiumFrequency PremiumFrequency { get; }
        int BrandId { get; }
    }
}