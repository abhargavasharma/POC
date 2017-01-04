using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyUnderwritingInterview
    {
        public string InterviewIdentifier { get; private set; }

        public string TemplateType { get; private set; }

        // for some reason template version is used when you create and templateid is used when getting an interview
        public string TemplateId { get; private set; }

        public string TemplateVersion { get; private set; }

        public bool Completed { get; private set; }
        public string CompletedBy { get; private set; }

        public IReadOnlyList<ReadOnlyInterviewBenefitResponse> Benefits { get; private set; }

        public string ConcurrencyToken { get; set; }

        public ReadOnlyUnderwritingInterview(UnderwritingInterview interview)
        {
            InterviewIdentifier = interview.InterviewIdentifier;
            TemplateType = interview.TemplateType;
            TemplateId = interview.TemplateId;
            TemplateVersion = interview.ActualTemplateVersion;
            Completed = interview.Completed;
            CompletedBy = interview.CompletedBy;
            Benefits = interview.Benefits.With(br => new ReadOnlyInterviewBenefitResponse(br)).Return(list => list.ToList(), null);
            ConcurrencyToken = interview.ConcurrencyToken;
        }

        public IEnumerable<ReadOnlyQuestion> AllQuestions
        {
            get
            {
                return Benefits.SelectMany(b => b.AnsweredQuestions)
                        .Concat(Benefits.SelectMany(b => b.UnansweredQuestions))
                        .DistinctBy(q => q.Id);
            }
        } 
        
    }
}