using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class OccupationClassFactorDtoMapper : DbItemClassMapper<OccupationClassFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<OccupationClassFactorDto> mapper)
        {
            mapper.MapTable("OccupationClassFactor");
            mapper.MapProperty(occ => occ.Gender, "GenderId");
        }
    }
}
