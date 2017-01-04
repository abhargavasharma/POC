using System.Collections.Generic;
using System.Linq;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class AnswerSubmission
    {
        public string Text;

        public string ResponseId;
    }

    public class AnswerQuestionRequest
    {
        public readonly string QuestionId;

        public readonly List<AnswerSubmission> Responses;

        public readonly string CreatedBy;

        public AnswerQuestionRequest(string questionId, IEnumerable<AnswerSubmission> responses, string createdBy)
        {
            QuestionId = questionId;
            CreatedBy = createdBy;
            Responses = responses.ToList();
        }
    }
}
