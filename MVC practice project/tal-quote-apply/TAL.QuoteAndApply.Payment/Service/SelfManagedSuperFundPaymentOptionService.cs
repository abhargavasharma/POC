using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface ISelfManagedSuperFundPaymentOptionService
    {
        ISelfManagedSuperFundPayment AssignSelfManagedSuperFundPayment(int policyId, string bsb, string accountNumber, string nameOnAccount);
        ISelfManagedSuperFundPayment GetCurrentSelfManagedSuperFundPayment(int policyId);
        void RemoveExistingPayment(int policyPaymentId);
    }

    public class SelfManagedSuperFundPaymentOptionService : BasePaymentOptionService<SelfManagedSuperFundPaymentDto>, ISelfManagedSuperFundPaymentOptionService
    {
        private readonly ISelfManagedSuperFundPaymentRepository _selfManagedSuperFundPaymentRepository;

        public ISelfManagedSuperFundPayment AssignSelfManagedSuperFundPayment(int policyId, string bsb, string accountNumber, string nameOnAccount)
        {
            var dd = new SelfManagedSuperFundPaymentDto()
            {
                AccountName = nameOnAccount,
                AccountNumber = accountNumber,
                BSBNumber = bsb
            };
            return AssignPayment(policyId, dd, PaymentType.SelfManagedSuperFund);
        }

        public ISelfManagedSuperFundPayment GetCurrentSelfManagedSuperFundPayment(int policyId)
        {
            var paymentDto = GetPolicyPaymentDto(policyId, PaymentType.SelfManagedSuperFund);
            if (paymentDto == null)
                return null;

            return _selfManagedSuperFundPaymentRepository.GetForPolicyPaymentId(paymentDto.Id);
        }

        public override SelfManagedSuperFundPaymentDto InsertIntoRepository(SelfManagedSuperFundPaymentDto payment)
        {
            return _selfManagedSuperFundPaymentRepository.Insert(payment);
        }

        public override void RemoveExistingPayment(int policyPaymentId)
        {
            var selfManagedSuperFundPayment = _selfManagedSuperFundPaymentRepository.GetForPolicyPaymentId(policyPaymentId);
            if (selfManagedSuperFundPayment != null)
            {
                _selfManagedSuperFundPaymentRepository.Delete(selfManagedSuperFundPayment);
            }
        }

        public SelfManagedSuperFundPaymentOptionService(IPolicyPaymentRepository policyPaymentRepository,
            ISelfManagedSuperFundPaymentRepository selfManagedSuperFundPaymentRepository) : base(policyPaymentRepository)
        {
            _selfManagedSuperFundPaymentRepository = selfManagedSuperFundPaymentRepository;
        }
    }
}