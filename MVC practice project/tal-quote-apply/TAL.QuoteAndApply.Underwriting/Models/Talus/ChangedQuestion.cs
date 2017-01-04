using System.Collections.Generic;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class ChangedQuestion
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public ValueContainer<string> Statement { get; set; }
        public ValueContainer<string> HelpText { get; set; }

        public ValueContainer<IReadOnlyList<ChangedAnswer>> ChangedAnswers { get; set; }
        public ValueContainer<IReadOnlyList<string>> Tags { get; set; }
    }
}
