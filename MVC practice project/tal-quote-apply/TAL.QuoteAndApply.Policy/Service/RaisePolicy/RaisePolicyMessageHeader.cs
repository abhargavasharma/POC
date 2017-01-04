using System;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public class RaisePolicyMessageHeader
    {
        public string ACORDStandardVersionCode { get; set; }
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}