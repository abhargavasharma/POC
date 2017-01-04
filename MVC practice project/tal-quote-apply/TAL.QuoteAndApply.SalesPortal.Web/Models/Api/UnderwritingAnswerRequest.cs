using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class UnderwritingQuestionAnswerRequest
    {
        public string QuestionId { get; set; }
        public string ConcurrencyToken { get; set; }
        public IEnumerable<UnderwritingAnswerRequest> Answers { get; set; }
    }

    public class UnderwritingAnswerRequest
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}