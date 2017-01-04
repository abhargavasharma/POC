using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Data.Encryption
{
    public class EncryptedPartyDtoRepository : BaseRepository<EncryptedPartyDto>
    {
        public EncryptedPartyDtoRepository(IPartyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public EncryptedPartyDto InsertParty(EncryptedPartyDto party)
        {
            return Insert(party);
        }

        public EncryptedPartyDto GetParty(int id)
        {
            return Get(id);
        }

        public void UpdateParty(EncryptedPartyDto party)
        {
            Update(party);
        }
    }
}