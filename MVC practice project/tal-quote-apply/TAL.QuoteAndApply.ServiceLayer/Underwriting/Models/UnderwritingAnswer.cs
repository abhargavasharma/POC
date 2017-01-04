using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingAnswer
    {
        public string Id { get; private set; }
        public string SelectedId { get; private set; }
        public string Text { get; private set; }
        public string HelpText { get; set; }
        public bool IsSelected { get; private set; }
        public IEnumerable<string> Tags { get; private set; }
        public UnderwritingAnswerType AnswerType { get; private set; }
        public IEnumerable<UnderwritingLoading> Loadings { get; private set; }
        public IEnumerable<UnderwritingExclusion> Exclusions { get; private set; }
        
        public UnderwritingAnswer(
            string id, 
            string text, 
            string selectedId = "", 
            bool isSelected = false, 
            IEnumerable<string> tags = null, 
            UnderwritingAnswerType answerType = UnderwritingAnswerType.Default,
            string helpText = "",
            IEnumerable<UnderwritingLoading> loadings = null,
            IEnumerable<UnderwritingExclusion> exclusions = null)
        {
            Id = id;
            Text = text;
            SelectedId = selectedId;
            IsSelected = isSelected;
            Tags = tags?? new List<string>();
            AnswerType = answerType;
            HelpText = helpText;
            Loadings = loadings;
            Exclusions = exclusions;
        }
        
    }
}
