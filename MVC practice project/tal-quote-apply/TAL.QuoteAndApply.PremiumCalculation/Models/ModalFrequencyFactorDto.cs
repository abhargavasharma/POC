using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class ModalFrequencyFactorDto : DbItem
    {
        public PremiumFrequency PremiumFrequency { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}