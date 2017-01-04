using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyChangedQuestion
    {
        public ReadOnlyChangedQuestion(ChangedQuestion question)
        {
            Id = question.Id;
            ParentId = question.ParentId;
            Statement = question.Statement;
            HelpText = question.HelpText;
            ChangedAnswers = new ValueContainer<IReadOnlyList<ReadOnlyChangedAnswer>>(
                question.ChangedAnswers.With(cal => cal.Value)
                    .With(cav => new ReadOnlyChangedAnswer(cav))
                    .Return(list => list.ToList(), null)
                );
            Tags = question.Tags;
        }

        public string Id { get; private set; }

        public string ParentId { get; private set; }

        public ValueContainer<string> Statement { get; private set; }
        public ValueContainer<string> HelpText { get; private set; }

        public ValueContainer<IReadOnlyList<ReadOnlyChangedAnswer>> ChangedAnswers { get; set; }
        public ValueContainer<IReadOnlyList<string>> Tags { get; set; }
    }
}