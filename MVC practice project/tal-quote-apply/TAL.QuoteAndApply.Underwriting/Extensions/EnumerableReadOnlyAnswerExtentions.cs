using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.Underwriting.Extensions
{
    public static class EnumerableReadOnlyAnswerExtentions
    {
        public static ReadOnlyAnswer SingleOrDefaultByResponseId(this IEnumerable<ReadOnlyAnswer> answers, string responseId)
        {
            return answers.SingleOrDefault(a => a.ResponseId.Equals(responseId, StringComparison.OrdinalIgnoreCase));
        }
        public static ReadOnlyAnswer SingleOrDefaultByText(this IEnumerable<ReadOnlyAnswer> answers, string answerText)
        {
            return answers.SingleOrDefault(a => a.Text.Equals(answerText, StringComparison.OrdinalIgnoreCase));
        }

        public static ReadOnlyAnswer SingleOrDefaultByTag(this IEnumerable<ReadOnlyAnswer> answers, string responseId)
        {
            return answers.SingleOrDefault(a => a.ResponseId.Equals(responseId, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<ReadOnlyAnswer> ContainsTag(this IEnumerable<ReadOnlyAnswer> answers, string tagName)
        {
            return answers.Where(a => a.Tags.Any(tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<ReadOnlyAnswer> Selected(this IEnumerable<ReadOnlyAnswer> answers)
        {
            return answers.Where(a => a.Selected);
        }

        public static ReadOnlyAnswer SingleOrDefaultSelected(this IEnumerable<ReadOnlyAnswer> answers)
        {
            return answers.SingleOrDefault(a => a.Selected);
        }
    }
}