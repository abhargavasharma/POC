using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyQuestion
    {
        public ReadOnlyQuestion(Question question)
        {
            Id = question.Id;
            ParentId = question.ParentId;
            OrderId = question.OrderId;
            Category = question.Id.Split('?').FirstOrDefault();
            ControlType = question.ControlType;
            Statement = question.Statement;
            HelpText = question.HelpText;
            Answers = question.Answers.With(q => new ReadOnlyAnswer(q)).Return(list => list.ToList(), null);
            Tags = question.Tags.ToList();
        }

        public string Id { get; private set; }

        public string ParentId { get; private set; }

        public int OrderId { get; private set; }

        public string Category { get; private set; }

        public ControlType ControlType { get; private set; }

        public string Statement { get; private set; }

        public string HelpText { get; private set; }

        public IReadOnlyList<ReadOnlyAnswer> Answers { get; private set; }

        public IReadOnlyList<string> Tags { get; private set; }
    }
}