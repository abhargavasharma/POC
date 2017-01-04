using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class IncreasingClaimsFactorDtoMapper : DbItemClassMapper<IncreasingClaimsFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<IncreasingClaimsFactorDto> mapper)
        {
            mapper.MapTable("IncreasingClaimsFactor");
        }
    }
}