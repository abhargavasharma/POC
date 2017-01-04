using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class BrandDto: DbItem
    {
        public string ProductKey { get; set; }
        public string Description { get; set; }
    }
}
