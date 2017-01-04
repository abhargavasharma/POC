using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPolicyAnalyticsIdentifierDtoRepository
    {
        PolicyAnalyticsIdentifierDto GetByPolicyId(int policyId);
        PolicyAnalyticsIdentifierDto InsertPolicyAnalyticsIdentifier(PolicyAnalyticsIdentifierDto policyAnalyticsIdentifier);
        void UpdatePolicyAnalyticsIdentifier(PolicyAnalyticsIdentifierDto policyAnalyticsIdentifier);
    }

    public class PolicyAnalyticsIdentifierDtoRepository : BaseRepository<PolicyAnalyticsIdentifierDto>, IPolicyAnalyticsIdentifierDtoRepository
    {
        public PolicyAnalyticsIdentifierDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }


        public PolicyAnalyticsIdentifierDto GetByPolicyId(int policyId)
        {
            var policyAnalyticsIdentifier = Where(pai => pai.PolicyId, Op.Eq, policyId);
            return policyAnalyticsIdentifier.FirstOrDefault();
        }

        public PolicyAnalyticsIdentifierDto InsertPolicyAnalyticsIdentifier(PolicyAnalyticsIdentifierDto policyAnalyticsIdentifier)
        {
            return Insert(policyAnalyticsIdentifier);
        }

        public void UpdatePolicyAnalyticsIdentifier(PolicyAnalyticsIdentifierDto policyAnalyticsIdentifier)
        {
            Update(policyAnalyticsIdentifier);
        }
    }
}
