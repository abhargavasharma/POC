using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PlansUpdateResponse
    {
        public ComprehensivePlanUpdateResponse CurrentActivePlan { get; set; }
        public List<PlanUpdateResponse> Plans { get; set; }
        public PlanStatus OverallStatus { get; set; }
		public RiskPremiumSummaryViewModel RiskPremiumSummary { get; set; }
        public bool IsOccupationTpdAny { get; set; }
        public bool IsOccupationTpdOwn { get; set; }
    }

    public class PlanUpdateResponse
    {
        public int PlanId { get; set; }
        public bool Selected { get; set; }
        public decimal Premium { get; set; }
        public decimal PremiumIncludingRiders { get; set; }
        public string Code { get; set; }
        public PlanStatus Status { get; set; }
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }
        public Dictionary<string, IEnumerable<string>> Warnings { get; set; }
    }

    public class ComprehensivePlanUpdateResponse : PlanUpdateResponse
    {
        public int CoverAmount { get; set; }
        public bool? LinkedToCpi { get; set; }
        public bool? PremiumHoliday { get; set; }
        public IEnumerable<UpdateCoverResponse> Covers { get; set; }
        public string PremiumFrequency { get; set; }
		public IEnumerable<ComprehensivePlanUpdateResponse> Riders { get; set; } 
        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public string OccupationDefinition { get; set; }
    }

    public class UpdateCoverResponse
    {
        public string Code { get; set; }
        public bool Selected { get; set; }
        public decimal Premium { get; set; }
    }

    public class UpdateWarningResponse
    {
        public string Message { get; set; }
        public string Location { get; set; }
    }
}