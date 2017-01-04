using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationDefinitionTypeFactorDtoMapper : DbItemClassMapper<OccupationDefinitionTypeFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<OccupationDefinitionTypeFactorDto> mapper)
        {
            mapper.MapTable("OccupationDefinitionTypeFactor");
            mapper.MapProperty(cbr => cbr.OccupationDefinitionType, "OccupationDefinitionTypeId");
        }
    }
}