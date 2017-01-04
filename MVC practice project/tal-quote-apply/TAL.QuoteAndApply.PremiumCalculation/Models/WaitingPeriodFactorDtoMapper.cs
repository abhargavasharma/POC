using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class WaitingPeriodFactorDtoMapper : DbItemClassMapper<WaitingPeriodFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<WaitingPeriodFactorDto> mapper)
        {
            mapper.MapTable("WaitingPeriodFactor");
        }
    }
}