using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingQuestionsByBenefitCode
    {
        public UnderwritingQuestionsByBenefitCode(string benefitCode, IEnumerable<UnderwritingQuestion> questions)
        {
            BenefitCode = benefitCode;
            Questions = questions;
        }

        public string BenefitCode { get; private set; }
        public IEnumerable<UnderwritingQuestion> Questions { get; private set; }
    }
}