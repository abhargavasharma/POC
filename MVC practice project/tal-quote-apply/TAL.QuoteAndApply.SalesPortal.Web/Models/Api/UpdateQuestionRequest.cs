using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class UpdateQuestionRequest
    {
        public string QuestionId { get; set; }
        public string QuestionType { get; set; }
        public IEnumerable<UpdateQuestionAnswerRequest> SelectedAnswers { get; set; }
    }

    public class UpdateQuestionAnswerRequest
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}