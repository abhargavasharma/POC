using System;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Payment.Service.TFN;

namespace TAL.QuoteAndApply.Payment.UnitTests.Service
{
    [TestFixture]
    public class SuperAnnuationPaymentOptionServiceTests
    {
        private Mock<IPolicyPaymentRepository> _policyPaymentRepository;
        private Mock<ISuperAnnuationPaymentRepository> _superAnnuationPaymentRepository;
        private Mock<ITaxFileNumberEncyptionService> _taxFileNumberEncrpytionService;

        [SetUp]
        public void Setup()
        {
            _policyPaymentRepository = new Mock<IPolicyPaymentRepository>(MockBehavior.Strict);
            _superAnnuationPaymentRepository = new Mock<ISuperAnnuationPaymentRepository>(MockBehavior.Strict);
            _taxFileNumberEncrpytionService = new Mock<ITaxFileNumberEncyptionService>(MockBehavior.Strict);

            _taxFileNumberEncrpytionService.Setup(call => call.Encrypt(It.IsAny<string>())).Returns("******");
        }

        [Test]
        public void AssignSuperAnnuationPayment_NoSuperAnnuationCurrentlyExistsAndNoPaymentDtoExists_AddNewSuperAnnuationAndUseExistingPaymentDto()
        {
            var service = new SuperAnnuationPaymentOptionService(_policyPaymentRepository.Object,
                _superAnnuationPaymentRepository.Object, _taxFileNumberEncrpytionService.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>());

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>())).Returns(new PolicyPaymentDto()
            {
                PolicyId = 200,
                PaymentType = PaymentType.SuperAnnuation,
                Id = 1
            });

            _superAnnuationPaymentRepository.Setup(call => call.Insert(It.Is<SuperAnnuationPaymentDto>(cc => cc.FundUSI == "1112224444001")))
                .Returns(new SuperAnnuationPaymentDto()
                {
                    FundName = "Name",
                    FundProduct = "Product",
                    FundABN = "123456789",
                    MembershipNumber = "1112224444",
                    FundUSI = "1112224444001",
                    TaxFileNumber = "112334225",
                    PolicyPaymentId = 1
                });

            _superAnnuationPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<DirectDebitPaymentDto>(null);

            var retVal = service.AssignSuperAnnuationPayment(200, "Name", "Product", "123456789", "1112224444001", "1112224444", "112334225");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _policyPaymentRepository.Verify(call => call.Insert(It.IsAny<PolicyPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.FundUSI, Is.EqualTo("1112224444001"));
        }

        [Test]
        public void AssignSuperAnnuationPayment_NoSuperAnnuationCurrentlyExistsAndPaymentDtoExists_AddNeSuperAnnuationAndUseExistingPaymentDto()
        {
            var service = new SuperAnnuationPaymentOptionService(_policyPaymentRepository.Object,
          _superAnnuationPaymentRepository.Object, _taxFileNumberEncrpytionService.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.SuperAnnuation,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _superAnnuationPaymentRepository.Setup(call => call.Insert(It.Is<SuperAnnuationPaymentDto>(cc => cc.FundUSI == "1112224444001")))
                .Returns(new SuperAnnuationPaymentDto()
                {
                    FundName = "Name",
                    FundProduct = "Product",
                    FundABN = "123456789",
                    MembershipNumber = "1112224444",
                    FundUSI = "1112224444001",
                    TaxFileNumber = "112334225",
                    PolicyPaymentId = 1
                });

            _superAnnuationPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns<DirectDebitPaymentDto>(null);

            var retVal = service.AssignSuperAnnuationPayment(200, "Name", "Product", "123456789", "1112224444001", "1112224444", "112334225");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.FundUSI, Is.EqualTo("1112224444001"));
        }

        [Test]
        public void AssignSuperAnnuationPayment_SuperAnnuationCurrentlyExistsAndPaymentDtoExists_ReplaceSuperAnnuationAndUseExistingPaymentDto()
        {
            var service = new SuperAnnuationPaymentOptionService(_policyPaymentRepository.Object,
         _superAnnuationPaymentRepository.Object, _taxFileNumberEncrpytionService.Object);

            _policyPaymentRepository.Setup(call => call.GetForPolicy(200)).Returns(new Collection<PolicyPaymentDto>()
            {
                new PolicyPaymentDto()
                {
                    PolicyId = 200,
                    PaymentType = PaymentType.SuperAnnuation,
                    Id = 1
                }
            });

            _policyPaymentRepository.Setup(call => call.Insert(It.IsAny<PolicyPaymentDto>()))
                .Throws(new Exception("Should not be called"));

            _superAnnuationPaymentRepository.Setup(call => call.Insert(It.Is<SuperAnnuationPaymentDto>(cc => cc.FundUSI == "123456789001")))
                 .Returns(new SuperAnnuationPaymentDto()
                 {
                     FundName = "Name",
                     FundProduct = "Product",
                     FundABN = "123456789",
                     MembershipNumber = "1112224444",
                     FundUSI = "123456789001",
                     TaxFileNumber = "112334225",
                     PolicyPaymentId = 1
                 });

            _superAnnuationPaymentRepository.Setup(call => call.GetForPolicyPaymentId(1)).Returns(
                new SuperAnnuationPaymentDto()
                {
                    FundName = "Name",
                    FundProduct = "Product",
                    FundABN = "123456789",
                    MembershipNumber = "1112224444",
                    FundUSI = "1112224444001",
                    TaxFileNumber = "112334225",
                    PolicyPaymentId = 1
                });

            _superAnnuationPaymentRepository.Setup(call => call.Delete(It.IsAny<SuperAnnuationPaymentDto>())).Returns(true);

            var retVal = service.AssignSuperAnnuationPayment(200, "Name", "Product", "123456789", "123456789001", "1112224444", "112334225");

            _policyPaymentRepository.Verify(call => call.GetForPolicy(It.Is<int>(i => i == 200)));
            _superAnnuationPaymentRepository.Verify(call => call.GetForPolicyPaymentId(It.Is<int>(i => i == 1)));
            _superAnnuationPaymentRepository.Verify(call => call.Delete(It.IsAny<SuperAnnuationPaymentDto>()));

            Assert.That(retVal, Is.Not.Null);
            Assert.That(retVal.PolicyPaymentId, Is.EqualTo(1));
            Assert.That(retVal.FundUSI, Is.EqualTo("123456789001"));
        }
    }
}
