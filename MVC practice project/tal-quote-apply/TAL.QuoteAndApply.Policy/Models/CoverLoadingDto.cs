using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface ICoverLoading
    {
        int CoverId { get; }
        LoadingType LoadingType { get; }
        decimal Loading { get; }
    }

    public class CoverLoadingDto : DbItem, ICoverLoading
    {
        public int CoverId { get; set; }
        public LoadingType LoadingType { get; set; }
        public decimal Loading { get; set; }
    }
}
