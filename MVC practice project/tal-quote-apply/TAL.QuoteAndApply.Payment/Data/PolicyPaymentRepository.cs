using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Data
{
    public interface IPolicyPaymentRepository
    {
        PolicyPaymentDto Get(int id);
        PolicyPaymentDto Insert(PolicyPaymentDto dbItem);
        void Update(PolicyPaymentDto dbItem);
        bool Delete(PolicyPaymentDto dbItem);
        ICollection<PolicyPaymentDto> GetForPolicy(int policyId);
    }

    public class PolicyPaymentRepository : BaseRepository<PolicyPaymentDto>, IPolicyPaymentRepository
    {
        public PolicyPaymentRepository(IPaymentConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public ICollection<PolicyPaymentDto> GetForPolicy(int policyId)
        {
            return Where(pp => pp.PolicyId, Op.Eq, policyId).ToArray();
        }
    }
}