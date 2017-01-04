using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationMappingDtoMapper : DbItemClassMapper<OccupationMappingDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<OccupationMappingDto> mapper)
        {
            mapper.MapTable("OccupationMapping");
        }
    }
}