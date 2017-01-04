using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    public class AnswerResponse
    {
        public string Id { get; private set; }
        public string SelectedId { get; private set; }
        public bool IsSelected { get; private set; }
        public string Text { get; private set; }
        public string HelpText { get; set; }
        
        public IEnumerable<string> Tags { get; private set; }

        public UnderwritingAnswerType AnswerType { get; private set; }
        
        public AnswerResponse(
            string id, 
            string selectedId, 
            bool isSelected, 
            string text, 
            IEnumerable<string> tags, 
            UnderwritingAnswerType answerType,
            string helpText)
        {
            Id = id;
            SelectedId = selectedId;
            IsSelected = isSelected;
            Text = text;
            Tags = tags;
            AnswerType = answerType;
            HelpText = helpText;
        }
    }
}