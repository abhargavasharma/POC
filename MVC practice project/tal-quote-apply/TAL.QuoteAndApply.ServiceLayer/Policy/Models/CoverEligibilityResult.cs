using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class CoverEligibilityResult
    {
        public string CoverCode { get; private set; }
        public bool EligibleForCover { get; private set; }
        public IEnumerable<string> IneligibleReasons { get; private set; }

        private CoverEligibilityResult(string coverCode, bool eligibleForCover, IEnumerable<string> ineligibleReasons)
        {
            CoverCode = coverCode;
            EligibleForCover = eligibleForCover;
            IneligibleReasons = ineligibleReasons;
        }

        public static CoverEligibilityResult Eligible(string code)
        {
            return new CoverEligibilityResult(code, true, new string[0]);
        }
        public static CoverEligibilityResult Ineligible(string code, params string[] reasons)
        {
            return new CoverEligibilityResult(code, false, reasons);
        }
    }
}
