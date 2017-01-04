using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Models
{
    public static class ReadOnlyAnswerExtensions
    {
        public static AnswerSearchItemDto ToAnswerSearchItemDto(this ReadOnlyAnswer answer)
        {
            return new AnswerSearchItemDto
            {
                Tags = string.Join(",", answer.Tags.With(tags => tags.ToList())),
                ResponseId = answer.ResponseId,
                Text = answer.Text,
                HelpText = answer.HelpText
            };
        }

        public static AnswerSearchItemDto ToAnswerSearchItemDto(this ReadOnlyAnswer answer, ReadOnlyAnswer parentAnswer)
        {
            return new AnswerSearchItemDto
            {
                Tags = string.Join(",", answer.Tags.With(tags => tags.ToList())),
                ResponseId = answer.ResponseId,
                Text = answer.Text,
                HelpText = answer.HelpText,
                ParentId = parentAnswer.ResponseId
            };
        }
    }

}
