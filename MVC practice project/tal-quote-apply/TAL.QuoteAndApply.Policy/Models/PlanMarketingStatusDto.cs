using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PlanMarketingStatusDto : DbItem, IPlanMarketingStatus
    {
        public int PlanId { get; set; }
        public MarketingStatus MarketingStatusId { get; set; }
    }
}
