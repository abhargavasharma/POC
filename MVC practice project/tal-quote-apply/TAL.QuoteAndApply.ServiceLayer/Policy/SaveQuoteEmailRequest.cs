using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Notifications.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public class SaveQuoteEmailRequest
    {
        public string ClientEmailAddress { get; set; }
        public string UserEmailAddress { get; set; }
        public string ClientFullName { get; set; }
        public string ClientFirstName { get; set; }
        public bool EmailSent { get; set; }
        public bool IsValidForEmailCorrespondence { get; set; }
        public List<PlanCorrespondenceSummaryViewModel> PlanSummaries { get; set; }
    }

    public class PlanCorrespondenceSummaryViewModel
    {
        public string Name { get; set; }
        public long CoverAmount { get; set; }
        public decimal Premium { get; set; }
        public List<CoverCorrespondenceSummaryViewModel> CoverCorrespondenceSummaries { get; set; }
    }

    public class CoverCorrespondenceSummaryViewModel
    {
        public string Name { get; set; }
    }
}
