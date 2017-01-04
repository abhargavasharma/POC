using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Service;

namespace TAL.QuoteAndApply.Payment.UnitTests.Service
{
    [TestFixture]
    public class CreditCardPaymentOptionServiceTests
    {
        private Mock<IPolicyPaymentRepository> _policyPaymentRepository;
        private Mock<ICreditCardTokenisationServiceFactory> _creditCardTokenisationServiceFactory;
        private Mock<ICreditCardPaymentRepository> _creditCardPaymentRepository;
        private Mock<ICreditCardTokenisationService> _creditCardTokenisationService;
        private Mock<ILoggingService> _loggingService;

        [SetUp]
        public void Setup()
        {
            
            _policyPaymentRepository = new Mock<IPolicyPaymentRepository>(MockBehavior.Strict);
            _creditCardPaymentRepository = new Mock<ICreditCardPaymentRepository>(MockBehavior.Strict);
            _creditCardTokenisationServiceFactory = new Mock<ICreditCardTokenisationServiceFactory>(MockBehavior.Strict);
            _creditCardTokenisationService = new Mock<ICreditCardTokenisationService>(MockBehavior.Strict);
            _loggingService = new Mock<ILoggingService>(MockBehavior.Strict);

            _creditCardTokenisationServiceFactory.Setup(call => call.Build()).Returns(_creditCardTokenisationService.Object);
        }

        [Test]
        public void AssignCreditCardPayment_NoCreditCardCurrentlyExistsAndNoPaymentDtoExists_AddNewCreditCardAndUseExistingPaymentDto()
        {
            var service = new CreditCardPaymentOptionService(_policyPaymentRepository.Object,
                _creditCardTokenisationServiceFactory.Object, _creditCardPaymentRepository.Object, _loggingService.Object);

            _creditCardTokenisationService.Setup(call => call.TokeniseCreditCard(It.IsAny<CreditCard>()))
                .Returns(new CreditCard
                {
                    CardNumber = "4111111122222222",
                    Token = Guid.NewGuid(),
                    CardName = "Chris Moretton",
                    CardExpiryMonth = 8,
                    CardExpiryYear = 17,
                    CardType = "Unknown",
                    MaskedCreditCardNumber = "411111********22"
                });
            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>());

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>())).Returns(new PolicyPaymentDto()
            {
                PolicyId = 200,
                PaymentType = PaymentType.CreditCard,
                Id = 1
            });

            _creditCardPaymentRepository.Setup(call => call.Insert(It.Is<CreditCardPaymentDto>(cc => cc.CardNumber == "411111********22")))
                .Returns(new CreditCardPaymentDto
                {
                    CardNumber = "411111********22",
                    Token = Guid.NewGuid().ToString(),
                    NameOnCard = "Chris Moretton",
                    ExpiryMonth = "08",
                    ExpiryYear = "17",
                });

            _creditCardPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<CreditCardPaymentDto>(null);

            var retVal = service.AssignCreditCardPayment(200, "Chris Moretton", "411111122222222", "08", "17", CreditCardType.Unknown);

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _policyPaymentRepository.Verify(call => call.Insert(It.IsAny<PolicyPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.CardNumber, Is.EqualTo("411111********22"));
        }

        [Test]
        public void AssignCreditCardPayment_NoCreditCardCurrentlyExistsAndPaymentDtoExists_AddNewCreditCardAndUseExistingPaymentDto()
        {
            var service = new CreditCardPaymentOptionService(_policyPaymentRepository.Object,
                _creditCardTokenisationServiceFactory.Object, _creditCardPaymentRepository.Object, _loggingService.Object);

            _creditCardTokenisationService.Setup(call => call.TokeniseCreditCard(It.IsAny<CreditCard>()))
                .Returns(new CreditCard
                {
                    CardNumber = "4111111122222222",
                    Token = Guid.NewGuid(),
                    CardName = "Chris Moretton",
                    CardExpiryMonth = 8,
                    CardExpiryYear = 17,
                    CardType = "Unknown",
                    MaskedCreditCardNumber = "411111********22"
                });

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.CreditCard,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _creditCardPaymentRepository.Setup(call => call.Insert(It.Is<CreditCardPaymentDto>(cc => cc.CardNumber == "411111********22")))
                .Returns(new CreditCardPaymentDto
                {
                    CardNumber = "411111********22",
                    Token = Guid.NewGuid().ToString(),
                    NameOnCard = "Chris Moretton",
                    ExpiryMonth = "08",
                    ExpiryYear = "17",
                });

            _creditCardPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<CreditCardPaymentDto>(null);

            var retVal = service.AssignCreditCardPayment(200, "Chris Moretton", "411111122222222", "08", "17", CreditCardType.Unknown);

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.CardNumber, Is.EqualTo("411111********22"));
        }

        [Test]
        public void AssignCreditCardPayment_CreditCardCurrentlyExistsAndPaymentDtoExists_ReplaceCreditCardAndUseExistingPaymentDto()
        {
            var service = new CreditCardPaymentOptionService(_policyPaymentRepository.Object,
                _creditCardTokenisationServiceFactory.Object, _creditCardPaymentRepository.Object, _loggingService.Object);

            _creditCardTokenisationService.Setup(call => call.TokeniseCreditCard(It.IsAny<CreditCard>()))
                .Returns(new CreditCard
                {
                    CardNumber = "4111333322222222",
                    Token = Guid.NewGuid(),
                    CardName = "Chris Moretton",
                    CardExpiryMonth = 8,
                    CardExpiryYear = 17,
                    CardType = "Unknown",
                    MaskedCreditCardNumber = "4111333********22"
                });

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.CreditCard,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _creditCardPaymentRepository.Setup(call => call.Insert(It.Is<CreditCardPaymentDto>(cc => cc.CardNumber == "4111333********22")))
                .Returns(new CreditCardPaymentDto
                {
                    CardNumber = "4111333********22",
                    Token = Guid.NewGuid().ToString(),
                    NameOnCard = "Chris Moretton",
                    ExpiryMonth = "08",
                    ExpiryYear = "17",
                });

            _creditCardPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns(new CreditCardPaymentDto
                {
                    CardNumber = "411111********22",
                    Token = Guid.NewGuid().ToString(),
                    NameOnCard = "Chris Moretton",
                    ExpiryMonth = "08",
                    ExpiryYear = "17",
                });

            _creditCardPaymentRepository.Setup(call => call.Delete(It.IsAny<CreditCardPaymentDto>())).Returns(true);

            var retVal = service.AssignCreditCardPayment(200, "Chris Moretton", "4111333322222222", "08", "17", CreditCardType.Unknown);

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _creditCardPaymentRepository.Verify(call => call.GetForPolicyPaymentId(It.Is<int>(i => i == 1)));
            _creditCardPaymentRepository.Verify(call => call.Delete(It.IsAny<CreditCardPaymentDto>()));
            
            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.CardNumber, Is.EqualTo("4111333********22"));
        }
    }
}
