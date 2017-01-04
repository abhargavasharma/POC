using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class SmokerFactorDtoMapper : DbItemClassMapper<SmokerFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<SmokerFactorDto> mapper)
        {
            mapper.MapTable("SmokerFactor");
        }
    }
}