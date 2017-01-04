using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IRiskMarketingStatus
    {
        int RiskId { get; set; }
        MarketingStatus MarketingStatusId { get; set; }
    }
}
