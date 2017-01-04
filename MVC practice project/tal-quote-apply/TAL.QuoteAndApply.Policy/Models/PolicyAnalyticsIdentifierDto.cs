using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PolicyAnalyticsIdentifierDto : DbItem
    {
        public int PolicyId { get; set; }
        public string SitecoreContactId { get; set; }
    }
}