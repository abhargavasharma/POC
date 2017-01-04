using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IReferralDetailsResultConverter
    {
        ReferralsDetailsResponse From(List<ReferralDetailsResult> referralsDetailsResult, List<string> underwriters);
        ReferralDetailsResponse From(ReferralDetailsResult referralDetailsResult);
    }

    public class ReferralDetailsResultConverter : IReferralDetailsResultConverter
    {
        public ReferralsDetailsResponse From(List<ReferralDetailsResult> referralsDetailsResult, List<string> underwriters)
        {
            return new ReferralsDetailsResponse()
            {
                Underwriters = underwriters.Select(u => u).ToList(),
                Referrals = referralsDetailsResult.Select(From).ToList()
            };
        }

        public ReferralDetailsResponse From(ReferralDetailsResult referralDetailsResult)
        {
            return new ReferralDetailsResponse()
            {
                QuoteReferenceNumber = referralDetailsResult.QuoteReferenceNumber,
                AssignedTo = referralDetailsResult.AssignedTo,
                ClientName = referralDetailsResult.ClientName,
                CreatedBy = referralDetailsResult.CreatedBy,
                CreatedTS = referralDetailsResult.CreatedTS,
                Plans = referralDetailsResult.Plans,
                State = referralDetailsResult.State,
                ExternalCustomerReference = referralDetailsResult.ExternalCustomerReference,
                Brand = referralDetailsResult.Brand
            };
        }
    }
}