using System;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public class RaisePolicyPolicyOrderDetails
    {
        public string DocumentId { get; set; }
        public string TransactionFunctionCode { get; set; }
        public string ApplicationTypeCode { get; set; }
        public string Description { get; set; }
        public string CaseId { get; set; }
        public string BroadLineOfBusinessCode { get; set; }
        public string LodgementTypeCode { get; set; }
    }
}