using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class IndemnityFactorDto : DbItem
    {
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }

    
}