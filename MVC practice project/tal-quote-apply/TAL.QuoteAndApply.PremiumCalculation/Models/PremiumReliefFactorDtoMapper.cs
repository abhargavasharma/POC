using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumReliefFactorDtoMapper : DbItemClassMapper<PremiumReliefFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PremiumReliefFactorDto> mapper)
        {
            mapper.MapTable("PremiumReliefFactor");
        }
    }
}