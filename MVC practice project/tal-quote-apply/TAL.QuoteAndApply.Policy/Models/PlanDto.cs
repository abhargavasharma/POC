using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PlanDto : DbItem, IPlan
    {
        public int CoverAmount { get; set; }
        public int PolicyId { get; set; }
        public int RiskId { get; set; }
        public int? ParentPlanId { get; set; }
        public string Code { get; set; }
        public bool? LinkedToCpi { get; set; }
        public bool? PremiumHoliday { get; set; }
        public bool Selected { get; set; }
        public decimal Premium { get; set; }
        public decimal MultiCoverDiscount { get; set; }
        public decimal MultiPlanDiscount { get; set; }
        public decimal MultiPlanDiscountFactor { get; set; }
        public PremiumType PremiumType { get; set; }
        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public OccupationDefinition OccupationDefinition { get; set; }
    }
}
