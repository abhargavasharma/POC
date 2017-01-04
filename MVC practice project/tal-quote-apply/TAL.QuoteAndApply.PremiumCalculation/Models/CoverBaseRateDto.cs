using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverBaseRateDto : DbItem, IBaseRate
    {
        public string PlanCode { get; set; }
        public string CoverCode { get; set; }
        public int Age { get; set; }
        public int BrandId { get; set; }
        public Gender Gender { get; set; }
        public PremiumType PremiumType { get; set; }
        public bool? IsSmoker { get; set; }
        public string OccupationGroup { get; set; }
        public int? BenefitPeriod { get; set; }
        public int? WaitingPeriod { get; set; }
        public bool? BuyBack { get; set; }
        public decimal BaseRate { get; set; }
    }
}
