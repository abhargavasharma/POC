using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Concurrency;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class UpdatedUnderwritingInterview : IConcurrencyToken
    {
        public List<UpdatedInterviewBenefitResponse> BenefitResponses { get; set; }

        public bool Completed { get; set; }

        public string ConcurrencyToken { get; set; }
    }
}