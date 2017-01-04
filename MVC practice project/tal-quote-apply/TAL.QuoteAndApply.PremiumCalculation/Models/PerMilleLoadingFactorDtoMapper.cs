using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PerMilleLoadingFactorDtoMapper : DbItemClassMapper<PerMilleLoadingFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PerMilleLoadingFactorDto> mapper)
        {
            mapper.MapTable("PerMilleLoadingFactor");
        }
    }
}