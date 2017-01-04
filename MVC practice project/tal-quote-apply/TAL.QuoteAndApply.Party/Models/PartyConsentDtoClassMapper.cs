
using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Party.Models
{
    public sealed class PartyConsentDtoClassMapper : DbItemClassMapper<PartyConsentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PartyConsentDto> mapper)
        {
            mapper.MapTable("PartyConsent");
        }
    }
}