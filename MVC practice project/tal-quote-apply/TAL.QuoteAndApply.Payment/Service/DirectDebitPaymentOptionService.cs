using System.Linq;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface IDirectDebitPaymentOptionService
    {
        IDirectDebitPayment AssignDebitCardPayment(int policyId, string bsb, string accountNumber, string nameOnAccount);
        IDirectDebitPayment GetCurrentDirectDebitPayment(int policyId);
        void RemoveExistingPayment(int policyPaymentId);
    }

    public class DirectDebitPaymentOptionService : BasePaymentOptionService<DirectDebitPaymentDto>, IDirectDebitPaymentOptionService
    {
        private readonly IDirectDebitPaymentRepository _directDebitPaymentRepository;

        public IDirectDebitPayment AssignDebitCardPayment(int policyId, string bsb, string accountNumber, string nameOnAccount)
        {
            var dd = new DirectDebitPaymentDto()
            {
                AccountName = nameOnAccount,
                AccountNumber = accountNumber,
                BSBNumber = bsb
            };
            return AssignPayment(policyId, dd, PaymentType.DirectDebit);
        }

        public IDirectDebitPayment GetCurrentDirectDebitPayment(int policyId)
        {
            var paymentDto = GetPolicyPaymentDto(policyId, PaymentType.DirectDebit);
            if (paymentDto == null)
                return null;

            return _directDebitPaymentRepository.GetForPolicyPaymentId(paymentDto.Id);
        }

        public override DirectDebitPaymentDto InsertIntoRepository(DirectDebitPaymentDto payment)
        {
            return _directDebitPaymentRepository.Insert(payment);
        }

        public override void RemoveExistingPayment(int policyPaymentId)
        {
            var directDebitPayment = _directDebitPaymentRepository.GetForPolicyPaymentId(policyPaymentId);
            if (directDebitPayment != null)
            {
                _directDebitPaymentRepository.Delete(directDebitPayment);
            }
        }

        public DirectDebitPaymentOptionService(IPolicyPaymentRepository policyPaymentRepository,
            IDirectDebitPaymentRepository directDebitPaymentRepository) : base(policyPaymentRepository)
        {
            _directDebitPaymentRepository = directDebitPaymentRepository;
        }
    }
}