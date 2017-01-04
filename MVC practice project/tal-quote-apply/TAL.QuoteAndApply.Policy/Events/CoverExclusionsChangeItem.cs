using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public class CoverExclusionsChangeItem : ICoverExclusion
    {
        public CoverExclusionsChangeItem(int coverId)
        {
            CoverId = coverId;
        }

        public int CoverId { get; }
        public string Name { get; }
        public string Text { get; }
    }
}