using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class Loading
    {
        public string Name { get; set; }
        public LoadingType LoadingType { get; set; }
        public decimal Amount { get; set; }
    }
}
