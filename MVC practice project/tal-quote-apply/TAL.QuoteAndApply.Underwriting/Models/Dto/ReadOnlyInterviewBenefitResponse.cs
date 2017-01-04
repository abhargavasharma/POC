using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyInterviewBenefitResponse
    {
        public ReadOnlyInterviewBenefitResponse(InterviewBenefitResponse benefitResponse)
        {
            UnansweredQuestions =
                benefitResponse.UnansweredQuestions.With(q => new ReadOnlyQuestion(q)).Return(list => list.ToList(), null);
            AnsweredQuestions =
                benefitResponse.AnsweredQuestions.With(q => new ReadOnlyQuestion(q)).Return(list => list.ToList(), null);
            Categories = benefitResponse.Categories.With(c => new ReadOnlyCategory(c)).Return(list => list.ToList(), null);
            Exclusions = benefitResponse.Exclusions.With(e => new ReadOnlyExclusion(e)).Return(list => list.ToList(), null);
            BenefitCode = benefitResponse.BenefitCode;
            IncludedInInterview = benefitResponse.IncludedInInterview;
            Status = benefitResponse.Status;
            TotalLoadings = new ReadOnlyTotalLoadings(benefitResponse.TotalLoadings);
            Variables = benefitResponse.Variables;
            HasUnansweredQuestions = benefitResponse.HasUnansweredQuestions;

        }
        public string BenefitCode { get; private set; }

        public bool IncludedInInterview { get; private set; }

        public UnderwritingStatus Status { get; private set; }

        public ReadOnlyTotalLoadings TotalLoadings { get; private set; }

        public IReadOnlyList<ReadOnlyCategory> Categories { get; private set; }

        public IReadOnlyList<ReadOnlyQuestion> UnansweredQuestions { get; private set; }

        public IReadOnlyList<ReadOnlyQuestion> AnsweredQuestions { get; private set; }

        public IReadOnlyList<ReadOnlyExclusion> Exclusions { get; private set; }

        public IReadOnlyDictionary<string, string> Variables { get; private set; }

        public bool HasUnansweredQuestions { get; private set; }
    }
}