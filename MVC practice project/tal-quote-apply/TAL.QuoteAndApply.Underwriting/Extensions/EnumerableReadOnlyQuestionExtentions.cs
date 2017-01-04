using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.Underwriting.Extensions
{
    public static class EnumerableReadOnlyQuestionExtentions
    {
        public static ReadOnlyQuestion SingleOrDefaultById(this IEnumerable<ReadOnlyQuestion> questions, string id)
        {
            return questions.SingleOrDefault(q => q.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static ReadOnlyQuestion SingleOrDefaultByTag(this IEnumerable<ReadOnlyQuestion> questions, string tagName)
        {
            return questions.SingleOrDefault(q => q.Tags.Any(tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<ReadOnlyQuestion> ContainsTag(this IEnumerable<ReadOnlyQuestion> questions, string tagName)
        {
            return questions.Where(q => q.Tags.Any(tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
