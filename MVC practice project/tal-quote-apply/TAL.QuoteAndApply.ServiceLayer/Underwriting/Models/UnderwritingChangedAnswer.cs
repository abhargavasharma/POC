using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingChangedAnswer
    {
        public UnderwritingChangedAnswer(string id, string selectedId, bool isSelected)
        {
            Id = id;
            SelectedId = selectedId;
            IsSelected = isSelected;
        }

        public string Id { get; private set; }
        public string SelectedId { get; private set; }
        public bool IsSelected { get; private set; }

    }
}
