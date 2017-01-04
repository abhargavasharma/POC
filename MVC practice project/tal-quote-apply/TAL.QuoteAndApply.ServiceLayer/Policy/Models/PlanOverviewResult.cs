using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PlanOverviewResult
    {
        public string QuoteNumber { get; set; }
        public  int CoverAmount { get; set; }
        public int MaxCoverAmount { get; set; }
        public int MinCoverAmount { get; set; }
        public int Age { get; set; }
        public decimal? Income { get; set; }
        public int PolicyId { get; set; }
        public int RiskId { get; set; }
        public IEnumerable<CoverDefinition> Covers { get; set; }
        public Dictionary<int, bool> ReliefFactors { get; set; }
        public string Code { get; set; }
        public bool? LinkedToCpi { get; set; }
        public int PlanId { get; set; }
        public bool Selected { get; set; }
        public decimal Premium { get; set; }
        public bool? PremiumHoliday { get; set; }
        public PremiumType PremiumType { get; set; }
        public int? ParentPlanId { get; set; }
        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public OccupationDefinition OccupationDefinition { get; set; }

        public bool IsRider => ParentPlanId != null;
    }
}
