using System.Collections.Generic;
using System.Diagnostics;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    [DebuggerDisplay("{Id} {Text}")]
    public class QuestionResponse
    {
        public bool IsAnswered { get; set; }        
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string QuestionType { get; set; }
        public string Text { get; set; }
        public string HelpText { get; set; }
        public string Category { get; set; }
        public IList<AnswerResponse> Answers { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public int OrderId { get; set; }
    }
}