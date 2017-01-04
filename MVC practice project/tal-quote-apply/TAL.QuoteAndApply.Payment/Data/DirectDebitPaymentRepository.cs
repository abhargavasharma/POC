using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Data
{
    public interface IDirectDebitPaymentRepository
    {
        DirectDebitPaymentDto Get(int id);
        DirectDebitPaymentDto Insert(DirectDebitPaymentDto dbItem);
        void Update(DirectDebitPaymentDto dbItem);
        bool Delete(DirectDebitPaymentDto dbItem);
        DirectDebitPaymentDto GetForPolicyPaymentId(int paymentId);
    }

    public class DirectDebitPaymentRepository : BaseRepository<DirectDebitPaymentDto>, IDirectDebitPaymentRepository
    {
        public DirectDebitPaymentRepository(IPaymentConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {

        }

        public DirectDebitPaymentDto GetForPolicyPaymentId(int paymentId)
        {
            return Where(cc => cc.PolicyPaymentId, Op.Eq, paymentId).FirstOrDefault();
        }
    }
}