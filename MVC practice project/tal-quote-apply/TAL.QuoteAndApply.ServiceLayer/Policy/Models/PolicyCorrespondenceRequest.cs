using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Notifications.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyCorrespondenceRequest
    {
        public string ClientEmailAddress { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserFullName { get; set; }
        public string ClientFirstName { get; set; }
        public bool IsValidForEmailCorrespondence { get; set; }
        public List<PlanCorrespondenceSummary> PlanSummaries { get; set; }
        public string PremiumFrequency { get; set; }
        public string BrandKey { get; set; }
    }

    public class PlanCorrespondenceSummary
    {
        public string Name { get; set; }
        public long CoverAmount { get; set; }
        public decimal Premium { get; set; }
        public bool Selected { get; set; }
        public bool IsRider { get; set; }
        public string ParentName { get; set; }
        public List<CoverCorrespondenceSummary> CoverCorrespondenceSummaries { get; set; }
    }

    public class CoverCorrespondenceSummary
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
