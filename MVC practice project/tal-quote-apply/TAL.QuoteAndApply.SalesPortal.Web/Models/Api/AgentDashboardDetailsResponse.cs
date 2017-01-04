using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class AgentDashboardDetailsResponse
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int TotalRecords { get; set; }
        public List<AgentDashboardQuoteResult> Quotes { get; set; }
    }

    public class AgentDashboardQuoteResult
    {
        public string QuoteReference { get; set; }
        public int PolicyId { get; set; }
        public decimal Premium { get; set; }
        public string FullName { get; set; }
        public string Plans { get; set; }
        public DateTime LastTouchedByTS { get; set; }
        public string Progress { get; set; }
        public DateTime? ProgressUpdateTs { get; set; }
        public string ExternalCustomerReference { get; set; }
        public string Brand { get; set; }
    }
}