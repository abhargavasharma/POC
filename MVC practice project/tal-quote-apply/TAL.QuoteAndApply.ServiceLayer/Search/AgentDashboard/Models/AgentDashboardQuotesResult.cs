using System;

namespace TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models
{
    public class AgentDashboardQuotesResult
    {
        public string QuoteReference { get; set; }
        public int PolicyId { get; set; }
        public decimal Premium { get; set; }
        public string FullName { get; set; }
        public string Plans { get; set; }
        public DateTime LastTouchedByTS { get; set; }
        public string CreatedBy { get; set; }
        public string Progress { get; set; }
        public DateTime? ProgressUpdateTs { get; set; }
        public int TotalRecords { get; set; }
        public string ExternalCustomerReference { get; set; }
        public string Brand { get; set; }
    }
}
