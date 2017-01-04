using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyChangedAnswer
    {
        public ReadOnlyChangedAnswer(ChangedAnswer answer)
        {
            ResponseId = answer.ResponseId;
            Selected = answer.Selected;
            SelectedId = answer.SelectedId;
            SelectedText = answer.SelectedText;
        }

        public string ResponseId { get; private set; }
        public bool Selected { get; private set; }
        public string SelectedId { get; private set; }
        public ValueContainer<string> SelectedText { get; private set; }
    }
}