using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface ICoverMarketingStatus
    {
        int CoverId { get; set; }
        MarketingStatus MarketingStatusId { get; set; }
    }
}
