using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingAnswerQuestionResult
    {        
        public IEnumerable<UnderwritingQuestion> AddedQuestions { get; private set; }        
        public IEnumerable<string> RemovedQuestionIds { get; private set; }
        public IEnumerable<UnderwritingChangedQuestion> ChangedQuestions { get; private set; }

        public UnderwritingAnswerQuestionResult(IEnumerable<UnderwritingQuestion> addedQuestions,
            IEnumerable<string> removedQuestionIds, IEnumerable<UnderwritingChangedQuestion> changedQuestions)
        {
            AddedQuestions = addedQuestions;
            RemovedQuestionIds = removedQuestionIds;
            ChangedQuestions = changedQuestions;
        }
    }
}
