using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class RiskPlanDetailReposone
    {
        public IEnumerable<PlanDetailResponse> Plans { get; set; }
        public bool IsOccupationTpdAny { get; set; }
        public bool IsOccupationTpdOwn { get; set; }
    }

    public class PlanDetailResponse
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string PlanType { get; set; }
        public string Code { get; set; }
        public int CoverAmount { get; set; }
        public bool? LinkedToCpi { get; set; }
        public bool? PremiumHoliday { get; set; }
        public bool Selected { get; set; }
        public int? WaitingPeriod { get; set; }
        public int? BenefitPeriod { get; set; }
        public OccupationDefinition OccupationDefinition { get; set; }

        public IEnumerable<PlanDetailCoverResponse> Covers { get; set; }
        public IEnumerable<PlanDetailOptionResponse> Options { get; set; }
        public IEnumerable<PlanDetailVariableResponse> Variables { get; set; }
        public IEnumerable<PlanDetailResponse> Riders { get; set; }
        public decimal Premium { get; set; }
        public decimal PremiumIncludingRiders { get; set; }
        
        public string PremiumFrequency { get; set; }
        public string PremiumType { get; set; }
        public bool IsFilledIn { get; set; }
        public bool EligibleForPlan { get; set; }
        public IEnumerable<string> IneligibleReasons { get; set; }
        public bool IsRider { get; set; }
    }

    public class PlanDetailCoverResponse
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Selected { get; set; }
        public PremiumType PremiumTypeId { get; set; }
        public PlanDetailCoverUnderwritingIndicator UnderwritingIndicator { get; set; }
        public decimal Premium { get; set; }
        public bool Eligible { get; set; }
        public IEnumerable<string> IneligibleReasons { get; set; }
    }

    public enum PlanDetailCoverUnderwritingIndicator
    {
        Accepted,
        Declined,
        NotCompleted
    }

    public class PlanDetailOptionResponse
    {
        public string Name { get; set; }
        public bool? Selected { get; set; }
        public string Code { get; set; }
    }

    public class PlanDetailVariableResponse
    {
        public string Name { get; set; }
        public string Code { get; set; }      
    }
}