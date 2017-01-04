using System;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IAgentDashboardSearchDto
    {
        int PolicyId { get; }
        string QuoteReference { get; }
        decimal Premium { get; }
        DateTime LastTouchedByTS { get; }
        DateTime LastPipelineStatusTS { get; }
        string LastPipelineStatusDescription { get; }
        string OwnerFullName { get; }
        int OwnerPartyId { get; }
        string PlanCodes { get; }
        int TotalRecords { get; }
        string OwnerExternalCustomerReference { get; }
        int BrandId { get; }
    }

    public class AgentDashboardSearchDto : DbItem, IAgentDashboardSearchDto
    {
        public int PolicyId { get; set; }
        public string QuoteReference { get; set; }
        public decimal Premium { get; set; }
        public DateTime LastTouchedByTS { get; set; }
        public DateTime LastPipelineStatusTS { get; set; }
        public string LastPipelineStatusDescription { get; set; }
        public string OwnerFullName { get; set; }
        public int OwnerPartyId { get; set; }
        public string PlanCodes { get; set; }
        public int TotalRecords { get; set; }
        public string OwnerExternalCustomerReference { get; set; }
        public int BrandId { get; set; }
    }
}
