using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class LargeSumInsuredDiscountFactorDto : DbItem, IPremiumCalculationFactorResult
    {
        public decimal MinSumInsured { get; set; }
        public decimal MaxSumInsured { get; set; }
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}