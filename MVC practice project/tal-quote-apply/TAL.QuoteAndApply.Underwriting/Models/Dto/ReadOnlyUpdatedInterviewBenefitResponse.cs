using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyUpdatedInterviewBenefitResponse
    {
        public ReadOnlyUpdatedInterviewBenefitResponse(UpdatedInterviewBenefitResponse response)
        {
            BenefitCode = response.BenefitCode;
            IncludedInInterview = response.IncludedInInterview;
            Status = response.Status;
            TotalLoadings = response.TotalLoadings.With(tl => new ReadOnlyTotalLoadings(tl));
            DeltaTotalLoadings = response.DeltaTotalLoadings.With(tl => new ReadOnlyTotalLoadings(tl));
            AddedQuestions = response.AddedQuestions.With(q => new ReadOnlyQuestion(q)).Return(list => list.ToList(), null);
            RemovedQuestions = response.RemovedQuestions.With(q => new ReadOnlyRemovedQuestion(q)).Return(list => list.ToList(), null);
            ChangedQuestions = response.ChangedQuestions.With(q => new ReadOnlyChangedQuestion(q)).Return(list => list.ToList(), null);
            Variables = response.Variables.Return(v => new Dictionary<string, string>(v), null);
            HasUnansweredQuestions = response.HasUnansweredQuestions;
            Exclusions = response.Exclusions.With(e => new ReadOnlyExclusion(e)).Return(list=> list.ToList(), null);
        }

        public string BenefitCode { get; private set; }
        public bool IncludedInInterview { get; private set; }
        public UnderwritingStatus Status { get; private set; }
        public ReadOnlyTotalLoadings TotalLoadings { get; private set; }
        public ReadOnlyTotalLoadings DeltaTotalLoadings { get; private set; }
        public IReadOnlyList<ReadOnlyQuestion> AddedQuestions { get; private set; }
        public IReadOnlyList<ReadOnlyRemovedQuestion> RemovedQuestions { get; private set; }
        public IReadOnlyList<ReadOnlyChangedQuestion> ChangedQuestions { get; private set; }
        public IReadOnlyDictionary<string, string> Variables { get; private set; }
        public IReadOnlyList<ReadOnlyExclusion> Exclusions { get; private set; }
        public bool HasUnansweredQuestions { get; private set; }
    }
}