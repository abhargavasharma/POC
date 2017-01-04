using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyProgressParam
    {
        public string QuoteReferenceNumber { get; set; }
        public PolicyProgress Progress { get; set; }
    }
}