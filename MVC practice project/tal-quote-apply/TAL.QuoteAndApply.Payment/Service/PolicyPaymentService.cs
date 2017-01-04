using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface IPolicyPaymentService
    {
        void UpdateExistingPaymentType(int policyPaymentId, PaymentType paymentType);
    }

    public class PolicyPaymentService : IPolicyPaymentService
    {
        private readonly IPolicyPaymentRepository _policyPaymentRepository;

        public PolicyPaymentService(IPolicyPaymentRepository policyPaymentRepository)
        {
            _policyPaymentRepository = policyPaymentRepository;
        }

        public PolicyPaymentDto GetCurrentPayment(int policyId)
        {
            var paymentDto = _policyPaymentRepository.GetForPolicy(policyId);
            if (paymentDto == null)
                return null;

            return paymentDto.FirstOrDefault();
        }

        public void UpdateExistingPaymentType(int policyPaymentId, PaymentType paymentType)
        {
            var policyPayment = GetCurrentPayment(policyPaymentId);
            policyPayment.PaymentType = paymentType;
            if (policyPayment != null)
            {
                _policyPaymentRepository.Update(policyPayment);
            }
        }
    }
}
