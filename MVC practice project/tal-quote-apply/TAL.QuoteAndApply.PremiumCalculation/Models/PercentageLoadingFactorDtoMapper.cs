using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PercentageLoadingFactorDtoMapper : DbItemClassMapper<PercentageLoadingFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PercentageLoadingFactorDto> mapper)
        {
            mapper.MapTable("PercentageLoadingFactor");
        }
    }
}