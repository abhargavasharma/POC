using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyPlan : IPlan
    {
        public int CoverAmount { get; set; }
        public int PolicyId { get; set; }
        public int RiskId { get; set; }
        public string BrandKey { get; set; }
        public int? ParentPlanId { get; set; }
        public string Code { get; set; }
        public bool? LinkedToCpi { get; set; }
        public bool Selected { get; set; }
        public decimal Premium { get; set; }
        public decimal MultiPlanDiscount { get; set; }
        public decimal MultiCoverDiscount { get; set; }
        public decimal MultiPlanDiscountFactor { get; set; }
        public PremiumType PremiumType { get; set; }
        public int Id { get; set; }
        public List<RaisePolicyCover> Covers { get; set; }
        public List<RaisePolicyOption> Options { get; set; }
        public bool? PremiumHoliday { get; set; }
        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public OccupationDefinition OccupationDefinition { get; set; }
    }
}