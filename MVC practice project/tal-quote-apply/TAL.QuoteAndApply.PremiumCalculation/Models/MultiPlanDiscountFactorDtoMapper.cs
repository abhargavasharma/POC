using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class MultiPlanDiscountFactorDtoMapper : DbItemClassMapper<MultiPlanDiscountFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<MultiPlanDiscountFactorDto> mapper)
        {
            mapper.MapTable("MultiPlanDiscountFactor");
        }
    }
}