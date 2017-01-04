using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Interactions.Configuration;
using TAL.QuoteAndApply.Interactions.Models;

namespace TAL.QuoteAndApply.Interactions.Data
{
    public interface IPolicyInteractionDtoRepository
    {
        PolicyInteractionDto InsertInteraction (PolicyInteractionDto policyInteraction);
        PolicyInteractionDto GetInteraction (int id);
        IEnumerable<PolicyInteractionDto> GetInteractionsByPolicyId(int policyId);
        void UpdateInteraction (PolicyInteractionDto policyInteraction);
    }
    public class PolicyInteractionDtoRepository : BaseRepository<PolicyInteractionDto>, IPolicyInteractionDtoRepository
    {

        public PolicyInteractionDtoRepository(IInteractionsConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
         IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public PolicyInteractionDto InsertInteraction (PolicyInteractionDto policyInteraction)
        {
            return Insert(policyInteraction);
        }

        public PolicyInteractionDto GetInteraction (int id)
        {
            return Get(id);
        }

        public IEnumerable<PolicyInteractionDto> GetInteractionsByPolicyId(int policyId)
        {
            var interactions = Where(policy => policy.PolicyId, Op.Eq, policyId);
            interactions = interactions.OrderByDescending(policy => policy.CreatedTS);
            return interactions;
        }

        public void UpdateInteraction (PolicyInteractionDto policyInteraction)
        {
            Update(policyInteraction);
        }
    }
}
