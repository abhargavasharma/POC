using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class MultiCoverDiscountFactorDtoMapper : DbItemClassMapper<MultiCoverDiscountFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<MultiCoverDiscountFactorDto> mapper)
        {
            mapper.MapTable("MultiCoverDiscountFactor");
        }
    }
}