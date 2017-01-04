using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingQuestionAnswer
    {
        public string QuestionId { get; private set; }
        public IEnumerable<UnderwritingAnswer> Answers { get; private set; }

        public UnderwritingQuestionAnswer(string questionId, IEnumerable<UnderwritingAnswer> answers)
        {
            QuestionId = questionId;
            Answers = answers;
        }
    }
}