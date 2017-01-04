using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Service;

namespace TAL.QuoteAndApply.Payment.UnitTests.Service
{
    [TestFixture]
    public class DirectDebitPaymentOptionServiceTests
    {
        private Mock<IPolicyPaymentRepository> _policyPaymentRepository;
        private Mock<IDirectDebitPaymentRepository> _directDebitPaymentRepository;

        [SetUp]
        public void Setup()
        {
            _policyPaymentRepository = new Mock<IPolicyPaymentRepository>(MockBehavior.Strict);
            _directDebitPaymentRepository = new Mock<IDirectDebitPaymentRepository>(MockBehavior.Strict);
        }

        [Test]
        public void AssignCreditCardPayment_NoDirectDebitCurrentlyExistsAndNoPaymentDtoExists_AddNewCreditCardAndUseExistingPaymentDto()
        {
            var service = new DirectDebitPaymentOptionService(_policyPaymentRepository.Object,
                _directDebitPaymentRepository.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>());

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>())).Returns(new PolicyPaymentDto()
            {
                PolicyId = 200,
                PaymentType = PaymentType.DirectDebit,
                Id = 1
            });

            _directDebitPaymentRepository.Setup(call => call.Insert(It.Is<DirectDebitPaymentDto>(cc => cc.AccountNumber == "12345678")))
                .Returns(new DirectDebitPaymentDto()
                {
                    AccountNumber = "12345678",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _directDebitPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<DirectDebitPaymentDto>(null);

            var retVal = service.AssignDebitCardPayment(200, "011033", "12345678", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _policyPaymentRepository.Verify(call => call.Insert(It.IsAny<PolicyPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("12345678"));
        }

        [Test]
        public void AssignCreditCardPayment_NoDirectDebitCurrentlyExistsAndPaymentDtoExists_AddNewCreditCardAndUseExistingPaymentDto()
        {
            var service = new DirectDebitPaymentOptionService(_policyPaymentRepository.Object,
                _directDebitPaymentRepository.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.DirectDebit,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _directDebitPaymentRepository.Setup(call => call.Insert(It.Is<DirectDebitPaymentDto>(cc => cc.AccountNumber == "12345678")))
                .Returns(new DirectDebitPaymentDto()
                {
                    AccountNumber = "12345678",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _directDebitPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<DirectDebitPaymentDto>(null);

            var retVal = service.AssignDebitCardPayment(200, "011033", "12345678", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("12345678"));
        }

        [Test]
        public void AssignCreditCardPayment_DirectDebitCurrentlyExistsAndPaymentDtoExists_ReplaceCreditCardAndUseExistingPaymentDto()
        {
            var service = new DirectDebitPaymentOptionService(_policyPaymentRepository.Object,
                 _directDebitPaymentRepository.Object);
            
            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.DirectDebit,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _directDebitPaymentRepository.Setup(call => call.Insert(It.Is<DirectDebitPaymentDto>(cc => cc.AccountNumber == "98765432")))
                .Returns(new DirectDebitPaymentDto()
                {
                    AccountNumber = "98765432",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _directDebitPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns(new DirectDebitPaymentDto
            {
                AccountNumber = "12345678",
                BSBNumber = "011033",
                AccountName = "AccountName"
            });

            _directDebitPaymentRepository.Setup(call => call.Delete(It.IsAny<DirectDebitPaymentDto>())).Returns(true);

            var retVal = service.AssignDebitCardPayment(200, "011033", "98765432", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _directDebitPaymentRepository.Verify(call => call.GetForPolicyPaymentId(It.Is<int>(i => i == 1)));
            _directDebitPaymentRepository.Verify(call => call.Delete(It.IsAny<DirectDebitPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("98765432"));
        }
    }
}
