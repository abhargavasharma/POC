using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class UpdatedInterviewBenefitResponse
    {
        public UpdatedInterviewBenefitResponse()
        {
            AddedQuestions = new List<Question>();
            ChangedQuestions = new List<ChangedQuestion>();
            RemovedQuestions = new List<RemovedQuestion>();
        }
        public string BenefitCode { get; set; }

        public bool IncludedInInterview { get; set; }

        public UnderwritingStatus Status { get; set; }

        public TotalLoadings TotalLoadings { get; set; }

        public TotalLoadings DeltaTotalLoadings { get; set; }

        public List<Question> AddedQuestions { get; set; }

        public List<RemovedQuestion> RemovedQuestions { get; set; }

        public List<ChangedQuestion> ChangedQuestions { get; set; }

        public List<Exclusion> Exclusions { get; set; }

        public Dictionary<string, string> Variables { get; set; }

        public bool HasUnansweredQuestions { get; set; }
    }
}