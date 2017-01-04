using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPolicyDtoRepository
    {
        PolicyDto GetPolicy(int policyId);
        PolicyDto GetPolicyByQuoteReference(string quoteReferenceNumber);
        PolicyDto InsertPolicy(PolicyDto policy);
        void UpdatePolicy(PolicyDto policy);
    }

    public class PolicyDtoRepository : BaseRepository<PolicyDto>, IPolicyDtoRepository
    {
        private readonly ICachingWrapper _cache;

        public PolicyDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cache)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cache = cache;
        }

        public PolicyDto GetPolicy(int id)
        {
            return _cache.GetOrAddCacheItemRequestScope(GetType(), $"PolicyId-{id}", () => Get(id));
        }

        public PolicyDto GetPolicyByQuoteReference(string quoteReferenceNumber)
        {
            return
                _cache.GetOrAddCacheItemRequestScope(GetType(), $"QuoteReferenceNumber-{quoteReferenceNumber}",
                    () => Where(policy => policy.QuoteReference, Op.Eq, quoteReferenceNumber)).SingleOrDefault();
        }

        public PolicyDto InsertPolicy(PolicyDto policy)
        {
            return Insert(policy);
        }

        public void UpdatePolicy(PolicyDto policy)
        {
            Update(policy);
            var updatedDto = Get(policy.Id);
            policy.RV = updatedDto.RV;
            _cache.UpdateOrAddCacheItemRequestScope(GetType(), $"PolicyId-{policy.Id}", policy);
        }

        public new bool Delete(PolicyDto policy)
        {
            var result = base.Delete(policy);

            if (result)
            {
                _cache.RemoveItem(GetType(), $"PolicyId-{policy.Id}");
            }

            return result;
        }
    }

}
