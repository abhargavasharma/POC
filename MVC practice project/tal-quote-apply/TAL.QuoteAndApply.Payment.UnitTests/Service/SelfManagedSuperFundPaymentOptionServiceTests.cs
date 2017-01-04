using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Service;

namespace TAL.QuoteAndApply.Payment.UnitTests.Service
{
    [TestFixture]
    public class SelfManagedSuperFundPaymentOptionServiceTests
    {
        private Mock<IPolicyPaymentRepository> _policyPaymentRepository;
        private Mock<ISelfManagedSuperFundPaymentRepository> _selfManagedSuperPaymentRepository;

        [SetUp]
        public void Setup()
        {
            _policyPaymentRepository = new Mock<IPolicyPaymentRepository>(MockBehavior.Strict);
            _selfManagedSuperPaymentRepository = new Mock<ISelfManagedSuperFundPaymentRepository>(MockBehavior.Strict);
        }

        [Test]
        public void AssignSelfManagedSuperFundPayment_NoPaymentCurrentlyExistsAndNoPaymentDtoExists_AddNewSelfManagedSuperFundAndUseExistingPaymentDto()
        {
            var service = new SelfManagedSuperFundPaymentOptionService(_policyPaymentRepository.Object,
                _selfManagedSuperPaymentRepository.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>());

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>())).Returns(new PolicyPaymentDto()
            {
                PolicyId = 200,
                PaymentType = PaymentType.SelfManagedSuperFund,
                Id = 1
            });

            _selfManagedSuperPaymentRepository.Setup(call => call.Insert(It.Is<SelfManagedSuperFundPaymentDto>(cc => cc.AccountNumber == "12345678")))
                .Returns(new SelfManagedSuperFundPaymentDto()
                {
                    AccountNumber = "12345678",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _selfManagedSuperPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<SuperAnnuationPaymentDto>(null);

            var retVal = service.AssignSelfManagedSuperFundPayment(200, "011033", "12345678", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _policyPaymentRepository.Verify(call => call.Insert(It.IsAny<PolicyPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("12345678"));
        }

        [Test]
        public void AssignSelfManagedSuperFundPayment_NoDirectDebitCurrentlyExistsAndPaymentDtoExists_AddSelfManagedSuperFundAndUseExistingPaymentDto()
        {
            var service = new SelfManagedSuperFundPaymentOptionService(_policyPaymentRepository.Object,
                _selfManagedSuperPaymentRepository.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.SelfManagedSuperFund,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _selfManagedSuperPaymentRepository.Setup(call => call.Insert(It.Is<SelfManagedSuperFundPaymentDto>(cc => cc.AccountNumber == "12345678")))
                .Returns(new SelfManagedSuperFundPaymentDto()
                {
                    AccountNumber = "12345678",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _selfManagedSuperPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<SelfManagedSuperFundPaymentDto>(null);

            var retVal = service.AssignSelfManagedSuperFundPayment(200, "011033", "12345678", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("12345678"));
        }

        [Test]
        public void AssignSelfManagedSuperFundPayment_DirectDebitCurrentlyExistsAndPaymentDtoExists_ReplaceSelfManagedSuperFundAndUseExistingPaymentDto()
        {
            var service = new SelfManagedSuperFundPaymentOptionService(_policyPaymentRepository.Object,
                 _selfManagedSuperPaymentRepository.Object);
            
            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.SelfManagedSuperFund,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _selfManagedSuperPaymentRepository.Setup(call => call.Insert(It.Is<SelfManagedSuperFundPaymentDto>(cc => cc.AccountNumber == "98765432")))
                .Returns(new SelfManagedSuperFundPaymentDto()
                {
                    AccountNumber = "98765432",
                    BSBNumber = "011033",
                    AccountName = "AccountName"
                });

            _selfManagedSuperPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns(new SelfManagedSuperFundPaymentDto
            {
                AccountNumber = "12345678",
                BSBNumber = "011033",
                AccountName = "AccountName"
            });

            _selfManagedSuperPaymentRepository.Setup(call => call.Delete(It.IsAny<SelfManagedSuperFundPaymentDto>())).Returns(true);

            var retVal = service.AssignSelfManagedSuperFundPayment(200, "011033", "98765432", "AccountName");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _selfManagedSuperPaymentRepository.Verify(call => call.GetForPolicyPaymentId(It.Is<int>(i => i == 1)));
            _selfManagedSuperPaymentRepository.Verify(call => call.Delete(It.IsAny<SelfManagedSuperFundPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.AccountNumber, Is.EqualTo("98765432"));
        }
    }
}
