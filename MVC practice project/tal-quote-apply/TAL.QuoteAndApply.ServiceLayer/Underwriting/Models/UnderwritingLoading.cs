using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class UnderwritingLoading
    {
        public UnderwritingLoading(string name, LoadingType loadingType, decimal amount)
        {
            Name = name;
            LoadingType = loadingType;
            Amount = amount;
        }

        public string Name { get; private set; }
        public LoadingType LoadingType { get; private set; }
        public decimal Amount { get; private set; }
    }
}
