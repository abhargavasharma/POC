using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Models
{
    public class UnderwritingAnswerSearchResult
    {
        public string ResponseId { get; private set; }
        public string Text { get; private set; }
        public string HelpText { get; private set; }
        public string ParentResponseId { get; private set; }
        public string ParentText { get; private set; }
        public float Score { get; private set; }

        public static UnderwritingAnswerSearchResult From(AnswerSearchItemDto answer)
        {
            return new UnderwritingAnswerSearchResult
            {
                ResponseId = answer.ResponseId,
                Text = answer.Text,
                HelpText = answer.HelpText,
                ParentResponseId = answer.ParentId
            };
        }

        public static UnderwritingAnswerSearchResult From(AnswerSearchItemDto answer, AnswerSearchItemDto parentAnswer, float score)
        {
            return new UnderwritingAnswerSearchResult
            {
                ResponseId = answer.ResponseId,
                Text = answer.Text,
                HelpText = answer.HelpText,
                ParentResponseId = parentAnswer.ResponseId,
                ParentText = parentAnswer.Text,
                Score = score
            };
        }

    }
}