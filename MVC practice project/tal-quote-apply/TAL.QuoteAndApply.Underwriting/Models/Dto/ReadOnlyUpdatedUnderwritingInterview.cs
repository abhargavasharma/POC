using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyUpdatedUnderwritingInterview
    {
        public ReadOnlyUpdatedUnderwritingInterview(UpdatedUnderwritingInterview updatedInterview)
        {
            BenefitResponses =
                updatedInterview.BenefitResponses.With(b => new ReadOnlyUpdatedInterviewBenefitResponse(b))
                    .Return(list => list.ToList(), null);
            Completed = updatedInterview.Completed;
            ConcurrencyToken = updatedInterview.ConcurrencyToken;
        }

        public IReadOnlyList<ReadOnlyUpdatedInterviewBenefitResponse> BenefitResponses { get; private set; }

        public bool Completed { get; private set; }

        public string ConcurrencyToken { get; private set; }

        public IEnumerable<ReadOnlyQuestion> AllAddedQuestions
        {
            get
            {
                return BenefitResponses.SelectMany(b => b.AddedQuestions)
                        .DistinctBy(q => q.Id);
            }
        }

        public IEnumerable<ReadOnlyChangedQuestion> AllChangedQuestions
        {
            get
            {
                return BenefitResponses.SelectMany(b => b.ChangedQuestions)
                        .DistinctBy(q => q.Id);
            }
        }
    }
}
