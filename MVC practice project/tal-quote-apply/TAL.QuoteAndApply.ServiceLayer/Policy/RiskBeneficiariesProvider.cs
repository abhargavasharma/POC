using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public class RiskBeneficiariesProvider
    {
        public IEnumerable<RiskBeneficiaryDetailsParam> GetAllFor(string quoteReferenceNumber, int riskId)
        {
            // TODO: Fill in the code
            return new List<RiskBeneficiaryDetailsParam>();
        }
    }
}