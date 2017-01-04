using System;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class ReferralDetailsResult
    {
        public string QuoteReferenceNumber { get; set; }
        public string ClientName { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? CreatedTS { get; set; }
        public string CreatedBy { get; set; }
        public ReferralState State { get; set; }
        public List<string> Plans { get; set; }
        public string ExternalCustomerReference { get; set; }
        public string Brand { get; set; }
    }
}
