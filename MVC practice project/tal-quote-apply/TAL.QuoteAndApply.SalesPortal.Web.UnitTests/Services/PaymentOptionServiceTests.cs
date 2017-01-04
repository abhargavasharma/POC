using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Payment.Rules;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services
{
    [TestFixture]
    public class PaymentOptionServiceTests
    {
        private Mock<IPaymentOptionsAvailabilityProvider> _paymentOptionsAvailabilityProvider;
        private Mock<ICreditCardPaymentOptionService> _creditCardPaymentOptionService;
        private Mock<IDirectDebitPaymentOptionService> _directDebitPaymentOptionService;
        private Mock<ISelfManagedSuperFundPaymentOptionService> _selfManagedSuperFundPaymentOptionService;
        private Mock<ISuperAnnuationPaymentOptionService> _superAnnuationPaymentOptionService;
        private Mock<IDirectDebitPaymentParamConverter> _directDebitPaymentParamConverter;
        private Mock<ISelfManagedSuperFundPaymentParamConverter> _SelfManagedSuperFundPaymentParamConverter;
        private Mock<ICreditCardPaymentParamConverter> _creditCardPaymentParamConverter;
        private Mock<ISuperannuationPaymentParamConverter> _superannuationPaymentParamConverter;
        private Mock<IPlanService> _planService;
        private IPaymentRulesService _paymentRulesService;
        private Mock<IPolicyService> _policyService;
        private Mock<IPolicyPaymentService> _policyPaymentService;
        private CreditCardTypeConverter _creditCardTypeConverter;

        [SetUp]
        public void SetupMocks()
        {
            IPaymentOptionsRuleFactory fact = new PaymentOptionsRuleFactory();

            _paymentOptionsAvailabilityProvider = new Mock<IPaymentOptionsAvailabilityProvider>(MockBehavior.Strict);
            _creditCardPaymentOptionService = new Mock<ICreditCardPaymentOptionService>(MockBehavior.Strict);
            _directDebitPaymentOptionService = new Mock<IDirectDebitPaymentOptionService>(MockBehavior.Strict);
            _selfManagedSuperFundPaymentOptionService = new Mock<ISelfManagedSuperFundPaymentOptionService>(MockBehavior.Strict);
            _superAnnuationPaymentOptionService = new Mock<ISuperAnnuationPaymentOptionService>(MockBehavior.Strict);

            _directDebitPaymentParamConverter = new Mock<IDirectDebitPaymentParamConverter>(MockBehavior.Strict);
            _SelfManagedSuperFundPaymentParamConverter = new Mock<ISelfManagedSuperFundPaymentParamConverter>(MockBehavior.Strict);
            _creditCardPaymentParamConverter = new Mock<ICreditCardPaymentParamConverter>(MockBehavior.Strict);
            _superannuationPaymentParamConverter = new Mock<ISuperannuationPaymentParamConverter>(MockBehavior.Strict);
            _planService = new Mock<IPlanService>(MockBehavior.Strict);            
            _paymentRulesService = new PaymentRulesService(fact);
            _policyService = new Mock<IPolicyService>(MockBehavior.Strict);
            _policyPaymentService = new Mock<IPolicyPaymentService>(MockBehavior.Strict);
            _creditCardTypeConverter = new CreditCardTypeConverter();
        }

        private IPaymentOptionService CreatePaymentOptionService()
        {
            var service = new PaymentOptionService(_paymentOptionsAvailabilityProvider.Object,
                _creditCardPaymentOptionService.Object,
                _directDebitPaymentOptionService.Object,
                 _superAnnuationPaymentOptionService.Object,
                _selfManagedSuperFundPaymentOptionService.Object,
                _directDebitPaymentParamConverter.Object,
                _creditCardPaymentParamConverter.Object,
                _SelfManagedSuperFundPaymentParamConverter.Object,
                _superannuationPaymentParamConverter.Object,
                _policyService.Object,
                _planService.Object,
                _paymentRulesService,
                _policyPaymentService.Object, 
                _creditCardTypeConverter
                );

            return service;
        }

        [Test]
        public void ShouldPassValidateVisaCreditCardNumber()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("4444333322221111", CreditCardType.Visa);
            Assert.True(valid, "Validation fail for visa");
        }

        [Test]
        public void ShouldPassValidateMastercardCreditCardNumber()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("5555555555554444", CreditCardType.MasterCard);
            Assert.True(valid, "Validation fail for mastercard");
        }

        [Test]
        public void ShouldFailValidateVisaCreditCardNumber()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("1111111111111111", CreditCardType.Visa);
            Assert.False(valid, "Validation fail for visa");

            var valid1 = service.ValidateCardNumber("5555555555554444", CreditCardType.Visa);
            Assert.False(valid1, "Validation fail for visa");
        }

        [Test]
        public void ShouldFailValidateVisaCreditCardNumberForLessThan16Digits()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("44443333", CreditCardType.Visa);
            Assert.False(valid, "Validation fail for visa");
        }

        [Test]
        public void ShouldFailValidateVisaCreditCardNumberForNonDigits()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("4444333322d21111", CreditCardType.Visa);
            Assert.False(valid, "Validation fail for visa");
        }

        [Test]
        public void ShouldFailValidateVisaCreditCardNumberForEmpty()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("", CreditCardType.Visa);
            Assert.False(valid, "Validation fail for visa");
        }

        [Test]
        public void ShouldFailValidateMastercardCreditCardNumber()
        {
            var service = CreatePaymentOptionService();

            var valid = service.ValidateCardNumber("3333333333333333", CreditCardType.MasterCard);
            Assert.False(valid, "Validation fail for mastercard");

            var valid1 = service.ValidateCardNumber("4444333322221111", CreditCardType.MasterCard);
            Assert.False(valid1, "Validation fail for mastercard");
        }
    }
}
