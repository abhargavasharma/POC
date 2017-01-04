using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingChangedQuestion
    {
        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Text { get; private set; }
        public string HelpText { get; private set; }
        public IEnumerable<UnderwritingChangedAnswer> ChangedAnswers { get; private set; }

        public UnderwritingChangedQuestion(string id, string parentId, string text,
            IEnumerable<UnderwritingChangedAnswer> changedAnswers, string helpText)
        {
            Id = id;
            ChangedAnswers = changedAnswers;
            ParentId = parentId;
            Text = text;
            HelpText = helpText;
        }

    }
}
