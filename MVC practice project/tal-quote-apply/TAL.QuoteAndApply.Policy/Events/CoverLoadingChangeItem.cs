using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public class CoverLoadingChangeItem : ICoverLoading
    {
        public CoverLoadingChangeItem(int coverId)
        {
            CoverId = coverId;
        }

        public int CoverId { get; }
        public LoadingType LoadingType { get; }
        public decimal Loading { get; }
    }
}