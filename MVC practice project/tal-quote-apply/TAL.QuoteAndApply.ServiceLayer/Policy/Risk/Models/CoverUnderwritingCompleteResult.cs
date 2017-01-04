using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Models
{
    public class CoverUnderwritingCompleteResult
    {
        public string CoverCode { get; private set; }
        public bool IsUnderwritingComplete { get; private set; }
        public ValidationType ValidationType { get; private set; }

        public CoverUnderwritingCompleteResult(string coverCode, bool isUnderwritingComplete, ValidationType validationType)
        {
            CoverCode = coverCode;
            IsUnderwritingComplete = isUnderwritingComplete;
            ValidationType = validationType;
        }
    }
}