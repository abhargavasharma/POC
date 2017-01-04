using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class RiskMarketingStatusDto : DbItem, IRiskMarketingStatus
    {
        public int RiskId { get; set; }
        public MarketingStatus MarketingStatusId { get; set; }
    }
}
