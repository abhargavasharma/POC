using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Data
{
    public interface IPartyDtoRepository
    {
        PartyDto InsertParty(PartyDto party);
        PartyDto GetParty(int id, bool ignoreCache = false);
        void UpdateParty(PartyDto party);
        IList<PartyDto> GetPartiesByLeadId(long leadId);
        void DeleteParty(int partyId);
    }

    public class PartyDtoRepository : BaseRepository<PartyDto>, IPartyDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PartyDtoRepository(IPartyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PartyDto InsertParty(PartyDto party)
        {
            var newDto = Insert(party);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PartyId-{party.Id}", newDto);
        }

        public PartyDto GetParty(int id, bool ignoreCache = false)
        {
            PartyDto party;
            if (ignoreCache)
            {
                party = Get(id);

                return party.IsDeleted 
                    ? null
                    : _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PartyId-{id}", party);
            }

            party = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PartyId-{id}", () => Get(id));
            return party.IsDeleted ? null : party;
        }

        public void UpdateParty(PartyDto party)
        {
            Update(party);
            var updatedDto = Get(party.Id);
            party.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PartyId-{party.Id}", party);
        }

        public IList<PartyDto> GetPartiesByLeadId(long leadId)
        {
            var parties = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"LeadId-{leadId}",
                () => Where(party => party.LeadId, Op.Eq, leadId));

            return parties
                .Where(p => p.IsDeleted == false)
                .Select(p => GetParty(p.Id))
                .ToList();
        }

        public void DeleteParty(int id)
        {
            var party = GetParty(id);
            party.IsDeleted = true;
            UpdateParty(party);

            _cachingWrapper.RemoveItem(GetType(), $"PartyId-{id}");
        }
    }
    
}
