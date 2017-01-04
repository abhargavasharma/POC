using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Models
{
    public class RiskUnderwritingCompleteResult
    {
        public bool IsUnderwritingCompleteForRisk { get; private set; }
        public ValidationType ValidationTypeForRisk { get; private set; }
        public IEnumerable<CoverUnderwritingCompleteResult> CoverUnderwritingCompleteResults { get; private set; }

        public RiskUnderwritingCompleteResult(bool isUnderwritingCompleteForRisk, ValidationType validationTypeForRisk, IEnumerable<CoverUnderwritingCompleteResult> coverUnderwritingCompleteResults)
        {
            IsUnderwritingCompleteForRisk = isUnderwritingCompleteForRisk;
            ValidationTypeForRisk = validationTypeForRisk;
            CoverUnderwritingCompleteResults = coverUnderwritingCompleteResults;
        }
    }
}