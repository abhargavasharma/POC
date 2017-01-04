using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyLoading
    {
        public ReadOnlyLoading(Loading loading)
        {
            Name = loading.Name;
            LoadingType = loading.LoadingType;
            Amount = loading.Amount;
        }

        public string Name { get; private set; }
        public LoadingType LoadingType { get; private set; }
        public decimal Amount { get; private set; }
    }
}
