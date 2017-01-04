using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class ReferralsDetailsResponse
    {
        public List<ReferralDetailsResponse> Referrals { get; set; }
        public List<string> Underwriters { get; set; }
    }

    public class ReferralDetailsResponse
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