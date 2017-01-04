using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Party.Models
{
    public sealed class PartyDtoClassMapper : DbItemClassMapper<PartyDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PartyDto> mapper)
        {
            mapper.MapTable("Party");
            mapper.MapProperty(party => party.Gender, "GenderId");
            mapper.MapProperty(party => party.Title, "TitleId");
            mapper.MapProperty(party => party.State, "StateId");
            mapper.MapProperty(party => party.Country, "CountryId");
        }
    }
}