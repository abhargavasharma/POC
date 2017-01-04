using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class CreateCoverParam
    {
        public string QuoteNumber { get; set; }
        public bool Selected { get; set; }
        public int PolicyId { get; private set; }
        public int RiskId { get; private set; }
        public int PlanId { get; private set; }
        public string Code { get; private set; }

        public CreateCoverParam(string quoteNumber, int policyId, int riskId, bool selected, int planId, string code)
        {
            QuoteNumber = quoteNumber;
            PolicyId = policyId;
            RiskId = riskId;
            Selected = selected;
            PlanId = planId;
            Code = code;
        }
    }
}
