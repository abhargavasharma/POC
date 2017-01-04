using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationClassFactorDto : DbItem, IPremiumCalculationFactorResult
    {
        public Gender Gender { get; set; }
        public string OccupationClass { get; set; }
        public string PlanCode { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}
