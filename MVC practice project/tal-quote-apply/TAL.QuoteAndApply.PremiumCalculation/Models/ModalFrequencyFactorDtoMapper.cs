using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class ModalFrequencyFactorDtoMapper : DbItemClassMapper<ModalFrequencyFactorDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<ModalFrequencyFactorDto> mapper)
        {
            mapper.MapTable("ModalFrequencyFactor");
            mapper.MapProperty(cbr => cbr.PremiumFrequency, "PremiumFrequencyId");
        }
    }
}