using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class RaisePolicySubmissionAuditDto : DbItem
    {
        public int PolicyId { get; set; }
        public string RaisePolicyXml { get; set; }
        public RaisePolicyAuditType RaisePolicyAuditType { get; set; }
    }
}
