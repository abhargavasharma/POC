using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PolicyCorrespondenceViewModel
    {
        public string CustomerEmailAddress { get; set; }
        public string UserEmailAddress { get; set; }
        public string ClientFullName { get; set; }
        public bool EmailSent { get; set; }
        public bool IsValidForEmailCorrespondence { get; set; }
        public List<PlanCorrespondenceSummaryViewModel> PlanSummaries { get; set; }
        public string PremiumFrequency { get; set; }
    }

    public class PlanCorrespondenceSummaryViewModel
    {
        public bool IsRider { get; set; }
        public string Name { get; set; }
        public long CoverAmount { get; set; }
        public string ParentName { get; set; }
        public decimal Premium { get; set; }
        public List<CoverCorrespondenceSummaryViewModel> CoverCorrespondenceSummaries { get; set; }
    }

    public class CoverCorrespondenceSummaryViewModel
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
    
}