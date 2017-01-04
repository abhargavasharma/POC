using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    public class UpdateQuestionRequest
    {
        public string QuestionId { get; set; }
        public string QuestionType { get; set; }
        public IEnumerable<UpdateQuestionAnswerRequest> SelectedAnswers { get; set; }
    }
}