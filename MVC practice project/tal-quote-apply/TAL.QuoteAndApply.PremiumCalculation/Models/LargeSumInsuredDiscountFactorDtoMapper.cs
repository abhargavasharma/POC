using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class LargeSumInsuredDiscountFactorDtoMapper : DbItemClassMapper<LargeSumInsuredDiscountFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<LargeSumInsuredDiscountFactorDto> mapper)
        {
            mapper.MapTable("LargeSumInsuredDiscountFactor");
        }
    }
}