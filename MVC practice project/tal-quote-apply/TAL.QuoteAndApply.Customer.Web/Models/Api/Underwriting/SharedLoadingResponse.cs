using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    public class SharedLoadingResponse
    {
        public SharedLoadingResponse(string name, decimal premiumDiff, IEnumerable<string> applicablePlanCodes)
        {
            Name = name;
            PremiumDiff = premiumDiff;
            ApplicablePlanCodes = applicablePlanCodes;
        }

        public string Name { get; }
        public decimal PremiumDiff { get; }
        public IEnumerable<string> ApplicablePlanCodes { get; }
        public bool ZeroLoadingCalcuation => PremiumDiff == 0;
    }
}