using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationDefinitionTypeFactorDto : DbItem
    {
        public OccupationDefinition OccupationDefinitionType { get; set; }
        public int BrandId { get; set; }
        public decimal Factor { get; set; }
    }
}
