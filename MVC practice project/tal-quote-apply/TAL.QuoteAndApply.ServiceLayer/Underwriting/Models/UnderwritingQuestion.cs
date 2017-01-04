using System.Collections.Generic;
using System.Linq;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingQuestion
    {
        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Text { get; private set; }
        public string HelpText { get; private set; }
        public string Category { get; private set; }
        public IEnumerable<UnderwritingAnswer> Answers { get; private set; }
        public UnderwritingQuestionType QuestionType { get; private set; }
        public IEnumerable<string> Tags { get; private set; }
        public int OrderId { get; set; }

        public bool IsAnswered
        {
            get { return Answers.Any(a => a.IsSelected); }
        }

        public UnderwritingQuestion(UnderwritingQuestionType questionType, string id, string parentId, string text,
            IEnumerable<UnderwritingAnswer> answers, IEnumerable<string> tags, string helpText, string category, int orderId)
        {
            Id = id;
            Text = text;
            Answers = answers;
            ParentId = parentId;
            QuestionType = questionType;
            Tags = tags;
            HelpText = helpText;
            Category = category;
            OrderId = orderId;
        }
    }
}
