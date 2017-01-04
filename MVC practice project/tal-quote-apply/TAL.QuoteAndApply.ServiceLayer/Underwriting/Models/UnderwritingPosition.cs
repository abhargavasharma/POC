using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingPosition
    {
        public string ConcurrencyToken { get; }
        public IEnumerable<UnderwritingQuestion> Questions { get; }
        public IEnumerable<UnderwritingCategory> Categories { get; }

        public UnderwritingPosition(string concurrencyToken, IEnumerable<UnderwritingCategory> categories, IEnumerable<UnderwritingQuestion> questions)
        {
            ConcurrencyToken = concurrencyToken;
            Categories = categories;
            Questions = questions;
        }
    }
}
