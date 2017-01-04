using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverDivisionalFactorDtoMapper : DbItemClassMapper<CoverDivisionalFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverDivisionalFactorDto> mapper)
        {
            mapper.MapTable("CoverDivisionalFactor");
        }
    }
}