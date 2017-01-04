using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class InterviewBenefitResponse
    {
        public InterviewBenefitResponse()
        {
            UnansweredQuestions = new List<Question>();
            AnsweredQuestions = new List<Question>();
            Categories = new List<Category>();
        }
        public string BenefitCode { get; set; }

        public bool IncludedInInterview { get; set; }

        public UnderwritingStatus Status { get; set; }

        public TotalLoadings TotalLoadings { get; set; }

        public List<Category> Categories { get; set; }

        public List<Question> UnansweredQuestions { get; set; }

        public List<Question> AnsweredQuestions { get; set; }

        public List<Exclusion> Exclusions { get; set; } 

        public Dictionary<string, string> Variables { get; set; }

        public bool HasUnansweredQuestions { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int OrderId { get; set; }
    }
}