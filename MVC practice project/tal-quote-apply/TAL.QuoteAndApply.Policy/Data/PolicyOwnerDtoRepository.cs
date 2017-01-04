using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPolicyOwnerDtoRepository
    {
        PolicyOwnerDto GetPolicyOwnerForPolicyId(int policyId);
        PolicyOwnerDto Insert(PolicyOwnerDto policyOwner);
        void Update(PolicyOwnerDto policyOwner);

    }

    public class PolicyOwnerDtoRepository : BaseRepository<PolicyOwnerDto>, IPolicyOwnerDtoRepository
    {
        public PolicyOwnerDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public PolicyOwnerDto GetPolicyOwnerForPolicyId(int policyId)
        {
            return Where(po => po.PolicyId, Op.Eq, policyId).FirstOrDefault();
        }
    }
}
