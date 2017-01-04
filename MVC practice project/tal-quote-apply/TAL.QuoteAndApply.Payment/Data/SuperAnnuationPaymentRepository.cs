using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Data
{
    public interface ISuperAnnuationPaymentRepository
    {
        SuperAnnuationPaymentDto Get(int id);
        SuperAnnuationPaymentDto Insert(SuperAnnuationPaymentDto dbItem);
        void Update(SuperAnnuationPaymentDto dbItem);
        bool Delete(SuperAnnuationPaymentDto dbItem);
        SuperAnnuationPaymentDto GetForPolicyPaymentId(int paymentId);
    }

    public class SuperAnnuationPaymentRepository : BaseRepository<SuperAnnuationPaymentDto>, ISuperAnnuationPaymentRepository
    {
        public SuperAnnuationPaymentRepository(IPaymentConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {

        }

        public SuperAnnuationPaymentDto GetForPolicyPaymentId(int paymentId)
        {
            return Where(cc => cc.PolicyPaymentId, Op.Eq, paymentId).FirstOrDefault();
        }
    }
}