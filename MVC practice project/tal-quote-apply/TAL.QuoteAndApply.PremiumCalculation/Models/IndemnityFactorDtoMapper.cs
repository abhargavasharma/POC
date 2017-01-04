using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class IndemnityFactorDtoMapper : DbItemClassMapper<IndemnityFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<IndemnityFactorDto> mapper)
        {
            mapper.MapTable("IndemnityFactor");
        }
    }
}