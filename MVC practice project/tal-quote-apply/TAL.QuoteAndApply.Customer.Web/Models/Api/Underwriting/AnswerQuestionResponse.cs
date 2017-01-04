using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    public class AnswerQuestionResponse
    {
        public AnswerQuestionResponse(IEnumerable<QuestionResponse> addedQuestions,
            IEnumerable<string> removedQuestionIds,
            IEnumerable<ChangedQuestionResponse> changedQuestions)
        {
            AddedQuestions = addedQuestions;
            RemovedQuestionIds = removedQuestionIds;
            ChangedQuestions = changedQuestions;
        }

        public IEnumerable<QuestionResponse> AddedQuestions { get; private set; }
        public IEnumerable<string> RemovedQuestionIds { get; private set; }
        public IEnumerable<ChangedQuestionResponse> ChangedQuestions { get; private set; }
    }
}