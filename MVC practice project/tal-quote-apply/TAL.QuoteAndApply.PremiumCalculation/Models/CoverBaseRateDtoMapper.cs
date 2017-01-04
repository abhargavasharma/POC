using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class CoverBaseRateDtoMapper : DbItemClassMapper<CoverBaseRateDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CoverBaseRateDto> mapper)
        {
            mapper.MapTable("CoverBaseRate");
            mapper.MapProperty(cbr => cbr.Gender, "GenderId");
            mapper.MapProperty(cbr => cbr.PremiumType, "PremiumTypeId");
        }
    }
}