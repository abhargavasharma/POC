using System;
using System.Linq;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface ICreditCardPaymentOptionService
    {
        ICreditCardPayment AssignCreditCardPayment(int policyId, string nameOnCard, string cardNumber, string expiryMonth, string expiryYear, CreditCardType cardType);
        ICreditCardPayment GetCurrentCreditCardPayment(int policyId);
        void RemoveExistingPayment(int policyPaymentId);
    }

    public class CreditCardPaymentOptionService : BasePaymentOptionService<CreditCardPaymentDto>, ICreditCardPaymentOptionService
    {
        private readonly ICreditCardTokenisationServiceFactory _creditCardTokenisationServiceFactory;
        private readonly ICreditCardPaymentRepository _creditCardPaymentRepository;
        private readonly ILoggingService _loggingService;

        public ICreditCardPayment AssignCreditCardPayment(int policyId, string nameOnCard, string cardNumber, string expiryMonth, string expiryYear, CreditCardType cardType)
        {
            var cc = new CreditCard
            {
                CardName = nameOnCard,
                CardNumber = cardNumber,
                CardExpiryMonth = int.Parse(expiryMonth),
                CardExpiryYear = int.Parse(expiryYear),
                CardType = cardType.ToString(),
            };

            var tokenizationService = _creditCardTokenisationServiceFactory.Build();
            CreditCard tokenizedCreditCard;
            try
            {
                tokenizedCreditCard = tokenizationService.TokeniseCreditCard(cc);
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Tokenization Service failed to tokenize the credit card", ex);
                _loggingService.Error(exception);
                throw exception;
            }

            var creditCardDto = new CreditCardPaymentDto
            {
                CardNumber = tokenizedCreditCard.MaskedCreditCardNumber,
                NameOnCard = tokenizedCreditCard.CardName,
                ExpiryMonth = tokenizedCreditCard.CardExpiryMonth.ToString(),
                ExpiryYear = tokenizedCreditCard.CardExpiryYear.ToString(),
                CardType = cardType,
                Token = tokenizedCreditCard.Token.ToString()
            };

            return AssignPayment(policyId, creditCardDto, PaymentType.CreditCard);
        }

        public ICreditCardPayment GetCurrentCreditCardPayment(int policyId)
        {
            var paymentDto = GetPolicyPaymentDto(policyId, PaymentType.CreditCard);
            if (paymentDto == null)
                return null;

            return _creditCardPaymentRepository.GetForPolicyPaymentId(paymentDto.Id);
        }
      
        public override CreditCardPaymentDto InsertIntoRepository(CreditCardPaymentDto payment)
        {
            return _creditCardPaymentRepository.Insert(payment);
        }

        public override void RemoveExistingPayment(int policyPaymentId)
        {
            var creditCardPayment = _creditCardPaymentRepository.GetForPolicyPaymentId(policyPaymentId);
            if (creditCardPayment != null)
            {
                _creditCardPaymentRepository.Delete(creditCardPayment);
            }
            
        }

        public CreditCardPaymentOptionService(IPolicyPaymentRepository policyPaymentRepository,
            ICreditCardTokenisationServiceFactory creditCardTokenisationServiceFactory,
            ICreditCardPaymentRepository creditCardPaymentRepository, ILoggingService loggingService) : base(policyPaymentRepository)
        {
            _creditCardTokenisationServiceFactory = creditCardTokenisationServiceFactory;
            _creditCardPaymentRepository = creditCardPaymentRepository;
            _loggingService = loggingService;
        }
    }
}
