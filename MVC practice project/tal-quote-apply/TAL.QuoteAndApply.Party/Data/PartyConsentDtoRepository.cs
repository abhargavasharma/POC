using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Data
{
    public interface IPartyConsentDtoRepository
    {
        PartyConsentDto InsertPartyConsent(PartyConsentDto partyConsent);
        PartyConsentDto GetPartyConsent(int partyConsentId);
        void UpdatePartyConsent(PartyConsentDto partyConsent);
        PartyConsentDto GetPartyConsentByPartyId(int partyId);
        void DeletePartyConsent(int partyId);
    }

    public class PartyConsentDtoRepository : BaseRepository<PartyConsentDto>, IPartyConsentDtoRepository
    {

        public PartyConsentDtoRepository(IPartyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            
        }

        public PartyConsentDto InsertPartyConsent(PartyConsentDto partyConsent)
        {
            return Insert(partyConsent);
        }

        public PartyConsentDto GetPartyConsent(int partyConsentId)
        {
            return Get(partyConsentId);
        }

        public PartyConsentDto GetPartyConsentByPartyId(int partyId)
        {
            return Where(p => p.PartyId, Op.Eq, partyId).FirstOrDefault();
        }

        public void DeletePartyConsent(int partyId)
        {
            Delete(DbItemPredicate<PartyConsentDto>.Where(pc => pc.PartyId, Op.Eq, partyId));
        }

        public void UpdatePartyConsent(PartyConsentDto partyConsent)
        {
            Update(partyConsent);
        }
    }

}
