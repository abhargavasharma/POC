using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PolicyDetailsResponse
    {
        public string QuoteReference { get; set; }
        public bool HasSaved { get; set; }
        public PolicySource Source { get; set; }
    }
}