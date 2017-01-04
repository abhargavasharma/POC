using System.Linq;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Service
{
    public abstract class BasePaymentOptionService<T> where T : IPayment
    {
        private readonly IPolicyPaymentRepository _policyPaymentRepository;

        protected BasePaymentOptionService(IPolicyPaymentRepository policyPaymentRepository)
        {
            _policyPaymentRepository = policyPaymentRepository;
        }

        public T AssignPayment(int policyId, T payment, PaymentType paymentType)
        {
            var policyPayment = GetOrCreateNewPolicyPaymentDto(policyId, paymentType);
            payment.PolicyPaymentId = policyPayment.Id;
            InsertIntoRepository(payment);
            return payment;
        }

        protected PolicyPaymentDto GetPolicyPaymentDto(int policyId, PaymentType paymentType)
        {
            return _policyPaymentRepository.GetForPolicy(policyId)
                .FirstOrDefault(pp => pp.PaymentType == paymentType);
        }

        private PolicyPaymentDto GetOrCreateNewPolicyPaymentDto(int policyId, PaymentType paymentType)
        {
            var policyPayment = GetPolicyPaymentDto(policyId, paymentType);
            if (policyPayment == null)
            {
                policyPayment = CreateNewPolicyPayment(policyId, paymentType);
            }
            else
            {
                RemoveExistingPayment(policyPayment.Id);
            }
            return policyPayment;
        }

        private PolicyPaymentDto CreateNewPolicyPayment(int policyId, PaymentType paymentType)
        {
            var paymentDto = new PolicyPaymentDto
            {
                PolicyId = policyId,
                PaymentType = paymentType
            };
            return _policyPaymentRepository.Insert(paymentDto);
        }

        public abstract T InsertIntoRepository(T payment);

        public abstract void RemoveExistingPayment(int policyPaymentId);
    }
}