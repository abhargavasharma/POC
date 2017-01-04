using System;

namespace TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models
{
    public class AgentDashboardRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public int PageNumber { get; set; }
        public string Brand { get; set; }
    }
}
