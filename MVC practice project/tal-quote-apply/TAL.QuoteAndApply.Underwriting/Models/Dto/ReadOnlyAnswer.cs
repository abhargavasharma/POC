using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyAnswer
    {
        public ReadOnlyAnswer(Answer answer)
        {
            Tags = answer.Tags.With(tags => tags.ToList());
            ResponseId = answer.ResponseId;
            OrderId = answer.OrderId;
            Text = answer.Text;
            Selected = answer.Selected;
            SelectedId = answer.SelectedId;
            SelectedText = answer.SelectedText;
            HelpText = answer.HelpText;
            Loadings = answer.Loadings.With(loading => new ReadOnlyLoading(loading)).Return(list => list.ToList(), new List<ReadOnlyLoading>());
            Exclusions = answer.Exclusions.With(exclusion => new ReadOnlyExclusion(exclusion)).Return(list => list.ToList(), new List<ReadOnlyExclusion>());
        }

        public string ResponseId { get; private set; }

        public int OrderId { get; private set; }

        public string Text { get; private set; }

        public bool Selected { get; private set; }

        public string SelectedId { get; private set; }

        public string SelectedText { get; private set; }

        public string HelpText { get; private set; }

        public IReadOnlyList<string> Tags { get; private set; }

        public IReadOnlyList<ReadOnlyLoading> Loadings { get; private set; }
        public IReadOnlyList<ReadOnlyExclusion> Exclusions { get; private set; }
    }
}