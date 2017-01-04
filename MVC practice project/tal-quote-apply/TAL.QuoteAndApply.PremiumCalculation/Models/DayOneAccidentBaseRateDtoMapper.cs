using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class DayOneAccidentBaseRateDtoMapper : DbItemClassMapper<DayOneAccidentBaseRateDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<DayOneAccidentBaseRateDto> mapper)
        {
            mapper.MapTable("DayOneAccidentBaseRate");
            mapper.MapProperty(dayOne => dayOne.Gender, "GenderId");
            mapper.MapProperty(dayOne => dayOne.PremiumType, "PremiumTypeId");
        }
    }
}