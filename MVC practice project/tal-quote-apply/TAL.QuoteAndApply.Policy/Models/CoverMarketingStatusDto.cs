using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class CoverMarketingStatusDto : DbItem, ICoverMarketingStatus
    {
        public int CoverId { get; set; }
        public MarketingStatus MarketingStatusId { get; set; }
    }
}
