using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Data.Encryption
{
    public class EncryptedPartyDtoClassMapper : DbItemClassMapper<EncryptedPartyDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<EncryptedPartyDto> mapper)
        {
            mapper.MapTable("Party");
            mapper.MapProperty(party => party.Gender, "GenderId");
            mapper.MapProperty(party => party.Title, "TitleId");
            mapper.MapProperty(party => party.State, "StateId");
            mapper.MapProperty(party => party.Country, "CountryId");
            mapper.Encrypt(p => p.EmailAddress);
        }
    }
}