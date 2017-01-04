using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Data
{
    public interface ICreditCardPaymentRepository
    {
        CreditCardPaymentDto Get(int id);
        CreditCardPaymentDto Insert(CreditCardPaymentDto dbItem);
        void Update(CreditCardPaymentDto dbItem);
        bool Delete(CreditCardPaymentDto dbItem);
        CreditCardPaymentDto GetForPolicyPaymentId(int paymentId);
    }

    public class CreditCardPaymentRepository : BaseRepository<CreditCardPaymentDto>, ICreditCardPaymentRepository
    {
        public CreditCardPaymentRepository(IPaymentConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public CreditCardPaymentDto GetForPolicyPaymentId(int paymentId)
        {
            return Where(cc => cc.PolicyPaymentId, Op.Eq, paymentId).FirstOrDefault();
        }
    }
}
