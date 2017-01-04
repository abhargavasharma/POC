using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Underwriting
{
    public class ChangedQuestionResponse
    {
        public string Id { get; private set; }
        public ChangedAttributes ChangedAttributes { get; private set; }
        public IEnumerable<ChangedAnswerResponse> ChangedAnswers { get; private set; }
        public string HelpText { get; private set; }
        
        public ChangedQuestionResponse(string id, ChangedAttributes changedAttributes,
            IEnumerable<ChangedAnswerResponse> changedAnswers, string helpText)
        {
            Id = id;
            ChangedAttributes = changedAttributes;
            ChangedAnswers = changedAnswers;
            HelpText = helpText;
        }
    }
}