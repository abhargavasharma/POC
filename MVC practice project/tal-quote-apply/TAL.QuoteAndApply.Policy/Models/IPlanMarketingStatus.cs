using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IPlanMarketingStatus
    {
        int PlanId { get; set; }
        MarketingStatus MarketingStatusId { get; set; }
    }
}
