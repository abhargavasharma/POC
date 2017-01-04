using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Data
{
    public interface ISelfManagedSuperFundPaymentRepository
    {
        SelfManagedSuperFundPaymentDto Get(int id);
        SelfManagedSuperFundPaymentDto Insert(SelfManagedSuperFundPaymentDto dbItem);
        void Update(SelfManagedSuperFundPaymentDto dbItem);
        bool Delete(SelfManagedSuperFundPaymentDto dbItem);
        SelfManagedSuperFundPaymentDto GetForPolicyPaymentId(int paymentId);
    }

    public class SelfManagedSuperFundPaymentRepository : BaseRepository<SelfManagedSuperFundPaymentDto>, ISelfManagedSuperFundPaymentRepository
    {
        public SelfManagedSuperFundPaymentRepository(IPaymentConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {

        }

        public SelfManagedSuperFundPaymentDto GetForPolicyPaymentId(int paymentId)
        {
            return Where(cc => cc.PolicyPaymentId, Op.Eq, paymentId).FirstOrDefault();
        }
    }
}