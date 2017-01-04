using System;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IAgentDashboardSearchRequest
    {
        DateTime DateTimeFrom { get; }
        DateTime DateTimeTo { get; }
        bool InProgressPreUw { get; }
        bool InProgressUwReferral { get; }
        bool InProgressRecommendation { get; }
        bool InProgressCantContact { get; }
        bool ClosedSale { get;}
        bool ClosedNoSale { get; }
        bool ClosedTriage { get; }
        bool ClosedCantContact { get; }
        bool Unknown { get; }
        string User { get; }
        int PageSize { get; }
        int PageNumber { get; }
        int BrandId { get; }
    }

    public class AgentDashboardSearchRequest : IAgentDashboardSearchRequest
    {
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        public bool InProgressPreUw { get; set; }
        public bool InProgressUwReferral { get; set; }
        public bool InProgressRecommendation { get; set; }
        public bool InProgressCantContact { get; set; }
        public bool ClosedSale { get; set; }
        public bool ClosedNoSale { get; set; }
        public bool ClosedTriage { get; set; }
        public bool ClosedCantContact { get; set; }
        public bool Unknown { get; set; }
        public string User { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int BrandId { get; set; }
    }
}
